using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Yuusha.gui
{
    public class TalentsWindow : Window
    {
        private int TalentsCount = 0;

        public TalentsWindow(string name, string owner, Rectangle rectangle, bool visible, bool locked, bool disabled, string font,
            VisualKey visualKey, Color tintColor, byte visualAlpha, bool dropShadow, Map.Direction shadowDirection, int shadowDistance,
            List<Enums.EAnchorType> anchors, string cursorOverride) : base(name, owner, rectangle, visible, locked, disabled, font, visualKey, tintColor, visualAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride)
        {
        }

        public static void CreateTalentsWindow()
        {
            if (GuiManager.GenericSheet["TalentsWindow"] is TalentsWindow w)
            {
                w.IsVisible = true;
                return;
            }

            TalentsWindow talentsWindow = new TalentsWindow("TalentsWindow", "", new Rectangle(150, 100, 300, 500), true, false, false, GuiManager.GenericSheet.Font,
                new VisualKey("WhiteSpace"), Color.Black, 140, true, Map.Direction.Northwest, 10, new List<Enums.EAnchorType>() { Enums.EAnchorType.Center }, "Dragging")
            {
                IsVisible = false
            };
            GuiManager.GenericSheet.AddControl(talentsWindow);

            WindowTitle wTitle = new WindowTitle(talentsWindow.Name + "Title", talentsWindow.Name, talentsWindow.Font, "Talents", Color.PaleGreen, Color.DimGray,
                140, BitmapFont.TextAlignment.Center, new VisualKey("WhiteSpace"), false,
                new VisualKey("WindowCloseBox"), new VisualKey("WindowCloseBoxDown"), 25, 5, 15, 15, Color.LightSteelBlue, 24)
            {
                Width = talentsWindow.Width
            };
            GuiManager.GenericSheet.AddControl(wTitle);

            SquareBorder wBorder = new SquareBorder(talentsWindow.Name + "Border", talentsWindow.Name, 1, new gui.VisualKey("WhiteSpace"), false, Color.DimGray, 255);
            GuiManager.GenericSheet.AddControl(wBorder);

            talentsWindow.CreateTalentLabels();
        }

        private void CreateTalentLabels()
        {
            foreach(Control c in new List<Control>(Controls))
            {
                if (c.Name.StartsWith("TalentLabel"))
                    Controls.Remove(c);
            }

            TalentsCount = 0;

            int x = 3;
            int y = 25;
            string font = "lemon12";
            int lineHeight = BitmapFont.ActiveFonts[font].LineHeight;

            List<string> activatedTalentsList = new List<string>();
            List<string> passiveTalentsList = new List<string>();

            foreach(Talent talent in Character.CurrentCharacter.Talents)
            {
                if (!talent.IsPassive) activatedTalentsList.Add(talent.Name);
                else passiveTalentsList.Add(talent.Name);
            }

            activatedTalentsList.Sort();
            passiveTalentsList.Sort();
            int longestTalentName = 0;

            for (int i = 0; i < activatedTalentsList.Count; i++)
            {
                int length = BitmapFont.ActiveFonts[font].MeasureString(activatedTalentsList[i]);
                if (length > longestTalentName) longestTalentName = length;

                Label talentLabel = new Label("ActivatedTalentLabel" + i, Name, new Rectangle(x, y, length, lineHeight + 2),
                activatedTalentsList[i], Color.White, true, false, font, new VisualKey("WhiteSpace"), Color.Black, 0, 255, BitmapFont.TextAlignment.Left,
                0, 0, "", "", new List<Enums.EAnchorType>(), "");
                GuiManager.GenericSheet.AddControl(talentLabel);
                y += talentLabel.Height + 2;
                TalentsCount++;
            }

            x = longestTalentName + 20;
            y = 25;

            for (int i = 0; i < passiveTalentsList.Count; i++)
            {
                int length = BitmapFont.ActiveFonts[font].MeasureString(passiveTalentsList[i]);
                if (length > longestTalentName) longestTalentName = length;

                Label talentLabel = new Label("PassiveTalentLabel" + i, Name, new Rectangle(x, y, length, lineHeight + 2),
                passiveTalentsList[i], Color.White, true, false, font, new VisualKey("WhiteSpace"), Color.Black, 0, 255, BitmapFont.TextAlignment.Left,
                0, 0, "", "", new List<Enums.EAnchorType>(), "");
                GuiManager.GenericSheet.AddControl(talentLabel);
                y += talentLabel.Height + 2;
                TalentsCount++;
            }

            Width = x + longestTalentName + 3;
            Height = WindowTitle.Height + (Math.Max(activatedTalentsList.Count, passiveTalentsList.Count) * (lineHeight + 2));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Character.CurrentCharacter != null && Character.CurrentCharacter.Talents.Count != TalentsCount)
                CreateTalentLabels();
        }

        public override void OnClose()
        {
            base.OnClose();

            GuiManager.RemoveControl(this);
        }
    }
}
