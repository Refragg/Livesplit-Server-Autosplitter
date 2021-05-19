//Using GoldenTails SRB2's AppImage

using System;
using System.Diagnostics;

namespace Livesplit_Server_Autosplitter
{
    public class SRB2StateAppImage
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

        public SRB2StateAppImage RefreshValues(Process game)
        {
            IntPtr PBaseAdress = game.MainModule.BaseAddress; 
            SRB2StateAppImage clone = (SRB2StateAppImage) this.MemberwiseClone();

            Memory.Read<int>(PBaseAdress + 0x56964F8, out start, game);
            Memory.Read<int>(PBaseAdress + 0x4A0AE0, out split, game);
            Memory.Read<int>(PBaseAdress + 0x38B0C8, out level, game);
            Memory.Read<int>(PBaseAdress + 0x5672B94, out framecounter, game);
            Memory.Read<int>(PBaseAdress + 0x5672C84, out msframecounter, game);
            Memory.Read<int>(PBaseAdress + 0x494224, out mframecounter, game);
            Memory.Read<int>(PBaseAdress + 0x5672B70, out exitCountdown, game);
            Memory.Read<int>(PBaseAdress + 0x4A0B90, out TBonus, game);
            Memory.Read<int>(PBaseAdress + 0x4A0BA4, out RBonus, game);
            Memory.Read<int>(PBaseAdress + 0x4A0BB0, out LBonus, game);
            Memory.Read<int>(PBaseAdress + 0x494218, out TA, game);
            Memory.Read<int>(PBaseAdress + 0x5677090, out emerald, game);
            Memory.ReadString(PBaseAdress + 0x494240, 13, out mod_id, game);
            Memory.ReadString(PBaseAdress + 0x452A389, 8, out music, game);
            Memory.Read<int>(PBaseAdress + 0x56723E0, out isWatching, game);
            Memory.Read<int>(PBaseAdress + 0x56669E0, out isPlaying, game);
            
            return clone;
        }
    }
}