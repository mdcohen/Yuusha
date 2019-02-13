using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Yuusha.gui
{
    public class IOKTileDefinition
    {
        #region Private Data
        private readonly string m_graphic, m_name, m_displayGraphic;
        private readonly Color m_foreColor, m_backColor;
        private readonly byte m_backAlpha, m_foreAlpha; 
        #endregion

        #region Public Properties
        public string Graphic
        {
            get { return m_graphic; }
        }
        public string DisplayGraphic
        {
            get { return m_displayGraphic; }
        }
        public Color ForeColor
        {
            get { return m_foreColor; }
        }
        public Color BackColor
        {
            get { return m_backColor; }
        }
        public byte BackAlpha
        {
            get { return m_backAlpha; }
        }
        public byte ForeAlpha
        {
            get { return m_foreAlpha; }
        } 
        #endregion

        #region Constructor
        public IOKTileDefinition(System.Xml.XmlTextReader reader)
        {
            for (int i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);
                if (reader.Name == "Graphic")
                    m_graphic = reader.Value;
                else if (reader.Name == "Name")
                    m_name = reader.Value;
                else if (reader.Name == "DisplayGraphic")
                    m_displayGraphic = reader.Value;
                else if (reader.Name == "ForeColor")
                    m_foreColor = Utils.GetColor(reader.Value);
                else if (reader.Name == "BackColor")
                    m_backColor = Utils.GetColor(reader.Value);
                else if (reader.Name == "ForeAlpha")
                    m_foreAlpha = Convert.ToByte(reader.ReadContentAsInt());
                else if (reader.Name == "BackAlpha")
                    m_backAlpha = Convert.ToByte(reader.ReadContentAsInt());
            }

            if (m_displayGraphic == "")
                m_displayGraphic = m_graphic;
        } 
        #endregion
    }
}
