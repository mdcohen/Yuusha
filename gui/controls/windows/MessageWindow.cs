using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Yuusha.gui
{
    /// <summary>
    /// Used to display tips and hints. Also used to display sage advice.
    /// </summary>
    public class MessageWindow : Window
    {
        public ScrollableTextBox TipTextBox
        { get; private set; }

        public MessageWindow(string name, string owner, Rectangle rectangle, bool visible, bool locked, bool disabled, string font,
            VisualKey visualKey, Color tintColor, byte visualAlpha, bool dropShadow, Map.Direction shadowDirection, int shadowDistance,
            List<Enums.EAnchorType> anchors, string cursorOverride) : base(name, owner, rectangle, visible, locked, disabled, font, visualKey, tintColor, visualAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride)
        {
        }

        public static void CreateTipMessageWindow()
        {
            // HintWindow exists, if this is called then another hint is desired.
            if(GuiManager.GenericSheet["TipWindow"] is MessageWindow existingWindow)
                existingWindow.OnClose();

            MessageWindow w = new MessageWindow("TipWindow", "", new Rectangle(100, 50, 300, 250), false, false, false,
                GuiManager.GenericSheet.Font, new VisualKey("WhiteSpace"), Color.Black, 255, true, Map.Direction.Northwest, 5, new List<Enums.EAnchorType>() { Enums.EAnchorType.Center }, "Dragging");

            WindowTitle wTitle = new WindowTitle(w.Name + "Title", w.Name, "lemon12", "Tip of the Trade", Color.PaleGreen, Color.MediumPurple,
                255, BitmapFont.TextAlignment.Center, new VisualKey("WhiteSpace"), false,
                new VisualKey("WindowCloseBox"), new VisualKey("WindowCloseBoxDown"), 18, 2, 17, 17, Color.Thistle, 21)
            {
                Width = w.Width
            };

            SquareBorder border = new SquareBorder(w.Name + "Border", w.Name, 1, new VisualKey("WhiteSpace"), false, Color.MediumPurple, 255);

            ScrollableTextBox s = new ScrollableTextBox(w.Name + "ScrollableTextBox", w.Name, new Rectangle(3, 20, 296, 228), "", Color.White, true,
                false, w.Font, new VisualKey("WhiteSpace"), Color.Black, 0, 255, new VisualKey(""), new VisualKey(""), new VisualKey(""), 0, 0, BitmapFont.TextAlignment.Left, new List<Enums.EAnchorType>(), true);

            w.TipTextBox = s;

            GuiManager.GenericSheet.AddControl(w);
            GuiManager.GenericSheet.AddControl(wTitle);
            GuiManager.GenericSheet.AddControl(border);
            GuiManager.GenericSheet.AddControl(s);

            string[] tipInfo = TextManager.GetRandomHintText();
            s.AddLine(tipInfo[1], Enums.ETextType.Hint);
            w.WindowTitle.Text = tipInfo[0] + " Tip";
            w.AdjustHeight();
            Audio.AudioManager.PlaySoundEffect("GUISounds/tip_notification");
            w.IsVisible = true;
        }

        public static void CreateSageAdviceMessageWindow(string advice)
        {
            // Window exists, if this is called then another hint is desired.
            if (GuiManager.GenericSheet["SageAdviceTipWindow"] is MessageWindow existingWindow)
            {
                existingWindow.OnClose();
            }

            MessageWindow w = new MessageWindow("SageAdviceTipWindow", "", new Rectangle(Client.Width / 2 - 150, 50, 300, 250), false, false, false,
                GuiManager.GenericSheet.Font, new VisualKey("WhiteSpace"), Color.Black, 180, true, Map.Direction.Northwest, 5, new List<Enums.EAnchorType>() { Enums.EAnchorType.Center }, "Dragging");

            WindowTitle wTitle = new WindowTitle(w.Name + "Title", w.Name, "lemon12", "Sage Advice", Color.PaleGreen, Color.DarkGreen,
                255, BitmapFont.TextAlignment.Center, new VisualKey("WhiteSpace"), false,
                new VisualKey("WindowCloseBox"), new VisualKey("WindowCloseBoxDown"), 18, 2, 17, 17, Color.LightGreen, 21)
            {
                Width = w.Width
            };

            SquareBorder border = new SquareBorder(w.Name + "Border", w.Name, 1, new VisualKey("WhiteSpace"), false, Color.DarkGreen, 255);

            ScrollableTextBox s = new ScrollableTextBox(w.Name + "ScrollableTextBox", w.Name, new Rectangle(3, 20, 296, 228), "", Color.White, true,
                false, w.Font, new VisualKey("WhiteSpace"), Color.Black, 0, 255, new VisualKey(""), new VisualKey(""), new VisualKey(""), 0, 0, BitmapFont.TextAlignment.Center, new List<Enums.EAnchorType>(), true);

            w.TipTextBox = s;
            s.AddLine(advice, Enums.ETextType.SageAdvice);

            GuiManager.GenericSheet.AddControl(w);
            GuiManager.GenericSheet.AddControl(wTitle);
            GuiManager.GenericSheet.AddControl(border);
            GuiManager.GenericSheet.AddControl(s);

            w.AdjustHeight();
            Audio.AudioManager.PlaySoundEffect("GUISounds/sageadvice_cymbal");
            w.IsVisible = true;
        }

        public static void CreateNewsMessageWindow(string news)
        {
            // Window exists, if this is called then another hint is desired.
            if (GuiManager.GenericSheet["NewsWindow"] is MessageWindow existingWindow)
            {
                existingWindow.OnClose();
            }

            MessageWindow w = new MessageWindow("NewsWindow", "", new Rectangle(500, 50, 500, 250), false, false, false,
                GuiManager.GenericSheet.Font, new VisualKey("WhiteSpace"), Color.Black, 255, true, Map.Direction.Northwest, 5, new List<Enums.EAnchorType>() { Enums.EAnchorType.Center }, "Dragging");

            WindowTitle wTitle = new WindowTitle(w.Name + "Title", w.Name, "lemon12", "Game News", Color.White, Color.Gray,
                255, BitmapFont.TextAlignment.Center, new VisualKey("WhiteSpace"), false,
                new VisualKey("WindowCloseBox"), new VisualKey("WindowCloseBoxDown"), 18, 2, 17, 17, Color.LightSteelBlue, 21)
            {
                Width = w.Width
            };

            SquareBorder border = new SquareBorder(w.Name + "Border", w.Name, 1, new VisualKey("WhiteSpace"), false, Color.Gray, 255);

            ScrollableTextBox s = new ScrollableTextBox(w.Name + "ScrollableTextBox", w.Name, new Rectangle(3, 22, w.Width - 3 - border.Width, 228), "", Color.White, true,
                false, w.Font, new VisualKey("WhiteSpace"), Color.Black, 0, 255, new VisualKey(""), new VisualKey(""), new VisualKey(""), 0, 0, BitmapFont.TextAlignment.Left, new List<Enums.EAnchorType>(), true);

            w.TipTextBox = s;

            try
            {
                    string[] nz = news.Split(Protocol.ISPLIT.ToCharArray());
                    foreach (string line in nz)
                        s.AddLine(line.Trim(), Enums.ETextType.Default);
            }
            catch (System.IO.FileNotFoundException)
            {
                s.AddLine("Failed to display news.", Enums.ETextType.Default);
            }

            //s.AddLine(news, Enums.ETextType.News);

            GuiManager.GenericSheet.AddControl(w);
            GuiManager.GenericSheet.AddControl(wTitle);
            GuiManager.GenericSheet.AddControl(border);
            GuiManager.GenericSheet.AddControl(s);

            w.AdjustHeight();
            //Audio.AudioManager.PlaySoundEffect("GUISounds/sageadvice_cymbal");
            w.IsVisible = true;
        }

        public override void OnClose()
        {
            base.OnClose();

            GuiManager.RemoveControl(this);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            AdjustHeight();
        }

        private void AdjustHeight()
        {
            int height = BitmapFont.ActiveFonts[TipTextBox.Font].LineHeight * TipTextBox.FormattedLinesCount;

            TipTextBox.Height = height;

            Height = WindowTitle.Height + WindowBorder.Height + TipTextBox.Height + 4;
        }
    }
}
