using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class IconImageSelectionButton : HotButton
    {
        public IconImageSelectionButton(string name, string owner, Rectangle rectangle, string text, bool textVisible, Color textColor, bool visible, bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, byte textAlpha, VisualKey visualKeyOver, VisualKey visualKeyDown, VisualKey visualKeyDisabled, string onMouseDownEvent, BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, Color textOverColor, bool hasTextOverColor, List<Enums.EAnchorType> anchors, bool dropShadow, Map.Direction shadowDirection, int shadowDistance, string command) : base(name, owner, rectangle, text, textVisible, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha, borderAlpha, textAlpha, visualKeyOver, visualKeyDown, visualKeyDisabled, onMouseDownEvent, textAlignment, xTextOffset, yTextOffset, textOverColor, hasTextOverColor, anchors, dropShadow, shadowDirection, shadowDistance, command)
        {
        }

        protected override bool OnKeyDown(KeyboardState ks)
        {
            return false;
        }

        protected override void OnMouseDown(MouseState ms)
        {
            if (ms.LeftButton == ButtonState.Pressed)
            {
                m_visualAlpha = 0;

                HotButtonEditWindow owner = GuiManager.CurrentSheet[this.Owner] as HotButtonEditWindow;

                if (owner != null)
                {
                    owner.SelectedIconLabel.VisualKey = this.VisualKey;
                    owner.SelectedVisualKey = this.VisualKey;
                }
            }
        }

        protected override void OnMouseOver(MouseState ms)
        {
            HotButtonEditWindow owner = GuiManager.CurrentSheet[this.Owner] as HotButtonEditWindow;

            if (owner != null)
            {
                Label previewLabel = owner["HotButtonEditWindowPreviewIconLabel"] as Label;

                if (previewLabel != null)
                {
                    if (previewLabel.VisualKey != this.VisualKey)
                    {
                        previewLabel.VisualKey = this.VisualKey;
                        //previewLabel.Text = this.VisualKey.ToString();
                    }
                }
            }
        }

        protected override void OnMouseLeave(MouseState ms)
        {
            HotButtonEditWindow owner = GuiManager.CurrentSheet[this.Owner] as HotButtonEditWindow;

            if (owner != null)
            {
                owner.SelectedIconLabel.VisualKey = owner.SelectedVisualKey;

                Label previewLabel = owner["HotButtonEditWindowPreviewIconLabel"] as Label;

                if (previewLabel != null)
                {
                    previewLabel.Text = "";
                }
            }
        }
    }
}
