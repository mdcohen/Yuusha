using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Yuusha.gui
{
    public class TalentsWindow : Window
    {
        private int TalentsCount = 0;
        private Window m_activatedTalentsWindow;
        private Window m_passiveTalentsWindow;

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

            talentsWindow.m_activatedTalentsWindow = activatedTalentsWindow;
            talentsWindow.m_passiveTalentsWindow = passiveTalentsWindow;
        }

        public void CreateTalentHotButtons()
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

                    PercentageBarLabel activatedTalentHotButtonPercentageBarLabel = new PercentageBarLabel(activatedTalentHotButton.Name + "PercentageBar", "ActivatedTalentsWindow",
                    new Rectangle(x, y, size, size), "", Color.White, false, false, activatedTalentHotButton.Font, new VisualKey("WhiteSpace"), Color.DarkMagenta,
                    40, 0, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>(), "", false)
                    {
                        Percentage = 100,
                        Segmented = false
                    };
                    activatedTalentHotButtonPercentageBarLabel.MidLabel = new Label(activatedTalentHotButtonPercentageBarLabel.Name + "MidLabel", activatedTalentHotButtonPercentageBarLabel.Name,
                        new Rectangle(activatedTalentHotButtonPercentageBarLabel.Position.X, activatedTalentHotButtonPercentageBarLabel.Position.Y, 0, activatedTalentHotButtonPercentageBarLabel.Height), "", Color.White,
                        true, false, activatedTalentHotButtonPercentageBarLabel.Font, new VisualKey("WhiteSpace"), Color.DimGray, 230, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>(), "");

                    GuiManager.GenericSheet.AddControl(activatedTalentHotButton);
                    GuiManager.CurrentSheet.AddControl(activatedTalentHotButtonPercentageBarLabel);
                    x += size + spacing;
                    TalentsCount++;
                }

                y = padding;
                x = padding;

                for (int i = 0; i < passiveTalentsList.Count; i++)
                {
                    HotButton passiveTalentHotButton = new HotButton("PassiveTalentHotButton" + i, "PassiveTalentsWindow", new Rectangle(x, y, size, size),
                    passiveTalentsList[i], false, Color.Black, true, true, "lemon14", new VisualKey(GameHUD.GameIconsDictionary[Talent.IconsDictionary[passiveTalentsList[i]]]), Color.Silver, 255, 255, new VisualKey(""), new VisualKey(""),
                    new VisualKey(""), "send_command", BitmapFont.TextAlignment.Center, 0, 0, Color.White, false, Color.White, false, new List<Enums.EAnchorType>(), false, Map.Direction.None, 0, "toggletalent " + Character.CurrentCharacter.Talents.Find(t => t.Name == passiveTalentsList[i]).Command, passiveTalentsList[i])
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
                    aWindow.Width = Math.Max(activatedTalentsList.Count, passiveTalentsList.Count) * size + Math.Max(activatedTalentsList.Count, passiveTalentsList.Count) * spacing;
                }

                if (this["PassiveTalentsWindow"] is Window pWindow)
                {
                    pWindow.Height = Height - (WindowTitle != null ? WindowTitle.Height : 0) - 2;
                    pWindow.Width = Math.Max(activatedTalentsList.Count, passiveTalentsList.Count) * size + Math.Max(activatedTalentsList.Count, passiveTalentsList.Count) * spacing;
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

            if (!Client.InGame) return;

            try
            {
                if (Character.CurrentCharacter != null && Character.CurrentCharacter.Talents != null)
                {
                    foreach (Talent t in Character.CurrentCharacter.Talents)
                    {
                        if (t.IsPassive && m_passiveTalentsWindow != null)
                        {
                            if (m_passiveTalentsWindow.Controls.FindIndex(c => c.Text == t.Name) is int index && index > -1)
                            {
                                if (t.IsEnabled)
                                {
                                    m_passiveTalentsWindow.Controls[index].TintColor = Color.White;
                                }
                                else
                                {
                                    m_passiveTalentsWindow.Controls[index].TintColor = Color.DimGray;
                                }
                            }

                        }
                        else if(m_activatedTalentsWindow != null)
                        {
                            if (m_activatedTalentsWindow.Controls.FindIndex(c => c.Text == t.Name) is int index && index > -1)
                            {
                                if (DateTime.Now - t.LastUse < t.DownTime) // not available
                                {
                                    m_activatedTalentsWindow.Controls[index].TintColor = Color.LightGray;
                                    if (m_activatedTalentsWindow.Controls.FindIndex(c => c.Name == m_activatedTalentsWindow.Controls[index].Name + "PercentageBar") is int pctIndex && pctIndex > -1)
                                    {
                                        if (m_activatedTalentsWindow.Controls[pctIndex] is PercentageBarLabel pctLabel)
                                        {
                                            // After next server update change this to DateTime.UtcNow
                                            TimeSpan timeRemaining = DateTime.Now - t.LastUse;
                                            pctLabel.Percentage = timeRemaining.TotalMilliseconds / t.DownTime.TotalMilliseconds * 100;
                                            pctLabel.IsVisible = true;
                                        }
                                    }
                                }
                                else
                                {
                                    m_activatedTalentsWindow.Controls[index].TintColor = Color.White;
                                    if (m_activatedTalentsWindow.Controls.FindIndex(c => c.Name == m_activatedTalentsWindow.Controls[index].Name + "PercentageBar") is int pctIndex && pctIndex > -1)
                                    {
                                        if (m_activatedTalentsWindow.Controls[pctIndex] is PercentageBarLabel pctLabel)
                                        {
                                            pctLabel.IsVisible = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Utils.LogException(e);
            }
        }

        public override void OnClose()
        {
            base.OnClose();

            GuiManager.RemoveControl(this);
        }
    }
}
