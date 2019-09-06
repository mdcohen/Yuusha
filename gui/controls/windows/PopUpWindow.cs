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

        public static void CreateVolumeControlPopUpWindow()
        {
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
