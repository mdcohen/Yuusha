using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class TextCue : GameComponent
    {
        public enum TextCueTag { None, PromptState, WarmedSpell, XPGain, XPLoss, HealthGain, HealthLoss, StaminaGain, StaminaLoss, ManaGain, ManaLoss }

        #region Private Data
        private string m_text;
        private int m_x;
        private int m_y;
        private string m_font;
        private byte m_alpha;
        private Color m_color;
        private Color m_backgroundColor;
        private byte m_backgroundAlpha;
        private double m_lifeCycle;
        private TimeSpan m_lifeStart;
        private bool m_lifeStarted;
        private bool m_dropShadow;
        private int m_shadowDistance;
        private Map.Direction m_shadowDirection;
        private bool m_centered;
        private bool m_fadeIn;
        private bool m_fadeOut;
        private TextCueTag m_tag;
        #endregion

        #region Public Properties
        public string Text
        {
            get { return m_text; }
        }
        public int X
        {
            get { return m_x; }
            set { m_x = value; }
        }
        public int Y
        {
            get { return m_y; }
            set { m_y = value; }
        }
        public string Font
        {
            get { return m_font; }
        }
        public bool IsCentered
        {
            get { return m_centered; }
        }
        public TextCueTag Tag
        {
            get { return m_tag; }
            set { m_tag = value; }
        }
        #endregion

        public TextCue(string text, int x, int y, byte alpha, Color color, Color backgroundColor, byte backgroundAlpha, string font, double lifeCycle,
            bool dropShadow, int shadowDistance, Map.Direction shadowDirection, bool centered, bool fadeIn, bool fadeOut, TextCueTag tag) : base (Program.Client)
        {
            m_text = text;
            m_x = x;
            m_y = y;
            m_alpha = alpha;
            m_color = new Color(color.R, color.G, color.B, color.A);
            m_backgroundColor = new Color(backgroundColor.R, backgroundColor.G, backgroundColor.B, backgroundColor.A);
            m_backgroundAlpha = backgroundAlpha;
            m_font = font;
            m_lifeCycle = lifeCycle;
            m_dropShadow = dropShadow;
            m_shadowDistance = shadowDistance;
            m_shadowDirection = shadowDirection;
            m_centered = centered;
            m_fadeIn = fadeIn;
            m_fadeOut = fadeOut;
            m_tag = tag;
        }

        public void Update(GameTime gameTime, List<TextCue> cueList)
        {
            switch(Tag)
            {
                case TextCueTag.XPGain:
                case TextCueTag.XPLoss:
                    X += 1;
                    Y -= 1;
                    break;
                case TextCueTag.HealthGain:
                    Y -= 2;
                    break;
                case TextCueTag.HealthLoss:
                    Y -= 1;
                    break;
                case TextCueTag.ManaGain:
                case TextCueTag.StaminaGain:
                    X -= 2;
                    break;
                case TextCueTag.ManaLoss:
                case TextCueTag.StaminaLoss:
                    X += 2;
                    break;
                default:
                    break;
            }

            if (!m_lifeStarted)
            {
                m_lifeStart = gameTime.TotalGameTime;
                m_lifeStarted = true;
            }
            else if (m_lifeCycle > 0 && gameTime.TotalGameTime - TimeSpan.FromMilliseconds(m_lifeCycle) > m_lifeStart) // lifeCycle of 0 or less means the TextCue stays until forcibly removed
            {
                cueList.Remove(this);
            }
            else
            {
                byte alphaPlus = Client.ClientSettings.DefaultPopUpFadeInSpeed;
                byte alphaMinus = Client.ClientSettings.DefaultPopUpFadeOutSpeed;
                switch(Tag)
                {
                    case TextCueTag.XPGain:
                        alphaMinus++;
                        break;
                }

                if (m_alpha < 255 && m_fadeIn && !m_fadeOut)
                {
                    m_alpha += alphaPlus;
                    if(m_alpha >= 255)
                        cueList.Remove(this);
                }

                if (m_alpha > 0 && !m_fadeIn && m_fadeOut)
                {
                    m_alpha -= alphaMinus;
                    if(m_alpha <= 20)
                        cueList.Remove(this);
                }

                if (m_fadeIn && m_fadeOut)
                {
                    m_alpha += alphaPlus;

                    if (m_alpha >= 255)
                    {
                        m_alpha = 255;
                        m_fadeIn = false;
                        m_fadeOut = true;
                    }
                }
            }

            if (cueList.Contains(this))
            {
                m_color = new Color(m_color.R, m_color.G, m_color.B, m_alpha);

                if (m_backgroundColor != null && m_backgroundColor != Color.Transparent)
                    m_backgroundColor = new Color(m_backgroundColor.R, m_backgroundColor.G, m_backgroundColor.B, m_alpha);
            }
            else GuiManager.TextCues.Remove(this);
           
        }

        public void Draw(GameTime gameTime)
        {
            BitmapFont.ActiveFonts[m_font].SpriteBatchOverride(Client.SpriteBatch);

            // if the TextCue has a background color, draw the rectangle using WhiteSpace visual
            if (m_backgroundColor != null && m_backgroundColor != Color.Transparent)
            {
                VisualInfo vi = GuiManager.Visuals["WhiteSpace"];
                Rectangle rect = new Rectangle(m_x, m_y, BitmapFont.ActiveFonts[m_font].MeasureString(m_text), BitmapFont.ActiveFonts[m_font].LineHeight);
                Color backgroundColor = new Color(m_backgroundColor, m_backgroundAlpha);
                Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], rect, vi.Rectangle, backgroundColor);
            }

            if (m_dropShadow)
            {
                Color shadowColor = new Color((int)Color.Black.R, (int)Color.Black.G, (int)Color.Black.B, 50);
                BitmapFont.ActiveFonts[m_font].DrawString(X + GetXShadow(), Y + GetYShadow(), shadowColor, m_text);
            }

            BitmapFont.ActiveFonts[m_font].DrawString(m_x, m_y, m_color, m_text);
        }

        public int GetXShadow()
        {
            switch (m_shadowDirection)
            {
                case Map.Direction.East:
                    return m_shadowDistance;
                case Map.Direction.North:
                    return 0;
                case Map.Direction.Northeast:
                    return m_shadowDistance;
                case Map.Direction.Northwest:
                    return -(m_shadowDistance);
                case Map.Direction.South:
                    return 0;
                case Map.Direction.Southeast:
                    return m_shadowDistance;
                case Map.Direction.Southwest:
                    return -(m_shadowDistance);
                case Map.Direction.West:
                    return -(m_shadowDistance);
                default:
                    return 0;
            }
        }

        public int GetYShadow()
        {
            switch (m_shadowDirection)
            {
                case Map.Direction.East:
                    return 0;
                case Map.Direction.North:
                    return -(m_shadowDistance);
                case Map.Direction.Northeast:
                    return -(m_shadowDistance);
                case Map.Direction.Northwest:
                    return -(m_shadowDistance);
                case Map.Direction.South:
                    return m_shadowDistance;
                case Map.Direction.Southeast:
                    return m_shadowDistance;
                case Map.Direction.Southwest:
                    return m_shadowDistance;
                case Map.Direction.West:
                    return 0;
                default:
                    return 0;
            }
        }

        public static void AddMouseCursorTextCue(string text)
        {
            AddMouseCursorTextCue(text, Client.ClientSettings.DefaultMouseCursorTextCueColor, GuiManager.CurrentSheet.Font);
        }

        public static void AddMouseCursorTextCue(string text, Color color, string font)
        { 
            MouseState ms = GuiManager.MouseState;

            MouseCursor cursor;

            if(GuiManager.CurrentSheet.CursorOverride != "")
                cursor = GuiManager.Cursors[GuiManager.CurrentSheet.CursorOverride];
            else cursor = GuiManager.Cursors[GuiManager.CurrentSheet.Cursor];

            if (cursor != null)
            {
                TextCue tc = new TextCue(text, ms.X, ms.Y - BitmapFont.ActiveFonts[font].LineHeight, 255, color, Color.Transparent, 0, font, 2500, false, 2, Map.Direction.Southeast, false, false, false, TextCueTag.None);

                cursor.TextCues.Clear();

                cursor.TextCues.Add(tc);
            }
        }

        public static void AddMouseCursorTextCue(string text, Color forecolor, Color backcolor, byte backgroundAlpha, string font)
        {
            MouseState ms = GuiManager.MouseState;

            MouseCursor cursor;

            if (GuiManager.CurrentSheet.CursorOverride != "")
                cursor = GuiManager.Cursors[GuiManager.CurrentSheet.CursorOverride];
            else cursor = GuiManager.Cursors[GuiManager.CurrentSheet.Cursor];

            if (cursor != null)
            {
                TextCue tc = new TextCue(text, ms.X, ms.Y - BitmapFont.ActiveFonts[font].LineHeight, 255, forecolor, backcolor, backgroundAlpha, font, 2500, false, 2, Map.Direction.Southeast, false, false, true, TextCueTag.None);

                cursor.TextCues.Clear();

                cursor.TextCues.Add(tc);
            }
        }

        public static void ClearMouseCursorTextCue()
        {
            MouseCursor cursor;

            if (GuiManager.CurrentSheet.CursorOverride != "")
                cursor = GuiManager.Cursors[GuiManager.CurrentSheet.CursorOverride];
            else cursor = GuiManager.Cursors[GuiManager.CurrentSheet.Cursor];

            if (cursor != null)
                cursor.TextCues.Clear();
        }

        public static void RemoveMouseCursorTextCue(string text)
        {
            MouseCursor cursor;

            if (GuiManager.CurrentSheet.CursorOverride != "")
                cursor = GuiManager.Cursors[GuiManager.CurrentSheet.CursorOverride];
            else cursor = GuiManager.Cursors[GuiManager.CurrentSheet.Cursor];

            if (cursor != null)
            {
                foreach(TextCue tc in new List<TextCue>(cursor.TextCues))
                {
                    if (tc.Text == text)
                        cursor.TextCues.Remove(tc);
                }
            }
        }

        #region Client Info TextCues
        public static void AddClientInfoTextCue(string text)
        {
            AddClientInfoTextCue(text, TextCueTag.None, Color.Yellow, Color.Black, 255, 3500, false, false, true);
        }

        public static void AddClientInfoTextCue(string text, double lifeCycle)
        {
            AddClientInfoTextCue(text, TextCueTag.None, Color.Yellow, Color.Black, 255, lifeCycle, false, false, true);
        }

        public static void AddClientInfoTextCue(string text, Color foreColor, Color backgroundColor, double lifeCycle)
        {
            AddClientInfoTextCue(text, TextCueTag.None, foreColor, backgroundColor, 255, lifeCycle, false, false, true);
        }

        public static void AddClientInfoTextCue(string text, TextCueTag tag, Color color, Color backgroundColor, byte backgroundAlpha, double lifeCycle, bool fadeIn, bool fadeOut, bool addOnce)
        {
            if (addOnce)
            {
                foreach (TextCue cue in GuiManager.TextCues)
                {
                    if (cue.Text == text)
                        return;
                }
            }

            int x = 0;
            int y = 0;
            bool centered = false;

            switch (Client.GameState)
            {
                //case Enums.EGameState.Game:
                case Enums.EGameState.IOKGame:
                //case Enums.EGameState.LOKGame:
                //case Enums.EGameState.YuushaGame:
                case Enums.EGameState.SpinelGame:
                    x = 10;
                    y = Client.Height - Convert.ToInt32(BitmapFont.ActiveFonts[GuiManager.CurrentSheet.Font].LineHeight * 1.25);
                    break;
                default:
                    centered = true;
                    break;
            }

            TextCue tc = new TextCue(text, x, y, (!fadeIn ? (byte)255 : (byte)40), color, backgroundColor, backgroundAlpha, GuiManager.CurrentSheet.Font, lifeCycle,
                true, 2, Map.Direction.None, centered, fadeIn, fadeOut, tag);

            if (fadeIn) tc.m_alpha = 40;

            // disable multiple text cues for the time being
            //if (GuiManager.TextCues.Count >= 50)
            //    GuiManager.TextCues.RemoveAt(0);

            // only one text cure allowed?? this should be changed
            if (!centered)
                GuiManager.TextCues.Clear();

            if(!GuiManager.TextCues.Contains(tc))
                GuiManager.TextCues.Add(tc);
        }
        #endregion

        public static void AddPromptStateTextCue(Protocol.PromptStates promptState)
        {
            Color backgroundColor = Color.Transparent;
            Color cueColor = Color.IndianRed;
            string promptText = "No text.";

            switch (promptState)
            {
                case Protocol.PromptStates.Resting:
                case Protocol.PromptStates.Meditating:
                    cueColor = Color.LightGreen;
                    backgroundColor = Color.DarkSlateGray;
                    promptText = "You are " + promptState.ToString().ToLower() + ".";
                    break;
                case Protocol.PromptStates.Blind:
                    promptText = "You are blind!";
                    backgroundColor = Color.Maroon;
                    break;
                case Protocol.PromptStates.Feared:
                    promptText = "You are scared!";
                    backgroundColor = Color.Maroon;
                    break;
                case Protocol.PromptStates.Stunned:
                default:
                    cueColor = Color.White;
                    promptText = "You are stunned!";
                    backgroundColor = Color.Maroon;
                    break;
            }

            AddClientInfoTextCue(promptText, TextCueTag.PromptState, cueColor, backgroundColor, 255, 5000, false, false, true);
        }

        public static void AddXPGainTextCue(string text)
        {
            int measureStringHalved = BitmapFont.ActiveFonts["changaone20"].MeasureString(text) / 2;
            int lineHeightHalved = BitmapFont.ActiveFonts["changaone20"].LineHeight / 2;
            int x = Client.Width / 2 - measureStringHalved;
            int y = Client.Height / 2 - lineHeightHalved - 100;

            TextCue tc = new TextCue(text, x, y, 255, Color.Lime, Color.Transparent, 0, "changaone20", 4500, false, 0, Map.Direction.None, false, false, false, TextCueTag.XPGain);

            if (!GuiManager.ContainsVitalsUpdateTextCue(tc))
                GuiManager.TextCues.Add(tc);
        }

        public static void AddXPLossTextCue(string text)
        {
            int measureStringHalved = BitmapFont.ActiveFonts["changaone20"].MeasureString(text) / 2;
            int lineHeightHalved = BitmapFont.ActiveFonts["changaone20"].LineHeight / 2;
            int x = Client.Width / 2 - measureStringHalved;
            int y = Client.Height / 2 - lineHeightHalved - 100;

            TextCue tc = new TextCue(text, x, y, 255, Color.Red, Color.Transparent, 0, "changaone20", 3500, false, 0, Map.Direction.None, false, false, true, TextCueTag.XPLoss);

            if (!GuiManager.ContainsVitalsUpdateTextCue(tc))
                GuiManager.TextCues.Add(tc);
        }

        public static void AddHealthGainTextCue(string text)
        {
            int lineHeight = BitmapFont.ActiveFonts["changaone20"].LineHeight;
            int x = Client.Width / 2;
            int y = Client.Height / 2 - (lineHeight * 2);

            Control control = GuiManager.GetControl("Tile24");
            if(control != null)
            {
                x = control.Position.X;
                y = control.Position.Y;
            }

            TextCue tc = new TextCue(text, x, y, 255, Color.PaleGreen, Color.Transparent, 0, "changaone20", 3500, false, 0, Map.Direction.None, false, false, true, TextCueTag.HealthGain);

            if(!GuiManager.ContainsVitalsUpdateTextCue(tc))
                GuiManager.TextCues.Add(tc);
        }

        public static void AddHealthLossTextCue(string text)
        {
            int lineHeight = BitmapFont.ActiveFonts["changaone20"].LineHeight;
            int x = Client.Width / 2;
            int y = Client.Height / 2 - (lineHeight * 2);

            TextCue tc = new TextCue(text, x, y, 255, Color.Firebrick, Color.Transparent, 0, "changaone20", 3500, false, 0, Map.Direction.None, false, false, true, TextCueTag.HealthLoss);

            if (!GuiManager.ContainsVitalsUpdateTextCue(tc))
                GuiManager.TextCues.Add(tc);
        }

        public static void AddManaGainTextCue(string text)
        {
            int measureString = BitmapFont.ActiveFonts["changaone20"].MeasureString(text);
            int x = Client.Width - (measureString * 2);
            int y = Client.Height / 2;

            TextCue tc = new TextCue(text, x, y, 255, Color.RoyalBlue, Color.Transparent, 0, "changaone20", 3500, false, 0, Map.Direction.None, false, false, true, TextCueTag.ManaGain);

            if (GuiManager.GetControl("Tile24") is Control control)
                y = control.Position.Y;

            if (!GuiManager.ContainsVitalsUpdateTextCue(tc))
                GuiManager.TextCues.Add(tc);
        }

        public static void AddManaLossTextCue(string text)
        {
            int measureString = BitmapFont.ActiveFonts["changaone20"].MeasureString(text);
            int x = Client.Width / 2;
            int y = Client.Height / 2;

            TextCue tc = new TextCue(text, x, y, 255, Color.Blue, Color.Transparent, 0, "changaone20", 3500, false, 0, Map.Direction.None, false, false, true, TextCueTag.ManaLoss);

            if (!GuiManager.ContainsVitalsUpdateTextCue(tc))
                GuiManager.TextCues.Add(tc);
        }

        public static void AddStaminaGainTextCue(string text)
        {
            int measureString = BitmapFont.ActiveFonts["changaone20"].MeasureString(text);
            int lineHeight = BitmapFont.ActiveFonts["changaone20"].LineHeight;
            int x = Client.Width - (measureString * 2);
            int y = Client.Height / 2 + (lineHeight * 2);

            if (GuiManager.GetControl("Tile24") is Control control)
                y = control.Position.Y;

            TextCue tc = new TextCue(text, x, y, 255, Color.Orange, Color.Transparent, 0, "changaone20", 3500, false, 0, Map.Direction.None, false, false, true, TextCueTag.StaminaGain);

            if (!GuiManager.ContainsVitalsUpdateTextCue(tc))
                GuiManager.TextCues.Add(tc);
        }

        public static void AddStaminaLossTextCue(string text)
        {
            int x = Client.Width / 2;
            int y = Client.Height / 2;

            TextCue tc = new TextCue(text, x, y, 255, Color.DarkOrange, Color.Transparent, 0, "changaone20", 3500, false, 0, Map.Direction.None, false, false, true, TextCueTag.StaminaLoss);

            if (!GuiManager.ContainsVitalsUpdateTextCue(tc))
                GuiManager.TextCues.Add(tc);
        }

        public void OnClientResize(Rectangle prev, Rectangle now)
        {
            X += (now.Width - prev.Width);
            Y += (now.Height - prev.Height);
        }
    }
}
