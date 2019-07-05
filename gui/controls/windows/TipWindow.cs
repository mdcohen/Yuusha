using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Yuusha.gui
{
    /// <summary>
    /// Used to display tips and hints. Also used to display sage advice.
    /// </summary>
    public class TipWindow : Window
    {
        public ScrollableTextBox TipTextBox
        { get; private set; }

        public TipWindow(string name, string owner, Rectangle rectangle, bool visible, bool locked, bool disabled, string font,
            VisualKey visualKey, Color tintColor, byte visualAlpha, bool dropShadow, Map.Direction shadowDirection, int shadowDistance,
            List<Enums.EAnchorType> anchors, string cursorOverride) : base(name, owner, rectangle, visible, locked, disabled, font, visualKey, tintColor, visualAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride)
        {
        }

        public static void CreateTipWindow()
        {
            // HintWindow exists, if this is called then another hint is desired.
            if(GuiManager.GenericSheet["TipWindow"] is TipWindow existingWindow)
            {
                existingWindow.OnClose();
                //if (existingWindow[existingWindow.Name + "ScrollableTextBox"] is ScrollableTextBox existingTextBox)
                //{
                //    string[] tInfo = TextManager.GetRandomHintText();
                //    existingTextBox.Clear();
                //    existingTextBox.AddLine(tInfo[1], Enums.ETextType.Hint);
                //    existingWindow.WindowTitle.Text = tInfo[0] + " Tip";
                //    existingWindow.IsVisible = true;
                //}
                //return;
            }

            TipWindow w = new TipWindow("TipWindow", "", new Rectangle(100, 50, 300, 250), true, false, false,
                GuiManager.GenericSheet.Font, new VisualKey("WhiteSpace"), Color.Black, 255, true, Map.Direction.Northwest, 5, new List<Enums.EAnchorType>() { Enums.EAnchorType.Center }, "Dragging");

            WindowTitle wTitle = new WindowTitle(w.Name + "Title", w.Name, "changaone14", "Tip of the Trade", Color.PaleGreen, Color.MediumPurple,
                255, BitmapFont.TextAlignment.Center, new VisualKey("WhiteSpace"), false,
                new VisualKey("WindowCloseBox"), new VisualKey("WindowCloseBoxDown"), 18, 2, 17, 17, Color.Thistle, 20)
            {
                Width = w.Width
            };

            SquareBorder border = new SquareBorder(w.Name + "Border", w.Name, 1, new VisualKey("WhiteSpace"), false, Color.MediumPurple, 255);

            ScrollableTextBox s = new ScrollableTextBox(w.Name + "ScrollableTextBox", w.Name, new Rectangle(3, 20, 296, 228), "", Color.White, true,
                false, w.Font, new VisualKey("WhiteSpace"), Color.Black, 0, 255, new VisualKey(""), new VisualKey(""), new VisualKey(""), 0, 0, BitmapFont.TextAlignment.Center, new List<Enums.EAnchorType>(), true);

            w.TipTextBox = s;

            GuiManager.GenericSheet.AddControl(w);
            GuiManager.GenericSheet.AddControl(wTitle);
            GuiManager.GenericSheet.AddControl(border);
            GuiManager.GenericSheet.AddControl(s);

            string[] tipInfo = TextManager.GetRandomHintText();
            s.AddLine(tipInfo[1], Enums.ETextType.Hint);
            w.WindowTitle.Text = tipInfo[0] + " Tip";

            Audio.AudioManager.PlaySoundEffect("GUISounds/tip_notification");
        }

        public static void CreateSageAdviceHintWindow(string advice)
        {
            // HintWindow exists, if this is called then another hint is desired.
            if (GuiManager.GenericSheet["SageAdviceTipWindow"] is TipWindow existingWindow)
            {
                existingWindow.OnClose();
                //if (existingWindow[existingWindow.Name + "ScrollableTextBox"] is ScrollableTextBox existingTextBox)
                //{
                //    existingTextBox.Clear();
                //    existingTextBox.AddLine(advice, Enums.ETextType.SageAdvice);
                //    existingWindow.WindowTitle.Text = "Sage Advice";
                //    existingWindow.IsVisible = true;
                //}
                //return;
            }

            TipWindow w = new TipWindow("SageAdviceTipWindow", "", new Rectangle(Client.Width / 2 - 150, 50, 300, 250), true, false, false,
                GuiManager.GenericSheet.Font, new VisualKey("WhiteSpace"), Color.Black, 180, true, Map.Direction.Northwest, 5, new List<Enums.EAnchorType>() { Enums.EAnchorType.Center }, "Dragging");

            WindowTitle wTitle = new WindowTitle(w.Name + "Title", w.Name, "changaone14", "Sage Advice", Color.PaleGreen, Color.DarkGreen,
                255, BitmapFont.TextAlignment.Center, new VisualKey("WhiteSpace"), false,
                new VisualKey("WindowCloseBox"), new VisualKey("WindowCloseBoxDown"), 18, 2, 17, 17, Color.LightGreen, 20)
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

            Audio.AudioManager.PlaySoundEffect("GUISounds/sageadvice_cymbal");
        }

        public override void OnClose()
        {
            base.OnClose();

            GuiManager.GenericSheet.RemoveControl(this);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            int height = BitmapFont.ActiveFonts[TipTextBox.Font].LineHeight * TipTextBox.FormattedLinesCount;

            TipTextBox.Height = height;

            Height = WindowTitle.Height + WindowBorder.Height + TipTextBox.Height + 4;
        }
    }
}
