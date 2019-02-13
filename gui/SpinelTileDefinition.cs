using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Yuusha.gui
{
    public class SpinelTileDefinition
    {
        private readonly string m_graphic;
        private readonly string m_name;
        private readonly gui.VisualKey m_foreVisual;
        private readonly gui.VisualKey m_backVisual;
        private readonly byte m_foreAlpha;
        private readonly byte m_backAlpha;
        private readonly Color m_foreTintColor;
        private readonly Color m_backTintColor;

        public string Graphic
        {
            get { return m_graphic; }
        }
        public string Name
        {
            get { return m_name; }
        }
        public gui.VisualKey ForeVisual
        {
            get { return m_foreVisual; }
        }
        public gui.VisualKey BackVisual
        {
            get { return m_backVisual; }
        }
        public byte ForeAlpha
        {
            get { return m_foreAlpha; }
        }
        public byte BackAlpha
        {
            get { return m_backAlpha; }
        }
        public Color ForeTint
        {
            get { return m_foreTintColor; }
        }
        public Color BackTint
        {
            get { return m_backTintColor; }
        }
        public SpinelTileDefinition(System.Xml.XmlTextReader reader)
        {
            for (int i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);
                if (reader.Name == "Graphic")
                    m_graphic = reader.Value;
                else if (reader.Name == "Name")
                    m_name = reader.Value;
                else if (reader.Name == "ForeVisual")
                    m_foreVisual = new VisualKey(reader.Value);
                else if (reader.Name == "BackVisual")
                    m_backVisual = new VisualKey(reader.Value);
                else if (reader.Name == "ForeAlpha")
                    m_foreAlpha = Convert.ToByte(reader.ReadContentAsInt());
                else if (reader.Name == "BackAlpha")
                    m_backAlpha = Convert.ToByte(reader.ReadContentAsInt());
                else if (reader.Name == "ForeTint")
                    m_foreTintColor = Utils.GetColor(reader.Value);
                else if (reader.Name == "BackTint")
                    m_backTintColor = Utils.GetColor(reader.Value);
            }
        }
    }
}
