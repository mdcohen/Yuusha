using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Xna.Framework;

namespace Yuusha.Utility.Settings
{
    public class GUIPositionSettings
    {
        [Serializable]
        public class GUIPositionDetail
        {
            public string ControlName
            { get; set; }

            public string Sheet
            { get; set; }

            public bool FullScreen // Position is for full screen or windowed mode
            { get; set; }

            public Point Coordinates
            {get; set;}

            public int Height
            { get; set; }

            public int Width
            { get; set; }
        }

        public List<GUIPositionDetail> GUIPositions = new List<GUIPositionDetail>();

        public void UpdateGUIPosition(gui.Control c)
        {
            if (GUIPositionsContains(c, out int index))
            {
                GUIPositions[index].Coordinates = new Point(c.Position.X, c.Position.Y);
                GUIPositions[index].FullScreen = Client.IsFullScreen;
                GUIPositions[index].Width = c.Width;
                GUIPositions[index].Height = c.Height;
            }
            else
            {
                GUIPositions.Add(
                    new GUIPositionDetail()
                    {
                        ControlName = c.Name,
                        Sheet = c.Sheet,
                        FullScreen = Client.IsFullScreen,
                        Coordinates = new Point(c.Position.X, c.Position.Y),
                        Width = c.Width,
                        Height = c.Height
                    });
            }

            Save();
        }

        public void OnLoad(gui.Window w)
        {
            if(GUIPositionsContains(w, out int index) && !w.IsLocked)
            {
                w.ForcePosition(GUIPositions[index].Coordinates);
            }

            #region Currently the only resized windows (besides logcally resized GridBoxWindows, etc..)
            // Change map display size
            if (Client.GameState == Enums.EGameState.YuushaGame)
            {
                if (w.Name == "MapDisplayWindow" && index > -1)
                {
                    int width = Character.GUIPositionSettings.GUIPositions[index].Width;
                    if (width > 0 && width != w.Width)
                    {
                        int loopCount = 0;
                        if (w.Width < width)
                        {
                            while (w.Width < width && loopCount < 50)
                            {
                                gui.GameHUD.ChangeMapDisplayWindowSize(5);
                                loopCount++;
                            }

                        }
                        else
                        {
                            while (w.Width > width && loopCount < 50)
                            {
                                gui.GameHUD.ChangeMapDisplayWindowSize(-5);
                                loopCount++;
                            }
                        }
                    }
                }
            }

            // Change horizontal hot button window size
            if (w.Name == "HorizontalHotButtonWindow" && index > -1)
            {
                int width = Character.GUIPositionSettings.GUIPositions[index].Width;
                if (width > 0 && width != w.Width)
                {
                    int loopCount = 0;
                    if (w.Width < width)
                    {
                        while (w.Width < width && loopCount < 10)
                        {
                            Events.RegisterEvent(Events.EventName.HotButtonWindow_Increase_Size, w["HorizontalHotButtonWindowIncreaseSizeButton"]);
                            loopCount++;
                        }
                    }
                    else
                    {
                        while (w.Width > width && loopCount < 10)
                        {
                            Events.RegisterEvent(Events.EventName.HotButtonWindow_Decrease_Size, w["HorizontalHotButtonWindowDecreaseSizeButton"]);
                            loopCount++;
                        }
                    }
                }
            }
            #endregion
        }

        public void OnLoad()
        {
            foreach(GUIPositionDetail detail in GUIPositions)
            {
                if(Client.IsFullScreen == detail.FullScreen && gui.GuiManager.GetControl(detail.ControlName) is gui.Window w && (detail.Sheet == "Generic" || detail.Sheet == Client.GameState.ToString()))
                {
                    if (!w.IsLocked)
                        w.ForcePosition(detail.Coordinates);
                }
            }

            #region Currently the only resized windows (besides logcally resized GridBoxWindows, etc..)
            // Change map display size
            if (Client.GameState == Enums.EGameState.YuushaGame)
            {
                if (gui.GuiManager.GetControl("MapDisplayWindow") is gui.Window mapDispWindow && Character.GUIPositionSettings.GUIPositionsContains(mapDispWindow, out int indexOfMapDisplayWindow))
                {
                    int width = Character.GUIPositionSettings.GUIPositions[indexOfMapDisplayWindow].Width;
                    if (width > 0 && width != mapDispWindow.Width)
                    {
                        int loopCount = 0;
                        if (mapDispWindow.Width < width)
                        {
                            while (mapDispWindow.Width < width && loopCount < 50)
                            {
                                gui.GameHUD.ChangeMapDisplayWindowSize(5);
                                loopCount++;
                            }

                        }
                        else
                        {
                            while (mapDispWindow.Width > width && loopCount < 50)
                            {
                                gui.GameHUD.ChangeMapDisplayWindowSize(-5);
                                loopCount++;
                            }
                        }
                    }
                }
            }

            // Change horizontal hot button window size
            if(gui.GuiManager.GetControl("HorizontalHotButtonWindow") is gui.Window horizHBW && Character.GUIPositionSettings.GUIPositionsContains(horizHBW, out int indexOfHHBW))
            {
                int width = Character.GUIPositionSettings.GUIPositions[indexOfHHBW].Width;
                if (width > 0 && width != horizHBW.Width)
                {
                    int loopCount = 0;
                    if (horizHBW.Width < width)
                    {
                        while (horizHBW.Width < width && loopCount < 10)
                        {
                            Events.RegisterEvent(Events.EventName.HotButtonWindow_Increase_Size, horizHBW["HorizontalHotButtonWindowIncreaseSizeButton"]);
                            loopCount++;
                        }
                    }
                    else
                    {
                        while (horizHBW.Width > width && loopCount < 10)
                        {
                            Events.RegisterEvent(Events.EventName.HotButtonWindow_Decrease_Size, horizHBW["HorizontalHotButtonWindowDecreaseSizeButton"]);
                            loopCount++;
                        }
                    }
                }
            }
            #endregion

        }

        public bool GUIPositionsContains(gui.Control c, out int index)
        {
            index = 0;
            foreach(GUIPositionDetail detail in GUIPositions)
            {
                if (detail.ControlName == c.Name && (detail.Sheet == "Generic" || detail.Sheet == c.Sheet))
                {
                    return true;
                }
                index++;
            }

            index = -1;
            return false;
        }

        #region Load/Save code
        /// <summary>
        /// Saves the current character's GUI position settings.
        /// </summary>
        /// <param name="filename">The filename to save to.</param>
        public void Save()
        {
            if (Character.CurrentCharacter == null)
                return;

            try
            {
                string fileName = Utils.GetCharacterGUIFileName(Character.CurrentCharacter.Name);
                string dirName = Utils.StartupPath + Utils.AccountsFolder + Account.Name + "\\";

                Stream stream = File.Create(dirName + fileName);

                XmlSerializer serializer = new XmlSerializer(typeof(GUIPositionSettings));
                serializer.Serialize(stream, this);
                stream.Close();
            }
            catch (Exception e)
            {
                Utils.LogException(e);
            }
        }

        /// <summary>
        /// Loads character GUI position settings from a file.
        /// </summary>
        /// <param name="filename">The filename to load.</param>
        public static GUIPositionSettings Load()
        {
            if (Character.CurrentCharacter == null)
                return new GUIPositionSettings();

            try
            {
                string fileName = Utils.GetCharacterGUIFileName(Character.CurrentCharacter.Name);
                string dirName = Utils.StartupPath + Utils.AccountsFolder + Account.Name + "\\";

                if (!File.Exists(dirName + fileName))
                {
                    return new GUIPositionSettings();
                }

                Stream stream = File.OpenRead(dirName + fileName);
                XmlSerializer serializer = new XmlSerializer(typeof(GUIPositionSettings));
                GUIPositionSettings settings = (GUIPositionSettings)serializer.Deserialize(stream);
                stream.Close();
                return settings;
            }
            catch (Exception e)
            {
                Utils.LogException(e);
                return new GUIPositionSettings();
            }
        }
        #endregion

    }
}
