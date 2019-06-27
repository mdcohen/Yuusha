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
            { "Bless", "hotbuttonicon_135" },
            { "Blind", "hotbuttonicon_483" },
            { "Bonfire", "hotbuttonicon_132" },
            { "Breathe Water", "hotbuttonicon_249" },
            { "Concussion", "hotbuttonicon_157" },
            { "Create Snake", "hotbuttonicon_215" },
            { "Create Web", "hotbuttonicon_386" },
            { "Cure", "hotbuttonicon_469" },
            { "Curse", "hotbuttonicon_186" },
            { "Death", "hotbuttonicon_266" },
            { "Fear", "hotbuttonicon_2" },
            { "Fireball", "hotbuttonicon_130" },
            { "Firewall", "hotbuttonicon_100" },
            { "Hide in Shadows", "hotbuttonicon_393" },
            { "Icestorm", "hotbuttonicon_342" },
            { "Improved Disguise", "hotbuttonicon_358" },
            { "Light", "hotbuttonicon_279" },
            { "Lightning Bolt", "hotbuttonicon_331" },
            { "Magic Missile", "hotbuttonicon_486" },
            { "Neutralize Poison", "hotbuttonicon_244" },
            { "Night Vision", "hotbuttonicon_123" },
            { "Power Word: Silence", "hotbuttonicon_490" },
            { "Protection from Acid", "hotbuttonicon_201" },
            { "Protection from Cold", "hotbuttonicon_309" },
            { "Protection from Fire", "hotbuttonicon_99" },
            { "Protection from Fire and Ice", "hotbuttonicon_135" },
            { "Protection from Poison", "hotbuttonicon_199" },
            { "Protection from Stun and Death", "hotbuttonicon_471" },
            { "Raise the Dead", "hotbuttonicon_432" },
            { "Resist Blind", "hotbuttonicon_147" },
            { "Resist Fear", "hotbuttonicon_448" },
            { "Resistance from Blind and Fear", "hotbuttonicon_472" },
            { "Resist Lightning", "hotbuttonicon_417" },
            { "Shelder", "hotbuttonicon_174" },
            { "Shield", "hotbuttonicon_211" },
            { "Speed", "hotbuttonicon_348" },
            { "Summon Phantasm", "hotbuttonicon_280" },
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
