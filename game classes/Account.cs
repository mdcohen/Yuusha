using System;
using System.Collections.Generic;

namespace Yuusha
{
    public class Account
    {
        #region Private Data
        private static string m_name = "";
        private static int m_id = -1;
        private static int m_lifetimeMarks = 0;
        private static int m_currentMarks = 0;
        private static string m_ipAddress = "";
        private static List<Character> m_characters = new List<Character>(); // list of characters on the account 
        #endregion

        #region Public Properties
        public static string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }
        public static List<Character> Characters
        {
            get { return m_characters; }
        }
        #endregion

        public static void SetAccountInfo(string info)
        {
            string[] accountInfo = info.Split(Protocol.VSPLIT.ToCharArray());

            m_name = accountInfo[0];
            m_id = Convert.ToInt32(accountInfo[1]);
            m_lifetimeMarks = Convert.ToInt32(accountInfo[2]);
            m_currentMarks = Convert.ToInt32(accountInfo[3]);
            m_ipAddress = accountInfo[4];

            Account.CreateAccountDirectory();
            
        }        

        /// <summary>
        /// Create a directory for the account if it does not exist.
        /// </summary>
        public static void CreateAccountDirectory()
        {
            string accountDirectory = Utils.StartupPath + Utils.AccountsFolder + Account.Name + "\\";

            if (!System.IO.Directory.Exists(accountDirectory))
                System.IO.Directory.CreateDirectory(accountDirectory);
        }

        /// <summary>
        /// Get the next character in the Characters List
        /// </summary>
        /// <returns></returns>
        public static Character GetNextCharacter()
        {
            if (m_characters.Count == 1)
            { return m_characters[0]; }

            if (m_characters.IndexOf(Character.CurrentCharacter) + 1 >= m_characters.Count)
                return m_characters[0];

            return m_characters[m_characters.IndexOf(Character.CurrentCharacter) + 1];
        }

        /// <summary>
        /// Get the previous character in the Characters List
        /// </summary>
        /// <returns></returns>
        public static Character GetPreviousCharacter()
        {
            if(m_characters.Count == 1)
            { return m_characters[0]; }

            if ((m_characters.IndexOf(Character.CurrentCharacter) - 1) < 0)
            {
                return m_characters[m_characters.Count - 1];
            }
            
            return m_characters[m_characters.IndexOf(Character.CurrentCharacter) - 1];
        }

        /// <summary>
        /// Get Character by ID.
        /// </summary>
        /// <param name="id">The ID of the Character to retrieve.</param>
        /// <returns></returns>
        public static Character GetCharacterByID(int id)
        {
            foreach (Character ch in m_characters)
            {
                if (ch.ID == id)
                {
                    return ch;
                }
            }
            return null;
        }
    }
}
