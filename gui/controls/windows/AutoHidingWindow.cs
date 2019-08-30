using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{

    public class AutoHidingWindow : Window
    {
        public Enums.EAnchorType WindowTitleOrientation
        { get; set; }
        public int AutoHideVisualAlpha
        { get; set; }
        public int OriginalVisualAlpha
        { get; set; }
        public bool FadeIn
        { get; set; }
        public bool FadeOut
        { get; set; }
        public int FadeSpeed
        { get; set; }

        public AutoHidingWindow(string name, string owner, Rectangle rectangle, bool visible, bool locked, bool disabled, string font,
            VisualKey visualKey, Color tintColor, byte visualAlpha, bool dropShadow, Map.Direction shadowDirection, int shadowDistance,
            List<Enums.EAnchorType> anchors, string cursorOverride, Enums.EAnchorType titleOrientation, int autoHideVisualAlpha,
            bool fadeIn, bool fadeOut, int fadeSpeed) : base(name, owner, rectangle, visible, locked, disabled, font, visualKey, tintColor, visualAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride)
        {
            WindowTitleOrientation = titleOrientation;
            OriginalVisualAlpha = visualAlpha;
            AutoHideVisualAlpha = autoHideVisualAlpha;
            FadeIn = fadeIn;
            FadeOut = fadeOut;
            FadeSpeed = fadeSpeed;            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if((AutoHideVisualAlpha == 0 && WindowTitle != null && WindowTitle.Contains(GuiManager.MouseState.Position)) ||
                (AutoHideVisualAlpha > 0 && ControlState == Enums.EControlState.Over))
            {
                if (AutoHideVisualAlpha == 0)
                {
                    if (m_cropped)
                        OnCrop();
                }
                else
                {
                    if(!FadeIn)
                        VisualAlpha = OriginalVisualAlpha;
                    else
                    {
                        VisualAlpha += FadeSpeed;
                        if (VisualAlpha > OriginalVisualAlpha)
                            VisualAlpha = OriginalVisualAlpha;
                    }

                    foreach (Control c in Controls)
                    {
                        c.VisualAlpha = VisualAlpha;
                        c.TextAlpha = VisualAlpha;
                    }
                }
            }

            if (!m_cropped && ControlState != Enums.EControlState.Over)
            {
                if (AutoHideVisualAlpha == 0)
                    OnCrop();
                else
                {
                    if(!FadeOut)
                        VisualAlpha = AutoHideVisualAlpha;
                    else
                    {
                        VisualAlpha -= FadeSpeed;
                        if (VisualAlpha < AutoHideVisualAlpha)
                            VisualAlpha = AutoHideVisualAlpha;
                    }

                    foreach (Control c in Controls)
                        c.VisualAlpha = VisualAlpha;
                }
            }
        }

        public override bool MouseHandler(MouseState ms)
        {
            if (m_cropped || VisualAlpha == 0)
                return false;
            else
                return base.MouseHandler(ms);
        }

        protected override void OnMouseLeave(MouseState ms)
        {
            base.OnMouseLeave(ms);

            ControlState = Enums.EControlState.Normal;
        }

        public void SetWindowOrientation()
        {
            switch (WindowTitleOrientation)
            {
                case Enums.EAnchorType.Bottom:
                    WindowTitle.Position = new Point(WindowTitle.Position.X, Position.Y + this.Height - WindowTitle.Height);
                    break;
                case Enums.EAnchorType.Left:
                    WindowTitle.Width = WindowTitle.Height;
                    WindowTitle.Height = Height;
                    break;
                case Enums.EAnchorType.Right:
                    WindowTitle.Position = new Point(WindowTitle.Position.X + Width - Height, WindowTitle.Position.Y);
                    WindowTitle.Width = WindowTitle.Height;
                    WindowTitle.Height = Height;
                    break;
                case Enums.EAnchorType.Center:
                    break;
            }
        }
    }
}
