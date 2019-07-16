using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Yuusha.gui
{
    public class AchievementLabel : Label
    {
        public enum AchievementType
        {
            SkillUp,
            VitalsGain_HitsMax,
            VitalsGain_StaminaMax,
            VitalsGain_ManaMax,
        }

        bool m_fadeIn = true;
        DateTime m_timeAdded;
        Color m_stopColor = Color.Indigo;
        int m_stopSize = 640;
        //bool m_scaleFont;
        //int m_scaleFontIndex;
        int m_enlargenRate = 14; // factor of 2
        string m_soundFile = "";
        bool m_soundPlayed = false;
        bool m_slideOffScreen = true;
        Map.Direction m_slideDirection = Map.Direction.None;

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

        public static void CreateAchievementLabel(string text, AchievementType achievement)
        {
            Color textColor = Color.White;
            Color tintColor = Color.Indigo;
            string font = "courier12";
            string soundFile = "";
            int stopSize = 640;
            bool slideOffScreen = true;
            string visualKey = "";
            Map.Direction slideDirection = Map.Direction.Southwest;

            int x = Client.Width / 2;
            int y = Client.Height / 2;
            int size = 25;

            if (GuiManager.CurrentSheet["Tile24"] is SpinelTileLabel spLabel)
            {
                x = spLabel.Position.X;
                y = spLabel.Position.Y;
                size = spLabel.Width;

                if (achievement.ToString().StartsWith("VitalsGain"))
                {
                    if (GuiManager.CurrentSheet["VitalsWindow"] is Window w)
                    {
                        slideDirection = Map.GetDirection(spLabel, w);
                    }
                }
            }

            switch(achievement)
            {
                case AchievementType.VitalsGain_HitsMax:
                    tintColor = Color.Red;
                    textColor = Color.White;
                    visualKey = GameHUD.GameIconsDictionary["upgrade"];
                    soundFile = "GUISounds/sword_draw";
                    stopSize = 256;
                    font = TextManager.ScalingNumberFontList[7];
                    break;
                case AchievementType.VitalsGain_ManaMax:
                    tintColor = Color.RoyalBlue;
                    textColor = Color.White;
                    visualKey = GameHUD.GameIconsDictionary["upgrade"];
                    soundFile = "GUISounds/sword_draw";
                    stopSize = 256;
                    font = TextManager.ScalingNumberFontList[7];
                    break;
                case AchievementType.VitalsGain_StaminaMax:
                    tintColor = Color.ForestGreen;
                    textColor = Color.White;
                    visualKey = GameHUD.GameIconsDictionary["upgrade"];
                    soundFile = "GUISounds/sword_draw";
                    stopSize = 256;
                    font = TextManager.ScalingNumberFontList[7];
                    break;
            }

            AchievementLabel label = new AchievementLabel("AchievementLabel_" + Program.Client.ClientGameTime.ElapsedGameTime.ToString() + "_" + text, "",
                new Rectangle(x, y, size, size), text, textColor, true, false, font,
                new VisualKey(visualKey), tintColor, 40, 0, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>() { }, "")
            {
                m_soundFile = soundFile,
                m_slideOffScreen = slideOffScreen,
                m_stopSize = stopSize,
                m_stopColor = tintColor,
                m_slideDirection = slideDirection,
            };

            if (GameHUD.AchievementLabelList.Count <= 0)
            {
                label.TimeAdded = DateTime.Now;
                GuiManager.GenericSheet.AddControl(label);
            }

            GameHUD.AchievementLabelList.Add(label);

        }

        public static void CreateAchievementLabel(string text, string font, string visualKey, Color tintColor, Color textColor, string soundFile, bool slideOffScreen, Map.Direction slideDirection)
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
                new VisualKey(visualKey), tintColor, 40, 0, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>(), "")
            {
                m_soundFile = soundFile,
                m_slideOffScreen = slideOffScreen,
                m_slideDirection = slideDirection,
            };

            //if (scaleFont)
            //{
            //    label.m_scaleFont = true;
            //    label.m_scaleFontIndex = TextManager.ScalingFontList.IndexOf(label.Font);
            //}

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
                        //if (m_scaleFont)
                        //{
                        //    if (TextManager.ScalingFontList.Count > m_scaleFontIndex + 1 && (BitmapFont.ActiveFonts[TextManager.ScalingFontList[m_scaleFontIndex + 1]].MeasureString(Text) <= Width))
                        //    {
                        //        Font = TextManager.ScalingFontList[m_scaleFontIndex + 1];
                        //        m_scaleFontIndex++;
                        //    }
                        //    else if (TextManager.ScalingFontList.Count < m_scaleFontIndex + 1)
                        //        m_scaleFont = false;
                        //}
                        Position = new Point(Position.X - m_enlargenRate, Position.Y - m_enlargenRate);
                        Width += m_enlargenRate * 2;
                        Height += m_enlargenRate * 2;

                        if (Width >= m_stopSize || Position.X <= m_enlargenRate * 2 || Position.Y <= m_enlargenRate * 2 ||
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
                            int xMove = 0;
                            int yMove = 0;

                            switch(m_slideDirection)
                            {
                                case Map.Direction.Southwest:
                                    xMove = -10;
                                    yMove = 15;
                                    break;
                                case Map.Direction.Southeast:
                                    xMove = 30;
                                    yMove = 35;
                                    break;
                                case Map.Direction.Northwest:
                                    xMove = -10;
                                    yMove = -15;
                                    break;
                                case Map.Direction.Northeast:
                                    xMove = 30;
                                    yMove = -15;
                                    break;
                                case Map.Direction.North:
                                    yMove = -35;
                                    break;
                                case Map.Direction.South:
                                    yMove = 15;
                                    break;
                                case Map.Direction.East:
                                    xMove = 30;
                                    break;
                                case Map.Direction.West:
                                    xMove = -10;
                                    break;
                            }

                            Position = new Point(Position.X + xMove, Position.Y + yMove); // southwest
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
