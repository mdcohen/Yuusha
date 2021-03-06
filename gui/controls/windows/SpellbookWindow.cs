﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class SpellBookWindow : Window
    {
        public TextBox LeftChantTextBox = null;
        public Spell LeftPageSpell = null;
        public TextBox RightChantTextBox = null;
        public Spell RightPageSpell = null;
        public int PageSet = 1;
        private bool ChantsFading = true;

        public SpellBookWindow(string name, string owner, Rectangle rectangle, bool visible, bool locked, bool disabled, string font,
            VisualKey visualKey, Color tintColor, byte visualAlpha, bool dropShadow, Map.Direction shadowDirection, int shadowDistance,
            List<Enums.EAnchorType> anchors, string cursorOverride) : base(name, owner, rectangle, visible, locked, disabled, font, visualKey, tintColor, visualAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride)
        {
        }

        public static void CreateSpellBookWindow()
        {
            if (GuiManager.GenericSheet["SpellbookWindow"] is SpellBookWindow w)
            {
                //w.InitSpellbook();
                //w.IsVisible = true;
                return;
            }

            SpellBookWindow spellbook = new SpellBookWindow("SpellbookWindow", "", new Rectangle(5, 40, 1013, 647), false, false, false, GuiManager.GenericSheet.Font,
                new VisualKey("SpellbookBackground"), Color.White, 255, true, Map.Direction.Northwest, 10, new List<Enums.EAnchorType>() { Enums.EAnchorType.Center }, "Dragging")
            {
                IsVisible = false
            };
            GuiManager.GenericSheet.AddControl(spellbook);

            WindowTitle wTitle = new WindowTitle(spellbook.Name + "Title", spellbook.Name, spellbook.Font, "", Color.PaleGreen, Color.Transparent,
                0, BitmapFont.TextAlignment.Center, new VisualKey("WhiteSpace"), false,
                new VisualKey("WindowCloseBox"), new VisualKey("WindowCloseBoxDown"), 25, 10, 20, 20, Color.RosyBrown, 24)
            {
                Width = spellbook.Width
            };
            GuiManager.GenericSheet.AddControl(wTitle);

            #region Left Page
            // Spell Name
            Label SpellbookLeftPageSpellNameLabel = new Label("SpellbookLeftPageSpellNameLabel", spellbook.Name, new Rectangle(80, 56, 405, 30),
                    "PROTECTION FROM STUN AND DEATH", Color.Black, true, false, "lemon14", new VisualKey(""), Color.Transparent, 0, 255, BitmapFont.TextAlignment.Center,
                    0, 0, "", "", new List<Enums.EAnchorType>(), "");
            GuiManager.GenericSheet.AddControl(SpellbookLeftPageSpellNameLabel);
            // Spell Icon
            HotButton SpellbookLeftPageSpellHotButton = new HotButton("SpellbookLeftPageSpellHotButton", spellbook.Name, new Rectangle(217, 100, 128, 128),
                "", false, Color.Black, true, false, "lemon14", new VisualKey("hotbuttonicon_471"), Color.White, 255, 255, new VisualKey(""), new VisualKey(""),
                new VisualKey(""), "send_command", BitmapFont.TextAlignment.Center, 0, 0, Color.White, false, Color.White, false, new List<Enums.EAnchorType>(), false, Map.Direction.None, 0, "", "")
            {
                IsLocked = false  // allows for dragging a label copy to make a hotbutton copy
            };
            GuiManager.GenericSheet.AddControl(SpellbookLeftPageSpellHotButton);
            // Spell Icon Border
            SquareBorder iconLabelBorder = new SquareBorder("SpellbookLeftPageSpellIconLabelBorder", SpellbookLeftPageSpellHotButton.Name, 1, new VisualKey("WhiteSpace"), false, Color.White, 255);
            GuiManager.GenericSheet.AddControl(iconLabelBorder);
            // Level Symbol
            Label SpellbookLeftLevelSymbolLabel = new Label("SpellbookLeftLevelSymbolLabel", spellbook.Name, new Rectangle(125, 134, 50, 50),
                "19", Color.White, true, false, "changaone20", new VisualKey("LevelUpSymbol"), Color.White, 255, 255, BitmapFont.TextAlignment.Center,
                0, 0, "", "", new List<Enums.EAnchorType>(), "");
            GuiManager.GenericSheet.AddControl(SpellbookLeftLevelSymbolLabel);
            // Mana Symbol
            Label SpellbookLeftManaSymbolLabel = new Label("SpellbookLeftManaSymbolLabel", spellbook.Name, new Rectangle(380, 134, 50, 50),
                "34", Color.White, true, false, "changaone20", new VisualKey("ManaSymbol"), Color.White, 255, 255, BitmapFont.TextAlignment.Center,
                0, 0, "", "", new List<Enums.EAnchorType>(), "");
            GuiManager.GenericSheet.AddControl(SpellbookLeftManaSymbolLabel);
            // Text Boxes
            int x = 90, y = 240, width = 405, height = 25;
            int seed = Guid.NewGuid().GetHashCode();
            for (int i = 0; i <= 13; i++)
            {
                seed = Guid.NewGuid().GetHashCode();
                TextBox textBox = new TextBox(spellbook.Name + "LeftChant" + i + "TextBox", spellbook.Name, new Rectangle(x, y, width, height), TextManager.GenerateMagicWords(new Random(seed).Next(2, 4)).ToUpper(), Color.Black,
                    BitmapFont.TextAlignment.Center, true, true, "lemon12", new VisualKey(""), Color.Transparent, 0, 255, false, 100, false, false, Color.Brown,
                     new VisualKey(""), new VisualKey(""), new VisualKey(""), 0, 0, "", Color.Brown, new List<Enums.EAnchorType>(), 0);
                y += height;
                GuiManager.GenericSheet.AddControl(textBox);
            }
            // Left Flip Button
            Button SpellbookLeftBackFlipButton = new Button("SpellbookLeftBackFlipButton", spellbook.Name, new Rectangle(30, 27, 30, spellbook.Height - 53), "", false, Color.White,
                true, false, spellbook.Font, new VisualKey("WhiteSpace"), Color.Transparent, 30, 0, new VisualKey(""), new VisualKey(""), new VisualKey(""), "Spellbook_Flip", BitmapFont.TextAlignment.Left,
                0, 0, Color.White, false, Color.Yellow, true, new List<Enums.EAnchorType>(), false, Map.Direction.None, 0, "", "", "");
            GuiManager.GenericSheet.AddControl(SpellbookLeftBackFlipButton);
            #endregion

            #region Right Page
            Label SpellbookRightPageSpellNameLabel = new Label("SpellbookRightPageSpellNameLabel", spellbook.Name, new Rectangle(530, 56, 405, 30),
                    "SUMMON PHANTASM", Color.Black, true, false, "lemon14", new VisualKey(""), Color.Transparent, 0, 255, BitmapFont.TextAlignment.Center,
                    0, 0, "", "", new List<Enums.EAnchorType>(), "");
            GuiManager.GenericSheet.AddControl(SpellbookRightPageSpellNameLabel);

            HotButton SpellbookRightPageSpellHotButton = new HotButton("SpellbookRightPageSpellHotButton", spellbook.Name, new Rectangle(667, 100, 128, 128),
                "", false, Color.Black, true, false, "lemon14", new VisualKey("hotbuttonicon_280"), Color.White, 255, 255, new VisualKey(""), new VisualKey(""),
                new VisualKey(""), "send_command", BitmapFont.TextAlignment.Center, 0, 0, Color.White, false, Color.White, false, new List<Enums.EAnchorType>(), false, Map.Direction.None,
                0, "", "")
            {
                IsLocked = false // allows for dragging a label copy to make a hotbutton copy
            };
            GuiManager.GenericSheet.AddControl(SpellbookRightPageSpellHotButton);

            SquareBorder iconRightLabelBorder = new SquareBorder("SpellbookRightPageSpellIconLabelBorder", SpellbookRightPageSpellHotButton.Name, 1, new gui.VisualKey("WhiteSpace"), false, Color.White, 255);
            GuiManager.GenericSheet.AddControl(iconRightLabelBorder);

            Label SpellbookRightLevelSymbolLabel = new Label("SpellbookRightLevelSymbolLabel", spellbook.Name, new Rectangle(574, 134, 50, 50),
                "11", Color.White, true, false, "changaone20", new VisualKey("LevelUpSymbol"), Color.White, 255, 255, BitmapFont.TextAlignment.Center,
                0, 0, "", "", new List<Enums.EAnchorType>(), "");
            GuiManager.GenericSheet.AddControl(SpellbookRightLevelSymbolLabel);

            Label SpellbookRightManaSymbolLabel = new Label("SpellbookRightManaSymbolLabel", spellbook.Name, new Rectangle(830, 134, 50, 50),
                "30", Color.White, true, false, "changaone20", new VisualKey("ManaSymbol"), Color.White, 255, 255, BitmapFont.TextAlignment.Center,
                0, 0, "", "", new List<Enums.EAnchorType>(), "");
            GuiManager.GenericSheet.AddControl(SpellbookRightManaSymbolLabel);
            // Text Boxes
            x = 540; y = 240; width = 405; height = 25;
            for (int i = 0; i <= 13; i++)
            {
                seed = Guid.NewGuid().GetHashCode();
                TextBox textBox = new TextBox(spellbook.Name + "RightChant" + i + "TextBox", spellbook.Name, new Rectangle(x, y, width, height), TextManager.GenerateMagicWords(new Random(seed).Next(2, 4)).ToUpper(), Color.Black,
                    BitmapFont.TextAlignment.Center, true, true, "lemon12", new VisualKey(""), Color.Transparent, 0, 255, false, 100, false, false, Color.Brown,
                     new VisualKey(""), new VisualKey(""), new VisualKey(""), 0, 0, "", Color.Brown, new List<Enums.EAnchorType>(), 0);
                y += height;
                GuiManager.GenericSheet.AddControl(textBox);
            }
            // Right Flip Button
            Button SpellbookRightForwardFlipButton = new Button("SpellbookRightForwardFlipButton", spellbook.Name, new Rectangle(spellbook.Width - 60, 25, 30, spellbook.Height - 50), "", false, Color.White,
                true, false, spellbook.Font, new VisualKey("WhiteSpace"), Color.Transparent, 30, 0, new VisualKey(""), new VisualKey(""), new VisualKey(""), "Spellbook_Flip", BitmapFont.TextAlignment.Left,
                0, 0, Color.White, false, Color.Yellow, true, new List<Enums.EAnchorType>(), false, Map.Direction.None, 0, "", "", "");
            GuiManager.GenericSheet.AddControl(SpellbookRightForwardFlipButton);
            #endregion
        }

        public void UpdateVisiblePages(int indexLeftSpell, int indexRightSpell)
        {
            try
            {
                if (Character.CurrentCharacter != null && Character.CurrentCharacter.Spells != null && Character.CurrentCharacter.Spells.Count > 0)
                {
                    if (Character.CurrentCharacter.Spells.Count >= indexLeftSpell + 1)
                        ScribePage(Character.CurrentCharacter.Spells[indexLeftSpell], true);
                    else ErasePage(true);

                    if (Character.CurrentCharacter.Spells.Count >= indexRightSpell + 1)
                        ScribePage(Character.CurrentCharacter.Spells[indexRightSpell], false);
                    else ErasePage(false);
                }
            }
            catch(Exception e)
            {
                Utils.LogException(e);
            }
        }

        public void ScribePage(Spell spell, bool leftPage)
        {
            string leftOrRight = leftPage ? "Left" : "Right";

            if (leftPage)
                LeftPageSpell = spell;
            else RightPageSpell = spell;

            // Spell Name
            this["Spellbook" + leftOrRight + "PageSpellNameLabel"].Text = spell.Name.ToUpper();
            // Spell Icon
            if (Effect.IconsDictionary.ContainsKey(spell.Name))
            {
                this["Spellbook" + leftOrRight + "PageSpellHotButton"].VisualKey = Effect.IconsDictionary[spell.Name];
                //(this["Spellbook" + leftOrRight + "PageSpellHotButton"] as HotButton).
                (this["Spellbook" + leftOrRight + "PageSpellHotButton"] as HotButton).Command = spell.Incantation;
            }
            else
            {
                this["Spellbook" + leftOrRight + "PageSpellHotButton"].VisualKey = "question_mark";
                (this["Spellbook" + leftOrRight + "PageSpellHotButton"] as HotButton).Command = "";
            }
            // Level Symbol Text
            this["Spellbook" + leftOrRight + "LevelSymbolLabel"].Text = spell.RequiredSkillLevel.ToString();
            // Mana Symbol Text
            this["Spellbook" + leftOrRight + "ManaSymbolLabel"].Text = spell.ManaCost.ToString();

            // Choose text box to display spell chant
            int randomTextBox = new Random(Guid.NewGuid().GetHashCode()).Next(0, 13);

            for (int i = 0; i <= 13; i++)
            {
                TextBox tbxBox = this[Name + leftOrRight + "Chant" + i + "TextBox"] as TextBox;
                if (i == randomTextBox)
                {
                    tbxBox.Text = spell.Incantation;
                    tbxBox.TextColor = Color.Gold;
                    tbxBox.TextAlpha = 255;
                    if (leftPage)
                        LeftChantTextBox = tbxBox;
                    else RightChantTextBox = tbxBox;
                }
                else
                {
                    int seed = Guid.NewGuid().GetHashCode();
                    tbxBox.Text = TextManager.GenerateMagicWords(new Random(seed).Next(2, 4));
                    tbxBox.TextColor = Color.Black;
                }
            }

            ChantsFading = true;

            foreach (Control c in Controls)
                if (c.Name.Contains(leftOrRight))
                    c.IsVisible = true;
        }

        public void ErasePage(bool leftPage)
        {
            string leftOrRight = leftPage ? "Left" : "Right";

            foreach (Control c in Controls)
                if (c.Name.Contains(leftOrRight))
                    c.IsVisible = false;

            if (leftPage)
            {
                LeftChantTextBox = null;
                LeftPageSpell = null;
            }
            else
            {
                RightChantTextBox = null;
                RightPageSpell = null;
            }
        }

        public void FlipTo(string spellName)
        {
            int count = 0;
            while (LeftPageSpell != null && LeftPageSpell.Name != spellName && RightPageSpell != null && RightPageSpell.Name != spellName && count < Character.CurrentCharacter.Spells.Count)
            {
                Events.RegisterEvent(Events.EventName.Spellbook_Flip, "forward");
                count++;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if(Character.CurrentCharacter == null || Character.CurrentCharacter.Spells.Count <= 0)
            {
                IsVisible = false;
                return;
            }

            base.Update(gameTime);

            ZDepth = 1; // always on top (some other controls may briefly take precedence ie: effectlabels)

            if (PageSet == 1)
                this["SpellbookLeftBackFlipButton"].IsVisible = false;
            else this["SpellbookLeftBackFlipButton"].IsVisible = true;

            string finalSpellChant = Character.CurrentCharacter.Spells[Character.CurrentCharacter.Spells.Count - 1].Incantation.ToLower();

            if ((LeftChantTextBox != null && LeftChantTextBox.Text.ToLower() == finalSpellChant)
                || (RightChantTextBox != null && RightChantTextBox.Text.ToLower() == finalSpellChant))
                this["SpellbookRightForwardFlipButton"].IsVisible = false;
            else this["SpellbookRightForwardFlipButton"].IsVisible = true;

            if (ChantsFading)
            {
                if(LeftChantTextBox != null)
                {
                    LeftChantTextBox.TextAlpha -= 5;
                    if (LeftChantTextBox.TextAlpha <= 100)
                        ChantsFading = false;
                }

                if(RightChantTextBox != null)
                {
                    RightChantTextBox.TextAlpha -= 5;
                    if (RightChantTextBox.TextAlpha <= 100)
                        ChantsFading = false;
                }
            }
            else
            {
                if (LeftChantTextBox != null)
                {
                    LeftChantTextBox.TextAlpha += 5;
                    if (LeftChantTextBox.TextAlpha >= 255)
                        ChantsFading = true;
                }

                if (RightChantTextBox != null)
                {
                    RightChantTextBox.TextAlpha += 5;
                    if (RightChantTextBox.TextAlpha >= 255)
                        ChantsFading = true;
                }
            }
        }

        protected override void OnMouseLeave(MouseState ms)
        {
            base.OnMouseLeave(ms);

            if (GuiManager.DraggedControl == this)
                GuiManager.StopDragging();
        }
    }
}