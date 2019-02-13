using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Yuusha.gui
{
    static public class IOKMode
    {
        #region Private Data
        private static string m_tileXMLFile = "";
        private static string[] m_critterListNames = new string[12];
        private static List<string> m_bufferedCommands = new List<string>();
        private static int m_maxBufferedCommands = 20;
        private static int m_bufferPreview = 0;
        private static Dictionary<string, IOKTileDefinition> m_tilesDict = new Dictionary<string, IOKTileDefinition>();
        private static string[] m_letters = new string[] {"A",
            "B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q",
			"R","S","T","U","V","W","X","Y","Z" };
        private static string[] m_alignment = new string[] { " ", " ", "!", "*", "+", " ", "+" };
        private static string m_usedLetters = "";
        private static List<Cell> m_cells = new List<Cell>();
        private static TimeSpan m_lastExpUpdate;
        private static TimeSpan m_lastHealthUpdate;
        private static TimeSpan m_lastStaminaUpdate;
        private static TimeSpan m_lastManaUpdate;
        #endregion

        #region Public Properties
        public static string TileXMLFile
        {
            get { return m_tileXMLFile; }
            set { m_tileXMLFile = value; }
        }
        public static List<string> BufferedCommands
        {
            get { return m_bufferedCommands; }
        }
        public static int BufferPreview
        {
            get { return m_bufferPreview; }
            set { m_bufferPreview = value; }
        }
        public static Dictionary<string, IOKTileDefinition> Tiles
        {
            get { return m_tilesDict; }
        }
        #endregion

        public static void DisplayGameText(string text, Enums.ETextType textType)
        {
            try
            {
                (gui.GuiManager.CurrentSheet["GameTextScrollableTextBox"] as gui.ScrollableTextBox).AddLine(text, textType);
                (gui.GuiManager.Sheets["SpinelGame"]["GameTextScrollableTextBox"] as gui.ScrollableTextBox).AddLine(text, textType);
            }
            catch (Exception e)
            {
                Utils.LogException(e);
            }
        }

        public static void UpdateGUI(GameTime gameTime, gui.Sheet sheet)
        {
            Character pre = Character.PreviousRoundCharacter;
            Character chr = Character.CurrentCharacter;

            try
            {
                if (chr != null)
                {
                    sheet["HitsAmountLabel"].Text = string.Format("{0}/{1}", chr.Hits, chr.HitsFull);
                    sheet["ExpAmountLabel"].Text = Convert.ToString(chr.Experience);
                    sheet["StaminaAmountLabel"].Text = Convert.ToString(chr.Stamina);
                    sheet["HitsTakenAmountLabel"].Text = Convert.ToString(chr.HitsFull - chr.Hits);

                    if (sheet["HitsTakenAmountLabel"].Text != "0")
                        sheet["HitsTakenAmountLabel"].TextColor = Color.DarkSalmon;
                    else sheet["HitsTakenAmountLabel"].TextColor = Color.White;

                    if (chr.IsSpellUser)
                    {
                        sheet["MagicPtsLabel"].IsVisible = true;
                        sheet["MagicPtsAmountLabel"].IsVisible = true;
                        sheet["MagicPtsAmountLabel"].Text = Convert.ToString(chr.Mana);
                    }
                    else
                    {
                        sheet["MagicPtsLabel"].IsVisible = false;
                        sheet["MagicPtsAmountLabel"].IsVisible = false;
                    }

                    if (chr.RightHand != null)
                    {
                        sheet["RHNameLabel"].Text = chr.RightHand.name;
                        if (chr.RightHand.nocked)
                        {
                            if (chr.RightHand.name.ToLower().Contains("crossbow"))
                                sheet["RHNameLabel"].Text += "*";
                            else if (chr.LeftHand == null)
                                sheet["LHNameLabel"].Text = "*";
                        }
                        else sheet["LHNameLabel"].Text = "";
                    }
                    else sheet["RHNameLabel"].Text = "";

                    if (chr.LeftHand != null)
                    {
                        sheet["LHNameLabel"].Text = chr.LeftHand.name;
                        if (chr.LeftHand.nocked)
                        {
                            if (chr.LeftHand.name.ToLower().Contains("crossbow"))
                                sheet["LHNameLabel"].Text += "*";
                            else if (chr.RightHand == null)
                                sheet["RHNameLabel"].Text = "*";
                        }
                    }
                    else if (sheet["LHNameLabel"].Text != "*")
                        sheet["LHNameLabel"].Text = "";

                    if (pre != null)
                    {
                        // Update health.
                        if (pre.Hits != chr.Hits && pre.Hits > 0)
                        {
                            if (pre.Hits > chr.Hits)
                            {
                                sheet["HitsAdjLabel"].Text = string.Format("-{0}", Convert.ToString(pre.Hits - chr.Hits));
                                sheet["HitsAdjLabel"].TextColor = Color.Red;
                            }
                            else if(chr.Hits > pre.Hits)
                            {
                                sheet["HitsAdjLabel"].Text = string.Format("+{0}", Convert.ToString(chr.Hits - pre.Hits));
                                sheet["HitsAdjLabel"].TextColor = Color.LimeGreen;
                            }
                        }
                        else if (gameTime == null || gameTime.TotalGameTime - m_lastHealthUpdate >= TimeSpan.FromSeconds(5))
                        {
                            sheet["HitsAdjLabel"].IsVisible = false;
                        }

                        // Update experience.
                        if (pre.Experience < chr.Experience && pre.Experience > 0)
                        {
                            sheet["GainedExpAmountLabel"].Text = Convert.ToString(chr.Experience - pre.Experience);

                            sheet["GainedExpLabel"].IsVisible = true;
                            sheet["GainedExpAmountLabel"].IsVisible = true;
                            m_lastExpUpdate = gameTime.TotalGameTime;
                        }
                        else if (gameTime == null || gameTime.TotalGameTime - m_lastExpUpdate >= TimeSpan.FromSeconds(5))
                        {
                            sheet["GainedExpLabel"].IsVisible = false;
                            sheet["GainedExpAmountLabel"].IsVisible = false;
                        }
                    }
                }

                // Place focus on the input text box.
                if (sheet[Globals.GAMEINPUTTEXTBOX] != null)
                {
                    // Options window is only other typing textboxes with focus while in IOK mode currently.
                    if (GuiManager.ControlWithFocus != null && (GuiManager.ControlWithFocus.Name != "OptionsWindow" || GuiManager.ControlWithFocus.Owner != "OptionsWindow"))
                    {
                        sheet[Globals.GAMEINPUTTEXTBOX].HasFocus = true;
                        GuiManager.ActiveTextBox = Globals.GAMEINPUTTEXTBOX;
                    }

                    if (GuiManager.ControlWithFocus != null && GuiManager.ControlWithFocus.Owner == "OptionsWindow")
                    {
                        sheet[Globals.GAMEINPUTTEXTBOX].HasFocus = true;
                    }
                }

                if (!Client.HasFocus)
                {
                    if (sheet[Globals.GAMEINPUTTEXTBOX] != null)
                    {
                        sheet[Globals.GAMEINPUTTEXTBOX].HasFocus = false;
                    }
                }
            }
            catch (Exception e)
            {
                Utils.LogException(e);
            }
        }

        public static void NewGameRound()
        {
            m_usedLetters = string.Empty;
            m_cells.Clear();
        }

        public static void EndGameRound()
        {
            BuildCritterList();
            BuildMap();
            Character.PreviousRoundCharacter = Character.CurrentCharacter.Clone();
        }

        private static string AssignCritterLetter()
        {
            foreach (string letter in m_letters)
            {
                if (m_usedLetters.IndexOf(letter) == -1)
                {
                    m_usedLetters += letter;
                    return letter;
                }
            }

            foreach (string letter in m_letters)
            {
                string str = letter.ToLower();
                if (m_usedLetters.IndexOf(str) == -1)
                {
                    m_usedLetters += str;
                    return str;
                }
            }

            return "Z";
        }

        public static void FormatCells(string inData)
        {
            try
            {
                Cell cell;

                if (inData.Length > 0)
                {
                    int a;

                    string cellInfo = Protocol.GetProtoInfoFromString(inData, Protocol.GAME_CELL_INFO, Protocol.GAME_CELL_INFO_END);
                    string critterInfo = Protocol.GetProtoInfoFromString(inData, Protocol.GAME_CELL_CRITTERS, Protocol.GAME_CELL_CRITTERS_END);
                    //string itemsInfo = Protocol.GetProtoInfoFromString(inData, Protocol.GAME_CELL_ITEMS, Protocol.GAME_CELL_ITEMS_END);
                    string effectsInfo = Protocol.GetProtoInfoFromString(inData, Protocol.GAME_CELL_EFFECTS, Protocol.GAME_CELL_EFFECTS_END);

                    cell = new Cell(cellInfo);

                    if (critterInfo.Length > 0)
                    {
                        string[] critters = critterInfo.Split(Protocol.USPLIT.ToCharArray());

                        for (a = 0; a < critters.Length; a++)
                        {
                            Character ch = FormatCellCritter(critters[a]);
                            if (ch != null)
                                cell.Add(ch);
                        }
                    }

                    if (effectsInfo.Length > 0)
                    {
                        string[] effects = effectsInfo.Split(Protocol.ISPLIT.ToCharArray());
                        for (a = 0; a < effects.Length; a++)
                        {
                            Effect effect = new Effect(effects[a]);
                            if (effect != null)
                                cell.Add(effect);
                        }
                    }
                }
                else cell = new Cell();

                if (m_cells.Count <= 49)
                    m_cells.Add(cell);
                else
                {
                    Utils.Log("Attempt to add more than 49 cells in IOKMode.FormatCells.m_Cells Count: " + m_cells.Count + " inData: " + inData);
                }
            }
            catch (Exception e)
            {
                Utils.LogException(e);
            }
        }

        private static Character FormatCellCritter(string inData)
        {
            Character crit = new Character();

            try
            {
                string[] critterInfo = Protocol.GetProtoInfoFromString(inData, Protocol.GAME_CRITTER_INFO, Protocol.GAME_CRITTER_INFO_END).Split(Protocol.VSPLIT.ToCharArray());

                //string[] critterInventory = Protocol.GetProtoInfoFromString(inData, Protocol.GAME_CRITTER_INVENTORY, Protocol.GAME_CRITTER_INVENTORY_END).Split(Protocol.ISPLIT.ToCharArray());

                int tempA = 0;

                crit.ID = Convert.ToInt32(critterInfo[tempA]); // player id or worldnpc id
                crit.isPC = crit.ID >= 0;
                crit.Name = critterInfo[tempA + 1];
                //crit.shortDesc = critterInfo[tempA + 2]; //
                //crit.longDesc = critterInfo[tempA + 3]; //
                crit.VisualKey = critterInfo[tempA + 2];
                if (crit.VisualKey == "") crit.VisualKey = crit.Name.ToLower().Replace(".", "_");
                //crit.Profession = (Character.ClassType)Convert.ToInt32(critterInfo[tempA + 5]); //
                crit.Alignment = (World.Alignment)Convert.ToInt32(critterInfo[tempA + 3]);
                //crit.Level = Convert.ToInt32(critterInfo[tempA + 7]); //
                //crit.Gender = (Character.GenderType)Convert.ToInt32(critterInfo[tempA + 8]); //
                //crit.Race = critterInfo[tempA + 9]; //
                //crit.Hits = Convert.ToInt32(critterInfo[tempA + 10]); //
                //crit.HitsMax = Convert.ToInt32(critterInfo[tempA + 11]); //

                if (critterInfo[tempA + 4].Length > 0)
                {
                    crit.RightHand.id = Convert.ToInt32(critterInfo[tempA + 4]);
                    crit.RightHand.name = critterInfo[tempA + 5];
                    crit.RightHand.visualKey = critterInfo[tempA + 6];
                    //crit.RightHand.longDesc = critterInfo[tempA + 14];
                }
                else
                {
                    crit.RightHand = null;
                }
                if (critterInfo.Length > tempA + 6 && critterInfo[tempA + 7].Length > 0)
                {
                    crit.LeftHand.id = Convert.ToInt32(critterInfo[tempA + 7]);
                    crit.LeftHand.name = critterInfo[tempA + 8];
                    crit.RightHand.visualKey = critterInfo[tempA + 9];
                    //crit.LeftHand.longDesc = critterInfo[tempA + 17]; //
                }
                else
                {
                    crit.LeftHand = null;
                }
                crit.visibleArmor = critterInfo[tempA + 10];
                return crit;
            }
            catch (Exception e)
            {
                Utils.LogException(e);
                return crit;
            }
        }

        private static Item FormatCellItem(string inData)
        {
            try
            {
                string[] itemInfo = inData.Split(Protocol.VSPLIT.ToCharArray());
                Item item = new Item
                {
                    id = Convert.ToInt32(itemInfo[0]),
                    name = itemInfo[1],
                    //item.longDesc = itemInfo[2];
                    visualKey = itemInfo[3],
                    //item.wearLocation = (Character.WearLocation)Convert.ToInt32(itemInfo[4]);
                    //item.attuneType = (Item.AttuneType)Convert.ToInt32(itemInfo[5]);
                    worldItemID = Convert.ToInt32(itemInfo[4])
                };
                return item;
            }
            catch (Exception e)
            {
                Utils.LogException(e);
                return null;
            }
        }

        private static void BuildCritterList()
        {
            try
            {
                if (m_cells.Count > 0)
                {
                    int labelNum = 0;

                    for (int a = 0; a < 12; a++)
                    {
                        GuiManager.GetControl("CritterList" + a.ToString()).IsVisible = false;
                        m_critterListNames[a] = "";
                    }

                    Cell cell = m_cells[24]; // start with our cell

                    if (cell.Characters.Count > 0)
                    {
                        foreach (Character ch in cell.Characters)
                        {
                            #region Create Critter Label (Center Cell)
                            string critterInfo = "  " + m_alignment[(int)ch.Alignment] + ch.Name;
                            critterInfo = critterInfo.PadRight(19);

                            if (ch.RightHand != null)
                                critterInfo += ch.RightHand.name;

                            critterInfo = critterInfo.PadRight(31);

                            if (ch.LeftHand != null)
                                critterInfo += ch.LeftHand.name;

                            critterInfo = critterInfo.PadRight(43);

                            critterInfo += ch.visibleArmor;

                            Color foreColor = Color.White;
                            Color backColor = Color.Black;

                            switch (ch.Alignment)
                            {
                                case World.Alignment.Amoral:
                                    foreColor = Client.UserSettings.Color_Gui_Amoral_Fore;
                                    backColor = Client.UserSettings.Color_Gui_Amoral_Back;
                                    break;
                                case World.Alignment.Chaotic:
                                    foreColor = Client.UserSettings.Color_Gui_Chaotic_Fore;
                                    backColor = Client.UserSettings.Color_Gui_Chaotic_Back;
                                    break;
                                case World.Alignment.ChaoticEvil:
                                    foreColor = Client.UserSettings.Color_Gui_ChaoticEvil_Fore;
                                    backColor = Client.UserSettings.Color_Gui_ChaoticEvil_Back;
                                    break;
                                case World.Alignment.Evil:
                                    foreColor = Client.UserSettings.Color_Gui_Evil_Fore;
                                    backColor = Client.UserSettings.Color_Gui_Evil_Back;
                                    break;
                                case World.Alignment.Lawful:
                                    foreColor = Client.UserSettings.Color_Gui_Lawful_Fore;
                                    backColor = Client.UserSettings.Color_Gui_Lawful_Back;
                                    break;
                                case World.Alignment.Neutral:
                                    foreColor = Client.UserSettings.Color_Gui_Neutral_Fore;
                                    backColor = Client.UserSettings.Color_Gui_Neutral_Back;
                                    break;
                            }

                            m_critterListNames[labelNum] = ch.Name;
                            CritterListLabel label = GuiManager.GetControl("CritterList" + labelNum.ToString()) as CritterListLabel;
                            label.Critter = ch;
                            label.CenterCell = true;
                            label.Text = critterInfo;
                            label.TextColor = foreColor;
                            label.TintColor = backColor;
                            label.IsVisible = true;
                            #endregion

                            labelNum++;

                            if (labelNum >= 12)
                                break;
                        }
                    }


                    for (int a = 0; a < m_cells.Count; a++) // move through each cell and update the map and mobs list
                    {
                        if (labelNum >= m_critterListNames.Length - 1)
                            break;

                        #region Create Critter Label (Non Center Cells)
                        if (a != 24)
                        {
                            cell = m_cells[a];

                            if (cell.Characters.Count > 0)
                            {
                                string letter = AssignCritterLetter();
                                foreach (Character ch in cell.Characters)
                                {
                                    ch.assignedLetter = letter;
                                    string critterInfo = ch.assignedLetter + " " + m_alignment[(int)ch.Alignment] + ch.Name;

                                    Color foreColor = Color.White;
                                    Color backColor = Color.Black;

                                    switch (ch.Alignment)
                                    {
                                        case World.Alignment.Amoral:
                                            foreColor = Client.UserSettings.Color_Gui_Amoral_Fore;
                                            backColor = Client.UserSettings.Color_Gui_Amoral_Back;
                                            break;
                                        case World.Alignment.Chaotic:
                                            foreColor = Client.UserSettings.Color_Gui_Chaotic_Fore;
                                            backColor = Client.UserSettings.Color_Gui_Chaotic_Back;
                                            break;
                                        case World.Alignment.ChaoticEvil:
                                            foreColor = Client.UserSettings.Color_Gui_ChaoticEvil_Fore;
                                            backColor = Client.UserSettings.Color_Gui_ChaoticEvil_Back;
                                            break;
                                        case World.Alignment.Evil:
                                            foreColor = Client.UserSettings.Color_Gui_Evil_Fore;
                                            backColor = Client.UserSettings.Color_Gui_Evil_Back;
                                            break;
                                        case World.Alignment.Lawful:
                                            foreColor = Client.UserSettings.Color_Gui_Lawful_Fore;
                                            backColor = Client.UserSettings.Color_Gui_Lawful_Back;
                                            break;
                                        case World.Alignment.Neutral:
                                            foreColor = Client.UserSettings.Color_Gui_Neutral_Fore;
                                            backColor = Client.UserSettings.Color_Gui_Neutral_Back;
                                            break;
                                    }

                                    m_critterListNames[labelNum] = ch.Name;
                                    CritterListLabel label = GuiManager.GetControl("CritterList" + labelNum.ToString()) as CritterListLabel;
                                    label.Critter = ch;
                                    label.CenterCell = false;
                                    label.Text = critterInfo;
                                    label.TextColor = foreColor;
                                    label.TintColor = backColor;
                                    label.IsVisible = true;
                                    labelNum++;

                                    if (labelNum >= 12)
                                        break;
                                }
                            }
                        }
                        #endregion
                    }
                }
            }
            catch (Exception e)
            {
                Utils.LogException(e);
            }
        }

        private static void BuildMap()
        {
            try
            {
                if (m_cells.Count > 0)
                {
                    IOKTileLabel label = null;
                    Cell cell = null;
                    VisualInfo backVI = GuiManager.Visuals["WhiteSpace"];
                    IOKTileDefinition currTile = null;

                    for (int count = 0; count < m_cells.Count; count++) // move through each cell and update the map and mobs list
                    {
                        cell = m_cells[count];

                        label = GuiManager.GetControl(Enums.EGameState.IOKGame.ToString(), "Tile" + count.ToString()) as IOKTileLabel;

                        if (label == null)
                            continue;

                        label.CreatureText = ""; // clear creature text;
                        label.LootText = ""; // clear loot text;

                        if (cell.visible)
                        {
                            if (m_tilesDict.ContainsKey(cell.displayGraphic))
                            {
                                currTile = m_tilesDict[cell.displayGraphic];
                            }
                            else
                            {
                                Utils.LogOnce("Failed to find IOKTile for cell graphic [ " + cell.displayGraphic + " ]");
                                currTile = m_tilesDict["  "];
                            }

                            label.Text = currTile.DisplayGraphic;
                            label.TextColor = currTile.ForeColor;
                            label.TintColor = currTile.BackColor;
                            label.TextAlpha = currTile.ForeAlpha;
                            label.VisualAlpha = currTile.BackAlpha;

                            if (cell.Characters.Count > 0)
                            {
                                if (count != 24) // not our cell
                                {
                                    label.CreatureText = cell.Characters[0].assignedLetter;
                                }
                                else
                                {
                                    label.CreatureText = Character.CurrentCharacter.directionPointer + " ";
                                }
                            }
                            else if(count == 24)
                            {
                                label.CreatureText = Character.CurrentCharacter.directionPointer + " ";
                            }

                            if (cell.hasItems && cell.IsLootVisible)
                            {
                                label.LootText = " $";
                            }
                        }
                        else
                        {
                            if (m_tilesDict.ContainsKey("  "))
                                currTile = m_tilesDict["  "];
                            else
                            {
                                Utils.LogOnce("Failed to find IOKTile for cell graphic [    ]");
                                continue;
                            }

                            label.Text = currTile.DisplayGraphic;
                            label.TextColor = currTile.ForeColor;
                            label.TintColor = currTile.BackColor;
                            label.TextAlpha = currTile.ForeAlpha;
                            label.VisualAlpha = currTile.BackAlpha;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utils.LogException(e);
            }
        }

        public static void AddBufferedCommand(string text)
        {
            if (m_bufferedCommands.Count == m_maxBufferedCommands)
                m_bufferedCommands.RemoveAt(0);

            m_bufferedCommands.Add(text);
            m_bufferPreview = m_bufferedCommands.Count - 1;
        }
    }
}
