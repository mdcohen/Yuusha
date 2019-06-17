using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Yuusha.gui
{
    static public class YuushaMode
    {
        #region Private Data
        private static string m_tileXMLFile = "";
        private static Dictionary<string, SpinelTileDefinition> m_tilesDict = new Dictionary<string, SpinelTileDefinition>();
        private static List<Cell> m_cells = new List<Cell>();
        public static List<Cell> Cells
        {
            get { return m_cells; }
        }
        private static string[] m_critterListNames = new string[12];
        private static string[] m_letters = new string[] {"A",
            "B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q",
            "R","S","T","U","V","W","X","Y","Z" };

        private static string[] m_alignment = new string[] { " ", " ", "!", "*", "+", " ", "+" };
        private static string m_usedLetters = "";
        private static List<string> m_bufferedCommands = new List<string>();
        private static int m_maxBufferedCommands = 20;
        private static int m_bufferPreview = 0;
        private static TimeSpan m_lastExpUpdate;
        #endregion

        #region Properties
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
        public static Dictionary<string, SpinelTileDefinition> Tiles
        {
            get { return m_tilesDict; }
        }
        #endregion

        public static void DisplayGameText(string text, Enums.ETextType textType)
        {
            try
            {
                (GuiManager.CurrentSheet["GameTextScrollableTextBox"] as gui.ScrollableTextBox).AddLine(text, textType);
                (GuiManager.Sheets["IOKGame"]["GameTextScrollableTextBox"] as gui.ScrollableTextBox).AddLine(text, textType);
                (GuiManager.Sheets["SpinelGame"]["GameTextScrollableTextBox"] as gui.ScrollableTextBox).AddLine(text, textType);
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
                    sheet["ExpAmountLabel"].Text = string.Format("{0:n0}", chr.Experience);
                    sheet["StaminaAmountLabel"].Text = string.Format("{0}/{1}", chr.Stamina, chr.StaminaFull);
                    sheet["HitsTakenAmountLabel"].Text = Convert.ToString(chr.HitsFull - chr.Hits);

                    if (sheet["HitsTakenAmountLabel"].Text != "0")
                    {
                        sheet["HitsTakenAmountLabel"].TextColor = Color.DarkSalmon;
                    }
                    else
                    {
                        sheet["HitsTakenAmountLabel"].TextColor = Color.Plum;
                    }


                    #region Magic Points Labels
                    if (chr.IsSpellUser)
                    {

                        sheet["MagicPtsLabel"].IsVisible = true;
                        sheet["MagicPtsAmountLabel"].IsVisible = true;
                        string.Format("{0}/{1}", chr.Mana, chr.ManaFull);
                        sheet["MagicPtsAmountLabel"].Text = string.Format("{0}/{1}", chr.Mana, chr.ManaFull);
                    }
                    else
                    {
                        sheet["MagicPtsLabel"].IsVisible = false;
                        sheet["MagicPtsAmountLabel"].IsVisible = false;
                    }
                    #endregion

                    #region Right Hand Update
                    if (chr.RightHand != null)
                    {
                        (sheet["RHNameDragAndDropButton"] as DragAndDropButton).IsTextVisible = false;
                        sheet["RHNameDragAndDropButton"].PopUpText = chr.RightHand.Name;
                        sheet["RHNameDragAndDropButton"].VisualKey = chr.RightHand.VisualKey;
                        sheet["RHNameDragAndDropButton"].TintColor = Color.White;
                        if (chr.RightHand.nocked)
                        {
                            if (chr.RightHand.Name.ToLower().Contains("crossbow"))
                            {
                                sheet["RHNameDragAndDropButton"].Text += "*";
                                (sheet["RHNameDragAndDropButton"] as DragAndDropButton).IsTextVisible = true;
                            }
                            else if (chr.LeftHand == null)
                            {
                                sheet["LHNameDragAndDropButton"].Text = "*";
                                (sheet["LHNameDragAndDropButton"] as DragAndDropButton).IsTextVisible = true;
                            }
                        }
                    }
                    else
                    {
                        sheet["RHNameDragAndDropButton"].Text = "empty";
                        (sheet["RHNameDragAndDropButton"] as DragAndDropButton).IsTextVisible = true;
                        sheet["RHNameDragAndDropButton"].VisualKey = "WhiteSpace";
                        sheet["RHNameDragAndDropButton"].TintColor = Color.Black;
                    }
                    #endregion

                    #region Left Hand Update
                    if (chr.LeftHand != null)
                    {
                        (sheet["LHNameDragAndDropButton"] as DragAndDropButton).IsTextVisible = false;
                        sheet["LHNameDragAndDropButton"].PopUpText = chr.LeftHand.Name;
                        sheet["LHNameDragAndDropButton"].VisualKey = chr.LeftHand.VisualKey;
                        sheet["LHNameDragAndDropButton"].TintColor = Color.White;
                        if (chr.LeftHand.nocked)
                        {
                            if (chr.LeftHand.Name.ToLower().Contains("crossbow"))
                            {
                                sheet["LHNameDragAndDropButton"].Text = "*";
                                (sheet["LHNameDragAndDropButton"] as DragAndDropButton).IsTextVisible = true;
                            }
                            else if (chr.RightHand == null)
                            {
                                sheet["RHNameDragAndDropButton"].Text = "*";
                                (sheet["RHNameDragAndDropButton"] as DragAndDropButton).IsTextVisible = true;
                            }
                        }
                    }
                    else
                    {
                        sheet["LHNameDragAndDropButton"].Text = "empty";
                        (sheet["LHNameDragAndDropButton"] as DragAndDropButton).IsTextVisible = true;
                        sheet["LHNameDragAndDropButton"].VisualKey = "WhiteSpace";
                        sheet["LHNameDragAndDropButton"].TintColor = Color.Black;
                    }
                    #endregion

                    if (pre != null)
                    {
                        // hits update (if pre.hits is greater than 0, in other words not first update)
                        if (pre.Hits != chr.Hits && pre.Hits > 0)
                        {
                            if (pre.Hits > chr.Hits)
                            {
                                sheet["HitsAdjLabel"].Text = string.Format("-{0}", Convert.ToString(pre.Hits - chr.Hits));
                                sheet["HitsAdjLabel"].TextColor = Color.Red;
                            }
                            else
                            {
                                sheet["HitsAdjLabel"].Text = string.Format("+{0}", Convert.ToString(chr.Hits - pre.Hits));
                                sheet["HitsAdjLabel"].TextColor = Color.LimeGreen;
                            }

                            sheet["HitsAdjLabel"].IsVisible = true;
                        }
                        else sheet["HitsAdjLabel"].IsVisible = false;

                        // experience update (if pre.exp is greater than 0, in other words not first update)
                        if (pre.Experience < chr.Experience && pre.Experience > 0)
                        {
                            sheet["GainedExpAmountLabel"].Text = string.Format("{0:n0}", chr.Experience - pre.Experience);

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

                // Overrides to focus on input text box.
                // Options window and private messages have focus priority.
                if (!GuiManager.GenericSheet["OptionsWindow"].IsVisible && (GuiManager.ControlWithFocus == null || !GuiManager.ControlWithFocus.Name.Contains("PrivateMessage")))
                    sheet[Globals.GAMEINPUTTEXTBOX].HasFocus = true;

                if (!Client.HasFocus)
                {
                    if (sheet[Globals.GAMEINPUTTEXTBOX] != null)
                        sheet[Globals.GAMEINPUTTEXTBOX].HasFocus = false;

                    GuiManager.ActiveTextBox = null;
                    GuiManager.ControlWithFocus = null;
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

        public static void FormatCell(string inData)
        {
            try
            {
                Cell cell;

                if (inData.Length > 0)
                {
                    int a;

                    string cellInfo = Protocol.GetProtoInfoFromString(inData, Protocol.GAME_CELL_INFO, Protocol.GAME_CELL_INFO_END);
                    string critterInfo = Protocol.GetProtoInfoFromString(inData, Protocol.GAME_CELL_CRITTERS, Protocol.GAME_CELL_CRITTERS_END);
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

                m_cells.Add(cell);
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
                //crit.shortDesc = critterInfo[tempA + 2];
                //crit.longDesc = critterInfo[tempA + 3];
                crit.VisualKey = critterInfo[tempA + 2];
                if (crit.VisualKey == "")
                {
                    crit.VisualKey = crit.Name.ToLower().Replace(".", "_");
                    //Utils.LogOnce(crit.Name + " has no visualKey information.");
                    //crit.VisualKey = crit.Gender.ToString().ToLower() + "_" + crit.Profession.ToString().ToLower() + "_npc";
                    //crit.VisualKey = crit.Profession.Name.ToLower().Replace(".", "_");
                }
                //crit.Profession = (Character.ClassType)Convert.ToInt32(critterInfo[tempA + 5]);
                // change visual key to thief if critter is a thief
                //if (crit.Profession == Character.ClassType.Thief && crit.VisualKey.ToLower().Contains("fighter"))
                //   crit.VisualKey = crit.VisualKey.Replace("fighter", "thief");
                crit.Alignment = (World.Alignment)Convert.ToInt32(critterInfo[tempA + 3]);
                //crit.Level = Convert.ToInt32(critterInfo[tempA + 7]);
                //crit.Gender = (Character.GenderType)Convert.ToInt32(critterInfo[tempA + 8]);
                //crit.Race = critterInfo[tempA + 9];
                //crit.Hits = Convert.ToInt32(critterInfo[tempA + 10]);
                //crit.HitsMax = Convert.ToInt32(critterInfo[tempA + 11]);

                if (critterInfo[tempA + 4].Length > 0)
                {
                    crit.RightHand.ID = Convert.ToInt32(critterInfo[tempA + 4]);
                    crit.RightHand.Name = critterInfo[tempA + 5];
                    crit.RightHand.VisualKey = critterInfo[tempA + 6];
                }
                else
                {
                    crit.RightHand = null;
                }
                if (critterInfo[tempA + 7].Length > 0)
                {
                    crit.LeftHand.ID = Convert.ToInt32(critterInfo[tempA + 7]);
                    crit.LeftHand.Name = critterInfo[tempA + 8];
                    crit.LeftHand.VisualKey = critterInfo[tempA + 9];
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
                Item item = new Item();
                item.ID = Convert.ToInt32(itemInfo[0]);
                item.Name = itemInfo[1];
                //item.longDesc = itemInfo[2];
                item.VisualKey = itemInfo[3];
                //item.wearLocation = (Character.WearLocation)Convert.ToInt32(itemInfo[4]);
                //item.attuneType = (Item.AttuneType)Convert.ToInt32(itemInfo[5]);
                item.WorldItemID = Convert.ToInt32(itemInfo[4]);
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
                        Control critterList = GuiManager.GetControl("CritterList" + a.ToString());
                        if (critterList != null)
                            critterList.IsVisible = false;
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
                                critterInfo += ch.RightHand.Name;

                            critterInfo = critterInfo.PadRight(31);

                            if (ch.LeftHand != null)
                                critterInfo += ch.LeftHand.Name;

                            critterInfo = critterInfo.PadRight(43);

                            critterInfo += ch.visibleArmor;

                            Color foreColor = Color.White;
                            Color backColor = Color.Black;

                            switch (ch.Alignment)
                            {
                                case World.Alignment.Amoral:
                                    foreColor = Client.ClientSettings.Color_Gui_Amoral_Fore;
                                    backColor = Client.ClientSettings.Color_Gui_Amoral_Back;
                                    break;
                                case World.Alignment.Chaotic:
                                    foreColor = Client.ClientSettings.Color_Gui_Chaotic_Fore;
                                    backColor = Client.ClientSettings.Color_Gui_Chaotic_Back;
                                    break;
                                case World.Alignment.ChaoticEvil:
                                    foreColor = Client.ClientSettings.Color_Gui_ChaoticEvil_Fore;
                                    backColor = Client.ClientSettings.Color_Gui_ChaoticEvil_Back;
                                    break;
                                case World.Alignment.Evil:
                                    foreColor = Client.ClientSettings.Color_Gui_Evil_Fore;
                                    backColor = Client.ClientSettings.Color_Gui_Evil_Back;
                                    break;
                                case World.Alignment.Lawful:
                                    foreColor = Client.ClientSettings.Color_Gui_Lawful_Fore;
                                    backColor = Client.ClientSettings.Color_Gui_Lawful_Back;
                                    break;
                                case World.Alignment.Neutral:
                                    foreColor = Client.ClientSettings.Color_Gui_Neutral_Fore;
                                    backColor = Client.ClientSettings.Color_Gui_Neutral_Back;
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
                        if (labelNum > m_critterListNames.Length - 1)
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
                                            foreColor = Client.ClientSettings.Color_Gui_Amoral_Fore;
                                            backColor = Client.ClientSettings.Color_Gui_Amoral_Back;
                                            break;
                                        case World.Alignment.Chaotic:
                                            foreColor = Client.ClientSettings.Color_Gui_Chaotic_Fore;
                                            backColor = Client.ClientSettings.Color_Gui_Chaotic_Back;
                                            break;
                                        case World.Alignment.ChaoticEvil:
                                            foreColor = Client.ClientSettings.Color_Gui_ChaoticEvil_Fore;
                                            backColor = Client.ClientSettings.Color_Gui_ChaoticEvil_Back;
                                            break;
                                        case World.Alignment.Evil:
                                            foreColor = Client.ClientSettings.Color_Gui_Evil_Fore;
                                            backColor = Client.ClientSettings.Color_Gui_Evil_Back;
                                            break;
                                        case World.Alignment.Lawful:
                                            foreColor = Client.ClientSettings.Color_Gui_Lawful_Fore;
                                            backColor = Client.ClientSettings.Color_Gui_Lawful_Back;
                                            break;
                                        case World.Alignment.Neutral:
                                            foreColor = Client.ClientSettings.Color_Gui_Neutral_Fore;
                                            backColor = Client.ClientSettings.Color_Gui_Neutral_Back;
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
                    SpinelTileLabel spLabel;
                    Cell cell;
                    VisualInfo backVI = GuiManager.Visuals["WhiteSpace"];
                    SpinelTileDefinition currTile;

                    for (int count = 0; count < m_cells.Count; count++) // move through each cell and update the map and mobs list
                    {
                        cell = m_cells[count];

                        //spLabel = GuiManager.GetControl("Tile" + count.ToString()) as SpinelTileLabel;
                        spLabel = GuiManager.GetControl(Enums.EGameState.YuushaGame.ToString(), "Tile" + count.ToString()) as SpinelTileLabel;

                        if (spLabel == null)
                            continue;

                        spLabel.CritterVisuals.Clear();
                        spLabel.LootVisual = "";
                        spLabel.CreatureText = "";

                        if (cell.visible)
                        {
                            if (m_tilesDict.ContainsKey(cell.DisplayGraphic))
                                currTile = m_tilesDict[cell.DisplayGraphic];
                            else
                            {
                                Utils.LogOnce("Failed to find SpinelTileDefinition for cell graphic [ " + cell.DisplayGraphic + " ]");
                                currTile = m_tilesDict["  "];
                            }

                            spLabel.Text = "";
                            spLabel.TextColor = Color.White;
                            spLabel.TintColor = currTile.BackTint;
                            spLabel.TextAlpha = 255;
                            try
                            {
                                spLabel.VisualKey = currTile.BackVisual.Key;
                            }
                            catch (Exception e) // troubleshooting 10/19/2013
                            {
                                Utils.LogException(e);
                                Utils.Log("spLabel = " + spLabel.Name + " currTile = " + currTile.Name);
                            }
                            spLabel.VisualAlpha = currTile.BackAlpha;
                            spLabel.ForeVisual = currTile.ForeVisual.Key;
                            spLabel.ForeColor = currTile.ForeTint;
                            spLabel.ForeAlpha = currTile.ForeAlpha;

                            if (cell.portal)
                                spLabel.VisualKey = m_tilesDict["pp"].ForeVisual.Key;

                            if (cell.Characters.Count > 0)
                            {
                                if (count != 24) // not our cell
                                {
                                    spLabel.CritterVisuals.Add(new VisualKey(cell.Characters[0].VisualKey));
                                    spLabel.CreatureText = cell.Characters[0].assignedLetter;
                                }
                                else
                                {
                                    spLabel.CritterVisuals.Add(new VisualKey(cell.Characters[0].VisualKey));
                                    if (!Character.CurrentCharacter.IsPeeking)
                                    {
                                        spLabel.CritterVisuals.Add(new VisualKey(Character.CurrentCharacter.VisualKey));
                                        Character.CurrentCharacter.UpdateCoordinates(cell);
                                    }
                                }
                            }
                            else if (count == 24)
                            {
                                spLabel.CritterVisuals.Add(new VisualKey(Character.CurrentCharacter.VisualKey));
                                Character.CurrentCharacter.UpdateCoordinates(cell);
                            }

                            if (cell.hasItems && cell.IsLootVisible)
                            {
                                spLabel.LootVisual = m_tilesDict[" $"].ForeVisual.Key;
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

                            spLabel.Text = "";// currTile.DisplayGraphic;
                            spLabel.TextColor = Color.White;// currTile.ForeColor;
                            spLabel.TintColor = currTile.BackTint;
                            spLabel.TextAlpha = 255;
                            spLabel.VisualKey = currTile.BackVisual.Key;
                            spLabel.VisualAlpha = currTile.BackAlpha;
                            spLabel.ForeVisual = currTile.ForeVisual.Key;
                            spLabel.ForeColor = currTile.ForeTint;
                            spLabel.ForeAlpha = currTile.ForeAlpha;
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
            // Don't add if the command is already the last one sent.
            if (m_bufferedCommands.Count >= 1 && m_bufferedCommands[m_bufferedCommands.Count - 1] == text)
                return;

            if (m_bufferedCommands.Count == m_maxBufferedCommands)
                m_bufferedCommands.RemoveAt(0);

            m_bufferedCommands.Add(text);
            m_bufferPreview = m_bufferedCommands.Count - 1;
        }
    }
}
