using System;
using System.Collections.Generic;
using System.Text;

namespace Yuusha.gui
{
    public class LOKTileDefinition
    {
        private readonly string m_code;
        private readonly string m_visualKey;

        public string Code
        {
            get { return m_code; }
        }
        public string VisualKey
        {
            get { return m_visualKey; }
        }

        public LOKTileDefinition(System.Xml.XmlReader reader)
        {
            for (int i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);
                if (reader.Name == "Code")
                    m_code = reader.Value;
                else if (reader.Name == "VisualKey")
                    m_visualKey = reader.Value;
            }
        }
    }
}
