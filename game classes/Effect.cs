using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Yuusha
{
    public class Effect
    {
        /// <summary>
        /// Use display graphic vice cell graphic for map.
        /// </summary>
        public static List<string> NonDisplayableCellEffects = new List<string>()
        {
            "Find Secret Door", "Find Secret Rockwall", "Illusion", "Unlocked Horizontal Door", "Unlocked Vertical Door",
        };

        public static List<string> IgnoreCellEffectsAbsence = new List<string>()
        {
            "Black Fog", "Illusion",
        };

        public string Name
        { get; set; }
        public int Amount
        { get; set; }
        public int Duration
        { get; set; }
        public string Caster
        { get; set; }

        /// <summary>
        /// Spell and Effect icons. (Character Effects)
        /// </summary>
        public static Dictionary<string, string> IconsDictionary = new Dictionary<string, string>()
        {
            // temporary until it's renamed Sacred Ring in server update
            { "Knight Ring", "hotbuttonicon_465" },

            // A
            { "Acid", "hotbuttonicon_266" }, { "Acid Orb", "hotbuttonicon_197" }, { "Acid Rain", "hotbuttonicon_247" }, { "Animal Affinity", "hotbuttonicon_463" },
            { "Animate Dead", "hotbuttonicon_410" }, { "Ataraxia", "hotbuttonicon_390" },
            // B
            { "Barkskin", "hotbuttonicon_185" }, { "Banish", "hotbuttonicon_91" }, { "Balm", "hotbuttonicon_451" }, { "Bazymon's Bounty", "hotbuttonicon_278" },
            { "Bless", "hotbuttonicon_274" }, { "Blessing of the Faithful", "hotbuttonicon_274" },
            { "Blind", "hotbuttonicon_483" }, { "Bonfire", "hotbuttonicon_156" }, { "Breathe Water", "hotbuttonicon_249" },
            // C
            { "Chaos Portal", "hotbuttonicon_432" }, { "Charm Animal", "hotbuttonicon_259" }, { "Close Or Open Door", "hotbuttonicon_396" },
            { "Cognoscere", "hotbuttonicon_178" }, { "Command Undead", "hotbuttonicon_70" },
            { "Concussion", "hotbuttonicon_157" }, { "Contagion", "hotbuttonicon_169" }, { "Create Illusion", "hotbuttonicon_3" }, { "Create Portal", "hotbuttonicon_306" },
            { "Create Lava", "hotbuttonicon_26" }, { "Create Snake", "hotbuttonicon_215" }, { "Create Web", "hotbuttonicon_386" }, { "Cure", "hotbuttonicon_468" },
            { "Curse", "hotbuttonicon_186" }, { "Cynosure", "hotbuttonicon_297" }, 
            // D
            { "Darkness", "hotbuttonicon_424" }, { "Death", "hotbuttonicon_266" }, { "Detect Undead", "hotbuttonicon_14" }, { "Disintegrate", "hotbuttonicon_206" },
            { "Dismiss Undead", "hotbuttonicon_190" }, { "Dispel Illusion", "hotbuttonicon_243" }, { "Dragon's Breath", "hotbuttonicon_137" }, {"Drudgery", "hotbuttonicon_458" },
            // E
            { "Ensnare", "hotbuttonicon_225" },
            // F
            { "Faerie Fire", "hotbuttonicon_24" },{ "Fear", "hotbuttonicon_433" }, { "Feather Fall", "hotbuttonicon_402" }, { "Ferocity", "hotbuttonicon_158" }, { "Find Secret Door", "hotbuttonicon_372" },
            { "Fireball", "hotbuttonicon_130" }, { "Firebolt", "hotbuttonicon_103" }, { "Firestorm", "hotbuttonicon_68" }, {"Firewall", "hotbuttonicon_100" },
            { "Flame Shield", "hotbuttonicon_59" },
            // G
            { "Gnostikos", "hotbuttonicon_240" }, { "Ghod's Hooks", "hotbuttonicon_319" },
            // H
            { "Halt Undead", "hotbuttonicon_201" }, { "Heal Servant", "hotbuttonicon_477" }, { "Hide Door", "hotbuttonicon_388" }, { "Hide in Shadows", "hotbuttonicon_393" }, {"Hunter's Mark", "hotbuttonicon_234"},
            // I
            { "Iceshard", "hotbuttonicon_398" }, { "Icespear", "hotbuttonicon_362" }, { "Icestorm", "hotbuttonicon_342" }, { "Identify", "hotbuttonicon_261" }, { "Image", "hotbuttonicon_273" },
            // J
            { "Juvenis", "hotbuttonicon_177" },
            // K
            // L
            { "Lagniappe", "hotbuttonicon_484" }, { "Lifeleech", "hotbuttonicon_314" }, { "Light", "hotbuttonicon_343" }, { "Lightning Bolt", "hotbuttonicon_345" }, { "Lightning Storm", "hotbuttonicon_327" },
            { "Locate Entity", "hotbuttonicon_23" }, { "Locust Swarm", "hotbuttonicon_195" },
            // M
            { "Magic Missile", "hotbuttonicon_486" }, { "Make Recall", "recallring" }, { "Mark of Vitality", "hotbuttonicon_464" },
            { "Minor Protection from Fire", "hotbuttonicon_51" }, // same icon as Protection from Fire...
            // N
            { "Neutralize Poison", "hotbuttonicon_244" }, { "Night Vision", "hotbuttonicon_123" },
            // O
            { "Obfuscation", "hotbuttonicon_358" },
            // P
            { "Peek", "hotbuttonicon_251" }, {"Poison", "hotbuttonicon_192" }, {"Poison Cloud", "hotbuttonicon_238" }, { "Power Word: Silence", "hotbuttonicon_490" },
            { "Protection from Acid", "hotbuttonicon_224" }, { "Protection from Cold", "hotbuttonicon_309" }, { "Protection from Fire", "hotbuttonicon_119" },
            { "Protection from Fire and Ice", "hotbuttonicon_135" }, { "Protection from Hellspawn", "hotbuttonicon_93" }, { "Protection from Poison", "hotbuttonicon_199" }, { "Protection from Stun and Death", "hotbuttonicon_471" },
            { "Protection from Undead", "hotbuttonicon_492" },
            // Q NONE
            // R
            { "Raise the Dead", "hotbuttonicon_469" }, {"Regenerate Health", "hotbuttonicon_264" }, {"Regenerate Mana", "hotbuttonicon_221" },
            { "Regenerate Stamina", "hotbuttonicon_464" }, { "Regeneration", "hotbuttonicon_264" }, { "Resist Blind", "hotbuttonicon_147" },
            { "Resist Blindness", "hotbuttonicon_147" }, { "Resist Fear", "hotbuttonicon_448" }, { "Resistance from Blind and Fear", "hotbuttonicon_472" },
            { "Resist Lightning", "hotbuttonicon_417" }, { "Resist Zonk", "hotbuttonicon_321" },
            // S
            { "Sacred Ring", "hotbuttonicon_465" }, { "Savagery", "hotbuttonicon_28" },{ "Shelter", "hotbuttonicon_174" }, { "Shield", "hotbuttonicon_211" }, { "Speed", "hotbuttonicon_305" },
            { "Summon Demon", "hotbuttonicon_32" }, { "Summon Hellhound", "hotbuttonicon_16" }, { "Summon Humanoid", "hotbuttonicon_235" }, { "Summon Lamassu", "hotbuttonicon_407" }, { "Summon Lammasu", "hotbuttonicon_407" }, // 7/8/2019 misspelled in server logic
            { "Summon Nature's Ally", "hotbuttonicon_176" }, { "Summon Phantasm", "hotbuttonicon_280" }, { "Stoneskin", "hotbuttonicon_227" },
            { "Strength", "hotbuttonicon_146" }, { "Stun", "hotbuttonicon_217" },
            { "Silence", "hotbuttonicon_490" }, // same as Power Word: Silence
            // T
            { "Tempest", "hotbuttonicon_356" }, { "Temporary Charisma", "hotbuttonicon_413" }, { "Temporary Constitution", "hotbuttonicon_413" }, { "Temporary Dexterity", "hotbuttonicon_413" }, { "Temporary Intelligence", "hotbuttonicon_413" },
            { "Temporary Strength", "hotbuttonicon_146" }, { "Temporary Wisdom", "hotbuttonicon_413" }, {"The Withering", "hotbuttonicon_22" },
            { "Thunderwave", "hotbuttonicon_494" }, {"Transmute", "hotbuttonicon_163" }, { "Trochilidae", "hotbuttonicon_221" }, { "Turn Undead", "hotbuttonicon_476" },
            // U NONE
            { "Umbral Form", "hotbuttonicon_418" },
            // V
            { "Venom", "hotbuttonicon_41" },
            // W
            { "Wall of Fog", "hotbuttonicon_423" }, { "Whirlwind", "hotbuttonicon_365" }, { "Wizard Eye", "hotbuttonicon_46" },
            // X Y Z NONE
        };

        /// <summary>
        /// Cell effects. Non animated. Tuple.Item1 = visualKey or animation name, Item2 = TintColor, Item3 = visualAlpha, animatedVisual
        /// </summary>
        public static Dictionary<string, Tuple<string, Color, int, bool>> CellEffectsDictionary = new Dictionary<string, Tuple<string, Color, int, bool>>()
        {
            {"Acid", Tuple.Create("hotbuttonicon_247", Color.White, 150, false) }, //
            {"Blizzard", Tuple.Create("hotbuttonicon_354", Color.White, 230, false) }, //
            {"Concussion", Tuple.Create("ExplosionAnimation", Color.White, 255, true) }, // animation
            {"Darkness", Tuple.Create("hotbuttonicon_423", Color.Black, 25, false) },
            {"Dragon's Breath Acid", Tuple.Create("hotbuttonicon_239", Color.Green, 230, false) },
            {"Dragon's Breath Fire", Tuple.Create("hotbuttonicon_139", Color.White, 230, false) },
            {"Dragon's Breath Ice", Tuple.Create("hotbuttonicon_298", Color.White, 230, false) }, //
            {"Dragon's Breath Poison", Tuple.Create("hotbuttonicon_238", Color.White, 230, false) },
            {"Dragon's Breath Storm", Tuple.Create("hotbuttonicon_296", Color.White, 230, false) }, //
            {"Dragon's Breath Wind", Tuple.Create("hotbuttonicon_365", Color.White, 230, false) }, //
            {"Find Secret Door", Tuple.Create("sptile_opendoor1", Color.White, 245, false) },
            {"Find Secret Rockwall", Tuple.Create("sptile_secret_rockwall", Color.White, 245, false) },
            {"Fire", Tuple.Create("CartoonFlames", Color.White, 200, true) }, // animation
            //{"Fire", Tuple.Create("SimpleFlames", Color.White, 255, true) }, // animation
            //{"Fire", Tuple.Create("sptile_fire", Color.White, 200, false) },
            {"Fireball", Tuple.Create("Fireball2Animation", Color.White, 250, true) }, // animation
            {"Fire Storm", Tuple.Create("hotbuttonicon_68", Color.White, 255, false) },
            {"Fog", Tuple.Create("hotbuttonicon_423", Color.White, 255, false) },
            {"Hide Door", Tuple.Create("sptile_wall3", Color.White, 255, false) },
            {"Ice", Tuple.Create("IcestormAnimation", Color.White, 255, true) }, // animation
            {"Lava", Tuple.Create("hotbuttonicon_26", Color.White, 255, false) },
            {"Light", Tuple.Create("hotbuttonicon_343", Color.White, 120, false) },
            {"Lightning Storm", Tuple.Create("hotbuttonicon_327", Color.White, 150, false) },
            //{"Locust Swarm", Tuple.Create("ThickSmoke", Color.White, 255, true) }, // animation
            {"Locust Swarm", Tuple.Create("hotbuttonicon_195", Color.White, 200, false) },
            {"Poison Cloud", Tuple.Create("WhirlwindAnimation", Color.Green, 200, true) }, // animation
            {"Ornic Flame", Tuple.Create("SimpleFlames", Color.BlueViolet, 255, true) }, // animation
            {"Tempest", Tuple.Create("hotbuttonicon_356", Color.White, 150, false) },
            {"Thunderwave", Tuple.Create("hotbuttonicon_494", Color.White, 100, false) },
            {"Turn Undead", Tuple.Create("hotbuttonicon_476", Color.White, 100, false) },
            {"Unknown", Tuple.Create("unknown", Color.White, 150, false) },
            {"Unlocked Horizontal Door", Tuple.Create("sptile_opendoor1", Color.White, 255, false) },
            {"Unlocked Vertical Door", Tuple.Create("sptile_opendoor1", Color.White, 255, false) },
            {"Web", Tuple.Create("hotbuttonicon_386", Color.White, 125, false) },
            {"Wall of Fog", Tuple.Create("hotbuttonicon_423", Color.White, 255, false) },
            {"Whirlwind", Tuple.Create("WhirlwindAnimation", Color.Khaki, 210, true) }, // animation
        };

        #region Animations
        public static List<string> NonLoopingAnimations = new List<string>
        { "Concussion", "Fireball" };

        public static List<string> RandomStartFrameAnimation = new List<string>
        {"Fire", "Ornic Flame" };

        public static List<string> AddAnimatedSmokePost = new List<string>
        { "Fire" };

        public static List<string> AddAnimatedSmokePrior = new List<string>
        { "Ornic Flame", "Whirlwind" }; 
        #endregion

        public static List<string> NegativeEffects = new List<string>
        { "Acid", "Contagion", "Cynosure", "Drudgery", "Faerie Fire", "Fear", "Poison", "Silence", "The Withering", "Venom" };

        public static List<string> ShortTermPositiveEffects = new List<string>() { "Balm" };

        /// <summary>
        /// Change the tint color for hotbuttonicons. (normal tint color is white for SpellWarmingLabels and SpellEffectLabels)
        /// </summary>
        public static Dictionary<string, Color> IconsTintDictionary = new Dictionary<string, Color>()
        {
            { "Stoneskin", Color.DarkMagenta }
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
