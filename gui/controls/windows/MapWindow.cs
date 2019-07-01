using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace Yuusha.gui
{
    public class MapWindow : Window
    {
        private const int DEFAULT_TILE_SIZE = 50;
        private const int GRIDLINE_SIZE = 1;
        public static int FogAlpha = 150;
        public static Color FogColor = Color.Black;

        private bool m_savedMap = false;

        private bool m_alwaysBackground = true;
        private int m_prevScrollWheelValue;
        private int m_xMod;
        private int m_yMod;
        private int m_columns;
        private int m_rows;
        private Rectangle m_mapViewRectangle;

        private System.Threading.Tasks.Task m_fogCallingTask;

        private RenderTarget2D m_mapRender2D;

        private bool m_updatingGrid = false;

        private List<SpinelTileLabel> SpinelLabels
        { get; } = new List<SpinelTileLabel>();
        public Dictionary<XYCoordinate, Cell> Cells
        { get; set; }
        public Cell LatestUpdateFromCell
        { get; set; }
        public Scrollbar Scrollbar
        { get; set; }
        public Point OriginalPosition
        { get; set; }

        public MapWindow(string name, string owner, Rectangle rectangle, bool visible, bool locked, bool disabled,
            string font, VisualKey visualKey, Color tintColor, byte visualAlpha, bool dropShadow,
            Map.Direction shadowDirection, int shadowDistance, List<Enums.EAnchorType> anchors, string cursorOverride)
            : base(name, owner, rectangle, visible, locked, disabled, font, visualKey, tintColor, visualAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride)
        {
            Cells = new Dictionary<XYCoordinate, Cell>();
            Scrollbar = new Scrollbar(m_name + "Scrollbar", 0, 1, 1, 20, 0, false);
        }

        public static void CreateFogOfWarMapWindow()
        {
            // -50 offset because that's where the map starts.
            // 0, -50
            MapWindow window = new MapWindow("FogOfWarMapWindow", "", new Rectangle(0, -50, Client.Width, Client.Height + 50), true, true, false, "courier28", new gui.VisualKey("WhiteSpace"), Color.Black,
                255, false, Map.Direction.None, 0, new List<Enums.EAnchorType>() { Enums.EAnchorType.Center }, "");
         
            GuiManager.Sheets[Enums.EGameState.YuushaGame.ToString()].AddControl(window);

            window.m_columns = 22; // 22
            window.m_rows = 16; // 16
            window.m_xMod = 9; // 9
            window.m_yMod = 7; // 7 (-50 position offset)

            //window.EnlargeGrid(20);

            window.CreateGrid();
        }

        private void CreateGrid()
        {
            m_updatingGrid = true;

            for(int y = 0; y < m_rows * DEFAULT_TILE_SIZE; y += DEFAULT_TILE_SIZE)
            {
                for(int x = 0; x < m_columns * DEFAULT_TILE_SIZE; x += DEFAULT_TILE_SIZE)
                {
                    SpinelTileLabel sptLabel = new SpinelTileLabel(Name + "SpinelTileLabel" + Controls.Count, Name, new Rectangle(x, y, DEFAULT_TILE_SIZE, DEFAULT_TILE_SIZE), "", Color.White,
                        true, false, "courier12", new gui.VisualKey("WhiteSpace"), Color.Black, 255, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>(), "");

                    sptLabel.TextAlignment = BitmapFont.TextAlignment.Center;
                    //sptLabel.IsVisible = false;
                    GuiManager.Sheets[Enums.EGameState.YuushaGame.ToString()].AddControl(sptLabel);
                    SpinelLabels.Add(sptLabel);
                }
            }

            Width = m_columns * DEFAULT_TILE_SIZE;
            Height = m_rows * DEFAULT_TILE_SIZE;

            m_updatingGrid = false;
        }

        public void EnlargeGrid(int amount)
        {
            foreach(Control c in new List<Control>(Controls))
            {
                if (c is SpinelTileLabel)
                    GuiManager.RemoveControl(c);
            }

            SpinelLabels.Clear();

            m_columns += amount * 2;
            m_rows += amount;
            m_xMod += amount;
            m_yMod += amount;
            Position = new Point(Position.X - (DEFAULT_TILE_SIZE * amount), Position.Y - (DEFAULT_TILE_SIZE * amount));
            Width += DEFAULT_TILE_SIZE * (amount * 2);
            Height += DEFAULT_TILE_SIZE * (amount * 2);
        }

        public override void Update(GameTime gameTime)
        {
            if (m_alwaysBackground)
            {
                if(GuiManager.DraggedControl != this)
                    ZDepth = 1000; // always in the back
            }

            base.Update(gameTime);

            if (IsVisible && Character.CurrentCharacter != null)
            {
                // Update if it hasn't been done yet, and if the CurrentCharacter hasn't moved.
                if (LatestUpdateFromCell == null || (Cell.GetCell(Character.CurrentCharacter.X, Character.CurrentCharacter.Y, Character.CurrentCharacter.Z) is Cell cell && cell != LatestUpdateFromCell))
                {
                    m_fogCallingTask = new System.Threading.Tasks.Task(CallUponTheFog);
                    m_fogCallingTask.Start();
                }
            }
        }

        //public override void Draw(GameTime gameTime)
        //{
        //    if (!IsVisible) return;

        //    base.Draw(gameTime);

        //    if (m_fogCallingTask != null && m_fogCallingTask.IsCompleted)
        //        RenderMap();

        //    try
        //    {
        //        //if (!m_savedMap && m_mapRender2D != null)
        //        //{
        //        //    string fileName = "map_" +
        //        //    DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Year + DateTime.Now.Hour + DateTime.Now.Minute +
        //        //    DateTime.Now.Second + DateTime.Now.Millisecond + ".png";
        //        //    try
        //        //    {
        //        //        //map.SaveAsPng(new System.IO.FileStream(Utils.StartupPath + Utils.LogsFolder + fileName, System.IO.FileMode.CreateNew), Width, Height);
        //        //        m_mapRender2D.SaveAsPng(new System.IO.FileStream(Utils.StartupPath + Utils.LogsFolder + fileName, System.IO.FileMode.CreateNew), Width, Height);
        //        //    }
        //        //    catch (Exception e)
        //        //    {
        //        //        Utils.LogException(e);
        //        //    }
        //        //    m_savedMap = true;
        //        //}

        //        if (m_mapRender2D != null && !m_mapRender2D.IsDisposed)
        //        {
        //            Client.SpriteBatch.Draw(m_mapRender2D, m_rectangle, Color.White);
        //        }
        //    }
        //    catch(Exception e)
        //    {
        //        Utils.LogException(e);
        //    }
        //}

        private void CallUponTheFog()
        {
            if (Character.CurrentCharacter == null || m_updatingGrid) return;

            LatestUpdateFromCell = Cell.GetCell(Character.CurrentCharacter.X, Character.CurrentCharacter.Y, Character.CurrentCharacter.Z);

            int x = Character.CurrentCharacter.X - m_xMod; // at position 0,0 (Control[0] 
            int y = Character.CurrentCharacter.Y - m_yMod; // at position 0,0
            int count = 0;
            int columnsCount = 0;
            //Control mainMapWindow = GuiManager.GetControl("MapDisplayWindow");
            
            if (Client.GameState == Enums.EGameState.SpinelGame || Client.GameState == Enums.EGameState.YuushaGame)
            {
                SpinelTileDefinition currentTile;
                foreach (SpinelTileLabel sptLabel in SpinelLabels)
                {
                    //if (!mainMapWindow.Contains(new Point(sptLabel.Position.X + 5, sptLabel.Position.Y + 5)))
                    //{
                        SpinelLabels[count].FogOfWarDetail.Map = Character.CurrentCharacter.m_mapID;
                        SpinelLabels[count].FogOfWarDetail.XCord = x;
                        SpinelLabels[count].FogOfWarDetail.YCord = y;
                        SpinelLabels[count].FogOfWarDetail.ZCord = Character.CurrentCharacter.Z;
                        //SpinelLabels[count].PopUpText = x + "," + y;
                        //SpinelLabels[count].Text = "[]";

                        if (Character.FogOfWarSettings.FogOfWar.Contains(SpinelLabels[count].FogOfWarDetail))
                        {
                            if (YuushaMode.Tiles.ContainsKey(Character.FogOfWarSettings.GetFogOfWarDetail(SpinelLabels[count].FogOfWarDetail).DisplayGraphic))
                                currentTile = YuushaMode.Tiles[Character.FogOfWarSettings.GetFogOfWarDetail(SpinelLabels[count].FogOfWarDetail).DisplayGraphic];
                            else
                            {
                                Utils.LogOnce("Failed to find SpinelTileDefinition for cell graphic [ " + SpinelLabels[count].FogOfWarDetail.DisplayGraphic + " ]");
                                currentTile = YuushaMode.Tiles["  "];
                            }

                            SpinelLabels[count].Text = "";
                            SpinelLabels[count].TextColor = Color.White;
                            SpinelLabels[count].TextAlpha = 255;
                            SpinelLabels[count].TintColor = currentTile.BackTint;
                            SpinelLabels[count].VisualKey = currentTile.BackVisual.Key;
                            SpinelLabels[count].VisualAlpha = currentTile.BackAlpha;
                            SpinelLabels[count].ForeVisual = currentTile.ForeVisual.Key;
                            SpinelLabels[count].ForeColor = currentTile.ForeTint;
                            SpinelLabels[count].ForeAlpha = currentTile.ForeAlpha;

                            if (Cell.GetCell(x, y, Character.CurrentCharacter.Z) is Cell cell && cell.IsPortal)
                                SpinelLabels[count].VisualKey = YuushaMode.Tiles["pp"].ForeVisual.Key;

                            SpinelLabels[count].FogVisual = "WhiteSpace";
                        }
                        else
                        {
                            currentTile = YuushaMode.Tiles["  "];

                            SpinelLabels[count].Text = "";
                            SpinelLabels[count].TextColor = Color.White;
                            SpinelLabels[count].TextAlpha = 0;
                            SpinelLabels[count].TintColor = Color.PowderBlue;
                            SpinelLabels[count].VisualKey = "";
                            SpinelLabels[count].VisualAlpha = 0;
                            SpinelLabels[count].ForeVisual = "";
                            SpinelLabels[count].ForeColor = Color.LemonChiffon;
                            SpinelLabels[count].ForeAlpha = 0;

                            //if (Cell.GetCell(x, y) is Cell cell && cell.portal)
                            //    SpinelLabels[count].VisualKey = YuushaMode.Tiles["pp"].ForeVisual.Key;

                            SpinelLabels[count].FogVisual = "";
                        }
                    //}

                    x++;
                    columnsCount++;
                    count++;
                    if (columnsCount == m_columns)
                    {
                        y++;
                        x = Character.CurrentCharacter.X - m_xMod;
                        columnsCount = 0;
                    }
                }
            }
            else
            {
                IOKTileDefinition currentTile;
                foreach (SpinelTileLabel sptLabel in SpinelLabels)
                {
                    //if (!mainMapWindow.Contains(new Point(sptLabel.Position.X + 5, sptLabel.Position.Y + 5)))
                    //{
                        SpinelLabels[count].FogOfWarDetail.Map = Character.CurrentCharacter.m_mapID;
                        SpinelLabels[count].FogOfWarDetail.XCord = x;
                        SpinelLabels[count].FogOfWarDetail.YCord = y;
                        SpinelLabels[count].FogOfWarDetail.ZCord = Character.CurrentCharacter.Z;

                        if (Character.FogOfWarSettings.FogOfWar.Contains(SpinelLabels[count].FogOfWarDetail))
                        {
                            if (IOKMode.Tiles.ContainsKey(Character.FogOfWarSettings.GetFogOfWarDetail(SpinelLabels[count].FogOfWarDetail).DisplayGraphic))
                                currentTile = IOKMode.Tiles[Character.FogOfWarSettings.GetFogOfWarDetail(SpinelLabels[count].FogOfWarDetail).DisplayGraphic];
                            else
                            {
                                Utils.LogOnce("Failed to find IOKTileDefinition for cell graphic [ " + SpinelLabels[count].FogOfWarDetail.DisplayGraphic + " ]");
                                currentTile = IOKMode.Tiles["  "];
                            }

                            SpinelLabels[count].CreatureText = ""; // clear creature text;
                            SpinelLabels[count].LootText = ""; // clear loot text;
                            SpinelLabels[count].Font = "courier28";
                            SpinelLabels[count].TextAlignment = BitmapFont.TextAlignment.Center;
                            SpinelLabels[count].Text = currentTile.DisplayGraphic;
                            SpinelLabels[count].TextColor = currentTile.ForeColor;
                            SpinelLabels[count].TintColor = currentTile.BackColor;
                            SpinelLabels[count].TextAlpha = currentTile.ForeAlpha;
                            SpinelLabels[count].VisualAlpha = currentTile.BackAlpha;

                            SpinelLabels[count].VisualKey = "WhiteSpace";
                            SpinelLabels[count].ForeVisual = "";

                            //if (Cell.GetCell(x, y) is Cell cell && cell.portal)
                            //    SpinelLabels[count].VisualKey = IOKMode.Tiles["pp"].ForeVisual.Key;

                            SpinelLabels[count].FogVisual = "WhiteSpace";
                        }
                        else
                        {
                            currentTile = IOKMode.Tiles["  "];

                            SpinelLabels[count].Text = "";
                            SpinelLabels[count].TextColor = Color.White;
                            SpinelLabels[count].TextAlpha = 0;
                            SpinelLabels[count].TintColor = Color.PowderBlue;

                            SpinelLabels[count].VisualKey = "";

                            SpinelLabels[count].VisualAlpha = 0;
                            SpinelLabels[count].ForeVisual = "";
                            SpinelLabels[count].ForeColor = Color.LemonChiffon;
                            //SpinelLabels[count].ForeColor = currTile.ForeTint;
                            SpinelLabels[count].ForeAlpha = 0;
                            ///SpinelLabels[count].ForeAlpha = currTile.ForeAlpha;

                            //if (Cell.GetCell(x, y) is Cell cell && cell.portal)
                            //    SpinelLabels[count].VisualKey = IOKMode.Tiles["pp"].ForeVisual.Key;

                            SpinelLabels[count].FogVisual = "";
                        }
                    //}

                    x++;
                    columnsCount++;
                    count++;
                    if (columnsCount == m_columns)
                    {
                        y++;
                        x = Character.CurrentCharacter.X - m_xMod;
                        columnsCount = 0;
                    }
                }
            }
        }

        private void RenderMap()
        {
            try
            {
                GraphicsDevice device = Program.Client.GraphicsDevice;

                m_mapRender2D = new RenderTarget2D(device,
                    Width, Height, true, SurfaceFormat.Color, DepthFormat.Depth24);
                
                device.SetRenderTarget(m_mapRender2D);

                Client.SpriteBatch.End();
                Client.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                foreach (Control c in Controls)
                {
                    if (c is SpinelTileLabel)
                    {
                        c.IsVisible = true;
                        c.Draw(new GameTime());
                        c.IsVisible = false;
                    }
                }

                Client.SpriteBatch.End();
                Client.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
                device.SetRenderTarget(null);
            }
            catch(Exception e)
            {
                Utils.LogException(e);
            }
        }

        protected override void OnMouseDown(MouseState ms)
        {
            base.OnMouseDown(ms);

            //FogAlpha += 10;
            //if (FogAlpha > 255) FogAlpha = 0;
            //TextCue.AddClientInfoTextCue("FogAlpha: " + FogAlpha);

            //if (ms.LeftButton == ButtonState.Pressed)
            //    TextCue.AddClientInfoTextCue("FogOfWar Count: " + Character.FogOfWarSettings.FogOfWar.Count);
        }

        protected override void OnZDelta(MouseState ms)
        {
            if (Scrollbar != null && !Scrollbar.IsDisabled)
            {
                int prev = GuiManager.CurrentSheet.PreviousScrollWheelValue;
                int curr = ms.ScrollWheelValue;
                int diff = Math.Max(prev, curr) - Math.Min(prev, curr);

                if (Math.Max(diff, m_prevScrollWheelValue) -
                    Math.Min(diff, m_prevScrollWheelValue) == 0)
                    Scrollbar.ScrollValue = 0;
                else if (prev < curr)
                    Scrollbar.ScrollValue += diff / 120;
                else if (prev > curr)
                    Scrollbar.ScrollValue -= diff / 120;

                // save scroll wheel value
                m_prevScrollWheelValue = ms.ScrollWheelValue;
            }
        }

        [System.Serializable]
        public class FogOfWarDetail
        {
            public int Map
            { get; set; }

            public int XCord
            { get; set; }

            public int YCord
            { get; set; }

            public int ZCord
            { get; set; }

            public string DisplayGraphic
            { get; set; }

            public FogOfWarDetail() { }
            public FogOfWarDetail(int m, int x, int y, int z, string g) : this() { Map = m; XCord = x; YCord = y; ZCord = z; DisplayGraphic = g; }

            public override bool Equals(object obj)
            {
                if (obj is FogOfWarDetail fog)
                {
                    if (Map == fog.Map && XCord == fog.XCord && YCord == fog.YCord && ZCord == fog.ZCord)
                        return true;
                    else return false;
                }
                else return false;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
    }
}
