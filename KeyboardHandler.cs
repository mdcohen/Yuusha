﻿using System;
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
            KeyboardState ks = GuiManager.KeyboardState;

            if (GuiManager.GenericSheet["LogoutOptionWindow"] is Window logoutOptionWindow && logoutOptionWindow.IsVisible)
            {
                return true;
            }

            // ALT + Enter
            // toggle full screen if sheet allows full screen
            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.Enter) && Utility.Settings.StaticSettings.FullScreenToggleEnabled)
            {
                return true;
            }

            // ALT + A
            // toggle ambience
            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.A))
            {
                return true;
            }

            // ALT + C
            // toggle sheet (testing purposes)
            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.C))
            {
                return true;
            }

            // ALT + F
            // frames per second text cue
            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.F))
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

            // ALT + Q
            // primary testing purpose
            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.Q))
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
            // Talents Window
            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.T))
            {
                return true;
            }

            // ALT + U
            // Reload IOK game tiles
            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.U))
            {
                return true;
            }

            // ALT + V
            // toggle client mode
            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.V))
            {
                return true;
            }

            // ALT + W
            // toggle Fog of War
            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.W))
            {
                return true;
            }

            // ALT + Add
            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.Add))
            {
                return true;
            }

            // ALT + Minus
            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.Subtract))
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
                        if (GuiManager.GenericSheet["LogoutOptionWindow"] is Window logoutOptionWindow && logoutOptionWindow.IsVisible)
                        {
                            if (Client.InGame && ks.IsKeyDown(Keys.Q))
                            {
                                Events.RegisterEvent(Events.EventName.Send_Command, "quit");
                            }
                            else if (ks.IsKeyDown(Keys.L))
                            {
                                if (Client.InGame)
                                    Events.RegisterEvent(Events.EventName.Send_Command, "quit");

                                Events.RegisterEvent(Events.EventName.Logout);
                            }
                            else if (ks.IsKeyDown(Keys.X))
                            {
                                if (Client.InGame)
                                    Events.RegisterEvent(Events.EventName.Send_Command, "quit");

                                Events.RegisterEvent(Events.EventName.Logout);

                                Program.Client.Exit();
                            }
                            else if (ks.IsKeyDown(Keys.R))
                            {
                                logoutOptionWindow.OnClose();
                                GuiManager.Dispose(logoutOptionWindow);
                            }

                            return true;
                        }

                        #region Login Game State
                        if (Client.GameState == Enums.EGameState.Login)
                        {
                            if (ks.IsKeyDown(Keys.Escape) && IO.LoginState == Enums.ELoginState.Disconnected)
                            {
                                //GameHUD.DisplayLogoutOptionScreen();
                                Program.Client.Exit();
                                return true;
                            }

                            #region ALT + I  Vertical Hot Button Window
                            //if (ks.IsKeyDown(Keys.LeftAlt) && ks.IsKeyDown(Keys.I))
                            //{
                            //    Events.RegisterEvent(Events.EventName.Toggle_VerticalHotbar);
                            //    result = true;
                            //}
                            #endregion

                            #region ALT + K  Horizontal Hot Button Window
                            //if (ks.IsKeyDown(Keys.LeftAlt) && ks.IsKeyDown(Keys.K))
                            //{
                            //    Events.RegisterEvent(Events.EventName.Toggle_HorizontalHotbar);
                            //    result = true;
                            //}
                            #endregion

                            if (ks.IsKeyDown(Keys.LeftAlt) && ks.IsKeyDown(Keys.F))
                            {
                                TextCue.AddFPSTextCue(GameHUD.UpdateCumulativeMovingAverageFPS((float)(1 / Program.Client.ClientGameTime.ElapsedGameTime.TotalSeconds)).ToString("0.00"));
                                result = true;
                            }

                            // Testing purposes ALT + C, ALT + W, ALT + Q
                            #region Testing Area aka the Playground
                            //if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.W))
                            //{
                            //Control mapWindow = GuiManager.GetControl("PrimaryMapWindow");

                            //if (mapWindow == null)
                            //    MapWindow.CreateMapWindow();
                            //else mapWindow.IsVisible = !mapWindow.IsVisible;
                            //result = true;
                            //}

                            //if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.C))
                            //{
                            //    Events.RegisterEvent(Events.EventName.Set_Game_State, Enums.EGameState.CharacterGeneration);
                            //}

                            //if(IsAltKeyDown(ks) && ks.IsKeyDown(Keys.F))
                            //{
                            //string randomSound = "0001";
                            //int random = new Random().Next(1, 273);
                            //randomSound = random.ToString();
                            //randomSound = randomSound.PadLeft(4, '0');

                            //try
                            //{
                            //    List<string> li = new List<string>() { randomSound, new Random().Next(0, 7).ToString(), new Random().Next(8).ToString() };
                            //    Sound.Play(li);
                            //}
                            //catch (Exception e)
                            //{
                            //    Utils.LogException(e);
                            //}

                            //if (GuiManager.ParticalEngine == null)
                            //    GuiManager.ParticalEngine = new gui.effects.ParticleEngine(new List<Texture2D>() { GuiManager.Textures["heartrate.png"] }, new Vector2(100, 100));
                            //else GuiManager.ParticalEngine = null;

                            //gui.effects.Particle particle = new gui.effects.Particle()
                            //}

                            //if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.Q))
                            //{
                            //    //Utils.LogCharacterFields();
                            //    //Utils.LogCharacterEffects();
                            //    //foreach (Spell spell in World.SpellsList)
                            //    //    Utils.Log(spell.Name);
                            //    //IO.Send(Protocol.REQUEST_CHARACTER_EFFECTS);
                            //    //TextCue.AddZNameTextCue("Haunt of the Ghost Paladin");
                            //    //IO.Send(Protocol.REQUEST_CHARACTER_STATS);

                            //    //SpellBookWindow.CreateSpellBookWindow();
                            //    //SpellRingWindow.CreateSpellRingWindow();
                            //    //SpellWarmingWindow.CreateSpellWarmingWindow("Icespear");

                            //    //AchievementLabel.CreateAchievementLabel("Mistress of Earth and Sky", TextManager.ScalingTextFontList[TextManager.ScalingTextFontList.Count - 1], GameHUD.GameIconsDictionary["magic"], Color.Indigo, Color.PaleGreen, "", true, Map.Direction.Southwest);


                            //    //if (GuiManager.GenericSheet["CharacterStatsWindow"] is Window characterStatsWindow)
                            //    //{
                            //    //    characterStatsWindow.IsVisible = !characterStatsWindow.IsVisible;
                            //    //    characterStatsWindow.ZDepth = 1;
                            //    //    if(Character.CurrentCharacter != null)
                            //    //        Utils.LogCharacterFields();
                            //    //}
                            //    //result = true;
                            //    //GameHUD.LightningFlash();
                            //    //GameHUD.CrumbleScreen(Map.Direction.South, 30, false, true);
                            //    //GameHUD.SynchronouslySlideScreen(Map.Direction.East, 15, true, true);
                            //    //GameHUD.SplitSlideScreen(true, 20, true);
                            //    result = true;
                            //}

                            //if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.B))
                            //{
                            //    //GameHUD.ReturnScreen = true;
                            //    //GameHUD.ReturnCrumbledScreen(false);
                            //    //GameHUD.ReturnSlidScreen = true;
                            //    //GameHUD.ReturnSplitSlidScreen = true;
                            //    result = true;
                            //}

                            #region ALT + O  Options Window
                            //if (ks.IsKeyDown(Keys.LeftAlt) && ks.IsKeyDown(Keys.O))
                            //{
                            //    Events.RegisterEvent(Events.EventName.Toggle_OptionsWindow);
                            //    result = true;
                            //}
                            #endregion
                            #endregion

                            #region ALT + N  News Window
                            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.N))
                            {
                                if (GuiManager.GenericSheet["NewsWindow"] is gui.MessageWindow newsWindow)
                                {
                                    newsWindow.IsVisible = !newsWindow.IsVisible;
                                    result = true;
                                }
                            }
                            #endregion

                            #region PrintScreen Saves a Screenshot
                            if (ks.IsKeyDown(Keys.PrintScreen))
                            {
                                Utils.SaveScreenshot();
                            }
                            #endregion

                            #region ALT + Enter  Full Screen Toggle
                            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.Enter) && Utility.Settings.StaticSettings.FullScreenToggleEnabled)
                            {
                                Events.RegisterEvent(Events.EventName.Toggle_FullScreen);
                                result = true;
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

                                TextCue.AddClientInfoTextCue("Reloaded " + GuiManager.CurrentSheet.Description + " and Generic Sheet.", TextCue.TextCueTag.None, Color.LimeGreen, Color.Transparent, 0, 2000, false, true, false);

                                result = true;
                            }
                            #endregion

                            if (ks.IsKeyDown(Keys.Tab) || (GuiManager.ControlWithFocus is TextBox && ks.IsKeyDown(Keys.Enter) && ks.GetPressedKeys().Length == 1))
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
                            //if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.C))
                            //{
                            //    Events.RegisterEvent(Events.EventName.Set_Game_State, Enums.EGameState.Login);
                            //}

                            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.Enter))
                            {
                                Events.RegisterEvent(Events.EventName.Toggle_FullScreen);
                                result = true;
                            }

                            //#region ALT + M  Macros Window
                            //if (ks.IsKeyDown(Keys.LeftAlt) && ks.IsKeyDown(Keys.M))
                            //{
                            //    Events.RegisterEvent(Events.EventName.Toggle_Macros);
                            //    result = true;
                            //}
                            //#endregion

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

                                TextCue.AddClientInfoTextCue("Reloaded " + GuiManager.CurrentSheet.Description + " and Generic Sheet.", TextCue.TextCueTag.None, Color.LimeGreen, Color.Transparent, 0, 2500, false, true, false);

                                result = true;

                                Character.LoadSettings();
                                GenericSheet.LoadMacros();
                            }
                            #endregion
                        }
                        else if (Client.GameState == Enums.EGameState.HotButtonEditMode)
                        {
                            // nothing
                            if (ks.IsKeyDown(Keys.Escape))
                            {
                                if (GuiManager.GetControl("HotButtonEditWindow") is Window hbeWindow)
                                    hbeWindow.OnClose();
                                result = true;
                            }

                            #region Tilde Saves a Screenshot
                            if (ks.IsKeyDown(Keys.PrintScreen))
                            {
                                Utils.SaveScreenshot();
                                result = true;
                            }
                            #endregion
                        }
                        else // Menu, Game, Conference
                        {
                            // ALT + W Toggle Fog of War
                            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.W))
                            {
                                Events.RegisterEvent(Events.EventName.Toggle_FogOfWar);
                                result = true;
                            }

                            // ALT + T Toggle Talents Window
                            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.T))
                            {
                                Events.RegisterEvent(Events.EventName.Toggle_Talents);
                                result = true;
                            }

                            if (ks.IsKeyDown(Keys.LeftAlt) && ks.IsKeyDown(Keys.F))
                            {
                                TextCue.AddFPSTextCue(Math.Round(1 / Program.Client.ClientGameTime.ElapsedGameTime.TotalSeconds, 1).ToString());
                                result = true;
                            }

                            // Escape closes gridboxwindows if there is no target. Otherwise, target is cleared first.
                            if (ks.IsKeyDown(Keys.Escape))
                            {
                                if (!result && GuiManager.GenericSheet["LogoutOptionWindow"] is Window loWindow)
                                {
                                    loWindow.OnClose();
                                    result = true;
                                }

                                // Close DropDownMenu
                                if (GuiManager.ActiveDropDownMenu != null)
                                {
                                    GuiManager.Dispose(GuiManager.ActiveDropDownMenu);
                                    result = true;
                                }

                                // Close spellbook. Close all GridBoxWindows. (target should always be cleared if GAMEINPUTTEXTBOX has focus)
                                if (GuiManager.Cursors[GuiManager.GenericSheet.Cursor] is gui.MouseCursor cursor && cursor.DraggedControl != null)
                                {
                                    if (cursor.DraggedControl is DragAndDropButton dadButton)
                                        dadButton.StopDragging();
                                    else
                                    {
                                        GuiManager.Dispose(cursor.DraggedControl);
                                        cursor.DraggedControl = null;
                                    }
                                    result = true;
                                }

                                // Clear pathing choices on SpinelTileLabels.
                                if (!result && (Client.GameState == Enums.EGameState.SpinelGame || Client.GameState == Enums.EGameState.YuushaGame) && GameHUD.MovementChoices.Count > 0)
                                {
                                    foreach (Cell cell in GameHUD.MovementChoices)
                                        if (GuiManager.CurrentSheet["Tile" + GameHUD.Cells.IndexOf(cell)] is SpinelTileLabel spLabel) spLabel.PathingVisual = "";

                                    GameHUD.MovementChoices.Clear();
                                    result = true;
                                }

                                if (!result && GuiManager.GetControl("SpellbookWindow") is SpellBookWindow spellbookWindow && spellbookWindow.IsVisible)
                                {
                                    spellbookWindow.OnClose();
                                    result = true;
                                }

                                if (!result && Client.InGame && GameHUD.CurrentTarget != null)
                                {
                                    Events.RegisterEvent(Events.EventName.Target_Cleared);
                                    result = true;
                                }

                                if (!Client.InGame && GuiManager.CurrentSheet[Globals.CONFINPUTTEXTBOX] is TextBox confInputTbx)
                                {
                                    if (confInputTbx.Text.Length > 0)
                                    {
                                        confInputTbx.Clear();
                                        result = true;
                                    }
                                }
                                else if(Client.InGame && GuiManager.CurrentSheet[Globals.GAMEINPUTTEXTBOX] is TextBox gameInputTbx)
                                {
                                    if (gameInputTbx.Text.Length > 0)
                                    {
                                        gameInputTbx.Clear();
                                        result = true;
                                    }
                                }

                                if (!result)
                                {
                                    result = GuiManager.CloseAllGridBoxes();
                                }

                                if(!result && (!(GuiManager.GenericSheet["LogoutOptionWindow"] is Window w) || !w.IsVisible))
                                {
                                    GameHUD.DisplayLogoutOptionScreen();
                                    result = true;
                                }
                            }

                            if (ks.IsKeyDown(Keys.Tab) || (GuiManager.ControlWithFocus is TextBox && ks.IsKeyDown(Keys.Enter) && ks.GetPressedKeys().Length == 1))
                            {
                                if (!ks.IsKeyDown(Keys.LeftAlt) && !ks.IsKeyDown(Keys.RightAlt))
                                {
                                    if (!ks.IsKeyDown(Keys.LeftShift) && !ks.IsKeyDown(Keys.RightShift))
                                        GuiManager.CurrentSheet.HandleTabOrderForward();
                                    else GuiManager.CurrentSheet.HandleTabOrderReverse();

                                    result = true;
                                }
                            }

                            #region ALT + Enter  Full Screen Toggle
                            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.Enter) && Utility.Settings.StaticSettings.FullScreenToggleEnabled)
                            {
                                if (GuiManager.CurrentSheet.AllowFullScreen)
                                {
                                    Events.RegisterEvent(Events.EventName.Toggle_FullScreen);
                                    result = true;
                                }
                                else TextCue.AddClientInfoTextCue("Fullscreen disabled in " + Client.GameState + ".");
                            }
                            #endregion

                            #region ALT + G  Toggle Game Mode
                            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.G))
                            {
                                if (Client.GameDisplayMode == Enums.EGameDisplayMode.IOK)
                                    Events.RegisterEvent(Events.EventName.Set_Client_Mode, Enums.EGameDisplayMode.Spinel);
                                else if (Client.GameDisplayMode == Enums.EGameDisplayMode.Spinel)
                                    Events.RegisterEvent(Events.EventName.Set_Client_Mode, Enums.EGameDisplayMode.Yuusha);
                                else if (Client.GameDisplayMode == Enums.EGameDisplayMode.Yuusha)
                                    Events.RegisterEvent(Events.EventName.Set_Client_Mode, Enums.EGameDisplayMode.IOK);

                                TextCue.AddClientInfoTextCue(Client.GameDisplayMode.ToString() + " Mode", TextCue.TextCueTag.None, Color.Lime, Color.Transparent, 0, 2700, false, true, false);

                                result = true;
                            }
                            #endregion

                            #region ALT + H  Help Window
                            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.H))
                            {
                                if (GuiManager.GenericSheet["HelpWindow"] is Window helpWindow)
                                {
                                    if (!helpWindow.IsVisible)
                                    {
                                        (helpWindow["HelpScrollableTextBox"] as ScrollableTextBox).Clear();

                                        try
                                        {
                                            System.IO.StreamReader sr = new System.IO.StreamReader(Utils.GetMediaFile("help.txt"));

                                            while (!sr.EndOfStream)
                                                (helpWindow["HelpScrollableTextBox"] as ScrollableTextBox).AddLine(sr.ReadLine(), Enums.ETextType.Default);
                                            sr.Close();
                                        }
                                        catch (System.IO.FileNotFoundException)
                                        {
                                            (helpWindow["HelpScrollableTextBox"] as ScrollableTextBox).AddLine("The help.txt file is missing from the media subdirectory.", Enums.ETextType.Default);
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
                                Events.RegisterEvent(Events.EventName.Toggle_VerticalHotbar);
                                result = true;
                            }
                            #endregion

                            #region ALT + K  Horizontal Hot Button Window
                            if (ks.IsKeyDown(Keys.LeftAlt) && ks.IsKeyDown(Keys.K))
                            {
                                Events.RegisterEvent(Events.EventName.Toggle_HorizontalHotbar);
                                result = true;
                            }
                            #endregion

                            #region ALT + L  Toggle Logging of Scrollable Text Boxes
                            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.L))
                            {
                                GuiManager.LoggingRequested = !GuiManager.LoggingRequested;
                                string onoff = "Enabled";
                                if (!GuiManager.LoggingRequested) onoff = "Disabled";
                                gui.TextCue.AddClientInfoTextCue("Logging " + onoff, TextCue.TextCueTag.None, Color.Red, Color.Transparent, 0, 2500, false, true, false);
                                result = true;
                            }
                            #endregion

                            #region ALT + M  Macros Window
                            if (ks.IsKeyDown(Keys.LeftAlt) && ks.IsKeyDown(Keys.M))
                            {
                                Events.RegisterEvent(Events.EventName.Toggle_Macros);
                                result = true;
                            }
                            #endregion

                            #region ALT + N  News Window
                            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.N))
                            {
                                if (GuiManager.GenericSheet["NewsWindow"] is gui.MessageWindow newsWindow)
                                {
                                    newsWindow.OnClose();
                                }
                                else MessageWindow.CreateNewsMessageWindow(World.News);
                            }
                            #endregion

                            #region ALT + O  Options Window
                            if (ks.IsKeyDown(Keys.LeftAlt) && ks.IsKeyDown(Keys.O))
                            {
                                Events.RegisterEvent(Events.EventName.Toggle_OptionsWindow);
                                result = true;
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

                                TextCue.AddClientInfoTextCue("Reloaded " + gui.GuiManager.CurrentSheet.Description + " and Generic Sheet.", gui.TextCue.TextCueTag.None, Color.LimeGreen, Color.Transparent, 0, 2000, false, true, false);

                                result = true;

                                if (Client.InGame)
                                    IO.Send("redraw");

                                Character.LoadSettings();
                                GenericSheet.LoadMacros();
                                Character.GUIPositionSettings.OnLoad();
                            }
                            #endregion

                            #region ALT + U  Reload Game Tiles (Mode Dependent)
                            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.U))
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
                                Client.UserSettings.AudioEnabled = !Client.UserSettings.AudioEnabled;

                                string onoff = "Enabled";
                                if (!Client.UserSettings.SoundEffects) onoff = "Disabled";
                                TextCue.AddClientInfoTextCue("Audio " + onoff, TextCue.TextCueTag.None, Color.Red, Color.Transparent, 0, 2500, false, true, false);
                                result = true;
                            }
                            #endregion

                            #region ALT + A  Toggle Ambient Sounds
                            if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.A))
                            {
                                Client.UserSettings.BackgroundAmbience = !Client.UserSettings.BackgroundAmbience;
                                string onoff = "Enabled";
                                if (!Client.UserSettings.BackgroundAmbience) onoff = "Disabled";
                                TextCue.AddClientInfoTextCue("Ambient Sounds " + onoff, TextCue.TextCueTag.None, Color.Red, Color.Transparent, 0, 2500, false, true, false);
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
                            // Only in YuushaMode?
                            if (Client.GameState == Enums.EGameState.YuushaGame)
                            {
                                if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.Add))
                                {
                                    Events.RegisterEvent(Events.EventName.MapDisplay_Increase);
                                    result = true;
                                }

                                if (IsAltKeyDown(ks) && ks.IsKeyDown(Keys.Subtract))
                                {
                                    Events.RegisterEvent(Events.EventName.MapDisplay_Decrease);
                                    result = true;
                                }
                            }

                            #region Tilde Saves a Screenshot
                            if (ks.IsKeyDown(Keys.PrintScreen))
                            {
                                Utils.SaveScreenshot();
                                result = true;
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

        public static bool IsCtrlKeyDown(KeyboardState ks)
        {
            return ks.IsKeyDown(Keys.LeftControl) || ks.IsKeyDown(Keys.RightControl);
        }

        public static bool IsShiftKeyDown(KeyboardState ks)
        {
            return ks.IsKeyDown(Keys.LeftShift) || ks.IsKeyDown(Keys.RightShift);
        }
    }
}