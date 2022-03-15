using System;
using System.IO;

namespace Livesplit_Server_Autosplitter
{
    public class SRB2Autosplitter
    {
        class CustomVars
        {
            //Init vars
            public int branch = 2;
            public int OSplit;
            
            //Startup vars
            public int dummy = 0;
            public int splitDelay = 0;
            public int totalTime = 0;
            public string line = "";
            public string prevLine = "";
            public int ESplit = 0;
        }
        
        public ASLSettings settings = new ASLSettings();
    
        private CustomVars vars = new CustomVars();
    
        public SRB2State current = new SRB2State();
        public SRB2State old = new SRB2State();
    
        public int refreshRate = 35;
        public void Init()
        {
            string version = "2.2.10";

            if(version == "2.2.10")
            {
                vars.branch = 2;
            }
            if(version == "")
            {
                vars.branch = 0;
            }
            
            // if(vars.branch == 0)
            // {
            //     var result = MessageBox.Show(timer.Form,
            //         "Your game version is not supported by this script version\n"
            //         + "You have to use the good version of the game\n"
            //         + "This script version works with SRB2 V2.1.25 and V2.2.8\n"
            //         + "\nClick Yes to open the game update page.",
            //         "SRB2 Livesplit Script",
            //         MessageBoxButtons.YesNo,
            //         MessageBoxIcon.Information);
            //     if (result == DialogResult.Yes)
            //     {
            //         Process.Start("https://www.srb2.org/download");
            //     }
            // }
            if(vars.branch == 0)
	            Console.WriteLine("Bad version");
            vars.OSplit = 0;
        }

        public void Startup()
        {
            settings.Builder.Add("TA_S", true, "Start on Record Attack");
            settings.Builder.Add("split", true, "Split time");
            settings.Builder.Add("finnish", false, "Finish sign", "split");
            settings.Builder.Add("a_clear", false, "Act clear appears", "split");
            settings.Builder.Add("s_b_clear", false, "Bonuses clear", "split");
            settings.Builder.Add("loading", false, "Next level Loading", "split");
            settings.Builder.Add("emerald", false, "Split on emerald tokens");
            settings.Builder.Add("emblem2", false, "Split on emblems using an external program (hover here please)");
            settings.Builder.Add("temple", false, "(2.1 only) (Mystic Realm) Temple split");
            settings.Builder.Add("sugo_WSplit", false, "(2.1 only) (SUGOI 1/2/3) Teleport Station split");
            settings.Builder.Add("igtmode", false, "(2.2 only) Marathon Mode Style IGT");
            settings.Builder.SetToolTip("split","You shouldn't choose more than 1 split timiing");
            settings.Builder.SetToolTip("finnish","Splits when you cross the finish sign");
            settings.Builder.SetToolTip("a_clear","Splits when the act clear screen appears");
            settings.Builder.SetToolTip("s_b_clear", "Splits when your bonuses got added to the total");
            settings.Builder.SetToolTip("loading","Splits when the transition to the next level begins");
            settings.Builder.SetToolTip("emblem2","Splits on hidden emblems, not on the record attack ones. You need to use Ors emblem display program and put the output to (Livesplit Path)\\Components\\SRB2 Emblems.txt in order to work");
            settings.Builder.SetToolTip("temple","Splits when activating a temple");
            settings.Builder.SetToolTip("sugo_WSplit","Splits when you warp into a level from the Teleport Station");
            settings.Builder.SetToolTip("igtmode","If unchecked, the standard Level IGT will be used");
        }

        public bool Start()
        {
            vars.dummy = 0;
            vars.OSplit = 0;
            vars.ESplit = 0;
            vars.splitDelay = 0;
            vars.totalTime = 0;
            if(current.isWatching == 0)
            {
                if(vars.branch == 2)
                {
                    if(old.mframecounter != 0 && current.mframecounter == 0)
                    {
	                    return true;
                    }
	                /*
	                if (current.start == 1 && old.start == 0)
	                {
		                return true;
	                }
                    */
                }
                /*
                if(settings.Reader["TA_S"])
                {
                    return (current.start == 1 && current.start != old.start);
                }
                else
                {
                    return (current.start == 1 && current.start != old.start && current.TA == 0);
                }
                */
            }
            return false;
        }

        public bool Update()
        {
	        //Console.WriteLine("Executable size is : " + Program.Runtime._game.MainModule.ModuleMemorySize);
	        //Console.WriteLine("vars.branch = " + vars.branch);
            if(vars.branch == 2 && settings.Reader["igtmode"])
            {
                int timeToAdd = Math.Max(0, current.msframecounter-old.msframecounter);
                if(current.level == 1)
                {
                    if(current.framecounter != 0)
                    {
                        vars.totalTime += timeToAdd;
                    }
                }
                else
                {
                    vars.totalTime += timeToAdd;
                }

                if(current.music == "_con" && old.music != "_con")
                {
                    vars.totalTime += 1;
                }

                if(current.level != 25 && current.level != 26)
                {
                    if (current.exitCountdown != 0 && old.exitCountdown == 0)
                    {
                        vars.totalTime += 1;
                    }
                }
            }
            else
            {
                int timeToAdd = Math.Max(0, current.framecounter-old.framecounter);
                if(current.framecounter-old.framecounter < 15)
                {
                    vars.totalTime += timeToAdd;
                }
            }

            if(current.split == 0 && vars.OSplit == 1)
            {
                vars.OSplit = 0;
            }

            //FIXME: Implement this
            // if (settings.Reader["emblem2"])
            // {
            //     vars.splitDelay = Math.Max(0, vars.splitDelay-1);
            //     if(File.Exists("Components\\SRB2 Emblems.txt"))
            //     {
            //         string[] lines = File.ReadAllLines("Components\\SRB2 Emblems.txt");
            //         vars.line = lines[0];
            //         if (vars.line != vars.prevLine)
            //         {
            //             vars.ESplit = 1;
            //             vars.splitDelay = 1;
            //         }
            //         vars.prevLine = vars.line;
            //         if (vars.splitDelay == 0)
            //         {
            //             vars.ESplit = 0;
            //         }
            //     }
            // }
            return true;
        }

        public bool Split()
        {
	        if (vars.branch == 1)
	        {
		        if (current.mod_id == "SUBARASHII" && current.level == 101)
		        {
			        return false;
		        }

		        if (current.mod_id == "KIMOKAWAII" && current.level == 1035)
		        {
			        return false;
		        }

		        if (settings.Reader["sugo_WSplit"])
		        {
			        if ((current.mod_id == "SUGOI V1.2" || current.mod_id == "SUBARASHII") && old.level == 100 &&
			            current.level != old.level)
			        {
				        return true;
			        }

			        if (current.mod_id == "KIMOKAWAII" && (old.level == 100 || old.level == 101) &&
			            current.level != old.level)
			        {
				        return true;
			        }
		        }

		        if (current.mod_id == "SUGOI V1.2" && current.level == 28 && current.sugoiBoss == 0 &&
		            old.sugoiBoss == 1)
		        {
			        return true;
		        }

		        if (settings.Reader["a_clear"] && current.mod_id == "KIMOKAWAII" && current.level == 52 &&
		            current.split == 1 && old.split == 0)
		        {
			        return true;
		        }

		        if (settings.Reader["temple"] && current.mod_id == "4.6" && current.scr_temple != old.scr_temple &&
		            current.scr_temple > 1)
		        {
			        return true;
		        }

		        if (current.mod_id == "" && settings.Reader["loading"] && current.level != old.level && old.level >= 50 &&
		            old.level <= 57)
		        {
			        return true;
		        }

		        if ((current.mod_id == "" && current.level == 25) ||
		            (current.mod_id == "4.6" && current.level == 122 || current.level == 134))
		        {
			        if (old.exitCountdown == 0 && current.exitCountdown != 0)
			        {
				        return true;
			        }
		        }
		        else
		        {
			        if (settings.Reader["finnish"] && old.exitCountdown == 0 && current.exitCountdown != 0)
			        {
				        return true;
			        }

			        if (settings.Reader["a_clear"] && old.exitCountdown != 1 && current.exitCountdown == 1)
			        {
				        return true;
			        }

			        if (settings.Reader["s_b_clear"] && current.split == 1 && current.TBonus == 0 && current.RBonus == 0 &&
			            vars.OSplit == 0)
			        {
				        vars.OSplit = 1;
				        return true;
			        }

			        if (settings.Reader["loading"] && current.split == 0 && old.split == 1)
			        {
				        return true;
			        }
		        }
	        }

	        if (vars.branch == 2)
	        {
		        if (current.mod_id == "CDR V1.5")
		        {
			        if (current.level == 234)
			        {
				        return old.exitCountdown != 1 && current.exitCountdown == 1;
			        }
		        }

		        if (old.level == 25 || old.level == 26 || old.level == 27)
		        {
			        if (old.exitCountdown > 1 && current.exitCountdown <= 1)
			        {
				        return true;
			        }
		        }
		        else
		        {
			        if (settings.Reader["finnish"] && old.exitCountdown == 0 && current.exitCountdown != 0)
			        {
				        return true;
			        }

			        if (settings.Reader["a_clear"] && old.exitCountdown != 1 && current.exitCountdown == 1)
			        {
				        return true;
			        }

			        if (settings.Reader["s_b_clear"] && current.split == 1 && current.TBonus == 0 && current.RBonus == 0 &&
			            vars.OSplit == 0)
			        {
				        vars.OSplit = 1;
				        return true;
			        }

			        if (settings.Reader["loading"] && current.split == 0 && (old.split == 1 || old.split == 5))
			        {
				        return true;
			        }
		        }
	        }

	        if (settings.Reader["emblem2"])
	        {
		        if (vars.ESplit == 1 && current.exitCountdown == 0)
		        {
			        vars.ESplit = 0;
			        return true;
		        }
	        }

	        if (settings.Reader["s_b_clear"] && current.LBonus == 0 && old.LBonus != 0)
	        {
		        return true;
	        }

	        if (settings.Reader["emerald"] && current.emerald > old.emerald)
	        {
		        return true;
	        }
            return false;
        }

        public bool Reset()
        {
	        if(vars.branch == 1 && current.reset == 0 && current.reset != old.reset)
	        {
		        return true;
	        }
	        if(vars.branch == 2)
	        {
		        if(current.isPlaying == 0 && current.isPlaying != old.isPlaying)
		        {
			        return true;
		        }
		        if(old.mframecounter != 0 && current.mframecounter == 0)
		        {
			        return true;
		        }
	        }
	        return false;
        }

        public TimeSpan GameTime()
        {
	        return TimeSpan.FromMilliseconds(vars.totalTime*28.5714285714);
        }

        public bool IsLoading()
        {
	        return true;
        }
    }
}