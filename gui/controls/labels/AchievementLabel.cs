using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Yuusha.gui
{
    /// <summary>
    /// Display achievements and losses. AchievementLabels sit in a queue and are shown chronologically.
    /// </summary>
    public class AchievementLabel : Label
    {
        public enum AchievementType
        {
            None,
            SkillUp,
            Vitals_HitsGain,
            Vitals_StaminaGain,
            Vitals_ManaGain,
            LevelUp,
            NewSpell,
            NewTalent,
            StrengthAdd,
            DexterityAdd,
            AbilityScoreGain,
            AbilityScoreLoss,
        }

        bool m_fadeIn = true;
        DateTime m_timeAdded;
        Color m_stopColor = Color.PaleGreen;
        int m_stopSize = 640; // 640
        //bool m_scaleFont;
        //int m_scaleFontIndex;
        int m_enlargenRate = 14; // factor of 2
        string m_soundFile = "";
        bool m_soundPlayed = false;
        bool m_slideOffScreen = true;
        Map.Direction m_slideDirection = Map.Direction.None;
        AchievementType m_achievementType = AchievementType.None;
        //float m_scaleSize = .25f;
        //float m_scaleIncrement = .1f;

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
            //TintColor = new Color(0, 0, 0);
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
            int yTextOffset = 0;
            int enlargenRate = 14;

            int x = Client.Width / 2;
            int y = Client.Height / 2;
            int height = 25;
            int width = 25;

            try
            {
                if (GuiManager.CurrentSheet["Tile24"] is SpinelTileLabel spLabel)
                {
                    x = spLabel.Position.X;
                    y = spLabel.Position.Y;
                    width = spLabel.Width;
                    height = spLabel.Height;

                    if (achievement.ToString().StartsWith("VitalsGain") && GuiManager.CurrentSheet["VitalsWindow"] is Window w)
                    {
                        slideDirection = Map.GetDirection(spLabel, w);
                    }
                }

                switch (achievement)
                {
                    case AchievementType.AbilityScoreGain:
                        tintColor = Color.LightSteelBlue;
                        textColor = Color.White;
                        visualKey = GameHUD.GameIconsDictionary["upgrade"];
                        soundFile = "GUISounds/stat_gain";
                        stopSize = 256;
                        font = Client.ClientSettings.DefaultHUDNumbersFont;
                        break;
                    case AchievementType.AbilityScoreLoss:
                        tintColor = Color.Firebrick;
                        textColor = Color.White;
                        visualKey = GameHUD.GameIconsDictionary["downgrade"];
                        soundFile = "GUISounds/stat_loss";
                        stopSize = 256;
                        font = Client.ClientSettings.DefaultHUDNumbersFont;
                        break;
                    case AchievementType.DexterityAdd:
                    case AchievementType.StrengthAdd:
                        tintColor = Color.Indigo;
                        textColor = Color.PaleGreen;
                        visualKey = GameHUD.GameIconsDictionary["upgrade"];
                        soundFile = "GUISounds/sword_draw";
                        stopSize = 256;
                        font = Client.ClientSettings.DefaultHUDNumbersFont;
                        break;
                    case AchievementType.Vitals_HitsGain:
                        tintColor = Color.Red;
                        textColor = Color.White;
                        visualKey = GameHUD.GameIconsDictionary["upgrade"];
                        soundFile = "GUISounds/sword_draw";
                        stopSize = 256;
                        font = Client.ClientSettings.DefaultHUDNumbersFont;
                        slideDirection = Map.Direction.Southeast;
                        break;
                    case AchievementType.Vitals_ManaGain:
                        tintColor = Color.RoyalBlue;
                        textColor = Color.White;
                        visualKey = GameHUD.GameIconsDictionary["upgrade"];
                        soundFile = "GUISounds/sword_draw";
                        stopSize = 256;
                        font = Client.ClientSettings.DefaultHUDNumbersFont;
                        slideDirection = Map.Direction.Southeast;
                        break;
                    case AchievementType.Vitals_StaminaGain:
                        tintColor = Color.ForestGreen;
                        textColor = Color.White;
                        visualKey = GameHUD.GameIconsDictionary["upgrade"];
                        soundFile = "GUISounds/sword_draw";
                        stopSize = 256;
                        font = Client.ClientSettings.DefaultHUDNumbersFont;
                        slideDirection = Map.Direction.Southeast;
                        break;
                    case AchievementType.LevelUp:
                        tintColor = Color.White;
                        textColor = Color.Indigo;
                        visualKey = "GoldDragonLogo";
                        soundFile = ""; // sound info from server
                        stopSize = 584;
                        slideOffScreen = false;
                        font = "lobster156";
                        enlargenRate = 10;
                        slideDirection = Map.Direction.North;
                        break;
                    case AchievementType.NewTalent:
                    case AchievementType.NewSpell:
                        tintColor = TextManager.GetAlignmentColor(false, Character.CurrentCharacter.Alignment);
                        textColor = TextManager.GetAlignmentColor(true, Character.CurrentCharacter.Alignment);
                        visualKey = "WhiteSpace";
                        soundFile = "GUISounds/new_spell"; // TODO: different sound 8/28/2019
                        font = TextManager.ScalingTextFontList[TextManager.ScalingTextFontList.Count - 1];
                        stopSize = BitmapFont.ActiveFonts[font].MeasureString(text);
                        slideOffScreen = false;
                        enlargenRate = 40;
                        x = Client.Width / 2 - BitmapFont.ActiveFonts[font].MeasureString(text) / 2;
                        y = 60;
                        width = BitmapFont.ActiveFonts[font].MeasureString(text);
                        height = BitmapFont.ActiveFonts[font].LineHeight / 4;
                        break;
                }

                AchievementLabel label = new AchievementLabel("AchievementLabel_" + Program.Client.ClientGameTime.TotalGameTime.ToString() + "_" + achievement.ToString() + "_" + text, "",
                    new Rectangle(x, y, width, height), text, textColor, true, false, font,
                    new VisualKey(visualKey), tintColor, 40, 0, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>() { }, "")
                {
                    m_soundFile = soundFile,
                    m_slideOffScreen = slideOffScreen,
                    m_stopSize = stopSize,
                    m_stopColor = tintColor,
                    m_slideDirection = slideDirection,
                    YTextOffset = yTextOffset,
                    //EnlargenTextRectangle = true,
                    m_enlargenRate = enlargenRate,
                    m_achievementType = achievement,

                    TextShadow = true,
                    TextShadowDirection = Map.Direction.Northwest,
                    TextShadowDistance = 5,
                    TextShadowAlpha = 130,
                    m_dropShadow = true,
                    m_shadowDirection = Map.Direction.Northwest,
                    m_shadowDistance = 5
                };

                if (GameHUD.AchievementLabelList.Count <= 0)
                {
                    label.TimeAdded = DateTime.Now;
                    GuiManager.GenericSheet.AddControl(label);
                }

                GameHUD.AchievementLabelList.Add(label);
            }
            catch(Exception e)
            {
                Utils.LogException(e);
            }

        }

        /// <summary>
        /// Currently only being used for skill level ups.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="visualKey"></param>
        /// <param name="tintColor"></param>
        /// <param name="textColor"></param>
        /// <param name="soundFile"></param>
        /// <param name="slideOffScreen"></param>
        /// <param name="slideDirection"></param>
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
                m_soundPlayed = soundFile == "" || soundFile == null,
                m_slideOffScreen = slideOffScreen,
                m_slideDirection = slideDirection,
                EnlargenTextRectangle = false,
                TextShadow = true,
                TextShadowDirection = Map.Direction.Northwest,
                TextShadowDistance = 5,
                TextShadowAlpha = 130,
                m_dropShadow = true,
                m_shadowDirection = Map.Direction.Northwest,
                m_shadowDistance = 5,
                m_achievementType = AchievementType.SkillUp,
                //m_scaleSize = .2f,
                //m_scaleIncrement = .3f
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
            if(m_soundFile != null && !string.IsNullOrEmpty(m_soundFile) && !m_soundPlayed)
            {
                Audio.AudioManager.PlaySoundEffect(m_soundFile);
                m_soundPlayed = true;
            }

            base.Update(gameTime);

            // The time for this achievement label has expired, or other conditions have been met.
            if(DateTime.Now - m_timeAdded > TimeSpan.FromSeconds(6.0) || VisualAlpha <= 0 || Width <= 10 || Height <= 10)
            {
                IsVisible = false;
                GameHUD.AchievementLabelList.Remove(this);

                if(GameHUD.AchievementLabelList.Count > 0)
                {
                    (GameHUD.AchievementLabelList[0] as AchievementLabel).TimeAdded = DateTime.Now;
                    GuiManager.GenericSheet.AddControl(GameHUD.AchievementLabelList[0]);
                }

                GuiManager.Dispose(this);
                return;
            }

            try
            {
                if (IsVisible)
                {
                    ZDepth = 0;

                    if (m_fadeIn)
                    {
                        //byte r = TintColor.R;
                        //byte g = TintColor.G;
                        //byte b = TintColor.B;
                        //if (r < m_stopColor.R) r += 3;
                        //if (g < m_stopColor.G) g += 3;
                        //if (b < m_stopColor.B) b += 3;
                        //TintColor = new Color(r, g, b);
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

                        //if(m_achievementType == AchievementType.SkillUp)
                        //{
                        //    m_scaleSize += m_scaleIncrement;
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
                        EnlargenTextRectangle = false;
                        VisualAlpha -= 3;
                        TextAlpha = VisualAlpha;
                        if (m_slideOffScreen)
                        {
                            int xMove = 0;
                            int yMove = 0;

                            switch(m_slideDirection)
                            {
                                case Map.Direction.Southwest:
                                    xMove = -15;
                                    yMove = 15;
                                    break;
                                case Map.Direction.Southeast:
                                    xMove = 35;
                                    yMove = 35;
                                    break;
                                case Map.Direction.Northwest:
                                    xMove = -15;
                                    yMove = -15;
                                    break;
                                case Map.Direction.Northeast:
                                    xMove = 35;
                                    yMove = -15;
                                    break;
                                case Map.Direction.North:
                                    yMove = -35;
                                    break;
                                case Map.Direction.South:
                                    yMove = 15;
                                    break;
                                case Map.Direction.East:
                                    xMove = 35;
                                    break;
                                case Map.Direction.West:
                                    xMove = -15;
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

        //public override void Draw(GameTime gameTime)
        //{
        //    if (!IsVisible)
        //        return;

        //    if (m_achievementType == AchievementType.SkillUp)
        //    {
        //        // Draw a rotating sprite around the outside of the window.
        //        VisualInfo vi = GuiManager.Visuals[m_visualKey.Key];
        //        Color color = new Color(m_tintColor.R, m_tintColor.G, m_tintColor.B, VisualAlpha);

        //        if (Character.CurrentCharacter != null)
        //            color = TextManager.GetAlignmentColor(Character.CurrentCharacter.Alignment);

        //        Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], new Vector2(Position.X + Width / 2, Position.Y + Height / 2),
        //                vi.Rectangle, color, 0f, new Vector2(vi.Width / 2, vi.Height / 2), m_scaleSize, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 1);

        //        Color textColor = new Color(m_textColor, TextAlpha);

        //        if (m_disabled)
        //            textColor = new Color(ColorDisabledStandard, TextAlpha);

        //        if (BitmapFont.ActiveFonts.ContainsKey(Font))
        //        {
        //            // override BitmapFont sprite batch
        //            BitmapFont.ActiveFonts[Font].SpriteBatchOverride(Client.SpriteBatch);
        //            // set font alignment
        //            BitmapFont.ActiveFonts[Font].Alignment = TextAlignment;
        //            // draw string
        //            Rectangle rect = new Rectangle(m_textRectangle.X + XTextOffset, m_textRectangle.Y + YTextOffset, m_textRectangle.Width, m_textRectangle.Height);
        //            // change color of text if mouse over text color is not null
        //            if (!string.IsNullOrEmpty(m_text) && m_text.Length > 0)
        //            {
        //                // draw shadow
        //                if (TextShadow)
        //                {
        //                    Rectangle shadowRect = new Rectangle(rect.X + GetXShadow(TextShadowDirection, TextShadowDistance), rect.Y + GetYShadow(TextShadowDirection, TextShadowDistance), rect.Width, rect.Height);
        //                    Color shadowColor = new Color(Color.Black, TextShadowAlpha);
        //                    BitmapFont.ActiveFonts[Font].TextBox(shadowRect, shadowColor, m_text);
        //                }

        //                if (!m_disabled && m_hasTextOverColor && m_controlState == Enums.EControlState.Over)
        //                    BitmapFont.ActiveFonts[Font].TextBox(rect, new Color(m_textOverColor, TextAlpha), m_text);
        //                else
        //                    BitmapFont.ActiveFonts[Font].TextBox(rect, textColor, m_text);
        //            }
        //        }
        //        else Utils.LogOnce("BitmapFont.ActiveFonts does not contain the Font [ " + Font + " ] for Label [ " + m_name + " ] of Sheet [ " + GuiManager.CurrentSheet.Name + " ]");

        //        if (Border != null) Border.Draw(gameTime);
        //    }
        //    else base.Draw(gameTime);
        //}
    }
}
