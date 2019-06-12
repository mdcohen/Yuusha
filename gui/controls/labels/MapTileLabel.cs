using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Yuusha.gui
{
    public class MapTileLabel : Label
    {
        protected bool m_onMouseDownSent = false;

        protected override void OnMouseOver(MouseState ms)
        {
            base.OnMouseOver(ms);

            //var border = new SquareBorder(this.Name + "SquareBorder", this.Name, Client.UserSettings.MapTileBorderSize, new VisualKey("WhiteSpace"), false, Client.UserSettings.ColorMapTileBorder);
            //this.Border = border;
            //TextCue.AddMouseCursorTextCue(this.Name);
        }

        protected override void OnMouseLeave(MouseState ms)
        {
            base.OnMouseLeave(ms);
        }

        protected override void OnMouseDown(MouseState ms)
        {
            if (!m_onMouseDownSent && ms.RightButton == ButtonState.Pressed)
            {
                if (Client.GameState == Enums.EGameState.IOKGame)
                    Cell.SendCellItemsRequest(IOKMode.Cells[System.Convert.ToInt32(this.Name.Replace("Tile", ""))]);
                else if(Client.GameState == Enums.EGameState.SpinelGame)
                    Cell.SendCellItemsRequest(SpinelMode.Cells[System.Convert.ToInt32(this.Name.Replace("Tile", ""))]);

                // if client.client settings echo ground items
                if (Client.ClientSettings.EchoGroundItemsOnExamination)
                {
                    switch (Name.ToLower())
                    {
                        case "tile24":
                            IO.Send("look");
                            // create a pop up list of items on the ground
                            break;
                        case "tile16":
                            if (Text.ToLower() == "==" || Text.ToLower() == "mm")
                            {
                                IO.Send("look on counter");
                            }
                            else IO.Send("look nw");
                            break;
                        case "tile17":
                            if (Text.ToLower() == "==" || Text.ToLower() == "mm")
                            {
                                IO.Send("look on counter");
                            }
                            else IO.Send("look n");
                            break;
                        case "tile18":
                            if (Text.ToLower() == "==" || Text.ToLower() == "mm")
                            {
                                IO.Send("look on counter");
                            }
                            else IO.Send("look ne");
                            break;
                        case "tile23":
                            if (Text.ToLower() == "==" || Text.ToLower() == "mm")
                            {
                                IO.Send("look on counter");
                            }
                            else IO.Send("look w");
                            break;
                        case "tile25":
                            if (Text.ToLower() == "==" || Text.ToLower() == "mm")
                            {
                                IO.Send("look on counter");
                            }
                            else IO.Send("look e");
                            break;
                        case "tile30":
                            if (Text.ToLower() == "==" || Text.ToLower() == "mm")
                            {
                                IO.Send("look on counter");
                            }
                            else IO.Send("look sw");
                            break;
                        case "tile31":
                            if (Text.ToLower() == "==" || Text.ToLower() == "mm")
                            {
                                IO.Send("look on counter");
                            }
                            else IO.Send("look s");
                            break;
                        case "tile32":
                            if (Text.ToLower() == "==" || Text.ToLower() == "mm")
                            {
                                IO.Send("look on counter");
                            }
                            else IO.Send("look se");
                            break;
                    }
                }

                m_onMouseDownSent = true;

                if (m_visuals.ContainsKey(Enums.EControlState.Down))
                    m_visualKey = m_visuals[Enums.EControlState.Down];
            }

            base.OnMouseDown(ms);
        }

        protected override void OnMouseRelease(MouseState ms)
        {
            if (m_disabled)
                return;

            m_onMouseDownSent = false;

            if (m_visuals.ContainsKey(Enums.EControlState.Normal))
                m_visualKey = m_visuals[Enums.EControlState.Normal];

            base.OnMouseRelease(ms);
        }
    }
}
