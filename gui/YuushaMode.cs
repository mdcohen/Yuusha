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
        private static string[] m_critterListNames = new string[20];
        private static readonly string[] m_letters = new string[] {"A",
            "B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q",
            "R","S","T","U","V","W","X","Y","Z" };

        private static readonly string[] m_alignment = new string[] { " ", " ", "!", "*", "+", " ", "+" };
        private static string m_usedLetters = "";
        private static List<string> m_bufferedCommands = new List<string>();
        private static readonly int m_maxBufferedCommands = 20;
        private static int m_bufferPreview = 0;
       // private static TimeSpan m_lastExpUpdate;
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
                (GuiManager.CurrentSheet["GameTextScrollableTextBox"] as ScrollableTextBox).AddLine(text, textType);
                (GuiManager.Sheets["IOKGame"]["GameTextScrollableTextBox"] as ScrollableTextBox).AddLine(text, textType);
                (GuiManager.Sheets["SpinelGame"]["GameTextScrollableTextBox"] as ScrollableTextBox).AddLine(text, textType);

                TextManager.CheckYuushaModeTextTriggers(text);

                if (Client.ClientSettings.DisplayConversationBubbles)
                {
                    if ((text.Contains(":") && text.IndexOf(" ") == text.IndexOf(":") + 1) || text.StartsWith("You say: "))
                    {
                        string[] f = text.Split(":".ToCharArray());
                        string[] g = f[1].Trim().Split(" ".ToCharArray());

                        bool magicChant = g.Length >= 4 && TextManager.MagicWords.Contains(g[0]) && TextManager.MagicWords.Contains(g[1]) && TextManager.MagicWords.Contains(g[2]) && TextManager.MagicWords.Contains(g[3]);

                        if (!magicChant || (magicChant && Client.ClientSettings.DisplayChantingConversationBubble))
                        {
                            int cellCount = 0;
                            bool done = false;
                            foreach (Cell cell in GameHUD.Cells)
                            {
                                foreach (Character chr in cell.Characters)
                                {
                                    if (chr.Name == f[0] || (f[0] == "You say" && cellCount == 24))
                                    {
                                        // IsVisible added because the control may be temporarily hidden.
                                        if (GuiManager.GetControl("Tile" + cellCount) is SpinelTileLabel lbl && lbl.IsVisible)
                                        {
                                            int x = lbl.Position.X + 10;
                                            int y = lbl.Position.Y - 20;
                                            if (f[0] == "You say")
                                            {
                                                x = lbl.Position.X + lbl.Height / 2;
                                                y = lbl.Position.Y + lbl.Height / 2;
                                            }

                                            BitmapFont bmf = BitmapFont.ActiveFonts["lemon12"];

                                            // add pop up 
                                            ScrollableTextBox scr = new ScrollableTextBox(chr.UniqueID + "_" + Program.Client.ClientGameTime.TotalGameTime + "_ScrollableTextBox", "",
                                                new Rectangle(x, y, 300, bmf.LineHeight + 5), "", TextManager.GetAlignmentColor(false, chr.Alignment), true, false, "lemon12",
                                                new VisualKey("ConversationBubble"), Color.FloralWhite, 255, 255, new VisualKey(""), new VisualKey(""), new VisualKey(""), 5, 5,
                                                BitmapFont.TextAlignment.Left, new List<Enums.EAnchorType>() { Enums.EAnchorType.Center }, true)
                                            {
                                                
                                                Colorize = false,
                                                MouseInvisible = true // mousehandling beneath this control
                                            };

                                            foreach (Tuple<ScrollableTextBox, DateTime> tuple in new List<Tuple<ScrollableTextBox, DateTime>>(GameHUD.ConversationBubbles))
                                            {
                                                int whileLoopCount = 0;
                                                Rectangle existingRect = new Rectangle(tuple.Item1.Position.X, tuple.Item1.Position.Y, tuple.Item1.Width, tuple.Item1.Height);
                                                Rectangle scrRect = new Rectangle(scr.Position.X, scr.Position.Y, scr.Width, scr.Height);

                                                while (whileLoopCount < 20 && scrRect.Intersects(existingRect))
                                                {
                                                    if (existingRect.X == scrRect.X)
                                                        scr.Position = new Point(scrRect.X, existingRect.Y + existingRect.Height + 2);
                                                    else if (existingRect.X > scrRect.X)
                                                        scr.Position = new Point(existingRect.X - scrRect.Width - 2, scrRect.Y);
                                                    else if (existingRect.X < scrRect.X)
                                                        scr.Position = new Point(existingRect.X + existingRect.Width + 2, scrRect.Y);

                                                    whileLoopCount++;
                                                }
                                            }

                                            GuiManager.CurrentSheet.AddControl(scr);
                                            scr.AddLine(f[1].Trim(), Enums.ETextType.Default);
                                            scr.Height = scr.FormattedLinesCount * (bmf.LineHeight + 5);
                                            scr.Width = scr.LongestFormattedLine() + 10;
                                            GameHUD.ConversationBubbles.Add(Tuple.Create(scr, DateTime.Now));
                                            done = true;
                                        }
                                        break;
                                    }
                                }
                                if (done) break;
                                cellCount++;
                            }
                        }
                    }
                }                
            }
            catch (Exception e)
            {
                Utils.LogException(e);
            }
        }

        public static void UpdateGUI()
        {
            Sheet sheet = GuiManager.Sheets[Enums.EGameState.YuushaGame.ToString()];
            Character pre = Character.PreviousRoundCharacter;
            Character chr = Character.CurrentCharacter;

            try
            {
                if (chr != null)
                {
                    #region Hits Labels
                    if (sheet["HitsRemainingLabel"].Text.Length > 0)
                    {
                        int hitsRemaining = Convert.ToInt32(sheet["HitsRemainingLabel"].Text);

                        if (Character.CurrentCharacter.CurrentRoundsPlayed > 0)
                        {
                            if (hitsRemaining < chr.Hits)
                                TextCue.AddHealthGainTextCue(string.Format("+{0}", Convert.ToString(chr.Hits - hitsRemaining)));
                            else if (hitsRemaining > chr.Hits)
                                TextCue.AddHealthLossTextCue(string.Format("-{0}", Convert.ToString(hitsRemaining - chr.Hits)));
                        }
                    }

                    sheet["HitsRemainingLabel"].Text = chr.Hits.ToString();
                    if (chr.Hits != chr.HitsFull)
                        sheet["HitsRemainingLabel"].TextColor = Color.DarkSalmon;
                    else sheet["HitsRemainingLabel"].TextColor = sheet["HitsAmountLabel"].TextColor;
                    sheet["HitsAmountLabel"].Text = "/" + chr.HitsFull.ToString();

                    if (sheet["HitsPercentageBarLabel"] is PercentageBarLabel hitsPctBarLabel)
                    {
                        hitsPctBarLabel.Percentage = (double)chr.Hits / chr.HitsFull * 100;
                        hitsPctBarLabel.ForeLabel.Text = chr.Hits + "/" + chr.HitsFull;
                        //hitsPctBarLabel.PopUpText = chr.Hits + "/" + chr.HitsFull;
                    }

                    if (!GameHUD.VitalsTextMode)
                    {
                        sheet["HitsRemainingLabel"].IsVisible = false;
                        sheet["HitsAmountLabel"].IsVisible = false;
                        sheet["HitsAmountLabel"].IsVisible = false;
                        sheet["HitsPercentageBarLabel"].IsVisible = true;
                    }
                    else
                    {
                        sheet["HitsRemainingLabel"].IsVisible = true;
                        sheet["HitsAmountLabel"].IsVisible = true;
                        sheet["HitsAmountLabel"].IsVisible = true;
                        sheet["HitsPercentageBarLabel"].IsVisible = false;
                    } 
                    #endregion

                    #region Stamina Labels
                    if (sheet["StaminaRemainingLabel"].Text.Length > 0)
                    {
                        int stamRemaining = Convert.ToInt32(sheet["StaminaRemainingLabel"].Text);

                        if (Character.CurrentCharacter.CurrentRoundsPlayed > 0)
                        {
                            if (stamRemaining < chr.Stamina)
                                TextCue.AddStaminaGainTextCue(string.Format("+{0}", Convert.ToString(chr.Stamina - stamRemaining)));
                            else if (stamRemaining > chr.Stamina)
                                TextCue.AddStaminaLossTextCue(string.Format("-{0}", Convert.ToString(stamRemaining - chr.Stamina)));
                        }
                    }

                    sheet["StaminaRemainingLabel"].Text = chr.Stamina.ToString();
                    if (chr.Stamina != chr.StaminaFull)
                        sheet["StaminaRemainingLabel"].TextColor = Color.DarkSalmon;
                    else sheet["StaminaRemainingLabel"].TextColor = sheet["StaminaAmountLabel"].TextColor;
                    sheet["StaminaAmountLabel"].Text = "/" + chr.StaminaFull.ToString();

                    if (sheet["StaminaPercentageBarLabel"] is PercentageBarLabel stamPctBarLabel)
                    {
                        stamPctBarLabel.Percentage = (double)chr.Stamina / chr.StaminaFull * 100;
                        stamPctBarLabel.ForeLabel.Text = chr.Stamina + "/" + chr.StaminaFull;
                        //stamPctBarLabel.PopUpText = chr.Stamina + "/" + chr.StaminaFull;
                    }

                    if (!GameHUD.VitalsTextMode)
                    {
                        sheet["StaminaRemainingLabel"].IsVisible = false;
                        sheet["StaminaAmountLabel"].IsVisible = false;
                        sheet["StaminaAmountLabel"].IsVisible = false;
                        sheet["StaminaPercentageBarLabel"].IsVisible = true;
                    }
                    else
                    {
                        sheet["StaminaRemainingLabel"].IsVisible = true;
                        sheet["StaminaAmountLabel"].IsVisible = true;
                        sheet["StaminaAmountLabel"].IsVisible = true;
                        sheet["StaminaPercentageBarLabel"].IsVisible = false;
                    } 
                    #endregion

                    #region Magic Points Labels
                    if (chr.IsManaUser)
                    {
                        if (sheet["MagicPtsRemainingLabel"].Text.Length > 0)
                        {
                            int manaRemaining = Convert.ToInt32(sheet["MagicPtsRemainingLabel"].Text);

                            if (Character.CurrentCharacter.CurrentRoundsPlayed > 0)
                            {
                                if (manaRemaining < chr.Mana)
                                    TextCue.AddManaGainTextCue(string.Format("+{0}", Convert.ToString(chr.Mana - manaRemaining)));
                                else if (manaRemaining > chr.Mana)
                                    TextCue.AddManaLossTextCue(string.Format("-{0}", Convert.ToString(manaRemaining - chr.Mana)));
                            }
                        }

                        sheet["MagicPtsRemainingLabel"].Text = chr.Mana.ToString();
                        if (chr.Mana != chr.ManaFull)
                            sheet["MagicPtsRemainingLabel"].TextColor = Color.DarkSalmon;
                        else sheet["MagicPtsRemainingLabel"].TextColor = sheet["MagicPtsAmountLabel"].TextColor;
                        sheet["MagicPtsAmountLabel"].Text = "/" + chr.ManaFull.ToString();

                        if (sheet["MagicPtsPercentageBarLabel"] is PercentageBarLabel magicPctBarLabel)
                        {
                            magicPctBarLabel.Percentage = (double)chr.Mana / chr.ManaFull * 100;
                            magicPctBarLabel.ForeLabel.Text = chr.Mana + "/" + chr.ManaFull;
                            //magicPctBarLabel.PopUpText = chr.Mana + "/" + chr.ManaFull;
                        }

                        if (!GameHUD.VitalsTextMode)
                        {
                            sheet["MagicPtsRemainingLabel"].IsVisible = false;
                            sheet["MagicPtsAmountLabel"].IsVisible = false;
                            sheet["MagicPtsAmountLabel"].IsVisible = false;
                            sheet["MagicPtsPercentageBarLabel"].IsVisible = true;
                        }
                        else
                        {
                            sheet["MagicPtsRemainingLabel"].IsVisible = true;
                            sheet["MagicPtsAmountLabel"].IsVisible = true;
                            sheet["MagicPtsAmountLabel"].IsVisible = true;
                            sheet["MagicPtsPercentageBarLabel"].IsVisible = false;
                        }
                    }
                    else
                    {
                        sheet["MagicPtsLabel"].IsVisible = false;
                        sheet["MagicPtsRemainingLabel"].IsVisible = false;
                        sheet["MagicPtsAmountLabel"].IsVisible = false;
                        sheet["MagicPtsPercentageBarLabel"].IsVisible = false;
                    }
                    #endregion

                    #region Right Hand Update
                    if (chr.RightHand != null)
                    {
                        (sheet["RHNameDragAndDropButton"] as DragAndDropButton).IsTextVisible = false;
                        (sheet["RHNameDragAndDropButton"] as DragAndDropButton).RepresentedItem = chr.RightHand;
                        sheet["RHNameDragAndDropButton"].PopUpText = chr.RightHand.Name;
                        sheet["RHNameDragAndDropButton"].VisualKey = chr.RightHand.VisualKey;
                        sheet["RHNameDragAndDropButton"].TintColor = Color.White;

                        if (chr.RightHand.Nocked)
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
                        (sheet["RHNameDragAndDropButton"] as DragAndDropButton).RepresentedItem = null;
                        sheet["RHNameDragAndDropButton"].VisualKey = "WhiteSpace";
                        sheet["RHNameDragAndDropButton"].TintColor = Color.Black;
                        sheet["RHNameDragAndDropButton"].PopUpText = "";
                    }
                    #endregion

                    #region Left Hand Update
                    if (chr.LeftHand != null)
                    {
                        (sheet["LHNameDragAndDropButton"] as DragAndDropButton).IsTextVisible = false;
                        (sheet["LHNameDragAndDropButton"] as DragAndDropButton).RepresentedItem = chr.LeftHand;
                        sheet["LHNameDragAndDropButton"].PopUpText = chr.LeftHand.Name;
                        sheet["LHNameDragAndDropButton"].VisualKey = chr.LeftHand.VisualKey;
                        sheet["LHNameDragAndDropButton"].TintColor = Color.White;

                        if (chr.LeftHand.Nocked)
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
                        (sheet["LHNameDragAndDropButton"] as DragAndDropButton).RepresentedItem = null;
                        sheet["LHNameDragAndDropButton"].VisualKey = "WhiteSpace";
                        sheet["LHNameDragAndDropButton"].TintColor = Color.Black;
                        sheet["LHNameDragAndDropButton"].PopUpText = "";
                    }
                    #endregion

                    if (pre != null && Character.CurrentCharacter.CurrentRoundsPlayed > 1)
                    {
                        if (chr.HitsFull != pre.HitsFull)
                        {
                            if (chr.HitsFull > pre.HitsFull)
                                AchievementLabel.CreateAchievementLabel(string.Format("+{0}", chr.HitsFull - pre.HitsFull), AchievementLabel.AchievementType.Vitals_HitsGain);

                            // Currently only displays a +Hits achievement when HitsFull increases.
                            Character.PreviousRoundCharacter.HitsMax = chr.HitsMax;
                            Character.PreviousRoundCharacter.HitsAdjustment = chr.HitsAdjustment;
                            Character.PreviousRoundCharacter.HitsDoctored = chr.HitsDoctored;
                        }

                        if (chr.StaminaFull != pre.StaminaFull)
                        {
                            if (chr.StaminaFull > pre.StaminaFull)
                                AchievementLabel.CreateAchievementLabel(string.Format("+{0}", chr.StaminaFull - pre.StaminaFull), AchievementLabel.AchievementType.Vitals_StaminaGain);

                            Character.PreviousRoundCharacter.StaminaMax = chr.StaminaMax;
                            Character.PreviousRoundCharacter.StaminaAdjustment = chr.StaminaAdjustment;
                        }

                        if (chr.IsManaUser && chr.ManaFull != pre.ManaFull)
                        { 
                            if (chr.ManaFull > pre.ManaFull)
                                AchievementLabel.CreateAchievementLabel(string.Format("+{0}", chr.ManaFull - pre.ManaFull), AchievementLabel.AchievementType.Vitals_ManaGain);

                            Character.PreviousRoundCharacter.ManaMax = chr.ManaMax;
                            Character.PreviousRoundCharacter.ManaAdjustment = chr.ManaAdjustment;
                        }

                        if (chr.StrengthAdd != pre.StrengthAdd)
                        {
                            if(chr.StrengthAdd > pre.StrengthAdd)
                                AchievementLabel.CreateAchievementLabel(string.Format("Str Add +{0}", chr.StrengthAdd - pre.StrengthAdd), AchievementLabel.AchievementType.StrengthAdd);

                            Character.PreviousRoundCharacter.StrengthAdd = chr.StrengthAdd;
                        }

                        if (chr.DexterityAdd != pre.DexterityAdd)
                        {
                            if (chr.DexterityAdd > pre.DexterityAdd)
                                AchievementLabel.CreateAchievementLabel(string.Format("Dex Add +{0}", chr.DexterityAdd - pre.DexterityAdd), AchievementLabel.AchievementType.DexterityAdd);

                            Character.PreviousRoundCharacter.DexterityAdd = chr.DexterityAdd;
                        }

                        #region Ability Score Changes
                        if (chr.Strength != pre.Strength)
                        {
                            if (chr.Strength > pre.Strength)
                                AchievementLabel.CreateAchievementLabel(string.Format("Strength +{0}", chr.Strength - pre.Strength), AchievementLabel.AchievementType.AbilityScoreGain);
                            else AchievementLabel.CreateAchievementLabel(string.Format("Strength -{0}", pre.Strength - chr.Strength), AchievementLabel.AchievementType.AbilityScoreLoss);

                            Character.PreviousRoundCharacter.Strength = chr.Strength;
                        }

                        if (chr.Dexterity != pre.Dexterity)
                        {
                            if (chr.Dexterity > pre.Dexterity)
                                AchievementLabel.CreateAchievementLabel(string.Format("Dexterity +{0}", chr.Dexterity - pre.Dexterity), AchievementLabel.AchievementType.AbilityScoreGain);
                            else AchievementLabel.CreateAchievementLabel(string.Format("Dexterity -{0}", pre.Dexterity - chr.Dexterity), AchievementLabel.AchievementType.AbilityScoreLoss);

                            Character.PreviousRoundCharacter.Dexterity = chr.Dexterity;
                        }

                        if (chr.Intelligence != pre.Intelligence)
                        {
                            if (chr.Intelligence > pre.Intelligence)
                                AchievementLabel.CreateAchievementLabel(string.Format("Intelligence +{0}", chr.Intelligence - pre.Intelligence), AchievementLabel.AchievementType.AbilityScoreGain);
                            else AchievementLabel.CreateAchievementLabel(string.Format("Intelligence -{0}", pre.Intelligence - chr.Intelligence), AchievementLabel.AchievementType.AbilityScoreLoss);

                            Character.PreviousRoundCharacter.Intelligence = chr.Intelligence;
                        }

                        if (chr.Wisdom != pre.Wisdom)
                        {
                            if (chr.Wisdom > pre.Wisdom)
                                AchievementLabel.CreateAchievementLabel(string.Format("Wisdom +{0}", chr.Wisdom - pre.Wisdom), AchievementLabel.AchievementType.AbilityScoreGain);
                            else AchievementLabel.CreateAchievementLabel(string.Format("Wisdom -{0}", pre.Wisdom - chr.Wisdom), AchievementLabel.AchievementType.AbilityScoreLoss);

                            Character.PreviousRoundCharacter.Wisdom = chr.Wisdom;
                        }

                        if (chr.Constitution != pre.Constitution)
                        {
                            if (chr.Constitution > pre.Constitution)
                                AchievementLabel.CreateAchievementLabel(string.Format("Constitution +{0}", chr.Constitution - pre.Constitution), AchievementLabel.AchievementType.AbilityScoreGain);
                            else AchievementLabel.CreateAchievementLabel(string.Format("Constitution -{0}", pre.Constitution - chr.Constitution), AchievementLabel.AchievementType.AbilityScoreLoss);

                            Character.PreviousRoundCharacter.Constitution = chr.Constitution;
                        }

                        if (chr.Charisma != pre.Charisma)
                        {
                            if (chr.Charisma > pre.Charisma)
                                AchievementLabel.CreateAchievementLabel(string.Format("Charisma +{0}", chr.Charisma - pre.Charisma), AchievementLabel.AchievementType.AbilityScoreGain);
                            else AchievementLabel.CreateAchievementLabel(string.Format("Charisma -{0}", pre.Charisma - chr.Charisma), AchievementLabel.AchievementType.AbilityScoreLoss);

                            Character.PreviousRoundCharacter.Charisma = chr.Charisma;
                        } 
                        #endregion

                        // Experience adjustments.
                        if (pre.Experience < chr.Experience && pre.Experience > 0)
                            TextCue.AddXPGainTextCue(string.Format("+{0:n0}", chr.Experience - pre.Experience));
                        else if (pre.Experience > chr.Experience)
                            TextCue.AddXPLossTextCue(string.Format("-{0:n0}", pre.Experience - chr.Experience));

                        Character.PreviousRoundCharacter.Experience = chr.Experience;
                    }                    
                }

                if (!Client.HasFocus)
                {
                    if (sheet[Globals.GAMEINPUTTEXTBOX] is Control c)
                        c.HasFocus = false;

                    GuiManager.ActiveTextBox = null;
                    GuiManager.ControlWithFocus = null;
                }

                if (sheet["ExpPercentageBarLabel"] is PercentageBarLabel expBar && chr != null)
                {
                    int level = Globals.GetExpLevel(chr.Experience);
                    if (level != chr.Level)
                        expBar.Percentage = 100;
                    else
                    {
                        long expNeededForLevelUp = Globals.GetExperienceRequiredForLevel(level + 1) - Globals.GetExperienceRequiredForLevel(level);
                        long expIntoLevel = chr.Experience - expNeededForLevelUp;
                        expBar.Percentage = (double)expIntoLevel / expNeededForLevelUp * 100;
                    }

                    try
                    {
                        if (expBar != null && expBar.Percentage < 100)
                            expBar.PopUpText = string.Format("{0:0.00}%", expBar.Percentage);
                        else if (expBar != null) expBar.PopUpText = "100%";
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
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
            SpellWarmingWindow.CreateNewRoundCountdownWindow();
            m_usedLetters = string.Empty;
            GameHUD.Cells.Clear();
            GameHUD.CharactersInView.Clear();
            //Character.PreviousRoundCharacter = Character.CurrentCharacter.Clone();
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

                    if (cellInfo == null || cellInfo.Length <= 0 || cellInfo == "")
                        cell = new Cell();
                    else cell = new Cell(cellInfo);

                    if (critterInfo.Length > 0)
                    {
                        string[] critters = critterInfo.Split(Protocol.USPLIT.ToCharArray());

                        for (a = 0; a < critters.Length; a++)
                        {
                            Character ch = FormatCellCritter(critters[a]);
                            if (ch != null)
                            {
                                if (!GameHUD.CharactersInView.ContainsKey(ch.UniqueID))
                                    GameHUD.CharactersInView.Add(ch.UniqueID, ch); // else log it?
                                cell.Add(ch);
                            }
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

                GameHUD.Cells.Add(cell);
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

                crit.UniqueID = Convert.ToInt32(critterInfo[0]); // player id or worldnpc id
                crit.isPC = crit.UniqueID >= 0;
                crit.Name = critterInfo[1];
                //crit.shortDesc = critterInfo[tempA + 2];
                //crit.longDesc = critterInfo[tempA + 3];
                crit.VisualKey = critterInfo[2];
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
                crit.Alignment = (World.Alignment)Convert.ToInt32(critterInfo[3]);
                //crit.Level = Convert.ToInt32(critterInfo[tempA + 7]);
                //crit.Gender = (Character.GenderType)Convert.ToInt32(critterInfo[tempA + 8]);
                //crit.Race = critterInfo[tempA + 9];
                //crit.Hits = Convert.ToInt32(critterInfo[tempA + 10]);
                //crit.HitsMax = Convert.ToInt32(critterInfo[tempA + 11]);

                if (critterInfo[4].Length > 0)
                {
                    crit.RightHand.CatalogID = Convert.ToInt32(critterInfo[4]);
                    crit.RightHand.Name = critterInfo[5];
                    crit.RightHand.VisualKey = critterInfo[6];
                }
                else
                {
                    crit.RightHand = null;
                }
                if (critterInfo[7].Length > 0)
                {
                    crit.LeftHand.CatalogID = Convert.ToInt32(critterInfo[7]);
                    crit.LeftHand.Name = critterInfo[8];
                    crit.LeftHand.VisualKey = critterInfo[9];
                }
                else
                {
                    crit.LeftHand = null;
                }
                crit.visibleArmor = critterInfo[10];
                if (critterInfo.Length >= 12)
                    crit.healthPercentage = Convert.ToDouble(critterInfo[11]);
                if (critterInfo.Length >= 13)
                    crit.staminaPercentage = Convert.ToDouble(critterInfo[12]);
                if (critterInfo.Length >= 14)
                    crit.manaPercentage = Convert.ToDouble(critterInfo[13]);
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
                    CatalogID = Convert.ToInt32(itemInfo[0]),
                    WorldItemID = Convert.ToInt32(itemInfo[1]),
                    Name = itemInfo[2],
                    VisualKey = itemInfo[3]
                };
                return item;
            }
            catch (Exception e)
            {
                Utils.LogException(e);
                return null;
            }
        }

        public static void BuildCritterList()
        {
            int labelCount = (GuiManager.GetControl("CritterListWindow") as Window).Controls.FindAll(c => c is CritterListLabel).Count;
            m_critterListNames = new string[labelCount];

            try
            {
                if (GameHUD.Cells.Count > 0)
                {
                    int labelNum = 0;

                    for (int a = 0; a < labelCount; a++) // 15 critter list items in YuushaMode
                    {
                        if (GuiManager.GetControl("CritterList" + a.ToString()) is CritterListLabel critterListLabel)
                        {
                            critterListLabel.IsVisible = false;
                            GuiManager.Dispose(critterListLabel.DropDownMenu);
                        }
                    }

                    Cell cell;

                    if (GameHUD.Cells.Count >= 25)
                    {
                        cell = GameHUD.Cells[24]; // start with our cell

                        if (cell.Characters.Count > 0)
                        {
                            foreach (Character ch in cell.Characters)
                            {
                                #region Create Critter Label (Center Cell)
                                string critterInfo = "  " + m_alignment[(int)ch.Alignment] + ch.Name;
                                //critterInfo = critterInfo.PadRight(19);

                                //if (ch.RightHand != null)
                                //    critterInfo += ch.RightHand.Name;

                                //critterInfo = critterInfo.PadRight(31);

                                //if (ch.LeftHand != null)
                                //    critterInfo += ch.LeftHand.Name;

                                //critterInfo = critterInfo.PadRight(43);

                                //critterInfo += ch.visibleArmor;

                                m_critterListNames[labelNum] = ch.Name;
                                CritterListLabel label = GuiManager.GetControl("CritterList" + labelNum.ToString()) as CritterListLabel;
                                label.Critter = ch;
                                label.CenterCell = true;
                                label.Text = critterInfo;
                                label.TextColor = TextManager.GetAlignmentColor(true, ch.Alignment);
                                label.TintColor = TextManager.GetAlignmentColor(false, ch.Alignment);
                                if (ch.RightHand != null)
                                {
                                    label.RightHandItemLabel.VisualKey = ch.RightHand.VisualKey;
                                    label.RightHandItemLabel.PopUpText = ch.RightHand.Name;
                                    label.RightHandItemLabel.IsVisible = true;
                                }
                                else label.RightHandItemLabel.IsVisible = false;
                                if (ch.LeftHand != null)
                                {
                                    label.LeftHandItemLabel.VisualKey = ch.LeftHand.VisualKey;
                                    label.LeftHandItemLabel.PopUpText = ch.LeftHand.Name;
                                    label.LeftHandItemLabel.IsVisible = true;
                                }
                                else label.LeftHandItemLabel.IsVisible = false;

                                label.IsVisible = true;
                                #endregion

                                labelNum++;

                                if (labelNum >= labelCount)
                                    break;
                            }
                        }
                    }

                    for (int a = 0; a < GameHUD.Cells.Count; a++) // move through each cell and update the map and mobs list
                    {
                        if (labelNum > m_critterListNames.Length - 1)
                            break;

                        #region Create Critter Label (Non Center Cells)
                        if (a != 24)
                        {
                            cell = GameHUD.Cells[a];

                            if (cell.Characters != null && cell.Characters.Count > 0)
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
                                    if (ch.RightHand != null)
                                    {
                                        label.RightHandItemLabel.VisualKey = ch.RightHand.VisualKey;
                                        label.RightHandItemLabel.PopUpText = ch.RightHand.Name;
                                        label.RightHandItemLabel.IsVisible = true;
                                    }
                                    else label.RightHandItemLabel.IsVisible = false;
                                    if (ch.LeftHand != null)
                                    {
                                        label.LeftHandItemLabel.VisualKey = ch.LeftHand.VisualKey;
                                        label.LeftHandItemLabel.PopUpText = ch.LeftHand.Name;
                                        label.LeftHandItemLabel.IsVisible = true;
                                    }
                                    else label.LeftHandItemLabel.IsVisible = false;
                                    label.IsVisible = true;
                                    labelNum++;

                                    if (labelNum >= labelCount)
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

        public static void BuildMap()
        {
            try
            {
                if (GameHUD.Cells.Count > 0)
                {
                    SpinelTileLabel spLabel;
                    Cell cell;
                    SpinelTileDefinition currTile;

                    for (int count = 0; count < GameHUD.Cells.Count; count++) // move through each cell and update the map and mobs list
                    {
                        cell = GameHUD.Cells[count];

                        //spLabel = GuiManager.GetControl("Tile" + count.ToString()) as SpinelTileLabel;
                        spLabel = GuiManager.GetControl(Enums.EGameState.YuushaGame.ToString(), "Tile" + count.ToString()) as SpinelTileLabel;

                        if (spLabel == null)
                            continue;

                        spLabel.EffectNames.Clear();
                        spLabel.CritterVisuals.Clear();
                        spLabel.LootVisual = "";
                        spLabel.PathingVisual = "";
                        spLabel.CreatureText = "";

                        if (cell.IsVisible)
                        {
                            string graphic = cell.CellGraphic;

                            //if (cell.Effects.FindAll(e => e.Name.Equals("Illusion")).Count > 0 && cell.Effects.Count == 1)
                            //    graphic = cell.DisplayGraphic;

                            if (cell.Effects.Count == 1 && Effect.NonDisplayableCellEffects.Contains(TextManager.FormatEnumString(cell.Effects[0].Name)))
                                graphic = cell.DisplayGraphic;
                            else if (cell.Effects.FindAll(e => e.Name.Equals("Illusion")).Count > 0)
                                graphic = cell.DisplayGraphic;

                            // uses DisplayGraphic -- should use CellGraphic and then items to display effects?
                            //if (m_tilesDict.ContainsKey(cell.DisplayGraphic))
                            //    currTile = m_tilesDict[cell.DisplayGraphic];
                                if (m_tilesDict.ContainsKey(graphic))
                                currTile = m_tilesDict[graphic];
                            else
                            {
                                Utils.LogOnce("Failed to find SpinelTileDefinition for cell graphic [ " + cell.DisplayGraphic + " ]");
                                currTile = m_tilesDict["  "];
                            }

                            if(cell.Effects.Count > 0)
                            {
                                foreach(Effect effect in cell.Effects)
                                {
                                    // make a decision here if you want to draw effects multiple times -- probably not, also effect amount!!
                                    if(Effect.CellEffectsDictionary.ContainsKey(TextManager.FormatEnumString(effect.Name))
                                        && !spLabel.EffectNames.Contains(TextManager.FormatEnumString(effect.Name)))
                                    {
                                        spLabel.EffectNames.Add(TextManager.FormatEnumString(effect.Name));
                                    }
                                    else if(!Effect.IgnoreCellEffectsAbsence.Contains(TextManager.FormatEnumString(effect.Name)))
                                        Utils.LogOnce("CellEffectsDictionary does not contain a key for effect: " + TextManager.FormatEnumString(effect.Name));
                                }
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

                            if (cell.IsPortal)
                                spLabel.VisualKey = m_tilesDict["pp"].ForeVisual.Key;

                            if (cell.Characters != null && cell.Characters.Count > 0)
                            {
                                if (count != 24) // not our cell
                                {
                                    VisualKey vk = new VisualKey(cell.Characters[0].VisualKey);
                                    if (vk.Key.Contains("sorcerer") || vk.Key.Contains("ravager"))
                                        vk.OverrideTintColor = Color.DarkGray;
                                    spLabel.CritterVisuals.Add(vk);
                                    spLabel.CreatureText = cell.Characters[0].assignedLetter;
                                }
                                else
                                {
                                    VisualKey vk = new VisualKey(cell.Characters[0].VisualKey);
                                    if (vk.Key.Contains("sorcerer") || vk.Key.Contains("ravager"))
                                        vk.OverrideTintColor = Color.DarkGray;

                                    spLabel.CritterVisuals.Add(vk);

                                    if (!Character.CurrentCharacter.IsPeeking)
                                    {
                                        if (Character.CurrentCharacter.IsDead)
                                            spLabel.CritterVisuals.Add(new VisualKey("ghost"));
                                        else
                                        {
                                            vk = new VisualKey(Character.CurrentCharacter.VisualKey);
                                            if (vk.Key.Contains("sorcerer") || vk.Key.Contains("ravager"))
                                                vk.OverrideTintColor = Color.DarkGray;
                                            spLabel.CritterVisuals.Add(vk);
                                        }
                                        Character.CurrentCharacter.UpdateCoordinates(cell);
                                    }
                                }
                            }
                            else if (count == 24)
                            {
                                if (Character.CurrentCharacter.IsDead)
                                    spLabel.CritterVisuals.Add(new VisualKey("ghost"));
                                else
                                {
                                    VisualKey vk = new VisualKey(Character.CurrentCharacter.VisualKey);
                                    if (vk.Key.Contains("sorcerer") || vk.Key.Contains("ravager"))
                                        vk.OverrideTintColor = Color.Gray;
                                    spLabel.CritterVisuals.Add(vk);
                                }

                                Character.CurrentCharacter.UpdateCoordinates(cell);
                            }

                            if (cell.HasItems && cell.IsLootVisible)
                            {
                                if(cell.Items.Count <= 7)
                                {
                                    spLabel.LootVisual = "LootPileSmall";
                                    
                                }
                                else
                                {
                                    spLabel.LootVisual = "LootPileLarge";
                                }

                                if (cell.IsLootPartiallyVisible)
                                    spLabel.LootVisualAlpha = 150;
                                else spLabel.LootVisualAlpha = 255;

                                //spLabel.LootVisual = m_tilesDict[" $"].ForeVisual.Key;
                            }
                        }
                        else
                        {
                            if (m_tilesDict.ContainsKey("  "))
                                currTile = m_tilesDict["  "];
                            else
                            {
                                Utils.LogOnce("Failed to find SpinelTileDefinition for cell graphic [    ]");
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
