using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;
using Yuusha.gui;

namespace Yuusha
{
    public static class KeyboardHandler
    {
        static Keys[] PressedKeys;

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);

        static bool IsClientKeyboardHandled()
        {
            KeyboardState ks = gui.GuiManager.KeyboardState;

            //if (Client.GameState != Enums.eGameState.Splash)
            //{
            // ALT + Enter
            // toggle full screen if sheet allows full screen
            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.Enter))
            {
                return true;
            }

            // ALT + C
            // toggle sheet (testing purposes)
            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.C))
            {
                return true;
            }

            // ALT + G
            // toggle client mode
            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.G))
            {
                return true;
            }

            // ALT + H
            // help window
            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.H))
            {
                return true;
            }

            // ALT + I
            // icons window
            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.I))
            {
                return true;
            }

            // ALT + K
            // hotkeys window
            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.K))
            {
                return true;
            }

            // ALT + L
            // logging toggle
            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.L))
            {
                return true;
            }

            // ALT + M
            // macros window
            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.M))
            {
                return true;
            }

            // ALT + N
            // news window
            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.N))
            {
                return true;
            }

            // ALT + O
            // options window
            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.O))
            {
                return true;
            }

            // ALT + P
            // ping response label
            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.P))
            {
                return true;
            }

            // ALT + R
            // reload the current sheet
            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.R))
            {
                return true;
            }

            // ALT + S
            // sound effects
            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.S))
            {
                return true;
            }

            // ALT + T
            // reload IOK tiles
            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.T))
            {
                return true;
            }

            // ALT + V
            // toggle client mode
            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.V))
            {
                return true;
            }

            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.W))
            {
                return true;
            }

            return false;
        }

        public static bool HandleKeyboard()
        {
            if (!Client.HasFocus) return false;

            KeyboardState ks = GuiManager.KeyboardState;

            bool result = false;

            Keys[] newKeys = ks.GetPressedKeys();

            if (PressedKeys != null)
            {
                foreach (Keys k in newKeys)
                {
                    bool bFound = false;

                    foreach (Keys k2 in PressedKeys)
                    {
                        if (k == k2)
                        {
                            bFound = true;
                            PressedKeys = newKeys;
                            return IsClientKeyboardHandled();
                        }
                    }

                    if (!bFound)
                    {
                        #region Login Game State
                        if (Client.GameState == Enums.EGameState.Login)
                        {
                            // Testing purposes ALT + C, ALT + W
                            #region Testing Area
                            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.C))
                            {
                                Events.RegisterEvent(Events.EventName.Set_Game_State, Enums.EGameState.CharacterGeneration);
                            }

                            if(IsAltKeyDown(ks) && ks.IsKeyDown(Keys.F))
                            {
                                string randomSound = "0001";
                                int random = new Random().Next(1, 273);
                                randomSound = random.ToString();
                                randomSound = randomSound.PadLeft(4, '0');

                                try
                                {
                                    List<string> li = new List<string>() { randomSound, new Random().Next(0, 7).ToString(), new Random().Next(8).ToString() };
                                    Sound.Play(li);
                                }
                                catch (Exception e)
                                {
                                    Utils.LogException(e);
                                }

                                //if (GuiManager.ParticalEngine == null)
                                //    GuiManager.ParticalEngine = new gui.effects.ParticleEngine(new List<Texture2D>() { GuiManager.Textures["heartrate.png"] }, new Vector2(100, 100));
                                //else GuiManager.ParticalEngine = null;

                                //gui.effects.Particle particle = new gui.effects.Particle()
                            }

                            #region ALT + O  Options Window
                            if (ks.IsKeyDown(Keys.LeftAlt) && ks.IsKeyDown(Keys.O))
                            {
                                gui.Window optWindow = gui.GuiManager.GenericSheet["OptionsWindow"] as gui.Window;
                                if (optWindow != null)
                                {
                                    if (!optWindow.IsVisible)
                                        Events.RegisterEvent(Events.EventName.Load_Character_Settings);
                                    optWindow.IsVisible = !optWindow.IsVisible;
                                    optWindow.HasFocus = optWindow.IsVisible;
                                    result = true;
                                }
                            }
                            #endregion

                            #region ALT + I  Vertical Hot Button Window
                            if (ks.IsKeyDown(Keys.LeftAlt) && ks.IsKeyDown(Keys.I))
                            {
                                gui.Window verticalHotButtonWindow = gui.GuiManager.GenericSheet["VerticalHotButtonWindow"] as gui.Window;
                                if (verticalHotButtonWindow != null)
                                {
                                    verticalHotButtonWindow.IsVisible = !verticalHotButtonWindow.IsVisible;
                                    result = true;
                                }
                            }
                            #endregion

                            #region ALT + K  Horizontal Hot Button Window
                            if (ks.IsKeyDown(Keys.LeftAlt) && ks.IsKeyDown(Keys.K))
                            {
                                gui.Window horizontalHotButtonWindow = gui.GuiManager.GenericSheet["HorizontalHotButtonWindow"] as gui.Window;
                                if (horizontalHotButtonWindow != null)
                                {
                                    horizontalHotButtonWindow.IsVisible = !horizontalHotButtonWindow.IsVisible;
                                    result = true;
                                }
                            }
                            #endregion
                            #endregion

                            #region ALT + N  News Window
                            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.N))
                            {
                                gui.Window newsWindow = gui.GuiManager.GenericSheet["NewsWindow"] as gui.Window;
                                if (newsWindow != null)
                                {
                                    if (!newsWindow.IsVisible)
                                    {
                                        (newsWindow["NewsScrollableTextBox"] as gui.ScrollableTextBox).Clear();

                                        try
                                        {
                                            for (int a = 0; a < World.News.Count; a++)
                                            {
                                                string[] nz = World.News[a].Split(Protocol.ISPLIT.ToCharArray());
                                                foreach (string line in nz)
                                                {
                                                    (newsWindow["NewsScrollableTextBox"] as gui.ScrollableTextBox).AddLine(line.Trim(), Enums.ETextType.Default);
                                                }
                                            }
                                        }
                                        catch (System.IO.FileNotFoundException)
                                        {
                                            (newsWindow["NewsScrollableTextBox"] as gui.ScrollableTextBox).AddLine("Failed to display news.", Enums.ETextType.Default);
                                        }
                                    }
                                    newsWindow.IsVisible = !newsWindow.IsVisible;
                                    result = true;
                                }
                            }
                            #endregion

                            #region Tilde Saves a Screenshot
                            if (ks.IsKeyDown(Keys.OemTilde))
                            {
                                Utils.SaveScreenshot();
                            }
                            #endregion

                            #region ALT + Enter  Full Screen Toggle
                            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.Enter))
                            {
                                Program.Client.ToggleFullScreen();
                                result = true;
                            }
                            #endregion

                            #region ALT + R  Reload Current GUI Sheet
                            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.R))
                            {
                                Program.Client.GUIManager.LoadSheet(gui.GuiManager.GenericSheet.FilePath);
                                Program.Client.GUIManager.LoadSheet(gui.GuiManager.CurrentSheet.FilePath);
                                if (Client.IsFullScreen)
                                {
                                    GuiManager.GenericSheet.OnClientResize(Client.PrevClientBounds, Client.NowClientBounds);
                                    GuiManager.CurrentSheet.OnClientResize(Client.PrevClientBounds, Client.NowClientBounds);
                                }

                                gui.TextCue.AddClientInfoTextCue("Reloaded " + gui.GuiManager.CurrentSheet.Description + " and Generic Sheet.", gui.TextCue.TextCueTag.None, Color.LimeGreen, Color.Transparent, 2000, false, true, false);

                                result = true;
                            }
                            #endregion

                            if ((ks.IsKeyDown(Keys.Tab)) || (GuiManager.ControlWithFocus is TextBox && (ks.IsKeyDown(Keys.Enter) && ks.GetPressedKeys().Length == 1)))
                            {
                                if (!IsAltKeyDown(ks))
                                {
                                    if (!IsShiftKeyDown(ks))
                                        GuiManager.CurrentSheet.HandleTabOrderForward();
                                    else GuiManager.CurrentSheet.HandleTabOrderReverse();
                                }
                            }

                            Sheet sheet = GuiManager.Sheets["Login"];
                            TextBox accountTextBox = (sheet["LoginWindow"] as Window)["AccountTextBox"] as TextBox;
                            TextBox passwordTextBox = (sheet["LoginWindow"] as Window)["PasswordTextBox"] as TextBox;
                            TextBox serverHostTextBox = (sheet["LoginWindow"] as Window)["ServerHostTextBox"] as TextBox;

                            if ((ks.IsKeyDown(Keys.LeftControl) || ks.IsKeyDown(Keys.RightControl)) && ks.IsKeyDown(Keys.V))
                            {
                                if (sheet != null)
                                {
                                    if (accountTextBox != null && accountTextBox.HasFocus)
                                    {
                                        accountTextBox.Text = Utils.GetClipboardText();
                                        accountTextBox.SelectAll();
                                    }
                                    else if (passwordTextBox != null && passwordTextBox.HasFocus)
                                    {
                                        passwordTextBox.Text = Utils.GetClipboardText();
                                        passwordTextBox.SelectAll();
                                    }
                                }
                            }
                        }
                        #endregion
                        else if (Client.GameState == Enums.EGameState.CharacterGeneration)
                        {
                            // Testing purposes
                            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.C))
                            {
                                Events.RegisterEvent(Events.EventName.Set_Game_State, Enums.EGameState.Login);
                            }

                            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.Enter))
                            {
                                Program.Client.ToggleFullScreen();
                                result = true;
                            }

                            #region ALT + M  Macros Window
                            if (ks.IsKeyDown(Keys.LeftAlt) && ks.IsKeyDown(Keys.M))
                            {
                                gui.Window macrosWindow = gui.GuiManager.GenericSheet["MacrosWindow"] as gui.Window;
                                if (macrosWindow != null)
                                {
                                    macrosWindow.IsVisible = !macrosWindow.IsVisible;
                                    result = true;
                                }
                            }
                            #endregion

                            #region ALT + R  Reload Current GUI Sheet
                            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.R))
                            {
                                Program.Client.GUIManager.LoadSheet(GuiManager.GenericSheet.FilePath);
                                Program.Client.GUIManager.LoadSheet(GuiManager.CurrentSheet.FilePath);
                                if (Client.IsFullScreen)
                                {
                                    GuiManager.GenericSheet.OnClientResize(Client.PrevClientBounds, Client.NowClientBounds);
                                    GuiManager.CurrentSheet.OnClientResize(Client.PrevClientBounds, Client.NowClientBounds);
                                }

                                gui.TextCue.AddClientInfoTextCue("Reloaded " + gui.GuiManager.CurrentSheet.Description + " and Generic Sheet.", gui.TextCue.TextCueTag.None, Color.LimeGreen, Color.Transparent, 2000, false, true, false);

                                result = true;

                                Character.LoadSettings();
                                GenericSheet.LoadMacros();
                            }
                            #endregion
                        }
                        else if (Client.GameState == Enums.EGameState.HotButtonEditMode)
                        {
                            // nothing
                        }
                        else // menu, game, conf
                        {
                            // Testing purposes
                            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.C))
                            {
                                Events.RegisterEvent(Events.EventName.Set_Game_State, Enums.EGameState.Login);
                            }

                            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.W))
                            {
                                IO.Send(Protocol.REQUEST_CHARACTER_SACK);
                                IO.Send(Protocol.REQUEST_CHARACTER_BELT);
                            }

                            if ((ks.IsKeyDown(Keys.Tab)) || (GuiManager.ControlWithFocus is TextBox && (ks.IsKeyDown(Keys.Enter) && ks.GetPressedKeys().Length == 1)))
                            {
                                if (!ks.IsKeyDown(Keys.LeftAlt) && !ks.IsKeyDown(Keys.RightAlt))
                                {
                                    if (!ks.IsKeyDown(Keys.LeftShift) && !ks.IsKeyDown(Keys.RightShift))
                                        GuiManager.CurrentSheet.HandleTabOrderForward();
                                    else GuiManager.CurrentSheet.HandleTabOrderReverse();
                                }
                            }

                            #region ALT + Enter  Full Screen Toggle
                            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.Enter))
                            {
                                if (Client.GameState == Enums.EGameState.HotButtonEditMode) return true;

                                Program.Client.ToggleFullScreen();
                                result = true;
                            }
                            #endregion

                            #region ALT + G  Toggle Game Mode
                            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.G))
                            {
                                if (Client.GameDisplayMode == Enums.EGameDisplayMode.IOK)
                                {
                                    Events.RegisterEvent(Events.EventName.Set_Client_Mode, Enums.EGameDisplayMode.Spinel);
                                }
                                else if (Client.GameDisplayMode == Enums.EGameDisplayMode.Spinel)
                                {
                                    Events.RegisterEvent(Events.EventName.Set_Client_Mode, Enums.EGameDisplayMode.IOK);
                                }

                                gui.TextCue.AddClientInfoTextCue(Client.GameDisplayMode.ToString() + " Mode", TextCue.TextCueTag.None, Color.Red, Color.Transparent, 2500, false, true, false);

                                result = true;
                            }
                            #endregion

                            #region ALT + H  Help Window
                                                        if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.H))
                                                        {
                                                            gui.Window helpWindow = gui.GuiManager.GenericSheet["HelpWindow"] as gui.Window;

                                                            if (helpWindow != null)
                                                            {
                                                                if (!helpWindow.IsVisible)
                                                                {
                                                                    (helpWindow["HelpScrollableTextBox"] as gui.ScrollableTextBox).Clear();

                                                                    try
                                                                    {
                                                                        System.IO.StreamReader sr = new System.IO.StreamReader(Utils.GetMediaFile("help.txt"));

                                                                        while (!sr.EndOfStream)
                                                                        {
                                                                            (helpWindow["HelpScrollableTextBox"] as gui.ScrollableTextBox).AddLine(sr.ReadLine(), Enums.ETextType.Default);
                                                                        }
                                                                        sr.Close();
                                                                    }
                                                                    catch (System.IO.FileNotFoundException)
                                                                    {
                                                                        (helpWindow["HelpScrollableTextBox"] as gui.ScrollableTextBox).AddLine("The help.txt file is missing from the media subdirectory.", Enums.ETextType.Default);
                                                                    }
                                                                }

                                                                helpWindow.IsVisible = !helpWindow.IsVisible;

                                                                result = true;
                                                            }
                                                        }
                            #endregion

                            #region ALT + I  Vertical Hot Button Window
                            if (ks.IsKeyDown(Keys.LeftAlt) && ks.IsKeyDown(Keys.I))
                            {
                                gui.Window verticalHotButtonWindow = gui.GuiManager.GenericSheet["VerticalHotButtonWindow"] as gui.Window;
                                if (verticalHotButtonWindow != null)
                                {
                                    verticalHotButtonWindow.IsVisible = !verticalHotButtonWindow.IsVisible;
                                    result = true;
                                }
                            }
                            #endregion

                            #region ALT + K  Horizontal Hot Button Window
                            if (ks.IsKeyDown(Keys.LeftAlt) && ks.IsKeyDown(Keys.K))
                            {
                                gui.Window horizontalHotButtonWindow = gui.GuiManager.GenericSheet["HorizontalHotButtonWindow"] as gui.Window;
                                if (horizontalHotButtonWindow != null)
                                {
                                    horizontalHotButtonWindow.IsVisible = !horizontalHotButtonWindow.IsVisible;
                                    result = true;
                                }
                            }
                            #endregion

                            #region ALT + L  Toggle Logging of Scrollable Text Boxes
                                                        if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.L))
                                                        {
                                                            GuiManager.LoggingRequested = !GuiManager.LoggingRequested;
                                                            string onoff = "Enabled";
                                                            if (!GuiManager.LoggingRequested) onoff = "Disabled";
                                                            gui.TextCue.AddClientInfoTextCue("Logging " + onoff, TextCue.TextCueTag.None, Color.Red, Color.Transparent, 2500, false, true, false);
                                                            result = true;
                                                        }
                            #endregion

                            #region ALT + M  Macros Window
                            if (ks.IsKeyDown(Keys.LeftAlt) && ks.IsKeyDown(Keys.M))
                            {
                                gui.Window macrosWindow = gui.GuiManager.GenericSheet["MacrosWindow"] as gui.Window;
                                if (macrosWindow != null)
                                {
                                    macrosWindow.IsVisible = !macrosWindow.IsVisible;
                                    result = true;
                                }
                            }
                            #endregion

                            #region ALT + N  News Window
                                                        if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.N))
                                                        {
                                                            gui.Window newsWindow = gui.GuiManager.GenericSheet["NewsWindow"] as gui.Window;
                                                            if (newsWindow != null)
                                                            {
                                                                if (!newsWindow.IsVisible)
                                                                {
                                                                    (newsWindow["NewsScrollableTextBox"] as gui.ScrollableTextBox).Clear();

                                                                    try
                                                                    {
                                                                        for (int a = 0; a < World.News.Count; a++)
                                                                        {
                                                                            string[] nz = World.News[a].Split(Protocol.ISPLIT.ToCharArray());
                                                                            foreach (string line in nz)
                                                                            {
                                                                                (newsWindow["NewsScrollableTextBox"] as gui.ScrollableTextBox).AddLine(line.Trim(), Enums.ETextType.Default);
                                                                            }
                                                                        }
                                                                    }
                                                                    catch (System.IO.FileNotFoundException)
                                                                    {
                                                                        (newsWindow["NewsScrollableTextBox"] as gui.ScrollableTextBox).AddLine("Failed to display news.", Enums.ETextType.Default);
                                                                    }
                                                                }
                                                                newsWindow.IsVisible = !newsWindow.IsVisible;
                                                                result = true;
                                                            }
                                                        }
                            #endregion

                            #region ALT + O  Options Window
                            if (ks.IsKeyDown(Keys.LeftAlt) && ks.IsKeyDown(Keys.O))
                            {
                                gui.Window optWindow = gui.GuiManager.GenericSheet["OptionsWindow"] as gui.Window;
                                if (optWindow != null)
                                {
                                    if (!optWindow.IsVisible)
                                        Events.RegisterEvent(Events.EventName.Load_Character_Settings);
                                    optWindow.IsVisible = !optWindow.IsVisible;
                                    optWindow.HasFocus = optWindow.IsVisible;
                                    result = true;
                                }
                            }
                            #endregion

                            #region ALT + R  Reload Current GUI Sheet
                            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.R))
                            {
                                Program.Client.GUIManager.LoadSheet(GuiManager.GenericSheet.FilePath);
                                Program.Client.GUIManager.LoadSheet(GuiManager.CurrentSheet.FilePath);
                                if (Client.IsFullScreen)
                                {
                                    GuiManager.GenericSheet.OnClientResize(Client.PrevClientBounds, Client.NowClientBounds);
                                    GuiManager.CurrentSheet.OnClientResize(Client.PrevClientBounds, Client.NowClientBounds);
                                }

                                gui.TextCue.AddClientInfoTextCue("Reloaded " + gui.GuiManager.CurrentSheet.Description + " and Generic Sheet.", gui.TextCue.TextCueTag.None, Color.LimeGreen, Color.Transparent, 2000, false, true, false);

                                result = true;

                                if (Client.GameState.ToString().EndsWith("Game"))
                                    IO.Send("redraw");

                                Character.LoadSettings();
                                gui.GenericSheet.LoadMacros();
                            }
                            #endregion

                            #region ALT + T  Reload Game Tiles (Mode Dependent)
                                                        if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.T))
                                                        {
                                                            if (Client.GameDisplayMode == Enums.EGameDisplayMode.IOK)
                                                            {
                                                                gui.IOKMode.Tiles.Clear();
                                                                Program.Client.GUIManager.ReloadIOKTiles();
                                                            }
                                                            result = true;
                                                        }
                            #endregion

                            #region ALT + S  Toggle Sound
                            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.S))
                            {
                                Client.UserSettings.SoundEffects = !Client.UserSettings.SoundEffects;
                                string onoff = "Enabled";
                                if (!Client.UserSettings.SoundEffects) onoff = "Disabled";
                                gui.TextCue.AddClientInfoTextCue("Sound Effects " + onoff, TextCue.TextCueTag.None, Color.Red, Color.Transparent, 2500, false, true, false);
                                result = true;
                            }
                            #endregion

#if DEBUG
                            #region ALT + V  Visuals Window
                            if (ks.IsKeyDown(Keys.LeftAlt) && ks.IsKeyDown(Keys.V))
                            {
                                gui.Window iconsWindow = gui.GuiManager.GenericSheet["VisualKeyWindow"] as gui.Window;
                                if (iconsWindow != null)
                                {
                                    //if (!iconsWindow.IsVisible)
                                    //    Events.RegisterEvent(Events.EventName.Load_Options);
                                    iconsWindow.IsVisible = !iconsWindow.IsVisible;
                                    //iconsWindow.HasFocus = iconsWindow.IsVisible;
                                    result = true;
                                }
                            }
                            #endregion
#endif

                            #region Tilde Saves a Screenshot
                            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.OemTilde))
                            {
                                Utils.SaveScreenshot();
                            }
                            #endregion

                        }
                    }
                }
            }

            PressedKeys = newKeys;
            return result;
        }

        public static bool IsAltKeyDown(KeyboardState ks)
        {
            return ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt);
        }

        public static bool IsShiftKeyDown(KeyboardState ks)
        {
            return ks.IsKeyDown(Keys.LeftShift) || ks.IsKeyDown(Keys.RightShift);
        }
    }
}