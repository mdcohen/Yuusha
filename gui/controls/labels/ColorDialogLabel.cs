using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class ColorDialogLabel : Label
    {
        private System.Windows.Forms.ColorDialog m_colorDialog;

        public ColorDialogLabel(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible, bool disabled, string font, VisualKey visualKey,
            Color tintColor, byte visualAlpha, byte borderAlpha, byte textAlpha, BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string cursorOverride,
            List<Enums.EAnchorType> anchors, string popUpText) : base(name, owner, rectangle, text, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha, borderAlpha, textAlpha, textAlignment, xTextOffset, yTextOffset, "", cursorOverride, anchors, popUpText)
        {
            
        }

        protected override void OnMouseDown(MouseState ms)
        {
            base.OnMouseDown(ms);

            if (ms.LeftButton == ButtonState.Pressed)
            {
                m_colorDialog = new System.Windows.Forms.ColorDialog();
                m_colorDialog.ShowDialog();
            }
        }
        public override void Update(GameTime gameTime)
        {
            if (!IsVisible || IsDisabled)
            {
                if (m_colorDialog != null)
                    m_colorDialog = null;
            }

            base.Update(gameTime);
        }
    }
}
