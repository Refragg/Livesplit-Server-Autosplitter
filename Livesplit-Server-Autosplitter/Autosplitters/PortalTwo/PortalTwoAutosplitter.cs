using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.IO;
using LiveSplit.ComponentUtil;

namespace Livesplit_Server_Autosplitter
{
    public class PortalTwoAutosplitter
    {
	    public ASLSettings settings = new ASLSettings();
    
        private dynamic vars = new ExpandoObject();

        public PortalTwoState current = new PortalTwoState();
        public PortalTwoState old = new PortalTwoState();

        public Process proc;
    
        public double refreshRate = 1000 / 15d; //default refresh rate
        public void Init()
        {
	        vars.Init = false;
	        proc = Program.Runtime._game;
        }

        public void Startup()
        {
	        vars.SAR = new MemoryWatcherList();
	        vars.FindInterface = (Func<bool>)(() =>
	        {
		        // TimerInterface
		        var target = new SigScanTarget(16,
			        "53 41 52 5F 54 49 4D 45 52 5F 53 54 41 52 54 00", // char start[16]
			        "?? ?? ?? ??", // int total
			        "?? ?? ?? ??", // float ipt
			        "?? ?? ?? ??", // TimerAction action
			        "53 41 52 5F 54 49 4D 45 52 5F 45 4E 44 00"); // char end[14]

		        string[] lines = File.ReadAllLines($"/proc/{proc.Id}/maps");

		        string heapLine = lines.FirstOrDefault(x => x.Contains("[heap]"));

		        if (heapLine == default)
			        return false;

		        int indexOfSpace = heapLine.IndexOf(' ');
		        int indexOfDash = heapLine.IndexOf('-');

		        if (indexOfSpace == -1 || indexOfDash == -1)
			        return false;

		        string[] heapAddresses = heapLine.Substring(0, indexOfSpace).Split('-');

		        if (!IntPtr.TryParse(heapAddresses[0], NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture, out IntPtr baseHeapAddress) 
		            || !IntPtr.TryParse(heapAddresses[1], NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture, out IntPtr endHeapAddress))
			        return false;

		        int heapSize = endHeapAddress.ToInt32() - baseHeapAddress.ToInt32();

		        var result = IntPtr.Zero;
		        
		        var scanner = new SignatureScanner(proc, baseHeapAddress, heapSize);
		        result = scanner.Scan(target);

		        if (result != IntPtr.Zero)
		        {
			        Console.WriteLine("[ASL] pubInterface = 0x" + result.ToString("X"));
			        vars.Total = new MemoryWatcher<int>(result);
			        vars.Ipt = new MemoryWatcher<float>(result + sizeof(int));
			        vars.Action = new MemoryWatcher<int>(result + sizeof(int) + sizeof(float));

			        vars.SAR.Clear();
			        vars.SAR.AddRange(new MemoryWatcher[]
			        {
				        vars.Total,
				        vars.Ipt,
				        vars.Action
			        });
			        vars.SAR.UpdateAll(proc);

			        Console.WriteLine("[ASL] pubInterface->ipt = " + vars.Ipt.Current.ToString());
			        return true;
		        }

			    Console.WriteLine("[ASL] Memory scan failed!");
		        return false;
	        });

	        vars.TimerAction = new Dictionary<string, int>()
	        {
		        { "none",    0 },
		        { "start",   1 },
		        { "restart", 2 },
		        { "split",   3 },
		        { "end",     4 },
		        { "reset",   5 },
	        };
        }

        public bool Update()
        {
	        if (vars.Init)
	        {
		        vars.SAR.UpdateAll(proc);

		        vars.Init = false;
		        
		        foreach (ProcessModule module in proc.Modules)
		        {
			        if (module.ModuleName == "sar.so")
			        {
				        vars.Init = true;
				        break;
			        }
		        }
	        }
	        else
	        {
		        foreach (ProcessModule module in proc.Modules)
		        {
			        if (module.ModuleName == "sar.so")
			        {
				        vars.Init = vars.FindInterface();
				        break;
			        }
		        }
	        }

	        if (vars.Init) {
		        if (vars.Action.Changed && (vars.Action.Current == vars.TimerAction["start"] || vars.Action.Current == vars.TimerAction["restart"] || vars.Action.Current == vars.TimerAction["reset"]))
		        {
			        AutosplitterRuntime.Reset();
		        }
	        }

	        return vars.Init;
        }
        
        public TimeSpan GameTime()
        {
	        return TimeSpan.FromSeconds(vars.Total.Current * vars.Ipt.Current);
        }
        
        public bool Start()
        {
	        return vars.Action.Changed
	               && (vars.Action.Current == vars.TimerAction["start"] || vars.Action.Current == vars.TimerAction["restart"]);
        }

        public bool Split()
        {
	        return vars.Action.Changed
	               && (vars.Action.Current == vars.TimerAction["split"] || vars.Action.Current == vars.TimerAction["end"]);
        }

        public bool Reset()
        {
	        return false;
        }

        public bool IsLoading()
        {
	        return vars.Init;
        }
    }
}