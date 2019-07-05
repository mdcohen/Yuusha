using System;
using System.Collections.Generic;
using System.Text;

namespace Yuusha
{
    public class Spell
    {
        public enum SpellType { Abjuration, Alteration, Conjuration, Divination, Evocation }
        public enum TargetType { Area_Effect, Group, Point_Blank_Area_Effect, Self, Single }

        #region Private Data
        private int m_id; // each spell has a unique spellID
        private bool m_beneficial; // used by npc's to assist in determining which spell to cast at a target
        private Character.ClassType[] m_classTypes; // array of classType that can cast this spell
        private SpellType m_spellType; //
        private TargetType m_targetType; //
        private int m_mana; // mana cost to cast the spell
        private int m_requiredLevel;
        private int m_trainingPrice; // purchase price
        private string m_description; // description of the spell
        private string m_command; // spell command
        private string m_name; // name of the spell
        private string m_soundFile;
        private string m_incantation; 
        #endregion

        #region Constructor
        public Spell(string info)
        {
            string[] spellInfo = info.Split(Protocol.VSPLIT.ToCharArray());
            m_id = Convert.ToInt32(spellInfo[0]);
            m_command = spellInfo[1];
            m_name = spellInfo[2];
            m_description = spellInfo[3];
            m_requiredLevel = Convert.ToInt32(spellInfo[4]);
            m_mana = Convert.ToInt32(spellInfo[5]);
            m_spellType = (SpellType)Convert.ToInt32(spellInfo[6]);
            m_targetType = (TargetType)Convert.ToInt32(spellInfo[7]);
            string[] classes = spellInfo[8].Split(Protocol.ASPLIT.ToCharArray());
            m_classTypes = new Character.ClassType[classes.Length];
            for (int a = 0; a < classes.Length; a++)
                m_classTypes[a] = (Character.ClassType)Convert.ToInt32(classes[a]);
            m_trainingPrice = Convert.ToInt32(spellInfo[9]);
            m_beneficial = Convert.ToBoolean(spellInfo[10]);
            m_soundFile = spellInfo[11];
        } 
        #endregion

        #region Public Properties
        public string Name
        {
            get { return m_name; }
        }
        public int ID
        {
            get { return m_id; }
        }

        public string Incantation
        {
            get { return m_incantation; }
            set { m_incantation = value; }
        } 
        #endregion

        public bool IsClassSpell(Character.ClassType classType)
        {
            if (Array.IndexOf(m_classTypes, classType) != -1)
            {
                return true;
            }
            return false;
        }
    }
}
