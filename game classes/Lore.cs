using System.Xml;
using System.Collections.Generic;

namespace Yuusha
{
    public static class Lore
    {
        public static string LoreXMLFile = "";

        public static Dictionary<string, string> GetAllHomelandLore()
        {
            Dictionary<string, string> homelandDescriptions = new Dictionary<string, string>();

            try
            {
                XmlTextReader reader = new XmlTextReader(Utils.GetMediaFile(LoreXMLFile))
                {
                    WhitespaceHandling = WhitespaceHandling.None
                };

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Homeland")
                    {
                        string homeland = "";
                        for(int i = 0; i < reader.AttributeCount; i++)
                        {
                            reader.MoveToAttribute(i);
                            if(reader.Name == "Name")
                            {
                                homeland = reader.Value;
                            }
                            else if(reader.Name == "Description")
                            {
                                if (!homelandDescriptions.ContainsKey(homeland))
                                {
                                    homelandDescriptions.Add(homeland, reader.Value);
                                }
                                else Utils.Log("Lore: Attempted to add existing homeland value of " + homeland + ".");
                            }
                        }
                    }
                }
            }
            catch(System.Exception e)
            {
                Utils.LogException(e);
            }

            return homelandDescriptions;
        }

        public static Dictionary<string, string> GetAllProfessionsLore()
        {
            Dictionary<string, string> professionDescriptions = new Dictionary<string, string>();

            try
            {
                XmlTextReader reader = new XmlTextReader(Utils.GetMediaFile(LoreXMLFile))
                {
                    WhitespaceHandling = WhitespaceHandling.None
                };

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Profession")
                    {
                        string profession = "";
                        for (int i = 0; i < reader.AttributeCount; i++)
                        {
                            reader.MoveToAttribute(i);
                            if (reader.Name == "Name")
                            {
                                profession = reader.Value;
                            }
                            else if (reader.Name == "Description")
                            {
                                if (!professionDescriptions.ContainsKey(profession))
                                {
                                    professionDescriptions.Add(profession, reader.Value);
                                }
                                else Utils.Log("Lore: Attempted to add existing profession value of " + profession + ".");
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Utils.LogException(e);
            }

            return professionDescriptions;
        }

        public static string GetHomelandLore(string homeland)
        {
            XmlTextReader reader = new XmlTextReader(Utils.GetMediaFile(LoreXMLFile))
            {
                WhitespaceHandling = WhitespaceHandling.None
            };

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "Homeland")
                    {
                        for (int i = 0; i < reader.AttributeCount; i++)
                        {
                            reader.MoveToAttribute(i);
                            if (reader.Name == "Name" && reader.Value == homeland)
                            {
                                reader.MoveToAttribute("Description");
                                return reader.Value;
                            }
                        }
                    }
                }
            }

            return "";
        }
    }
}
