using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class TextCue
    {
        public enum TextCueTag { None, PromptState, WarmedSpell }

        #region Private Data
        private string m_text;
        private int m_x;
        private int m_y;
        private string m_font;
        private byte m_alpha;
        private Color m_color;
        private Color m_backgroundColor;
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

        public TextCue(string text, int x, int y, byte alpha, Color color, Color backgroundColor, string font, double lifeCycle,
            bool dropShadow, int shadowDistance, Map.Direction shadowDirection, bool centered, bool fadeIn, bool fadeOut, TextCueTag tag)
        {
            m_text = text;
            m_x = x;
            m_y = y;
            m_alpha = alpha;
            m_color = new Color(color.R, color.G, color.B, color.A);
            m_backgroundColor = new Color(backgroundColor.R, backgroundColor.G, backgroundColor.B, backgroundColor.A);
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
            byte currentAlpha = m_alpha;

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
                // TODO: handle fade in and fade out
            }

            if (cueList.Contains(this) && m_alpha != currentAlpha)
            {
                m_color = new Color(m_color.R, m_color.G, m_color.B, m_alpha);

                if (m_backgroundColor != null && m_backgroundColor != Color.Transparent)
                    m_backgroundColor = new Color(m_backgroundColor.R, m_backgroundColor.G, m_backgroundColor.B, m_alpha);
            }
        }

        public void Draw(GameTime gameTime)
        {
            BitmapFont.ActiveFonts[m_font].SpriteBatchOverride(Client.SpriteBatch);

            // if the TextCue has a background color, draw the rectangle using WhiteSpace visual
            if (m_backgroundColor != null && m_backgroundColor != Color.Transparent)
            {
                VisualInfo vi = GuiManager.Visuals["WhiteSpace"];
                Rectangle rect = new Rectangle(m_x, m_y, BitmapFont.ActiveFonts[m_font].MeasureString(m_text), BitmapFont.ActiveFonts[m_font].LineHeight);
                Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], rect, vi.Rectangle, m_backgroundColor);
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
            AddMouseCursorTextCue(text, Client.UserSettings.ColorDefaultMouseCursorTextCue, GuiManager.CurrentSheet.Font);
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
                TextCue tc = new TextCue(text, ms.X, ms.Y - BitmapFont.ActiveFonts[font].LineHeight, 255, color, Color.Transparent, font, 2500, false, 2, Map.Direction.Southeast, false, false, true, TextCueTag.None);

                cursor.TextCues.Clear();

                cursor.TextCues.Add(tc);
            }
        }

        public static void AddCursorTextCue(string text, Color forecolor, Color backcolor, string font)
        {
            MouseState ms = GuiManager.MouseState;

            MouseCursor cursor;

            if (GuiManager.CurrentSheet.CursorOverride != "")
                cursor = GuiManager.Cursors[GuiManager.CurrentSheet.CursorOverride];
            else cursor = GuiManager.Cursors[GuiManager.CurrentSheet.Cursor];

            if (cursor != null)
            {
                TextCue tc = new TextCue(text, ms.X, ms.Y - BitmapFont.ActiveFonts[font].LineHeight, 255, forecolor, backcolor, font, 2500, false, 2, Map.Direction.Southeast, false, false, true, TextCueTag.None);

                cursor.TextCues.Clear();

                cursor.TextCues.Add(tc);
                //cursor.TextCues.Add(new TextCue(text, ms.X, ms.Y - BitmapFont.ActiveFonts[font].LineHeight, 255, color, Color.Transparent, font, 0, true, 2, Map.Direction.Southeast, false, false, false, TextCueTag.None));
            }
        }

        public static void ClearCursorTextCue()
        {
            MouseState ms = GuiManager.MouseState;

            MouseCursor cursor;

            if (GuiManager.CurrentSheet.CursorOverride != "")
                cursor = GuiManager.Cursors[GuiManager.CurrentSheet.CursorOverride];
            else cursor = GuiManager.Cursors[GuiManager.CurrentSheet.Cursor];

            if (cursor != null)
            {
                cursor.TextCues.Clear();
            }
        }

        public static void AddClientInfoTextCue(string text)
        {
            AddClientInfoTextCue(text, TextCueTag.None, Color.Yellow, Color.Black, 3500, false, false, true);
        }

        public static void AddClientInfoTextCue(string text, double lifeCycle)
        {
            AddClientInfoTextCue(text, TextCueTag.None, Color.Yellow, Color.Black, lifeCycle, false, false, true);
        }

        public static void AddClientInfoTextCue(string text, TextCueTag tag, Color color, Color backgroundColor, double lifeCycle, bool fadeIn, bool fadeOut, bool addOnce)
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
                case Enums.EGameState.Game:
                case Enums.EGameState.IOKGame:
                case Enums.EGameState.LOKGame:
                case Enums.EGameState.SpinelGame:
                    x = 10;
                    y = Client.Height - Convert.ToInt32(BitmapFont.ActiveFonts[GuiManager.CurrentSheet.Font].LineHeight * 1.25);
                    break;
                default:
                    centered = true;
                    break;
            }

            TextCue tc = new TextCue(text, x, y, (!fadeIn ? (byte)255 : (byte)1), color, backgroundColor, GuiManager.CurrentSheet.Font, lifeCycle,
                true, 2, Map.Direction.None, centered, fadeIn, fadeOut, tag);

            // disable multiple text cues for the time being
            if (GuiManager.TextCues.Count >= 10)
                GuiManager.TextCues.RemoveAt(0);

            if (!centered)
                GuiManager.TextCues.Clear();

            GuiManager.TextCues.Add(tc);
        }

        public static void AddChantingTextCue(string chant)
        {
            if(Client.GameState.ToString().ToLower().EndsWith("game") && Character.CurrentCharacter != null)
            {
                int x = 10;
                int y = Client.Height - Convert.ToInt32(BitmapFont.ActiveFonts["levRunes20"].LineHeight * 1.25);

                TextCue tc = new TextCue(chant, x, y, 255, Client.UserSettings.Color_Gui_Spell_Warming_TextCue, Color.Transparent, "levRunes20", 1500,
                    true, 2, Map.Direction.Southeast, true, true, true, TextCueTag.WarmedSpell);
            }
        }

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
                    cueColor = Color.White;
                    promptText = "You are stunned!";
                    backgroundColor = Color.Maroon;
                    break;
            }

            gui.TextCue.AddClientInfoTextCue(promptText, gui.TextCue.TextCueTag.PromptState, cueColor, backgroundColor, 0, false, false, true);
        }

        public static void AddWarmingSpellTextCue(string text)
        {

        }
    }
}
