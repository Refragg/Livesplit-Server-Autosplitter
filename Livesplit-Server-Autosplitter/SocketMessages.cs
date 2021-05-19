using System;
using System.Text;
using System.Linq;

namespace Livesplit_Server_Autosplitter
{
    public static class SocketMessages
    {
        public static readonly byte[] StartTimer = GetMessageBytes("starttimer\r\n");
        public static readonly byte[] StartOrSplit = GetMessageBytes("startorsplit\r\n");

        public static readonly byte[] Split = GetMessageBytes("split\r\n");
        public static readonly byte[] Unsplit = GetMessageBytes("unsplit\r\n");
        public static readonly byte[] SkipSplit = GetMessageBytes("skipsplit\r\n");

        public static readonly byte[] Pause = GetMessageBytes("pause\r\n");
        public static readonly byte[] Resume = GetMessageBytes("resume\r\n");

        public static readonly byte[] Reset = GetMessageBytes("reset\r\n");

        public static readonly byte[] InitGameTime = GetMessageBytes("initgametime\r\n");

        public static readonly byte[] PauseGameTime = GetMessageBytes("pausegametime\r\n");
        public static readonly byte[] UnpauseGameTime = GetMessageBytes("unpausegametime\r\n");
        
        public static readonly byte[] GetCurrentTime = GetMessageBytes("getcurrentrealtime\r\n");

        public static readonly byte[] GetCurrentTimerPhase = GetMessageBytes("getcurrenttimerphase\r\n");

        public static byte[] GetMessageBytes(string message) //FIXME: make private
        {
            return UTF8Encoding.UTF8.GetBytes(message);
        }

        public static byte[] SetGameTime(string time)
        {
            return GetMessageBytes("setgametime " + time + "\r\n");
        }

        public static byte[] SetGameTime(TimeSpan time)
        {
            return SetGameTime(time.ToString(@"hh\:mm\:ss\.fff"));
        }
    }

}