﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class HotButtonEditWindow : Window
    {
        public string OriginatingWindow; // which window opened up HotButtonEditMode

        public string SelectedHotButton; // The name of the button being edited
        public Label SelectedIconLabel;
        public string SelectedVisualKey;
        public string IconImagePrefix = "";
        private bool IconSelectionButtonTextVisible = false;

        public HotButtonEditWindow(string name, string owner, Rectangle rectangle, bool visible, bool locked, bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, bool dropShadow, Map.Direction shadowDirection, int shadowDistance, List<Enums.EAnchorType> anchors, string cursorOverride) : base(name, owner, rectangle, visible, locked, disabled, font, visualKey, tintColor, visualAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride)
        {
        }

        public override void OnClose()
        {
            base.OnClose();

            Events.RegisterEvent(Events.EventName.Set_Game_State, GameHUD.PreviousGameState);

            // Make the Icons Window visible again. It was right clicked to reach this HotButtonEditWindow.

            if (GuiManager.GetControl(OriginatingWindow) is Window iconWindow)
                iconWindow.IsVisible = true;
        }

        protected override bool OnKeyDown(KeyboardState ks)
        {
            if (ks.IsKeyDown(Keys.LeftControl) && ks.IsKeyDown(Keys.V))
            {
                IconSelectionButtonTextVisible = true;
                return true;
            }

            return base.OnKeyDown(ks);
        }

        public void CreateIconSelectionButtons()
        {
            foreach (Control c in new System.Collections.Concurrent.ConcurrentBag<Control>(Controls))
            {
                if (c is IconImageSelectionButton)
                {
                    Controls.Remove(c);
                }
            }

            try
            {
                List<string> IconVisualKeys = new List<string>();
                int x = 6;
                int y = 180;
                int width = 48; // 34
                int height = 48; // 34
                int padding = 3;

                if (!Client.IsFullScreen)
                {
                    width = 31;
                    height = 31;
                    padding = 1;
                }

                foreach (string visualName in GuiManager.Visuals.Keys)
                {
                    if (IconImagePrefix != "" && visualName.StartsWith(IconImagePrefix))
                        IconVisualKeys.Add(visualName);
                }

                int columnCount = 0;
                int rowCount = 0;
                int a = 0;

                VisualKey emptyKey = new VisualKey("");

                for (a = 0; a < IconVisualKeys.Count; a++)
                {
                    GuiManager.CurrentSheet.CreateButton("IconImageSelectionButton", IconImagePrefix + "_" + a, Name, new Rectangle(x, y, width, height), a.ToString(), false, Color.White, true, false, GuiManager.Sheets[Sheet].Font, new VisualKey(IconVisualKeys[a]), Color.White,
                        255, 255, emptyKey, emptyKey, emptyKey, "", BitmapFont.TextAlignment.Right, 0, height - (BitmapFont.ActiveFonts[GuiManager.Sheets[Sheet].Font].LineHeight + 2), Color.White, false, Color.White, false, new List<Enums.EAnchorType>(),
                        false, Map.Direction.Northwest, 2, "", "", "", "", false, Client.ClientSettings.DefaultOnClickSound);

                    GuiManager.CurrentSheet.CreateSquareBorder(IconImagePrefix + "_" + a + "SquareBorder", IconImagePrefix + "_" + a, 2, new gui.VisualKey("WhiteSpace"), false, Color.White, 255);

                    columnCount++;
                    x += width + padding;

                    if (columnCount == 31)
                    {
                        columnCount = 0;
                        rowCount++;
                        x = 6;
                        y += height + padding;
                    }
                }

                //IconSelectionButtonsCreated = true;
            }
            catch(Exception e)
            { Utils.LogException(e); }
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Control control in new List<Control>(m_controls))
            {
                //if (!m_cropped || (control is WindowControlBox) || (control is WindowTitle))
                //    control.IsDisabled = m_disabled; // disabled

                control.Update(gameTime);
            }

            if (IconSelectionButtonTextVisible)
            {
                foreach(Control c in Controls)
                {
                    if(c is IconImageSelectionButton b)
                    {
                        b.IsTextVisible = true;

                        string effectUsed = "";
                        foreach(string effectName in Effect.IconsDictionary.Keys)
                        {
                            if(Effect.IconsDictionary[effectName] == b.VisualKey)
                            {
                                effectUsed = effectName;
                                break;
                            }
                        }

                        if (!string.IsNullOrEmpty(effectUsed))
                        {
                            if (b.Border != null)
                            {
                                b.Border.TintColor = Color.Red;
                                b.Border.IsVisible = true;
                            }
                            b.PopUpText = effectUsed;
                        }
                        else
                        {
                            if (b.Border != null)
                            {
                                b.Border.TintColor = Color.Green;
                                b.Border.IsVisible = true;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (Control c in Controls)
                {
                    if (c is IconImageSelectionButton b)
                    {
                        b.IsTextVisible = false;
                        if (b.Border != null)
                        {
                            b.Border.TintColor = Color.White;
                            if (!b.Contains(GuiManager.MouseState.Position))
                                b.Border.IsVisible = false;
                        }
                    }
                }
            }
        }
    }
}
