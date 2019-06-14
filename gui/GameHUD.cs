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
        public static Enums.EGameState PreviousGameState { get;set; }
        public static Cell ExaminedCell { get; set; }
        public static GridBoxWindow.GridBoxPurpose GridBoxWindowRequestUpdate
        { get; set; }

        // Text is not cleared from scrolling textboxes in these ClientStates (they fill the screen)
        public static List<Enums.EGameState> OverrideDisplayStates = new List<Enums.EGameState>
        {
            Enums.EGameState.CharacterGeneration, Enums.EGameState.HotButtonEditMode
        };

        public GameHUD(Game game): base(game)
        {

        }

        /// <summary>
        /// Handles dropping from a drag and drop area to a drag and drop area.
        /// </summary>
        /// <param name="b">The drag and drop area where an item is being manipulated.</param>
        public static void DragAndDropLogic(DragAndDropButton b)
        {
            // Right hand or left hand items
            if (b.Name.StartsWith("RH") || b.Name.StartsWith("LH"))
            {
                #region Right and Left Hand Items
                string rightOrLeft = b.Name.StartsWith("RH") ? "right" : "left";
                if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Sack"))
                {
                    IO.Send("put " + rightOrLeft + " in sack");
                    IO.Send(Protocol.REQUEST_CHARACTER_SACK);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Belt"))
                {
                    IO.Send("belt " + rightOrLeft);
                    IO.Send(Protocol.REQUEST_CHARACTER_BELT);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Pouch"))
                {
                    IO.Send("put " + rightOrLeft + " in pouch");
                    IO.Send(Protocol.REQUEST_CHARACTER_POUCH);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Locker"))
                {
                    IO.Send("put " + rightOrLeft + " in locker");
                    IO.Send(Protocol.REQUEST_CHARACTER_LOCKER);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH") || GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                {
                    IO.Send("swap");
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Ground"))
                {
                    GridBoxWindow window = GuiManager.MouseOverDropAcceptingControl as GridBoxWindow;
                    if (window.WindowTitle != null)
                    {
                        switch (window.WindowTitle.Text.ToLower())
                        {
                            case "altar":
                                IO.Send("put " + rightOrLeft + " on altar");
                                break;
                            case "counter":
                                IO.Send("put " + rightOrLeft + " on counter");
                                break;
                            default:
                                IO.Send("drop " + rightOrLeft);
                                break;
                        }

                        Cell.SendCellItemsRequest(GameHUD.ExaminedCell);
                    }
                } 
                #endregion
            }
            else if (b.Name.StartsWith("Belt"))
            {
                if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH") || GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                {
                    IO.Send("wield " + b.RepresentedItem.Name);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Locker"))
                {
                    IO.Send("wield " + b.RepresentedItem.Name + ";put " + b.RepresentedItem.Name + " in locker");
                    IO.Send(Protocol.REQUEST_CHARACTER_LOCKER);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Sack"))
                {
                    IO.Send("wield " + b.RepresentedItem.Name + ";put " + b.RepresentedItem.Name + " in sack");
                    IO.Send(Protocol.REQUEST_CHARACTER_SACK);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Pouch"))
                {
                    IO.Send("wield " + b.RepresentedItem.Name + ";put " + b.RepresentedItem.Name + " in pouch");
                    IO.Send(Protocol.REQUEST_CHARACTER_POUCH);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Ground"))
                {
                    GridBoxWindow window = GuiManager.MouseOverDropAcceptingControl as GridBoxWindow;
                    if (window.WindowTitle != null)
                    {
                        switch (window.WindowTitle.Text.ToLower())
                        {
                            case "altar":
                                IO.Send("wield " + b.RepresentedItem.Name + ";put " + b.RepresentedItem.Name + " on altar");
                                break;
                            case "counter":
                                IO.Send("wield " + b.RepresentedItem.Name + ";put " + b.RepresentedItem.Name + " on counter");
                                break;
                            default:
                                IO.Send("wield " + b.RepresentedItem.Name + ";drop " + b.RepresentedItem.Name);
                                break;
                        }

                        Cell.SendCellItemsRequest(GameHUD.ExaminedCell);
                    }
                }

                IO.Send(Protocol.REQUEST_CHARACTER_BELT);
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

                    IO.Send(swapbefore + "take " + b.GetNItemName(b) + " from sack" + swapafter);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Locker"))
                {
                    IO.Send("take " + b.GetNItemName(b) + " from sack; put " + b.RepresentedItem.Name + " in locker");
                    IO.Send(Protocol.REQUEST_CHARACTER_LOCKER);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Belt"))
                {
                    IO.Send("take " + b.GetNItemName(b) + " from sack;belt " + b.RepresentedItem.Name);
                    IO.Send(Protocol.REQUEST_CHARACTER_BELT);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Pouch"))
                {
                    IO.Send("take " + b.GetNItemName(b) + " from sack;put " + b.RepresentedItem.Name + " in pouch");
                    IO.Send(Protocol.REQUEST_CHARACTER_POUCH);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Ground"))
                {
                    GridBoxWindow window = GuiManager.MouseOverDropAcceptingControl as GridBoxWindow;
                    if (window.WindowTitle != null)
                    {
                        switch (window.WindowTitle.Text.ToLower())
                        {
                            case "altar":
                                IO.Send("take " + b.GetNItemName(b) + " from sack;put " + b.RepresentedItem.Name + " on altar");
                                break;
                            case "counter":
                                IO.Send("take " + b.GetNItemName(b) + " from sack;put " + b.RepresentedItem.Name + " on counter");
                                break;
                            default:
                                IO.Send("take " + b.GetNItemName(b) + " from sack;drop " + b.RepresentedItem.Name);
                                break;
                        }

                        Cell.SendCellItemsRequest(GameHUD.ExaminedCell);
                    }
                }

                IO.Send(Protocol.REQUEST_CHARACTER_SACK);
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

                    IO.Send(swapbefore + "take " + b.GetNItemName(b) + " from pouch" + swapafter);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Locker"))
                {
                    IO.Send("take " + b.GetNItemName(b) + " from pouch; put " + b.RepresentedItem.Name + " in locker");
                    IO.Send(Protocol.REQUEST_CHARACTER_LOCKER);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Belt"))
                {
                    IO.Send("take " + b.GetNItemName(b) + " from pouch;belt " + b.RepresentedItem.Name);
                    IO.Send(Protocol.REQUEST_CHARACTER_BELT);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Sack"))
                {
                    IO.Send("take " + b.GetNItemName(b) + " from pouch;put " + b.RepresentedItem.Name + " in sack");
                    IO.Send(Protocol.REQUEST_CHARACTER_SACK);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Ground"))
                {
                    GridBoxWindow window = GuiManager.MouseOverDropAcceptingControl as GridBoxWindow;
                    if (window.WindowTitle != null)
                    {
                        switch (window.WindowTitle.Text.ToLower())
                        {
                            case "altar":
                                IO.Send("take " + b.GetNItemName(b) + " from sack;put " + b.RepresentedItem.Name + " on altar");
                                break;
                            case "counter":
                                IO.Send("take " + b.GetNItemName(b) + " from sack;put " + b.RepresentedItem.Name + " on counter");
                                break;
                            default:
                                IO.Send("take " + b.GetNItemName(b) + " from sack;drop " + b.RepresentedItem.Name);
                                break;
                        }

                        Cell.SendCellItemsRequest(GameHUD.ExaminedCell);
                    }
                }

                IO.Send(Protocol.REQUEST_CHARACTER_POUCH);
            }
            else if(b.Name.StartsWith("Ground"))
            {
                if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH") || GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                {
                    string swapafter = "";
                    string swapbefore = "";

                    if (Character.CurrentCharacter.RightHand == null && GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                        swapafter = ";swap";
                    else if (Character.CurrentCharacter.LeftHand == null && GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH"))
                        swapbefore = "swap;";

                    IO.Send(swapbefore + "take " + b.GetNItemName(b) + swapafter);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Locker"))
                {
                    IO.Send("take " + b.GetNItemName(b) + "; put " + b.RepresentedItem.Name + " in locker");
                    IO.Send(Protocol.REQUEST_CHARACTER_LOCKER);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Belt"))
                {
                    IO.Send("take " + b.GetNItemName(b) + ";belt " + b.RepresentedItem.Name);
                    IO.Send(Protocol.REQUEST_CHARACTER_BELT);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Sack"))
                {
                    IO.Send("take " + b.GetNItemName(b) + ";put " + b.RepresentedItem.Name + " in sack");
                    IO.Send(Protocol.REQUEST_CHARACTER_SACK);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Ground"))
                {
                    GridBoxWindow window = GuiManager.MouseOverDropAcceptingControl as GridBoxWindow;
                    if (window.WindowTitle != null)
                    {
                        switch (window.WindowTitle.Text.ToLower())
                        {
                            case "altar":
                                IO.Send("take " + b.GetNItemName(b) + " from sack;put " + b.RepresentedItem.Name + " on altar");
                                break;
                            case "counter":
                                IO.Send("take " + b.GetNItemName(b) + " from sack;put " + b.RepresentedItem.Name + " on counter");
                                break;
                            default:
                                IO.Send("take " + b.GetNItemName(b) + " from sack;drop " + b.RepresentedItem.Name);
                                break;
                        }

                        Cell.SendCellItemsRequest(GameHUD.ExaminedCell);
                    }
                }

                Cell.SendCellItemsRequest(GameHUD.ExaminedCell);
            }
        }

        public static Cell GetCurrentCharacterCell()
        {
            if (Client.GameState == Enums.EGameState.IOKGame)
                return IOKMode.Cells[24];
            else// if (Client.GameState == Enums.EGameState.SpinelGame)
                return SpinelMode.Cells[24];
        }

        //public void MapPortalFade()
        //{

        //}

        //public void Resurrection()
        //{

        //}
    }
}
