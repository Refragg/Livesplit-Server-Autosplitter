using System;
using System.Collections.Generic;

namespace Livesplit_Server_Autosplitter
{
    public class SADXAutosplitter
    {
        class CustomVars
        {
            //Startup vars
            public int totalFrameCount = 0;
            public TimeSpan totalCutsceneRTA = new TimeSpan(0); //For pre-rendered cutscene rta and pausing
            public TimeSpan previousRTA = new TimeSpan(0);
            public int creditsCounter = -1;
            public bool bossStarted = false;
            public int delay = 0;
            public byte[] splitMask = new byte[353];

            //Start vars
            public List<int> bossArray = new List<int>();
            public List<int> ignoredStages = new List<int>();
            public List<int> finalBosses = new List<int>();

        }
        
        public ASLSettings settings = new ASLSettings();

        private CustomVars vars = new CustomVars();

        public SADXState current = new SADXState();
        public SADXState old = new SADXState();

        public int refreshRate = 63;

        public void Init() { }
        
        public void Startup()
        {
            settings.Builder.Add("reset",false,"Auto Reset on Main Menu");
            settings.Builder.Add("charSplit", false, "Splits when entering a story");
            
            settings.Builder.Add("stages",true,"Stages NOT To Split");
                settings.Builder.Add("36",true,"Sky Chace Act 1","stages");
                settings.Builder.Add("37",true,"Sky Chase Act 2","stages");
            
            settings.Builder.Add("bosses",true,"Final Boss To Split");
                settings.Builder.Add("22",true,"Egg Viper","bosses");//final 
                settings.Builder.Add("21",true,"Egg Walker","bosses");//final
                settings.Builder.Add("18",true,"Chaos 6","bosses");//final and not final
                settings.Builder.Add("23",true,"ZERO","bosses");//final
                settings.Builder.Add("25",true,"E-101 'Beta' mkII","bosses");//final
                settings.Builder.Add("19",true,"Perfect Chaos","bosses");//final
                
                
            settings.Builder.Add("sonicBoss", true, "Sonic Bosses");
                settings.Builder.Add("zeroSonic", true, "Chaos 0", "sonicBoss");
                settings.Builder.Add("eggHornetSonic", true, "Egg Hornet", "sonicBoss");
                settings.Builder.Add("knuxSonic", true, "Knuckles", "sonicBoss");
                settings.Builder.Add("fourSonic", true, "Chaos 4", "sonicBoss");
                settings.Builder.Add("gammaSonic", true, "Gamma", "sonicBoss");
                settings.Builder.Add("sixSonic", true, "Chaos 6", "sonicBoss");

            settings.Builder.Add("tailsBoss", true, "Tails Bosses");
                settings.Builder.Add("eggHornetTails", true, "Egg Hornet", "tailsBoss");
                settings.Builder.Add("knuxTails", true, "Knuckles", "tailsBoss");
                settings.Builder.Add("fourTails", true, "Chaos 4", "tailsBoss");
                settings.Builder.Add("gammaTails", true, "Gamma", "tailsBoss");	
                    
            settings.Builder.Add("knucklesBoss", true, "Knuckles Bosses");
                settings.Builder.Add("twoKnuckles", true, "Chaos 2", "knucklesBoss");
                settings.Builder.Add("sonicKnuckles", true, "Sonic", "knucklesBoss");
                settings.Builder.Add("fourKnuckles", true, "Chaos 4", "knucklesBoss");	
                
            settings.Builder.Add("gammaBoss", true, "Gamma Bosses");
                settings.Builder.CurrentDefaultParent = "gammaBoss";
                settings.Builder.Add("E101", false, "E101 BETA");
                settings.Builder.Add("sonicGamma", true, "Sonic", "gammaBoss");
                settings.Builder.CurrentDefaultParent = null;

            settings.Builder.Add("globalMisc", false, "Miscellaneous");
                settings.Builder.CurrentDefaultParent = "globalMisc";
                settings.Builder.Add("actSplit", false, "Split between the Acts (might not work for characters other than sonic)");
                settings.Builder.CurrentDefaultParent = null;

            settings.Builder.Add("MisceSonic", false, "Sonic Miscellaneous");
                settings.Builder.CurrentDefaultParent = "MisceSonic";
                settings.Builder.Add("EnterSewerSonic", false, "Enter Sewers");
                settings.Builder.Add("EnterCASHubSonic", false, "Enter Casino Area (To Red Mountain)");
                settings.Builder.Add("shipTransform", false, "Ship Transformation");
                settings.Builder.Add("backFromPast", false, "Splits when going back to the present after Lost World");
                settings.Builder.Add("EnterSS", false, "Enter Station Square from the Train Station");
                settings.Builder.Add("EnterMR", false, "Enter Mystic Ruins from the Train Station");
                settings.Builder.CurrentDefaultParent = null;
                    
            settings.Builder.Add("MisceKnuckles", false, "Knuckles Miscellaneous");
            settings.Builder.CurrentDefaultParent = "MisceKnuckles";
                settings.Builder.Add("deathMRKnux", false, "Death Warp Mystic Ruin");
                settings.Builder.Add("EnterEGCarrierKnux", false, "Enter Egg Carrier (Before Sky Deck)");
                settings.Builder.CurrentDefaultParent = null;	
                    
            settings.Builder.Add("MisceAmy", false, "Amy Miscellaneous");
                settings.Builder.CurrentDefaultParent = "MisceAmy";
                settings.Builder.Add("EnterCASHubAmy", false, "Enter Casino Area");
                settings.Builder.Add("EnterJungleToFEAmy", false, "Enter Jungle to Final Egg");
                settings.Builder.Add("ExitJungleToECAmy", false, "Exit Jungle from Final Egg");
                settings.Builder.Add("EnterEGCarrierAmy", false, "Enter Egg Carrier (Before Zero)");
                settings.Builder.CurrentDefaultParent = null;	
                
            settings.Builder.Add("MisceGamma", false, "Gamma Miscellaneous");
                settings.Builder.CurrentDefaultParent = "MisceGamma";
                settings.Builder.Add("deathWV", false, "Death Mystic Ruin (Windy Valley Flag)");
                settings.Builder.Add("deathMRGamma", false, "Death Warp (Post Red Mountain)");
                settings.Builder.Add("eggCarrierMK2", false, "Enter Egg Carrier (Before MKII)");
                settings.Builder.CurrentDefaultParent = null;	
                    
            settings.Builder.Add("MisceSS", false, "Super Sonic Miscellaneous");
                settings.Builder.CurrentDefaultParent = "MisceSS";
                settings.Builder.Add("EnterCave", false, "Enter Cave");
                settings.Builder.Add("AngelIsland", false, "Angel Island Cutscene");
                settings.Builder.Add("Tikal", false, "Tikal Cutscene");
                settings.Builder.Add("deathSS", false, "Death Warp post Tikal Cutscene");
                settings.Builder.Add("jungleSS", false, "Enter Jungle");
                settings.Builder.CurrentDefaultParent = null;	
                
            settings.Builder.Add("Boss_Enter", false, "Enter Boss");
                settings.Builder.CurrentDefaultParent = "Boss_Enter";
                settings.Builder.Add("EnterEggHornet", false, "Enter Egg Hornet");
                settings.Builder.Add("EnterChaos2", false, "Enter Chaos 2");
                settings.Builder.Add("EnterChaos4", false, "Enter Chaos 4");
                settings.Builder.Add("EnterChaos6", false, "Enter Chaos 6");
                settings.Builder.Add("EnterBeta1", false, "Enter E-101 'Beta' mkI");
                settings.Builder.Add("EnterBeta2", false, "Enter E-101 'Beta' mkII");
                settings.Builder.CurrentDefaultParent = null;

            settings.Builder.Add("S_Enter", false, "Enter Stage");
                settings.Builder.CurrentDefaultParent = "S_Enter";
                settings.Builder.Add("EEC", false, "Enter Emerald Coast");
                settings.Builder.Add("EWV", false, "Enter Windy Valley");
                settings.Builder.Add("EC", false, "Enter Casino");
                settings.Builder.Add("EIC", false, "Enter Ice Cap");
                settings.Builder.Add("ETP", false, "Enter Twinkle Park");
                settings.Builder.Add("ESH", false, "Enter Speed Highway");
                settings.Builder.Add("ERM", false, "Enter Red Mountain");
                settings.Builder.Add("ESD", false, "Enter Sky Deck");
                settings.Builder.Add("ELW", false, "Enter Lost World");
                settings.Builder.Add("EFE", false, "Enter Final Egg");
                settings.Builder.Add("EHS", false, "Enter Hot Shelter");
                settings.Builder.CurrentDefaultParent = null;		
                    
            settings.Builder.Add("S_Power", false, "Upgrades");
                settings.Builder.CurrentDefaultParent = "S_Power";
                settings.Builder.Add("Sonic_Powerup1", false, "Light Speed Shoes");
                settings.Builder.Add("Sonic_Powerup2", false, "Crystal Ring");
                settings.Builder.Add("Sonic_Powerup3", false, "Ancient Light");
                settings.Builder.Add("Tails_Powerup1", false, "Jet Anklet");
                settings.Builder.Add("Tails_Powerup2", false, "Rhythm Badge");
                settings.Builder.Add("Knux_Powerup1", false, "Shovel Claw");
                settings.Builder.Add("Knux_Powerup2", false, "Fighting Gloves");
                settings.Builder.Add("Amy_Powerup1", false, "Warrior Feather");
                settings.Builder.Add("Amy_Powerup2", false, "Long Hammer");
                settings.Builder.Add("Big_Powerup1", false, "Power Rod");
                settings.Builder.Add("Big_Powerup2", false, "Life Belt");
                settings.Builder.Add("Big_Powerup3", false, "Lure");
                settings.Builder.Add("Gamma_Powerup1", false, "Jet Booster");
                settings.Builder.Add("Gamma_Powerup2", false, "Laser Blaster");
                settings.Builder.CurrentDefaultParent = null;	
        }

        public bool Start()
        {
            vars.totalFrameCount = 0;
            vars.totalCutsceneRTA = new TimeSpan(0); //For pre-rendered cutscene rta and pausing
            vars.previousRTA = new TimeSpan(0);
            vars.creditsCounter = -1;
            vars.delay = 0;

            vars.bossStarted = false;
            vars.bossArray = new List<int>();
            vars.ignoredStages = new List<int>();
            vars.finalBosses = new List<int>();

            // Splits for final bosses  http://info.sonicretro.org/SCHG:Sonic_Adventure/Level_List
            if (settings.Reader["bosses"]){
                if(settings.Reader["18"])
                    vars.bossArray.Add(18);
                if(settings.Reader["22"])
                    vars.bossArray.Add(22);
            }

            // Ignore splits for stages if selected
            if(settings.Reader["stages"]){
                if(settings.Reader["36"])
                    vars.ignoredStages.Add(36);
                if(settings.Reader["37"])
                    vars.ignoredStages.Add(37);
            }

            // tempMask to be assigned to splitMask at the end.
            byte[] tempMask = new byte[vars.splitMask.Length];	// One byte per event

            // Other boss splits
            if (settings.Reader["sonicBoss"])
            {
                if (settings.Reader["gammaSonic"])
                    tempMask[36] = 1;
                if (settings.Reader["knuxSonic"])
                    tempMask[37] = 1;
                if (settings.Reader["zeroSonic"])
                    tempMask[48] = 1;
                if (settings.Reader["fourSonic"])
                    tempMask[49] = 1;
                if (settings.Reader["sixSonic"])
                    tempMask[50] = 1;
                if (settings.Reader["eggHornetSonic"])
                    tempMask[51] = 1;
            }
            if (settings.Reader["tailsBoss"])
            {
                if (settings.Reader["gammaTails"])
                    tempMask[95] = 1;
                if (settings.Reader["knuxTails"])
                    tempMask[96] = 1;
                if (settings.Reader["fourTails"])
                    tempMask[103] = 1;
                if (settings.Reader["eggHornetTails"])
                    tempMask[105] = 1;
            }
            if (settings.Reader["knucklesBoss"])
            {
                if (settings.Reader["sonicKnuckles"])
                    tempMask[158] = 1;
                if (settings.Reader["twoKnuckles"])
                    tempMask[163] = 1;
                if (settings.Reader["fourKnuckles"])
                    tempMask[165] = 1;
            }
            if (settings.Reader["gammaBoss"])
            {
                if (settings.Reader["sonicGamma"])
                    tempMask[282] = 1;
            }

            vars.splitMask = tempMask;


            // Split on fade-to-black on story screen
            if (current.demoPlaying != 1 && old.gameStatus == 21 && (current.gameMode != 12 && current.gameMode != 20) && (old.gameMode == 12 || old.gameMode == 20))
                return true;
            return false;
        }

        public bool Update()
        {
            TimeSpan? rawRTA = Program.Runtime.CurrentRTA;
            TimeSpan currentRTA = new TimeSpan(0);
            if (rawRTA.HasValue)
            {
                currentRTA = new TimeSpan(0).Add(rawRTA.Value);
            }
            
            /* Add RTA to the IGT when the frame counter
                does not increase */
            if (current.inPrerenderedMovie != 0 ||
                current.gameMode == 1 || //Splash logos
                current.gameMode == 12 || //Title + menus
                current.gameMode == 18 || //Story introduction screen
                current.gameMode == 20 || //Instruction
                current.gameStatus == 19 || //Game over screen
                current.gameStatus == 16) //Pause
            {
                vars.totalCutsceneRTA = vars.totalCutsceneRTA.Add(currentRTA-vars.previousRTA);
            }
            
            if (current.gameMode == 22) //Credits
            {
                if (old.gameMode != 22)
                {
                    /* Arbitrary delay before frames are added during credits,
                        so that categories that skip the credits don't get
                        the frames added */
                    vars.creditsCounter = 3600;
                }
                
                vars.creditsCounter -= 1;
                
                if (vars.creditsCounter == 0)
                {
                    int character = current.currCharacter;
                    switch (character)
                    {
                        case 0: 
                            if (current.lastStory == 0)
                                vars.totalFrameCount += 16804; //Sonic credits + emblem frame count
                            else
                                vars.totalFrameCount += 17360; //Super Sonic credits + emblem frame count
                            break;
                        case 2: vars.totalFrameCount += 14657; break;//Tails credits + emblem frame count
                        case 3: vars.totalFrameCount += 16804; break;//Knuckles credits + emblem frame count
                        case 5: vars.totalFrameCount += 18950; break;//Amy credits + emblem frame count
                        case 6: vars.totalFrameCount += 17545; break;//Gamma credits + emblem frame count
                        case 7: vars.totalFrameCount += 15333; break;//Big credits + emblem frame count
                        default: break;
                    }
                }
            }
            
            int deltaFrames = current.globalFrameCount - old.globalFrameCount;
            
            /* Add twice the amount of time when in a cutscene */
            if (current.inCutscene != 0)
            {
                deltaFrames*=2;
            }
            
            vars.totalFrameCount+=deltaFrames;
            
            vars.previousRTA = new TimeSpan(currentRTA.Ticks);
            return true;
        }

        public TimeSpan GameTime()
        {
            return TimeSpan.FromSeconds((vars.totalFrameCount)/60.0)+vars.totalCutsceneRTA;
        }

        public bool IsLoading()
        {
            return true;
        }

        public bool Reset()
        {
            if (settings.Reader["reset"])
		        if(current.gameMode == 12 && old.currCharacter != 6 && current.levelCopy != 32)
			        return true;
            return false;
        }

        public bool Split()
        {
            if (settings.Reader["charSplit"] && current.demoPlaying != 1 && old.gameStatus == 21 && (current.gameMode != 12 && current.gameMode != 20) && (old.gameMode == 12 || old.gameMode == 20))
                return true;

            //Ignore Stages
            if(vars.ignoredStages.Contains(current.level))
                return false;
            
            // Event split code
            for (int i = 0; i < vars.splitMask.Length; i++)
            {
                if (vars.splitMask[i] == 1 && current.gameStatus != 21 &&
                    (current.eventBytes[i] != old.eventBytes[i]))
                {
                    System.Console.WriteLine("Event Split");
                    return true;
                }
            }
            
            //Big ending
            if(current.inCutscene != 0 && old.inCutscene == 0 && current.level == 29 && current.currCharacter == 7)
                return true;

            //E-101 split
            if (settings.Reader["gammaBoss"])
            {
                if (settings.Reader["E101"] && current.currCharacter == 6 && current.e101Flag > old.e101Flag) 
                {return true;}
            }
            
            if (settings.Reader["bosses"])
            {
                //MKII split
                if (current.level == 25 && current.mk2Value == 255)
                {
                    if (settings.Reader["25"] && current.bossHealth == 0 && current.timerStart == 0 && old.timerStart == 1)
                    {return true;}
                }
                //Perfect Chaos split
                if (current.level == 19)
                {
                    if (settings.Reader["19"] && current.bossHealth == 0 && old.bossHealth == 2)
                    {return true;}
                }

                //Zero split
                if (current.level == 23)
                {
                    if (settings.Reader["23"] && current.bossHealth == 0 && old.bossHealth == 1)
                    {return true;}
                }
                
                //Egg Walker split
                if (current.level == 21 && current.WalkerValue == 9)
                {
                    if (settings.Reader["21"] && current.bossHealth == 0 && current.timerStart == 0 && old.timerStart == 1)
                    {return true;}	
                }
                
            }


            //Boss Split timing
            if (vars.bossArray.Contains(current.level))
            {
                // Prevents split at beginning of boss
                if(current.bossHealth == 0 && current.timerStart == 0)
                {
                    vars.bossStarted = true;
                }
                // Boss beaten checks
                if(vars.bossStarted && current.inCutscene == 0 && old.inCutscene == 0
                    && current.bossHealth == 0 
                    && current.gameStatus == 15 && current.timerStart == 0 && old.timerStart == 1)
                {
                    if(current.level == 18 && current.currCharacter == 0)
                        return false;
                    else
                        return true;
                }
            }


            //if (current.emblemBytes != old.emblemBytes)
            //	return true;
            byte[] curB = new byte[1];
            curB = current.emblemBytes;
            byte[] oldB = new byte[1];
            oldB = old.emblemBytes;
            
            //Upgrades split
            
            if (settings.Reader["S_Power"] && current.level != 0)
            {
                if (settings.Reader["Sonic_Powerup1"] && current.lightshoes > old.lightshoes)	{return true;}
                else if (settings.Reader["Sonic_Powerup2"] && current.crystalring > old.crystalring)	{return true;}
                else if (settings.Reader["Sonic_Powerup3"] && current.ancientlight > old.ancientlight)	{return true;}
                else if (settings.Reader["Tails_Powerup1"] && current.jetanklet > old.jetanklet)	{return true;}
                else if (settings.Reader["Tails_Powerup2"] && current.rhythmbadge > old.rhythmbadge) 	{return true;}
                else if (settings.Reader["Knux_Powerup1"] && current.shovelclaw > old.shovelclaw)	{return true;}
                else if (settings.Reader["Knux_Powerup2"] && current.fightinggloves > old.fightinggloves)	{return true;}
                else if (settings.Reader["Amy_Powerup1"] && current.warriorfeather > old.warriorfeather) 	{return true;}
                else if (settings.Reader["Amy_Powerup2"] && current.longhammer > old.longhammer)	{return true;}
                else if (settings.Reader["Big_Powerup1"] && current.powerrod > old.powerrod)	{return true;}
                else if (settings.Reader["Big_Powerup2"] && current.lifebelt > old.lifebelt)	{return true;}
                else if (settings.Reader["Big_Powerup3"] && (current.luress > old.luress || current.lureic != old.lureic || current.lureec != old.lureec || current.luremr != old.luremr))	{return true;}
                else if (settings.Reader["Gamma_Powerup1"] && current.jetbooster > old.jetbooster) 	{return true;}
                else if (settings.Reader["Gamma_Powerup2"] && current.laserblaster > old.laserblaster) {return true;}
            }

            // Enter bosses splits

            if (settings.Reader["Boss_Enter"])
            {
                if (settings.Reader["EnterEggHornet"] && current.level == 20 && old.level == 33) {return true;}
                else if (settings.Reader["EnterChaos2"] && current.level == 16 && old.level == 26) {return true;}
                else if (settings.Reader["EnterChaos4"] && current.level == 17 && old.level == 33) {return true;}
                else if (settings.Reader["EnterChaos6"] && current.level == 18 && old.level == 29) {return true;}
                else if (settings.Reader["EnterBeta1"] && current.level == 24 && old.level == 33) {return true;}
                else if (settings.Reader["EnterBeta2"] && current.level == 25 && old.level == 29) {return true;}
            }
            
            // enter level split
            
            if (settings.Reader["S_Enter"])
            {
                if (settings.Reader["EEC"] && current.level == 01 && old.level == 26)	{return true;}
                else if (settings.Reader["EWV"] && current.level == 02 && old.level == 33)	{return true;}
                else if (settings.Reader["EC"] && current.level == 09 && old.level == 26)	{return true;}
                else if (settings.Reader["EIC"] && current.level == 08 && old.level == 33)	{return true;}
                else if (settings.Reader["ETP"] && current.level == 03 && old.level == 26)	{return true;}
                else if (settings.Reader["ESH"] && current.level == 04 && old.level == 26)	{return true;}
                else if (settings.Reader["ERM"] && current.level == 05 && old.level == 33)	{return true;}
                else if (settings.Reader["ESD"] && current.level == 06 && old.level == 29)	{return true;}
                else if (settings.Reader["ELW"] && current.level == 07 && old.level == 33)	{return true;}
                else if (settings.Reader["EFE"] && current.level == 10 && old.level == 33)	{return true;}
                else if (settings.Reader["EHS"] && current.level == 12 && old.level == 32)	{return true;}
            }
            
            //Miscellaneous splits
            
            //Global misc
            if(settings.Reader["globalMisc"])
            {
                if(settings.Reader["actSplit"] && current.level != 0 && current.level != 15 && current.level != 22 && current.level != 26 && current.level != 29 && current.level != 32 && current.level != 33 && current.level != 37)
                {
                    if (current.act != old.act) {return true;}
                }
            }
            
            //Sonic
            if (settings.Reader["MisceSonic"])
            {
                if (current.level == 26 && (current.act == 2 && old.act == 0))
                {
                    if (settings.Reader["EnterSewerSonic"] && current.currCharacter == 0) {return true;}
                }

                if (current.level == 26 && ((curB[8] & 0x8) >> 3) == 1 && ((curB[8] & 0x10) >> 4) == 0)
                {
                    if (current.act == 1 && (old.act == 3 || old.act == 4)) 
                    {
                        if (settings.Reader["EnterCASHubSonic"] && current.currCharacter == 0) {return true;}
                    }
                }
                
                if(settings.Reader["shipTransform"] && current.level == 29 && current.act == 6 && old.act == 3)
                {
                    return true;
                }
                
                if(settings.Reader["backFromPast"] && current.level == 33 && current.act == 2 && old.level == 34 && old.act == 2)
                {
                    return true;
                }
                
                if(settings.Reader["EnterSS"] && current.level == 26 && current.act == 1 && old.level == 33 && old.act == 0)
                {
                    return true;
                }
                
                if(settings.Reader["EnterMR"] && current.level == 33 && current.act == 0 && old.level == 26 && old.act == 1)
                {
                    return true;
                }
            }

            //Gamma
            if (settings.Reader["MisceGamma"])
            {				 
                if (current.level == 33 && current.act == 0) //the condition is split like this, because otherwise too many "&&" make the autosplitter not working for some reason... 
                {
                    if (settings.Reader["deathWV"] && current.adventureData != old.adventureData && current.currCharacter == 6) {return true;}
                }
                
                if (current.level == 33 && current.act == 1) //the condition is split like this, because otherwise too many "&&" make the autosplitter not working for some reason... 
                {
                    if (settings.Reader["deathMRGamma"] && current.lives<old.lives && current.currCharacter == 6 && current.adventureData == 2) {return true;}
                }
                
                if (current.level == 29 && old.level == 33 && current.rmFlag == 1)
                {
                    if (settings.Reader["eggCarrierMK2"] && current.currCharacter == 6) {return true;} 
                }
            }
            
            //Knuckles
            if (settings.Reader["MisceKnuckles"])
            {				
                if (current.level == 33 && current.act == 1)
                {
                    if (settings.Reader["deathMRKnux"] && current.lwFlag == 1 && current.lives<old.lives && current.currCharacter == 3) {return true;}
                }
                
                    if (current.level == 29 && old.level == 33)
                {
                    if (settings.Reader["EnterEGCarrierKnux"] && current.currCharacter == 3) {return true;}
                }
            }
            
            //Amy
            if (settings.Reader["MisceAmy"])
            {				
                if (current.level == 29 && old.level == 33)
                {
                    if (settings.Reader["EnterEGCarrierAmy"] && current.currCharacter == 5) {return true;}
                }

                if (((curB[0xA] & 0x10) >> 4) == 0 && current.level == 26 && (current.act == 1 && old.act == 3)) 
                {
                    if (settings.Reader["EnterCASHubAmy"] && current.currCharacter == 5) {return true;}
                }

                if (current.level == 33 && (current.act == 2 && old.act == 0))
                {
                    if (settings.Reader["EnterJungleToFEAmy"] && current.currCharacter == 5) {return true;}
                }

                if (current.level == 33 && (current.act == 0 && old.act == 2))
                {
                    if (settings.Reader["ExitJungleToECAmy"] && current.currCharacter == 5) {return true;}
                }
            }
                
            //Super Sonic
            if (settings.Reader["MisceSS"] && current.selectedCharacter == 6)
            {
                if (settings.Reader["EnterCave"] && current.level == 33 && current.act == 1 && old.act == 0) {return true;}
                if (settings.Reader["AngelIsland"] && current.level == 33 && current.act == 1 && current.music == 40 && old.music != 40) {return true;}
                if (settings.Reader["Tikal"] && current.level == 33 && old.level == 34) {return true;} // leaving the past
                if (settings.Reader["deathSS"] && current.level == 33 && current.act == 1 && current.lives<old.lives) {return true;} // death warp
                if (settings.Reader["jungleSS"] && current.level == 33 && old.level != 0 && current.act == 2 && old.act == 0) {return true;} // enter jungle
            }
            
            for (int i = 0; i < curB.Length; i++)
            {
                // Do not check during menus
                if (current.gameMode != 12 && (curB[i] != oldB[i]))
                {
                    if(current.level < 15)  //the action stages
                    {
                        System.Console.WriteLine("Going to split soon");
                        vars.delay = current.globalFrameCount + 190;
                    }
                    else 
                    {
                        System.Console.WriteLine("Field Emblem Split");
                        return true;
                    }
                }
            }
            
            if(Math.Abs(vars.delay - current.globalFrameCount) <= 2 && vars.delay > 0)
            {
                vars.delay = 0;
                System.Console.WriteLine("Action Emblem Split");
                return true;
            }
            return false;
        }
    }
}