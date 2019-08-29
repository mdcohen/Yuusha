using Microsoft.Xna.Framework;

namespace Yuusha.gui
{
    public class VisualKey
    {
        private string m_key;
        private Color m_overrideTintColor;

        public string Key
        {
            get { return m_key; }
            set { m_key = value; }
        }

        public Color OverrideTintColor
        {
            get { return m_overrideTintColor; }
            set { m_overrideTintColor = value; }
        }

        public VisualKey(string key)
        {
            m_key = key;
            m_overrideTintColor = Color.White;
        }

        public override string ToString()
        {
            return m_key;
        }
    }
}
