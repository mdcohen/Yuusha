﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Yuusha.gui
{
    public class GameHUD : GameComponent
    {
        public static List<string> NonDiscreetlyDraggableWindows = new List<string>()
        {
            //"EffectsWindow",
            "TextVitalsWindow"
        };

        public static Character CurrentTarget;
        public static string TextSendOverride = "";

        public static Enums.EGameState PreviousGameState { get; set; }
        public static Cell ExaminedCell { get; set; }
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
                    if(GuiManager.MouseOverDropAcceptingControl.Name.Substring(0, 2) != b.Name.Substring(0, 2))
                        Events.RegisterEvent(Events.EventName.Send_Command, "swap");
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Ground"))
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
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Ground"))
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
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Ground"))
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
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Ground"))
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

                GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Pouch);
            }
            else if (b.Name.StartsWith("Ground") || b.Name.StartsWith("Altar") || b.Name.StartsWith("Counter"))
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

        public static Cell GetCurrentCharacterCell()
        {
            if (Client.GameState == Enums.EGameState.IOKGame)
                return IOKMode.Cells[24];
            else if (Client.GameState == Enums.EGameState.SpinelGame)
                return SpinelMode.Cells[24];
            else//if (Client.GameState == Enums.EGameState.YuushaGame)
                return YuushaMode.Cells[24];
        }

        public static void UpdateInventoryWindow()
        {
            if(GuiManager.GenericSheet["InventoryWindow"] is Window w)
            {
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

        public static void UpdateEffectsWindow(Sheet sheet)
        {
            if (sheet["EffectsWindow"] is Window w)
            {
                w.Controls.RemoveAll(c => c is Label);
                int size = 30;
                int spacing = 1;

                //if (Character.CurrentCharacter.Effects.Count <= 0)
                //{
                //    Label label = new Label("NoEffectsLabel", w.Name, new Rectangle(0, (w.WindowTitle != null ? w.WindowTitle.Height : 0) + 2, BitmapFont.ActiveFonts[w.Font].MeasureString("No Effects"), 30), "No Effects", Color.White, true, false, w.Font,
                //            new VisualKey("WhiteSpace"), Color.Transparent, 0, 0, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>(), "");

                //    GuiManager.GenericSheet.AddControl(label);
                //}
                if (Character.CurrentCharacter.Effects.Count > 0)
                {
                    int x = 0;
                    int y = (w.WindowTitle != null ? w.WindowTitle.Height : 0) + 2;


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
                        }

                        EffectLabel label = new EffectLabel(effect.Name + "Label", w.Name, new Rectangle(x, y, size, size), text, Color.White, true, false, w.Font,
                            visual, tintColor, 255, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>(), effectPopUp)
                        {
                            EffectName = effect.Name,
                            TimeCreated = System.DateTime.Now,
                            Duration = effect.Duration,
                            Timeless = effect.Duration == 0
                        };

                        GuiManager.CurrentSheet.AddControl(label);

                        SquareBorder border = new SquareBorder(label.Name + "Border", label.Name, 1, new VisualKey("WhiteSpace"), false, Color.DimGray, 175);
                        GuiManager.CurrentSheet.AddControl(border);

                        x += size + spacing;

                        //count++;
                        //if (count == 10) // make another row at 15 effects
                        //{
                        //    x = 0;
                        //    y += size + spacing;
                        //    count = 0;
                        //}
                    }
                }

                w.Width = (Character.CurrentCharacter.Effects.Count * (size + spacing)) - spacing;
                if (w.WindowTitle != null) w.WindowTitle.Width = w.Width;
                w.Height = (w.WindowTitle != null ? w.WindowTitle.Height : 0) + size + spacing;

                if (Client.GameState.ToString().EndsWith("Game") && Character.CurrentCharacter.Effects.Count > 0)
                    w.IsVisible = true;
                else w.IsVisible = false;
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

        //public void MapPortalFade()
        //{

        //}

        //public void Resurrection()
        //{

        //}
    }
}
