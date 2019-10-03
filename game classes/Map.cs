using System;
using Microsoft.Xna.Framework;

namespace Yuusha
{
    public class Map
    {
        public enum ClimateType { None, Temperate, Subtropical, Tropical, Desert, Tundra, Frozen, Subterranean }
        public enum Direction { None, North, South, East, West, Northeast, Southeast, Northwest, Southwest }

        public static System.Collections.Generic.Dictionary<Direction, Point> DirectionCoordinates = new System.Collections.Generic.Dictionary<Direction, Point>()
        {
            {Direction.None, new Point(0, 0) },
            {Direction.North, new Point(0, -1) },
            {Direction.South, new Point(0, 1) },
            {Direction.East, new Point(1, 0) },
            {Direction.West, new Point(-1, 0) },
            {Direction.Northeast, new Point(1, -1) },
            {Direction.Southeast, new Point(1, 1) },
            {Direction.Northwest, new Point(-1, -1) },
            {Direction.Southwest, new Point(-1, 1) },
        };

        #region Private Data
        private short m_landID; // the land ID of the land object this map belongs to
        private short m_mapID; // the map id
        private string m_name; // the name of this map
        private string m_shortDesc; // short description
        private string m_longDesc; // the full description
        private int m_suggestedMaximumLevel; // suggested maximum level
        private int m_suggestedMinimumLevel; // suggested minimum level
        private bool m_pvpEnabled; // pvp enabled
        private double m_experienceModifier; // experience modifier
        private int m_difficulty;
        private ClimateType m_climateType; // climate type
        #endregion

        #region Constructors (2)
        public Map()
        {
            this.m_landID = 0;
            this.m_mapID = 0;
            this.m_name = "";
            this.m_shortDesc = "";
            this.m_longDesc = "";
            this.m_suggestedMaximumLevel = 0;
            this.m_suggestedMinimumLevel = 0;
            this.m_pvpEnabled = false;
            this.m_experienceModifier = 0;
            this.m_difficulty = 0;
            this.m_climateType = ClimateType.None;
        }

        public Map(string info)
        {
            string[] mapInfo = info.Split(Protocol.VSPLIT.ToCharArray());

            this.m_landID = Convert.ToInt16(mapInfo[0]);
            this.m_mapID = Convert.ToInt16(mapInfo[1]);
            this.m_name = mapInfo[2];
            this.m_shortDesc = mapInfo[3];
            this.m_longDesc = mapInfo[4];
            this.m_suggestedMaximumLevel = Convert.ToInt32(mapInfo[5]);
            this.m_suggestedMinimumLevel = Convert.ToInt32(mapInfo[6]);
            this.m_pvpEnabled = Convert.ToBoolean(mapInfo[7]);
            this.m_experienceModifier = Convert.ToDouble(mapInfo[8]);
            this.m_difficulty = Convert.ToInt32(mapInfo[9]);
            this.m_climateType = (ClimateType)Convert.ToInt32(mapInfo[10]);
        } 
        #endregion

        #region Public Properties
        public short LandID
        {
            get { return this.m_landID; }
        }
        public short MapID
        {
            get { return this.m_mapID; }
        }
        public string Name
        {
            get { return this.m_name; }
        }
        #endregion

        public static Direction GetDirection(XYCoordinate xy1, XYCoordinate xy2)
        {
            if (xy1 == null || xy2 == null) return Direction.None;

            try
            {
                if (xy1.X < xy2.X && xy1.Y < xy2.Y)
                {
                    return Direction.Southeast;
                }
                else if (xy1.X < xy2.X && xy1.Y > xy2.Y)
                {
                    return Direction.Northeast;
                }
                else if (xy1.X > xy2.X && xy1.Y < xy2.Y)
                {
                    return Direction.Southwest;
                }
                else if (xy1.X > xy2.X && xy1.Y > xy2.Y)
                {
                    return Direction.Northwest;
                }
                else if (xy1.X == xy2.X && xy1.Y > xy2.Y)
                {
                    return Direction.North;
                }
                else if (xy1.X == xy2.X && xy1.Y < xy2.Y)
                {
                    return Direction.South;
                }
                else if (xy1.X < xy2.X && xy1.Y == xy2.Y)
                {
                    return Direction.East;
                }
                else if (xy1.X > xy2.X && xy1.Y == xy2.Y)
                {
                    return Direction.West;
                }
                else
                {
                    return Direction.None; // same cell
                }
            }
            catch (Exception e)
            {
                Utils.LogException(e);
                return Direction.None;
            }
        }

        public static Direction GetDirection(Cell from, Cell to)
        {
            if (from == null || to == null) return Direction.None;

            try
            {
                if (from.xCord < to.xCord && from.yCord < to.yCord)
                {
                    return Direction.Southeast;
                }
                else if (from.xCord < to.xCord && from.yCord > to.yCord)
                {
                    return Direction.Northeast;
                }
                else if (from.xCord > to.xCord && from.yCord < to.yCord)
                {
                    return Direction.Southwest;
                }
                else if (from.xCord > to.xCord && from.yCord > to.yCord)
                {
                    return Direction.Northwest;
                }
                else if (from.xCord == to.xCord && from.yCord > to.yCord)
                {
                    return Direction.North;
                }
                else if (from.xCord == to.xCord && from.yCord < to.yCord)
                {
                    return Direction.South;
                }
                else if (from.xCord < to.xCord && from.yCord == to.yCord)
                {
                    return Direction.East;
                }
                else if (from.xCord > to.xCord && from.yCord == to.yCord)
                {
                    return Direction.West;
                }
                else
                {
                    return Direction.None; // same cell
                }
            }
            catch (Exception)
            {
                //Utils.LogException(e);
                return Direction.None;
            }
        }

        public static Direction GetDirection(gui.Control c1, gui.Control c2)
        {
            if (c1 == null || c2 == null) return Direction.None;

            try
            {
                if (c1.Position.X < c2.Position.X && c1.Position.Y < c2.Position.Y)
                {
                    return Direction.Southeast;
                }
                else if (c1.Position.X < c2.Position.X && c1.Position.Y > c2.Position.Y)
                {
                    return Direction.Northeast;
                }
                else if (c1.Position.X > c2.Position.X && c1.Position.Y < c2.Position.Y)
                {
                    return Direction.Southwest;
                }
                else if (c1.Position.X > c2.Position.X && c1.Position.Y > c2.Position.Y)
                {
                    return Direction.Northwest;
                }
                else if (c1.Position.X == c2.Position.X && c1.Position.Y > c2.Position.Y)
                {
                    return Direction.North;
                }
                else if (c1.Position.X == c2.Position.X && c1.Position.Y < c2.Position.Y)
                {
                    return Direction.South;
                }
                else if (c1.Position.X < c2.Position.X && c1.Position.Y == c2.Position.Y)
                {
                    return Direction.East;
                }
                else if (c1.Position.X > c2.Position.X && c1.Position.Y == c2.Position.Y)
                {
                    return Direction.West;
                }
                else
                {
                    return Direction.None; // same cell
                }
            }
            catch (Exception e)
            {
                Utils.LogException(e);
                return Direction.None;
            }
        }

        public static Direction GetOppositeDirection(Direction direction)
        {
            switch(direction)
            {
                case Direction.North:
                    return Direction.South;
                case Direction.South:
                    return Direction.North;
                case Direction.East:
                    return Direction.West;
                case Direction.West:
                    return Direction.East;
                case Direction.Northeast:
                    return Direction.Southwest;
                case Direction.Southeast:
                    return Direction.Northwest;
                case Direction.Northwest:
                    return Direction.Southeast;
                case Direction.Southwest:
                    return Direction.Northeast;
                default:
                    return Direction.None;
            }
        }
    }
}
