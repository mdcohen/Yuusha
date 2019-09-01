﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;

namespace Yuusha.gui
{
    public class GameHUD : GameComponent
    {
        public static List<string> NonDiscreetlyDraggableWindows = new List<string>()
        {
            "GameTextScrollableTextBox",
            "EffectsWindow",
            //"FogOfWarWindow",
            "CharacterStatsWindow",
            //"InventoryWindow",
            "RingsWindow",
            "SpellbookWindow",
            "SpellringWindow",
            "SpellWarmingWindow",
            "VitalsWindow"
        };
        public static List<int> InitialSpellbookUpdated = new List<int>();
        public static List<int> InitialTalentbookUpdated = new List<int>();
        public static Dictionary<string, string> GameIconsDictionary = new Dictionary<string, string>()
        {
            // skills
            {"bow", "yuushaicon_0" },
            {"dagger", "yuushaicon_1" },
            {"flail", "yuushaicon_2" },
            {"polearm", "yuushaicon_3" },
            {"mace", "yuushaicon_4" },
            {"rapier", "yuushaicon_5" },
            {"shuriken", "yuushaicon_6" },
            {"staff", "yuushaicon_7" },
            {"sword", "yuushaicon_8" },
            {"threestaff", "yuushaicon_9" },
            {"two handed", "yuushaicon_10" },
            {"unarmed", "yuushaicon_11" },
            {"magic", "yuushaicon_12" },
            {"thievery", "yuushaicon_13" },
            {"bash", "yuushaicon_14" },
            {"poisoned", "yuushaicon_15" },
            {"throwing", "yuushaicon_16" },
            {"stunned", "yuushaicon_17" },
            {"resting", "yuushaicon_18" },
            {"meditating", "yuushaicon_19" },
            {"thirdeye", "yuushaicon_20" },
            {"swap", "yuushaicon_21" },
            {"upgrade", "yuushaicon_22" },
            {"downgrade", "yuushaicon_23" },
        };
        const int MAP_GRID_MINIMUM = 30;
        const int MAP_GRID_MAXIMUM = 100;

        public static List<Tuple<ScrollableTextBox, DateTime>> ConversationBubbles = new List<Tuple<ScrollableTextBox, DateTime>>();

        public static bool ChangingMapDisplaySize = false; // used in Events.UpdateGUI to prevent issues while changing map display size

        public static List<Control> AchievementLabelList = new List<Control>(); // level up label also goes here to prevent achievements from showing until it's done

        public static Character CurrentTarget;
        public static string TextSendOverride = "";
        public static bool VitalsTextMode = false;

        public static Spell DraggedSpell { get; set; }
        public static Enums.EGameState PreviousGameState { get; set; }
        public static Cell ExaminedCell { get; set; }

        public static List<Cell> Cells = new List<Cell>();
        public static Dictionary<int, Character> CharactersInView = new Dictionary<int, Character>();

        private static int FramesQuantity = 0;
        private static float CurrentAvgFPS = 0;

        // TODO a boolean here to prevent ZName pop up

        public static GridBoxWindow.GridBoxPurpose GridBoxWindowRequestUpdate
        { get; set; } = GridBoxWindow.GridBoxPurpose.None;

        public static bool NextGridBoxUpdateIsSilent
        { get; set; } = true;

        // Text is not cleared from scrolling textboxes in these ClientStates (they fill the screen)
        public static List<Enums.EGameState> OverrideDisplayStates = new List<Enums.EGameState>
        {
            Enums.EGameState.CharacterGeneration, Enums.EGameState.HotButtonEditMode
        };

        public GameHUD(Game game) : base(game)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //if (Character.PreviousRoundCharacter != null && Character.CurrentCharacter != null)
            //{
            //    if (Client.GameState.ToString().EndsWith("Game") && Character.PreviousRoundCharacter.Z != Character.CurrentCharacter.Z)
            //    {
            //        Events.RegisterEvent(Events.EventName.Request_Stats);
            //    }
            //}

            foreach(Tuple<ScrollableTextBox, DateTime> tuple in new List<Tuple<ScrollableTextBox, DateTime>>(ConversationBubbles))
            {
                tuple.Item1.ZDepth = 0;

                if (DateTime.Now - tuple.Item2 >= TimeSpan.FromSeconds(2))
                {
                    tuple.Item1.VisualAlpha -= Client.ClientSettings.ConversationBubbleFadeOutSpeed;
                    tuple.Item1.TextAlpha -= Client.ClientSettings.ConversationBubbleFadeOutSpeed;
                }
                
                if(DateTime.Now - tuple.Item2 >= TimeSpan.FromSeconds(Utility.Settings.StaticSettings.RoundDelayLength * 1.3) || tuple.Item1.VisualAlpha <= 0)
                {
                    GuiManager.RemoveControl(tuple.Item1);
                    ConversationBubbles.Remove(tuple);
                }
            }
        }

        public static float UpdateCumulativeMovingAverageFPS(float newFPS)
        {
            ++FramesQuantity;
            CurrentAvgFPS += (newFPS - CurrentAvgFPS) / FramesQuantity;

            return CurrentAvgFPS;
        }

        /// <summary>
        /// Handles dropping from a drag and drop area to a drag and drop area.
        /// </summary>
        /// <param name="b">The drag and drop area where an item is being manipulated FROM.</param>
        public static void DragAndDropLogic(DragAndDropButton b)
        {
            // Right hand or left hand items
            if (b.Name.StartsWith("RH") || b.Name.StartsWith("LH"))
            {
                #region Right and Left Hand Items
                string rightOrLeft = b.Name.StartsWith("RH") ? "right" : "left";
                if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Sack"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command, "put " + rightOrLeft + " in sack");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Sack);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Belt"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command, "belt " + rightOrLeft);
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Belt);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Pouch"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command, "put " + rightOrLeft + " in pouch");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Pouch);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Locker"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command, "put " + rightOrLeft + " in locker");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Locker);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH") || GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                {
                    // when returning to original position prevent a swap command
                    if(GuiManager.MouseOverDropAcceptingControl.Name != b.Name)
                        Events.RegisterEvent(Events.EventName.Send_Command, "swap");
                }
                else if (AcceptingGridBoxIsLocation(GuiManager.MouseOverDropAcceptingControl.Name))
                {
                    GridBoxWindow window = GuiManager.MouseOverDropAcceptingControl as GridBoxWindow;

                    if (window.WindowTitle != null)
                    {
                        switch (window.WindowTitle.Text.ToLower())
                        {
                            case "altar":
                                Events.RegisterEvent(Events.EventName.Send_Command, "put " + rightOrLeft + " on altar");
                                break;
                            case "counter":
                                Events.RegisterEvent(Events.EventName.Send_Command, "put " + rightOrLeft + " on counter");
                                break;
                            default:
                                Events.RegisterEvent(Events.EventName.Send_Command, "drop " + rightOrLeft);
                                break;
                        }

                        GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Ground);
                    }
                }
                else if(GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Inventory"))
                {
                    //TODO: wearOrientation
                    Events.RegisterEvent(Events.EventName.Send_Command, "wear " + rightOrLeft);
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Inventory);
                }
                #endregion
            }
            else if (b.Name.StartsWith("Belt"))
            {
                if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH") || GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command, "wield " + b.RepresentedItem.Name);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Locker"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command, "wield " + b.RepresentedItem.Name + ";put " + b.RepresentedItem.Name + " in locker");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Locker);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Sack"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command,"wield " + b.RepresentedItem.Name + ";put " + b.RepresentedItem.Name + " in sack");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Sack);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Pouch"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command,"wield " + b.RepresentedItem.Name + ";put " + b.RepresentedItem.Name + " in pouch");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Pouch);
                }
                else if (AcceptingGridBoxIsLocation(GuiManager.MouseOverDropAcceptingControl.Name))
                {
                    GridBoxWindow window = GuiManager.MouseOverDropAcceptingControl as GridBoxWindow;
                    if (window.WindowTitle != null)
                    {
                        switch (window.WindowTitle.Text.ToLower())
                        {
                            case "altar":
                                Events.RegisterEvent(Events.EventName.Send_Command,"wield " + b.RepresentedItem.Name + ";put " + b.RepresentedItem.Name + " on altar");
                                break;
                            case "counter":
                                Events.RegisterEvent(Events.EventName.Send_Command,"wield " + b.RepresentedItem.Name + ";put " + b.RepresentedItem.Name + " on counter");
                                break;
                            default:
                                Events.RegisterEvent(Events.EventName.Send_Command,"wield " + b.RepresentedItem.Name + ";drop " + b.RepresentedItem.Name);
                                break;
                        }

                        GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Ground);
                    }
                }

                GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Belt);
            }
            else if (b.Name.StartsWith("Sack"))
            {
                if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH") || GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                {
                    string swapafter = "";
                    string swapbefore = "";

                    if (Character.CurrentCharacter.RightHand == null && GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                        swapafter = ";swap";
                    else if (Character.CurrentCharacter.LeftHand == null && GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH"))
                        swapbefore = "swap;";

                    Events.RegisterEvent(Events.EventName.Send_Command,swapbefore + "take " + b.GetNItemName(b) + " from sack" + swapafter);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Locker"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + " from sack; put " + b.RepresentedItem.Name + " in locker");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Locker);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Belt"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + " from sack;belt " + b.RepresentedItem.Name);
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Belt);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Pouch"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + " from sack;put " + b.RepresentedItem.Name + " in pouch");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Pouch);
                }
                else if (AcceptingGridBoxIsLocation(GuiManager.MouseOverDropAcceptingControl.Name))
                {
                    GridBoxWindow window = GuiManager.MouseOverDropAcceptingControl as GridBoxWindow;
                    if (window.WindowTitle != null)
                    {
                        switch (window.WindowTitle.Text.ToLower())
                        {
                            case "altar":
                                Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + " from sack;put " + b.RepresentedItem.Name + " on altar");
                                break;
                            case "counter":
                                Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + " from sack;put " + b.RepresentedItem.Name + " on counter");
                                break;
                            default:
                                Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + " from sack;drop " + b.RepresentedItem.Name);
                                break;
                        }

                        GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Ground);
                    }
                }

                GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Sack);
            }
            else if (b.Name.StartsWith("Pouch"))
            {
                if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH") || GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                {
                    string swapafter = "";
                    string swapbefore = "";

                    if (Character.CurrentCharacter.RightHand == null && GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                        swapafter = ";swap";
                    else if (Character.CurrentCharacter.LeftHand == null && GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH"))
                        swapbefore = "swap;";

                    Events.RegisterEvent(Events.EventName.Send_Command,swapbefore + "take " + b.GetNItemName(b) + " from pouch" + swapafter);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Locker"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + " from pouch; put " + b.RepresentedItem.Name + " in locker");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Locker);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Belt"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + " from pouch;belt " + b.RepresentedItem.Name);
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Belt);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Sack"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + " from pouch;put " + b.RepresentedItem.Name + " in sack");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Sack);
                }
                else if (AcceptingGridBoxIsLocation(GuiManager.MouseOverDropAcceptingControl.Name))
                {
                    GridBoxWindow window = GuiManager.MouseOverDropAcceptingControl as GridBoxWindow;
                    if (window.WindowTitle != null)
                    {
                        switch (window.WindowTitle.Text.ToLower())
                        {
                            case "altar":
                                Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + " from pouch;put " + b.RepresentedItem.Name + " on altar");
                                break;
                            case "counter":
                                Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + " from pouch;put " + b.RepresentedItem.Name + " on counter");
                                break;
                            default:
                                Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + " from pouch;drop " + b.RepresentedItem.Name);
                                break;
                        }

                        GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Ground);
                    }
                }

                GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Pouch);
            }
            else if (AcceptingGridBoxIsLocation(b.Name))
            {
                string fromLocation = "";
                if (b.Name.StartsWith("Altar"))
                    fromLocation = " from altar";
                else if (b.Name.StartsWith("Counter"))
                    fromLocation = " from counter";

                if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH") || GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                {
                    string swapafter = "";
                    string swapbefore = "";

                    if (Character.CurrentCharacter.RightHand == null && GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                        swapafter = ";swap";
                    else if (Character.CurrentCharacter.LeftHand == null && GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH"))
                        swapbefore = "swap;";

                    Events.RegisterEvent(Events.EventName.Send_Command,swapbefore + "take " + b.GetNItemName(b) + fromLocation + swapafter);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Locker"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + fromLocation + "; put " + b.RepresentedItem.Name + " in locker");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Locker);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Belt"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + fromLocation + ";belt " + b.RepresentedItem.Name);
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Belt);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Sack"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + fromLocation + ";put " + b.RepresentedItem.Name + " in sack");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Sack);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Pouch"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command, "take " + b.GetNItemName(b) + fromLocation + ";put " + b.RepresentedItem.Name + " in pouch");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Pouch);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Ground") || GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Altar") ||
                    GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Counter"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command, "take " + b.GetNItemName(b) + fromLocation + ";drop " + b.RepresentedItem.Name);
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Ground);
                }

                GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Ground);
            }
            else if(b.Name.Contains("Inventory"))
            {
                Character.WearOrientation wearOrientation = b.RepresentedItem.wearOrientation;
                if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH") || GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                {
                    string swapafter = "";
                    string swapbefore = "";

                    if (Character.CurrentCharacter.RightHand == null && GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                        swapafter = ";swap";
                    else if (Character.CurrentCharacter.LeftHand == null && GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH"))
                        swapbefore = "swap;";

                    Events.RegisterEvent(Events.EventName.Send_Command, swapbefore + "remove" + (wearOrientation != Character.WearOrientation.None ? wearOrientation.ToString() + " " : " ") + b.RepresentedItem.Name + swapafter);
                }
            }
        }

        public static void UpdateStatDetailsWindow()
        {
            if (GuiManager.GetControl("StatDetailsWindow") is Window w)
            {
                bool wasVisible = w.IsVisible;

                w.IsVisible = false;

                //bool createLabels = w.Controls.Count <= 0; // tricky here... currently no border or window title for this window 9/1/2019

                foreach (Control c in new List<Control>(w.Controls))
                {
                    GuiManager.RemoveControl(c);
                }

                w.Controls.Clear(); // just in case

                List<Tuple<string, string>> StatsLeftColumn = new List<Tuple<string, string>>()
                {
                    Tuple.Create("Race", "Race"), // Age                    
                    Tuple.Create("Gender", "Gender"), // Deity
                    Tuple.Create("Profession", "Profession"), // Specialty
                    Tuple.Create("Experience", "Experience"),
                    Tuple.Create("BLANK", "BLANK"),
                    Tuple.Create("Health","Hits"),
                    Tuple.Create("Health Adj", "HitsAdjustment"),
                    Tuple.Create("Health Full", "HitsFull"),
                    Tuple.Create("BLANK", "BLANK"),
                    Tuple.Create("Stamina", "Stamina"),
                    Tuple.Create("Stamina Adj", "StaminaAdjustment"),
                    Tuple.Create("BLANK", "BLANK"),
                    Tuple.Create("Mana", "Mana"),
                    Tuple.Create("Mana Adj", "ManaAdjustment"),
                    Tuple.Create("BLANK", "BLANK"),
                    Tuple.Create("Strength", "Strength"),
                    Tuple.Create("Dexterity", "Dexterity"),
                    Tuple.Create("Intelligence", "Intelligence"),
                    Tuple.Create("Wisdom", "Wisdom"),
                    Tuple.Create("Constitution", "Constitution"),
                    Tuple.Create("Charisma", "Charisma"),
                };

                List<Tuple<string, string>> StatsRightColumn = new List<Tuple<string, string>>()
                {
                    Tuple.Create("Age", "AgeDescription"),
                    Tuple.Create("Alignment", "Alignment"),
                    Tuple.Create("Specialty", "ClassFullName"),
                    Tuple.Create("Level", "Level"),
                    Tuple.Create("BLANK", "BLANK"),
                    Tuple.Create("Health Max", "HitsMax"),
                    Tuple.Create("Health Doctored", "HitsDoctored"),
                    Tuple.Create("BLANK", "BLANK"),
                    Tuple.Create("BLANK", "BLANK"),
                    Tuple.Create("BLANK", "BLANK"),
                    Tuple.Create("Stamina Max", "StaminaMax"),
                    Tuple.Create("Stamina Full", "StaminaFull"),
                    Tuple.Create("BLANK", "BLANK"),
                    Tuple.Create("Mana Max", "ManaMax"),
                    Tuple.Create("Mana Full", "ManaFull"),
                    Tuple.Create("BLANK", "BLANK"),
                    Tuple.Create("Strength Add", "StrengthAdd"),
                    Tuple.Create("Dexterity Add", "DexterityAdd"),
                    Tuple.Create("BLANK", "BLANK"),
                    Tuple.Create("Encumbrance", "Encumbrance"),
                    Tuple.Create("Birthday", "Birthday"),
                    Tuple.Create("Rounds", "RoundsPlayed"),
                    Tuple.Create("# Kills", "NumKills"),
                    Tuple.Create("# Deaths", "NumDeaths"),
                };

                PropertyInfo[] properties = Character.CurrentCharacter.GetType().GetProperties(BindingFlags.Public |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Instance);

                Dictionary<string, string> PropertyValues = new Dictionary<string, string>();
                List<string> TypesDesired = new List<string>()
                {
                    "System.String", "System.Int32", "System.Int64", "System.Boolean", "System.Decimal",
                    "Yuusha.World+Alignment", "Yuusha.Character+GenderType", "Yuusha.Character+ClassType"
                };

                foreach (PropertyInfo p in properties)
                {
                    try
                    {
                        object value = p.GetValue(Character.CurrentCharacter);

                        if (value != null && TypesDesired.Contains(value.GetType().ToString()))
                            PropertyValues.Add(p.Name, value == null ? "" : value.ToString());
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                        continue;
                    }
                }

                int x = 2;
                int y = 2;

                foreach (Tuple<string, string> t in StatsLeftColumn)
                {
                    if (t.Item1.ToLower() != "blank")
                    {
                        if (!Character.CurrentCharacter.IsManaUser && t.Item1.StartsWith("Mana"))
                            continue;

                        PropertyValues.TryGetValue(t.Item2, out string value);
                        if (int.TryParse(value, out int i)) // add commas to numerical values
                            value = string.Format("{0:n0}", i);
                        Label label = new Label(t.Item2 + "StatsLabel", w.Name, new Rectangle(x, y, 200, 20), t.Item1 + ": " + value, Color.White, true, false, w.Font, new VisualKey("WhiteSpace"), Color.White, 0, 255, BitmapFont.TextAlignment.Left, 0, 0, "", "", new List<Enums.EAnchorType>(), "");
                        GuiManager.GenericSheet.AddControl(label);
                        y += 18;
                    }
                    else y += 9;
                }

                x = 212;
                y = 2;

                foreach (Tuple<string, string> t in StatsRightColumn)
                {
                    if (!Character.CurrentCharacter.IsManaUser && t.Item1.StartsWith("Mana"))
                        continue;

                    if (t.Item1.ToLower() != "blank")
                    {
                        PropertyValues.TryGetValue(t.Item2, out string value);
                        if (int.TryParse(value, out int i)) // add commas to numerical values
                            value = string.Format("{0:n0}", i);
                        Label label = new Label(t.Item2 + "StatsLabel", w.Name, new Rectangle(x, y, 200, 20), t.Item1 + ": " + value, Color.White, true, false, w.Font, new VisualKey("WhiteSpace"), Color.White, 0, 255, BitmapFont.TextAlignment.Left, 0, 0, "", "", new List<Enums.EAnchorType>(), "");
                        GuiManager.GenericSheet.AddControl(label);
                        y += 18;
                    }
                    else y += 9;
                }

                w.IsVisible = wasVisible;
            }
        }

        public static void UpdateFurtherStatDetailsWindow()
        {
            if (GuiManager.GetControl("FurtherStatDetailsWindow") is Window w)
            {
                bool wasVisible = w.IsVisible;
                w.IsVisible = false;

                foreach (Control c in new List<Control>(w.Controls))
                {
                    GuiManager.RemoveControl(c);
                }

                w.Controls.Clear(); // just in case

                if (!string.IsNullOrEmpty(Character.CurrentCharacter.ResistsData))
                {
                    string[] resists = Character.CurrentCharacter.ResistsData.Split(Protocol.VSPLIT.ToCharArray());

                    // create resists scrollable text box
                    ScrollableTextBox resistsSTB = new ScrollableTextBox("ResistsScrollableTextBox", "",
                                                   new Rectangle(2, 2, 405, (resists.Length + 2) * (BitmapFont.ActiveFonts[w.Font].LineHeight + 5)), "", Color.White, true, false, w.Font,
                                                   new VisualKey("WhiteSpace"), Color.DarkMagenta, 150, 255, new VisualKey(""), new VisualKey(""), new VisualKey(""), 0, 0,
                                                   BitmapFont.TextAlignment.Left, new List<Enums.EAnchorType>() { }, true)
                    {

                        Colorize = false,
                    };
                    resistsSTB.AddLine("Resists", Enums.ETextType.Default);
                    resistsSTB.AddLine("", Enums.ETextType.Default);
                    foreach (string line in resists)
                    {
                        string[] resist = line.Split(Protocol.ISPLIT.ToCharArray());
                        string withColon = resist[0] + ":";
                        resistsSTB.AddLine(withColon.PadRight(20) + resist[1], Enums.ETextType.Default);
                    }
                    GuiManager.GenericSheet.AddControl(resistsSTB);

                    if (!string.IsNullOrEmpty(Character.CurrentCharacter.ProtectionsData))
                    {
                        string[] protections = Character.CurrentCharacter.ProtectionsData.Split(Protocol.VSPLIT.ToCharArray());

                        // create protections scrollable text box
                        ScrollableTextBox protectionsSTB = new ScrollableTextBox("ProtectionsScrollableTextBox", "",
                                                       new Rectangle(2, resistsSTB.Height + 20, 405, (protections.Length + 2) * (BitmapFont.ActiveFonts[w.Font].LineHeight + 5)), "", Color.White, true, false, w.Font,
                                                       new VisualKey("WhiteSpace"), Color.DarkMagenta, 150, 255, new VisualKey(""), new VisualKey(""), new VisualKey(""), 0, 0,
                                                       BitmapFont.TextAlignment.Left, new List<Enums.EAnchorType>() { }, true)
                        {

                            Colorize = false,
                        };

                        protectionsSTB.AddLine("Protections", Enums.ETextType.Default);
                        protectionsSTB.AddLine("", Enums.ETextType.Default);
                        foreach (string line in protections)
                        {
                            string[] protection = line.Split(Protocol.ISPLIT.ToCharArray());
                            string withColon = protection[0] + ":";
                            protectionsSTB.AddLine(withColon.PadRight(20) + protection[1], Enums.ETextType.Default);
                        }
                        GuiManager.GenericSheet.AddControl(protectionsSTB);
                    }
                }

                w.IsVisible = wasVisible;
            }
        }

        public static void UpdateInventoryWindow()
        {
            if(GuiManager.GenericSheet["InventoryWindow"] is Window w)
            {
                if (Character.CurrentCharacter.Gender == Character.GenderType.Female)
                    w.VisualKey = "Inventory_Female";
                else w.VisualKey = "Inventory_Male";

                Item item = null;

                foreach(Control c in w.Controls)
                {
                    if(c is DragAndDropButton b)
                    {
                        string slot = b.Name.Replace("DragAndDropButton", "");

                        switch(slot)
                        {
                            case "Neck":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Neck, Character.WearOrientation.None);
                                break;
                            case "Head":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Head, Character.WearOrientation.None);
                                break;
                            case "Shoulders":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Shoulders, Character.WearOrientation.None);
                                break;
                            case "Back":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Back, Character.WearOrientation.None);
                                break;
                            case "Face":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Face, Character.WearOrientation.None);
                                break;
                            case "Torso":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Torso, Character.WearOrientation.None);
                                break;
                            case "LeftBicep":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Bicep, Character.WearOrientation.Left);
                                break;
                            case "RightBicep":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Bicep, Character.WearOrientation.Right);
                                break;
                            case "LeftWrist":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Wrist, Character.WearOrientation.Left);
                                break;
                            case "RightWrist":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Wrist, Character.WearOrientation.Right);
                                break;
                            case "Waist":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Waist, Character.WearOrientation.None);
                                break;
                            case "Legs":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Legs, Character.WearOrientation.None);
                                break;
                            case "Feet":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Feet, Character.WearOrientation.None);
                                break;
                            case "Hands":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Hands, Character.WearOrientation.None);
                                break;
                        }

                        if(item != null)
                        {
                            b.VisualKey = item.VisualKey;
                            b.IsLocked = false;
                            b.AcceptingDroppedButtons = false;
                            b.PopUpText = item.Name;
                            b.RepresentedItem = item;
                        }
                        else
                        {
                            b.AcceptingDroppedButtons = true;
                            b.VisualKey = "";
                            b.IsLocked = true;
                            b.PopUpText = "";
                            b.RepresentedItem = null;
                        }
                    }
                }
            }
        }

        public static void UpdateRingsWindow()
        {
            if (GuiManager.GenericSheet["RingsWindow"] is Window w)
            {
                Item item = null;

                foreach (Control c in w.Controls)
                {
                    if (c is DragAndDropButton b)
                    {
                        string slot = b.Name.Replace("DragAndDropButton", "");

                        switch (slot)
                        {
                            case "LeftRing1":
                                item = Character.CurrentCharacter.GetRing(Character.WearOrientation.LeftRing1);
                                break;
                            case "LeftRing2":
                                item = Character.CurrentCharacter.GetRing(Character.WearOrientation.LeftRing2);
                                break;
                            case "LeftRing3":
                                item = Character.CurrentCharacter.GetRing(Character.WearOrientation.LeftRing3);
                                break;
                            case "LeftRing4":
                                item = Character.CurrentCharacter.GetRing(Character.WearOrientation.LeftRing4);
                                break;
                            case "RightRing1":
                                item = Character.CurrentCharacter.GetRing(Character.WearOrientation.RightRing1);
                                break;
                            case "RightRing2":
                                item = Character.CurrentCharacter.GetRing(Character.WearOrientation.RightRing2);
                                break;
                            case "RightRing3":
                                item = Character.CurrentCharacter.GetRing(Character.WearOrientation.RightRing3);
                                break;
                            case "RightRing4":
                                item = Character.CurrentCharacter.GetRing(Character.WearOrientation.RightRing4);
                                break;
                        }

                        if (item != null)
                        {
                            b.VisualKey = item.VisualKey;
                            b.IsLocked = false;
                            b.AcceptingDroppedButtons = false;
                            b.PopUpText = item.Name;
                            b.RepresentedItem = item;
                        }
                        else
                        {
                            b.AcceptingDroppedButtons = true;
                            b.VisualKey = "";
                            b.IsLocked = true;
                            b.PopUpText = "";
                            b.RepresentedItem = null;
                        }
                    }
                }
            }
        }

        public static void UpdateEffectsWindow(Sheet sheet)
        {
            try
            {
                if (sheet["EffectsWindow"] is Window w)
                {
                    w.Controls.RemoveAll(c => c is Label);
                    int size = 40;
                    int spacing = 1;
                    int borderWidth = w.WindowBorder != null ? w.WindowBorder.Width : 0;
                    int labelsPerRow = 8;

                    //if (Character.CurrentCharacter.Effects.Count <= 0)
                    //{
                    //    Label label = new Label("NoEffectsLabel", w.Name, new Rectangle(0, (w.WindowTitle != null ? w.WindowTitle.Height : 0) + 2, BitmapFont.ActiveFonts[w.Font].MeasureString("No Effects"), 30), "No Effects", Color.White, true, false, w.Font,
                    //            new VisualKey("WhiteSpace"), Color.Transparent, 0, 0, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>(), "");

                    //    GuiManager.GenericSheet.AddControl(label);
                    //}
                    if (Character.CurrentCharacter.Effects.Count > 0)
                    {
                        int x = borderWidth + spacing;
                        int y = (w.WindowTitle != null ? w.WindowTitle.Height + spacing : 0) + borderWidth + spacing;

                        foreach (Effect effect in Character.CurrentCharacter.Effects)
                        {
                            VisualKey visual = new VisualKey("WhiteSpace");
                            string text = effect.Name[0].ToString();
                            string effectPopUp = Utils.FormatEnumString(effect.Name);
                            Color tintColor = Color.Transparent;
                            //int count = 0;

                            // time remaining
                            if (effect.Duration > 0)
                            {
                                System.TimeSpan timeRemaining = Utils.RoundsToTimeSpan(effect.Duration);
                                if (timeRemaining < System.TimeSpan.FromMinutes(60))
                                    effectPopUp += " [" + string.Format("{0:D2}", timeRemaining.Minutes) + ":" + string.Format("{0:D2}", timeRemaining.Seconds) + "]";
                                else effectPopUp += " [" + timeRemaining.ToString() + "]";
                            }

                            // visual key of effect/spell if it exists
                            if (Effect.IconsDictionary.ContainsKey(Utils.FormatEnumString(effect.Name)))
                            {
                                visual = new VisualKey(Effect.IconsDictionary[Utils.FormatEnumString(effect.Name)]);
                                text = "";
                                tintColor = Color.White;

                                if (Effect.IconsTintDictionary.ContainsKey(Utils.FormatEnumString(effect.Name)))
                                    tintColor = Effect.IconsTintDictionary[Utils.FormatEnumString(effect.Name)];
                            }

                            EffectLabel label = new EffectLabel(effect.Name + "Label", w.Name, new Rectangle(x, y, size, size), text, Color.White, true, false, w.Font,
                                visual, tintColor, 255, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>(), effectPopUp)
                            {
                                EffectName = effect.Name,
                                TimeCreated = System.DateTime.Now,
                                Duration = effect.Duration,
                                Timeless = effect.Duration <= 0,
                            };

                            GuiManager.CurrentSheet.AddControl(label);

                            Color borderColor = Color.DimGray;

                            if (Effect.NegativeEffects.Contains(effect.Name))
                                borderColor = Color.Red;

                            if (Effect.ShortTermPositiveEffects.Contains(effect.Name))
                                borderColor = Color.Lime;

                            SquareBorder border = new SquareBorder(label.Name + "Border", label.Name, 1, new VisualKey("WhiteSpace"), false, borderColor, 175);
                            GuiManager.CurrentSheet.AddControl(border);

                            x += size + spacing;

                            if (Client.GameDisplayMode == Enums.EGameDisplayMode.Yuusha)
                            {
                                if (x >= labelsPerRow * (size + spacing) + borderWidth + 1)
                                {
                                    x = borderWidth + 1;
                                    y += size + spacing;
                                }
                            }
                        }
                    }

                    int rowCount = 1;
                    if (Client.GameDisplayMode == Enums.EGameDisplayMode.Yuusha && Character.CurrentCharacter.Effects.Count > labelsPerRow)
                    {
                        rowCount = (int)Math.Ceiling(Character.CurrentCharacter.Effects.Count / (decimal)labelsPerRow);
                        w.Width = labelsPerRow * (size + spacing) + spacing + (borderWidth * 2);
                    }
                    else w.Width = Character.CurrentCharacter.Effects.Count * (size + spacing) + spacing + 1;
                    if (w.WindowTitle != null) w.WindowTitle.Width = w.Width;
                    w.Height = (w.WindowTitle != null ? w.WindowTitle.Height : 0) + (rowCount * (size + spacing)) + (borderWidth * 2);

                    if (Client.GameState.ToString().EndsWith("Game") && Character.CurrentCharacter.Effects.Count > 0)
                        w.IsVisible = true;
                    else w.IsVisible = false;
                }
            }
            catch(Exception e)
            {
                Utils.LogException(e);
            }
        }

        public static void UpdateWornEffectsWindow()
        {
            try
            {
                if (GuiManager.GenericSheet["WornEffectsWindow"] is Window w)
                {
                    w.Controls.RemoveAll(c => c is Label);
                    int size = 40;
                    int spacing = 1;
                    int borderWidth = w.WindowBorder != null ? w.WindowBorder.Width : 0;
                    int labelsPerRow = 16;
                    //string font = "robotomonobold11";

                    //if (Character.CurrentCharacter.Effects.Count <= 0)
                    //{
                    //    Label label = new Label("NoEffectsLabel", w.Name, new Rectangle(0, (w.WindowTitle != null ? w.WindowTitle.Height : 0) + 2, BitmapFont.ActiveFonts[w.Font].MeasureString("No Effects"), 30), "No Effects", Color.White, true, false, w.Font,
                    //            new VisualKey("WhiteSpace"), Color.Transparent, 0, 0, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>(), "");

                    //    GuiManager.GenericSheet.AddControl(label);
                    //}
                    if (Character.CurrentCharacter.WornEffects.Count > 0)
                    {
                        
                        int x = borderWidth + spacing;
                        int y = (w.WindowTitle != null ? w.WindowTitle.Height + spacing : 0) + borderWidth + spacing;
                        
                        List<string> AddedEffects = new List<string>();

                        foreach (Effect effect in Character.CurrentCharacter.WornEffects)
                        {
                            if (!AddedEffects.Contains(effect.Name))
                            {
                                VisualKey visual = new VisualKey("WhiteSpace");
                                //string text = effect.Name[0].ToString();
                                string text = effect.Amount.ToString();
                                if (effect.Amount <= 0) text = "";
                                string effectPopUp = Utils.FormatEnumString(effect.Name);
                                Color tintColor = Color.Transparent;
                                //int count = 0;

                                // time remaining
                                if (effect.Duration > 0)
                                {
                                    TimeSpan timeRemaining = Utils.RoundsToTimeSpan(effect.Duration);
                                    if (timeRemaining < System.TimeSpan.FromMinutes(60))
                                        effectPopUp += " [" + string.Format("{0:D2}", timeRemaining.Minutes) + ":" + string.Format("{0:D2}", timeRemaining.Seconds) + "]";
                                    else effectPopUp += " [" + timeRemaining.ToString() + "]";
                                }

                                // visual key of effect/spell if it exists
                                if (Effect.IconsDictionary.ContainsKey(Utils.FormatEnumString(effect.Name)))
                                {
                                    visual = new VisualKey(Effect.IconsDictionary[Utils.FormatEnumString(effect.Name)]);
                                    if (effect.Amount > 0)
                                        text = effect.Amount.ToString();
                                    tintColor = Color.White;

                                    if (Effect.IconsTintDictionary.ContainsKey(Utils.FormatEnumString(effect.Name)))
                                        tintColor = Effect.IconsTintDictionary[Utils.FormatEnumString(effect.Name)];
                                }

                                EffectLabel label = new EffectLabel(effect.Name + "Label", w.Name, new Rectangle(x, y, size, size), text, Color.White, true, false, w.Font,
                                    visual, tintColor, 255, 255, BitmapFont.TextAlignment.Right, 0, 10, "", "", new List<Enums.EAnchorType>(), effectPopUp)
                                {
                                    EffectName = effect.Name,
                                    TimeCreated = DateTime.Now,
                                    Duration = effect.Duration,
                                    Timeless = effect.Duration <= 0,
                                };

                                GuiManager.CurrentSheet.AddControl(label);
                                AddedEffects.Add(effect.Name);

                                Color borderColor = Color.DimGray;
                                if (Effect.NegativeEffects.Contains(effect.Name))
                                    borderColor = Color.Red;

                                SquareBorder border = new SquareBorder(label.Name + "Border", label.Name, 1, new VisualKey("WhiteSpace"), false, borderColor, 175);
                                GuiManager.CurrentSheet.AddControl(border);

                                x += size + spacing;

                                if (Client.GameDisplayMode == Enums.EGameDisplayMode.Yuusha)
                                {
                                    if (x >= labelsPerRow * (size + spacing) + borderWidth + 1)
                                    {
                                        x = borderWidth + 1;
                                        y += size + spacing;
                                    }
                                }
                            }
                            else
                            {
                                if (effect.Amount > 0)
                                {
                                    if (w[effect.Name + "Label"] is Control c)
                                    {
                                        int currentAmount = Convert.ToInt32(c.Text);
                                        c.Text = (currentAmount + effect.Amount).ToString();
                                    }
                                }
                            }
                        }
                    }

                    int rowCount = 1;
                    if (Client.GameDisplayMode == Enums.EGameDisplayMode.Yuusha && Character.CurrentCharacter.WornEffects.Count > labelsPerRow)
                    {
                        rowCount = (int)Math.Ceiling(Character.CurrentCharacter.WornEffects.Count / (decimal)labelsPerRow);
                        w.Width = labelsPerRow * (size + spacing) + spacing + (borderWidth * 2);
                    }
                    else w.Width = Character.CurrentCharacter.WornEffects.Count * (size + spacing) + spacing + 1;
                    if (w.WindowTitle != null) w.WindowTitle.Width = w.Width;
                    w.Height = (w.WindowTitle != null ? w.WindowTitle.Height : 0) + (rowCount * (size + spacing)) + (borderWidth * 2);

                    //if (Client.GameState.ToString().EndsWith("Game") && Character.CurrentCharacter.WornEffects.Count > 0)
                    //    w.IsVisible = true;
                    //else w.IsVisible = false;
                    if(!string.IsNullOrEmpty(w.Owner))
                        w.Width = GuiManager.GetControl(w.Owner).Width;
                }
            }
            catch(Exception e)
            {
                Utils.LogException(e);
            }
        }

        public static void UpdateCritterListWindow()
        {
            //if (GuiManager.CurrentSheet["CritterListWindow"] is Window w)
            //{
            //    List<Control> critterLabels = new List<Control>(w.Controls.RemoveAll(c => !(c is CritterListLabel)));
            //    critterLabels.RemoveAll(c => !c.IsVisible);

            //    int height = w.WindowTitle != null ? w.WindowTitle.Height : 0;

            //    if (critterLabels.Count > 0)
            //        height += critterLabels.Count * critterLabels[0].Height;
            //}
        }

        public static void RemoveCharacterFromCell(int uniqueID)
        {

        }

        //public void MapPortalFade()
        //{

        //}

        //public void Resurrection()
        //{

        //}

        private static bool AcceptingGridBoxIsLocation(string name)
        {
            return name.StartsWith("Ground") || name.StartsWith("Altar") || name.StartsWith("Counter");
        }

        public static void TestChangeMapDisplayWindowSize(int tileSizeChange)
        {
            if (GuiManager.GetControl("MapDisplayWindow") is Window w)
            {
                Rectangle rect = new Rectangle(w.Position.X, w.Position.Y, w.Width, w.Height);

                w.Position = new Point(w.Position.X - tileSizeChange * 7 / 2, w.Position.Y - tileSizeChange * 7 / 2);

                w.Width = ((w["Tile0"].Width + tileSizeChange) * 7) + 4; // 2 pixel padding on each side
                w.Height = ((w["Tile0"].Width + tileSizeChange) * 7) + 4;

                w.OnClientResize(rect, new Rectangle(w.Position.X, w.Position.Y, w.Width, w.Height), false);
            }
        }

        public static void ChangeMapDisplayWindowSize(int tileSizeChange)
        {
            if (GuiManager.GetControl("MapDisplayWindow") is Window w)
            {
                int currentSize = w["Tile0"].Width;

                // Maximum or minimum size decided here...
                if (currentSize + tileSizeChange < MAP_GRID_MINIMUM || currentSize + tileSizeChange > MAP_GRID_MAXIMUM)
                    return;

                List<Control> copiedControls = new List<Control>(w.Controls);

                ChangingMapDisplaySize = true;

                w.Controls.RemoveAll(c => c is SpinelTileLabel);

                w.Position = new Point(w.Position.X - (tileSizeChange / 2 * 7), w.Position.Y - (tileSizeChange / 2 * 7));
                w.Width = ((currentSize + tileSizeChange) * 7) + 4; // 2 pixel padding on each side
                w.Height = ((currentSize + tileSizeChange) * 7) + 4;

                int x = 2;
                int y = 2;
                int count = 0;
                for (int a = 0; a < 49; a++, count++, x += currentSize + tileSizeChange)
                {
                    if (count == 7) // new row, reset column
                    {
                        x = 2;
                        y += currentSize + tileSizeChange;
                        count = 0;
                    }

                    GuiManager.Sheets[w.Sheet].CreateSpinelTileLabel("Tile" + a, w.Name, new Rectangle(x, y, currentSize + tileSizeChange, currentSize + tileSizeChange), "", copiedControls[a].TextColor, copiedControls[a].IsVisible,
                        copiedControls[a].IsDisabled, copiedControls[a].Font, new VisualKey(copiedControls[a].VisualKey), copiedControls[a].TintColor, 255, 255, copiedControls[a].TextAlignment, 0, 0, "", "", new List<Enums.EAnchorType>(), "");
                }

                if (w.WindowBorder != null)
                    w.WindowBorder.Width = w.Width;
                w.WindowBorder.Height = w.Height;

                if (w.WindowTitle != null)
                    w.WindowTitle.Width = w.Width;

                YuushaMode.BuildMap();

                if(GuiManager.GetControl("FogOfWarMapWindow") is MapWindow m)
                {
                    GuiManager.RemoveControl(GuiManager.GetControl("FogOfWarMapWindow"));
                    System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(() => Events.RegisterEvent(Events.EventName.Toggle_FogOfWar));
                    t.Start();
                    t.Wait();
                }

                ChangingMapDisplaySize = false;
            }
        }
    }
}
