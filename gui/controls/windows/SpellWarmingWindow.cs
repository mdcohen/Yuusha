using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Yuusha.gui
{
    public class SpellWarmingWindow : Window
    {
        private float circleRotation = 0;
        private DateTime timeCreated;
        private bool spellWarmed = false;
        private DateTime timeWarmed;
        public Label SpellIconLabel
        { get; set; }
        private PercentageBarLabel PercentageBar
        { get; set; }

        public SpellWarmingWindow(string name, string owner, Rectangle rectangle, bool visible, bool locked, bool disabled, string font,
            VisualKey visualKey, Color tintColor, byte visualAlpha, bool dropShadow, Map.Direction shadowDirection, int shadowDistance,
            List<Enums.EAnchorType> anchors, string cursorOverride) : base(name, owner, rectangle, visible, locked, disabled, font, visualKey, tintColor, visualAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride)
        {
        }

        public static void CreateSpellWarmingWindow(string spellName)
        {
            // SpellWarmingWindow exists, if this is called then another spell is being warmed.
            if (GuiManager.GenericSheet["SpellWarmingWindow"] is SpellWarmingWindow existingWindow)
                existingWindow.OnClose();

            int x = Client.Width - 200;
            int y = 200;

            if(GuiManager.GetControl("MapDisplayWindow") is Window mapWindow)
            {
                x = mapWindow.Position.X + mapWindow.Width + 35;
                y = mapWindow.Position.Y + 30;
            }

            SpellWarmingWindow w = new SpellWarmingWindow("SpellWarmingWindow", "", new Rectangle(x, y, 74, 74), true, false, false,
                "lemon12", new VisualKey("WhiteSpace"), Color.DimGray, 0, true, Map.Direction.Northwest, 5, new List<Enums.EAnchorType>() { Enums.EAnchorType.Center }, "Dragging");
            GuiManager.GenericSheet.AddControl(w);

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
                new List<Enums.EAnchorType>(), "", false);
            warmingTimePercentageBarLabel.Percentage = 100;
            warmingTimePercentageBarLabel.Segmented = false;
            warmingTimePercentageBarLabel.MidLabel = new Label(warmingTimePercentageBarLabel.Name + "MidLabel", warmingTimePercentageBarLabel.Name,
                new Rectangle(warmingTimePercentageBarLabel.Position.X, warmingTimePercentageBarLabel.Position.Y, 0, warmingTimePercentageBarLabel.Height), "", Color.White,
                true, false, warmingTimePercentageBarLabel.Font, new VisualKey("WhiteSpace"), Color.RoyalBlue, 255, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>(), "");
            w.PercentageBar = warmingTimePercentageBarLabel;

            GuiManager.SpellWarmingWindow = w;
            GuiManager.GenericSheet.AddControl(spellIconLabel);
            GuiManager.GenericSheet.AddControl(spellIconBorder);
            GuiManager.GenericSheet.AddControl(warmingTimePercentageBarLabel);
            //Audio.AudioManager.PlaySoundEffect("KSND0080");
            w.timeCreated = DateTime.Now;
        }

        public override void Update(GameTime gameTime)
        {
            if (Character.CurrentCharacter != null && (int)Character.CurrentCharacter.Alignment <= 2)
                circleRotation += .01f;
            else circleRotation -= .01f;

            if (!spellWarmed)
            {
                SpellIconLabel.VisualAlpha += 2;
                if (SpellIconLabel.Border != null)
                    SpellIconLabel.Border.VisualAlpha = SpellIconLabel.VisualAlpha;
            }
            else if((DateTime.Now - timeWarmed).TotalSeconds > .5)
            {
                SpellIconLabel.VisualAlpha -= 3;
                SpellIconLabel.TextAlpha = SpellIconLabel.VisualAlpha;
                if (SpellIconLabel.Border != null)
                {
                    //SpellIconLabel.Border.IsVisible = !SpellIconLabel.Border.IsVisible;
                    SpellIconLabel.Border.VisualAlpha = SpellIconLabel.VisualAlpha;
                }

                //if((timeWarmed - timeCreated).TotalSeconds % 2 == 0)
                //    SpellIconLabel.IsVisible = !SpellIconLabel.IsVisible;

                if (SpellIconLabel.VisualAlpha <= 0 || (DateTime.Now - timeCreated).TotalSeconds >= Utility.Settings.StaticSettings.RoundDelayLength * 2 / 2)
                {
                    OnClose();
                    return;
                }
            }

            //SpellIconLabel.PopUpText = (DateTime.Now - timeCreated).TotalSeconds.ToString();

            if (!spellWarmed)
            {
                PercentageBar.Percentage = (DateTime.Now - timeCreated).TotalMilliseconds / Utility.Settings.StaticSettings.RoundDelayLength * 100;
                if (PercentageBar.Percentage >= 100)
                {
                    //SpellIconLabel.TextColor = Color.Yellow;
                    //SpellIconLabel.Text = "WARMED";
                    PercentageBar.IsVisible = false;
                    spellWarmed = true;
                    SpellIconLabel.VisualAlpha = 255;
                    timeWarmed = DateTime.Now;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!IsVisible)
                return;

            // Draw a rotating sprite around the outside of the window.
            VisualInfo vi = GuiManager.Visuals["SmokeCircle"];
            Color color = new Color(Color.White, SpellIconLabel.VisualAlpha);

            if (Character.CurrentCharacter != null)
            {
                switch (Character.CurrentCharacter.Alignment)
                {
                    case World.Alignment.Amoral:
                        color = new Color(Client.ClientSettings.Color_Gui_Amoral_Back, SpellIconLabel.VisualAlpha);
                        break;
                    case World.Alignment.Chaotic:
                        color = new Color(Client.ClientSettings.Color_Gui_Chaotic_Back, SpellIconLabel.VisualAlpha);
                        break;
                    case World.Alignment.ChaoticEvil:
                        color = new Color(Client.ClientSettings.Color_Gui_ChaoticEvil_Back, SpellIconLabel.VisualAlpha);
                        break;
                    case World.Alignment.Evil:
                        color = new Color(Client.ClientSettings.Color_Gui_Evil_Back, SpellIconLabel.VisualAlpha);
                        break;
                    case World.Alignment.Lawful:
                        color = new Color(Client.ClientSettings.Color_Gui_Lawful_Fore, SpellIconLabel.VisualAlpha);
                        break;
                    case World.Alignment.Neutral:
                        color = new Color(Client.ClientSettings.Color_Gui_Neutral_Back, SpellIconLabel.VisualAlpha);
                        break;
                }
            }

            Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], new Vector2(SpellIconLabel.Position.X + SpellIconLabel.Width / 2, SpellIconLabel.Position.Y + SpellIconLabel.Height / 2),
                    vi.Rectangle, color, circleRotation, new Vector2(vi.Width / 2, vi.Height / 2), .78f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 1);

            base.Draw(gameTime);            
        }

        public override void OnClose()
        {
            base.OnClose();

            if (GuiManager.SpellWarmingWindow == this)
                GuiManager.SpellWarmingWindow = null;

            GuiManager.RemoveControl(this);
        }
    }
}
