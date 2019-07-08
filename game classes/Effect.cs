using System;
using System.Collections.Generic;
using System.Text;

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

        public static Dictionary<string, string> IconsDictionary = new Dictionary<string, string>()
        {
            { "Animal Affinity", "hotbuttonicon_463" },
            { "Banish", "hotbuttonicon_91" },
            { "Bless", "hotbuttonicon_274" },
            { "Blessing of the Faithful", "hotbuttonicon_274" },
            { "Blind", "hotbuttonicon_483" },
            { "Bonfire", "hotbuttonicon_132" },
            { "Breathe Water", "hotbuttonicon_249" },
            { "Close Or Open Door", "hotbuttonicon_396" },
            { "Concussion", "hotbuttonicon_157" },
            { "Create Portal", "hotbuttonicon_306" },
            { "Create Snake", "hotbuttonicon_215" },
            { "Create Web", "hotbuttonicon_386" },
            { "Cure", "hotbuttonicon_469" },
            { "Curse", "hotbuttonicon_186" },
            { "Darkness", "hotbuttonicon_424" },
            { "Death", "hotbuttonicon_266" },
            { "Disintegrate", "hotbuttonicon_206" },
            { "Fear", "hotbuttonicon_2" },
            { "Feather Fall", "hotbuttonicon_402" },
            { "Find Secret Door", "hotbuttonicon_372" },
            { "Fireball", "hotbuttonicon_130" },
            { "Firewall", "hotbuttonicon_100" },
            { "FlameShield", "hotbuttonicon_59" },
            { "Flame Shield", "hotbuttonicon_59" },
            { "Hide Door", "hotbuttonicon_388" },
            { "Hide in Shadows", "hotbuttonicon_393" },
            { "Icespear", "hotbuttonicon_362" },
            { "Icestorm", "hotbuttonicon_342" },
            { "Identify", "hotbuttonicon_261" },
            { "Improved Disguise", "hotbuttonicon_358" },
            { "Lifeleech", "hotbuttonicon_314" },
            { "Light", "hotbuttonicon_279" },
            { "Lightning Bolt", "hotbuttonicon_345" },
            { "Locate Entity", "hotbuttonicon_23" },
            { "Magic Missile", "hotbuttonicon_486" },
            { "Make Recall", "recallring" },
            { "Minor Protection from Fire", "hotbuttonicon_51" }, // same icon as Protection from Fire...
            { "Neutralize Poison", "hotbuttonicon_244" },
            { "Night Vision", "hotbuttonicon_123" },
            { "Power Word: Silence", "hotbuttonicon_490" },
            { "Protection from Acid", "hotbuttonicon_201" },
            { "Protection from Cold", "hotbuttonicon_309" },
            { "Protection from Fire", "hotbuttonicon_51" },
            { "Protection from Fire and Ice", "hotbuttonicon_135" },
            { "Protection from Poison", "hotbuttonicon_199" },
            { "Protection from Stun and Death", "hotbuttonicon_471" },
            { "Protection from Undead", "hotbuttonicon_492" },
            { "Raise the Dead", "hotbuttonicon_432" },
            { "Resist Blind", "hotbuttonicon_147" },
            { "Resist Blindness", "hotbuttonicon_147" },
            { "Resist Fear", "hotbuttonicon_448" },
            { "Resistance from Blind and Fear", "hotbuttonicon_472" },
            { "Resist Lightning", "hotbuttonicon_417" },
            { "Shelter", "hotbuttonicon_174" },
            { "Shield", "hotbuttonicon_211" },
            { "Speed", "hotbuttonicon_305" },
            { "Summon Hellhound", "hotbuttonicon_16" },
            { "Summon Lamassu", "hotbuttonicon_407" },
            { "Summon Lammasu", "hotbuttonicon_407" }, // 7/8/2019 misspelled in server logic
            { "Summon Phantasm", "hotbuttonicon_280" },
            { "Strength", "hotbuttonicon_146" },
            { "Stun", "hotbuttonicon_217" },
            { "Temporary Strength", "hotbuttonicon_146" },
            { "Turn Undead", "hotbuttonicon_476" },
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
