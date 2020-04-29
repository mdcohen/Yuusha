using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class SpellRingWindow : Window
    {
        private bool m_fillerLabelRotateClockwise = true;
        private float m_fillerLabelScale = 1.0f;
        private float m_fillerLabelRotation = 0;
        private Label FillerLabel //TODO: check hybrid's mana and make the ring disappear...
        { get; set; }
        private bool MouseEnteredRing = false;

        public SpellRingWindow(string name, string owner, Rectangle rectangle, bool visible, bool locked, bool disabled, string font,
            VisualKey visualKey, Color tintColor, byte visualAlpha, bool dropShadow, Map.Direction shadowDirection, int shadowDistance,
            List<Enums.EAnchorType> anchors, string cursorOverride) : base(name, owner, rectangle, visible, locked, disabled, font, visualKey, tintColor, visualAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride)
        {
        }

        public static void CreateSpellRingWindow()
        {
            if (GuiManager.GenericSheet["SpellringWindow"] is SpellRingWindow)
            {
                return;
            }

            SpellRingWindow spellring = new SpellRingWindow("SpellringWindow", "", new Rectangle(160, 200, 60, 60), false, false, false, GuiManager.GenericSheet.Font,
                new VisualKey("knightsringtransparent"), Color.White, 255, false, Map.Direction.Northwest, 5, new List<Enums.EAnchorType>() { Enums.EAnchorType.Left, Enums.EAnchorType.Top }, "Dragging")
            {
                PopUpText = "" // "Symbol of the Order"
            };
            GuiManager.GenericSheet.AddControl(spellring);            

            HotButton RingSpell0Button = new HotButton("RingSpell0Button", spellring.Name, new Rectangle(-2, -2, 64, 64), "", false, Color.White,
                true, false, spellring.Font, new VisualKey(Effect.IconsDictionary["Cure"]), Color.White, 255, 0, new VisualKey(""), new VisualKey(""),
                new VisualKey(""), "send_command", BitmapFont.TextAlignment.Center, 0, 0, Color.White, false, Color.PaleGreen, false, new List<Enums.EAnchorType>(),
                false, Map.Direction.None, 0, "cast cure", "Cure");
            GuiManager.GenericSheet.AddControl(RingSpell0Button);
            SquareBorder iconLabelBorder = new SquareBorder("RingSpell0ButtonBorder", RingSpell0Button.Name, 1, new VisualKey("WhiteSpace"), false, Color.White, 255);
            GuiManager.GenericSheet.AddControl(iconLabelBorder);

            HotButton RingSpell1Button = new HotButton("RingSpell1Button", spellring.Name, new Rectangle(-2, -80, 64, 64), "", false, Color.White,
                true, false, spellring.Font, new VisualKey(Effect.IconsDictionary["Light"]), Color.White, 255, 0, new VisualKey(""), new VisualKey(""),
                new VisualKey(""), "send_command", BitmapFont.TextAlignment.Center, 0, 0, Color.White, false, Color.PaleGreen, false, new List<Enums.EAnchorType>(),
                false, Map.Direction.None, 0, "cast light", "Light");
            GuiManager.GenericSheet.AddControl(RingSpell1Button);
            iconLabelBorder = new SquareBorder("RingSpell1ButtonBorder", RingSpell1Button.Name, 1, new VisualKey("WhiteSpace"), false, Color.White, 255);
            GuiManager.GenericSheet.AddControl(iconLabelBorder);

            HotButton RingSpell2Button = new HotButton("RingSpell2Button", spellring.Name, new Rectangle(80, -35, 64, 64), "", false, Color.White,
                true, false, spellring.Font, new VisualKey(Effect.IconsDictionary["Bless"]), Color.White, 255, 0, new VisualKey(""), new VisualKey(""),
                new VisualKey(""), "send_command", BitmapFont.TextAlignment.Center, 0, 0, Color.White, false, Color.PaleGreen, false, new List<Enums.EAnchorType>(),
                false, Map.Direction.None, 0, "cast bless", "Bless");
            GuiManager.GenericSheet.AddControl(RingSpell2Button);
            iconLabelBorder = new SquareBorder("RingSpell2ButtonBorder", RingSpell2Button.Name, 1, new VisualKey("WhiteSpace"), false, Color.White, 255);
            GuiManager.GenericSheet.AddControl(iconLabelBorder);

            HotButton RingSpell3Button = new HotButton("RingSpell3Button", spellring.Name, new Rectangle(67, 50, 64, 64), "", false, Color.White,
                true, false, spellring.Font, new VisualKey(Effect.IconsDictionary["Strength"]), Color.White, 255, 0, new VisualKey(""), new VisualKey(""),
                new VisualKey(""), "send_command", BitmapFont.TextAlignment.Center, 0, 0, Color.White, false, Color.PaleGreen, false, new List<Enums.EAnchorType>(),
                false, Map.Direction.None, 0, "cast strength", "Strength");
            GuiManager.GenericSheet.AddControl(RingSpell3Button);
            iconLabelBorder = new SquareBorder("RingSpell3ButtonBorder", RingSpell3Button.Name, 1, new VisualKey("WhiteSpace"), false, Color.White, 255);
            GuiManager.GenericSheet.AddControl(iconLabelBorder);

            HotButton RingSpell4Button = new HotButton("RingSpell4Button", spellring.Name, new Rectangle(-71, 50, 64, 64), "", false, Color.White,
                true, true, spellring.Font, new VisualKey(Effect.IconsDictionary["Locate Entity"]), Color.White, 255, 0, new VisualKey(""), new VisualKey(""),
                new VisualKey(""), "send_command", BitmapFont.TextAlignment.Center, 0, 0, Color.White, false, Color.PaleGreen, false, new List<Enums.EAnchorType>(),
                false, Map.Direction.None, 0, "cast locate", "Locate Entity");
            GuiManager.GenericSheet.AddControl(RingSpell4Button);
            iconLabelBorder = new SquareBorder("RingSpell4ButtonBorder", RingSpell4Button.Name, 1, new VisualKey("WhiteSpace"), false, Color.White, 255);
            GuiManager.GenericSheet.AddControl(iconLabelBorder);

            HotButton RingSpell5Button = new HotButton("RingSpell5Button", spellring.Name, new Rectangle(-84, -35, 64, 64), "", false, Color.White,
                true, true, spellring.Font, new VisualKey(Effect.IconsDictionary["Summon Lamassu"]), Color.White, 255, 0, new VisualKey(""), new VisualKey(""),
                new VisualKey(""), "send_command", BitmapFont.TextAlignment.Center, 0, 0, Color.White, false, Color.PaleGreen, false, new List<Enums.EAnchorType>(),
                false, Map.Direction.None, 0, "cast summonlamassu", "Summon Lamassu");
            GuiManager.GenericSheet.AddControl(RingSpell5Button);
            iconLabelBorder = new SquareBorder("RingSpell5ButtonBorder", RingSpell5Button.Name, 1, new VisualKey("WhiteSpace"), false, Color.White, 255);
            GuiManager.GenericSheet.AddControl(iconLabelBorder);

            Label SpellringFillerLabel = new Label("SpellringFillerLabel", spellring.Name, new Rectangle(-80, -80, 212, 212),
                    "", Color.Black, true, false, spellring.Font, new VisualKey(SpellWarmingWindow.SpellRotatingVisualKey), Color.White, 0, 255, BitmapFont.TextAlignment.Center,
                    0, 0, "", "", new List<Enums.EAnchorType>(), "");
            spellring.FillerLabel = SpellringFillerLabel;
            GuiManager.GenericSheet.AddControl(SpellringFillerLabel);

            spellring.SetRingProfession(Character.ClassType.Ravager);
        }

        public override void Update(GameTime gameTime)
        {
            if (Character.CurrentCharacter == null || !Character.CurrentCharacter.knightRing && !Character.HasEffect("Knight Ring") && !Character.HasEffect("Sacred Ring"))
                IsVisible = false;

            ZDepth = 1;

            if (m_fillerLabelRotateClockwise)
                m_fillerLabelRotation += .01f;
            else m_fillerLabelRotation -= .01f;

            bool HideIcons = true;

            if (MouseEnteredRing)
            {
                m_fillerLabelScale = .7f;

                foreach (Control c in Controls)
                {
                    if (c.Contains(GuiManager.MouseState.Position))
                        HideIcons = false;
                }

                if (FillerLabel.Contains(GuiManager.MouseState.Position))
                    HideIcons = false;

                if (HideIcons)
                    MouseEnteredRing = false;
            }

            if (Contains(GuiManager.MouseState.Position))
                HideIcons = false;

            if (!GuiManager.KeyboardState.IsKeyDown(Keys.LeftAlt) && !GuiManager.KeyboardState.IsKeyDown(Keys.RightAlt))
            {
                if (GuiManager.DraggedControl == this)
                    GuiManager.StopDragging();
            }

            if (GuiManager.DraggedControl == this || (GuiManager.KeyboardState.IsKeyDown(Keys.LeftAlt) || GuiManager.KeyboardState.IsKeyDown(Keys.RightAlt)))
            {
                HideIcons = true;
            }

            foreach (Control c in Controls)
                if(c is HotButton) c.IsVisible = !HideIcons;

            if (HideIcons)
                m_fillerLabelScale = .3f;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if(IsVisible && FillerLabel != null && FillerLabel.IsVisible)
            {
                VisualInfo vi = GuiManager.Visuals[FillerLabel.VisualKey];
                Color color = new Color(FillerLabel.TintColor, 255);

                Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], new Vector2(Position.X + Width / 2, Position.Y + Height / 2),
                    vi.Rectangle, color, m_fillerLabelRotation, new Vector2(vi.Width / 2, vi.Height / 2), m_fillerLabelScale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 1);
            }

            base.Draw(gameTime);
        }

        protected override void OnMouseOver(MouseState ms)
        {
            base.OnMouseOver(ms);
            
            if(!GuiManager.KeyboardState.IsKeyDown(Keys.LeftAlt) && !GuiManager.KeyboardState.IsKeyDown(Keys.RightAlt))
                MouseEnteredRing = true;
        }

        protected override void OnMouseLeave(MouseState ms)
        {
            base.OnMouseLeave(ms);

            if (GuiManager.DraggedControl == this)
                GuiManager.StopDragging();
        }

        protected override void OnMouseDown(MouseState ms)
        {
            foreach (Control c in Controls)
            {
                if (c is HotButton hb && c.IsVisible && !c.IsDisabled && c.Contains(GuiManager.MouseState.Position))
                {
                    hb.ForceClick();
                }
            }

            base.OnMouseDown(ms);
        }

        public void SetRingProfession(Character.ClassType profession)
        {
            switch(profession)
            {
                case Character.ClassType.Knight:
                    //PopUpText = "Symbol of the Order";
                    m_fillerLabelRotateClockwise = true;
                    VisualKey = "knightsringtransparent";
                    FillerLabel.TintColor = Color.White;
                    this["RingSpell0Button"].VisualKey = Effect.IconsDictionary["Cure"];
                    this["RingSpell0Button"].Command = "cast cure";
                    this["RingSpell0Button"].PopUpText = "Cure";
                    this["RingSpell1Button"].VisualKey = Effect.IconsDictionary["Light"];
                    this["RingSpell1Button"].Command = "cast light";
                    this["RingSpell1Button"].PopUpText = "Light";
                    this["RingSpell2Button"].VisualKey = Effect.IconsDictionary["Blessing of the Faithful"];
                    this["RingSpell2Button"].Command = "cast Bless";
                    this["RingSpell2Button"].PopUpText = "Blessing of the Faithful";
                    this["RingSpell3Button"].VisualKey = Effect.IconsDictionary["Strength"];
                    this["RingSpell3Button"].Command = "cast strength";
                    this["RingSpell3Button"].PopUpText = "Strength";
                    this["RingSpell4Button"].VisualKey = Effect.IconsDictionary["Locate Entity"];
                    this["RingSpell4Button"].Command = "cast locate";
                    this["RingSpell4Button"].PopUpText = "Locate Entity";
                    this["RingSpell5Button"].VisualKey = Effect.IconsDictionary["Summon Lamassu"];
                    this["RingSpell5Button"].Command = "cast summonlamassu";
                    this["RingSpell5Button"].PopUpText = "Summon Lamassu";
                    break;
                case Character.ClassType.Ravager:
                    //PopUpText = "Ring of Nergal";
                    m_fillerLabelRotateClockwise = false;
                    VisualKey = "ravagersringtransparent";
                    FillerLabel.TintColor = Color.Crimson;
                    this["RingSpell0Button"].VisualKey = Effect.IconsDictionary["Lifeleech"];
                    this["RingSpell0Button"].Command = "cast lifeleech %t";
                    this["RingSpell0Button"].PopUpText = "Lifeleech";
                    this["RingSpell1Button"].VisualKey = Effect.IconsDictionary["Flame Shield"];
                    this["RingSpell1Button"].Command = "cast flameshield";
                    this["RingSpell1Button"].PopUpText = "Flame Shield";
                    this["RingSpell2Button"].VisualKey = Effect.IconsDictionary["Minor Protection from Fire"];
                    this["RingSpell2Button"].Command = "cast mprfire";
                    this["RingSpell2Button"].PopUpText = "Minor Protection from Fire";
                    this["RingSpell3Button"].VisualKey = Effect.IconsDictionary["Strength"];
                    this["RingSpell3Button"].Command = "cast strength";
                    this["RingSpell3Button"].PopUpText = "Strength";
                    this["RingSpell4Button"].VisualKey = Effect.IconsDictionary["Locate Entity"];
                    this["RingSpell4Button"].Command = "cast locate";
                    this["RingSpell4Button"].PopUpText = "Locate Entity";
                    this["RingSpell5Button"].VisualKey = Effect.IconsDictionary["Summon Hellhound"];
                    this["RingSpell5Button"].Command = "cast summonhellhound";
                    this["RingSpell5Button"].PopUpText = "Summon Hellhound";
                    break;
            }
        }
    }
}
