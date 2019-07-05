using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class SpellEffectLabel : Label
    {
        public const int BuffFadeSpeed = 2; // slower
        public const int DefaultFadeSpeed = 3;
        public const int DamageFadeSpeed = 4; // faster
        public static readonly Color BuffBorderColor = Color.LightGoldenrodYellow;
        public static readonly Color DamageBorderColor = Color.Red;
        public static readonly Color HealBorderColor = Color.Lime;

        public bool IsShrinking
        { get; set; } = true;

        public int FadeSpeed
        { get; set; } = DefaultFadeSpeed;

        public SpellEffectLabel(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string onDoubleClickEvent,
            string cursorOverride, System.Collections.Generic.List<Enums.EAnchorType> anchors, string popUpText) : base(name, owner, rectangle, text,
                textColor, visible, disabled, font, visualKey, tintColor, visualAlpha, textAlpha, textAlignment,
                xTextOffset, yTextOffset, onDoubleClickEvent, cursorOverride, anchors, popUpText)
        {
        }

        /// <summary>
        /// Create a SpellEffectLabel without a border, and isn't shrinking.
        /// </summary>
        /// <param name="effectName"></param>
        public static void CreateSpellEffectLabel(string effectName)
        {
            if(!Effect.IconsDictionary.ContainsKey(effectName))
            {
                Utils.Log("No SpellEffectLabel visual key for [ " + effectName + " ]");
                return;
            }

            if (!Client.ClientSettings.DisplaySpellEffectLabels)
                return;

            int size = 600;
            int x = Client.Width / 2 - (size / 2);
            int y = Client.Height / 2 - (size / 2);

            if (GuiManager.GetControl("MapDisplayWindow") is Window mapWindow)
            {
                x = mapWindow.Position.X;
                y = mapWindow.Position.Y;
                size = mapWindow.Width;
            }

            string text = "";

            if (Client.ClientSettings.DisplaySpellEffectNameOnLabels)
                text = effectName;

            SpellEffectLabel label = new SpellEffectLabel(effectName + "_" + Program.Client.ClientGameTime + "_SpellEffectLabel", "",
                new Rectangle(x, y, size, size), text, Color.White, true, false, "changaone16", new VisualKey(Effect.IconsDictionary[effectName]), Color.White,
                255, 0, BitmapFont.TextAlignment.Center, 0, 0, "", "", new System.Collections.Generic.List<Enums.EAnchorType>() { Enums.EAnchorType.Center }, "")
            {
                IsShrinking = false // still fades out
            };

            GuiManager.CurrentSheet.AddControl(label);
        }

        public static void CreateSpellEffectLabel(string effectName, Color borderColor)
        {
            if (!Effect.IconsDictionary.ContainsKey(effectName))
            {
                Utils.Log("No SpellEffectLabel visual key for [ " + effectName + " ]");
                return;
            }

            if (!Client.ClientSettings.DisplaySpellEffectLabels)
                return;

            int size = 600;
            int x = Client.Width / 2;
            int y = Client.Height / 2;

            if(GuiManager.GetControl("Tile24") is SpinelTileLabel spinelLabel)
            {
                x = spinelLabel.Position.X + spinelLabel.Width / 2;
                y = spinelLabel.Position.Y + spinelLabel.Height / 2;
            }

            x -= size / 2;
            y -= size / 2;

            string text = "";

            if (Client.ClientSettings.DisplaySpellEffectNameOnLabels)
                text = effectName;

            Color textColor = borderColor;

            SpellEffectLabel label = new SpellEffectLabel(effectName + "_" + Program.Client.ClientGameTime + "_SpellEffectLabel", "",
                new Rectangle(x, y, size, size), text, textColor, true, false, "changaone16", new VisualKey(Effect.IconsDictionary[effectName]), Color.White,
                255, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new System.Collections.Generic.List<Enums.EAnchorType>() { Enums.EAnchorType.Center }, "");
            SquareBorder border = new SquareBorder(label.Name + "Border", label.Name, 3, new gui.VisualKey("WhiteSpace"), false, borderColor, 255);

            if (borderColor == BuffBorderColor)
                label.FadeSpeed = BuffFadeSpeed;
            else if (borderColor == DamageBorderColor)
                label.FadeSpeed = DamageFadeSpeed;

            GuiManager.CurrentSheet.AddControl(label);
            GuiManager.CurrentSheet.AddControl(border);
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsVisible)
            {
                GuiManager.CurrentSheet.RemoveControl(this);
                return;
            }

            base.Update(gameTime);

            ZDepth = 1;

            if (VisualAlpha > 201) VisualAlpha = 201;

            VisualAlpha -= FadeSpeed;
            TextAlpha = VisualAlpha + FadeSpeed; // Text, if displayed, fades slower

            if (Border != null)
            {
                Border.VisualAlpha = VisualAlpha - (FadeSpeed * 2); // Border fades faster
                Border.Update(gameTime);
            }

            if (VisualAlpha <= 0)
            {
                VisualAlpha = 0;
                IsVisible = false;
                IsDisabled = true;
            }

            if (IsVisible)
            {
                if (IsShrinking)
                {
                    Position = new Point(Position.X + 5, Position.Y + 5);
                    Width = Width - 10;
                    Height = Height - 10;

                    if (Border != null && (Border.TintColor == new Color(DamageBorderColor, Border.VisualAlpha)
                        || Border.TintColor == new Color(HealBorderColor, Border.VisualAlpha)))
                    {
                        Position = new Point(Position.X, Position.Y - 3);
                    }

                    if (Text != "")
                    {
                        if (BitmapFont.ActiveFonts[Font].MeasureString(Text) > Width)
                            Text = "";
                    }

                    if (Width <= 0 || Height <= 0)
                        IsVisible = false;
                }
            }
            
            if(!IsVisible)
                GuiManager.CurrentSheet.RemoveControl(this);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (Border != null) Border.Draw(gameTime);
        }

        public override bool MouseHandler(MouseState ms)
        {
            return false;
        }

        public override bool KeyboardHandler(KeyboardState ks)
        {
            return false;
        }
    }
}
