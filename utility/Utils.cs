using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;

namespace Yuusha
{
	/// <summary>
	/// Utils class handles file manipulation, logging, and formatting.
	/// </summary>
	public class Utils
	{
        private const string m_mediaFolder = @"media\";
        private const string m_accountsFolder = @"accounts\";
        private const string m_logsFolder = @"logs\";
        private const string m_screenshotsFolder = @"screenshots\";
        private const string m_accountFileName = "account.xml";
        private const string m_settingsFileName = "settings.xml";

        private static System.Collections.Generic.List<string> m_logOnceList = new System.Collections.Generic.List<string>();

        // string message to log = Key, string fileName = Value
        private static System.Collections.Generic.Dictionary<string, string> m_logOnceFileNames = new System.Collections.Generic.Dictionary<string, string>();

        private static readonly string[] searchFolders = new string[] 
        { 
            @".\", @"..\", @"..\..\", @"{0}\", @"{0}\..\", @"{0}\..\..\", @"{0}\..\{1}\", @"{0}\..\..\{1}\" 
        };

        public static string MediaFolder
        {
            get { return m_mediaFolder; }
        }

        public static string AccountsFolder
        {
            get { return m_accountsFolder; }
        }

        public static string AccountFileName
        {
            get { return m_accountFileName; }
        }

        public static string LogsFolder
        {
            get { return m_logsFolder; }
        }

        public static string SettingsFileName
        {
            get { return m_settingsFileName; }
        }

        public static string StartupPath
        {
            get
            {
                System.Reflection.Assembly executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                string exeName = Path.GetFileNameWithoutExtension(executingAssembly.Location);
                string exeFolder = Path.GetDirectoryName(executingAssembly.Location);
                return exeFolder + "\\";
            }
        }

		public Utils()
		{
            // empty
		}

        public static string GetCharacterFileName(string name)
        {
            name = name.Replace(".", "");
            name = name.ToLower();
            name = name + ".xml";

            return name;
        }

        public static void CreateDirectories()
        {            
            // create account folder
            if(!Directory.Exists(StartupPath + AccountsFolder))
                Directory.CreateDirectory(StartupPath + AccountsFolder);
            // create logs folder
            if (!Directory.Exists(StartupPath + LogsFolder))
                Directory.CreateDirectory(StartupPath + LogsFolder);
        }

        /// <summary>
        /// Finds a media file. Adapted from the DirectX Sample Framework.
        /// </summary>
        /// <param name="file">Name of the file we're looking for.</param>
        /// <returns>The path of the file.</returns>
        /// <remarks>If the file cannot be found, an exception will be thrown.</remarks>
        public static string GetMediaFile( string file )
        {
            // Find out the executing assembly information
            System.Reflection.Assembly executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            string exeName = Path.GetFileNameWithoutExtension( executingAssembly.Location );
            string exeFolder = Path.GetDirectoryName( executingAssembly.Location );
            string filePath;

            // Search all the folders in searchFolders
            if ( SearchFolders( file, exeFolder, exeName, out filePath ) )
            {
                return filePath;
            }

            // Search all the folders in searchFolders with media\ appended to the end
            if ( SearchFolders( m_mediaFolder + file, exeFolder, exeName, out filePath ) )
            {
                return filePath;
            }

            Utils.LogOnce("Unable to find media file [ " + file + " ]");
            return "";
        }

        /// <summary>
        /// Searches the list of folders for the file. From the DirectX Sample Framework.
        /// </summary>
        /// <param name="filename">File we are looking for</param>
        /// <param name="exeFolder">Folder of the executable</param>
        /// <param name="exeName">Name of the executable</param>
        /// <param name="fullPath">Returned path if file is found.</param>
        /// <returns>true if the file was found; false otherwise</returns>
        private static bool SearchFolders( string filename, string exeFolder, string exeName, out string fullPath )
        {
            // Look through each folder to find the file
            for ( int i = 0; i < searchFolders.Length; i++ )
            {
                try
                {
                    FileInfo info = new FileInfo( string.Format( searchFolders[i], exeFolder, exeName) + filename );
                    if ( info.Exists )
                    {
                        fullPath = info.FullName;
                        return true;
                    }
                }
                catch ( NotSupportedException )
                {
                    continue;
                }
            }
            // didn't find it
            fullPath = string.Empty;
            return false;
        }

        public static Color GetColor(string colorName)
        {
            if (colorName.Contains(" "))
            {
                try
                {
                    string[] colorValues = colorName.Split(" ".ToCharArray());

                    if (colorValues.Length == 3)
                    {
                        return new Color(Convert.ToByte(colorValues[0]), Convert.ToByte(colorValues[1]),
                            Convert.ToByte(colorValues[2]));
                    }
                }
                catch
                {
                    Utils.LogOnce("Failed to convert [ " + colorName + " ] to new Color. Returned Color.White instead");
                    return Color.White;
                }
            }

            PropertyInfo property = typeof(Color).GetProperty(colorName);

            object color = Activator.CreateInstance(typeof(Color));

            if (property != null)
            {
                return (Color)property.GetValue(color, null);
            }
            
            return Color.White;
        }

        public static void LogOnce(string message)
        {
            if (!m_logOnceList.Contains(message))
            {
                Utils.Log(message);
                m_logOnceList.Add(message);
            }
        }

        public static void Log(string message)
        {
            if (m_logOnceList.Contains(message))
                return;

            try
            {
                FileStream file = new FileStream(Utils.StartupPath + Utils.LogsFolder + "Yuusha_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Year.ToString().Substring(2) + ".log", FileMode.Append, FileAccess.Write);
                StreamWriter rw = new StreamWriter(file);
                rw.WriteLine(DateTime.Now.ToString() + ": " + message);
                rw.Close();
                file.Close();
            }
            catch (DirectoryNotFoundException dnfEx)
            {
                Directory.CreateDirectory(Utils.StartupPath + Utils.LogsFolder);
                Utils.Log(dnfEx.Message);
            }
        }

        public static void LogOnceToFile(string message, string fileName)
        {
            if (m_logOnceFileNames.ContainsKey(message) && m_logOnceFileNames[message] == fileName)
                return;
            else m_logOnceFileNames.Add(message, fileName);

            try
            {
                FileStream file = new FileStream(Utils.StartupPath + Utils.LogsFolder + "Yuusha_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Year.ToString().Substring(2) + "_" + fileName + ".log", FileMode.Append, FileAccess.Write);
                StreamWriter rw = new StreamWriter(file);
                rw.WriteLine(DateTime.Now.ToString() + ": " + message);
                rw.Close();
                file.Close();
            }
            catch (DirectoryNotFoundException dnfEx)
            {
                Directory.CreateDirectory(Utils.StartupPath + Utils.LogsFolder);
                Utils.Log(dnfEx.Message);
            }
        }

        public static void LogRequest(string message)
        {
            try
            {
                message = message.Replace("\r\n", "");
                FileStream file = new FileStream(Utils.StartupPath + Utils.LogsFolder + "Yuusha_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Year.ToString().Substring(2) + "_" + Client.GameState.ToString() + ".log", FileMode.Append, FileAccess.Write);
                StreamWriter rw = new StreamWriter(file);
                rw.WriteLine(DateTime.Now.ToString() + ": " + message);
                rw.Close();
                file.Close();
            }
            catch (DirectoryNotFoundException dnfEx)
            {
                Directory.CreateDirectory(Utils.StartupPath + Utils.LogsFolder);
                Utils.Log(dnfEx.Message);
            }
        }

        public static void SaveScreenshot()
        {
            try
            {
                GraphicsDevice device = Program.Client.GraphicsDevice;

                Color[] screenData = new Color[device.PresentationParameters.BackBufferWidth *
                                           device.PresentationParameters.BackBufferHeight];

                RenderTarget2D screenShot = new RenderTarget2D(device,
                    device.PresentationParameters.BackBufferWidth,
                    device.PresentationParameters.BackBufferHeight);

                device.SetRenderTarget(screenShot);

                Program.Client.GUIManager.Draw(new GameTime());

                device.SetRenderTarget(null);

                string fileName = "screenshot_" +
                    DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Year + DateTime.Now.Hour + DateTime.Now.Minute +
                    DateTime.Now.Second + DateTime.Now.Millisecond + ".png";

                if(!Directory.Exists(Utils.StartupPath + m_screenshotsFolder))
                    Directory.CreateDirectory(Utils.StartupPath + m_screenshotsFolder);

                //save to disk
                FileStream fs = new FileStream(Utils.StartupPath + m_screenshotsFolder + fileName, FileMode.CreateNew);

                screenShot.SaveAsPng(fs, screenShot.Width, screenShot.Height);

                fs.Dispose();

                screenShot.Dispose();

                gui.TextCue.AddClientInfoTextCue(fileName, gui.TextCue.TextCueTag.None, Color.Lime, Color.Black, 2000, false, false, true);
            }
            catch (Exception e)
            { Utils.LogException(e); }
        }

        public static void LogException(Exception e)
        {
            Log("{Exception} " + e.Message + " Stack: " + e.StackTrace);
        }

        public static string FormatEnumString(string enumString)
        {
            enumString = enumString.Replace("__", "'");
            enumString = enumString.Replace("_", " ");
            return enumString;
        }

        private static string m_clipboard = "";

        public static String GetClipboardText()
        {
            System.Threading.Thread t = new System.Threading.Thread(GetClipboard);
            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.Start();
            while (t.IsAlive) { }
            return m_clipboard;
        }

        public static void SetClipboardText(string text)
        {
            m_clipboard = text;
            System.Threading.Thread t = new System.Threading.Thread(SetClipboard);
            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.Start();
            while (t.IsAlive) { }
        }

        [STAThread]
        private static void GetClipboard()
        {
            if (Clipboard.ContainsText())
            {
                m_clipboard = Clipboard.GetText(TextDataFormat.UnicodeText);
                //Clipboard.SetText(replacementHtmlText, TextDataFormat.UnicodeText);
            }
        }

        [STAThread]
        private static void SetClipboard()
        {
            Clipboard.SetText(m_clipboard, TextDataFormat.UnicodeText);
        }

        /// <summary>
        /// Returns a color for drawing display text based on values in UserSettings.cs.
        /// </summary>
        /// <param name="textType">The text type color requested.</param>
        /// <returns>The Color of the eTextType.</returns>
        public static Color GetTextTypeColor(Enums.ETextType textType)
        {
            foreach (var t in System.Reflection.Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.GetType() == typeof(Utility.Settings.UserSettings))
                {
                    FieldInfo colorField = t.GetField("ColorTextType_" + textType.ToString());

                    if (colorField == null) return Color.White;
                    else
                    {
                        try
                        {
                            return (Color)colorField.GetValue(typeof(Color));
                        }
                        catch(Exception e)
                        {
                            Utils.LogException(e);
                            return Color.White;
                        }
                    }
                }
            }

            return Color.White;
        }
	}
}
