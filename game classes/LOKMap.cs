using System;
using System.Collections.Generic;
using System.Text;

namespace Yuusha
{
    public class LOKMap : Map
    {
        private Dictionary<int, List<Cell>> m_cellsDictionaryByZ;
        private Dictionary<string, Cell> m_cells;
        private string m_fileName;

        public Dictionary<string, Cell> Cells
        {
            get { return m_cells; }
        }
        public string FileName
        {
            get { return m_fileName; }
            set { m_fileName = value; }
        }
        public LOKMap()
            : base()
        {
            m_fileName = "";
            m_cellsDictionaryByZ = new Dictionary<int, List<Cell>>();
            m_cells = new Dictionary<string, Cell>();
        }

        public void Add(Cell cell)
        {
            string key = cell.xCord + "," + cell.yCord + "," + cell.zCord;
            m_cells.Add(key, cell);
        }
    }
}
