using System;
using System.Collections.Generic;

namespace Yuusha
{
    public class Talent
    {
        public static Dictionary<string, string> IconsDictionary = new Dictionary<string, string>()
        {

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
            //m_downTime = 
            m_soundFile = talentInfo[8];
        }
    }
}
