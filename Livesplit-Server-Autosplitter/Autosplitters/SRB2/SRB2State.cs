//Using the executable in the repo (should work on debian based systems (compiled on Linux Mint 20.04))

using System;
using System.Diagnostics;

namespace Livesplit_Server_Autosplitter
{
    public class SRB2State
    {
        //2.1 specific variables
        public int reset = 0;
        public byte scr_temple = 0;
        public int sugoiBoss = 0;
        
        public int start;
        public int split;
        public int level;
        public int framecounter;
        public int msframecounter;
        public int mframecounter;
        public int exitCountdown;
        public int TBonus;
        public int RBonus;
        public int LBonus;
        public int TA;
        public int emerald;
        public string mod_id;
        public string music = ""; //FIXME: find the address for this again?
        public int isWatching;
        public int isPlaying;
        
        public string GameVersion = ""; //FIXME: Not implemented

        public string ProcessName = "lsdl2srb2";

        public SRB2State RefreshValues(Process game)
        {
            IntPtr PBaseAdress = game.MainModule.BaseAddress - 0x7D000; //big hack because the base address seems wrong question mark?
            SRB2State clone = (SRB2State) this.MemberwiseClone();

            Memory.Read<int>(PBaseAdress + 0x4a3b38, out start, game);
            Memory.Read<int>(PBaseAdress + 0x6747c0, out split, game);
            Memory.Read<int>(PBaseAdress + 0x3bfbce, out level, game);
            Memory.Read<int>(PBaseAdress + 0x6611fc, out framecounter, game);
            Memory.Read<int>(PBaseAdress + 0x661310, out msframecounter, game);
            Memory.Read<int>(PBaseAdress + 0x665d88, out mframecounter, game);
            Memory.Read<int>(PBaseAdress + 0x6611d8, out exitCountdown, game);
            Memory.Read<int>(PBaseAdress + 0x674870, out TBonus, game);
            Memory.Read<int>(PBaseAdress + 0x674884, out RBonus, game);
            Memory.Read<int>(PBaseAdress + 0x674890, out LBonus, game);
            Memory.Read<int>(PBaseAdress + 0x665d78, out TA, game);
            Memory.Read<int>(PBaseAdress + 0x65e210, out emerald, game);
            Memory.ReadString(PBaseAdress + 0x665dc0, 13, out mod_id, game);
            //Memory.ReadString(PBaseAdress + 0x507B6B1, 8, out music, game); //refer to above
            Memory.Read<int>(PBaseAdress + 0x3bab40, out isWatching, game);
            Memory.Read<int>(PBaseAdress + 0x619020, out isPlaying, game);
            
            return clone;
        }
    }
}