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

        // Text is not cleared from scrolling textboxes in these ClientStates (they fill the screen)
        public static List<Enums.EGameState> OverrideDisplayStates = new List<Enums.EGameState>
        {
            Enums.EGameState.CharacterGeneration, Enums.EGameState.HotButtonEditMode
        };

        public GameHUD(Game game): base(game)
        {

        }

        public static void DragAndDropLogic(DragAndDropButton button)
        {
            // Right hand or left hand items
            if (button.Name.StartsWith("RH") || button.Name.StartsWith("LH"))
            {
                string rightOrLeft = button.Name.StartsWith("RH") ? "right" : "left";
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
                else if(GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Ground"))
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
            }
            else if (button.Name.StartsWith("Belt"))
            {
                if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH") || GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                {
                    IO.Send("wield " + button.RepresentedItem.worldItemID);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Locker"))
                {
                    // TODO: unique IDs server side for item manipulation (biiiiig deal)
                    IO.Send("wield " + button.RepresentedItem.worldItemID + ";put " + button.RepresentedItem.worldItemID + " in locker");
                    IO.Send(Protocol.REQUEST_CHARACTER_LOCKER);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Sack"))
                {
                    IO.Send("wield " + button.RepresentedItem.worldItemID + ";put " + button.RepresentedItem.worldItemID + " in sack");
                    IO.Send(Protocol.REQUEST_CHARACTER_SACK);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Pouch"))
                {
                    IO.Send("wield " + button.RepresentedItem.worldItemID + ";put " + button.RepresentedItem.worldItemID + " in pouch");
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
                                IO.Send("wield " + button.RepresentedItem.worldItemID + ";put " + button.RepresentedItem.worldItemID + " on altar");
                                break;
                            case "counter":
                                IO.Send("wield " + button.RepresentedItem.worldItemID + ";put " + button.RepresentedItem.worldItemID + " on counter");
                                break;
                            default:
                                IO.Send("wield " + button.RepresentedItem.worldItemID + ";drop " + button.RepresentedItem.worldItemID);
                                break;
                        }

                        Cell.SendCellItemsRequest(GameHUD.ExaminedCell);
                    }
                }

                IO.Send(Protocol.REQUEST_CHARACTER_BELT);
            }
            else if (button.Name.StartsWith("Sack"))
            {
                if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH") || GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                {
                    string swapafter = "";
                    string swapbefore = "";

                    if (Character.CurrentCharacter.RightHand == null && GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                        swapafter = ";swap";
                    else if (Character.CurrentCharacter.LeftHand == null && GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH"))
                        swapbefore = "swap;";

                    IO.Send(swapbefore + "take " + button.RepresentedItem.worldItemID + " from sack" + swapafter);
                    IO.Send(Protocol.REQUEST_CHARACTER_SACK);
                }
            }
        }

        //public void MapPortalFade()
        //{

        //}

        //public void Resurrection()
        //{

        //}
    }
}
