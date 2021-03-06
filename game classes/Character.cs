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

        public enum WearLocation { None, Head, Neck, Nose, Face, Shoulders, Back, Torso, Bicep, Wrist, Finger, Waist, Legs, Feet, Hands }

        public enum WearOrientation { None, RightRing1, RightRing2, RightRing3, RightRing4, LeftRing1, LeftRing2, LeftRing3, LeftRing4, Left = 9, Right = 10 }

        public enum ClassType { None, Fighter, Thaumaturge, Wizard, Martial_Artist, Thief, Knight, Ravager, Sorcerer, Druid, Ranger, Berserker }

        public enum GenderType { It, Male, Female, Random }

        public enum HomelandType { Barbarian, Draznia, Hovath, Illyria, Lemuria, Leng, Mnar, Mu }

        public const short NAME_MIN_LENGTH = 4;
        public const short NAME_MAX_LENGTH = 14;

        public static int[] maxWearable = { 0, 1, 1, 0, 1, 2, 1, 1, 1, 1, 0, 2, 1, 2, 8, 1, 1, 0, 1, 1, 1, 1 };

        private static readonly XYCoordinate[] directions =
        {
            new XYCoordinate(-1, -1), new XYCoordinate(-1, 0), new XYCoordinate(-1, 1), new XYCoordinate(0, -1), new XYCoordinate(0, 1), new XYCoordinate(1, -1),
            new XYCoordinate(1, 0), new XYCoordinate(1, 1)
        };

        private static Character m_currentCharacter;
        private static Character m_previousRoundCharacter = new Character();

        private static CharacterSettings m_characterSettings = new CharacterSettings();

        public static Character CurrentCharacter
        {
            get { return m_currentCharacter; }
            set
            {
                m_currentCharacter = value;

                if (value != null)
                    PreviousRoundCharacter = CurrentCharacter.Clone();
            }
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

        public static Utility.Settings.FogOfWarSettings FogOfWarSettings
        { get; set; }

        public static Utility.Settings.GUIPositionSettings GUIPositionSettings
        { get; set; }

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
                        m_currentCharacter.UniqueID = Convert.ToInt32(pcStats[0]);
                        m_currentCharacter.Name = pcStats[1];
                        m_currentCharacter.Gender = (GenderType)Convert.ToInt32(pcStats[2]);
                        m_currentCharacter.Race = pcStats[3];
                        m_currentCharacter.Profession = (ClassType)Convert.ToInt32(pcStats[4]);
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
                        m_currentCharacter.m_ancestor = Convert.ToBoolean(pcStats[16]); //
                        m_currentCharacter.m_anonymous = Convert.ToBoolean(pcStats[17]); //
                        m_currentCharacter.LandID = Convert.ToInt16(pcStats[18]);
                        m_currentCharacter.MapID = Convert.ToInt16(pcStats[19]);
                        m_currentCharacter.X = Convert.ToInt32(pcStats[20]);
                        m_currentCharacter.Y = Convert.ToInt32(pcStats[21]);
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
                        //string.Format("{0:C0}", pcStats[45]);
                        // there is a server side bug with decimal places in bank gold 10/14/2019
                        m_currentCharacter.BankGold = Convert.ToInt64(string.Format("{0:C0}", pcStats[45]));
                        m_currentCharacter.Strength = Convert.ToInt32(pcStats[46]);
                        m_currentCharacter.Dexterity = Convert.ToInt32(pcStats[47]);
                        m_currentCharacter.Intelligence = Convert.ToInt32(pcStats[48]);
                        m_currentCharacter.Wisdom = Convert.ToInt32(pcStats[49]);
                        m_currentCharacter.Constitution = Convert.ToInt32(pcStats[50]);
                        m_currentCharacter.Charisma = Convert.ToInt32(pcStats[51]);
                        m_currentCharacter.StrengthAdd = Convert.ToInt32(pcStats[52]);
                        m_currentCharacter.DexterityAdd = Convert.ToInt32(pcStats[53]);
                        m_currentCharacter.m_encumbrance = Convert.ToDecimal(pcStats[54]);
                        m_currentCharacter.m_birthday = pcStats[55];
                        m_currentCharacter.lastOnline = pcStats[56];
                        m_currentCharacter.Karma = Convert.ToInt32(pcStats[57]);
                        m_currentCharacter.marks = Convert.ToInt32(pcStats[58]);
                        m_currentCharacter.PvPKills = Convert.ToInt64(pcStats[59]);
                        m_currentCharacter.PvPDeaths = Convert.ToInt64(pcStats[60]);
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
                        m_currentCharacter.ZName = pcStats[72];
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
                    m_currentCharacter.SkillsData = info;
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

                        if (string.IsNullOrEmpty(info) || pcInventory == null || pcInventory.Length <= 0)
                            break;

                        if (pcInventory[0].Length > 0)
                        {
                            for (a = 0; a < pcInventory.Length; a++)
                            {
                                Item inventoryItem = new Item(pcInventory[a]);
                                m_currentCharacter.Inventory.Add(inventoryItem);
                            }
                            GameHUD.UpdateInventoryWindow();
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
                        if (string.IsNullOrEmpty(info) || pcSack == null || pcSack.Length <= 0)
                            break;
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
                        if (string.IsNullOrEmpty(info) || pcPouch == null || pcPouch.Length <= 0)
                            break;
                        if (pcPouch[0].Length > 0)
                        {
                            for (a = 0; a < pcPouch.Length; a++)
                            {
                                Item pouchItem = new Item(pcPouch[a]);
                                m_currentCharacter.Pouch.Add(pouchItem);
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
                        if (string.IsNullOrEmpty(info) || pcBelt == null || pcBelt.Length <= 0)
                            break;
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
                        if (string.IsNullOrEmpty(info) || pcRings == null || pcRings.Length <= 0)
                            break;
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
                        if (string.IsNullOrEmpty(info) || pcLocker == null || pcLocker.Length <= 0)
                            break;
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
                        List<Spell> prevSpells = null;

                        if (Client.InGame && GameHUD.InitialSpellbookUpdated.Contains(CurrentCharacter.UniqueID))
                            prevSpells = new List<Spell>(CurrentCharacter.Spells);

                        CurrentCharacter.Spells.Clear();

                        string[] pcSpellbook = info.Split(Protocol.ISPLIT.ToCharArray());
                        if (string.IsNullOrEmpty(info) || pcSpellbook == null || pcSpellbook.Length <= 0)
                            break;
                        if (pcSpellbook[0].Length > 0)
                        {
                            for (a = 0; a < pcSpellbook.Length; a++)
                            {
                                string[] spellInfo = pcSpellbook[a].Split(Protocol.VSPLIT.ToCharArray());
                                Spell spell = World.GetSpellByID(Convert.ToInt32(spellInfo[0]));
                                if (spell != null)
                                {
                                    spell.Incantation = spellInfo[1];
                                    CurrentCharacter.Spells.Add(spell);
                                }
                            }
                        }

                        if (prevSpells != null && Client.InGame && CurrentCharacter.Spells.Count > prevSpells.Count &&
                            GameHUD.InitialSpellbookUpdated.Contains(CurrentCharacter.UniqueID))
                        {
                            Events.RegisterEvent(Events.EventName.New_Spell, prevSpells);
                        }

                        if (!GameHUD.InitialSpellbookUpdated.Contains(CurrentCharacter.UniqueID))
                            GameHUD.InitialSpellbookUpdated.Add(CurrentCharacter.UniqueID);
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                    }
                    break;
                #endregion
                case Enums.EPlayerUpdate.Talents:
                    #region Talents
                    try
                    {
                        List<Talent> prevTalents = null;

                        if (Client.InGame && GameHUD.InitialTalentbookUpdated.Contains(CurrentCharacter.UniqueID))
                            prevTalents = new List<Talent>(CurrentCharacter.Talents);

                        CurrentCharacter.Talents.Clear();

                        string[] pcTalentbook = info.Split(Protocol.ISPLIT.ToCharArray());

                        if (string.IsNullOrEmpty(info) || pcTalentbook == null || pcTalentbook.Length <= 0)
                            break;

                        if (pcTalentbook[0].Length > 0)
                        {
                            for (a = 0; a < pcTalentbook.Length; a++)
                            {
                                string[] talentInfo = pcTalentbook[a].Split(Protocol.VSPLIT.ToCharArray());
                                Talent talent = World.GetTalentByCommand(talentInfo[0]);
                                if (talent != null)
                                {
                                    talent.LastUse = Convert.ToDateTime(talentInfo[1]);
                                    CurrentCharacter.Talents.Add(talent);
                                }
                            }
                        }

                        if (prevTalents != null && Client.InGame && CurrentCharacter.Talents.Count > prevTalents.Count &&
                            GameHUD.InitialTalentbookUpdated.Contains(CurrentCharacter.UniqueID))
                        {
                            Events.RegisterEvent(Events.EventName.New_Talent, prevTalents);
                        }
                        else if (!GameHUD.InitialTalentbookUpdated.Contains(CurrentCharacter.UniqueID))
                            GameHUD.InitialTalentbookUpdated.Add(CurrentCharacter.UniqueID);
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                    }
                    break;
                #endregion
                case Enums.EPlayerUpdate.TalentUse: // Name, DateTime
                    string[] talentUseArray = info.Split(Protocol.VSPLIT.ToCharArray());
                    if (talentUseArray.Length >= 2)
                    {
                        if (CurrentCharacter != null && CurrentCharacter.Talents != null &&
                            CurrentCharacter.Talents.FindIndex(talent => talent.Name == talentUseArray[0]) is int index && index > -1)
                        {
                            // Next update UTC is used so below can be uncommented
                            //CurrentCharacter.Talents[index].LastUse = Convert.ToDateTime(talentUseArray[1]);
                            CurrentCharacter.Talents[index].LastUse = DateTime.UtcNow;// Convert.ToDateTime(talentUseArray[1]);
                        }
                    }
                    break;
                case Enums.EPlayerUpdate.Effects:
                    #region Effects
                    try
                    {
                        m_currentCharacter.Effects.Clear();
                        string[] pcEffects = info.Split(Protocol.ISPLIT.ToCharArray());
                        if (string.IsNullOrEmpty(info) || pcEffects == null || pcEffects.Length <= 0)
                            break;
                        if (pcEffects[0].Length > 0)
                        {
                            for (a = 0; a < pcEffects.Length; a++)
                                m_currentCharacter.Effects.Add(new Effect(pcEffects[a]));
                        }
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                    }
                    break;
                #endregion
                case Enums.EPlayerUpdate.WornEffects:
                    #region Worn Effects
                    try
                    {
                        m_currentCharacter.WornEffects.Clear();
                        string[] pcWornEffects = info.Split(Protocol.ISPLIT.ToCharArray());
                        if (string.IsNullOrEmpty(info) || pcWornEffects == null || pcWornEffects.Length <= 0)
                            break;

                        if (pcWornEffects[0].Length > 0)
                        {
                            for (a = 0; a < pcWornEffects.Length; a++)
                                m_currentCharacter.WornEffects.Add(new Effect(pcWornEffects[a]));
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

                    var macroList = new List<string>();

                    if (CurrentCharacter != null && CurrentCharacter.Macros != null)
                        CurrentCharacter.Macros.Clear();

                    foreach (string macro in macrosArray)
                        macroList.Add(macro);

                    m_currentCharacter.m_macros = new List<string>(macroList);

                    GenericSheet.LoadMacros();
                    break;
                #endregion
                case Enums.EPlayerUpdate.Resists:
                    m_currentCharacter.ResistsData = info;
                    break;
                case Enums.EPlayerUpdate.Protections:
                    m_currentCharacter.ProtectionsData = info;
                    break;
                case Enums.EPlayerUpdate.SkillRisk:
                    m_currentCharacter.SkillRisk = Convert.ToDouble(info);
                    SpellWarmingWindow.CreateCombatSkillRiskWindow();
                    break;
                case Enums.EPlayerUpdate.SkillExpChange:
                    if(Client.GameState == Enums.EGameState.YuushaGame && Utility.Settings.StaticSettings.DisplaySkillAmountChanges)
                    {
                        try
                        {
                            string[] expInfo = info.Split(Protocol.VSPLIT.ToCharArray());
                            long skillXPChange = Convert.ToInt64(expInfo[1]);
                            // set local skill experience here...
                            if(skillXPChange > 0)
                            {
                                //string.Format("+{0:n0}", skillXPChange)
                                TextCue.AddSkillXPGainTextCue(TextManager.FormatEnumString(expInfo[0]) + string.Format(" +{0:n0}", skillXPChange));
                            }
                            else if(skillXPChange < 0)
                            {
                                TextCue.AddSkillXPLossTextCue(TextManager.FormatEnumString(expInfo[0]) + string.Format(" {0:n0}", skillXPChange));
                            }
                            // experience change is expInfo[0]
                            //m_currentCharacter.Experience = Convert.ToInt64(expInfo[1]);
                        }
                        catch (Exception e)
                        {
                            Utils.LogException(e);
                        }
                    }
                    break;
            }
        }

        public static void LoadSettings()
        {
            m_characterSettings = CharacterSettings.Load();

            Events.RegisterEvent(Events.EventName.Load_Character_Settings);

            FogOfWarSettings = Utility.Settings.FogOfWarSettings.Load();

            GUIPositionSettings = Utility.Settings.GUIPositionSettings.Load();
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
        public double healthPercentage; // remaining
        public double staminaPercentage; // remaining
        public double manaPercentage; // remaining

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
        public int LandID;
        public int MapID;
        public int X
        { get; set; }
        public int Y
        { get; set; }
        public int Z
        { get; set; }
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

        private decimal m_encumbrance;
        private string m_birthday;
        public string lastOnline;
        public int Karma
        { get; set; }
        public int marks;
        public long PvPKills
        { get; set; }
        public long PvPDeaths
        { get; set; }
        public int[] playersKilled;
        public int[] playersFlagged;
        public bool knightRing;
        public string directionPointer;

        #region Private Data
        private int m_uniqueID;
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
        //private int m_strengthFull;
        private int m_dexterity;
        //private int m_dexterityFull;
        private int m_intelligence;
        //private int m_intelligenceFull;
        private int m_wisdom;
        //private int m_wisdomFull;
        private int m_constitution;
        //private int m_constitutionFull;
        private int m_charisma;
        //private int m_charismaFull;

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
        private List<Talent> m_talents;
        private List<Effect> m_effects;
        private List<Effect> m_wornEffects;
        private List<string> m_macros;
        #endregion

        #region Public Properties
        public int UniqueID
        {
            get { return m_uniqueID; }
            set { m_uniqueID = value; }
        }
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }
        public string MapName
        {
            get { return m_mapName; }
            private set
            {
                bool mapChange = value != m_mapName;

                m_mapName = value;

                // Called after the value change because of font choices in AddMapNameTextCue.
                if (mapChange && Client.InGame)
                    TextCue.AddMapNameTextCue(value);
            }
        }
        public string ZName
        {
            get { return m_zName; }
            private set
            {
                if (value != m_zName && Client.InGame)
                    TextCue.AddZNameTextCue(value);

                m_zName = value;
            }
        }
        private string m_zName = "";
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
            get { return m_classFullName; }
            set { m_classFullName = value; }
        }
        public ClassType Profession
        {
            get { return m_profession; }
            set { m_profession = value; }
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
        public string AgeDescription
        {
            get { return GetAgeDescription(); }
        }
        public long RoundsPlayed
        {
            get { return m_roundsPlayed; }
            set { m_roundsPlayed = value; }
        }
       
        public string ProtectionsData
        {
            get; set;
        }
        public string ResistsData
        {
            get; set;
        }
        public string SkillsData
        {
            get; set;
        }
        public double SkillRisk { get; set; }
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
            get { return m_alignment; }
            set { m_alignment = value; }
        }
        public string Location
        {
            get { return m_location; }
            set { m_location = value; }
        }
        public World.ImpLevel ImpLevel
        {
            get { return m_impLevel; }
            set { m_impLevel = value; }
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
            get { return m_rightHand; }
            set { m_rightHand = value; }
        }
        public Item LeftHand
        {
            get { return m_leftHand; }
            set { m_leftHand = value; }
        }
        public List<Item> Inventory
        {
            get { return m_inventory; }
        }
        public List<Item> Sack
        {
            get { return m_sack; }
        }
        public List<Item> Pouch
        {
            get { return m_pouch; }
        }
        public List<Item> Belt
        {
            get { return m_belt; }
        }
        public List<Item> Rings
        {
            get { return m_rings; }
        }
        public List<Item> Locker
        {
            get { return m_locker; }
        }
        public List<Spell> Spells
        {
            get { return m_spells; }
        }
        public List<Talent> Talents
        {
            get { return m_talents; }
        }
        public List<Effect> Effects
        {
            get { return m_effects; }
        }
        public List<Effect> WornEffects
        {
            get { return m_wornEffects; }
        }
        public List<string> Macros
        {
            get { return m_macros; }
        }
        public string Birthday
        { get { return m_birthday; } }
        public decimal Encumbrance
        { get { return m_encumbrance; } }
        public int HitsFull
        {
            get { return HitsMax + HitsAdjustment + HitsDoctored; }
        }
        public int ManaFull
        {
            get { return ManaMax + ManaAdjustment; }
        }
        public int StaminaFull
        {
            get { return StaminaMax + StaminaAdjustment; }
        }
        public bool IsManaUser
        {
            get
            {
                return Array.IndexOf(World.ManaUser, Profession) > -1;
            }
        }
        public bool HasSpellbook
        {
            get
            {
                return Array.IndexOf(World.SpellbookUser, Profession) > -1;
            }
        }
        public bool IsDead
        {
            get
            {
                return Hits <= 0;
            }
        }
        public bool IsHybrid
        {
            get
            {
                return Array.IndexOf(World.SpellbookUser, Profession) == -1 && Array.IndexOf(World.ManaUser, Profession) > -1;
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
        public bool HasFreeHand
        {
            get { return RightHand == null || LeftHand == null; }
        }
        public string WarmedSpell
        { get; set; }
        public int CurrentRoundsPlayed
        { get; set; } = 0;
        public string MemorizedSpell
        { get; set; }
        #endregion

        #region Constructor
        public Character()
        {
            m_visualKey = new VisualKey("unknown");
            m_rightHand = new Item();
            m_leftHand = new Item();
            m_inventory = new List<Item>();
            m_sack = new List<Item>();
            m_pouch = new List<Item>();
            m_belt = new List<Item>();
            m_rings = new List<Item>();
            m_locker = new List<Item>();
            m_spells = new List<Spell>();
            m_talents = new List<Talent>();
            m_effects = new List<Effect>();
            m_wornEffects = new List<Effect>();
            m_macros = new List<string>();

            m_location = "";
        }
        #endregion

        public int SackCountWithoutCoins()
        {
            int count = 0;

            foreach(Item item in Sack)
            {
                if (!item.Name.StartsWith("coin"))
                    count++;
            }

            return count;
        }

        public static bool HasEffect(string effectName)
        {
            if (GuiManager.GetControl("EffectsWindow") is Window effectsWindow)
            {
                foreach (Control label in effectsWindow.Controls)
                {
                    if (label.PopUpText.StartsWith(TextManager.FormatEnumString(effectName))) return true;
                }
            }

            if (GuiManager.GetControl("WornEffectsWindow") is Window wornEffectsWindow)
            {
                foreach (Control label in wornEffectsWindow.Controls)
                {
                    if (label.PopUpText.StartsWith(TextManager.FormatEnumString(effectName))) return true;
                }
            }

            return false;
        }

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
            return (Character)MemberwiseClone();
        }

        public void UpdateCoordinates(Cell cell)
        {
            bool statsRequested = false;

            LandID = cell.LandID;

            if (CurrentCharacter != null && CurrentCharacter.MapID != cell.MapID)
            {
                Events.RegisterEvent(Events.EventName.Request_Stats);
                statsRequested = true;
            }

            // Character has moved.
            if (X != cell.xCord || Y != cell.yCord || Z != cell.zCord || MapID != cell.MapID)
            {
                if (MapID != cell.MapID)
                    Events.RegisterEvent(Events.EventName.Character_Map_Changed, cell.MapID);
                Events.RegisterEvent(Events.EventName.Character_Moved);
            }

            MapID = cell.MapID;
            X = cell.xCord;
            Y = cell.yCord;

            if (!statsRequested && CurrentCharacter != null && CurrentCharacter.Z != cell.zCord)
                Events.RegisterEvent(Events.EventName.Request_Stats);

            Z = cell.zCord;
        }

        public Cell Cell
        {
            get
            {
                return Cell.GetCell(X, Y, CurrentCharacter.Z);
            }
        }

        public bool IsNextToCounter(out string locationName)
        {
            for (int ypos = -1; ypos <= 1; ypos += 1)
            {
                for (int xpos = -1; xpos <= 1; xpos += 1)
                {
                    Cell curCell = Cell.GetCell(X + xpos, Y + ypos, CurrentCharacter.Z);
                    if (curCell != null)
                    {
                        if (curCell.CellGraphic == Cell.GRAPHIC_COUNTER_PLACEABLE)
                        {
                            locationName = "counter";
                            return true;
                        }
                        else if(curCell.CellGraphic == Cell.GRAPHIC_ALTAR_PLACEABLE)
                        {
                            locationName = "altar";
                            return true;
                        }
                    }
                }
            }
            locationName = "";
            return false;
        }

        public bool IsNextToCounter(out Cell counterCell)
        {
            for (int ypos = -1; ypos <= 1; ypos += 1)
            {
                for (int xpos = -1; xpos <= 1; xpos += 1)
                {
                    Cell curCell = Cell.GetCell(X + xpos, Y + ypos, CurrentCharacter.Z);
                    if (curCell != null)
                    {
                        if (curCell.CellGraphic == Cell.GRAPHIC_COUNTER_PLACEABLE)
                        {
                            counterCell = curCell;
                            return true;
                        }
                        else if (curCell.CellGraphic == Cell.GRAPHIC_ALTAR_PLACEABLE)
                        {
                            counterCell = curCell;
                            return true;
                        }
                    }
                }
            }

            counterCell = null;
            return false;
        }

        private static string GetDirectionString(XYCoordinate beg, XYCoordinate end)
        {
            XYCoordinate dp = end - beg;
            string lhs = "", rhs = "";

            if (dp.Y == -1)
                lhs = "n";
            else if (dp.Y == 1)
                lhs = "s";

            if (dp.X == -1)
                rhs = "w";
            else if (dp.X == 1)
                rhs = "e";

            return lhs + rhs;
        }

        public Item GetRing(WearOrientation orientation)
        {
            foreach (Item item in Rings)
            {
              //Utils.LogOnce(Rings.Count + ":" + item.VisualKey + ", " + item.wearOrientation);
                if (item.WearOrientation == orientation)
                    return item;
            }

            return null;
        }

        public string GetAgeDescription()
        {
            int index = 0;

            if (Age < World.AgeCycles[0])
            {
                index = 0;
            }
            else if (Age >= World.AgeCycles[0] && this.Age < World.AgeCycles[1])
            {
                index = 1;
            }
            else if (Age >= World.AgeCycles[1] && this.Age < World.AgeCycles[2])
            {
                index = 2;
            }
            else if (Age >= World.AgeCycles[2] && this.Age < World.AgeCycles[3])
            {
                index = 3;
            }
            else if (Age >= World.AgeCycles[3] && this.Age < World.AgeCycles[4])
            {
                index = 4;
            }
            else
            {
                index = 5;
            }
            return World.age_humanoid[index];
        }

        public Item GetInventoryItem(WearLocation location, WearOrientation orientation)
        {
            foreach (Item item in Inventory)
            {
                if (item.WearLocation == location && item.WearOrientation == orientation)
                    return item;
            }

            return null;
        }

        public static List<Item> GetItemsList(Character chr, GridBoxWindow.GridBoxPurpose purpose)
        {
            switch(purpose)
            {
                case GridBoxWindow.GridBoxPurpose.Belt:
                    return chr.Belt;
                case GridBoxWindow.GridBoxPurpose.Locker:
                    return chr.Locker;
                case GridBoxWindow.GridBoxPurpose.Pouch:
                    return chr.Pouch;
                case GridBoxWindow.GridBoxPurpose.Rings:
                    return chr.Rings;
                case GridBoxWindow.GridBoxPurpose.Sack:
                    return chr.Sack;
                case GridBoxWindow.GridBoxPurpose.Altar:
                case GridBoxWindow.GridBoxPurpose.Counter:
                case GridBoxWindow.GridBoxPurpose.Ground:
                    return GameHUD.ExaminedCell.Items;
            }

            return new List<Item>();
        }
    }
}
