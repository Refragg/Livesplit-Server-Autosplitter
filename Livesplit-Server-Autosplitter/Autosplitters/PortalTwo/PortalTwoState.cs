using System;
using System.Diagnostics;

namespace Livesplit_Server_Autosplitter
{
    public class PortalTwoState
    {
        public string GameVersion = ""; //FIXME: Not implemented

        public string ProcessName = "portal2_linux";

        public PortalTwoState RefreshValues(Process game)
        {
            PortalTwoState clone = (PortalTwoState) this.MemberwiseClone();
            
            return clone;
        }
    }
}