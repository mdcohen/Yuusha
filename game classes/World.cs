using System;
using System.Collections;
using System.Collections.Generic;

namespace Yuusha
{
    public class World
    {
        public enum WorldUpdate { Lands, Maps, Spells, Users, Scores, CharGen, Items }
        public enum Alignment { None, Lawful, Neutral, Chaotic, Evil, Amoral, ChaoticEvil }
        public enum ImpLevel { USER, GMA, GMA2, GMA3, GMA4, GM, GM2, GM3, GM4, DEVJR, DEV }

        private static List<Spell> m_spells = new List<Spell>(); // world spells
        private static List<Character> m_users = new List<Character>(); // world users list
        private static List<Character> m_scores = new List<Character>(); // world scores list
        private static List<Land> m_lands = new List<Land>(); // lands list
        private static List<Character> m_newbies = new List<Character>(); // new characters
        private static List<string> m_news = new List<string>(); // server news

        public static Character.ClassType[] ManaUser = new Character.ClassType[] { Character.ClassType.Knight,
            Character.ClassType.Thaumaturge, Character.ClassType.Wizard, Character.ClassType.Thief, Character.ClassType.Ravager,
            Character.ClassType.Sorcerer, Character.ClassType.Druid, Character.ClassType.Ranger};

        public static Character.ClassType[] SpellbookUser = new Character.ClassType[] { Character.ClassType.Thaumaturge,
        Character.ClassType.Wizard, Character.ClassType.Thief, Character.ClassType.Sorcerer, Character.ClassType.Druid,
            Character.ClassType.Ranger };


        public static int[] age_cycle = { 14400, 28800, 43200, 57600, 72000 };

        public static String[] ageTitle_humanoid = { "very young", "young", "middle aged", "old", "very old", "ancient" };

        public static String[] ageTitle_wyrmkin = { "hatchling", "baby", "adolescent", "mature", "aged", "ancient" };

        public static List<string> News
        {
            get { return m_news; }
        }

        public static void GatherWorldData(string info, WorldUpdate worldUpdate)
        {
            int a;
            switch (worldUpdate)
            {
                case WorldUpdate.Lands:
                    #region Lands
                    try
                    {
                        m_lands.Clear();
                        string[] worldLandList = info.Split(Protocol.ISPLIT.ToCharArray());
                        for (a = 0; a < worldLandList.Length; a++)
                        {
                            Land land = new Land(worldLandList[a]);
                            m_lands.Add(land);
                        }
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                    }
                    break; 
                    #endregion
                case WorldUpdate.Maps:
                    #region Maps
                    try
                    {
                        string[] worldMapList = info.Split(Protocol.ISPLIT.ToCharArray());
                        for (a = 0; a < worldMapList.Length; a++)
                        {
                            Map map = new Map(worldMapList[a]);
                            World.GetLandByID(map.LandID).Add(map);
                        }
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                    }
                    break; 
                    #endregion
                case WorldUpdate.Spells:
                    #region Spells
                    try
                    {
                        m_spells.Clear();
                        string[] worldSpellList = info.Split(Protocol.ISPLIT.ToCharArray());
                        for (a = 0; a < worldSpellList.Length; a++)
                        {
                            Spell spell = new Spell(worldSpellList[a]);
                            m_spells.Add(spell);
                        }
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                    }
                    break; 
                    #endregion
                case WorldUpdate.Users:
                    #region Users
                    try
                    {
                        m_users.Clear();
                        string[] worldUsersList = info.Split(Protocol.ISPLIT.ToCharArray());
                        Character user;
                        for (a = 0; a < worldUsersList.Length; a++)
                        {
                            string[] worldUserItem = worldUsersList[a].Split(Protocol.VSPLIT.ToCharArray());
                            user = new Character();
                            user.ID = Convert.ToInt32(worldUserItem[0]);
                            user.ImpLevel = (ImpLevel)Convert.ToInt32(worldUserItem[1]);
                            user.Name = worldUserItem[2];
                            user.ClassFullName = worldUserItem[3];
                            user.Level = Convert.ToInt32(worldUserItem[4]);
                            user.Location = worldUserItem[5]; // user location.. including conf rooms, maps, menus
                            user.m_landID = Convert.ToInt16(worldUserItem[6]);
                            user.m_mapID = Convert.ToInt16(worldUserItem[7]);
                            user.m_anonymous = Convert.ToBoolean(worldUserItem[8]);
                            user.m_invisible = Convert.ToBoolean(worldUserItem[9]);
                            user.m_afk = Convert.ToBoolean(worldUserItem[10]);
                            user.m_receivePages = Convert.ToBoolean(worldUserItem[11]);
                            user.m_receiveTells = Convert.ToBoolean(worldUserItem[12]);
                            user.m_showStaffTitle = Convert.ToBoolean(worldUserItem[13]);
                            m_users.Add(user);
                        }
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                        Utils.Log("info = " + info);
                    }
                    break; 
                    #endregion
                case WorldUpdate.Scores:
                    #region Scores
                    try
                    {
                        m_scores.Clear();

                        string[] worldScoresList = info.Split(Protocol.ISPLIT.ToCharArray());
                        for (a = 0; a < worldScoresList.Length; a++)
                        {
                            string[] worldScoreItem = worldScoresList[a].Split(Protocol.VSPLIT.ToCharArray());
                            Character score = new Character();
                            // these will be sent in order.... once a player enters the conference room
                            // playerID, name, profession, level, experience, kills per hour, last login, anon, implevel, land
                            score.ID = Convert.ToInt32(worldScoreItem[0]);
                            score.Name = worldScoreItem[1];
                            score.Profession = (Character.ClassType)Convert.ToInt32(worldScoreItem[2]);
                            score.ClassFullName = worldScoreItem[3];
                            score.Level = Convert.ToInt32(worldScoreItem[4]);
                            score.Experience = Convert.ToInt64(worldScoreItem[5]);
                            score.NumKills = Convert.ToInt64(worldScoreItem[6]);
                            score.RoundsPlayed = Convert.ToInt64(worldScoreItem[7]);
                            score.lastOnline = worldScoreItem[8];
                            score.m_anonymous = Convert.ToBoolean(worldScoreItem[9]);
                            score.ImpLevel = (ImpLevel)Convert.ToInt32(worldScoreItem[10]);
                            score.m_landID = Convert.ToInt16(worldScoreItem[11]);
                            World.m_scores.Add(score);
                        }

                        //if (Essence.client.scores != null)
                        //{
                        //    System.Threading.Thread populateThread = new System.Threading.Thread(Essence.client.scores.populateScoresGrid);
                        //    populateThread.Start();
                        //}
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                    } 
                    break;
                    #endregion
                case WorldUpdate.CharGen:
                    #region CharGen
                    try
                    {
                        string[] worldCharGenItem = info.Split(Protocol.ISPLIT.ToCharArray());

                        Character newbie = new Character();

                        newbie.Race = worldCharGenItem[0];
                        newbie.Profession = (Character.ClassType)Convert.ToInt32(worldCharGenItem[1]);
                        newbie.Alignment = (Alignment)Convert.ToInt32(worldCharGenItem[2]);

                        string[] startingEquipment = worldCharGenItem[3].Split(Protocol.VSPLIT.ToCharArray());
                        for (a = 0; a < startingEquipment.Length; a++)
                        {
                            Item item = new Item();
                            item.notes = startingEquipment[a];
                            newbie.Inventory.Add(item);
                        }
                        string[] startingSpells = worldCharGenItem[4].Split(Protocol.VSPLIT.ToCharArray());
                        if (startingSpells[0].Length > 0)
                        {
                            for (a = 0; a < startingSpells.Length; a++)
                            {
                                Spell spell = World.GetSpellByID(Convert.ToInt32(startingSpells[a]));
                                newbie.Spells.Add(spell);
                            }
                        }
                        string[] startingSkills = worldCharGenItem[5].Split(Protocol.VSPLIT.ToCharArray());
                        for (a = 0; a < startingSkills.Length; a++)
                        {
                            string[] skillInfo = startingSkills[a].Split(Protocol.ASPLIT.ToCharArray());
                            newbie.SetSkillExperience((Character.SkillType)Convert.ToInt32(skillInfo[0]), Convert.ToInt64(skillInfo[1]));
                        }
                        World.m_newbies.Add(newbie);
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                    }
                    break; 
                    #endregion
                case WorldUpdate.Items:
                    try
                    {
                        string[] worldItemCatalog = info.Split(Protocol.ISPLIT.ToCharArray());
                        for (a = 0; a < worldItemCatalog.Length; a++)
                        {
                            Item item = new Item(worldItemCatalog[a]);
                            Item.addToCatalog(item);
                        }
                        //Utility.Log("Item catalog count = " + Item.getItemCatalogCount());
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                    }
                    break;
                default:
                    break;
            }
        }

        public static Land GetLandByID(short id)
        {
            foreach (Land land in m_lands)
            {
                if (land.ID == id)
                {
                    return land;
                }
            }
            return null;
        }

        public static Spell GetSpellByID(int id)
        {
            foreach (Spell spell in m_spells)
            {
                if (spell.ID == id)
                {
                    return spell;
                }
            }
            return null;
        }

        public static Spell GetSpellByName(string name)
        {
            foreach (Spell spell in m_spells)
            {
                if (spell.Name == name)
                {
                    return spell;
                }
            }
            return null;
        }

        public static Character GetUserByName(string name)
        {
            foreach (Character ch in m_users)
            {
                if (ch.Name == name)
                {
                    return ch;
                }
            }
            return null;
        }

        public static Character GetUserByID(int id)
        {
            foreach (Character ch in m_users)
            {
                if (ch.ID == id)
                {
                    return ch;
                }
            }
            return null;
        }

        public static List<Character> GetSortedUsersList()
        {
            try
            {
                List<Character> masterUsersList = new List<Character>();
                List<Character> temporaryUserList = new List<Character>();
                string[] names;
                int[] ids;
                int[] implevels;
                int a;

                foreach (Character ch in m_users)
                {
                    if (ch.ImpLevel > World.ImpLevel.USER && ch.m_showStaffTitle)
                    {
                        temporaryUserList.Add(ch);
                    }
                }

                if (temporaryUserList.Count > 0)
                {
                    ids = new int[temporaryUserList.Count];
                    implevels = new int[temporaryUserList.Count];

                    for (a = 0; a < temporaryUserList.Count; a++)
                    {
                        Character ch = (Character)temporaryUserList[a];
                        ids[a] = ch.ID;
                        implevels[a] = (int)ch.ImpLevel;
                    }

                    Array.Sort(implevels, ids);

                    foreach (int id in ids)
                    {
                        masterUsersList.Add(World.GetUserByID(id));
                    }
                }

                temporaryUserList.Clear();

                foreach (Character ch in m_users)
                {
                    if (!masterUsersList.Contains(ch))
                    {
                        temporaryUserList.Add(ch);
                    }
                }

                if (temporaryUserList.Count > 0)
                {
                    names = new string[temporaryUserList.Count];

                    for (a = 0; a < temporaryUserList.Count; a++)
                    {
                        Character ch = (Character)temporaryUserList[a];
                        names[a] = ch.Name;
                    }

                    Array.Sort(names);

                    foreach (string name in names)
                    {
                        masterUsersList.Add(World.GetUserByName(name));
                    }
                }
                return masterUsersList;
            }
            catch (Exception e)
            {
                Utils.LogException(e);
                return null;
            }
        }

        public static List<Spell> SpellsList
        {
            get { return World.m_spells; }
        }

        public static List<Character> ScoresList
        {
            get
            {
                return World.m_scores;
            }
        }

        public static Character GetNewbie(string homeland, string classType)
        {
            foreach (Character ch in World.m_newbies)
            {
                if (ch.Race == homeland && Utils.FormatEnumString(ch.Profession.ToString()) == classType)
                {
                    return ch;
                }
            }
            return null;
        }

        public static void AddNews(string news)
        {
            if (m_news.Contains(news))
                return;

            m_news.Add(news);

            try
            {
                if (Client.UserSettings.AutoDisplayNews)
                {
                    gui.Window newsWindow = gui.GuiManager.GenericSheet["NewsWindow"] as gui.Window;
                    if (newsWindow != null)
                    {
                        (newsWindow["NewsScrollableTextBox"] as gui.ScrollableTextBox).Clear();
                        try
                        {
                            for (int a = 0; a < m_news.Count; a++)
                            {
                                string[] nz = m_news[a].Split(Protocol.ISPLIT.ToCharArray());
                                foreach (string line in nz)
                                {
                                    (newsWindow["NewsScrollableTextBox"] as gui.ScrollableTextBox).AddLine(line.Trim(), Enums.ETextType.Default);
                                }
                            }
                        }
                        catch (System.IO.FileNotFoundException)
                        {
                            (newsWindow["NewsScrollableTextBox"] as gui.ScrollableTextBox).AddLine("Failed to display news.", Enums.ETextType.Default);
                        }

                        newsWindow.IsVisible = !newsWindow.IsVisible;
                    }
                }

            }
            catch(Exception e)
            {
                Utils.LogException(e);
            }
        }

        public static string GetImpLevelFullTitle(World.ImpLevel impLevel)
        {
            string t = Client.ClientSettings.ServerName;

            switch (impLevel)
            {
                case ImpLevel.DEV:
                    return t + " Developer";
                case ImpLevel.DEVJR:
                    return t + " Database Administrator";
                case ImpLevel.GM:
                case ImpLevel.GM2:
                case ImpLevel.GM3:
                case ImpLevel.GM4:
                    return t + " Guide";
                case ImpLevel.GMA:
                case ImpLevel.GMA2:
                case ImpLevel.GMA3:
                case ImpLevel.GMA4:
                    return t + " Event Coordinator";
                default:
                    return "";
            }
        }
    }
}
