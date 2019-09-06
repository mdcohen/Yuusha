using System;
using Microsoft.Xna.Framework;

namespace Yuusha.gui
{
    public class PercentageBarLabel : Label
    {
        public Enums.EAnchorType Orientation
        { get; set; } = Enums.EAnchorType.Left;

        private double m_percentage = 91;

        public double Percentage
        {
            get { return m_percentage; }
            set
            {
                if (value < 0) value = 0;
                else if (value > 100) value = 100;

                m_percentage = value;
            }
        }

        public Label MidLabel
        { get; set; }

        public Border MidBorder
        { get; set; }

        public Label ForeLabel
        { get; set; }

        public Border ForeBorder
        { get; set; }

        // Broken up into 10 visible segments.
        public bool Segmented
        { get; set; } = true;

        public int SegmentGapSize
        { get; set; } = 2;

        public Color SegmentGapColor
        { get; set; } = Color.Black;

        public Color SegmentGapOriginalColor
        { get; set; } = Color.Black;

        public PercentageBarLabel(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string onDoubleClickEvent,
            string cursorOverride, System.Collections.Generic.List<Enums.EAnchorType> anchors, string popUpText, bool segmented) : base(name, owner, rectangle, text,
                textColor, visible, disabled, font, visualKey, tintColor, visualAlpha, textAlpha, textAlignment,
                xTextOffset, yTextOffset, onDoubleClickEvent, cursorOverride, anchors, popUpText)
        {
            Segmented = segmented;

            MidLabel = new Label(Name + "MidLabel", Name, new Rectangle(Position.X, Position.Y, Width, Height), "", Color.White, true, false,
                    Font, new VisualKey(""), Color.Transparent, 0, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new System.Collections.Generic.List<Enums.EAnchorType>(), "");
            ForeLabel = new Label(Name + "ForeLabel", Name, new Rectangle(Position.X, Position.Y, Width, Height), "", Color.White, true, false,
                    Font, new VisualKey(""), Color.Transparent, 0, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new System.Collections.Generic.List<Enums.EAnchorType>(), "");

            //m_originalHeight = Height;
            //m_originalWidth = Width;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            try
            {
                switch (Orientation)
                {
                    case Enums.EAnchorType.Left:
                        MidLabel.Width = Convert.ToInt32(Width * Percentage / 100);
                        break;
                    case Enums.EAnchorType.Top:
                        MidLabel.Height = Convert.ToInt32(Height * Percentage / 100);
                        break;
                }
            }
            catch(Exception e)
            {
                Utils.LogException(e);
            }

            if (MidLabel != null)
            {
                MidLabel.Update(gameTime);
                MidLabel.Position = Position;
            }

            //if (MidBorder != null)
            //{
            //    MidBorder.Update(gameTime);
            //    MidBorder.Position = MidLabel.Position;
            //}

            if (ForeLabel != null)
            {
                ForeLabel.Update(gameTime);
                ForeLabel.Position = Position;
            }

            //if (ForeBorder != null)
            //{
            //    ForeBorder.Update(gameTime);
            //    ForeBorder.Position = Position;
            //}
        }

        public override void Draw(GameTime gameTime)
        {
            if (!IsVisible)
                return;

            base.Draw(gameTime);

            if (MidLabel != null) MidLabel.Draw(gameTime);
            if (MidBorder != null) MidBorder.Draw(gameTime);            

            if (Segmented)
            {
                VisualInfo vi = GuiManager.Visuals["WhiteSpace"];
                int segmentLength = Width / 10;
                for (int i = 0; i < 10; i++)
                {
                    Rectangle segRec = new Rectangle(Position.X + (segmentLength * i), Position.Y, SegmentGapSize, Height);
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], segRec, vi.Rectangle, new Color(Color.Black, VisualAlpha));
                }
            }

            if (ForeLabel != null) ForeLabel.Draw(gameTime);
            if (ForeBorder != null) ForeBorder.Draw(gameTime);
        }
    }
}
