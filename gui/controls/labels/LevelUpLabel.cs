using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace Yuusha.gui
{
    public class LevelUpLabel : Label
    {
        Rectangle m_rectangleQuad1;
        Rectangle m_rectangleQuad2;
        Rectangle m_rectangleQuad3;
        Rectangle m_rectangleQuad4;
        List<Rectangle> QuadrantsList = new List<Rectangle>();
        Point m_deadCenter;

        public int Speed
        { get; set; } = 8;

        public DateTime TimeAdded
        { get; set; }

        public LevelUpLabel(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string onDoubleClickEvent,
            string cursorOverride, List<Enums.EAnchorType> anchors, string popUpText) : base(name, owner, rectangle, text,
                textColor, visible, disabled, font, visualKey, tintColor, visualAlpha, textAlpha, textAlignment,
                xTextOffset, yTextOffset, onDoubleClickEvent, cursorOverride, anchors, popUpText)
        {
            m_rectangleQuad1 = new Rectangle(-168, -147, 168, 147);
            m_rectangleQuad2 = new Rectangle(Client.Width + 168, -147, 168, 147);
            m_rectangleQuad3 = new Rectangle(-168, Client.Height + 146, 168, 146);
            m_rectangleQuad4 = new Rectangle(Client.Width + 168, Client.Height + 146, 168, 146);
        }

        public static void CreateLevelUpLabel(string level)
        {
            int width = 336;
            int height = 293;
            int x = Client.Width / 2 - (width / 2);
            int y = Client.Height / 2 - (height / 2);

            if (GuiManager.CurrentSheet["Tile24"] is SpinelTileLabel spLabel)
            {
                x = spLabel.Position.X + (spLabel.Width / 2) - (width / 2);
                y = spLabel.Position.Y + (spLabel.Height / 2) - (height / 2);
            }

            LevelUpLabel label = new LevelUpLabel("LevelUpLabel_" + level, "", new Rectangle(x, y, width, height), level, Color.White, true, false, TextManager.ScalingNumberFontList[TextManager.ScalingNumberFontList.Count - 1],
                new VisualKey("LevelUpSymbol"), Color.White, 255, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>(), "")
            {
                TimeAdded = DateTime.Now,
                m_deadCenter = new Point(x + (width / 2), y + (height / 2))
            };

            GameHUD.AchievementLabelList.Add(label);

            GuiManager.GenericSheet.AddControl(label);
        }

        public override void Update(GameTime gameTime)
        {
            if(DateTime.Now - TimeAdded > TimeSpan.FromSeconds(5))
            {
                IsVisible = false;
            }

            if(!IsVisible)
            {
                GameHUD.AchievementLabelList.Remove(this);
                if (GameHUD.AchievementLabelList.Count > 0)
                {
                    (GameHUD.AchievementLabelList[0] as AchievementLabel).TimeAdded = DateTime.Now;
                    GuiManager.GenericSheet.AddControl(GameHUD.AchievementLabelList[0]);
                }
                GuiManager.Dispose(this);
                return;
            }

            base.Update(gameTime);

            QuadrantsList.Clear();

            if (m_rectangleQuad1.X < m_rectangle.X)
                m_rectangleQuad1.X += Speed;
            if (m_rectangleQuad1.Y < m_rectangle.Y)
                m_rectangleQuad1.Y += Speed;

            if (m_rectangleQuad2.X > m_deadCenter.X)
                m_rectangleQuad2.X -= Speed;
            if (m_rectangleQuad2.Y < m_rectangle.Y)
                m_rectangleQuad2.Y += Speed;

            if (m_rectangleQuad3.X < m_rectangle.X)
                m_rectangleQuad3.X += Speed;
            if (m_rectangleQuad3.Y > m_deadCenter.Y)
                m_rectangleQuad3.Y -= Speed;            

            if (m_rectangleQuad4.X > m_deadCenter.X)
                m_rectangleQuad4.X -= Speed;
            if (m_rectangleQuad4.Y > m_deadCenter.Y)
                m_rectangleQuad4.Y -= Speed;           

            if (m_rectangleQuad1.X > m_rectangle.X)
                m_rectangleQuad1.X = m_rectangle.X;
            if (m_rectangleQuad1.Y > m_rectangle.Y)
                m_rectangleQuad1.Y = m_rectangle.Y;

            if (m_rectangleQuad2.X < m_deadCenter.X)
                m_rectangleQuad2.X = m_deadCenter.X;
            if (m_rectangleQuad2.Y > m_rectangle.Y)
                m_rectangleQuad2.Y = m_rectangle.Y;

            if (m_rectangleQuad3.X > m_rectangle.X)
                m_rectangleQuad3.X = m_rectangle.X;
            if (m_rectangleQuad3.Y < m_deadCenter.Y)
                m_rectangleQuad3.Y = m_deadCenter.Y;

            if (m_rectangleQuad4.X < m_deadCenter.X)
                m_rectangleQuad4.X = m_deadCenter.X;
            if (m_rectangleQuad4.Y < m_deadCenter.Y)
                m_rectangleQuad4.Y = m_deadCenter.Y;

            QuadrantsList.Add(m_rectangleQuad1);
            QuadrantsList.Add(m_rectangleQuad2);
            QuadrantsList.Add(m_rectangleQuad3);
            QuadrantsList.Add(m_rectangleQuad4);

        }

        public override void Draw(GameTime gameTime)
        {
            if (!IsVisible)
                return;

            for(int i = 1; i <= 4; i++)
            {
                VisualInfo vi = GuiManager.Visuals["LevelUpQuad" + i];

                if(QuadrantsList.Count >= i)
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], QuadrantsList[i - 1], vi.Rectangle, new Color(TintColor, VisualAlpha));
            }
        }
    }
}
