using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Yuusha.gui
{
    public class PrivateMessageWindow : Window
    {
        public string RecipientName { get; set; }
        public ScrollableTextBox MessageBox { get; set; }

        public PrivateMessageWindow(string name, string owner, Rectangle rectangle, bool visible, bool locked, bool disabled,
            string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, bool dropShadow,
            Map.Direction shadowDirection, int shadowDistance, List<Enums.EAnchorType> anchors, string cursorOverride) : base(name, owner, rectangle, visible, locked, disabled, font, visualKey, tintColor, visualAlpha, borderAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride)
        {

        }

        public static PrivateMessageWindow CreateNewPrivateMessageWindow(string player)
        {
            if (GuiManager.GenericSheet[player + "PrivateMessageWindow"] != null)
                return GuiManager.GenericSheet[player + "PrivateMessageWindow"] as PrivateMessageWindow;

            PrivateMessageWindow pmWindow = new PrivateMessageWindow(player + "PrivateMessageWindow", "", new Rectangle(100, 100, 500, 300), true, false, false, GuiManager.GenericSheet.Font, new VisualKey("WhiteSpace"),
                Client.UserSettings.PrivateMessageWindowTintColor, 255, 255,
                true, Map.Direction.Northwest, 5, new List<Enums.EAnchorType> { Enums.EAnchorType.Top, Enums.EAnchorType.Right }, "Dragging");
            pmWindow.RecipientName = player;

            WindowTitle windowTitle = new WindowTitle(pmWindow.Name + "Title", pmWindow.Name, pmWindow.Font, "Private Message: " + pmWindow.RecipientName, Client.UserSettings.PrivateMessageWindowTitleTextColor, Client.UserSettings.PrivateMessageWindowTitleTintColor, 255,
                BitmapFont.TextAlignment.Center, new VisualKey("WhiteSpace"), false, new VisualKey("WindowCloseBox"), new VisualKey(""), new VisualKey(""),
                new VisualKey("WindowCropBox"), new VisualKey("WindowCloseBoxDown"), new VisualKey(""), new VisualKey(""), new VisualKey("WindowCropBoxDown"), Client.UserSettings.PrivateMessageWindowTitleCloseBoxDistanceFromRight, Client.UserSettings.PrivateMessageWindowTitleCloseBoxDistanceFromTop,
                Client.UserSettings.PrivateMessageWindowTitleCloseBoxWidth, Client.UserSettings.PrivateMessageWindowTitleCloseBoxHeight, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                Client.UserSettings.PrivateMessageWindowTitleCropBoxDistanceFromRight, Client.UserSettings.PrivateMessageWindowTitleCropBoxDistanceFromTop, Client.UserSettings.PrivateMessageWindowTitleCropBoxWidth,
                Client.UserSettings.PrivateMessageWindowTitleCropBoxHeight, 255, Client.UserSettings.PrivateMessageWindowTitleCloseBoxTintColor, Color.White, Color.White,
                Client.UserSettings.PrivateMessageWindowTitleCropBoxTintColor, Client.UserSettings.PrivateMessageWindowTitleHeight);

            SquareBorder windowBorder = new SquareBorder(pmWindow.Name + "Border", pmWindow.Name, 1, new VisualKey("WhiteSpace"), false, Client.UserSettings.PrivateMessageBorderTintColor, 255);

            ScrollableTextBox scrollBox = new ScrollableTextBox(pmWindow.Name + "ScrollableTextBox", pmWindow.Name, new Rectangle(2, windowTitle.Height, 496, 260), "", Color.White,
                true, false, Client.ClientSettings.DefaultFont, new VisualKey("WhiteSpace"), Color.Black, 255, 0, 255, new VisualKey(""), new VisualKey(""), new VisualKey(""), 0, 0, BitmapFont.TextAlignment.Left,
                new List<Enums.EAnchorType>() { Enums.EAnchorType.Top, Enums.EAnchorType.Left, Enums.EAnchorType.Right, Enums.EAnchorType.Bottom }, true);
            pmWindow.MessageBox = scrollBox;

            TextBox textBox = new TextBox(pmWindow.Name + "TextBox", pmWindow.Name, new Rectangle(2, 279, 496, 19), "", Color.White,
                BitmapFont.TextAlignment.Left, true, false, pmWindow.Font, new VisualKey("WhiteSpace"), Color.CornflowerBlue, 255, 0, 255, true, 300, false, true, Color.WhiteSmoke, new VisualKey(""), new VisualKey(""), new VisualKey(""),
                0, 0, "send_tell", Color.Navy, new List<Enums.EAnchorType>() { Enums.EAnchorType.Left, Enums.EAnchorType.Right, Enums.EAnchorType.Bottom }, 0);

            GuiManager.GenericSheet.AddControl(pmWindow);
            GuiManager.GenericSheet.AttachControlToWindow(windowTitle);
            GuiManager.GenericSheet.AttachControlToWindow(windowBorder);
            GuiManager.GenericSheet.AttachControlToWindow(scrollBox);
            GuiManager.GenericSheet.AttachControlToWindow(textBox);

            return pmWindow;
        }

        public void ReceivedMessage(string message)
        {
            string messageReceived = message.Replace(RecipientName + " tells you, ", "");
            messageReceived = messageReceived.Substring(1, messageReceived.LastIndexOf('"') - 1); // removes quotation marks
            bool firstMessage = MessageBox.LinesCount <= 0;
            MessageBox.AddLine(RecipientName + ": " + messageReceived, Enums.ETextType.PrivateMessageSender);
            // play sound
            if (Client.ClientSettings.PlayPrivateMessageSounds)
            {
                if (firstMessage || !IsVisible)
                    Sound.Play("GUISounds/pm_alert");
                else Sound.Play("GUISounds/pm_alert_short");
            }
            IsVisible = true;
        }

        public void SentMessage(string message)
        {
            string messageSent = message.Replace("You tell " + RecipientName + ", ", "");
            messageSent = messageSent.Substring(1, messageSent.LastIndexOf('"') - 1); // removes quotation marks
            MessageBox.AddLine("You: " + messageSent, Enums.ETextType.PrivateMessageReceiver);
        }

        public override void OnClose()
        {
            foreach(Control c in new List<Control>(Controls))
            {
                Controls.Remove(c);
                GuiManager.GenericSheet.RemoveControl(c);
            }

            GuiManager.GenericSheet.RemoveControl(this);

            base.OnClose();
        }
    }
}
