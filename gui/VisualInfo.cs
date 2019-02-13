using Microsoft.Xna.Framework;

namespace Yuusha.gui
{
    public class VisualInfo
    {
        private string m_parentTexture;
        private string m_name;
        private int m_x;
        private int m_y;
        private int m_width;
        private int m_height;
        private Color m_tintColor;

        public string ParentTexture
        {
            get { return m_parentTexture; }
            set { m_parentTexture = value; }
        }
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }
        public int X
        {
            get { return m_x; }
            set { m_x = value; }
        }
        public int Y
        {
            get { return m_y; }
            set { m_y = value; }
        }
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }
        public Rectangle Rectangle
        {
            get { return new Rectangle(m_x, m_y, m_width, m_height); }
        }

        public Color TintColor
        {
            get { return m_tintColor; }
        }

        public VisualInfo(System.Xml.XmlReader reader)
        {
            for (int i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);
                if (reader.Name == "ParentTexture")
                    m_parentTexture = reader.Value;
                else if (reader.Name == "Name")
                    m_name = reader.Value;
                else if (reader.Name == "X")
                    m_x = reader.ReadContentAsInt();
                else if (reader.Name == "Y")
                    m_y = reader.ReadContentAsInt();
                else if (reader.Name == "Width")
                    m_width = reader.ReadContentAsInt();
                else if (reader.Name == "Height")
                    m_height = reader.ReadContentAsInt();
                else if (reader.Name == "Tint")
                    m_tintColor = Utils.GetColor(reader.Value);
            }
        }

        public VisualInfo(string parentTexture, string name, int x, int y, int width, int height)
        {
            m_parentTexture = parentTexture;
            m_name = name;
            m_x = x;
            m_y = y;
            m_width = width;
            m_height = height;
            m_tintColor = Color.White;
        }
    }
}
