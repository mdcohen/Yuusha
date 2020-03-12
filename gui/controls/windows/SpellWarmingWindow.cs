using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Yuusha.gui
{
    public class SpellWarmingWindow : Window
    {
        private float m_circleRotation = 0;
        private DateTime m_timeCreated;
        public bool SpellWarmed // if 
        { get; set; } = false;
        private DateTime m_timeWarmed;
        private bool m_drawRotatingCircle = true;
        private int m_fadeInSpeed = 2;
        private int m_fadeOutSpeed = 3;
        private float m_drawScale = .65f; // .78f
        private bool m_scalingOut = false;
        private float m_scalingSpeed = .0018f;
        private float m_rotationSpeed = .008f;
        private float m_rotationSpeedIncrement = .00008f;
        public static string SpellRotatingVisualKey = "SpellWarmingCircle";
        private double m_subtractionRate = .05; // .05 magically worked for 3 second rounds... this could be calculated another way

        public Label SpellIconLabel
        { get; set; }
        private PercentageBarLabel PercentageBar
        { get; set; }

        public SpellWarmingWindow(string name, string owner, Rectangle rectangle, bool visible, bool locked, bool disabled, string font,
            VisualKey visualKey, Color tintColor, byte visualAlpha, bool dropShadow, Map.Direction shadowDirection, int shadowDistance,
            List<Enums.EAnchorType> anchors, string cursorOverride) : base(name, owner, rectangle, visible, locked, disabled, font, visualKey, tintColor, visualAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride)
        {
        }

        public static void CreateNewRoundCountdownWindow()
        {
            if (GuiManager.CurrentSheet["NewRoundCountdownWindow"] is SpellWarmingWindow existingWindow)
            {
                existingWindow.ResetWindow();
                return;
                //existingWindow.OnClose();
            }

            int x = Client.Width - 200;
            int y = 200;
            int width = 200;
            int height = 1;

            if (GuiManager.GetControl("MapDisplayWindow") is Window mapWindow)
            {
                width = mapWindow.Width;
                x = mapWindow.Position.X;
                y = mapWindow.Position.Y - height;
            }

            SpellWarmingWindow w = new SpellWarmingWindow("NewRoundCountdownWindow", "", new Rectangle(x, y, width, height), true, true, false,
                "lemon12", new VisualKey("WhiteSpace"), Color.DimGray, 0, false, Map.Direction.Northwest, 5, new List<Enums.EAnchorType>() { Enums.EAnchorType.Center }, "Dragging")
            {
                m_fadeInSpeed = 100,
                m_fadeOutSpeed = 50,
                m_drawRotatingCircle = false
            };
            GuiManager.CurrentSheet.AddControl(w);

            PercentageBarLabel newRoundCountdownPercentageBarLabel = new PercentageBarLabel("NewRoundCountdownLabelPercentageBarLabel", w.Name,
                new Rectangle(0, 0, width, height), "", Color.White, true, false, w.Font, new VisualKey("WhiteSpace"), Color.LightGreen,
                40, 0, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>(), "", false)
            {
                Percentage = 100,
                Segmented = false
            };
            newRoundCountdownPercentageBarLabel.MidLabel = new Label(newRoundCountdownPercentageBarLabel.Name + "MidLabel", newRoundCountdownPercentageBarLabel.Name,
                new Rectangle(newRoundCountdownPercentageBarLabel.Position.X, newRoundCountdownPercentageBarLabel.Position.Y, 0, newRoundCountdownPercentageBarLabel.Height), "", Color.White,
                true, false, newRoundCountdownPercentageBarLabel.Font, new VisualKey("WhiteSpace"), Color.LightGreen, 255, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>(), "");
            w.PercentageBar = newRoundCountdownPercentageBarLabel;

            GuiManager.CurrentSheet.AddControl(newRoundCountdownPercentageBarLabel);
            w.m_timeCreated = DateTime.Now;
        }

        public static void CreateCombatSkillRiskWindow()
        {
            if (GuiManager.CurrentSheet["CombatSkillRiskWindow"] is SpellWarmingWindow existingWindow)
            {
                //existingWindow.ResetWindow();
                //return;
                existingWindow.OnClose();
            }

            int x = Client.Width - 200;
            int y = 200;
            int width = 200;
            int height = 10;

            if (GuiManager.GetControl("CritterListWindow") is Window critterListWindow)
            {
                width = critterListWindow.Width;
                x = critterListWindow.Position.X;
                y = critterListWindow.Position.Y - height;
            }

            SpellWarmingWindow w = new SpellWarmingWindow("CombatSkillRiskWindow", "", new Rectangle(x, y, width, height), true, true, false,
                "lemon12", new VisualKey("WhiteSpace"), Color.DimGray, 0, false, Map.Direction.Northwest, 5, new List<Enums.EAnchorType>() { Enums.EAnchorType.Center }, "Dragging")
            {
                m_fadeInSpeed = 100,
                m_fadeOutSpeed = 50,
                m_drawRotatingCircle = false,
                m_subtractionRate = .08 // how fast skill risk appears to decay
            };
            GuiManager.CurrentSheet.AddControl(w);

            PercentageBarLabel skillRiskPercentageBarLabel = new PercentageBarLabel("SkillRiskPercentageBarLabel", w.Name,
                new Rectangle(0, 0, width, height), "", Color.White, true, false, w.Font, new VisualKey("WhiteSpace"), Color.PowderBlue,
                40, 0, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>(), "Combat Skill Risk", false)
            {
                Segmented = true,
                Percentage = Character.CurrentCharacter.SkillRisk * 1000,
                //Orientation = Enums.EAnchorType.Top
            };

            skillRiskPercentageBarLabel.MidLabel = new Label(skillRiskPercentageBarLabel.Name + "MidLabel", skillRiskPercentageBarLabel.Name,
                new Rectangle(skillRiskPercentageBarLabel.Position.X, skillRiskPercentageBarLabel.Height, 0, height), "", Color.White,
                true, false, skillRiskPercentageBarLabel.Font, new VisualKey("WhiteSpace"), Color.PowderBlue, 255, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>(), "");
            w.PercentageBar = skillRiskPercentageBarLabel;
            w.SpellWarmed = true;

            GuiManager.CurrentSheet.AddControl(skillRiskPercentageBarLabel);
            w.m_timeCreated = DateTime.Now;
        }

        public static void CreateNegativeStatusWindow(string spellName)
        {
            if (GuiManager.CurrentSheet[spellName + "NegativeStatusWindow"] is SpellWarmingWindow existingWindow)
            {
                existingWindow.ResetWindow();
                return;
                //existingWindow.OnClose();
            }

            int x = Client.Width - 200;
            int y = 200;

            if (GuiManager.GetControl("MapDisplayWindow") is Window mapWindow)
            {
                x = mapWindow.Position.X - 100;
                y = mapWindow.Position.Y;
            }

            SpellWarmingWindow w = new SpellWarmingWindow(spellName + "NegativeStatusWindow", "", new Rectangle(x, y, 74, 74), true, false, false,
                "lemon12", new VisualKey("WhiteSpace"), Color.DimGray, 0, true, Map.Direction.Northwest, 5, new List<Enums.EAnchorType>() { Enums.EAnchorType.Center }, "Dragging")
            {
                m_fadeOutSpeed = 5,
                m_drawRotatingCircle = false
            };
            GuiManager.CurrentSheet.AddControl(w);

            string iconVisual = "question_mark";
            if (Effect.IconsDictionary.ContainsKey(spellName))
                iconVisual = Effect.IconsDictionary[spellName];
            else if (GameHUD.GameIconsDictionary.ContainsKey(spellName))
                iconVisual = GameHUD.GameIconsDictionary[spellName];

            Color tintColor = Color.White;
            if (Effect.IconsTintDictionary.ContainsKey(spellName))
                tintColor = Effect.IconsTintDictionary[spellName];

            Label negativeEffectLabel = new Label(spellName + "NegativeEffectLabel", w.Name,
                new Rectangle(0, 0, 96, 96), "", Color.Red, true, false, "lemon12", new VisualKey(iconVisual), tintColor,
                255, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>() { Enums.EAnchorType.Center }, spellName)
            {
                Command = ""
            };
            w.SpellIconLabel = negativeEffectLabel;

            SquareBorder negativeEffectBorder = new SquareBorder(negativeEffectLabel.Name + "Border", negativeEffectLabel.Name, 1, new VisualKey("WhiteSpace"),
                false, Color.DimGray, negativeEffectLabel.VisualAlpha);

            PercentageBarLabel negativeEffectPercentageBarLabel = new PercentageBarLabel(spellName + "NegativeEffectLabelPercentageBarLabel", w.Name,
                new Rectangle(negativeEffectLabel.Position.X, negativeEffectLabel.Position.Y + negativeEffectLabel.Height, negativeEffectLabel.Width, 5), "",
                Color.White, false, false,
                negativeEffectLabel.Font, new VisualKey("WhiteSpace"), Color.Red, 255, 0, BitmapFont.TextAlignment.Center, 0, 0, "", "",
                new List<Enums.EAnchorType>(), "", false)
            {
                Percentage = 100,
                Segmented = false
            };
            negativeEffectPercentageBarLabel.MidLabel = new Label(negativeEffectPercentageBarLabel.Name + "MidLabel", negativeEffectPercentageBarLabel.Name,
                new Rectangle(negativeEffectPercentageBarLabel.Position.X, negativeEffectPercentageBarLabel.Position.Y, 0, negativeEffectPercentageBarLabel.Height), "", Color.White,
                false, false, negativeEffectPercentageBarLabel.Font, new VisualKey("WhiteSpace"), Color.Black, 255, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>(), "");
            w.PercentageBar = negativeEffectPercentageBarLabel;

            GuiManager.CurrentSheet.AddControl(negativeEffectLabel);
            GuiManager.CurrentSheet.AddControl(negativeEffectBorder);
            GuiManager.CurrentSheet.AddControl(negativeEffectPercentageBarLabel);
            w.m_timeCreated = DateTime.Now;
        }

        public static void CreatePositiveStatusWindow(string spellName)
        {
            if (GuiManager.CurrentSheet[spellName + "PositiveStatusWindow"] is SpellWarmingWindow existingWindow)
            {
                existingWindow.ResetWindow();
                return;
                //existingWindow.OnClose();
            }

            int x = Client.Width - 200;
            int y = 200;

            if (GuiManager.GetControl("MapDisplayWindow") is Window mapWindow)
            {
                x = mapWindow.Position.X - 100;
                y = mapWindow.Position.Y + 103;
            }

            SpellWarmingWindow w = new SpellWarmingWindow(spellName + "PositiveStatusWindow", "", new Rectangle(x, y, 74, 74), true, false, false,
                "lemon12", new VisualKey("WhiteSpace"), Color.DimGray, 0, true, Map.Direction.Northwest, 5, new List<Enums.EAnchorType>() { Enums.EAnchorType.Center }, "Dragging")
            {
                m_fadeOutSpeed = 5,
                m_drawRotatingCircle = false
            };
            GuiManager.CurrentSheet.AddControl(w);

            string iconVisual = "question_mark";
            if (Effect.IconsDictionary.ContainsKey(spellName))
                iconVisual = Effect.IconsDictionary[spellName];
            else if (GameHUD.GameIconsDictionary.ContainsKey(spellName))
                iconVisual = GameHUD.GameIconsDictionary[spellName];

            Color tintColor = Color.White;
            if (Effect.IconsTintDictionary.ContainsKey(spellName))
                tintColor = Effect.IconsTintDictionary[spellName];

            Label spellIconLabel = new Label(spellName + "NegativeEffectLabel", w.Name,
                new Rectangle(0, 0, 96, 96), "", Color.Red, true, false, "lemon12", new VisualKey(iconVisual), tintColor,
                255, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>() { Enums.EAnchorType.Center }, spellName)
            {
                Command = ""
            };
            w.SpellIconLabel = spellIconLabel;

            SquareBorder spellIconBorder = new SquareBorder(spellIconLabel.Name + "Border", spellIconLabel.Name, 1, new VisualKey("WhiteSpace"), false, Color.DimGray, spellIconLabel.VisualAlpha);

            PercentageBarLabel warmingTimePercentageBarLabel = new PercentageBarLabel(spellName + "NegativeEffectLabelPercentageBarLabel", w.Name,
                new Rectangle(spellIconLabel.Position.X, spellIconLabel.Position.Y + spellIconLabel.Height, spellIconLabel.Width, 1), "", Color.White, false, false,
                spellIconLabel.Font, new VisualKey("WhiteSpace"), Color.LimeGreen, 40, 0, BitmapFont.TextAlignment.Center, 0, 0, "", "",
                new List<Enums.EAnchorType>(), "", false)
            {
                Percentage = 100,
                Segmented = false
            };
            warmingTimePercentageBarLabel.MidLabel = new Label(warmingTimePercentageBarLabel.Name + "MidLabel", warmingTimePercentageBarLabel.Name,
                new Rectangle(warmingTimePercentageBarLabel.Position.X, warmingTimePercentageBarLabel.Position.Y, 0, warmingTimePercentageBarLabel.Height), "", Color.White,
                true, false, warmingTimePercentageBarLabel.Font, new VisualKey("WhiteSpace"), Color.LimeGreen, 255, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>(), "");
            w.PercentageBar = warmingTimePercentageBarLabel;

            GuiManager.CurrentSheet.AddControl(spellIconLabel);
            GuiManager.CurrentSheet.AddControl(spellIconBorder);
            GuiManager.CurrentSheet.AddControl(warmingTimePercentageBarLabel);
            w.m_timeCreated = DateTime.Now;
        }

        public static void CreateSpellWarmingWindow(string spellName)
        {
            if (GuiManager.SpellWarmingWindow is SpellWarmingWindow existingWindow)
            {
                existingWindow.OnClose();
            }

            int x = Client.Width - 200;
            int y = 200;

            if(GuiManager.CurrentSheet["MapDisplayWindow"] is Window mapWindow)
            {
                x = mapWindow.Position.X + mapWindow.Width + 35;
                y = mapWindow.Position.Y + 30;
            }

            SpellWarmingWindow w = new SpellWarmingWindow("SpellWarmingWindow", "", new Rectangle(x, y, 74, 74), true, false, false,
                "lemon12", new VisualKey("WhiteSpace"), Color.DimGray, 0, true, Map.Direction.Northwest, 5, new List<Enums.EAnchorType>() { Enums.EAnchorType.Center }, "Dragging");
            GuiManager.CurrentSheet.AddControl(w);

            string iconVisual = "question_mark";
            if (Effect.IconsDictionary.ContainsKey(spellName))
                iconVisual = Effect.IconsDictionary[spellName];

            Color tintColor = Color.White;
            if (Effect.IconsTintDictionary.ContainsKey(spellName))
                tintColor = Effect.IconsTintDictionary[spellName];

            Label spellIconLabel = new Label(spellName + "SpellWarmingLabel", w.Name,
                new Rectangle(0, 0, 96, 96), "", Color.White, true, false, "changaone16", new VisualKey(iconVisual), tintColor,
                40, 255, BitmapFont.TextAlignment.Center, 0, 0, "send_command", "", new List<Enums.EAnchorType>() { Enums.EAnchorType.Center }, "")
            {
                Command = "cast"
            };
            w.SpellIconLabel = spellIconLabel;

            SquareBorder spellIconBorder = new SquareBorder(spellIconLabel.Name + "Border", spellIconLabel.Name, 1, new VisualKey("WhiteSpace"), false, Color.White, spellIconLabel.VisualAlpha);

            PercentageBarLabel warmingTimePercentageBarLabel = new PercentageBarLabel(spellName + "SpellWarmingPercentageBarLabel", w.Name,
                new Rectangle(spellIconLabel.Position.X, spellIconLabel.Position.Y + spellIconLabel.Height, spellIconLabel.Width, 5), "", Color.White, true, false,
                spellIconLabel.Font, new VisualKey("WhiteSpace"), Color.RoyalBlue, 40, 0, BitmapFont.TextAlignment.Center, 0, 0, "", "",
                new List<Enums.EAnchorType>(), "", false)
            {
                Percentage = 100,
                Segmented = false
            };
            warmingTimePercentageBarLabel.MidLabel = new Label(warmingTimePercentageBarLabel.Name + "MidLabel", warmingTimePercentageBarLabel.Name,
                new Rectangle(warmingTimePercentageBarLabel.Position.X, warmingTimePercentageBarLabel.Position.Y, 0, warmingTimePercentageBarLabel.Height), "", Color.White,
                true, false, warmingTimePercentageBarLabel.Font, new VisualKey("WhiteSpace"), Color.RoyalBlue, 255, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>(), "");
            w.PercentageBar = warmingTimePercentageBarLabel;

            GuiManager.SpellWarmingWindow = w;
            GuiManager.CurrentSheet.AddControl(spellIconLabel);
            GuiManager.CurrentSheet.AddControl(spellIconBorder);
            GuiManager.CurrentSheet.AddControl(warmingTimePercentageBarLabel);
            w.m_timeCreated = DateTime.Now;
        }

        public override void Update(GameTime gameTime)
        {
            if(Name == "NewRoundCountdownWindow")
            {
                ZDepth = 1;
                if (GuiManager.GetControl("MapDisplayWindow") is Window mapWindow)
                {
                    //TextCue.AddClientInfoTextCue("Current Width: " + Width);
                    Width = mapWindow.Width;
                    //TextCue.AddClientInfoTextCue("MapWindow Width: " + mapWindow.Width);
                    //TextCue.AddClientInfoTextCue("New Width: " + Width);
                    Position = new Point(mapWindow.Position.X, mapWindow.Position.Y - (PercentageBar != null ? PercentageBar.Height : 5));
                }
            }

            if (Name == "CombatSkillRiskWindow")
            {
                ZDepth = 1;

                if (GuiManager.GetControl("CritterListWindow") is Window critterListWindow)
                {
                    Width = critterListWindow.Width;
                    Position = new Point(critterListWindow.Position.X, critterListWindow.Position.Y - (PercentageBar != null ? PercentageBar.Height : 5));
                }
            }

            if (Name.EndsWith("StatusWindow") || Name.Equals("SpellWarmingWindow"))
                ZDepth = 1;

            if (m_drawRotatingCircle)
            {
                if (Character.CurrentCharacter != null && (int)Character.CurrentCharacter.Alignment <= 2)
                    m_circleRotation += m_rotationSpeed;
                else m_circleRotation -= m_rotationSpeed;

                m_rotationSpeed += m_rotationSpeedIncrement;


                if (m_scalingOut)
                {
                    m_drawScale += m_scalingSpeed;
                    if (m_drawScale > 1f)
                        m_scalingOut = false;
                }
                else
                {
                    m_drawScale -= m_scalingSpeed;
                    if (m_drawScale < .05f)
                        m_scalingOut = true;
                }
            }

            if (SpellIconLabel != null)
            {
                if (!SpellWarmed)
                {
                    SpellIconLabel.VisualAlpha += m_fadeInSpeed;
                    if (SpellIconLabel.Border != null)
                        SpellIconLabel.Border.VisualAlpha = SpellIconLabel.VisualAlpha;
                }
                else if ((DateTime.Now - m_timeWarmed).TotalSeconds > .5)
                {
                    SpellIconLabel.VisualAlpha -= m_fadeOutSpeed;
                    SpellIconLabel.TextAlpha = SpellIconLabel.VisualAlpha;
                    if (SpellIconLabel.Border != null)
                        SpellIconLabel.Border.VisualAlpha = SpellIconLabel.VisualAlpha;

                    if (SpellIconLabel.VisualAlpha <= 0 || (DateTime.Now - m_timeCreated).TotalSeconds >= Utility.Settings.StaticSettings.RoundDelayLength * 2 / 2)
                    {
                        OnClose();
                        return;
                    }
                }
            }

            if (!SpellWarmed)
            {
                if (PercentageBar != null)
                {
                    PercentageBar.Percentage = (DateTime.Now - m_timeCreated).TotalMilliseconds / Utility.Settings.StaticSettings.RoundDelayLength * 100;
                    if (PercentageBar.Percentage >= 100)
                    {
                        PercentageBar.IsVisible = false;

                        SpellWarmed = true;

                        if (SpellIconLabel != null)
                            SpellIconLabel.VisualAlpha = 255;

                        m_timeWarmed = DateTime.Now;
                    }
                }
            }
            else
            {
                if (PercentageBar != null)
                {
                    PercentageBar.Percentage -= m_subtractionRate;
                    if (PercentageBar.Percentage <= 0)
                    {
                        PercentageBar.IsVisible = false;
                    }
                }
            }

            // Adjust percent bar position and width.
            if (PercentageBar != null)
            {
                PercentageBar.Width = Width;
                PercentageBar.Position = new Point(Position.X, Position.Y - PercentageBar.Height);
                if (PercentageBar.MidLabel != null)
                {
                    PercentageBar.MidLabel.Width = PercentageBar.Width;
                    PercentageBar.MidLabel.Position = new Point(Position.X, Position.Y - PercentageBar.Height);
                }

                if (PercentageBar.ForeLabel != null)
                {
                    PercentageBar.ForeLabel.Width = PercentageBar.Width;
                    PercentageBar.ForeLabel.Position = new Point(Position.X, Position.Y - PercentageBar.Height);
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!IsVisible)
                return;

            base.Draw(gameTime);

            if (m_drawRotatingCircle)
            {
                // Draw a rotating sprite around the outside of the window.
                VisualInfo vi = GuiManager.Visuals[SpellRotatingVisualKey];
                Color color = new Color(Color.White, SpellIconLabel.VisualAlpha);

                if (Character.CurrentCharacter != null)
                    color = TextManager.GetAlignmentColor(Character.CurrentCharacter.Alignment);

                Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], new Vector2(SpellIconLabel.Position.X + SpellIconLabel.Width / 2, SpellIconLabel.Position.Y + SpellIconLabel.Height / 2),
                        vi.Rectangle, color, m_circleRotation, new Vector2(vi.Width / 2, vi.Height / 2), m_drawScale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 1);
            }
        }

        public override void OnClose()
        {
            base.OnClose();

            if (GuiManager.SpellWarmingWindow == this)
                GuiManager.SpellWarmingWindow = null;

            GuiManager.RemoveControl(this);
        }

        /// <summary>
        /// Resets percentage to zero instead of creating a new Window.
        /// </summary>
        private void ResetWindow()
        {
            if (GuiManager.GetControl(Name) == null) return;

            if (PercentageBar != null)
            {
                PercentageBar.Percentage = 0;
                PercentageBar.IsVisible = true;

                if (PercentageBar.MidLabel != null)
                    PercentageBar.MidLabel.IsVisible = true;

                if (PercentageBar.ForeLabel != null)
                    PercentageBar.ForeLabel.IsVisible = true;
            }

            SpellWarmed = false;
            m_timeCreated = DateTime.Now;
        }
    }
}
