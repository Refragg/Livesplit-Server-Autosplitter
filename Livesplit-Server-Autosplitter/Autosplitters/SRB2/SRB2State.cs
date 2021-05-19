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
        public string music;
        public int isWatching;
        public int isPlaying;
        
        public string GameVersion = ""; //FIXME: Not implemented

        public string ProcessName = "lsdl2srb2";

        public SRB2State RefreshValues(Process game)
        {
            IntPtr PBaseAdress = game.MainModule.BaseAddress; 
            SRB2State clone = (SRB2State) this.MemberwiseClone();

            Memory.Read<int>(PBaseAdress + 0x725C84, out start, game);
            Memory.Read<int>(PBaseAdress + 0x5190C0, out split, game);
            Memory.Read<int>(PBaseAdress + 0x386568, out level, game);
            Memory.Read<int>(PBaseAdress + 0x506114, out framecounter, game);
            Memory.Read<int>(PBaseAdress + 0x506204, out msframecounter, game);
            Memory.Read<int>(PBaseAdress + 0x50A6A8, out mframecounter, game);
            Memory.Read<int>(PBaseAdress + 0x5060F0, out exitCountdown, game);
            Memory.Read<int>(PBaseAdress + 0x519170, out TBonus, game);
            Memory.Read<int>(PBaseAdress + 0x519184, out RBonus, game);
            Memory.Read<int>(PBaseAdress + 0x519190, out LBonus, game);
            Memory.Read<int>(PBaseAdress + 0x50A698, out TA, game);
            Memory.Read<int>(PBaseAdress + 0x503130, out emerald, game);
            Memory.ReadString(PBaseAdress + 0x50A6E0, 13, out mod_id, game);
            Memory.ReadString(PBaseAdress + 0x507B6B1, 8, out music, game);
            Memory.Read<int>(PBaseAdress + 0x4FE300, out isWatching, game);
            Memory.Read<int>(PBaseAdress + 0x4E8820, out isPlaying, game);
            
            return clone;
        }
    }
}