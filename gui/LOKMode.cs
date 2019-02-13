using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Yuusha.gui
{
    static public class LOKMode
    {
        static string m_tileXMLFile = "";
        static Dictionary<string, LOKTileDefinition> m_tilesDict = new Dictionary<string, LOKTileDefinition>();
        static LOKMap m_currentMapEdit = new LOKMap();
        static List<string> m_codesList = new List<string>();

        public static string TileXMLFile
        {
            get { return m_tileXMLFile; }
            set { m_tileXMLFile = value; }
        }
        public static Dictionary<string, LOKTileDefinition> Tiles
        {
            get { return m_tilesDict; }
        }
        public static List<string> Codes
        {
            get { return m_codesList; }
        }

        public static string GetCodeByVisualKey(string visualKey)
        {
            foreach (LOKTileDefinition ltd in m_tilesDict.Values)
            {
                if (ltd.VisualKey == visualKey)
                    return ltd.Code;
            }
            return "";
        }

        public static bool LoadMapIntoEditor(string fileName)
        {
            m_currentMapEdit = new LOKMap();
            m_currentMapEdit.FileName = fileName;

            short xOffset = 0;
            short x = 0;
            short y = 0;
            int z = 0;
            int a = 0;
            Cell cell = null; // the cell that will be added to this map's dictionary

            try
            {
                if (!File.Exists(Utils.GetMediaFile(fileName)))
                    return false;
                
                StreamReader sr = File.OpenText(Utils.GetMediaFile("\\maps\\ " + fileName));

                string[] mapLines = sr.ReadToEnd().Split("\n".ToCharArray());

                #region Check lines to ensure even numbers
                for (a = 0; a < mapLines.Length; a++)
                {
                    mapLines[a] = mapLines[a].Replace("\r", "");
                    // does not start with a comment, is not a header line, and is odd in length
                    if (!mapLines[a].StartsWith("//") && !mapLines[a].StartsWith("<") && mapLines[a].Length % 2 == 1)
                    {
                        Utils.Log("Map line " + a + " was odd in length for " + fileName);
                        return false;
                    }
                }
                #endregion

                foreach (string s in mapLines)
                {
                    if (s.Length > 0 && !s.StartsWith("//")) // new
                    {
                        a = 0;
                        x = xOffset;
                        if (s.StartsWith("<"))
                        {
                            // x offset
                            if (s.IndexOf("<x>") != -1 && s.IndexOf("</x>") != -1)
                                xOffset = Convert.ToInt16(s.Substring(s.IndexOf("<x>") + 3, s.IndexOf("</x>") - (s.IndexOf("<x>") + 3)));
                            else xOffset = 0;

                            // y offset
                            if (s.IndexOf("<y>") != -1 && s.IndexOf("</y>") != -1)
                                y = Convert.ToInt16(s.Substring(s.IndexOf("<y>") + 3, s.IndexOf("</y>") - (s.IndexOf("<y>") + 3)));
                            else y = 0;

                            // z coord (height)
                            if (s.IndexOf("<z>") != -1 && s.IndexOf("</z>") != -1)
                                z = Convert.ToInt32(s.Substring(s.IndexOf("<z>") + 3, s.IndexOf("</z>") - (s.IndexOf("<z>") + 3)));
                            else z = 0;
                        }
                        else
                        {
                            while (a < s.Length)
                            {
                                if (a < s.Length)
                                {
                                    if (s.Substring(a, 2) != "  ")
                                    {
                                        cell = new Cell();
                                        cell.xCord = x;
                                        cell.yCord = y;
                                        cell.zCord = z;
                                        cell.cellGraphic = s.Substring(a, 2);

                                        if (!m_currentMapEdit.Cells.ContainsKey("" + x + "," + y + "," + z))
                                            m_currentMapEdit.Add(cell);
                                        else
                                        {

                                        }

                                    }
                                }
                                a += 2;
                            }
                        }
                    }
                }

                List<string> visualFilesFound = new List<string>();

                foreach (string fileFound in Directory.GetFiles(Directory.GetCurrentDirectory() + "\\media\\maps"))
                {
                    if (fileFound.Contains(".vk")) // extension for visual key information
                        visualFilesFound.Add(fileFound);
                }

                foreach (string vkFile in visualFilesFound)
                {
                    if (!File.Exists(Utils.GetMediaFile("maps\\" + vkFile)))
                        continue;

                    sr = File.OpenText(Utils.GetMediaFile("maps\\" + vkFile));

                    mapLines = sr.ReadToEnd().Split("\n".ToCharArray());

                    #region Check lines to ensure even numbers
                    for (a = 0; a < mapLines.Length; a++)
                    {
                        mapLines[a] = mapLines[a].Replace("\r", "");
                        // does not start with a comment, is not a header line, and is odd in length
                        if (!mapLines[a].StartsWith("//") && !mapLines[a].StartsWith("<") && mapLines[a].Length % 2 == 1)
                        {
                            Utils.Log("Map line " + a + " was odd in length for " + vkFile);
                            return false;
                        }
                    }
                    #endregion

                    foreach (string s in mapLines)
                    {
                        if (s.Length > 0 && !s.StartsWith("//")) // new
                        {
                            a = 0;
                            x = xOffset;
                            if (s.StartsWith("<"))
                            {
                                // x offset
                                if (s.IndexOf("<x>") != -1 && s.IndexOf("</x>") != -1)
                                    xOffset = Convert.ToInt16(s.Substring(s.IndexOf("<x>") + 3, s.IndexOf("</x>") - (s.IndexOf("<x>") + 3)));
                                else xOffset = 0;

                                // y offset
                                if (s.IndexOf("<y>") != -1 && s.IndexOf("</y>") != -1)
                                    y = Convert.ToInt16(s.Substring(s.IndexOf("<y>") + 3, s.IndexOf("</y>") - (s.IndexOf("<y>") + 3)));
                                else y = 0;

                                // z coord (height)
                                if (s.IndexOf("<z>") != -1 && s.IndexOf("</z>") != -1)
                                    z = Convert.ToInt32(s.Substring(s.IndexOf("<z>") + 3, s.IndexOf("</z>") - (s.IndexOf("<z>") + 3)));
                                else z = 0;
                            }
                            else
                            {
                                while (a < s.Length)
                                {
                                    if (a < s.Length)
                                    {
                                        if (s.Substring(a, 2) != "  ")
                                        {
                                            string key = x + "," + y + "," + z;
                                            cell = m_currentMapEdit.Cells[key];
                                            if (vkFile.EndsWith("vk0"))
                                                cell.visual0 = s.Substring(a, 2);
                                            else if(vkFile.EndsWith("vk1"))
                                                cell.visual1 = s.Substring(a, 2);
                                            else if (vkFile.EndsWith("vk2"))
                                                cell.visual2 = s.Substring(a, 2);
                                        }
                                    }
                                    a += 2;
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Utils.LogException(e);
                return false;
            }
        }

        public static bool DisplayMapInEditor()
        {
            // reload map editor here
            return true;
        }
    }
}
