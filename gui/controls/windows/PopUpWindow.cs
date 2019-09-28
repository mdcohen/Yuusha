using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Yuusha.gui
{
    /// <summary>
    /// General rule of thumb is a PopUpWindow closes when the mouse leaves its rectangle.
    /// </summary>
    public class PopUpWindow : Window
    {
        public PopUpWindow(string name, string owner, Rectangle rectangle, bool visible, bool locked, bool disabled, string font,
            VisualKey visualKey, Color tintColor, byte visualAlpha, bool dropShadow, Map.Direction shadowDirection, int shadowDistance,
            List<Enums.EAnchorType> anchors, string cursorOverride) : base(name, owner, rectangle, visible, locked, disabled, font, visualKey, tintColor, visualAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride)
        {
        }

        public static void CreateCommonCommandsPopUpWindow()
        {
            if (GuiManager.GenericSheet["CommonCommandsPopUpWindow"] != null)
                return;

            // button name, button command
            Dictionary<string, string> CommonCommands = new Dictionary<string, string>()
            {
                {"Display Combat Damage", "displaycombatdamage" },
                {"Display Damage Shield", "displaydamageshield" },
                {"Display Game Round", "displaygameround" },
                {"Display Pet Damage", "displaypetdamage" },
                {"Display Pet Messages", "displaypetmessages" },
                {"Show Armor Class", "showac" },
                {"Show DPS Stats", "showdps" },
                {"Toggle DPS Logging", "toggleDPS" },
            };

            MouseState ms = GuiManager.MouseState;

            PopUpWindow w = new PopUpWindow("CommonCommandsPopUpWindow", "", new Rectangle(ms.X - 100, ms.Y - 10, 204, 400), true, false, false, GuiManager.GenericSheet.Font, new gui.VisualKey("WhiteSpace"),
                Color.Black, 150, true, Map.Direction.Northwest, 3, new List<Enums.EAnchorType>(), "");

            WindowTitle wTitle = new WindowTitle(w.Name + "Title", w.Name, "lemon10", "Common Commands", Color.White, Color.Black, 50, BitmapFont.TextAlignment.Center, new VisualKey("WhiteSpace"), false,
                new VisualKey(""), new VisualKey(""), 0, 0, 0, 0, Color.Black, 17);

            GuiManager.GenericSheet.AddControl(w);
            GuiManager.GenericSheet.AddControl(wTitle);

            int spacing = 2;
            int x = spacing;
            int y = wTitle.Height + spacing;

            foreach (string key in CommonCommands.Keys)
            {
                string command = CommonCommands[key];

                if (Client.GameState == Enums.EGameState.Conference)
                    command = "/" + command;

                Button commonCommandButton = new Button(command + "CommonCommandButton", w.Name, new Rectangle(x, y, 200, 21), key, true, Color.White,
                    true, false, "robotomonobold11", new VisualKey("WhiteSpace"), Color.DimGray, 255, 255, new VisualKey(""), new VisualKey(""), new VisualKey(""), "send_command", BitmapFont.TextAlignment.Center, 0, 0,
                Color.DarkMagenta, true, Color.PaleGreen, true, new List<Enums.EAnchorType>(), false, Map.Direction.None, 0, command, "", Client.ClientSettings.DefaultOnClickSound);
                GuiManager.GenericSheet.AddControl(commonCommandButton);
                y += 21 + spacing;
            }

            w.Height = y;
        }

        public static void CreateScoresCommandsPopUpWindow()
        {
            // button name, button command
            Dictionary<string, string> ScoresCommands = new Dictionary<string, string>()
            {
                {"Top Ten", "scores" },
                //{"Top Ten Berserkers", "scores ber" },
                //{"Top Ten Druids", "scores dr" },
                {"Top Fighters", "scores fi" },
                {"Top Knights", "scores kn" },
                {"Top Martial Artists", "scores ma" },
                //{"Top Ten Rangers", "scores rng" },
                {"Top Ravagers", "scores rav" },
                {"Top Sorcerers", "scores sorc" },
                {"Top Thaumaturges", "scores thaum" },
                {"Top Thieves", "scores thief" },
                {"Top Wizards", "scores wiz" },
            };

            MouseState ms = GuiManager.MouseState;

            PopUpWindow w = new PopUpWindow("CommonCommandsPopUpWindow", "", new Rectangle(ms.X - 120, ms.Y - 10, 204, 400), true, false, false, GuiManager.GenericSheet.Font, new gui.VisualKey("WhiteSpace"),
                Color.Black, 150, true, Map.Direction.Northwest, 3, new List<Enums.EAnchorType>(), "");

            WindowTitle wTitle = new WindowTitle(w.Name + "Title", w.Name, "lemon10", "Scores", Color.White, Color.Black, 50, BitmapFont.TextAlignment.Center, new VisualKey("WhiteSpace"), false,
                new VisualKey(""), new VisualKey(""), 0, 0, 0, 0, Color.Black, 17);

            GuiManager.GenericSheet.AddControl(w);
            GuiManager.GenericSheet.AddControl(wTitle);

            int spacing = 2;
            int x = spacing;
            int y = wTitle.Height + spacing;

            foreach (string key in ScoresCommands.Keys)
            {
                string command = ScoresCommands[key];

                if (Client.GameState == Enums.EGameState.Conference)
                    command = "/" + command;

                Button scoresCommandButton = new Button(command + "scoresCommandButton", w.Name, new Rectangle(x, y, 200, 21), key, true, Color.White,
                    true, false, "robotomonobold11", new VisualKey("WhiteSpace"), Color.DimGray, 255, 255, new VisualKey(""), new VisualKey(""), new VisualKey(""), "send_command", BitmapFont.TextAlignment.Center, 0, 0,
                Color.DarkMagenta, true, Color.PaleGreen, true, new List<Enums.EAnchorType>(), false, Map.Direction.None, 0, command, "", Client.ClientSettings.DefaultOnClickSound);
                GuiManager.GenericSheet.AddControl(scoresCommandButton);
                y += 21 + spacing;
            }

            w.Height = y;
        }

        public static void CreateSquareIconPopUpWindow(Control c, int size)
        {
            if (GuiManager.GenericSheet[c.Name + "PopUpWindow"] is PopUpWindow existingWindow)
                return;

            MouseState ms = GuiManager.MouseState;

            PopUpWindow w = new PopUpWindow(c.Name + "PopUpWindow", "", new Rectangle(ms.X - size / 2, ms.Y - size / 2, size, size), true, true, false, GuiManager.GenericSheet.Font, new VisualKey(""),
                c.TintColor, (byte)c.VisualAlpha, true, Map.Direction.Northwest, 3, new List<Enums.EAnchorType>(), c.PopUpText);

            Button b = new Button(w.Name + "CommandButton", w.Name, new Rectangle(0, 0, size, size), "", false, Color.White, true, false, "courier12", new VisualKey(c.VisualKey),
                Color.White, 255, 255, new VisualKey(""), new VisualKey(""), new VisualKey(""), "Send_Command", BitmapFont.TextAlignment.Center, 0, 0,
                Color.White, false, Color.Green, false, new List<Enums.EAnchorType>(), false, Map.Direction.None, 0, c.Command, c.PopUpText, Client.ClientSettings.DefaultOnClickSound);

            SquareBorder bBorder = new SquareBorder(b.Name + "SquareBorder", b.Name, 1, new VisualKey("WhiteSpace"), false, Color.DimGray, 255);

            GuiManager.GenericSheet.AddControl(w);
            GuiManager.GenericSheet.AddControl(b);
            GuiManager.GenericSheet.AddControl(bBorder);
        }

        public static void CreateVolumeControlPopUpWindow()
        {
            if (GuiManager.GenericSheet["VolumeControlPopUpWindow"] != null)
                return;

            MouseState ms = GuiManager.MouseState;

            PopUpWindow w = new PopUpWindow("VolumeControlPopUpWindow", "", new Rectangle(ms.X - 120, ms.Y - 10, 160, 57), true, false, false, GuiManager.GenericSheet.Font, new gui.VisualKey("WhiteSpace"),
                Color.Black, 150, true, Map.Direction.Northwest, 3, new List<Enums.EAnchorType>(), "");

            WindowTitle wTitle = new WindowTitle(w.Name + "Title", w.Name, "lemon10", "Volume Control", Color.White, Color.Black, 50, BitmapFont.TextAlignment.Center, new VisualKey("WhiteSpace"), false,
                new VisualKey(""), new VisualKey(""), 0, 0, 0, 0, Color.Black, 17);

            PercentageBarLabel pctLabel = new PercentageBarLabel(w.Name + "PercentageBarLabel", w.Name, new Rectangle(49, 34, 80, 5), "", Color.White,
                true, false, w.Font, new VisualKey("WhiteSpace"), Color.DimGray, 255, 150, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>(), "", false);
            pctLabel.MidLabel = new Label(pctLabel.Name + "ForeLabel", pctLabel.Name, new Rectangle(49, 34, 80, 5), "", Color.White,
                true, false, w.Font, new VisualKey("WhiteSpace"), Color.White, 255, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>(), "");

            pctLabel.Segmented = true;
            pctLabel.Percentage = Microsoft.Xna.Framework.Media.MediaPlayer.Volume * 100;

            CheckboxButton chkMuteButton = new CheckboxButton(w.Name + "MuteVolumeButton", w.Name, new Rectangle(3, 27, 20, 20), true, false, w.Font, new VisualKey("SoundWavesIcon"),
                Color.PaleGreen, 255, new VisualKey(""), new VisualKey(""), new VisualKey(""), new VisualKey("hotbuttonicon_490"), Color.White, Color.Green, true,
                new List<Enums.EAnchorType>(), false, Map.Direction.None, 0, "MediaPlayer_MuteVolume", "");

            Button lowerVolumeButton = new Button(w.Name + "LowerVolumeButton", w.Name, new Rectangle(24, 27, 20, 20), "-", true, Color.DarkMagenta, true, false, "courier12", new VisualKey("EmptyCircleIcon"),
                Color.PaleGreen, 255, 255, new VisualKey(""), new VisualKey("EmptyCircleReverseIcon"), new VisualKey(""), "MediaPlayer_LowerVolume", BitmapFont.TextAlignment.Center, 0, 0,
                Color.White, true, Color.Green, true, new List<Enums.EAnchorType>(), false, Map.Direction.None, 0, "", "", Client.ClientSettings.DefaultOnClickSound);

            Button raiseVolumeButton = new Button(w.Name + "RaiseVolumeButton", w.Name, new Rectangle(134, 27, 20, 20), "+", true, Color.DarkMagenta, true, false, "courier12", new VisualKey("EmptyCircleIcon"),
                Color.PaleGreen, 255, 255, new VisualKey(""), new VisualKey("EmptyCircleReverseIcon"), new VisualKey(""), "MediaPlayer_RaiseVolume", BitmapFont.TextAlignment.Center, 0, 0,
                Color.White, true, Color.Green, true, new List<Enums.EAnchorType>(), false, Map.Direction.None, 0, "", "", Client.ClientSettings.DefaultOnClickSound);

            GuiManager.GenericSheet.AddControl(w);
            GuiManager.GenericSheet.AddControl(wTitle);
            GuiManager.GenericSheet.AddControl(pctLabel);
            GuiManager.GenericSheet.AddControl(chkMuteButton);
            GuiManager.GenericSheet.AddControl(lowerVolumeButton);
            GuiManager.GenericSheet.AddControl(raiseVolumeButton);
        }

        public override void Update(GameTime gameTime)
        {
            if(!Contains(GuiManager.MouseState.Position))
                IsVisible = false;

            base.Update(gameTime);

            if (IsVisible)
                ZDepth = 0;
        }

        public override void OnClose()
        {
            base.OnClose();

            GuiManager.RemoveControl(this);
        }

        protected override void OnMouseLeave(MouseState ms)
        {
            base.OnMouseLeave(ms);

            OnClose();
        }
    }
}
