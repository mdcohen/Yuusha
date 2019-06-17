using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class GameHUD : GameComponent
    {
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
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Ground") || GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Altar") ||
                    GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Counter"))
                {
                    GridBoxWindow window = GuiManager.MouseOverDropAcceptingControl as GridBoxWindow;
                    if (window.WindowTitle != null)
                    {
                        switch (window.WindowTitle.Text.ToLower())
                        {
                            case "altar":
                                Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + fromLocation + ";put " + b.RepresentedItem.Name + " on altar");
                                break;
                            case "counter":
                                Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + fromLocation + ";put " + b.RepresentedItem.Name + " on counter");
                                break;
                            default:
                                Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + fromLocation + ";drop " + b.RepresentedItem.Name);
                                break;
                        }

                        GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Ground);
                    }
                }

                GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Ground);
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

        //public void MapPortalFade()
        //{

        //}

        //public void Resurrection()
        //{

        //}
    }
}
