using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace Yuusha.gui
{
    public class FogOfWarWindow : Window
    {
        private static int CURRENT_TILE_SIZE = 50;
        private const int GRIDLINE_SIZE = 1;
        public static int FogAlpha = 150;
        public static Color FogColor = Color.Black;
        public static int EnlargenGridSize = 10;

        //private bool m_savedMap = false;

        private bool m_alwaysBackground = true;
        private int m_prevScrollWheelValue;
        private int m_xMod;
        private int m_yMod;
        public int Columns;
        public int Rows;
        //private Rectangle m_mapViewRectangle;

        //private System.Threading.Tasks.Task m_fogCallingTask;

        //private RenderTarget2D m_mapRender2D;

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

        public FogOfWarWindow(string name, string owner, Rectangle rectangle, bool visible, bool locked, bool disabled,
            string font, VisualKey visualKey, Color tintColor, byte visualAlpha, bool dropShadow,
            Map.Direction shadowDirection, int shadowDistance, List<Enums.EAnchorType> anchors, string cursorOverride)
            : base(name, owner, rectangle, visible, locked, disabled, font, visualKey, tintColor, visualAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride)
        {
            Cells = new Dictionary<XYCoordinate, Cell>();
            Scrollbar = new Scrollbar(m_name + "Scrollbar", 0, 1, 1, 20, 0, false);
        }

        public static void CreateFogOfWarMapWindow()
        {
            Window mapDisplayWindow = GuiManager.GetControl("MapDisplayWindow") as Window;
            CURRENT_TILE_SIZE = (mapDisplayWindow as Window)["Tile0"].Width;
            int x = 0;
            int y = -CURRENT_TILE_SIZE;

            x = mapDisplayWindow.Position.X - (6 * CURRENT_TILE_SIZE) + 2;
            y = mapDisplayWindow.Position.Y - (4 * CURRENT_TILE_SIZE) + 2;

            FogOfWarWindow window = new FogOfWarWindow("FogOfWarMapWindow", "", new Rectangle(x, y, Client.Width, Client.Height + CURRENT_TILE_SIZE), true, true, false,
                "courier28", new VisualKey("WhiteSpace"), Color.Black, 0, false, Map.Direction.None, 0, new List<Enums.EAnchorType>() { Enums.EAnchorType.Center }, "");
         
            GuiManager.Sheets[Enums.EGameState.YuushaGame.ToString()].AddControl(window);

            window.Columns = 22;
            window.Rows = 16;
            window.m_xMod = 9;
            window.m_yMod = 7;

            window.EnlargeGrid(EnlargenGridSize);

            window.CreateGrid();

            //if (Client.IsFullScreen)
            //    window.OnClientResize(Client.PrevClientBounds, Client.NowClientBounds, false);
        }

        public void CreateGrid()
        {
            SpinelLabels.Clear();

            m_updatingGrid = true;

            for(int y = 0; y < Rows * CURRENT_TILE_SIZE; y += CURRENT_TILE_SIZE)
            {
                for(int x = 0; x < Columns * CURRENT_TILE_SIZE; x += CURRENT_TILE_SIZE)
                {
                    SpinelTileLabel sptLabel = new SpinelTileLabel(Name + "SpinelTileLabel" + Controls.Count, Name, new Rectangle(x, y, CURRENT_TILE_SIZE, CURRENT_TILE_SIZE), "", Color.White,
                        true, false, "courier12", new VisualKey("WhiteSpace"), Color.Black, 255, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>() { }, "")
                    {
                        TextAlignment = BitmapFont.TextAlignment.Center
                    };
                    //sptLabel.IsVisible = false;
                    GuiManager.Sheets[Enums.EGameState.YuushaGame.ToString()].AddControl(sptLabel);
                    SpinelLabels.Add(sptLabel);
                }
            }

            Width = Columns * CURRENT_TILE_SIZE;
            Height = Rows * CURRENT_TILE_SIZE;

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

            Columns += amount * 2;
            Rows += amount * 2;
            m_xMod += amount;
            m_yMod += amount;
            Position = new Point(Position.X - (CURRENT_TILE_SIZE * amount), Position.Y - (CURRENT_TILE_SIZE * amount));
            Width += CURRENT_TILE_SIZE * amount * 2;
            Height += CURRENT_TILE_SIZE * amount * 2;
        }

        public override void Update(GameTime gameTime)
        {
            if (m_alwaysBackground)
            {
                //if(GuiManager.DraggedControl != this)
                    ZDepth = 1000; // always in the back
            }

            //if(Position.X < 0 || Position.Y < 0 || Position.X + Width > Client.Width || Position.Y + Height > Client.Height)
            //{
            //    // automatically make labels out of view not visible
            //    foreach(Control c in Controls)
            //    {
            //        if(c is SpinelTileLabel sptLabel)
            //        {
            //            sptLabel.IsVisible = true;
            //            if (Position.X + c.Position.X + c.Width < 0)
            //                sptLabel.IsVisible = false;
            //            if (Position.X + Width - c.Position.X > Client.Width)
            //                sptLabel.IsVisible = false;
            //            if (Position.Y + c.Position.Y + c.Height < 0)
            //                sptLabel.IsVisible = false;
            //            if (Position.Y + Height - c.Position.Y > Client.Height)
            //                sptLabel.IsVisible = false;
            //        }
            //    }
            //}

            base.Update(gameTime);

            //if(GuiManager.GetControl("MapDisplayWindow") is Window mapDispWindow)
            //{
            //    Position.X = mapDispWindow.X
            //}

            if (IsVisible && Character.CurrentCharacter != null)
            {
                // Update if it hasn't been done yet, and if the CurrentCharacter hasn't moved.
                if (LatestUpdateFromCell == null || (Cell.GetCell(Character.CurrentCharacter.X, Character.CurrentCharacter.Y, Character.CurrentCharacter.Z) is Cell cell && cell != LatestUpdateFromCell))
                {
                    if(!GameHUD.ChangingMapDisplaySize)
                        CallUponTheFog();

                    //m_fogCallingTask = new System.Threading.Tasks.Task(CallUponTheFog);
                    //m_fogCallingTask.Start();
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

        //[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
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
                    SpinelLabels[count].FogOfWarDetail.Map = Character.CurrentCharacter.MapID;
                    SpinelLabels[count].FogOfWarDetail.XCord = x;
                    SpinelLabels[count].FogOfWarDetail.YCord = y;
                    SpinelLabels[count].FogOfWarDetail.ZCord = Character.CurrentCharacter.Z;

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
                    if (columnsCount == Columns)
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
                    SpinelLabels[count].FogOfWarDetail.Map = Character.CurrentCharacter.MapID;
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

                    x++;
                    columnsCount++;
                    count++;
                    if (columnsCount == Columns)
                    {
                        y++;
                        x = Character.CurrentCharacter.X - m_xMod;
                        columnsCount = 0;
                    }
                }
            }
        }

        //private void RenderMap()
        //{
        //    try
        //    {
        //        GraphicsDevice device = Program.Client.GraphicsDevice;

        //        m_mapRender2D = new RenderTarget2D(device,
        //            Width, Height, true, SurfaceFormat.Color, DepthFormat.Depth24);
                
        //        device.SetRenderTarget(m_mapRender2D);

        //        Client.SpriteBatch.End();
        //        Client.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

        //        foreach (Control c in Controls)
        //        {
        //            if (c is SpinelTileLabel)
        //            {
        //                c.IsVisible = true;
        //                c.Draw(new GameTime());
        //                c.IsVisible = false;
        //            }
        //        }

        //        Client.SpriteBatch.End();
        //        Client.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
        //        device.SetRenderTarget(null);
        //    }
        //    catch(Exception e)
        //    {
        //        Utils.LogException(e);
        //    }
        //}

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
