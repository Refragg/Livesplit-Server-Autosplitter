using System;
using System.Diagnostics;

namespace Livesplit_Server_Autosplitter
{
    public class SADXState
    {
        public int globalFrameCount;
        public int inCutscene;
        public byte gameStatus;
        public byte gameMode;
        public int inPrerenderedMovie;
        public byte currCharacter;
        public byte lastStory;
        public byte timerStart;
        public byte demoPlaying;
        public byte bossHealth;
        public byte level;
        public byte levelCopy;
        public byte act;
        public byte lives;
        public byte e101Flag;
        public byte adventureData;
        public byte rmFlag;
        public byte mk2Value;
        public byte WalkerValue;

        public byte AngelCutscene;
        public byte selectedCharacter;
        public byte music;
        public byte lwFlag;

        public byte[] emblemBytes = new byte[17];
        public byte[] eventBytes = new byte[353];

        public byte lightshoes;
        public byte crystalring;
        public byte ancientlight;
        public byte jetanklet;
        public byte rhythmbadge;
        public byte shovelclaw;
        public byte fightinggloves;
        public byte warriorfeather;
        public byte longhammer;
        public byte lifebelt;
        public byte powerrod;
        public byte lureec;
        public byte lureic;
        public byte luress;
        public byte luremr;
        public byte jetbooster;
        public byte laserblaster;
        
        public string GameVersion = ""; //FIXME: Not implemented

        //using this instead of process.MainModule.BaseAdress which seems to use
        //an other module than the one we want (probably because of wine)
        public IntPtr PBaseAdress = (IntPtr)0x00400000;

        public string ProcessName = "sonic.exe";

        public SADXState RefreshValues(Process game)
        {
            SADXState clone = (SADXState)this.MemberwiseClone();
            
            Memory.Read<int>(PBaseAdress + 0x0372C6C8, out globalFrameCount, game);
            Memory.Read<int>(PBaseAdress + 0x0372C55C, out inCutscene, game);
            Memory.Read<byte>(PBaseAdress + 0x03722DE4, out gameStatus, game);
            Memory.Read<byte>(PBaseAdress + 0x036BDC7C, out gameMode, game);
            Memory.Read<int>(PBaseAdress + 0x0368B0D0, out inPrerenderedMovie, game);
            Memory.Read<byte>(PBaseAdress + 0x03722DC0, out currCharacter, game);
            Memory.Read<byte>(PBaseAdress + 0x03718DB4, out lastStory, game);
            Memory.Read<byte>(PBaseAdress + 0x00512DF0, out timerStart, game);
            Memory.Read<byte>(PBaseAdress + 0x0372A2E4, out demoPlaying, game);
            Memory.Read<byte>(PBaseAdress + 0x03858150, out bossHealth, game);
            Memory.Read<byte>(PBaseAdress + 0x03722DCC, out level, game);
            Memory.Read<byte>(PBaseAdress + 0x03722DC4, out levelCopy, game);
            Memory.Read<byte>(PBaseAdress + 0x03722DEC, out act, game);
            Memory.Read<byte>(PBaseAdress + 0x370EF34, out lives, game);
            Memory.Read<byte>(PBaseAdress + 0x037189A8, out e101Flag, game);
            Memory.Read<byte>(PBaseAdress + 0x037183F0, out adventureData, game);
            Memory.Read<byte>(PBaseAdress + 0x037189A5, out rmFlag, game);
            Memory.Read<byte>(PBaseAdress + 0x0386C760, out mk2Value, game);
            Memory.Read<byte>(PBaseAdress + 0x0385A7ED, out WalkerValue, game);

            Memory.Read<byte>(PBaseAdress + 0x03883D08, out AngelCutscene, game);
            Memory.Read<byte>(PBaseAdress + 0x0372A2FD, out selectedCharacter, game);
            Memory.Read<byte>(PBaseAdress + 0x0512698, out music, game);
            Memory.Read<byte>(PBaseAdress + 0x0371892A, out lwFlag, game);

            Memory.ReadArray(PBaseAdress + 0x0372B5E8, 17, out emblemBytes, game);
            Memory.ReadArray(PBaseAdress + 0x03718888, 353, out eventBytes, game);

            Memory.Read<byte>(PBaseAdress + 0x3718895, out lightshoes, game);
            Memory.Read<byte>(PBaseAdress + 0x3718896, out crystalring, game);
            Memory.Read<byte>(PBaseAdress + 0x37188A7, out ancientlight, game);
            Memory.Read<byte>(PBaseAdress + 0x37188D5, out jetanklet, game);
            Memory.Read<byte>(PBaseAdress + 0x37188E3, out rhythmbadge, game);
            Memory.Read<byte>(PBaseAdress + 0x3718921, out shovelclaw, game);
            Memory.Read<byte>(PBaseAdress + 0x3718922, out fightinggloves, game);
            Memory.Read<byte>(PBaseAdress + 0x371895A, out warriorfeather, game);
            Memory.Read<byte>(PBaseAdress + 0x3718966, out longhammer, game);
            Memory.Read<byte>(PBaseAdress + 0x37189D8, out lifebelt, game);
            Memory.Read<byte>(PBaseAdress + 0x37189D9, out powerrod, game);
            Memory.Read<byte>(PBaseAdress + 0x37189E6, out lureec, game);
            Memory.Read<byte>(PBaseAdress + 0x37189E7, out lureic, game);
            Memory.Read<byte>(PBaseAdress + 0x37189E8, out luress, game);
            Memory.Read<byte>(PBaseAdress + 0x37189E9, out luremr, game);
            Memory.Read<byte>(PBaseAdress + 0x3718991, out jetbooster, game);
            Memory.Read<byte>(PBaseAdress + 0x3718992, out laserblaster, game);

            return clone;
        }
    }
}