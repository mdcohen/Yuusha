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
                if ((ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt)) && ks.IsKeyDown(Keys.Enter))
                {
                    return true;
                }

                // ALT + G
                // toggle client mode
                if ((ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt)) && ks.IsKeyDown(Keys.G))
                {
                    return true;
                }

                // ALT + H
                // help window
                if ((ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt)) && ks.IsKeyDown(Keys.H))
                {
                    return true;
                }

                // ALT + I
                // icons window
                if ((ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt)) && ks.IsKeyDown(Keys.I))
                {
                    return true;
                }

                // ALT + K
                // hotkeys window
                if ((ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt)) && ks.IsKeyDown(Keys.K))
                {
                    return true;
                }

                // ALT + L
                // logging toggle
                if ((ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt)) && ks.IsKeyDown(Keys.L))
                {
                    return true;
                }

                // ALT + M
                // macros window
                if ((ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt)) && ks.IsKeyDown(Keys.M))
                {
                    return true;
                }

                // ALT + N
                // news window
                if ((ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt)) && ks.IsKeyDown(Keys.N))
                {
                    return true;
                }

                // ALT + O
                // options window
                if ((ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt)) && ks.IsKeyDown(Keys.O))
                {
                    return true;
                }

                // ALT + P
                // ping response label
                if ((ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt)) && ks.IsKeyDown(Keys.P))
                {
                    return true;
                }

                // ALT + R
                // reload the current sheet
                if ((ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt)) && ks.IsKeyDown(Keys.R))
                {
                    return true;
                }

                // ALT + S
                // sound effects
                if ((ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt)) && ks.IsKeyDown(Keys.S))
                {
                    return true;
                }

                // ALT + T
                // reload IOK tiles
                if ((ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt)) && ks.IsKeyDown(Keys.T))
                {
                    return true;
                }

                // ALT + V
                // toggle client mode
                if ((ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt)) && ks.IsKeyDown(Keys.V))
                {
                    return true;
                }

            //}
            //else if (ks.IsKeyDown(Keys.Escape))
            //{ 
            //    return true;
            //}
            //else if (ks.IsKeyDown(Keys.PrintScreen))
            //{
            //    return true;
            //}
            return false;
        }        

        public static bool HandleKeyboard()
        {
            if (!Client.HasFocus) return false;

            KeyboardState ks = gui.GuiManager.KeyboardState;

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
                            #region ALT + N  News Window
                            if ((ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt)) && ks.IsKeyDown(Keys.N))
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
                            if ((ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt)) && ks.IsKeyDown(Keys.Enter))
                            {
                                Program.Client.ToggleFullScreen();
                                result = true;
                            }
#endregion

                            Sheet sheet = GuiManager.Sheets["Login"];
                            TextBox accountTextBox = (sheet["LoginWindow"] as Window)["AccountTextBox"] as TextBox;
                            TextBox passwordTextBox = (sheet["LoginWindow"] as Window)["PasswordTextBox"] as TextBox;
                            TextBox serverHostTextBox = (sheet["LoginWindow"] as Window)["ServerHostTextBox"] as TextBox;

                            // Client was deactivated while loading. Textboxes lose focus.
                            if (accountTextBox != null && accountTextBox.Text == "" && passwordTextBox != null && passwordTextBox.Text == "")
                                accountTextBox.HasFocus = true;

                            #region Tab between textboxes, or use enter key.
                            if (ks.IsKeyDown(Keys.Tab) || (ks.IsKeyDown(Keys.Enter) && !IsClientKeyboardHandled()))
                            {
                                if (sheet != null)
                                {
                                    Button connectButton = (sheet["LoginWindow"] as Window)["ConnectButton"] as Button;

                                    if (passwordTextBox.HasFocus && passwordTextBox.Text.Length > 0 && accountTextBox.Text.Length > 0)
                                    {
                                        accountTextBox.HasFocus = false;
                                        passwordTextBox.HasFocus = false;
                                        connectButton.IsDisabled = false;
                                        connectButton.HasFocus = true;
                                        connectButton.Click();
                                        return true;
                                    }

                                    bool gavePasswordTextBoxFocus = false;

                                    if (serverHostTextBox != null)
                                    {
                                        if (serverHostTextBox.HasFocus)
                                        {
                                            accountTextBox.HasFocus = true;
                                            accountTextBox.IsCursorVisible = true;

                                            serverHostTextBox.IsCursorVisible = false;
                                            serverHostTextBox.HasFocus = false;
                                        }
                                    }

                                    if (accountTextBox != null)
                                    {
                                        if (accountTextBox.HasFocus && passwordTextBox != null)
                                        {
                                            accountTextBox.HasFocus = false;
                                            accountTextBox.IsCursorVisible = false;

                                            passwordTextBox.IsCursorVisible = true;
                                            passwordTextBox.HasFocus = true;

                                            gavePasswordTextBoxFocus = true;

                                            if (connectButton != null)
                                            {
                                                connectButton.HasFocus = false;
                                            }

                                            result = true;
                                        }
                                    }

                                    if (passwordTextBox != null && !gavePasswordTextBoxFocus)
                                    {
                                        passwordTextBox.HasFocus = false;
                                        passwordTextBox.IsCursorVisible = false;

                                        if (connectButton != null && connectButton.IsVisible)
                                        {
                                            connectButton.HasFocus = true;
                                        }
                                        else if (accountTextBox != null)
                                        {
                                            accountTextBox.IsCursorVisible = true;
                                            accountTextBox.HasFocus = true;
                                            connectButton.HasFocus = false;
                                        }

                                        result = true;
                                    }

                                    if (connectButton != null && !gavePasswordTextBoxFocus && connectButton.IsVisible)
                                    {
                                        // if enter key is pressed when connect button has focus
                                        if (ks.IsKeyDown(Keys.Enter) && connectButton.HasFocus)
                                        {
                                            connectButton.Click();
                                            return true;
                                        }

                                        // hitting the tab key while password text box is focused
                                        if (accountTextBox != null)
                                        {
                                            connectButton.HasFocus = false;

                                            accountTextBox.IsCursorVisible = true;
                                            accountTextBox.HasFocus = true;

                                            result = true;
                                        }
                                    }
                                }
                            }
#endregion

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

                        else
                        {

                            #region ALT + Enter  Full Screen Toggle
                            if ((ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt)) && ks.IsKeyDown(Keys.Enter))
                            {
                                Program.Client.ToggleFullScreen();
                                result = true;
                            }
                            #endregion

                            #region ALT + G  Toggle Game Mode
                                                        if ((ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt)) && ks.IsKeyDown(Keys.G))
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
                                                        if ((ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt)) && ks.IsKeyDown(Keys.H))
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

                            #region ALT + I  Macro Icons Window
                                                        if (ks.IsKeyDown(Keys.LeftAlt) && ks.IsKeyDown(Keys.I))
                                                        {
                                                            gui.Window verticalHotButtonWindow = gui.GuiManager.GenericSheet["VerticalHotButtonWindow"] as gui.Window;
                                                            if (verticalHotButtonWindow != null)
                                                            {
                                                                //if (!iconsWindow.IsVisible)
                                                                //    Events.RegisterEvent(Events.EventName.Load_Options);
                                                                verticalHotButtonWindow.IsVisible = !verticalHotButtonWindow.IsVisible;
                                                                //iconsWindow.HasFocus = iconsWindow.IsVisible;
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
                                                                //if (!iconsWindow.IsVisible)
                                                                //    Events.RegisterEvent(Events.EventName.Load_Options);
                                                                horizontalHotButtonWindow.IsVisible = !horizontalHotButtonWindow.IsVisible;
                                                                //iconsWindow.HasFocus = iconsWindow.IsVisible;
                                                                result = true;
                                                            }
                                                        }
                            #endregion

                            #region ALT + L  Toggle Logging of Scrollable Text Boxes
                                                        if ((ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt)) && ks.IsKeyDown(Keys.L))
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
                                                        if ((ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt)) && ks.IsKeyDown(Keys.N))
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
                                                        if ((ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt)) && ks.IsKeyDown(Keys.R))
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

                                                            if (Client.GameState.ToString().EndsWith("Game"))
                                                                IO.Send("redraw");
                                                        }
                            #endregion

                            #region ALT + T  Reload Game Tiles (Mode Dependent)
                                                        if ((ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt)) && ks.IsKeyDown(Keys.T))
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
                                                        if ((ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt)) && ks.IsKeyDown(Keys.S))
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
                                                        if (ks.IsKeyDown(Keys.OemTilde))
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
    }
}
