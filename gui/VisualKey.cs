namespace Yuusha.gui
{
    public class VisualKey
    {
        private string m_key;

        public string Key
        {
            get { return m_key; }
            set { m_key = value; }
        }

        public VisualKey(string key)
        {
            m_key = key;
        }

        public override string ToString()
        {
            return m_key;
        }
    }
}
