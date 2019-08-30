using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Yuusha
{
    public class Effect
    {
        public string Name
        { get; set; }
        public int Amount
        { get; set; }
        public int Duration
        { get; set; }
        public string Caster
        { get; set; }

        /// <summary>
        /// Spell and Effect icons.
        /// </summary>
        public static Dictionary<string, string> IconsDictionary = new Dictionary<string, string>()
        {
            // A
            { "Acid", "hotbuttonicon_266" }, { "Acid Orb", "hotbuttonicon_197" }, { "Acid Rain", "hotbuttonicon_247" },{ "Animal Affinity", "hotbuttonicon_463" },
            { "Ataraxia", "hotbuttonicon_221" },
            // B
            { "Barkskin", "hotbuttonicon_185" }, { "Banish", "hotbuttonicon_91" }, { "Balm", "hotbuttonicon_451" }, { "Bless", "hotbuttonicon_274" },
            { "Blessing of the Faithful", "hotbuttonicon_274" },
            { "Blind", "hotbuttonicon_483" }, { "Bonfire", "hotbuttonicon_132" }, { "Breathe Water", "hotbuttonicon_249" },
            // C
            { "Charm Animal", "hotbuttonicon_259" }, { "Close Or Open Door", "hotbuttonicon_396" }, { "Command Undead", "hotbuttonicon_70" },
            { "Concussion", "hotbuttonicon_157" }, { "Contagion", "hotbuttonicon_169" }, { "Create Illusion", "hotbuttonicon_3" }, { "Create Portal", "hotbuttonicon_306" },
            { "Create Snake", "hotbuttonicon_215" }, { "Create Web", "hotbuttonicon_386" }, { "Cure", "hotbuttonicon_468" },
            { "Curse", "hotbuttonicon_186" },
            // D
            { "Darkness", "hotbuttonicon_424" }, { "Death", "hotbuttonicon_266" }, { "Detect Undead", "hotbuttonicon_14" }, { "Disintegrate", "hotbuttonicon_206" },
            { "Dismiss Undead", "hotbuttonicon_190" }, { "Dispel Illusion", "hotbuttonicon_243" }, { "Dragon's Breath", "hotbuttonicon_137" },
            // E
            { "Ensnare", "hotbuttonicon_225" },
            // F
            { "Fear", "hotbuttonicon_433" }, { "Feather Fall", "hotbuttonicon_402" }, { "Ferocity", "hotbuttonicon_158" }, { "Find Secret Door", "hotbuttonicon_372" },
            { "Fireball", "hotbuttonicon_130" }, { "Firebolt", "hotbuttonicon_103" }, { "Firestorm", "hotbuttonicon_68" }, {"Firewall", "hotbuttonicon_100" }, { "FlameShield", "hotbuttonicon_59" }, // FlameShield can be removed after server update 7/18/2019
            { "Flame Shield", "hotbuttonicon_59" },
            // H
            { "Halt Undead", "hotbuttonicon_201" }, { "Hide Door", "hotbuttonicon_388" }, { "Hide in Shadows", "hotbuttonicon_393" }, {"Hunter's Mark", "hotbuttonicon_234"},
            // I
            { "Icespear", "hotbuttonicon_362" }, { "Icestorm", "hotbuttonicon_342" }, { "Identify", "hotbuttonicon_261" }, { "Image", "hotbuttonicon_273" },
            // L
            { "Lifeleech", "hotbuttonicon_314" }, { "Light", "hotbuttonicon_279" }, { "Lightning Bolt", "hotbuttonicon_345" }, { "Locate Entity", "hotbuttonicon_23" },
            // M
            { "Magic Missile", "hotbuttonicon_486" }, { "Make Recall", "recallring" }, { "Mark of Vitality", "hotbuttonicon_464" },
            { "Minor Protection from Fire", "hotbuttonicon_51" }, // same icon as Protection from Fire...
            { "Neutralize Poison", "hotbuttonicon_244" }, { "Night Vision", "hotbuttonicon_123" },
            // O
            { "Obfuscation", "hotbuttonicon_358" },
            // P
            { "Peek", "hotbuttonicon_251" }, { "Power Word: Silence", "hotbuttonicon_490" }, { "Protection from Acid", "hotbuttonicon_201" },
            { "Protection from Cold", "hotbuttonicon_309" }, //{ "Protection from Fire", "hotbuttonicon_51" },
            { "Protection from Fire", "hotbuttonicon_119" }, { "Protection from Fire and Ice", "hotbuttonicon_135" }, { "Protection from Poison", "hotbuttonicon_199" },
            { "Protection from Stun and Death", "hotbuttonicon_471" }, { "Protection from Undead", "hotbuttonicon_492" },
            // R
            { "Raise the Dead", "hotbuttonicon_469" }, {"Regenerate Hits", "hotbuttonicon_264" }, {"Regenerate Mana", "hotbuttonicon_221" },
            { "Regenerate Stamina", "hotbuttonicon_464" }, { "Regeneration", "hotbuttonicon_264" }, { "Resist Blind", "hotbuttonicon_147" },
            { "Resist Blindness", "hotbuttonicon_147" }, { "Resist Fear", "hotbuttonicon_448" }, { "Resistance from Blind and Fear", "hotbuttonicon_472" },
            { "Resist Lightning", "hotbuttonicon_417" }, { "Resist Zonk", "hotbuttonicon_321" },
            // S
            { "Savagery", "hotbuttonicon_28" },{ "Shelter", "hotbuttonicon_174" }, { "Shield", "hotbuttonicon_211" }, { "Speed", "hotbuttonicon_305" },
            { "Summon Hellhound", "hotbuttonicon_16" }, { "Summon Lamassu", "hotbuttonicon_407" }, { "Summon Lammasu", "hotbuttonicon_407" }, // 7/8/2019 misspelled in server logic
            { "Summon Nature's Ally", "hotbuttonicon_218" }, { "Summon Phantasm", "hotbuttonicon_280" }, { "Stoneskin", "hotbuttonicon_227" },
            { "Strength", "hotbuttonicon_146" }, { "Stun", "hotbuttonicon_217" },
            // T
            { "Temporary Charisma", "hotbuttonicon_413" }, { "Temporary Constitution", "hotbuttonicon_413" }, { "Temporary Dexterity", "hotbuttonicon_413" }, { "Temporary Intelligence", "hotbuttonicon_413" },
            { "Temporary Strength", "hotbuttonicon_146" }, { "Temporary Wisdom", "hotbuttonicon_413" }, {"Thunderwave", "hotbuttonicon_298" }, {"Transmute", "hotbuttonicon_163" },
            { "Turn Undead", "hotbuttonicon_476" },
            // V
            { "Venom", "hotbuttonicon_41" },
            // W
            { "Wall of Fog", "hotbuttonicon_423" }, { "Wizard Eye", "hotbuttonicon_46" },
        };

        public static List<string> NegativeEffects = new List<string>() { "Acid", "Contagion", "Fear", "Venom" };

        public static List<string> ShortTermPositiveEffects = new List<string>() { "Balm" };

        /// <summary>
        /// Change the tint color for hotbuttonicons. (normal tint color is white for SpellWarmingLabels and SpellEffectLabels)
        /// </summary>
        public static Dictionary<string, Color> IconsTintDictionary = new Dictionary<string, Color>()
        {
            
        };

        public Effect(string info)
        {
            string[] effectInfo = info.Split(Protocol.VSPLIT.ToCharArray());
            Name = effectInfo[0];
            Amount = Convert.ToInt32(effectInfo[1]);
            Duration = Convert.ToInt32(effectInfo[2]);
            Caster = effectInfo[3];
        }
    }
}
