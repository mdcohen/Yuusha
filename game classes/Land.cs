using System;
using System.Collections.Generic;

namespace Yuusha
{
    public class Land
    {
        #region Private Data
        short id;
        string name;
        string shortDesc;
        string longDesc;
        List<Map> maps; 
        #endregion

        #region Constructor
        public Land(string info)
        {
            this.maps = new List<Map>();

            string[] landInfo = info.Split(Protocol.VSPLIT.ToCharArray());

            this.id = Convert.ToInt16(landInfo[0]);
            this.name = landInfo[1];
            this.shortDesc = landInfo[2];
            this.longDesc = landInfo[3];
        } 
        #endregion

        #region Public Properties
        public short ID
        {
            get
            {
                return this.id;
            }
        }

        public string Name
        {
            get { return this.name; }
        }

        public string ShortDesc
        {
            get { return this.shortDesc; }
        }

        public string LongDesc
        {
            get { return this.longDesc; }
        } 
        #endregion

        public void Add(Map map)
        {
            this.maps.Add(map);
        }

        public Map GetMapByID(int id)
        {
            foreach (Map map in this.maps)
            {
                if (map.MapID == id)
                {
                    return map;
                }
            }
            return null;
        }
    }
}
