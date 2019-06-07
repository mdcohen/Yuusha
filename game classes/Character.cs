using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CharacterSettings = Yuusha.Utility.Settings.CharacterSettings;
using Yuusha.gui;

namespace Yuusha
{
    public class Character
    {
        public enum SkillType { None, Bow, Dagger, Flail, Halberd, Mace, Rapier, Shuriken, Staff, Sword, Threestaff, Two_Handed, Unarmed, Thievery, Magic }

        public enum WearLocation { None, Head, Neck, Nose, Ear, Face, Shoulders, Back, Torso, Bicep, Forearms, Wrist, Finger, Waist, Legs, Calves, Shins, Feet, Hands }

        public enum WearOrientation { None, Left, Right }

        public enum ClassType { None, Fighter, Thaumaturge, Wizard, Martial_Artist, Thief, Knight, Ravager, Sorcerer }

        public enum SpeciesType
        {
            Unknown,
            FireDragon,
            IceDragon,
            LightningDrake,
            TundraYeti,
            Sandwyrm,
            Arachnid,
        }

        public enum GenderType { It, Male, Female, Random }

        public enum HomelandType { Barbarian, Draznia, Hovath, Illyria, Lemuria, Leng, Mnar, Mu }

        public static ClassType[] spellUsers = new ClassType[] { ClassType.Thaumaturge, ClassType.Wizard, ClassType.Thief, ClassType.Knight };

        public const short NAME_MIN_LENGTH = 4;
        public const short NAME_MAX_LENGTH = 14;

        public static int[] maxWearable = { 0, 1, 1, 0, 1, 2, 1, 1, 1, 1, 0, 2, 1, 2, 8, 1, 1, 0, 1, 1, 1, 1 };

        private static Character m_currentCharacter;
        private static Character m_previousRoundCharacter = new Character();

        private static CharacterSettings m_characterSettings = new CharacterSettings();

        public static Character CurrentCharacter
        {
            get { return m_currentCharacter; }
            set { m_currentCharacter = value; }
        }

        public static Character PreviousRoundCharacter
        {
            get { return m_previousRoundCharacter; }
            set { m_previousRoundCharacter = value; }
        }

        public static CharacterSettings Settings
        {
            get { return m_characterSettings; }
        }

        public static void GatherCharacterList(string info)
        {
            try
            {
                Account.Characters.Clear(); // clear account player character list

                ArrayList characterList = new ArrayList();

                // if a split is detected there is more than one character
                if (info.IndexOf(Protocol.CHARACTER_LIST_SPLIT) != -1)
                {
                    do // do it once, then do it again if there is another split
                    {
                        string pcInfo = info.Substring(0, info.IndexOf(Protocol.CHARACTER_LIST_SPLIT));
                        characterList.Add(pcInfo);
                        info = info.Remove(0, info.IndexOf(Protocol.CHARACTER_LIST_SPLIT) + Protocol.CHARACTER_LIST_SPLIT.Length);
                    }
                    while (info.IndexOf(Protocol.CHARACTER_LIST_SPLIT) != -1);
                }

                // always add the last character if more than one, or single character if only one character exists
                characterList.Add(info);

                for (int a = 0; a < characterList.Count; a++)
                {
                    m_currentCharacter = new Character();

                    GatherCharacterData(Protocol.GetProtoInfoFromString((string)characterList[a], Protocol.CHARACTER_STATS, Protocol.CHARACTER_STATS_END), Enums.EPlayerUpdate.Stats);
                    GatherCharacterData(Protocol.GetProtoInfoFromString((string)characterList[a], Protocol.CHARACTER_RIGHTHAND, Protocol.CHARACTER_RIGHTHAND_END), Enums.EPlayerUpdate.RightHand);
                    GatherCharacterData(Protocol.GetProtoInfoFromString((string)characterList[a], Protocol.CHARACTER_LEFTHAND, Protocol.CHARACTER_LEFTHAND_END), Enums.EPlayerUpdate.LeftHand);
                    GatherCharacterData(Protocol.GetProtoInfoFromString((string)characterList[a], Protocol.CHARACTER_MACROS, Protocol.CHARACTER_MACROS_END), Enums.EPlayerUpdate.Macros);

                    Account.Characters.Add(m_currentCharacter);
                }
            }
            catch (Exception e)
            {
                Utils.LogException(e);
            }
        }

        public static void GatherCharacterData(string info, Enums.EPlayerUpdate update)
        {
            int a;
            string[] list;

            switch (update)
            {
                case Enums.EPlayerUpdate.Stats:
                    #region Stats (full)
                    try
                    {
                        string[] pcStats = info.Split(Protocol.VSPLIT.ToCharArray());
                        m_currentCharacter.ID = Convert.ToInt32(pcStats[0]);
                        m_currentCharacter.Name = pcStats[1];
                        m_currentCharacter.Gender = (Character.GenderType)Convert.ToInt32(pcStats[2]);
                        m_currentCharacter.Race = pcStats[3];
                        m_currentCharacter.Profession = (Character.ClassType)Convert.ToInt32(pcStats[4]);
                        m_currentCharacter.ClassFullName = pcStats[5];
                        m_currentCharacter.Alignment = (World.Alignment)Convert.ToInt32(pcStats[6]);
                        m_currentCharacter.ImpLevel = (World.ImpLevel)Convert.ToInt32(pcStats[7]);
                        m_currentCharacter.IsImmortal = Convert.ToBoolean(pcStats[8]);
                        m_currentCharacter.m_showStaffTitle = Convert.ToBoolean(pcStats[9]);
                        m_currentCharacter.m_receivePages = Convert.ToBoolean(pcStats[10]);
                        m_currentCharacter.m_receiveTells = Convert.ToBoolean(pcStats[11]);
                        list = pcStats[12].Split(Protocol.ASPLIT.ToCharArray());
                        m_currentCharacter.m_friends = new int[list.Length];
                        for (a = 0; a < list.Length; a++)
                        {
                            m_currentCharacter.m_friends[a] = Convert.ToInt32(list[a]);
                        }
                        list = pcStats[13].Split(Protocol.ASPLIT.ToCharArray());
                        m_currentCharacter.m_ignored = new int[list.Length];
                        for (a = 0; a < list.Length; a++)
                        {
                            m_currentCharacter.m_ignored[a] = Convert.ToInt32(list[a]);
                        }
                        m_currentCharacter.m_friendNotify = Convert.ToBoolean(pcStats[14]);
                        m_currentCharacter.m_filterProfanity = Convert.ToBoolean(pcStats[15]);
                        m_currentCharacter.m_ancestor = Convert.ToBoolean(pcStats[16]);
                        m_currentCharacter.m_anonymous = Convert.ToBoolean(pcStats[17]);
                        m_currentCharacter.m_landID = Convert.ToInt16(pcStats[18]);
                        m_currentCharacter.m_mapID = Convert.ToInt16(pcStats[19]);
                        m_currentCharacter.m_xCord = Convert.ToInt32(pcStats[20]);
                        m_currentCharacter.m_yCord = Convert.ToInt32(pcStats[21]);
                        m_currentCharacter.m_stunned = Convert.ToInt16(pcStats[22]);
                        m_currentCharacter.m_floating = Convert.ToInt16(pcStats[23]);
                        m_currentCharacter.m_dead = Convert.ToBoolean(pcStats[24]);
                        m_currentCharacter.m_hidden = Convert.ToBoolean(pcStats[25]);
                        m_currentCharacter.m_invisible = Convert.ToBoolean(pcStats[26]);
                        m_currentCharacter.m_nightVision = Convert.ToBoolean(pcStats[27]);
                        m_currentCharacter.featherFall = Convert.ToBoolean(pcStats[28]);
                        m_currentCharacter.breatheWater = Convert.ToBoolean(pcStats[29]);
                        m_currentCharacter.blind = Convert.ToBoolean(pcStats[30]);
                        m_currentCharacter.poisoned = Convert.ToInt32(pcStats[31]);
                        m_currentCharacter.fighterSpecial = (Character.SkillType)Convert.ToInt32(pcStats[32]);
                        m_currentCharacter.Level = Convert.ToInt16(pcStats[33]);
                        m_currentCharacter.Experience = Convert.ToInt64(pcStats[34]);
                        m_currentCharacter.Hits = Convert.ToInt32(pcStats[35]);
                        m_currentCharacter.HitsMax = Convert.ToInt32(pcStats[36]);
                        m_currentCharacter.Stamina = Convert.ToInt32(pcStats[37]);
                        m_currentCharacter.StaminaMax = Convert.ToInt32(pcStats[38]);
                        m_currentCharacter.Mana = Convert.ToInt32(pcStats[39]);
                        m_currentCharacter.ManaMax = Convert.ToInt32(pcStats[40]);
                        m_currentCharacter.Age = Convert.ToInt32(pcStats[41]);
                        m_currentCharacter.RoundsPlayed = Convert.ToInt64(pcStats[42]);
                        m_currentCharacter.NumKills = Convert.ToInt64(pcStats[43]);
                        m_currentCharacter.NumDeaths = Convert.ToInt64(pcStats[44]);
                        m_currentCharacter.BankGold = Convert.ToInt64(pcStats[45]);
                        m_currentCharacter.Strength = Convert.ToInt32(pcStats[46]);
                        m_currentCharacter.Dexterity = Convert.ToInt32(pcStats[47]);
                        m_currentCharacter.Intelligence = Convert.ToInt32(pcStats[48]);
                        m_currentCharacter.Wisdom = Convert.ToInt32(pcStats[49]);
                        m_currentCharacter.Constitution = Convert.ToInt32(pcStats[50]);
                        m_currentCharacter.Charisma = Convert.ToInt32(pcStats[51]);
                        m_currentCharacter.StrengthAdd = Convert.ToInt32(pcStats[52]);
                        m_currentCharacter.DexterityAdd = Convert.ToInt32(pcStats[53]);
                        m_currentCharacter.encumbrance = Convert.ToInt32(pcStats[54]);
                        m_currentCharacter.birthday = pcStats[55];
                        m_currentCharacter.lastOnline = pcStats[56];
                        m_currentCharacter.karma = Convert.ToInt32(pcStats[57]);
                        m_currentCharacter.marks = Convert.ToInt32(pcStats[58]);
                        m_currentCharacter.pvpKills = Convert.ToInt64(pcStats[59]);
                        m_currentCharacter.pvpDeaths = Convert.ToInt64(pcStats[60]);
                        if (pcStats[61].Length > 0)
                        {
                            list = pcStats[61].Split(Protocol.ASPLIT.ToCharArray());
                            m_currentCharacter.playersKilled = new int[list.Length];
                            for (a = 0; a < list.Length; a++)
                            {
                                m_currentCharacter.playersKilled[a] = Convert.ToInt32(list[a]);
                            }
                        }
                        if (pcStats[62].Length > 0)
                        {
                            list = pcStats[62].Split(Protocol.ASPLIT.ToCharArray());
                            m_currentCharacter.playersFlagged = new int[list.Length];
                            for (a = 0; a < list.Length; a++)
                            {
                                m_currentCharacter.playersFlagged[a] = Convert.ToInt32(list[a]);
                            }
                        }
                        m_currentCharacter.knightRing = Convert.ToBoolean(pcStats[63]);
                        m_currentCharacter.directionPointer = Convert.ToString(pcStats[64]);
                        m_currentCharacter.HitsAdjustment = Convert.ToInt32(pcStats[65]);
                        m_currentCharacter.ManaAdjustment = Convert.ToInt32(pcStats[66]);
                        m_currentCharacter.StaminaAdjustment = Convert.ToInt32(pcStats[67]);
                        m_currentCharacter.VisualKey = pcStats[68];
                        m_currentCharacter.HitsDoctored = Convert.ToInt32(pcStats[69]);
                        m_currentCharacter.lastOnline = pcStats[70];
                        m_currentCharacter.MapName = pcStats[71];
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                        Utils.Log(info);
                    }
                    break;
                    #endregion
                case Enums.EPlayerUpdate.Hits:
                    #region Hits
                    try
                    {
                        string[] hitsInfo = info.Split(Protocol.VSPLIT.ToCharArray());
                        m_currentCharacter.Hits = Convert.ToInt32(hitsInfo[0]);
                        m_currentCharacter.HitsMax = Convert.ToInt32(hitsInfo[1]);
                        m_currentCharacter.HitsAdjustment = Convert.ToInt32(hitsInfo[2]);
                        m_currentCharacter.HitsDoctored = Convert.ToInt32(hitsInfo[3]);
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                    }
                    break; 
                    #endregion
                case Enums.EPlayerUpdate.Stamina:
                    #region Stamina
                    try
                    {
                        string[] staminaInfo = info.Split(Protocol.VSPLIT.ToCharArray());
                        m_currentCharacter.Stamina = Convert.ToInt32(staminaInfo[0]);
                        m_currentCharacter.StaminaMax = Convert.ToInt32(staminaInfo[1]);
                        m_currentCharacter.StaminaAdjustment = Convert.ToInt32(staminaInfo[2]);
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                    }
                    break; 
                    #endregion
                case Enums.EPlayerUpdate.Mana:
                    #region Mana
                    try
                    {
                        string[] manaInfo = info.Split(Protocol.VSPLIT.ToCharArray());
                        m_currentCharacter.Mana = Convert.ToInt32(manaInfo[0]);
                        m_currentCharacter.ManaMax = Convert.ToInt32(manaInfo[1]);
                        m_currentCharacter.ManaAdjustment = Convert.ToInt32(manaInfo[2]);
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                    }
                    break; 
                    #endregion
                case Enums.EPlayerUpdate.Experience:
                    #region Experience
                    try
                    {
                        string[] expInfo = info.Split(Protocol.VSPLIT.ToCharArray());
                        // experience change is expInfo[0]
                        m_currentCharacter.Experience = Convert.ToInt64(expInfo[1]);
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                    }
                    break; 
                    #endregion
                case Enums.EPlayerUpdate.Skills:
                    #region Skills
                    try
                    {
                        string[] pcSkills = info.Split(Protocol.VSPLIT.ToCharArray());
                        for (a = 0; a < pcSkills.Length; a++)
                        {
                            string[] skillInfo = pcSkills[a].Split(Protocol.ASPLIT.ToCharArray());
                            m_currentCharacter.SetSkillExperience((Character.SkillType)Convert.ToInt32(skillInfo[0]), Convert.ToInt64(skillInfo[1]));
                        }
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                    }
                    break;
                    #endregion
                case Enums.EPlayerUpdate.RightHand:
                    #region RightHand
                    try
                    {
                        if (info.Length > 0)
                        {
                            m_currentCharacter.RightHand = new Item(info);
                        }
                        else // empty right hand
                        {
                            m_currentCharacter.RightHand = null;
                        }
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                    }
                    break;
                    #endregion
                case Enums.EPlayerUpdate.LeftHand:
                    #region LeftHand
                    try
                    {
                        if (info.Length > 0)
                        {
                            m_currentCharacter.LeftHand = new Item(info);
                        }
                        else // empty left hand
                        {
                            m_currentCharacter.LeftHand = null;
                        }
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                    }
                    break;
                    #endregion
                case Enums.EPlayerUpdate.Inventory:
                    #region Inventory
                    try
                    {
                        m_currentCharacter.Inventory.Clear();
                        string[] pcInventory = info.Split(Protocol.ISPLIT.ToCharArray());
                        if (pcInventory[0].Length > 0)
                        {
                            for (a = 0; a < pcInventory.Length; a++)
                            {
                                Item inventoryItem = new Item(pcInventory[a]);
                                m_currentCharacter.Inventory.Add(inventoryItem);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                    }
                    break;
                    #endregion
                case Enums.EPlayerUpdate.Sack:
                    #region Sack
                    try
                    {
                        m_currentCharacter.Sack.Clear();
                        string[] pcSack = info.Split(Protocol.ISPLIT.ToCharArray());
                        if (pcSack[0].Length > 0)
                        {
                            for (a = 0; a < pcSack.Length; a++)
                            {
                                //Utils.Log("Sack Item " + a + ": " + pcSack[a]);
                                Item sackItem = new Item(pcSack[a]);
                                m_currentCharacter.Sack.Add(sackItem);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                    }
                    break;
                #endregion
                case Enums.EPlayerUpdate.Pouch:
                    #region Pouch
                    try
                    {
                        m_currentCharacter.Pouch.Clear();
                        string[] pcPouch = info.Split(Protocol.ISPLIT.ToCharArray());
                        if (pcPouch[0].Length > 0)
                        {
                            for (a = 0; a < pcPouch.Length; a++)
                            {
                                Item sackItem = new Item(pcPouch[a]);
                                m_currentCharacter.Sack.Add(sackItem);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                    }
                    break;
                #endregion
                case Enums.EPlayerUpdate.Belt:
                    #region Belt
                    try
                    {
                        m_currentCharacter.Belt.Clear();
                        string[] pcBelt = info.Split(Protocol.ISPLIT.ToCharArray());
                        if (pcBelt[0].Length > 0)
                        {
                            for (a = 0; a < pcBelt.Length; a++)
                            {
                                Item beltItem = new Item(pcBelt[a]);
                                m_currentCharacter.Belt.Add(beltItem);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                    }
                    break;
                    #endregion
                case Enums.EPlayerUpdate.Rings:
                    #region Rings
                    try
                    {
                        m_currentCharacter.Rings.Clear();
                        string[] pcRings = info.Split(Protocol.ISPLIT.ToCharArray());
                        for (a = 0; a < pcRings.Length; a++)
                        {
                            if (pcRings[a].Length > 0)
                            {
                                Item ringItem = new Item(pcRings[a]);
                                m_currentCharacter.Rings.Add(ringItem);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                    }
                    break;
                    #endregion
                case Enums.EPlayerUpdate.Locker:
                    #region Locker
                    try
                    {
                        m_currentCharacter.Locker.Clear();
                        string[] pcLocker = info.Split(Protocol.ISPLIT.ToCharArray());
                        if (pcLocker[0].Length > 0)
                        {
                            for (a = 0; a < pcLocker.Length; a++)
                            {
                                Item lockerItem = new Item(pcLocker[a]);
                                m_currentCharacter.Locker.Add(lockerItem);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                    }
                    break;
                    #endregion
                case Enums.EPlayerUpdate.Spells:
                    #region Spells
                    try
                    {
                        m_currentCharacter.Spells.Clear();
                        string[] pcSpellbook = info.Split(Protocol.ISPLIT.ToCharArray());
                        if (pcSpellbook[0].Length > 0)
                        {
                            for (a = 0; a < pcSpellbook.Length; a++)
                            {
                                string[] spellInfo = pcSpellbook[a].Split(Protocol.VSPLIT.ToCharArray());
                                Spell spell = World.GetSpellByID(Convert.ToInt32(spellInfo[0]));
                                spell.Incantation = spellInfo[1];
                                m_currentCharacter.Spells.Add(spell);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                    }
                    break;
                    #endregion
                case Enums.EPlayerUpdate.Effects:
                    #region Effects
                    try
                    {
                        m_currentCharacter.Effects.Clear();
                        string[] pcEffects = info.Split(Protocol.ISPLIT.ToCharArray());
                        if (pcEffects[0].Length > 0)
                        {
                            for (a = 0; a < pcEffects.Length; a++)
                            {
                                m_currentCharacter.Effects.Add(new Effect(pcEffects[a]));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                    }
                    break;
                    #endregion
                case Enums.EPlayerUpdate.Macros:
                    #region Macros
                    string[] macrosArray = info.Split(Protocol.ISPLIT.ToCharArray());

                    System.Collections.Generic.List<string> macroList = new System.Collections.Generic.List<string>();

                    if (Character.CurrentCharacter != null && Character.CurrentCharacter.Macros != null)
                        Character.CurrentCharacter.Macros.Clear();

                    foreach (string macro in macrosArray)
                        macroList.Add(macro);

                    m_currentCharacter.m_macros = new System.Collections.Generic.List<string>(macroList);

                    gui.GenericSheet.LoadMacros();
                    break; 
                    #endregion
            }

            switch (Client.GameState)
            {
                case Enums.EGameState.IOKGame:
                    break;
            }
        }

        public static void LoadSettings()
        {
            m_characterSettings = CharacterSettings.Load();

            Events.RegisterEvent(Events.EventName.Load_Character_Settings);
        }

        public string title; // sent in users list info
        public bool isPC;
        public string shortDesc;
        public string longDesc;
        public int imageX;
        public int imageY;
        public int corpseImageX;
        public int corpseImageY;
        public string visibleArmor;
        public string assignedLetter;
       
        public bool m_afk;
        public bool m_showStaffTitle;
        public string m_lastOnline;
        public bool m_receivePages;
        public bool m_receiveTells;
        public int[] m_friends;
        public int[] m_ignored;
        public bool m_friendNotify;
        public bool m_filterProfanity;
        public bool m_ancestor;
        public bool m_anonymous;
        public short m_landID;
        public short m_mapID;
        public int m_xCord;
        public int m_yCord;
        public short m_stunned;
        public short m_floating;
        public bool m_dead;
        public bool m_hidden;
        public bool m_invisible;
        public bool m_nightVision;
        public bool featherFall;
        public bool breatheWater;
        public bool blind;
        public int poisoned;
        public SkillType fighterSpecial;
        
        public int encumbrance;
        public string birthday;
        public string lastOnline;
        public int karma;
        public int marks;
        public long pvpKills;
        public long pvpDeaths;
        public int[] playersKilled;
        public int[] playersFlagged;
        public bool knightRing;
        public string directionPointer;

        #region Private Data
        private int m_id;
        private string m_name;
        private string m_mapName;
        private int m_level;
        private GenderType m_gender;
        private string m_race;
        private string m_classFullName;
        private ClassType m_profession;
        private int m_hits;
        private int m_hitsMax;
        private int m_hitsAdjustment;
        private int m_hitsDoctored;
        private int m_stamina;
        private int m_staminaMax;
        private int m_staminaAdjustment;
        private int m_mana;
        private int m_manaMax;
        private int m_manaAdjustment;
        private int m_age;
        private long m_roundsPlayed;
        private long m_numKills;
        private long m_numDeaths;
        private long m_bankGold;
        private int m_strength;
        private int m_dexterity;
        private int m_intelligence;
        private int m_wisdom;
        private int m_constitution;
        private int m_charisma;
        private int m_strengthAdd;
        private int m_dexterityAdd;
        private World.Alignment m_alignment;
        private string m_location;
        private World.ImpLevel m_impLevel;
        private bool m_immortal;
        private gui.VisualKey m_visualKey;
        private long m_experience;

        private long m_mace;
        private long m_bow;
        private long m_flail;
        private long m_dagger;
        private long m_rapier;
        private long m_twoHanded;
        private long m_staff;
        private long m_shuriken;
        private long m_sword;
        private long m_threestaff;
        private long m_halberd;
        private long m_unarmed;
        private long m_thievery;
        private long m_magic;

        private Item m_rightHand;
        private Item m_leftHand;
        private List<Item> m_inventory;
        private List<Item> m_sack;
        private List<Item> m_pouch;
        private List<Item> m_belt;
        private List<Item> m_rings;
        private List<Item> m_locker;
        private List<Spell> m_spells;
        private List<Effect> m_effects;
        private List<string> m_macros;
        #endregion

        #region Public Properties
        public int ID
        {
            get { return this.m_id; }
            set { this.m_id = value; }
        }
        public string Name
        {
            get { return this.m_name; }
            set { this.m_name = value; }
        }
        public string MapName
        {
            get { return this.m_mapName; }
            set { this.m_mapName = value; }
        }
        public int Level
        {
            get { return m_level; }
            set { m_level = value; }
        }
        public long Experience
        {
            get { return m_experience; }
            set { m_experience = value; }
        }
        public GenderType Gender
        {
            get { return this.m_gender; }
            set { this.m_gender = value; }
        }
        public string Race
        {
            get { return this.m_race; }
            set { this.m_race = value; }
        }
        public string ClassFullName
        {
            get { return this.m_classFullName; }
            set { this.m_classFullName = value; }
        }
        public ClassType Profession
        {
            get { return this.m_profession; }
            set { this.m_profession = value; }
        }
        public int Hits
        {
            get { return m_hits; }
            set { m_hits = value; }
        }
        public int HitsMax
        {
            get { return m_hitsMax; }
            set { m_hitsMax = value; }
        }
        public int HitsAdjustment
        {
            get { return m_hitsAdjustment; }
            set { m_hitsAdjustment = value; }
        }
        public int HitsDoctored
        {
            get { return m_hitsDoctored; }
            set { m_hitsDoctored = value; }
        }
        public int Stamina
        {
            get { return m_stamina; }
            set { m_stamina = value; }
        }
        public int StaminaMax
        {
            get { return m_staminaMax; }
            set { m_staminaMax = value; }
        }
        public int StaminaAdjustment
        {
            get { return m_staminaAdjustment; }
            set { m_staminaAdjustment = value; }
        }
        public int Mana
        {
            get { return m_mana; }
            set { m_mana = value; }
        }
        public int ManaMax
        {
            get { return m_manaMax; }
            set { m_manaMax = value; }
        }
        public int ManaAdjustment
        {
            get { return m_manaAdjustment; }
            set { m_manaAdjustment = value; }
        }
        public int Age
        {
            get { return m_age; }
            set { m_age = value; }
        }
        public long RoundsPlayed
        {
            get { return m_roundsPlayed; }
            set { m_roundsPlayed = value; }
        }
        public long NumKills
        {
            get { return m_numKills; }
            set { m_numKills = value; }
        }
        public long NumDeaths
        {
            get { return m_numDeaths; }
            set { m_numDeaths = value; }
        }
        public long BankGold
        {
            get { return m_bankGold; }
            set { m_bankGold = value; }
        }
        public int Strength
        {
            get { return m_strength; }
            set { m_strength = value; }
        }
        public int Dexterity
        {
            get { return m_dexterity; }
            set { m_dexterity = value; }
        }
        public int Intelligence
        {
            get { return m_intelligence; }
            set { m_intelligence = value; }
        }
        public int Wisdom
        {
            get { return m_wisdom; }
            set { m_wisdom = value; }
        }
        public int Constitution
        {
            get { return m_constitution; }
            set { m_constitution = value; }
        }
        public int Charisma
        {
            get { return m_charisma; }
            set { m_charisma = value; }
        }
        public int StrengthAdd
        {
            get { return m_strengthAdd; }
            set { m_strengthAdd = value; }
        }
        public int DexterityAdd
        {
            get { return m_dexterityAdd; }
            set { m_dexterityAdd = value; }
        }
        public World.Alignment Alignment
        {
            get { return this.m_alignment; }
            set { this.m_alignment = value; }
        }
        public string Location
        {
            get { return this.m_location; }
            set { this.m_location = value; }
        }
        public World.ImpLevel ImpLevel
        {
            get { return this.m_impLevel; }
            set { this.m_impLevel = value; }
        }
        public bool IsImmortal
        {
            get { return this.m_immortal; }
            set { this.m_immortal = value; }
        }
        public string VisualKey
        {
            get { return m_visualKey.Key; }
            set { m_visualKey.Key = value; }
        }
        public Item RightHand
        {
            get { return this.m_rightHand; }
            set { this.m_rightHand = value; }
        }
        public Item LeftHand
        {
            get { return this.m_leftHand; }
            set { this.m_leftHand = value; }
        }
        public List<Item> Inventory
        {
            get { return this.m_inventory; }
        }
        public List<Item> Sack
        {
            get { return this.m_sack; }
        }
        public List<Item> Pouch
        {
            get { return this.m_pouch; }
        }
        public List<Item> Belt
        {
            get { return this.m_belt; }
        }
        public List<Item> Rings
        {
            get { return this.m_rings; }
        }
        public List<Item> Locker
        {
            get { return this.m_locker; }
        }
        public List<Spell> Spells
        {
            get { return this.m_spells; }
        }
        public List<Effect> Effects
        {
            get { return this.m_effects; }
        }
        public List<string> Macros
        {
            get { return this.m_macros; }
        }
        public int HitsFull
        {
            get { return HitsMax + HitsAdjustment + HitsDoctored; }
        }
        public int ManaFull
        {
            get { return this.ManaMax + this.ManaAdjustment; }
        }
        public int StaminaFull
        {
            get { return this.StaminaMax + this.StaminaAdjustment; }
        }
        public bool IsSpellUser
        {
            get
            {
                return Array.IndexOf(World.m_spellUsers, this.Profession) > -1;
            }
        }
        public bool IsPeeking
        {
            get
            {
                foreach (Effect effect in m_effects)
                    if (effect.Name == "Peek") return true;
                return false;
            }
        }
        #endregion

        #region Constructor
        public Character()
        {
            m_visualKey = new VisualKey("unknown");
            this.m_rightHand = new Item();
            this.m_leftHand = new Item();
            this.m_inventory = new List<Item>();
            this.m_sack = new List<Item>();
            this.m_pouch = new List<Item>();
            this.m_belt = new List<Item>();
            this.m_rings = new List<Item>();
            this.m_locker = new List<Item>();
            this.m_spells = new List<Spell>();
            this.m_effects = new List<Effect>();
            this.m_macros = new List<string>();
        } 
        #endregion

        public void SetSkillExperience(SkillType skillType, long experience)
        {
            switch (skillType)
            {
                case SkillType.Bow:
                    this.m_bow = experience;
                    break;
                case SkillType.Sword:
                    this.m_sword = experience;
                    break;
                case SkillType.Two_Handed:
                    this.m_twoHanded = experience;
                    break;
                case SkillType.Unarmed:
                    this.m_unarmed = experience;
                    break;
                case SkillType.Staff:
                    this.m_staff = experience;
                    break;
                case SkillType.Dagger:
                    this.m_dagger = experience;
                    break;
                case SkillType.Halberd:
                    this.m_halberd = experience;
                    break;
                case SkillType.Rapier:
                    this.m_rapier = experience;
                    break;
                case SkillType.Shuriken:
                    this.m_shuriken = experience;
                    break;
                case SkillType.Magic:
                    this.m_magic = experience;
                    break;
                case SkillType.Mace:
                    this.m_mace = experience;
                    break;
                case SkillType.Flail:
                    this.m_flail = experience;
                    break;
                case SkillType.Threestaff:
                    this.m_threestaff = experience;
                    break;
                case SkillType.Thievery:
                    this.m_thievery = experience;
                    break;
            }
        }

        public long GetSkillExperience(SkillType skillType)
        {
            switch (skillType)
            {
                case SkillType.Bow:
                    return this.m_bow;
                case SkillType.Sword:
                    return this.m_sword;
                case SkillType.Two_Handed:
                    return this.m_twoHanded;
                case SkillType.Unarmed:
                    return this.m_unarmed;
                case SkillType.Staff:
                    return this.m_staff;
                case SkillType.Dagger:
                    return this.m_dagger;
                case SkillType.Halberd:
                    return this.m_halberd;
                case SkillType.Rapier:
                    return this.m_rapier;
                case SkillType.Shuriken:
                    return this.m_shuriken;
                case SkillType.Magic:
                    return this.m_magic;
                case SkillType.Mace:
                    return this.m_mace;
                case SkillType.Flail:
                    return this.m_flail;
                case SkillType.Threestaff:
                    return this.m_threestaff;
                case SkillType.Thievery:
                    return this.m_thievery;
            }
            return 0;
        }

        public Character Clone()
        {
            return (Character)this.MemberwiseClone();
        }
    }
}
