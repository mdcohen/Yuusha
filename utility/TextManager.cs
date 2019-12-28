using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Yuusha
{
    public static class TextManager
    {
        public static string YOU_ARE_STUNNED = "You are stunned!";
        public static string YOU_HAVE_BEEN_BLINDED = "You have been blinded!";
        public static string YOU_ARE_SCARED = "You are scared!";
        public static string PORTAL_CHANT = "ashtug ninda anghizidda arrflug";
        public const string AUTO_SEND_TEXT = "^";

        //public static List<string> MinorErrors = new List<string>()
        //{
        //    "cannot quit here"
        //};

        /// <summary>
        /// Entire list of available display fonts.
        /// </summary>
        public static List<string> DisplayFontsList = new List<string>()
        {
            "astlochbold36", "butcherman32", "cevicheone32", "cinzeldecorative22", "cinzeldecorative32", "dancingscript22", "frederickathegreat32", "griffy32",
            "lobster22", "medievalsharp32", "merienda32", "mogra22", "nosifer32", "raviprakash32", "rocksalt32", "shojumaru32", "tradewinds32",
            "unicalantiqua22", "unicalantiqua32"
        };

        public static List<string> ScalingTextFontList = new List<string>()
        {
            "lemon8", "lemon9", "lemon10", "lemon12", "lemon14", "lemon16", "lemon18", "lemon20", "lemon22", "lemon24", "lemon26", "lemon28"
        };

        public static List<string> ScalingNumberFontList = new List<string>()
        {
            "changaone14", "changaone16", "changaone18", "changaone20", "changaone22", "changaone24", "changaone26", "changaone28"
        };

        public static readonly List<string> MagicWords = new List<string>() {"aazag","alla","alsi","anaku","angarru","anghizidda","anna","annunna","ardata","ashak",
            "baad","dingir","duppira","edin","enaa","endul","enmeshir","enn","ennul","esha","gallu","gidim","gish","ia","idpa","igigi",
            "ina","isa","khitim","kia","kielgallal","kima","ku","lalartu","limutuma","lini","ma","mardukka","masqim","mass","na","naa",
            "namtar","nebo","nenlil","nergal","ninda","ningi","ninn","ninnda","ninnghizhidda","ninnme","nngi","nushi","qutri","raa","sagba",
            "shadu","shammash","shunu","shurrim","telal","uhddil","urruku","uruki","utug","utuk","utuq","ya","yu","zi","zumri","kanpa",
            "ziyilqa","luubluyi","luudnin","luuppatar","xul","ssaratu","zu","barra","kunushi","tamatunu","ega","cuthalu","egura","asaru",
            "urma","muxxisha","akki","ilani","gishtugbi","arrflug"};

        public static string GenerateMagicWords(int amount)
        {
            string words = null;

            for (int a = 1; a <= amount; a++)
            {
                if (words == null)
                    words = MagicWords[new Random(Guid.NewGuid().GetHashCode()).Next(0, MagicWords.Count)];
                else
                    words += " " + MagicWords[new Random(Guid.NewGuid().GetHashCode()).Next(0, MagicWords.Count)];
            }
            return words;
        }

        public static Color GetTextFilteredColor(Enums.EGameState gameState, string textLine, bool exactMatch)
        {
            Dictionary<string, Color> chosenDictionary = null;

            switch(gameState)
            {
                case Enums.EGameState.IOKGame:
                case Enums.EGameState.SpinelGame:
                case Enums.EGameState.YuushaGame:
                    chosenDictionary = GAME_TEXT_COLOR_FILTERS;
                    break;
                case Enums.EGameState.Conference:
                    chosenDictionary = CONF_TEXT_COLOR_FILTERS;
                    break;
                default:
                    return Color.White;
            }

            foreach(string str in chosenDictionary.Keys)
            {
                if (exactMatch && textLine.Contains(str) || (!exactMatch && textLine.ToLower().Contains(str)))
                    return chosenDictionary[str];
            }

            return Color.White;
        }

        public static Dictionary<string, Color> GAME_TEXT_COLOR_FILTERS = new Dictionary<string, Color>()
        {
            // Gains
            {"You have risen from ", Color.Orchid},
            {"You have earned enough experience for your next level!", Color.PaleGoldenrod },
            {"You are now a level ", Color.Goldenrod },
            {"You have gained ", Color.Plum},

            // Looking around
            {"You are looking at ", Color.LightBlue },
            {"On the ground you see ", Color.AliceBlue },
            {"In your pouch you see ", Color.DodgerBlue },
            {"In your sack you see ", Color.CadetBlue },
            {"On the counter you see ", Color.CornflowerBlue },

            // Combat: self
            {"Your pet ", Color.Thistle },
            {" hits for ", Color.PeachPuff },
            //{" hits with ", Color.PeachPuff },
            //{"Swing hits with ", Color.PeachPuff },
            //{"Kick hits with ", Color.PeachPuff },
            //{"Jumpkick hits with ", Color.PeachPuff },
            //{"Shot hits with ", Color.PeachPuff },
            {"You have scored a critical hit", Color.Peru },

            // Combat: others
            {"misses you.", Color.LightGreen },
            {" is blocked by your ", Color.Lime },
            {" is slain!", Color.Aquamarine },
            {" hits with ", Color.Tomato },
            {"You have suffered a critical hit", Color.OrangeRed },
            {"You have been stunned by the blow!", Color.Orange },

            // Magic
            {"You warm the spell", Color.LightSeaGreen },
            {"You cast", Color.LightSeaGreen},
            {"You have been enchanted with", Color.MediumSeaGreen },
            
            // Important messages
            {" has worn off.", Color.SandyBrown },
            {"WHUMP! You are stunned!", Color.Orange },
            {"You fumble", Color.Orange },
            {"You have forgotten your memorized spell chant!", Color.Bisque },

            // Gage messages
            {" appears far more experienced than you.", Color.Crimson },
            {" appears more experienced than you.", Color.Yellow },
            {" appears as experienced as you are.", Color.Azure },
            {" appears less experienced than you.", Color.SpringGreen },
            {" appears far less experienced than you.", Color.LightGray },

        };

        public static Dictionary<string, Color> CONF_TEXT_COLOR_FILTERS = new Dictionary<string, Color>()
        {
            // Conference room header
            {"You are in Conference Room", Color.Azure },
            {"You are the only player present.", Color.Azure },
            {"players present.", Color.SlateBlue },
            {"Type /help for a list of commands.", Color.Azure },
            {"adventurers in Dragon's Spine.", Color.Azure },

            //{"has entered the room", Color.Lime}
            //{"has" }
        };

        public static string FormatEnumString(string enumString)
        {
            enumString = enumString.Replace("__", "'");
            enumString = enumString.Replace("_", " ");
            return enumString;
        }

        public static string ConvertPluralToSingular(string str)
        {
            str = str.Replace("wolves", "wolf");
            str = str.Replace("elves", "elf");
            str = str.Replace("lammasi", "lammasu");
            str = str.Replace("llizardmens", "lizardmen");

            if (str.EndsWith("s"))
                str = str.Substring(0, str.Length - 1);

            return str;
        }

        public static void CheckAllGameStateTextTriggers(string input)
        {
            if(input.StartsWith("Your name has been changed to "))
            {
                try
                {
                    string newName = input.Replace("Your name has been changed to ", "");
                    newName = newName.Substring(0, newName.Length - 1); // remove the period at the end

                    //string fileName = Utils.GetCharacterFileName(Character.CurrentCharacter.Name);
                    string dirName = Utils.StartupPath + Utils.AccountsFolder + Account.Name + "\\";

                    if(System.IO.File.Exists(dirName + Utils.GetCharacterFileName(Character.CurrentCharacter.Name)))
                        System.IO.File.Move(dirName + Utils.GetCharacterFileName(Character.CurrentCharacter.Name), dirName + Utils.GetCharacterFileName(newName));
                    if (System.IO.File.Exists(dirName + "FoW-" + Utils.GetCharacterFileName(Character.CurrentCharacter.Name)))
                        System.IO.File.Move(dirName + Utils.GetCharacterFogOfWarFileName(Character.CurrentCharacter.Name), dirName + Utils.GetCharacterFogOfWarFileName(newName));

                    if (Character.CurrentCharacter != null)
                    {
                        foreach (Character chr in Account.Characters)
                        {
                            if (chr.Name == Character.CurrentCharacter.Name) chr.Name = newName;
                        }

                        Character.CurrentCharacter.Name = newName;
                    }
                }
                catch(Exception e)
                {
                    Utils.LogException(e);
                }
            }
            else if(input.Equals("You are no longer resting."))
            {
                if (gui.GuiManager.CurrentSheet["restingPositiveStatusWindow"] is gui.SpellWarmingWindow existingWindow)
                    existingWindow.OnClose();
            }
            else if(input.Equals("You are no longer meditating."))
            {
                if (gui.GuiManager.CurrentSheet["meditatingPositiveStatusWindow"] is gui.SpellWarmingWindow existingWindow)
                    existingWindow.OnClose();
            }
            else if(input.StartsWith("Your ") && input.EndsWith(" talent has been disabled."))
            {
                string talentName = input.Replace("Your ", "").Replace(" talent has been disabled.", "");
                if(Character.CurrentCharacter != null && Character.CurrentCharacter.Talents.FindIndex(t => t.Name == talentName) is int index && index > -1)
                {
                    Character.CurrentCharacter.Talents[index].IsEnabled = false;
                }
            }
            else if (input.StartsWith("Your ") && input.EndsWith(" talent has been enabled."))
            {
                string talentName = input.Replace("Your ", "").Replace(" talent has been enabled.", "");
                if (Character.CurrentCharacter != null && Character.CurrentCharacter.Talents.FindIndex(t => t.Name == talentName) is int index && index > -1)
                {
                    Character.CurrentCharacter.Talents[index].IsEnabled = true;
                }
            }
            
        }

        public static void CheckAllGameModeTextTriggers(string input)
        {
            if (input.StartsWith("You have memorized the chant for your "))
            {
                if (Character.CurrentCharacter != null)
                    Character.CurrentCharacter.MemorizedSpell = input.Replace("You have memorized the chant for your ", "").Replace(" spell.", "");
            }
            else if (input.Equals("You have forgotten your memorized spell chant!"))
            {
                if (Character.CurrentCharacter != null) Character.CurrentCharacter.MemorizedSpell = "";
            }
            else if (input.StartsWith("As you approach the wall a large drawer slides out from what was an empty wall."))
            {
                Audio.AudioManager.PlaySoundEffect("GUISounds/locker_opening");
            }
            else if (input.Equals("You have forgotten your memorized spell chant!") || input.Equals("You are no longer hidden!") ||
                    input.Equals("You don't have any balms to quaff. Good luck.") || input.Equals("You have only 1 balm remaining."))
                    //|| input.Contains(" is immune to "))
            {
                gui.TextCue.AddClientInfoTextCue(input, Color.Yellow, Color.Transparent, GetDisplayFont(out Color tintColor), 2500);
            }
        }

        public static void CheckYuushaModeTextTriggers(string input)
        {
            try
            {
                if (input.StartsWith("You have been hit by a death spell"))
                    gui.SpellEffectLabel.CreateSpellEffectLabel("Death", gui.SpellEffectLabel.DamageBorderColor);
                else if (input.StartsWith("You have been hit by a curse spell"))
                    gui.SpellEffectLabel.CreateSpellEffectLabel("Curse", gui.SpellEffectLabel.DamageBorderColor);
                else if (input.Equals("You have been healed."))
                    gui.SpellEffectLabel.CreateSpellEffectLabel("Cure", gui.SpellEffectLabel.HealBorderColor);
                else if (input.StartsWith("You have been enchanted with "))
                {
                    string effectName = input.Replace("You have been enchanted with ", "");
                    effectName = effectName.Substring(0, effectName.Length - 1);
                    gui.SpellEffectLabel.CreateSpellEffectLabel(effectName, gui.SpellEffectLabel.BuffBorderColor);

                    if (Character.CurrentCharacter.WarmedSpell == effectName)
                    {
                        if (gui.GuiManager.GetControl("SpellWarmingWindow") is gui.SpellWarmingWindow w && w.SpellIconLabel.Name.Contains(Character.CurrentCharacter.WarmedSpell))
                        {
                            w.OnClose();
                        }
                    }
                }
                else if (input.StartsWith("You are surrounded by the blue glow of a Shield spell"))
                {
                    gui.SpellEffectLabel.CreateSpellEffectLabel("Shield", gui.SpellEffectLabel.BuffBorderColor);

                    if (Character.CurrentCharacter.WarmedSpell == "Shield")
                    {
                        if (gui.GuiManager.GetControl("SpellWarmingWindow") is gui.SpellWarmingWindow w && w.SpellIconLabel.Name.Contains(Character.CurrentCharacter.WarmedSpell))
                        {
                            w.OnClose();
                        }
                    }
                } // WARNINGS TODO: Add these to a settings serializable collection for modification in options
                else if (input.Equals("You fade into the shadows."))
                {
                    gui.SpellEffectLabel.CreateSpellEffectLabel("Hide in Shadows", Color.DarkViolet);
                }
                else if (input.Equals("You have been hit by a lightning bolt!") || input.Equals("You have been hit by a violent lightning storm!"))
                {
                    //gui.SpellEffectLabel.CreateSpellEffectLabel("Lightning Bolt", gui.SpellEffectLabel.DamageBorderColor);
                    gui.GameHUD.LightningFlash();
                }
                else if (input.ToLower().Equals("you have been hit by magic missile!"))
                    gui.SpellEffectLabel.CreateSpellEffectLabel("Magic Missile", gui.SpellEffectLabel.DamageBorderColor);
                else if (input.ToLower().Equals("you have been hit by a fireball!"))
                    gui.SpellEffectLabel.CreateSpellEffectLabel("Fireball", gui.SpellEffectLabel.DamageBorderColor);
                else if (input.ToLower().Equals("you have been hit by a raging ice storm!"))
                    gui.SpellEffectLabel.CreateSpellEffectLabel("Icestorm", gui.SpellEffectLabel.DamageBorderColor);
                else if (input.ToLower().Equals("you have been hit by icespear!"))
                    gui.SpellEffectLabel.CreateSpellEffectLabel("Icespear", gui.SpellEffectLabel.DamageBorderColor);
                else if (input.ToLower().Equals("you have been hit by concussion!"))
                    gui.SpellEffectLabel.CreateSpellEffectLabel("Concussion", gui.SpellEffectLabel.DamageBorderColor);
                else if (input.ToLower().Equals("you have been hit by disintegrate!"))
                    gui.SpellEffectLabel.CreateSpellEffectLabel("Disintegrate", gui.SpellEffectLabel.DamageBorderColor);
                else if (input.StartsWith("You have been poisoned by"))
                    gui.SpellEffectLabel.CreateSpellEffectLabel("Venom", gui.SpellEffectLabel.DamageBorderColor);
                else if (input.StartsWith("You have been poisoned"))
                    gui.SpellEffectLabel.CreateSpellEffectLabel("Poison", gui.SpellEffectLabel.DamageBorderColor);
                else if (input.StartsWith("An involuntary tremor runs up and down"))
                {
                    gui.GameHUD.RandomlyShakeScreen(2000, 2, true);
                }
                else if (input.Equals("You shiver as the poison runs its course."))
                {
                    gui.GameHUD.SynchronouslyShakeScreen(500, 2, false);
                }
                else if (input.Contains("They latch onto " + Character.CurrentCharacter.Name + " and pull"))
                    gui.SpellEffectLabel.CreateSpellEffectLabel("Ghod's Hooks", gui.SpellEffectLabel.DamageBorderColor);
                else if (input.StartsWith("Sage: "))
                {
                    if (!input.ToLower().Contains("i do not know how to do that"))
                        gui.MessageWindow.CreateSageAdviceMessageWindow(input.Replace("Sage: ", ""));
                }
                else if (input.StartsWith("You warm the spell "))
                {
                    string spellName = input.Replace("You warm the spell ", "");
                    spellName = spellName.Substring(0, spellName.Length - 1);
                    if (spellName != Character.CurrentCharacter.MemorizedSpell)
                    {
                        gui.SpellWarmingWindow.CreateSpellWarmingWindow(spellName);
                        Character.CurrentCharacter.WarmedSpell = spellName;
                    }
                }
                else if (input.StartsWith("You cast "))
                {
                    if (!string.IsNullOrEmpty(Character.CurrentCharacter.WarmedSpell) && input.Contains(Character.CurrentCharacter.WarmedSpell))
                    {
                        if (gui.GuiManager.GetControl("SpellWarmingWindow") is gui.SpellWarmingWindow w && w.SpellIconLabel.Name.Contains(Character.CurrentCharacter.WarmedSpell))
                        {
                            w.OnClose();
                        }
                    }
                }
                else if (input.StartsWith("You have lost your warmed spell") || input.Equals("Your spell fails."))
                {
                    if (gui.GuiManager.GetControl("SpellWarmingWindow") is gui.SpellWarmingWindow warmedSpellWindow &&
                        !string.IsNullOrEmpty(Character.CurrentCharacter.WarmedSpell) && warmedSpellWindow.SpellIconLabel.Name.Contains(Character.CurrentCharacter.WarmedSpell))
                    {
                        gui.GameHUD.AddRandomlyShakingControl
                            (Tuple.Create(warmedSpellWindow as gui.Control, warmedSpellWindow.Position, DateTime.Now, TimeSpan.FromMilliseconds(3000), 10));
                        warmedSpellWindow.SpellWarmed = true;
                    }
                }
                else if (input.StartsWith("You are now a level "))
                {
                    // "You are now a level " + chr.Level + " " + chr.classFullName.ToLower() + "!!"
                    string level = input.Replace("You are now a level ", "");
                    level = level.Substring(0, level.IndexOf(" "));
                    gui.AchievementLabel.CreateAchievementLabel(level, gui.AchievementLabel.AchievementType.LevelUp);
                }
                else if (input.StartsWith("You have risen from "))
                {
                    // You have risen from <old skill title> to <new skill title> in your <skill name> skill.
                    string start = input.Replace("You have risen from ", "");
                    start = start.Replace(" to ", "|");
                    start = start.Replace(" in your ", "|");
                    start = start.Replace(" skill.", "");

                    string[] s = start.Split("|".ToCharArray());
                    // 0 = old skill title, 1 = new skill title, 2 = skill
                    //string text = char.ToUpper(s[2][0]) + s[2].Substring(1) + ": " + s[1];
                    gui.AchievementLabel.CreateAchievementLabel(s[1], ScalingTextFontList[ScalingTextFontList.Count - 1], gui.GameHUD.GameIconsDictionary[s[2].ToLower()], Color.Indigo, Color.PaleGreen, "", true, Map.Direction.Southwest);
                }
                #region Coin Sounds
                else if (input.Contains(": You have been trained") && input.EndsWith(", go now and practice."))
                {
                    Audio.AudioManager.PlaySoundEffect("GUISounds/coins_training");
                }
                else if (input.StartsWith("You scoop the coins into your sack."))
                {
                    Audio.AudioManager.PlaySoundEffect("GUISounds/coins_scooped");
                }
                else if (input.EndsWith("Thank you for your business.") || (input.Contains(": You now have") && input.EndsWith("coins in your account.")))
                {
                    Audio.AudioManager.PlaySoundEffect("GUISounds/coins_sale");
                }
                else if (input.EndsWith("Thanks for the business.") || input.StartsWith("You dumped coins") || (input.StartsWith("You dumped") && input.EndsWith("coins on the ground.")))
                {
                    Audio.AudioManager.PlaySoundEffect("GUISounds/coins_dumped");
                }
                #endregion
                else if (input.Equals("A portal of chaotic energy opens up before you and you step inside.") ||
                    (input.StartsWith("You follow") && input.Contains("through the chaos portal.")))
                {
                    gui.SpellEffectLabel.CreateSpellEffectLabel("Chaos Portal");
                }
                else if (input.Equals("WHUMP! You are stunned!"))
                {
                    gui.GameHUD.SynchronouslyShakeScreen(500, 5, true); // half a second
                }
                else if (input.Equals("You have been stunned by the blow!") || input.Equals("You are stunned!"))
                {
                    gui.GameHUD.RandomlyShakeScreen(500, 5, true); // half a second
                }
                //else if(input.StartsWith("Your vision blurs and, at first, you feel dizzy.")) // reverse portal chant, return to Kesmai
                //{
                //    gui.GameHUD.GoBlack(5500, 3, true);
                //}
                else if (input.Equals("Your vision blurs for a moment."))
                {
                    gui.GameHUD.FadeToColor(Color.Black, 4000, 5, true);
                }
                
                //else if(input.StartsWith("It looks possible to climb up here"))
                //{
                //    if (input.EndsWith("with both hands."))
                //        gui.GameHUD.AtClimbUp2H = new XYCoordinate(gui.GameHUD.Cells[24].xCord, gui.GameHUD.Cells[24].yCord);
                //    else gui.GameHUD.AtClimbUp1H = new XYCoordinate(gui.GameHUD.Cells[24].xCord, gui.GameHUD.Cells[24].yCord);
                //}
                //else if(input.StartsWith("It looks possible to climb down here"))
                //{
                //    if (input.EndsWith("with both hands."))
                //        gui.GameHUD.AtClimbDown2H = new XYCoordinate(gui.GameHUD.Cells[24].xCord, gui.GameHUD.Cells[24].yCord);
                //    else gui.GameHUD.AtClimbDown1H = new XYCoordinate(gui.GameHUD.Cells[24].xCord, gui.GameHUD.Cells[24].yCord);
                //}
                // Annoying beep input...
                //else if(input.StartsWith("You don't see a -"))
                //{

                    //}
                    //else if (input.Equals("WHUMP! You are stunned!"))
                    //{
                    //    if (Character.CurrentCharacter != null)
                    //    {
                    //        Audio.AudioManager.PlaySoundEffect("ss");
                    //    }
                    //}
                    //else if (input.StartsWith("You have been slain!"))
                    //{
                    //    Audio.AudioManager.PlaySecondarySong("A_Death_Song", false, false, 1f);
                    //}
            }
            catch (Exception e)
            {
                Utils.LogException(e);
            }
        }

        public static Color GetAlignmentColor(World.Alignment alignment)
        {
            switch (alignment)
            {
                case World.Alignment.Amoral:
                    return Client.ClientSettings.Color_Gui_Amoral_Fore;
                case World.Alignment.Chaotic:
                    return Client.ClientSettings.Color_Gui_Chaotic_Back;
                case World.Alignment.ChaoticEvil:
                    return Client.ClientSettings.Color_Gui_ChaoticEvil_Back;
                case World.Alignment.Evil:
                    return Client.ClientSettings.Color_Gui_Evil_Back;
                case World.Alignment.Lawful:
                    return Client.ClientSettings.Color_Gui_Lawful_Fore;
                case World.Alignment.Neutral:
                    return Client.ClientSettings.Color_Gui_Neutral_Back;
            }

            return Color.White;
        }

        public static Color GetAlignmentColor(bool fore, World.Alignment alignment)
        {
            switch (alignment)
            {
                case World.Alignment.Amoral:
                    if(fore) return Client.ClientSettings.Color_Gui_Amoral_Fore;
                    else return Client.ClientSettings.Color_Gui_Amoral_Back;
                case World.Alignment.Chaotic:
                    if(fore) return Client.ClientSettings.Color_Gui_Chaotic_Fore;
                    else return Client.ClientSettings.Color_Gui_Chaotic_Back;
                case World.Alignment.ChaoticEvil:
                    if(fore) return Client.ClientSettings.Color_Gui_ChaoticEvil_Fore;
                    else return Client.ClientSettings.Color_Gui_ChaoticEvil_Back;
                case World.Alignment.Evil:
                    if(fore) return Client.ClientSettings.Color_Gui_Evil_Fore;
                    else return Client.ClientSettings.Color_Gui_Evil_Back;
                case World.Alignment.Lawful:
                    if(fore) return Client.ClientSettings.Color_Gui_Lawful_Fore;
                    else return Client.ClientSettings.Color_Gui_Lawful_Back;
                case World.Alignment.Neutral:
                    if(fore) return Client.ClientSettings.Color_Gui_Neutral_Fore;
                    else return Client.ClientSettings.Color_Gui_Neutral_Back;
            }

            if (fore) return Color.White;
            return Color.Black;
        }

        public static string[] GetRandomHintText()
        {
            List<string> lines = new List<string>();

            try
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(Utils.GetMediaFile("tips.txt"));
                

                while (!sr.EndOfStream)
                    lines.Add(sr.ReadLine());
                sr.Close();
            }
            catch (System.IO.FileNotFoundException)
            {
                return new string[] { "Oops", "The tips.txt file is missing from the media subdirectory." };
            }

            if (lines.Count > 0)
            {
                return lines[new Random(Guid.NewGuid().GetHashCode()).Next(lines.Count)].Split("|".ToCharArray());
            }
            else
            {
                return new string[] { "Sage Advice", "Nothing to spend your gold on? A trip to the Sage is always worth it." };
            }
        }

        public static string GetDisplayFont(out Color tintColor)
        {
            tintColor = Color.GhostWhite;
            string font = "rocksalt32"; // Island of Kesmai

            if (Character.CurrentCharacter.MapID == (int)World.MapID.Annwn)
                font = "merienda32";
            else if (Character.CurrentCharacter.MapID == (int)World.MapID.Axe_Glacier)
                font = "tradewinds32";
            else if (Character.CurrentCharacter.MapID == (int)World.MapID.Eridu)
                font = "frederickathegreat32";
            else if (Character.CurrentCharacter.MapID == (int)World.MapID.Hell)
            {
                tintColor = Color.Red;
                font = "nosifer32";
            }
            else if (Character.CurrentCharacter.MapID == (int)World.MapID.Innkadi)
                font = "cinzeldecorative32";
            else if (Character.CurrentCharacter.MapID == (int)World.MapID.Oakvael)
                font = "griffy32";
            else if (Character.CurrentCharacter.MapID == (int)World.MapID.Leng)
                font = "cevicheone32";
            else if (Character.CurrentCharacter.MapID == (int)World.MapID.Praetoseba)
                font = "butcherman32";
            else if (Character.CurrentCharacter.MapID == (int)World.MapID.Rift_Glacier)
                font = "uncialantiqua32";
            else if (Character.CurrentCharacter.MapID == (int)World.MapID.Shukumei)
                font = "raviprakash32";
            else if (Character.CurrentCharacter.MapID == (int)World.MapID.Torii)
                font = "shojumaru32";
            else if (Character.CurrentCharacter.MapID == (int)World.MapID.Underkingdom)
                font = "astlochbold36";

            return font;
            //else return DisplayFontsList[new Random(Guid.NewGuid().GetHashCode()).Next(0, DisplayFontsList.Count)];
        }

        public static string GetDisplayFont(gui.TextCue.TextCueTag tag)
        {
            switch (tag)
            {
                case gui.TextCue.TextCueTag.SkillUp:
                    return "lemon28";
            }

            return GetDisplayFont(out Color tintColor);
        }

        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("ARGH!");
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }
}
