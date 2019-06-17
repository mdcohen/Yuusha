using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Yuusha.gui
{
    public class MapWindow : Window
    {
        private const int DEFAULT_TILE_SIZE = 50;
        private const int GRIDLINE_SIZE = 1;
        public static int FogAlpha = 150;
        public static Color FogColor = Color.Black;

        private bool AlwaysBackground = true;

        private int Columns;
        private int Rows;

        private List<SpinelTileLabel> SpinelLabels
        { get; } = new List<SpinelTileLabel>();

        public Dictionary<XYCoordinate, Cell> Cells
        { get; set; }

        public Cell LatestUpdateFromCell
        { get; set; }

        public MapWindow(string name, string owner, Rectangle rectangle, bool visible, bool locked, bool disabled,
            string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, bool dropShadow,
            Map.Direction shadowDirection, int shadowDistance, List<Enums.EAnchorType> anchors, string cursorOverride)
            : base(name, owner, rectangle, visible, locked, disabled, font, visualKey, tintColor, visualAlpha, borderAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride)
        {
            Cells = new Dictionary<XYCoordinate, Cell>();
        }

        public static void CreateMapWindow()
        {
            //if (!Client.ClientSettings.FogOfWar)
            //    return;

            MapWindow window = new MapWindow("PrimaryMapWindow", "", new Rectangle(0, 0, Client.Width, Client.Height), true, true, false, "courier28", new gui.VisualKey("WhiteSpace"), Color.Transparent,
                255, 0, false, Map.Direction.None, 0, new List<Enums.EAnchorType>() { Enums.EAnchorType.Center }, "");
         
            GuiManager.CurrentSheet.AddControl(window);
            window.Columns = 22;
            window.Rows = 16;
            window.CreateGrid(window.Columns, window.Rows);
            window.CallUponFog();
        }

        public void CreateGrid(int columns, int rows)
        {
            for(int y = 0; y < rows * DEFAULT_TILE_SIZE; y += DEFAULT_TILE_SIZE)
            {
                for(int x = 0; x < columns * DEFAULT_TILE_SIZE; x += DEFAULT_TILE_SIZE)
                {
                    SpinelTileLabel sptLabel = new SpinelTileLabel(Name + "SpinelTileLabel" + Controls.Count, Name, new Rectangle(x, y, DEFAULT_TILE_SIZE, DEFAULT_TILE_SIZE), "", Color.White,
                        true, false, "courier12", new gui.VisualKey("WhiteSpace"), Color.Black, 255, 255, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>(), "");

                    sptLabel.TextAlignment = BitmapFont.TextAlignment.Center;
                    GuiManager.CurrentSheet.AddControl(sptLabel);
                    SpinelLabels.Add(sptLabel);
                }
            }

            Width = columns * DEFAULT_TILE_SIZE;
            Height = rows * DEFAULT_TILE_SIZE;
        }

        public void EnlargeGrid(int amount)
        {

        }

        public override void Update(GameTime gameTime)
        {
            if (AlwaysBackground)
            {
                if(GuiManager.DraggedControl != this)
                    ZDepth = 1000; // always in the back
            }

            if (Character.CurrentCharacter != null)
            {
                if (LatestUpdateFromCell == null || LatestUpdateFromCell != Cell.GetCell(Character.CurrentCharacter.X, Character.CurrentCharacter.Y))
                {
                    CallUponFog();
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public void ToggleGrid()
        {
            if (TintColor == Color.Transparent)
                TintColor = Color.DimGray;
            else TintColor = Color.Transparent;
        }

        private Cell GetCell(int x, int y)
        {
            XYCoordinate xy = new XYCoordinate(x, y);

            if (Cells.ContainsKey(xy))
                return Cells[xy];
            else return null;
        }

        public void CallUponFog()
        {
            if (Character.CurrentCharacter == null) return;

            LatestUpdateFromCell = Cell.GetCell(Character.CurrentCharacter.X, Character.CurrentCharacter.Y);

            int x = Character.CurrentCharacter.X - 9; // at position 0,0
            int y = Character.CurrentCharacter.Y - 7; // at position 0,0
            int count = 0;
            int columnsCount = 0;
            VisualInfo backVI = GuiManager.Visuals["WhiteSpace"];
            SpinelTileDefinition currTile;

            foreach (SpinelTileLabel sptLabel in SpinelLabels)
            {
                SpinelLabels[count].FogOfWarDetail.Map = Character.CurrentCharacter.m_mapID;
                SpinelLabels[count].FogOfWarDetail.XCord = x;
                SpinelLabels[count].FogOfWarDetail.YCord = y;
                SpinelLabels[count].FogOfWarDetail.ZCord = Character.CurrentCharacter.Z;
                //SpinelLabels[count].PopUpText = x + "," + y;
                //SpinelLabels[count].Text = "[]";

                if (Character.FogOfWarSettings.FogOfWar.Contains(SpinelLabels[count].FogOfWarDetail))
                {
                    if (YuushaMode.Tiles.ContainsKey(Character.FogOfWarSettings.GetFogOfWarDetail(SpinelLabels[count].FogOfWarDetail).DisplayGraphic))
                        currTile = YuushaMode.Tiles[Character.FogOfWarSettings.GetFogOfWarDetail(SpinelLabels[count].FogOfWarDetail).DisplayGraphic];
                    else
                    {
                        Utils.LogOnce("Failed to find SpinelTileDefinition for cell graphic [ " + SpinelLabels[count].FogOfWarDetail.DisplayGraphic + " ]");
                        currTile = YuushaMode.Tiles["  "];
                    }

                    SpinelLabels[count].Text = "";
                    SpinelLabels[count].TextColor = Color.White;
                    SpinelLabels[count].TextAlpha = 255;
                    SpinelLabels[count].TintColor = currTile.BackTint;
                    SpinelLabels[count].VisualKey = currTile.BackVisual.Key;
                    SpinelLabels[count].VisualAlpha = currTile.BackAlpha;
                    SpinelLabels[count].ForeVisual = currTile.ForeVisual.Key;
                    SpinelLabels[count].ForeColor = currTile.ForeTint;
                    SpinelLabels[count].ForeAlpha = currTile.ForeAlpha;

                    if (Cell.GetCell(x, y) is Cell cell && cell.portal)
                        SpinelLabels[count].VisualKey = YuushaMode.Tiles["pp"].ForeVisual.Key;

                    SpinelLabels[count].FogVisual = "WhiteSpace";
                }
                else
                {
                    currTile = YuushaMode.Tiles["  "];

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
                    //    SpinelLabels[count].VisualKey = YuushaMode.Tiles["pp"].ForeVisual.Key;
                }

                x++;
                columnsCount++;
                count++;
                if (columnsCount == Columns)
                {
                    y++;
                    x = Character.CurrentCharacter.X - 9;
                    columnsCount = 0;
                }
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
            base.OnZDelta(ms);
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
