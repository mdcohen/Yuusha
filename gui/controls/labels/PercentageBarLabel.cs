using Microsoft.Xna.Framework;

namespace Yuusha.gui
{
    public class PercentageBarLabel : Label
    {
        public Enums.EAnchorType Orientation
        { get; set; } = Enums.EAnchorType.Left;

        public double Percentage
        { get; set; } = 91;

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

        public PercentageBarLabel(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string onDoubleClickEvent,
            string cursorOverride, System.Collections.Generic.List<Enums.EAnchorType> anchors, string popUpText) : base(name, owner, rectangle, text,
                textColor, visible, disabled, font, visualKey, tintColor, visualAlpha, textAlpha, textAlignment,
                xTextOffset, yTextOffset, onDoubleClickEvent, cursorOverride, anchors, popUpText)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            ForeLabel.Width = System.Convert.ToInt32(Width * Percentage / 100);

            if (Segmented) ForeLabel.Width -= SegmentGapSize;

            PopUpText = string.Format("{0:0.00}%", Percentage);

            if (ForeLabel != null)
            {
                ForeLabel.Update(gameTime);
                ForeLabel.Position = Position;
            }

            if (ForeBorder != null)
            {
                ForeBorder.Update(gameTime);
                ForeBorder.Position = ForeLabel.Position;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (ForeLabel != null) ForeLabel.Draw(gameTime);
            if (ForeBorder != null) ForeBorder.Draw(gameTime);

            if (Segmented)
            {
                VisualInfo vi = GuiManager.Visuals["WhiteSpace"];
                int segmentLength = Width / 10;
                for (int i = 1; i < 10; i++)
                {
                    Rectangle segRec = new Rectangle(Position.X + (segmentLength * i) - SegmentGapSize, Position.Y, SegmentGapSize, Height);
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], segRec, vi.Rectangle, new Color(Color.Black, VisualAlpha));
                }
            }
        }
    }
}
