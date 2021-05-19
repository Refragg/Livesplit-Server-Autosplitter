using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Livesplit_Server_Autosplitter
{
    public enum TimerPhase
    {
        NotRunning,
        Running,
        Ended,
        Paused,
        Unknown
    }

    class AutosplitterRuntime
    {
        public static TimeSpan ParseTimeSpan(string timeString)
        {
            timeString = timeString.Replace("−", "-");

            var factor = 1;
            if (timeString.StartsWith("-"))
            {
                factor = -1;
                timeString = timeString.Substring(1);
            }

            var seconds = timeString
                .Split(':')
                .Select(x => double.Parse(x, NumberStyles.Float, CultureInfo.InvariantCulture))
                .Aggregate((a, b) => 60 * a + b);

            return TimeSpan.FromSeconds(factor * seconds);
        }

        public static TimerPhase GetTimerPhaseFromString(string tphaseString)
        {
            switch (tphaseString)
            {
                case "NotRunning":
                    return TimerPhase.NotRunning;
                case "Running":
                    return TimerPhase.Running;
                case "Ended":
                    return TimerPhase.Ended;
                case "Paused":
                    return TimerPhase.Paused;
            }
            return TimerPhase.Unknown;
        }
        
        public Process _game;

        public bool _init_completed = false;

        public bool _init_started = false;

        public TimerPhase CurrentTimerPhase;

        public TimeSpan? CurrentRTA;

        public SRB2Autosplitter Autosplitter = new ();

        public bool start = true;
        public bool split = true;
        public bool reset = true;

        private string _game_version = string.Empty;
        public string GameVersion
        {
            get { return _game_version; }
            set
            {
                _game_version = value;
            }
        }

        public void Update()
        {
            if(!_init_completed && _init_started)
                return;

            if (_game == null)
            {
                TryConnect();
            }
            else if (_game.HasExited)
            {
                DoExit();
            }
            else
            {
                if (!_init_completed)
                    DoInit();
                else
                    DoUpdate();
            }
        }

        private void TryConnect()
        {
            _game = null;

            Process process = Process.GetProcessesByName(Autosplitter.current.ProcessName).OrderByDescending(x => x.StartTime).FirstOrDefault(x => !x.HasExited);

            if (process == null)
            {
                return;
            }

            _init_completed = false;
            _game = process;
            Autosplitter.current = new SRB2StateAppImage();

            if (Autosplitter.current.GameVersion == "")
            {
                Console.WriteLine(("Connected to game: {0} (using default state descriptor)", _game.ProcessName));
            }
            else
            {
                Console.WriteLine("Connected to game: {0} (state descriptor for version '{1}' chosen as default)",
                    _game.ProcessName,
                    Autosplitter.current.GameVersion);
            }

            DoInit();
        }

        private void DoInit()
        {
            _init_started = true;
            Console.WriteLine("Initializing");

            Autosplitter.current.RefreshValues(_game);
            Autosplitter.old = Autosplitter.current;
            GameVersion = string.Empty;

            Autosplitter.Init();
            /*
            // Fetch version from init-method
            var ver = string.Empty;
            RunMethod(_methods.init, state, ref ver);

            if (ver != GameVersion)
            {
                GameVersion = ver;

                var version_state = _states.Where(kv => kv.Key.ToLower() == _game.ProcessName.ToLower())
                    .Select(kv => kv.Value)
                    .First() // states
                    .FirstOrDefault(s => s.GameVersion == ver);

                if (version_state != null)
                {
                    // This state descriptor may already be selected
                    if (version_state != Autosplitter.current)
                    {
                        Autosplitter.current = version_state;
                        Autosplitter.current.RefreshValues(_game);
                        Autosplitter.old = Autosplitter.current;
                        Console.WriteLine($"Switched to state descriptor for version '{GameVersion}'");
                    }
                }
                else
                {
                    Console.WriteLine($"No state descriptor for version '{GameVersion}' (will keep using default one)");
                }
            }
            */ //TODO: implement that stuff (game version stuff)

            _init_completed = true;
            Console.WriteLine("Init completed, running main methods");
            _init_started = false;
            // foreach (ProcessModule processModule in _game.Modules)
            // {
            //     Console.WriteLine("size: {0}, name: {1}, baseAddress: {2}", processModule.ModuleMemorySize, processModule.ModuleName, processModule.BaseAddress.ToString());
            // }
            Console.WriteLine("size: {0}, name: {1}, baseAddress: {2}", _game.MainModule.ModuleMemorySize, _game.MainModule.ModuleName, string.Format("0x{0:X}", _game.MainModule.BaseAddress.ToInt64()));
        }

        private void DoExit()
        {
            _game = null;
        }

        private void DoUpdate()
        {
            Autosplitter.old = Autosplitter.current.RefreshValues(_game);

            Program.socket.Send(SocketMessages.GetCurrentTimerPhase);
            CurrentTimerPhase = GetTimerPhaseFromString(Program.ReceiveFromServer());

            Program.socket.Send(SocketMessages.GetCurrentTime);
            CurrentRTA = ParseTimeSpan(Program.ReceiveFromServer());

            if (!Autosplitter.Update())
            {
                // If Update explicitly returns false, don't run anything else
                return;
            }

            if (CurrentTimerPhase == TimerPhase.Running || CurrentTimerPhase == TimerPhase.Paused)
            {
                if (Autosplitter.IsLoading())
                    Program.socket.Send(SocketMessages.PauseGameTime);
                else
                    Program.socket.Send(SocketMessages.UnpauseGameTime);

                TimeSpan game_time = Autosplitter.GameTime();
                Program.socket.Send(SocketMessages.SetGameTime(game_time));

                if (Autosplitter.Reset())
                {
                    if (reset)
                    {
                        Console.WriteLine("reset");
                        Program.socket.Send(SocketMessages.Reset);
                    }
                }
                else if (Autosplitter.Split())
                {
                    if (split)
                    {
                        Console.WriteLine("split");
                        Program.socket.Send(SocketMessages.Split);
                    }
                }
            }

            if (CurrentTimerPhase == TimerPhase.NotRunning)
            {
                if (Autosplitter.Start())
                {
                    if (start)
                    {
                        Console.WriteLine("start");
                        Program.socket.Send(SocketMessages.StartTimer);
                    }
                }
            }
        }

        public void DoStartup()
        {
            Autosplitter.settings = new ASLSettings();
            Autosplitter.Startup();
            Console.WriteLine("Did startup");
            Program.socket.Send(SocketMessages.InitGameTime);
            Program.socket.Send(SocketMessages.PauseGameTime);
        }
    }
}