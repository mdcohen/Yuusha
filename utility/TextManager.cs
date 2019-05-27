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

        public static readonly string[] MagicWords = {"aazag","alla","alsi","anaku","angarru","anghizidda","anna","annunna","ardata","ashak",
            "baad","dingir","duppira","edin","enaa","endul","enmeshir","enn","ennul","esha","gallu","gidim","gish","ia","idpa","igigi",
            "ina","isa","khitim","kia","kielgallal","kima","ku","lalartu","limutuma","lini","ma","mardukka","masqim","mass","na","naa",
            "namtar","nebo","nenlil","nergal","ninda","ningi","ninn","ninnda","ninnghizhidda","ninnme","nngi","nushi","qutri","raa","sagba",
            "shadu","shammash","shunu","shurrim","telal","uhddil","urruku","uruki","utug","utuk","utuq","ya","yu","zi","zumri","kanpa",
            "ziyilqa","luubluyi","luudnin","luuppatar","xul","ssaratu","zu","barra","kunushi","tamatunu","ega","cuthalu","egura","asaru",
            "urma","muxxisha","akki","ilani","gishtugbi","arrflug"};

        //public static Dictionary<Entity, List<string>> WEAPON_REQUIREMENT = new Dictionary<Entity, List<string>>()
        //{
        //    {Entity.Axe_Glacier_Blue_Dragon, new List<string> { "greataxe"} },
        //    {Entity.Makon, new List<string> { "menuki"} },
        //    {Entity.Ydmos, new List<string> { "swordoflight"} },
        //    {Entity.Overlord, new List<string> { "nomelee"} },
        //    {Entity.Rift_Glacier_Cloud_Dragon, new List<string> { "ulfang"} },
        //};

        public static Color GetTextFilteredColor(Enums.EGameState gameState, string textLine, bool exactMatch)
        {
            Dictionary<string, Color> chosenDictionary = null;

            switch(gameState)
            {
                case Enums.EGameState.IOKGame:
                case Enums.EGameState.SpinelGame:
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
            {"You have risen from ", Color.Orchid },
            {"You have earned enough experience for your next level! Type REST ", Color.Goldenrod },
            {"You are now a level ", Color.Gold },

            // Looking around
            {"You are looking at ", Color.LightSkyBlue },
            {"On the ground you see ", Color.LightGreen },
            {"In your pouch you see ", Color.MediumSpringGreen },
            {"In your sack you see ", Color.MediumSpringGreen },
            {"On the counter you see ", Color.MediumSpringGreen },

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
            {"You warm the spell", Color.DarkSeaGreen },
            {"You cast", Color.DarkSeaGreen },
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
    }
}
