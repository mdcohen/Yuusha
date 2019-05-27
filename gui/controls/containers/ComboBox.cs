using System;
using System.Collections.Generic;
using System.Text;

namespace Yuusha.gui
{
    public class ComboBox : Control
    {
        bool m_isOpen = false;

        public bool IsOpen
        {
            get { return m_isOpen; }
            set { m_isOpen = value; }
        }
    }
}
