using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using FogOfWarDetail = Yuusha.gui.MapWindow.FogOfWarDetail;

namespace Yuusha.Utility.Settings
{
    public class FogOfWarSettings
    {
        public List<FogOfWarDetail> FogOfWar = new List<FogOfWarDetail>();

        //public bool ContainsFogOfWarDetail(FogOfWarDetail fog)
        //{
        //    foreach (FogOfWarDetail foo in new List<FogOfWarDetail>())
        //        if (fog.Map == foo.Map && fog.XCord == foo.XCord && fog.YCord == foo.YCord && fog.ZCord == foo.ZCord)
        //            return true;

        //    return false;
        //}

        public FogOfWarDetail GetFogOfWarDetail(FogOfWarDetail existingFog)
        {
            return GetFogOfWarDetail(existingFog.Map, existingFog.XCord, existingFog.YCord, existingFog.ZCord);
        }

        public FogOfWarDetail GetFogOfWarDetail(int map, int x, int y, int z)
        {
            foreach (FogOfWarDetail fog in new List<FogOfWarDetail>(FogOfWar))
                if (fog.Map == map && fog.XCord == x && fog.YCord == y && fog.ZCord == z)
                    return fog;

            return new FogOfWarDetail();
        }

        public void UpdateFogOfWarDetail(int map, int xCord, int yCord, int zCord, string DisplayGraphic)
        {
            int index = 0;
            FogOfWarDetail newFog = new FogOfWarDetail(map, xCord, yCord, zCord, DisplayGraphic);
            foreach (FogOfWarDetail fog in new List<FogOfWarDetail>(FogOfWar))
            {
                if (fog.Map == newFog.Map && fog.XCord == newFog.XCord && fog.YCord == newFog.YCord && fog.ZCord == newFog.ZCord)
                {
                    FogOfWar.Remove(fog);
                    FogOfWar.Insert(index, newFog);
                    return;
                }
                index++;
            }
        }

        #region Load/Save code
        /// <summary>
        /// Saves the current character settings.
        /// </summary>
        /// <param name="filename">The filename to save to</param>
        public void Save()
        {
            if (Character.CurrentCharacter == null)
                return;

            try
            {
                string fileName = Utils.GetCharacterFogOfWarFileName(Character.CurrentCharacter.Name);
                string dirName = Utils.StartupPath + Utils.AccountsFolder + Account.Name + "\\";

                Stream stream = File.Create(dirName + fileName);

                XmlSerializer serializer = new XmlSerializer(typeof(FogOfWarSettings));
                serializer.Serialize(stream, this);
                stream.Close();
            }
            catch (Exception e)
            {
                Utils.LogException(e);
            }
        }

        /// <summary>
        /// Loads settings from a file.
        /// </summary>
        /// <param name="filename">The filename to load</param>
        public static FogOfWarSettings Load()
        {
            if (Character.CurrentCharacter == null)
                return new FogOfWarSettings();

            try
            {
                string fileName = Utils.GetCharacterFogOfWarFileName(Character.CurrentCharacter.Name);
                string dirName = Utils.StartupPath + Utils.AccountsFolder + Account.Name + "\\";

                if (!File.Exists(dirName + fileName))
                {
                    return new FogOfWarSettings();
                }

                Stream stream = File.OpenRead(dirName + fileName);
                XmlSerializer serializer = new XmlSerializer(typeof(FogOfWarSettings));
                FogOfWarSettings settings = (FogOfWarSettings)serializer.Deserialize(stream);
                stream.Close();
                return settings;
            }
            catch (Exception e)
            {
                Utils.LogException(e);
                return new FogOfWarSettings();
            }
        }
        #endregion
    }
}
