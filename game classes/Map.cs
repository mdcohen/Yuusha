using System;
using System.Collections.Generic;
using System.Text;

namespace Yuusha
{
    public class Map
    {
        public enum ClimateType { None, Temperate, Subtropical, Tropical, Desert, Tundra, Frozen, Subterranean }
        public enum Direction { None, North, South, East, West, Northeast, Southeast, Northwest, Southwest }

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
    }
}
