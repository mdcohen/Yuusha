using System;
using System.Collections.Generic;
using System.Text;

namespace Yuusha.gui
{
    public class ListBox : Control
    {
        protected bool m_newData;

        public bool HasNewData
        {
            get { return m_newData; }
        }
    }
}
