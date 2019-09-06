using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class IconImageSelectionButton : HotButton
    {
        public IconImageSelectionButton(string name, string owner, Rectangle rectangle, string text, bool textVisible, Color textColor, bool visible, bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha, VisualKey visualKeyOver, VisualKey visualKeyDown, VisualKey visualKeyDisabled, string onMouseDownEvent, BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, Color textOverColor, bool hasTextOverColor, Color tintOverColor, bool hasTintOverColor, List<Enums.EAnchorType> anchors, bool dropShadow, Map.Direction shadowDirection, int shadowDistance, string command)
            : base(name, owner, rectangle, text, textVisible, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha, textAlpha, visualKeyOver, visualKeyDown, visualKeyDisabled, onMouseDownEvent, textAlignment, xTextOffset, yTextOffset, textOverColor, hasTextOverColor, tintOverColor, hasTintOverColor, anchors, dropShadow, shadowDirection, shadowDistance, command, "")
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

                if (GuiManager.CurrentSheet[Owner] is HotButtonEditWindow owner)
                {
                    owner.SelectedIconLabel.VisualKey = VisualKey;
                    owner.SelectedVisualKey = VisualKey;
                }
            }
        }

        protected override void OnMouseOver(MouseState ms)
        {
            if (GuiManager.CurrentSheet[Owner] is HotButtonEditWindow owner)
            {
                if (owner["HotButtonEditWindowPreviewIconLabel"] is Label previewLabel)
                {
                    if (previewLabel.VisualKey != VisualKey)
                        previewLabel.VisualKey = VisualKey;
                }

                if (owner["SelectedIconNameLabel"] is Label selectedNameLabel)
                {
                    selectedNameLabel.Text = VisualKey;
                }
            }

            if (Border != null)
                Border.IsVisible = true;

            base.OnMouseOver(ms);
        }

        protected override void OnMouseLeave(MouseState ms)
        {
            if (GuiManager.CurrentSheet[this.Owner] is HotButtonEditWindow owner)
            {
                owner.SelectedIconLabel.VisualKey = owner.SelectedVisualKey;

                if (owner["HotButtonEditWindowPreviewIconLabel"] is Label previewLabel)
                    previewLabel.Text = "";

                if (owner["SelectedIconNameLabel"] is Label selectedNameLabel)
                    selectedNameLabel.Text = "";
            }

            if (Border != null)
                Border.IsVisible = false;

            base.OnMouseLeave(ms);
        }
    }
}
