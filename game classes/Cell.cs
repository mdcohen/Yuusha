using System;
using System.Collections.Generic;

namespace Yuusha
{
    public class Cell
    {
        #region Cell graphic constants
        public const string GRAPHIC_WATER = "~~";
        public const string GRAPHIC_AIR = "%%";
        public const string GRAPHIC_WEB = "ww";
        public const string GRAPHIC_DARKNESS = "??";
        public const string GRAPHIC_CLOSED_DOOR_HORIZONTAL = "--";
        public const string GRAPHIC_OPEN_DOOR_HORIZONTAL = "\\ ";
        public const string GRAPHIC_CLOSED_DOOR_VERTICAL = "| ";
        public const string GRAPHIC_OPEN_DOOR_VERTICAL = "/ ";
        public const string GRAPHIC_ICE = "~.";
        public const string GRAPHIC_ICE_WALL = "~,";
        public const string GRAPHIC_FIRE = "**";
        public const string GRAPHIC_FOG = "FF";
        public const string GRAPHIC_WALL = "[]";
        public const string GRAPHIC_WALL_IMPENETRABLE = "DD";
        public const string GRAPHIC_MOUNTAIN = "/\\";
        public const string GRAPHIC_FOREST_IMPASSABLE = "TT";
        public const string GRAPHIC_SECRET_DOOR = "SD";
        public const string GRAPHIC_SECRET_MOUNTAIN = "SM";
        public const string GRAPHIC_LOCKED_DOOR_HORIZONTAL = "HD";
        public const string GRAPHIC_LOCKED_DOOR_VERTICAL = "VD";
        public const string GRAPHIC_COUNTER = "==";
        public const string GRAPHIC_COUNTER_PLACEABLE = "CC";
        public const string GRAPHIC_BOXING_RING = ")(";
        public const string GRAPHIC_ALTAR = "mm";
        public const string GRAPHIC_ALTAR_PLACEABLE = "MM";
        public const string GRAPHIC_REEF = "WW";
        public const string GRAPHIC_GRATE = "##";
        public const string GRAPHIC_EMPTY = ". ";
        public const string GRAPHIC_RUINS_LEFT = "_]";
        public const string GRAPHIC_RUINS_RIGHT = "[_";
        public const string GRAPHIC_SAND = ".\\";
        public const string GRAPHIC_FOREST_LEFT = "@ ";
        public const string GRAPHIC_FOREST_RIGHT = " @";
        public const string GRAPHIC_FOREST_FULL = "@@";
        public const string GRAPHIC_BRIDGE = "::";

        public const string GRAPHIC_FOREST_BURNT_LEFT = "t ";
        public const string GRAPHIC_FOREST_BURNT_RIGHT = " t";
        public const string GRAPHIC_FOREST_BURNT_FULL = "tt";

        public const string GRAPHIC_FOREST_FROSTY_LEFT = "f ";
        public const string GRAPHIC_FOREST_FROSTY_RIGHT = " f";
        public const string GRAPHIC_FOREST_FROSTY_FULL = "ff";

        public const string GRAPHIC_GRASS_THICK = "\"\"";
        public const string GRAPHIC_GRASS_LIGHT = "''";
        public const string GRAPHIC_GRASS_FROZEN = ", ";

        public const string GRAPHIC_UPSTAIRS = "up";
        public const string GRAPHIC_DOWNSTAIRS = "dn";
        public const string GRAPHIC_TRASHCAN = "o ";
        public const string GRAPHIC_UP_AND_DOWNSTAIRS = "ud";

        public const string GRAPHIC_LIGHTNING_STORM = "++";
        public const string GRAPHIC_POISON_CLOUD = ":%";
        public const string GRAPHIC_ICE_STORM = "`,";
        public const string GRAPHIC_ACID_STORM = "``";
        public const string GRAPHIC_WHIRLWIND = "%;";
        public const string GRAPHIC_LOCUST_SWARM = "-.";

        public const string GRAPHIC_BARREN_LEFT = ", ";
        public const string GRAPHIC_BARREN_RIGHT = " ,";
        public const string GRAPHIC_BARREN_FULL = ",,";

        public const string GRAPHIC_LOOT_SYMBOL = "$";
        #endregion

        public int xCord = 0;
        public int yCord = 0;
        public int zCord = 0;
        public int MapID = 0;
        public int LandID = 0;

        public string DisplayGraphic
        { get; set; } = "  ";
        public string CellGraphic
        { get; set; } = "  ";

        //public bool lair = false; // true if the janitor will ignore this cell
        
        //public bool secretDoor = false; // true if this cell is a secret door
        //public bool singleCustomer = false;	// true if only one player is allowed in this cell at a time
        //public bool teleport = false; // true if this is a teleport cell (uses this cell's CellLock for access)
        //public bool magicDead = false; // true if magic does not work
        //public bool townLimits = false;	// true if within town limits (terrain spells cast here by lawfuls make you neutral)
        //public bool noRecall = false; // true if cell does not allow setting recall or recall from
        //public bool underworldPortal = false; // true if cell allows access to the Underworld
        //public bool ancestorStart = false; // true if cell begins ancestoring process
        //public bool ancestorFinish = false;	// true if cell allows completion of ancestoring process
        //public bool oneHandClimbUp = false;
        //public bool oneHandClimbDown = false;
        //public bool twoHandClimbUp = false;
        //public bool twoHandClimbDown = false;
        //public bool pvpEnabled = false; // true if PvP enabled (no mark or karma penalties)

        private List<Effect> m_effects;
        private List<Character> m_characters; // collection of Character objects in this cell
        private List<Item> m_items; // collection of Item objects in this cell

        public List<Item> Items
        { get { return m_items; } }

        #region Public Properties
        public List<Character> Characters
        {
            get { return this.m_characters; }
        }

        public List<Effect> Effects
        {
            get { return this.m_effects; }
        }

        public bool HasItems
        { get; set; } = false;

        public bool IsLockers
        { get; set; } = false; // true if this is a lockers cell        

        public bool IsLootVisible
        {
            get
            {
                switch (DisplayGraphic)
                {
                    case GRAPHIC_WATER: // water
                        if (Character.CurrentCharacter != null && Character.CurrentCharacter.Cell != this)
                            return false;
                        break;
                    case GRAPHIC_FIRE: // fire
                    case GRAPHIC_ICE_STORM:
                    case GRAPHIC_WALL: // wall
                    case "  ": // empty
                    case GRAPHIC_DARKNESS: // darkness
                        return false;
                    default:
                        break;
                }

                return true;
            }
        }

        public bool IsLootPartiallyVisible
        {
            get
            {
                switch (DisplayGraphic)
                {
                    case GRAPHIC_FOREST_FULL:
                    case GRAPHIC_FOREST_LEFT:
                    case GRAPHIC_FOREST_RIGHT:
                    case GRAPHIC_FOREST_FROSTY_FULL:
                    case GRAPHIC_FOREST_FROSTY_LEFT:
                    case GRAPHIC_FOREST_FROSTY_RIGHT:
                    case GRAPHIC_FOREST_BURNT_FULL:
                    case GRAPHIC_FOREST_BURNT_LEFT:
                    case GRAPHIC_FOREST_BURNT_RIGHT:
                    case GRAPHIC_GRASS_THICK:
                    case GRAPHIC_UPSTAIRS:
                    case GRAPHIC_DOWNSTAIRS:
                    case GRAPHIC_UP_AND_DOWNSTAIRS:
                    case GRAPHIC_SAND:
                    case GRAPHIC_WEB:
                    case GRAPHIC_WATER: // water loot is only visible when it is the current cell
                        return true;
                    default:
                        return false;
                }
            }
        }

        public bool IsVisible
        { get; set; } = false; // is this cell visible from player's current position

        public bool IsPortal
        { get; set; } = false;	// true if this is a map portal cell
        #endregion

        #region Constructors (2)
        public Cell()
        {
            IsVisible = false;
            LandID = 0;
            MapID = -1;
            xCord = 0;
            yCord = 0;
            zCord = 0;
            CellGraphic = "  ";
            DisplayGraphic = "  ";
            IsLockers = false;
            IsPortal = false;
            m_effects = new List<Effect>();
            m_characters = new List<Character>();
            m_items = new List<Item>();
        }

        public Cell(string info)
        {
            string[] cellInfo = info.Split(Protocol.ISPLIT.ToCharArray());

            try
            {
                IsVisible = true;
                LandID = Convert.ToInt32(cellInfo[0]);
                if (Character.CurrentCharacter != null && Character.CurrentCharacter.m_landID != LandID)
                {
                    Utils.LogOnce("Map ID mismatch between cells being viewed and CurrentCharacter's mapID. Logging this until fixed.");
                    Character.CurrentCharacter.m_landID = LandID;
                }
                MapID = Convert.ToInt32(cellInfo[1]);
                xCord = Convert.ToInt32(cellInfo[2]);
                yCord = Convert.ToInt32(cellInfo[3]);
                zCord = Convert.ToInt32(cellInfo[4]);
                CellGraphic = cellInfo[5];
                DisplayGraphic = cellInfo[6];
                IsLockers = Convert.ToBoolean(cellInfo[7]);
                IsPortal = Convert.ToBoolean(cellInfo[8]);
                HasItems = Convert.ToBoolean(cellInfo[9]);
                m_effects = new List<Effect>();
                m_characters = new List<Character>();
                m_items = new List<Item>();

                // Fog of War
                gui.MapWindow.FogOfWarDetail fogDetail = new gui.MapWindow.FogOfWarDetail(MapID, xCord, yCord, zCord, DisplayGraphic);

                if (DisplayGraphic != "  ")
                {
                    if (!Character.FogOfWarSettings.FogOfWar.Contains(fogDetail))
                    {
                        Character.FogOfWarSettings.FogOfWar.Add(fogDetail);
                        Events.RegisterEvent(Events.EventName.Fog_of_War_Updated);
                    }
                    else if (fogDetail.DisplayGraphic != Character.FogOfWarSettings.GetFogOfWarDetail(MapID, xCord, yCord, zCord).DisplayGraphic)
                    {
                        Character.FogOfWarSettings.UpdateFogOfWarDetail(MapID, xCord, yCord, zCord, DisplayGraphic);
                        Events.RegisterEvent(Events.EventName.Fog_of_War_Updated);
                    }
                }
            }
            catch (Exception e)
            {
                Utils.LogException(e);
                Utils.Log("Invalid Cell info format: " + info);
            }
        } 
        #endregion

        #region Add Methods
        public void Add(Character ch)
        {
            this.m_characters.Add(ch);
        }

        public void Add(Item item)
        {
            this.m_items.Add(item);
        }

        public void Add(Effect effect)
        {
            this.m_effects.Add(effect);
        } 
        #endregion

        public static void SendCellItemsRequest(Cell cell)
        {
            if (cell != null)
            {
                gui.GameHUD.ExaminedCell = cell;
                string cellCoords = cell.xCord + Protocol.VSPLIT + cell.yCord + Protocol.VSPLIT + cell.zCord;
                IO.Send(Protocol.REQUEST_CELLITEMS + " " + cellCoords);
            }
        }

        public static Cell GetCell(int x, int y, int z)
        {
            try
            {
                return gui.GameHUD.Cells.Find(cell => cell.xCord == x && cell.yCord == y && cell.zCord == z);
                //if (Client.GameState == Enums.EGameState.SpinelGame)
                //    return gui.SpinelMode.Cells.Find(cell => cell.xCord == x && cell.yCord == y && cell.zCord == z);
                //else if (Client.GameState == Enums.EGameState.YuushaGame)
                //    return gui.GameHUD.Cells.Find(cell => cell.xCord == x && cell.yCord == y && cell.zCord == z);
                //else return gui.IOKMode.Cells.Find(cell => cell.xCord == x && cell.yCord == y && cell.zCord == z);
            }
            catch(Exception e)
            {
                Utils.LogException(e);
            }
            return null;
        }

        public static int GetCellDistance(int startXCord, int startYCord, int goalXCord, int goalYCord) // determine distance between two cells
        {
            try
            {
                if (startXCord == goalXCord && startYCord == goalYCord)
                {
                    return 0;
                }

                if (goalXCord <= Int32.MinValue || goalYCord <= Int32.MinValue)
                {
                    return 0;
                }

                int xAbsolute = Math.Abs(startXCord - goalXCord); // get absolute value of StartX - GoalX
                int yAbsolute = Math.Abs(startYCord - goalYCord); // get absolute value of StartY - GoalY

                if (xAbsolute > yAbsolute)
                {
                    return xAbsolute;
                }
                else
                {
                    return yAbsolute;
                }
            }
            catch (Exception e)
            {
                Utils.Log("Failure at Cell.GetCellDistance(" + startXCord + ", " + startYCord + ", " + goalXCord + ", " + goalYCord + ")");
                Utils.LogException(e);
                return 0;
            }
        }

        public int MovementWeight()
        {
            if (IsImpassable())
                return 10000;

            switch(DisplayGraphic)
            {
                case GRAPHIC_RUINS_LEFT:
                case GRAPHIC_RUINS_RIGHT:
                case GRAPHIC_SAND:
                    return 1;
                case GRAPHIC_WATER:
                    if (Character.CurrentCharacter.breatheWater) return 0;
                    else return 1;
            }

            return 0;
        }

        public bool IsExaminable()
        {
            switch(DisplayGraphic)
            {
                case GRAPHIC_WALL:
                case GRAPHIC_FOREST_IMPASSABLE:
                case GRAPHIC_REEF:
                case GRAPHIC_MOUNTAIN:
                case GRAPHIC_TRASHCAN:
                    return false;
            }

            return true;
        }

        public bool IsImpassable()
        {
            switch(DisplayGraphic)
            {
                case GRAPHIC_WALL:
                case GRAPHIC_FOREST_IMPASSABLE:
                case GRAPHIC_REEF:
                case GRAPHIC_MOUNTAIN:
                case GRAPHIC_COUNTER:
                case GRAPHIC_ALTAR:
                    return true;
            }

            return false;
        }

        public static bool operator ==(Cell c1, Cell c2)
        {
            try
            {
                if (c1 is null && !(c2 is null))
                    return false;

                if (!(c1 is null) && c2 is null)
                    return false;

                if (c1 is null && c2 is null)
                    return false;

                if (c1.MapID == c2.MapID && c1.xCord == c2.xCord && c1.yCord == c2.yCord && c1.zCord == c2.zCord)
                    return true;
            }
            catch(Exception e)
            {
                Utils.LogException(e);
            }

            return false;                   
        }

        public static bool operator !=(Cell c1, Cell c2)
        {
            try
            {
                if (c1 is null && !(c2 is null))
                    return true;

                if (!(c1 is null) && c2 is null)
                    return true;

                if (c1 is null && c2 is null)
                    return false;

                if (c1.xCord != c2.xCord || c1.yCord != c2.yCord || c1.zCord != c2.zCord)
                    return true;
            }
            catch(Exception e)
            {
                Utils.LogException(e);
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Cell)) return false;

            if(obj is Cell cell)
            {
                if (cell.MapID == MapID && cell.xCord == xCord && cell.yCord == yCord && cell.zCord == zCord)
                    return true;
                else return false;
            }

            return this == (Cell)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
