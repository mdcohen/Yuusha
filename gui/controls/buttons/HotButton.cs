using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class HotButton : Button
    {
        public int FadeIncrement = 10;
        public double FadeDelay = .035;
        readonly int m_originalVisualAlpha = 255;
        private SquareBorder m_border;
        
        public SquareBorder Border { get { return m_border; } set { m_border = value; } }

        public HotButton(string name, string owner, Rectangle rectangle, string text, bool textVisible,
            Color textColor, bool visible, bool disabled, string font, VisualKey visualKey, Color tintColor,
            byte visualAlpha, byte m_borderAlpha, byte textAlpha, VisualKey visualKeyOver, VisualKey visualKeyDown,
            VisualKey visualKeyDisabled, string onMouseDownEvent, BitmapFont.TextAlignment textAlignment,
            int xTextOffset, int yTextOffset, Color textOverColor, bool hasTextOverColor, Color tintOverColor, bool hasTintOverColor,
            List<Enums.EAnchorType> anchors, bool dropShadow, Map.Direction shadowDirection, int shadowDistance, string command, string popUpText)
            : base(name, owner, rectangle, text, textVisible, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha, m_borderAlpha, textAlpha, visualKeyOver, visualKeyDown, visualKeyDisabled, onMouseDownEvent, textAlignment, xTextOffset, yTextOffset, textOverColor, hasTextOverColor, tintOverColor, hasTintOverColor, anchors, dropShadow, shadowDirection, shadowDistance, command, popUpText)
        {
            m_originalVisualAlpha = visualAlpha;
        }

        protected override bool OnKeyDown(KeyboardState ks)
        {
            if (this.m_containsMousePointer)
            {
                bool controlDown = ks.IsKeyDown(Keys.LeftControl) || ks.IsKeyDown(Keys.RightControl);
                bool altDown = ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt);
                MouseState ms = GuiManager.MouseState;

                if ((controlDown || altDown) && ms.LeftButton != ButtonState.Pressed)
                {
                    if(this.Text.Length > 0)
                        TextCue.AddMouseCursorTextCue(this.Text, Client.ClientSettings.HotButtonText_ForeColor, Client.ClientSettings.HotButtonText_BackColor, this.Font);
                    return true;
                }
                else if(controlDown && altDown && ms.LeftButton == ButtonState.Pressed)
                {
                    this.Text = ""; // Hmm. This is how we bypass drawing a cleared HotButton.

                    TextCue.ClearMouseCursorTextCue();

                    TextCue.AddClientInfoTextCue("Hot Button Cleared", Color.LimeGreen, Color.Black, 1000);

                    Events.RegisterEvent(Events.EventName.Save_Character_Settings);

                    return true;
                }
            }

            return base.OnKeyDown(ks);
        }

        protected override void OnMouseOver(MouseState ms)
        {
            base.OnMouseOver(ms);

            if (Border == null)
                GuiManager.GenericSheet.CreateSquareBorder(Name + "SquareBorder", Name, 1, new VisualKey("WhiteSpace"), false, Color.OldLace, 255);

            this.m_borderAlpha = 255;

            if (Border != null)
                Border.IsVisible = true;
            
        }

        protected override void OnMouseLeave(MouseState ms)
        {
            base.OnMouseLeave(ms);

            if (Border != null)
                Border.IsVisible = false;

            this.m_borderAlpha = 0;

            TextCue.ClearMouseCursorTextCue();
        }

        protected override void OnMouseDown(MouseState ms)
        {
            base.OnMouseDown(ms);

            if (ms.LeftButton == ButtonState.Pressed)
            {
                if (this.Text.Length <= 0)
                    return;

                m_visualAlpha = 40;
            }
            else if(ms.RightButton == ButtonState.Pressed)
            {
                if(!Client.IsFullScreen)
                {
                    TextCue.AddClientInfoTextCue("Hot Button Edit Mode requires full screen display.",
                        TextCue.TextCueTag.None, Color.Red, Color.Black, 2000, false, false, true);
                    return;
                }

                try
                {
                    Sheet sheet = GuiManager.Sheets["HotButtonEditMode"];

                    if (sheet != null)
                    {
                        if (sheet["HotButtonEditWindow"] is HotButtonEditWindow window)
                        {
                            // Register game state change event.
                            Events.RegisterEvent(Events.EventName.Set_Game_State, Enums.EGameState.HotButtonEditMode);
                            // Set edit mode variables.
                            window.SelectedHotButton = this.Name;

                            try
                            {
                                window.IconImagePrefix = this.VisualKey.Substring(0, this.VisualKey.IndexOf("_"));
                            }
                            catch
                            {
                                window.IconImagePrefix = "hotbutton";
                            }

                            window.SelectedIconLabel = window["HotButtonEditWindowSelectedIconLabel"] as Label;
                            window.SelectedIconLabel.VisualKey = this.VisualKey;
                            if (window.SelectedIconLabel.VisualKey == "")
                                window.SelectedIconLabel.VisualKey = "WhiteSpace";
                            window.SelectedVisualKey = this.VisualKey;
                            if (window.SelectedVisualKey == "")
                                window.SelectedVisualKey = "WhiteSpace";
                            window.OriginatingWindow = this.Owner;

                            // Set textbox text.
                            window["HotButtonEditWindowTextBox"].Text = this.Text;
                            (window["HotButtonEditWindowTextBox"] as TextBox).SelectAll();
                            (window["HotButtonEditWindowTextBox"] as TextBox).DeselectText();

                            window["HotButtonEditWindowCommandTextBox"].Text = this.Command;

                            // Create labels for choosing a macro icon.
                            window.CreateIconSelectionButtons();

                            window.ForceMaximize(); // fill the screen

                            // macros and hotkeys are part of the generic sheet, always visible -- hide them when in edit mode
                            //(GuiManager.CurrentSheet[this.Owner] as Window).IsVisible = false;

                            window.IsVisible = true;
                        }
                    }
                }
                catch(Exception e)
                {
                    Utils.LogException(e);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (m_containsMousePointer && Border != null)
                Border.IsVisible = true;
            else if (!m_containsMousePointer && Border != null)
                Border.IsVisible = false;

            if (m_visualAlpha < m_originalVisualAlpha)
            {
                //Decrement the delay by the number of seconds that have elapsed since
                //the last time that the Update method was called
                FadeDelay -= gameTime.ElapsedGameTime.TotalSeconds;

                //If the Fade delays has dropped below zero, then it is time to 
                //fade in/fade out the image a little bit more.
                if (FadeDelay <= 0)
                {
                    //Reset the Fade delay
                    FadeDelay = .035;

                    //Increment/Decrement the fade value for the image
                    m_visualAlpha += FadeIncrement;

                    //If the AlphaValue is equal or above the max Alpha value or
                    //has dropped below or equal to the min Alpha value, then 
                    //reverse the fade
                    if (m_visualAlpha >= 255 || m_visualAlpha > m_originalVisualAlpha)// || m_visualAlpha <= 0)
                    {
                        m_visualAlpha = m_originalVisualAlpha;
                        //mFadeIncrement *= -1;
                    }
                }
            }

            base.Update(gameTime);

            if (Border != null) Border.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!this.IsVisible)
                return;

            if(this.Text == "" || (this.VisualKey == "WhiteSpace" && GuiManager.GetControl(this.Owner) is HotButtonEditWindow))
            {
                if (Border != null && Border.IsVisible) Border.Draw(gameTime);
                return;
            }

            VisualInfo vi = GuiManager.Visuals[m_visualKey.Key];

            Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], this.m_rectangle, vi.Rectangle,
                new Color(m_tintColor.R, m_tintColor.G, m_tintColor.B, (byte)MathHelper.Clamp(m_visualAlpha, 0, 255)));

            if (Border != null) Border.Draw(gameTime);
        }
    }
}
