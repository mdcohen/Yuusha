using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Yuusha.gui
{
    public class SoundIndicatorLabel : Label
    {
        private static bool AllSoundIndicatorsCreated = false;

        public Audio.AudioManager.SoundDirection Direction
        { get; set; }

        public SoundIndicatorLabel(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string onDoubleClickEvent,
            string cursorOverride, System.Collections.Generic.List<Enums.EAnchorType> anchors, string popUpText) : base(name, owner, rectangle, text,
                textColor, visible, disabled, font, visualKey, tintColor, visualAlpha, textAlpha, textAlignment,
                xTextOffset, yTextOffset, onDoubleClickEvent, cursorOverride, anchors, popUpText)
        {
        }

        public static void CreateAllSoundIndicators()
        {
            if (AllSoundIndicatorsCreated) return;

            for(int i = 4; i < 7; i++)
            {
                foreach (Audio.AudioManager.SoundDirection direction in System.Enum.GetValues(typeof(Audio.AudioManager.SoundDirection)))
                {
                    CreateSoundIndicator(direction, i);
                }
            }

            foreach (Control control in GuiManager.GenericSheet.Controls)
                if (control is SoundIndicatorLabel)
                    control.IsVisible = false;

            AllSoundIndicatorsCreated = true;
        }

        public static void CreateSoundIndicator(Audio.AudioManager.SoundDirection direction, int distance)
        {
            if (!Client.ClientSettings.DisplaySoundIndicators || Client.GameDisplayMode != Enums.EGameDisplayMode.Yuusha) return;

            if (GuiManager.GetControl(direction + "_" + distance + "_SoundIndicatorLabel") is SoundIndicatorLabel label)
            {
                label.VisualAlpha = (byte)(255 - (distance * 10));
                label.IsDisabled = false;
                label.IsVisible = true;
                return;
            }

            int width = Client.ClientSettings.SoundIndicatorDimensions;
            int height = Client.ClientSettings.SoundIndicatorDimensions;
            int x = (Client.Width / 2) - (width / 2);
            int y = (Client.Height / 2) - (height / 2);

            if(GuiManager.GetControl("Tile24") is MapTileLabel mapTileLabel)
            {
                x = mapTileLabel.Position.X + mapTileLabel.Width / 2;
                y = mapTileLabel.Position.Y + mapTileLabel.Height / 2;
            }

            // dead center
            //string directionString = "";
            switch(direction)
            {
                case Audio.AudioManager.SoundDirection.East:
                    x += 50 * distance;
                    //directionString = "E";
                    break;
                case Audio.AudioManager.SoundDirection.North:
                    y -= 50 * distance;
                    //directionString = "N";
                    break;
                case Audio.AudioManager.SoundDirection.South:
                    y += 50 * distance;
                    //directionString = "S";
                    break;
                case Audio.AudioManager.SoundDirection.West:
                    x -= 50 * distance;
                    //directionString = "W";
                    break;
                case Audio.AudioManager.SoundDirection.Northeast:
                    y -= 50 * distance;
                    x += 50 * distance;
                    //directionString = "NE";
                    break;
                case Audio.AudioManager.SoundDirection.Southeast:
                    y += 50 * distance;
                    x += 50 * distance;
                    //directionString = "SE";
                    break;
                case Audio.AudioManager.SoundDirection.Northwest:
                    y -= 50 * distance;
                    x -= 50 * distance;
                    //directionString = "NW";
                    break;
                case Audio.AudioManager.SoundDirection.Southwest:
                    y += 50 * distance;
                    x -= 50 * distance;
                    //directionString = "SW";
                    break;
            }

            while (y < 0) y++;
            while (x < 0) x++;

            while (x + width > Client.Width) x--;
            while (y + height > Client.Height) y--;

            //string text = directionString + " (" + distance.ToString() + ")";

            label = new SoundIndicatorLabel(direction + "_" + distance + "_SoundIndicatorLabel", "", new Rectangle(x, y, width, height), "", Client.ClientSettings.SoundIndicatorTextColor,
                true, false, Client.ClientSettings.SoundIndicatorFont, new VisualKey("SoundWavesIcon"), Client.ClientSettings.SoundIndicatorTintColor, (byte)(255 - (distance * 5)), 255, BitmapFont.TextAlignment.Center,
                0, 0, "", "", new System.Collections.Generic.List<Enums.EAnchorType>() { Enums.EAnchorType.Center }, "")
            {
                Direction = direction,
            };

            GuiManager.GenericSheet.AddControl(label);
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsVisible) return;

            base.Update(gameTime);

            VisualAlpha--;

            if (VisualAlpha <= 0)
            {
                VisualAlpha = 0;
                IsVisible = false;
                IsDisabled = true;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (!IsVisible) return;

            VisualInfo vi = GuiManager.Visuals[m_visualKey.Key];
            Color color = new Color(m_tintColor.R, m_tintColor.G, m_tintColor.B, VisualAlpha);

            float rotation = 0;
            SpriteEffects spriteEffect = SpriteEffects.None;

            switch(Direction)
            {
                case Audio.AudioManager.SoundDirection.North:
                    rotation = MathHelper.Pi / 2;
                    break;
                case Audio.AudioManager.SoundDirection.East:
                    rotation = MathHelper.Pi; // 180 degrees
                    break;
                case Audio.AudioManager.SoundDirection.South:
                    rotation = MathHelper.Pi / 2; // 90 degrees
                    spriteEffect = SpriteEffects.FlipHorizontally;
                    break;
                case Audio.AudioManager.SoundDirection.Northeast:
                    rotation = MathHelper.Pi - (MathHelper.Pi / 3);
                    break;
                case Audio.AudioManager.SoundDirection.Northwest:
                    rotation = MathHelper.Pi - (MathHelper.Pi / 3 * 2);
                    break;
                case Audio.AudioManager.SoundDirection.Southeast:
                    rotation = MathHelper.Pi - (MathHelper.Pi / 3 * 2);
                    spriteEffect = SpriteEffects.FlipHorizontally;
                    break;
                case Audio.AudioManager.SoundDirection.Southwest:
                    rotation = MathHelper.Pi - (MathHelper.Pi / 3);
                    spriteEffect = SpriteEffects.FlipHorizontally;
                    break;
            }

            Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_rectangle, vi.Rectangle, color, rotation, Vector2.Zero, spriteEffect, 0);

            if (TextAlpha > 0)
            {
                Color textColor;

                if (!m_disabled)
                    textColor = new Color(m_textColor.R, m_textColor.G, m_textColor.B, TextAlpha);
                else
                    textColor = new Color(ColorDisabledStandard.R, ColorDisabledStandard.G, ColorDisabledStandard.B, TextAlpha);

                if (BitmapFont.ActiveFonts.ContainsKey(Font))
                {
                    // override BitmapFont sprite batch
                    BitmapFont.ActiveFonts[Font].SpriteBatchOverride(Client.SpriteBatch);
                    // set font alignment
                    BitmapFont.ActiveFonts[Font].Alignment = TextAlignment;
                    // draw string
                    Rectangle rect = new Rectangle(m_textRectangle.X + XTextOffset, m_textRectangle.Y + YTextOffset, m_textRectangle.Width, m_textRectangle.Height);
                    // change color of text if mouse over text color is not null
                    if (m_text != null && m_text.Length > 0)
                    {
                        if (!m_disabled && m_hasTextOverColor && m_controlState == Enums.EControlState.Over)
                            BitmapFont.ActiveFonts[Font].TextBox(rect, m_textOverColor, m_text);
                        else
                            BitmapFont.ActiveFonts[Font].TextBox(rect, textColor, m_text);
                    }
                }
                else Utils.LogOnce("BitmapFont.ActiveFonts does not contain the Font [ " + Font + " ] for Label [ " + m_name + " ] of Sheet [ " + GuiManager.CurrentSheet.Name + " ]");
            }

            if (Border != null) Border.Draw(gameTime);
        }
    }
}
