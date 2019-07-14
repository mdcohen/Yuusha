using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Yuusha.gui
{
    public class AchievementLabel : Label
    {
        bool m_fadeIn = true;
        DateTime m_timeAdded;
        Color m_stopColor;
        bool m_scaleFont;
        int m_scaleFontIndex;
        int m_enlargenRate = 14; // factor of 2
        string m_soundFile = "";
        bool m_soundPlayed = false;
        bool m_slideOffScreen = true;

        public DateTime TimeAdded
        {
            set { m_timeAdded = value; }
        }

        public AchievementLabel(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string onDoubleClickEvent,
            string cursorOverride, List<Enums.EAnchorType> anchors, string popUpText) : base(name, owner, rectangle, text,
                textColor, visible, disabled, font, visualKey, tintColor, visualAlpha, textAlpha, textAlignment,
                xTextOffset, yTextOffset, onDoubleClickEvent, cursorOverride, anchors, popUpText)
        {
            m_stopColor = tintColor;
            TintColor = new Color(0, 0, 0);
        }

        public static void CreateAchievementLabel(string text, string font, bool scaleFont, string visualKey, Color tintColor, Color textColor, string soundFile, bool slideOffScreen)
        {
            int x = Client.Width / 2;
            int y = Client.Height / 2;
            int size = 25;

            if(GuiManager.GetControl("Tile24") is SpinelTileLabel spLabel)
            {
                x = spLabel.Position.X; y = spLabel.Position.Y; size = spLabel.Width;
            }

            AchievementLabel label = new AchievementLabel("AchievementLabel_" + Program.Client.ClientGameTime.ElapsedGameTime.ToString() + "_" + text, "",
                new Rectangle(x, y, size, size), text, textColor, true, false, font,
                new VisualKey(visualKey), tintColor, 40, 0, BitmapFont.TextAlignment.Center, 0, 0, "", "", new System.Collections.Generic.List<Enums.EAnchorType>(), "")
            {
                m_soundFile = soundFile,
                m_slideOffScreen = slideOffScreen
            };

            if (scaleFont)
            {
                label.m_scaleFont = true;
                label.m_scaleFontIndex = TextManager.ScalingFontList.IndexOf(label.Font);
            }

            if (GameHUD.AchievementLabelList.Count <= 0)
            {
                label.TimeAdded = DateTime.Now;
                GuiManager.GenericSheet.AddControl(label);
            }

            GameHUD.AchievementLabelList.Add(label);
        }

        public override void Update(GameTime gameTime)
        {
            if(m_soundFile != null && m_soundFile != "" && !m_soundPlayed)
            {
                Audio.AudioManager.PlaySoundEffect(m_soundFile);
                m_soundPlayed = true;
            }

            base.Update(gameTime);

            if(DateTime.Now - m_timeAdded > TimeSpan.FromSeconds(6.0) || VisualAlpha <= 0 || Width <= 10 || Height <= 10)
            {
                IsVisible = false;
                GameHUD.AchievementLabelList.Remove(this);
                if(GameHUD.AchievementLabelList.Count > 0)
                {
                    GameHUD.AchievementLabelList[0].TimeAdded = DateTime.Now;
                    GuiManager.GenericSheet.AddControl(GameHUD.AchievementLabelList[0]);
                }
                GuiManager.RemoveControl(this);
                return;
            }

            try
            {
                if (IsVisible)
                {
                    if (m_fadeIn)
                    {
                        byte r = TintColor.R;
                        byte g = TintColor.G;
                        byte b = TintColor.B;
                        if (r < m_stopColor.R) r += 3;
                        if (g < m_stopColor.G) g += 3;
                        if (b < m_stopColor.B) b += 3;
                        TintColor = new Color(r, g, b);
                        VisualAlpha += 10;
                        if (BitmapFont.ActiveFonts[Font].MeasureString(Text) <= Width)
                            TextAlpha = VisualAlpha;
                        if (m_scaleFont)
                        {
                            if (TextManager.ScalingFontList.Count > m_scaleFontIndex + 1 && (BitmapFont.ActiveFonts[TextManager.ScalingFontList[m_scaleFontIndex + 1]].MeasureString(Text) <= Width))
                            {
                                Font = TextManager.ScalingFontList[m_scaleFontIndex + 1];
                                m_scaleFontIndex++;
                            }
                            else if (TextManager.ScalingFontList.Count < m_scaleFontIndex + 1)
                                m_scaleFont = false;
                        }
                        Position = new Point(Position.X - m_enlargenRate, Position.Y - m_enlargenRate);
                        Width += m_enlargenRate * 2;
                        Height += m_enlargenRate * 2;

                        if (Width >= 640 || Position.X <= m_enlargenRate * 2 || Position.Y <= m_enlargenRate * 2 ||
                            Position.X >= Client.Width - (m_enlargenRate * 2) || Position.Y >= Client.Height - (m_enlargenRate * 2))
                        {
                            VisualAlpha = 255;
                            TextAlpha = 255;
                            m_fadeIn = false;
                            TintColor = m_stopColor;
                        }
                    }
                    else if (DateTime.Now - m_timeAdded >= TimeSpan.FromSeconds(1.75))
                    {
                        VisualAlpha -= 3;
                        TextAlpha = VisualAlpha;
                        if (m_slideOffScreen)
                        {
                            Position = new Point(Position.X - 10, Position.Y + 15);
                            Width -= 20;
                            Height -= 20;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utils.LogException(e);
            }
        }
    }
}
