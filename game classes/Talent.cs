using System;
using System.Collections.Generic;

namespace Yuusha
{
    public class Talent
    {
        public static Dictionary<string, string> IconsDictionary = new Dictionary<string, string>()
        {
            {"Assassinate", "talent_assassinate" },
            {"Backstab", "talent_backstab" },
            {"Battle Charge", "talent_battlecharge" },
            {"Blind Fighting", "talent_blindfighting" },
            {"Cleave", "talent_cleave" },
            {"DaggerStorm", "talent_daggerstorm" },
            {"Double Attack", "talent_doubleattack" },
            {"Dual Wield", "talent_dualwield" },
            {"Flying Fury", "talent_flyingfury" },
            {"Gage", "talent_gage" },
            {"Leg Sweep", "talent_legsweep" },
            {"Memorize", "talent_memorize" },
            {"Snoop", "talent_snoop" },
            {"Pick Locks", "talent_picklocks" },
            {"Rapid Kicks", "talent_rapidkicks" },
            {"Riposte", "talent_riposte" },
            {"Roundhouse Kick", "talent_roundhousekick" },
            {"Shield Bash", "talent_shieldbash" },
            {"Steal", "talent_steal" },

            {"Peek", "talent_snoop" }, // temporary until server update 10/14/2019
        };

        /// <summary>
        /// Holds the string command for the talent.
        /// </summary>
        private readonly string m_command;

        /// <summary>
        /// Holds the formal name of the talent.
        /// </summary>
        private readonly string m_name;

        /// <summary>
        /// Holds the description of the talent.
        /// </summary>
        private readonly string m_description;

        /// <summary>
        /// Holds the minimum experience level a Character must have to use the talent.
        /// </summary>
        private readonly int m_minimumLevel;

        /// <summary>
        /// Holds whether the GameTalent is passive (not activated).
        /// </summary>
        private readonly bool m_passive;

        /// <summary>
        /// Holds the stamina cost of the talent. If this is less than 0 then this GameTalent is passive, otherwise it must be activated.
        /// </summary>
        private readonly int m_performanceCost;

        /// <summary>
        /// Holds the initial purchase price of the talent.
        /// </summary>
        private readonly int m_purchasePrice;

        /// <summary>
        /// Holds the amount of time before the Talent may be used again.
        /// </summary>
        private readonly TimeSpan m_downTime;

        /// <summary>
        /// Holds whether this GameTalent is available to learn at generic mentors.
        /// </summary>
        private readonly bool m_availableAtMentor;

        private readonly string m_soundFile;

        public string Command
        {
            get { return m_command; }
        }

        public string Name
        {
            get { return m_name; }
        }

        public bool IsPassive
        {
            get { return m_passive; }
        }

        public bool IsEnabled
        { get; set; }

        public DateTime LastUse
        { get; set; }

        public TimeSpan DownTime
        { get { return m_downTime; } }

        public Talent(string info)
        {
            string[] talentInfo = info.Split(Protocol.VSPLIT.ToCharArray());

            m_command = talentInfo[0];
            m_name = talentInfo[1];
            m_description = talentInfo[2];
            m_minimumLevel = Convert.ToInt32(talentInfo[3]);
            m_performanceCost = Convert.ToInt32(talentInfo[4]);
            m_purchasePrice = Convert.ToInt32(talentInfo[5]);
            m_passive = Convert.ToBoolean(talentInfo[6]);
            m_availableAtMentor = Convert.ToBoolean(talentInfo[7]);
            m_soundFile = talentInfo[8];
            if (talentInfo.Length > 9)
            {
                m_downTime = Utils.RoundsToTimeSpan(Convert.ToInt32(talentInfo[9]));
            }

            IsEnabled = true; // always default from server
            LastUse = DateTime.UtcNow;
        }

        public static List<string> TalentsRequiringTargets = new List<string>()
        {
            "Assassinate",
            "Backstab",
            "Battle Charge",
            "Cleave",
            "DaggerStorm",
            "Flying Fury",
            "Gage",
            "Leg Sweep",
            "Peek",
            "Rapid Kicks",
            "Roundhouse Kick",
            "Shield Bash",
            "Steal",
        };

        public static List<string> TalentsRequiringInput = new List<string>()
        {
            "Memorize", "Pick Locks",
        };

        public static bool RequiresTarget(string talentName)
        {
            return TalentsRequiringTargets.Contains(talentName);
        }

        public static bool RequiresTextInput(string talentName)
        {
            return TalentsRequiringInput.Contains(talentName);
        }
    }
}
