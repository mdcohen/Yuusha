using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Yuusha.gui
{
    public class SpellEffectLabel : Label
    {
        public static Dictionary<string, string> IconsTintDictionary = new Dictionary<string, string>()
        {

        };

        public const int BuffFadeSpeed = 2; // slower
        public const int DefaultFadeSpeed = 2;
        public const int DamageFadeSpeed = 3; // faster
        public static readonly Color BuffBorderColor = Color.LightGoldenrodYellow;
        public static readonly Color DamageBorderColor = Color.Red;
        public static readonly Color HealBorderColor = Color.Lime;

        public bool IsShrinking
        { get; set; } = true;

        public int FadeSpeed
        { get; set; } = DefaultFadeSpeed;

        public bool IsFading
        { get; set; } = true;

        public SpellEffectLabel(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string onDoubleClickEvent,
            string cursorOverride, List<Enums.EAnchorType> anchors, string popUpText) : base(name, owner, rectangle, text,
                textColor, visible, disabled, font, visualKey, tintColor, visualAlpha, textAlpha, textAlignment,
                xTextOffset, yTextOffset, onDoubleClickEvent, cursorOverride, anchors, popUpText)
        {
        }

        /// <summary>
        /// Create a SpellEffectLabel without a border and isn't shrinking. It shows up center screen then fades away.
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

            Color tintColor = Color.White;
            if (Effect.IconsTintDictionary.ContainsKey(effectName))
                tintColor = Effect.IconsTintDictionary[effectName];

            SpellEffectLabel label = new SpellEffectLabel(effectName + "_" + Program.Client.ClientGameTime.ElapsedGameTime.ToString() + "_SpellEffectLabel", "",
                new Rectangle(x, y, size, size), text, Color.White, true, false, "changaone16", new VisualKey(Effect.IconsDictionary[effectName]), tintColor,
                255, 0, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>() { Enums.EAnchorType.Center }, "")
            {
                IsFading = true,
                IsShrinking = false // still fades out
            };

            GuiManager.CurrentSheet.AddControl(label);
        }

        /// <summary>
        /// Create a SpellEffectLabel with a border. Generally this is used for when the player is cast upon. This label will fade out or appear differently than a standard SpellEffectLabel.
        /// </summary>
        /// <param name="effectName"></param>
        /// <param name="borderColor"></param>
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

            Color tintColor = Color.White;
            if (Effect.IconsTintDictionary.ContainsKey(effectName))
                tintColor = Effect.IconsTintDictionary[effectName];

            SpellEffectLabel label = new SpellEffectLabel(effectName + "_" + Program.Client.ClientGameTime.ElapsedGameTime.TotalMilliseconds + "_SpellEffectLabel", "",
                new Rectangle(x, y, size, size), text, borderColor, true, false, "changaone16", new VisualKey(Effect.IconsDictionary[effectName]), tintColor,
                255, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>() { Enums.EAnchorType.Center }, "");
            SquareBorder border = new SquareBorder(label.Name + "Border", label.Name, 3, new VisualKey("WhiteSpace"), false, borderColor, 255);

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
                GuiManager.Dispose(this);//.CurrentSheet.RemoveControl(this);
                return;
            }

            base.Update(gameTime);

            ZDepth = 1;

            if (IsFading)
            {
                if (VisualAlpha > 201) VisualAlpha = 201;

                VisualAlpha -= FadeSpeed;
                TextAlpha = VisualAlpha + FadeSpeed; // Text, if displayed, fades slower.

                if (Border != null)
                    Border.VisualAlpha = VisualAlpha - (FadeSpeed * 2); // Border fades faster.
            }

            if (Border != null) Border.Update(gameTime);

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

                    // Damage and heal labels slowly move upward.
                    if (Border != null && (Border.TintColor == new Color(DamageBorderColor, Border.VisualAlpha)
                        || Border.TintColor == new Color(HealBorderColor, Border.VisualAlpha)))
                    {
                        Position = new Point(Position.X, Position.Y - 2);

                        // Stop shrinking and fading at 50 x 50.
                        //if (Width <= 60 || Height <= 60)
                        //{
                        //    Width = 50; Height = 50;
                        //    IsShrinking = false;
                        //    IsFading = false;
                        //}
                    }

                    // Text disappears if it no longer fits inside the label.
                    if (!string.IsNullOrEmpty(Text))
                    {
                        if (BitmapFont.ActiveFonts[Font].MeasureString(Text) > Width)
                            Text = "";
                    }

                    if (Width <= 0 || Height <= 0)
                        IsVisible = false;
                }
                else
                {
                    // When damage and heal stop shrinking they still move upward.
                    if (Border != null && (Border.TintColor == new Color(DamageBorderColor, Border.VisualAlpha)
                            || Border.TintColor == new Color(HealBorderColor, Border.VisualAlpha)))
                    {
                        // Keep moving up slowly. Slower
                        Position = new Point(Position.X, Position.Y - 2);

                        // Remove the control after it disappears off screen.
                        if (Position.Y + Height <= 0)
                            IsVisible = false;
                    }
                }
            }

            if (!IsVisible)
                GuiManager.Dispose(this);
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
