using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class ColorDialogButton : Button
    {
        public Border Border { get; set; }
        private System.Windows.Forms.ColorDialog m_colorDialog;

        public ColorDialogButton(string name, string owner, Rectangle rectangle, bool visible, bool disabled, string font, Color tintColor, string cursorOverride, List<Enums.EAnchorType> anchors, bool dropShadow, Map.Direction shadowDirection, int shadowDistance, string popUpText) :
            base(name, owner, rectangle, "", false, Color.Black, visible, disabled, font, new gui.VisualKey("WhiteSpace"), tintColor, 255, 0, new VisualKey(""), new VisualKey(""), new VisualKey(""), "", BitmapFont.TextAlignment.Left, 0, 0, Color.White, false, Color.White, false, anchors, dropShadow, shadowDirection, shadowDistance, "", popUpText)
        { 
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsVisible || IsDisabled)
            {
                if (m_colorDialog != null)
                    m_colorDialog = null;
            }

            base.Update(gameTime);

            if (Border != null) Border.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (Border != null) Border.Draw(gameTime);
        }

        protected override void OnMouseDown(MouseState ms)
        {
            base.OnMouseDown(ms);

            if (m_colorDialog == null && ms.LeftButton == ButtonState.Pressed)
            {
                m_colorDialog = new System.Windows.Forms.ColorDialog
                {
                    AllowFullOpen = true,
                };

                if (m_colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    TintColor = new Color(m_colorDialog.Color.R, m_colorDialog.Color.G, m_colorDialog.Color.B);
                }

                m_colorDialog.Dispose();
                m_colorDialog = null;
            }
        }
    }
}
