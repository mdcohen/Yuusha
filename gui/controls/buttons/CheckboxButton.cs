using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class CheckboxButton : Button
    {
        public bool IsChecked
        { get; set; }
        public Color VisualKeySelectedColor
        { get; set; }

        private bool m_checkOperation;
        private Color m_originalTintColor;

        public CheckboxButton(string name, string owner, Rectangle rectangle, bool visible, bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, VisualKey visualKeyOver, VisualKey visualKeyDown, VisualKey visualKeyDisabled, VisualKey visualKeySelected, Color visualKeySelectedTintColor, Color tintOverColor, bool hasTintOverColor, List<Enums.EAnchorType> anchors, bool dropShadow, Map.Direction shadowDirection, int shadowDistance, string popUpText)
            : base(name, owner, rectangle, "", false, Color.Black, visible, disabled, font, visualKey, tintColor, visualAlpha, 0, visualKeyOver, visualKeyDown, visualKeyDisabled, "", BitmapFont.TextAlignment.Left, 0, 0, Color.White, false, tintOverColor, hasTintOverColor, anchors, dropShadow, shadowDirection, shadowDistance, "", popUpText)
        {
            IsChecked = false;
            if (visualKeySelected.Key != "")
                m_visuals.Add(Enums.EControlState.Selected, visualKeySelected);
            VisualKeySelectedColor = visualKeySelectedTintColor;
            m_checkOperation = false;
            m_originalTintColor = tintColor;
        }

        public override void Draw(GameTime gameTime)
        {
            if (!IsVisible)
                return;

            base.Draw(gameTime);

            if (Border != null) Border.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (IsChecked)
            {
                m_visualKey = m_visuals[Enums.EControlState.Selected];
                TintColor = VisualKeySelectedColor;
            }
            else
            {
                m_visualKey = m_visuals[Enums.EControlState.Normal];
                TintColor = m_originalTintColor;
            }

            if (Border != null) Border.Update(gameTime);
        }

        protected override void OnMouseDown(MouseState ms)
        {
            if (IsDisabled) return;

            if (!m_checkOperation && ms.LeftButton == ButtonState.Pressed)
            {
                m_checkOperation = true;

                IsChecked = !IsChecked;

                if (IsChecked)
                {
                    if (m_visuals.ContainsKey(Enums.EControlState.Down))
                        m_visualKey = m_visuals[Enums.EControlState.Down];
                }
                else
                {
                    if (m_visuals.ContainsKey(Enums.EControlState.Normal))
                        m_visualKey = m_visuals[Enums.EControlState.Normal];
                }
            }
        }

        protected override void OnMouseRelease(MouseState ms)
        {
            if (IsDisabled) return;

            base.OnMouseRelease(ms);

            m_checkOperation = false;
        }
    }
}
