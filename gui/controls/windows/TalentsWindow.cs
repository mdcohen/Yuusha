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
            if(Character.CurrentCharacter.Talents.Count <= 0)
                return;

            if (GuiManager.GetControl("TalentsWindow") is TalentsWindow w)
            {
                return;
            }

            TalentsWindow talentsWindow = new TalentsWindow("TalentsWindow", "", new Rectangle(150, 100, 500, 100), false, false, false, "lemon10",
                new VisualKey("WhiteSpace"), Color.Black, 0, true, Map.Direction.Northwest, 10, new List<Enums.EAnchorType>(), "Dragging");
            GuiManager.GenericSheet.AddControl(talentsWindow);

            TalentsWindow activatedTalentsWindow = new TalentsWindow("ActivatedTalentsWindow", talentsWindow.Name, new Rectangle(20, 19, 480, 75), true, true, false, GuiManager.GenericSheet.Font,
                new VisualKey("WhiteSpace"), Color.Black, 140, false, Map.Direction.Northwest, 10, new List<Enums.EAnchorType>(), "Dragging");
            GuiManager.GenericSheet.AddControl(activatedTalentsWindow);

            TalentsWindow passiveTalentsWindow = new TalentsWindow("PassiveTalentsWindow", talentsWindow.Name, new Rectangle(20, 19, 480, 75), false, true, false, GuiManager.GenericSheet.Font,
                new VisualKey("WhiteSpace"), Color.SlateGray, 140, false, Map.Direction.Northwest, 10, new List<Enums.EAnchorType>(), "Dragging");
            GuiManager.GenericSheet.AddControl(passiveTalentsWindow);

            TabControlButton activatedTalentsTabControlButton = new TabControlButton(activatedTalentsWindow.Name + "TabControlButton", talentsWindow.Name, new Rectangle(1, 19, 20, 30),
                "A", true, Color.White, true, false, talentsWindow.Font, new VisualKey("WhiteSpace"), Color.Purple, 255, 255, new VisualKey(""), new VisualKey(""), new VisualKey(""),
                 BitmapFont.TextAlignment.Center, 0, 0, Color.PaleGreen, true, Color.MediumOrchid, true, new List<Enums.EAnchorType>(), false, Map.Direction.None, 0, activatedTalentsWindow.Name)
            {
                PopUpText = "Activated",
            };
            GuiManager.GenericSheet.AddControl(activatedTalentsTabControlButton);

            TabControlButton passiveTalentsTabControlButton = new TabControlButton(passiveTalentsWindow.Name + "TabControlButton", talentsWindow.Name, new Rectangle(1, 53, 20, 30),
                "P", true, Color.White, true, false, talentsWindow.Font, new VisualKey("WhiteSpace"), Color.Purple, 255, 255, new VisualKey(""), new VisualKey(""), new VisualKey(""),
                 BitmapFont.TextAlignment.Center, 0, 0, Color.PaleGreen, true, Color.MediumOrchid, true, new List<Enums.EAnchorType>(), false, Map.Direction.None, 0, passiveTalentsWindow.Name)
            {
                PopUpText = "Passive",
            };
            GuiManager.GenericSheet.AddControl(passiveTalentsTabControlButton);

            WindowTitle wTitle = new WindowTitle(talentsWindow.Name + "Title", talentsWindow.Name, talentsWindow.Font, "Talents", Color.PaleGreen, Color.DimGray,
                140, BitmapFont.TextAlignment.Center, new VisualKey("WhiteSpace"), false,
                new VisualKey("WindowCloseBox"), new VisualKey("WindowCloseBoxDown"), 15, 2, 15, 15, Color.LightSteelBlue, 18)
            {
                Width = talentsWindow.Width
            };
            GuiManager.GenericSheet.AddControl(wTitle);

            SquareBorder wBorder = new SquareBorder(talentsWindow.Name + "Border", talentsWindow.Name, 1, new VisualKey("WhiteSpace"), false, Color.DimGray, 255);
            GuiManager.GenericSheet.AddControl(wBorder);

            talentsWindow.CreateTalentHotButtons();
        }

        private void CreateTalentHotButtons()
        {
            try
            {
                foreach (Control c in new List<Control>(Controls))
                {
                    if (c is Window)
                        c.OnDispose();
                }

                TalentsCount = 0;

                int padding = 1;
                int x = padding;
                int y = padding;
                string font = "lemon12";
                int lineHeight = BitmapFont.ActiveFonts[font].LineHeight;

                List<string> activatedTalentsList = new List<string>();
                List<string> passiveTalentsList = new List<string>();

                foreach (Talent talent in Character.CurrentCharacter.Talents)
                {
                    if (!talent.IsPassive) activatedTalentsList.Add(talent.Name);
                    else passiveTalentsList.Add(talent.Name);
                }

                activatedTalentsList.Sort();
                passiveTalentsList.Sort();

                int size = 64;
                int spacing = 1;

                for (int i = 0; i < activatedTalentsList.Count; i++)
                {
                    HotButton activatedTalentHotButton = new HotButton("ActivatedTalentHotButton" + i, "ActivatedTalentsWindow", new Rectangle(x, y, size, size),
                    activatedTalentsList[i], false, Color.Black, true, false, "lemon14", new VisualKey(GameHUD.GameIconsDictionary[Talent.IconsDictionary[activatedTalentsList[i]]]), Color.Azure, 255, 255, new VisualKey(""), new VisualKey(""),
                    new VisualKey(""), "activate_talent", BitmapFont.TextAlignment.Center, 0, 0, Color.White, false, Color.White, false, new List<Enums.EAnchorType>(), false, Map.Direction.None, 0, "", activatedTalentsList[i])
                    {
                        IsLocked = false  // allows for dragging a label copy to make a hotbutton copy
                    };

                    GuiManager.GenericSheet.AddControl(activatedTalentHotButton);
                    x += size + spacing;
                    TalentsCount++;
                }

                y = padding;

                x = padding;

                for (int i = 0; i < passiveTalentsList.Count; i++)
                {
                    HotButton passiveTalentHotButton = new HotButton("PassiveTalentHotButton" + i, "PassiveTalentsWindow", new Rectangle(x, y, size, size),
                    passiveTalentsList[i], false, Color.Black, true, true, "lemon14", new VisualKey(GameHUD.GameIconsDictionary[Talent.IconsDictionary[passiveTalentsList[i]]]), Color.Silver, 255, 255, new VisualKey(""), new VisualKey(""),
                    new VisualKey(""), "", BitmapFont.TextAlignment.Center, 0, 0, Color.White, false, Color.White, false, new List<Enums.EAnchorType>(), false, Map.Direction.None, 0, "", passiveTalentsList[i])
                    {
                        IsLocked = false  // allows for dragging a label copy to make a hotbutton copy
                    };

                    GuiManager.GenericSheet.AddControl(passiveTalentHotButton);
                    x += size + spacing;
                    TalentsCount++;
                }

                Height = (WindowTitle != null ? WindowTitle.Height : 0) + size + padding * 4;
                Width = 21 + Math.Max(activatedTalentsList.Count, passiveTalentsList.Count) * size + Math.Max(activatedTalentsList.Count, passiveTalentsList.Count) * spacing;

                if (this["ActivatedTalentsWindow"] is Window aWindow)
                {
                    aWindow.Height = Height - (WindowTitle != null ? WindowTitle.Height : 0) - 2;
                    aWindow.Width = Math.Max(activatedTalentsList.Count, passiveTalentsList.Count) * size + Math.Max(activatedTalentsList.Count, passiveTalentsList.Count) * spacing - 21;
                }

                if (this["PassiveTalentsWindow"] is Window pWindow)
                {
                    pWindow.Height = Height - (WindowTitle != null ? WindowTitle.Height : 0) - 2;
                    pWindow.Width = Math.Max(activatedTalentsList.Count, passiveTalentsList.Count) * size + Math.Max(activatedTalentsList.Count, passiveTalentsList.Count) * spacing - 21;
                }

                if (WindowTitle != null) WindowTitle.Width = Width;
            }
            catch(Exception e)
            {
                Utils.LogException(e);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //if (Character.CurrentCharacter != null && Character.CurrentCharacter.Talents.Count > 0 && Character.CurrentCharacter.Talents.Count != TalentsCount)
            //    CreateTalentHotButtons();
        }

        public override void OnClose()
        {
            base.OnClose();

            GuiManager.RemoveControl(this);
        }
    }
}
