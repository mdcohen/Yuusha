using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Yuusha
{
    public static class TextManager
    {
        public static string YOU_ARE_STUNNED = "You are stunned!";
        public static string YOU_HAVE_BEEN_BLINDED = "You have been blinded!";
        public static string YOU_ARE_SCARED = "You are scared!";

        public static List<string> DisplayFontsList = new List<string>()
        {
            "simonetta20", "unicalantiqua20", "lobster20", "mogra20", "dancingscript20",
        };

        public static readonly string[] MagicWords = {"aazag","alla","alsi","anaku","angarru","anghizidda","anna","annunna","ardata","ashak",
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
                    words = MagicWords[new Random(Guid.NewGuid().GetHashCode()).Next(0, MagicWords.Length)];
                else
                    words += " " + MagicWords[new Random(Guid.NewGuid().GetHashCode()).Next(0, MagicWords.Length)];
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
            {"You have earned enough experience for your next level! Type REST ", Color.LightGoldenrodYellow },
            {"You are now a level ", Color.Gold },
            {"You have gained ", Color.Orchid},

            // Looking around
            {"You are looking at ", Color.LightBlue },
            {"On the ground you see ", Color.AliceBlue },
            {"In your pouch you see ", Color.DodgerBlue },
            {"In your sack you see ", Color.CadetBlue },
            {"On the counter you see ", Color.CornflowerBlue },

            // Combat: self
            {"Swing hits with ", Color.PeachPuff },
            {"Kick hits with ", Color.PeachPuff },
            {"Jumpkick hits with ", Color.PeachPuff },
            {"Shot hits with ", Color.PeachPuff },

            // Combat: others
            {"misses you.", Color.MintCream },
            {" is blocked by your ", Color.Lime },
            {" is slain!", Color.Aquamarine },
            {" hits with ", Color.Tomato },

            // Magic
            {"You warm the spell", Color.LightSeaGreen },
            {"You cast", Color.SeaGreen},
            {"You have been enchanted with", Color.MediumSeaGreen },
            
            // Important messages
            {" has worn off.", Color.SandyBrown },
            {"WHUMP! You are stunned!", Color.Red },
            {"You fumble", Color.Red },
        };

        public static Dictionary<string, Color> CONF_TEXT_COLOR_FILTERS = new Dictionary<string, Color>()
        {
            // Conference room header
            {"You are in Conference Room", Color.White },
            {"You are the only player present.", Color.White },
            {"players present.", Color.SlateBlue },
            {"Type /help for a list of commands.", Color.White },
            {"adventurers in Dragon's Spine.", Color.White }
        };

        public static string ConvertPluralToSingular(string str)
        {
            str = str.Replace("elves", "elf");
            str = str.Replace("lammasi", "lammasu");

            if (str.EndsWith("s"))
                str = str.Substring(0, str.Length - 1);

            return str;
        }

        public static void CheckTextTriggers(string input)
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
            }
            else if (input.Equals("You fade into the shadows."))
                gui.SpellEffectLabel.CreateSpellEffectLabel("Hide in Shadows", Color.DarkViolet);
            else if (input.Equals("You have been hit by a lightning bolt!"))
                gui.SpellEffectLabel.CreateSpellEffectLabel("Lightning Bolt");
            else if (input.ToLower().Equals("you have been hit by magic missile!"))
                gui.SpellEffectLabel.CreateSpellEffectLabel("Magic Missile", gui.SpellEffectLabel.DamageBorderColor);
            else if (input.ToLower().Equals("you have been hit by a fireball!"))
                gui.SpellEffectLabel.CreateSpellEffectLabel("Fireball");
            else if (input.ToLower().Equals("you have been hit by a raging ice storm!"))
                gui.SpellEffectLabel.CreateSpellEffectLabel("Icestorm");
            else if (input.ToLower().Equals("you have been hit by icespear!"))
                gui.SpellEffectLabel.CreateSpellEffectLabel("Icespear", gui.SpellEffectLabel.DamageBorderColor);
            else if (input.ToLower().Equals("you have been hit by concussion!"))
                gui.SpellEffectLabel.CreateSpellEffectLabel("Concussion");
            else if (input.ToLower().Equals("you have been hit by disintegrate!"))
                gui.SpellEffectLabel.CreateSpellEffectLabel("Disintegrate");
            else if (input.StartsWith("Sage: "))
                gui.TipWindow.CreateSageAdviceHintWindow(input.Replace("Sage: ", ""));
            else if(input.StartsWith("You warm the spell "))
            {
                string spellName = input.Replace("You warm the spell ", "");
                spellName = spellName.Substring(0, spellName.Length - 1);

                //if(World.SpellsList.Find(spell => spell.Name == spellName) != null)
                    gui.SpellWarmingWindow.CreateSpellWarmingWindow(spellName);
            }
            else if(input.StartsWith("You cast "))
            {

            }
            else if(input.StartsWith("locker description"))
            {
                // cell is lockers
            }
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

        public static string GetDisplayFont()
        {
            if (Character.CurrentCharacter.MapName == "Torii" || Character.CurrentCharacter.MapName == "Shukumei")
                return "shojumaru22";
            else return "rocksalt22";
            //else return "uncialantiqua22";
            //else return DisplayFontsList[new Random(Guid.NewGuid().GetHashCode()).Next(0, DisplayFontsList.Count)];
        }
    }
}
