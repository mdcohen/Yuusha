using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Yuusha.gui
{
    public class MapTileLabel : Label
    {
        private readonly List<string> CellsTouchingMe = new List<string>()
        {
            "16", "17", "18", "23", "24", "25", "30", "31", "32"
        };

        protected bool m_onMouseDownSent = false;
        public virtual Cell Cell
        {
            get { return GameHUD.Cells[int.Parse(Name.Replace("Tile", ""))]; }
        }

        protected override void OnMouseLeave(MouseState ms)
        {
            base.OnMouseLeave(ms);

            Border = null;
        }

        protected override void OnMouseDown(MouseState ms)
        {
            if (Owner == "MapDisplayWindow")
            {
                if (!m_onMouseDownSent && ms.RightButton == ButtonState.Pressed)
                {
                    Int32.TryParse(Name.Replace("Tile", ""), out int num);

                    if (GameHUD.Cells.Count >= num + 1)
                    {
                        Cell cell = GameHUD.Cells[num];
                        if (cell.IsExaminable())
                        {
                            Cell.SendCellItemsRequest(cell);

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
                        }
                    }

                    if (m_visuals.ContainsKey(Enums.EControlState.Down))
                        m_visualKey = m_visuals[Enums.EControlState.Down];

                }
                else if (!m_onMouseDownSent && ms.LeftButton == ButtonState.Pressed)
                {
                    //string directionsToTile = GetMovementDirections();

                    //if(!string.IsNullOrEmpty(directionsToTile))
                    //    gui.TextCue.AddClientInfoTextCue(GetMovementDirections().ToUpper());

                    //List<string> directionsToTile = new List<string>(GetMovementDirections().Split(" ".ToCharArray()));

                    //foreach(string dir in directionsToTile)
                    //{
                    //    // get corresponding SpinelTileLabel and place footprints
                    //}
                }
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

        protected override void OnDoubleLeftClick()
        {
            try
            {
                if (!Name.StartsWith("Tile") || GameHUD.Cells.Count < Convert.ToInt32(Name.Replace("Tile", "")) + 1)
                    return;
                else if (Name.EndsWith("24") && Character.CurrentCharacter.Cell.DisplayGraphic == Cell.GRAPHIC_DOWNSTAIRS)
                {
                    IO.Send("d");
                    return;
                }
                else if(Name.EndsWith("24") && Character.CurrentCharacter.Cell.DisplayGraphic == Cell.GRAPHIC_UPSTAIRS)
                {
                    IO.Send("u");
                    return;
                }
                else if(Name.EndsWith("24") && Character.CurrentCharacter.Cell.IsPortal)
                {
                    IO.Send(TextManager.PORTAL_CHANT);
                }
                else if(CellsTouchingMe.Contains(Name.Replace("Tile", "")) && GameHUD.Cells[int.Parse(Name.Replace("Tile", ""))].IsSearchable())
                {
                    IO.Send("search " + Map.GetDirection(GameHUD.Cells[24], GameHUD.Cells[int.Parse(Name.Replace("Tile", ""))]));
                }
                // Work on searching for secret doors / rock passageways
                else if (GetMovementDirections() is string directionsToSend && !string.IsNullOrEmpty(directionsToSend))
                    IO.Send(directionsToSend.TrimEnd());
            }
            catch (Exception e)
            {
                Utils.LogException(e);
                Utils.Log(Name.ToUpper());
            }
        }

        private string GetMovementDirections()
        {
            if (!Name.StartsWith("Tile") || GameHUD.Cells.Count < Convert.ToInt32(Name.Replace("Tile", "")) + 1)
                return "";

            Cell moveToCell = GameHUD.Cells[Convert.ToInt32(Name.Replace("Tile", ""))];

            if (moveToCell == null || !moveToCell.IsVisible || moveToCell.MovementWeight() >= 10000)
                return "";

            XYCoordinate currXY = new XYCoordinate(Character.CurrentCharacter.X, Character.CurrentCharacter.Y);
            XYCoordinate destXY = new XYCoordinate(moveToCell.xCord, moveToCell.yCord);
            string directionsToSend = "";
            bool destinationReached = false;
            int count = 0; // safety net for the while loop

            while (!destinationReached && count < 3)
            {
                Map.Direction destinationDirection = Map.GetDirection(currXY, destXY);

                List<Tuple<int, int, string>> tup = new List<Tuple<int, int, string>>();

                switch (destinationDirection)
                {
                    case Map.Direction.North:
                        tup.Add(Tuple.Create(0, -1, "n")); // North
                        tup.Add(Tuple.Create(-1, -1, "nw")); // Northwest
                        tup.Add(Tuple.Create(1, -1, "ne")); // Northeast
                        break;
                    case Map.Direction.Northwest:
                        tup.Add(Tuple.Create(-1, -1, "nw")); // Northwest
                        tup.Add(Tuple.Create(-1, 0, "w")); // West
                        tup.Add(Tuple.Create(0, -1, "n")); // North
                        break;
                    case Map.Direction.Northeast:
                        tup.Add(Tuple.Create(1, -1, "ne")); // Northeast
                        tup.Add(Tuple.Create(1, 0, "e")); // East
                        tup.Add(Tuple.Create(0, -1, "n")); // North                            
                        break;
                    case Map.Direction.West:
                        tup.Add(Tuple.Create(-1, 0, "w")); // West
                        tup.Add(Tuple.Create(-1, -1, "nw")); // Northwest                                
                        tup.Add(Tuple.Create(-1, 1, "sw")); // Southwest
                        break;
                    case Map.Direction.East:
                        tup.Add(Tuple.Create(1, 0, "e")); // East
                        tup.Add(Tuple.Create(1, -1, "ne")); // Northeast                                
                        tup.Add(Tuple.Create(1, 1, "se")); // Southeast
                        break;
                    case Map.Direction.South:
                        tup.Add(Tuple.Create(0, 1, "s")); // South
                        tup.Add(Tuple.Create(-1, 1, "sw")); // Southwest                            
                        tup.Add(Tuple.Create(1, 1, "se")); // Southeast
                        break;
                    case Map.Direction.Southeast:
                        tup.Add(Tuple.Create(1, 1, "se")); // Southeast
                        tup.Add(Tuple.Create(0, 1, "s")); // South
                        tup.Add(Tuple.Create(1, 0, "e")); // East                                
                        break;
                    case Map.Direction.Southwest:
                        tup.Add(Tuple.Create(-1, 1, "sw")); // Southwest
                        tup.Add(Tuple.Create(-1, 0, "w")); // West
                        tup.Add(Tuple.Create(0, 1, "s")); // South
                        break;
                }

                string direction = "";
                int weight = 10000;

                //List<Tuple<int, string>> DirectionsList = new List<Tuple<int, string>>();

                for (int i = 0; i < tup.Count; i++)
                {
                    direction = "";
                    Cell check = Cell.GetCell(currXY.X + tup[i].Item1, currXY.Y + tup[i].Item2, Character.CurrentCharacter.Z);
                    if (check != null)
                    {
                        if (check.MovementWeight() < 10000)
                        {
                            if (direction == "")
                            {
                                direction = tup[i].Item3;
                                weight = check.MovementWeight();
                                currXY.X = check.xCord;
                                currXY.Y = check.yCord;
                            }
                            else if (check.MovementWeight() < weight)
                            {
                                direction = tup[i].Item3;
                                currXY.X = check.xCord;
                                currXY.Y = check.yCord;
                            }
                        }

                        if (direction != "" && weight == 0)
                            break;
                    }
                }

                //if (direction != "")
                    directionsToSend += direction + " ";

                if (currXY.X == destXY.X && currXY.Y == destXY.Y)
                    destinationReached = true;

                count++;
            }

            return directionsToSend.TrimEnd();
        }
    }
}
