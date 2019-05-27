using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class WindowTitle : Control
    {
        #region Private Data
        readonly WindowControlBox m_closeBox;
        readonly WindowControlBox m_maximizeBox;
        readonly WindowControlBox m_minimizeBox;
        readonly WindowControlBox m_cropBox;
        readonly int m_height;
        #endregion

        #region Public Properties
        public WindowControlBox CloseBox
        {
            get { return m_closeBox; }
        }
        public WindowControlBox MaximizeBox
        {
            get { return m_maximizeBox; }
        }
        public WindowControlBox MinimizeBox
        {
            get { return m_minimizeBox; }
        }
        public WindowControlBox CropBox
        {
            get { return m_cropBox; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Instance of a WindowTitle with only a close box.
        /// </summary>
        public WindowTitle(string name, string owner, string font, string text, Color textColor, Color tintColor, byte visualAlpha,
            BitmapFont.TextAlignment textAlignment, VisualKey visualKey, bool visualTiled, VisualKey closeBoxVisualKey, VisualKey closeBoxVisualKeyDown,
            int closeBoxDistanceFromRight, int closeBoxDistanceFromTop, int closeBoxWidth, int closeBoxHeight,
            Color closeBoxTintColor, params object[] obj)
            : base()
        {
            m_name = name;
            m_owner = owner;
            m_text = text;
            m_textColor = textColor;
            m_textAlignment = textAlignment;
            m_font = font;
            m_visualKey = visualKey;
            m_visualTiled = visualTiled;
            m_visible = true;
            m_rectangle = new Rectangle(0, 0, 100, 14);
            m_tintColor = tintColor;
            m_visualAlpha = visualAlpha;
            m_height = Client.UserSettings.DefaultWindowTitleHeight;
            if (closeBoxVisualKey.Key != "")
                m_closeBox = new WindowControlBox(m_owner, Enums.EWindowControlBoxType.Close, closeBoxDistanceFromRight,
                    closeBoxDistanceFromTop, closeBoxWidth, closeBoxHeight, closeBoxVisualKey, closeBoxVisualKeyDown, closeBoxTintColor, visualAlpha);
            else m_closeBox = null;
            m_maximizeBox = null;
            m_minimizeBox = null;
            m_cropBox = null;

            if (obj != null)
                m_height = (int)obj[0];
        }

        /// <summary>
        /// Instance of a WindowTitle with all associated boxes.
        /// </summary>
        public WindowTitle(string name, string owner, string font, string text, Color textColor, Color tintColor, byte visualAlpha,
            BitmapFont.TextAlignment textAlignment, VisualKey visualKey, bool visualTiled, VisualKey closeBoxVisualKey,
            VisualKey maxBoxVisualKey, VisualKey minBoxVisualKey, VisualKey cropBoxVisualKey, VisualKey closeBoxVisualKeyDown,
            VisualKey maxBoxVisualKeyDown, VisualKey minBoxVisualKeyDown, VisualKey cropBoxVisualKeyDown,
            int closeBoxDistanceFromRight, int closeBoxDistanceFromTop, int closeBoxWidth, int closeBoxHeight, int closeBoxVisualAlpha,
            int maxBoxDistanceFromRight, int maxBoxDistanceFromTop, int maxBoxWidth, int maxBoxHeight, int maxBoxVisualAlpha,
            int minBoxDistanceFromRight, int minBoxDistanceFromTop, int minBoxWidth, int minBoxHeight, int minBoxVisualAlpha,
            int cropBoxDistanceFromRight, int cropBoxDistanceFromTop, int cropBoxWidth, int cropBoxHeight, int cropBoxVisualAlpha,
            Color closeBoxTintColor, Color maximizeBoxTintColor, Color minBoxTintColor, Color cropBoxTintColor, params object[] obj)
            : base()
        {
            m_name = name;
            m_owner = owner;
            m_text = text;
            m_textColor = textColor;
            m_textAlignment = textAlignment;
            m_font = font;
            m_visualKey = visualKey;
            m_visualTiled = visualTiled;
            m_visible = true;
            m_rectangle = new Rectangle(0, 0, 100, 14);
            m_tintColor = tintColor;
            m_visualAlpha = visualAlpha;
            m_height = Client.UserSettings.DefaultWindowTitleHeight;
            if (closeBoxVisualKey.Key != "")
                m_closeBox = new WindowControlBox(m_owner, Enums.EWindowControlBoxType.Close, closeBoxDistanceFromRight,
                    closeBoxDistanceFromTop, closeBoxWidth, closeBoxHeight, closeBoxVisualKey, closeBoxVisualKeyDown, closeBoxTintColor,
                    closeBoxVisualAlpha);
            else m_closeBox = null;
            if (maxBoxVisualKey.Key != "")
                m_maximizeBox = new WindowControlBox(m_owner, Enums.EWindowControlBoxType.Maximize, maxBoxDistanceFromRight,
                    maxBoxDistanceFromTop, maxBoxWidth, maxBoxHeight, maxBoxVisualKey, maxBoxVisualKeyDown, maximizeBoxTintColor,
                    maxBoxVisualAlpha);
            else m_maximizeBox = null;
            if (minBoxVisualKey.Key != "")
                m_minimizeBox = new WindowControlBox(m_owner, Enums.EWindowControlBoxType.Minimize, minBoxDistanceFromRight,
                    minBoxDistanceFromTop, minBoxWidth, minBoxHeight, minBoxVisualKey, minBoxVisualKeyDown, minBoxTintColor,
                    minBoxVisualAlpha);
            else m_minimizeBox = null;
            if (cropBoxVisualKey.Key != "")
                m_cropBox = new WindowControlBox(m_owner, Enums.EWindowControlBoxType.Crop, cropBoxDistanceFromRight,
                    cropBoxDistanceFromTop, cropBoxWidth, cropBoxHeight, cropBoxVisualKey, cropBoxVisualKeyDown, cropBoxTintColor,
                    cropBoxVisualAlpha);
            else m_cropBox = null;
        } 
        #endregion

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Control control = GuiManager.GetControl(m_owner);

            if (control != null)
            {
                Point p = control.Position;
                m_rectangle = new Rectangle(p.X, p.Y, control.Width, m_height);
            }

            if (m_closeBox != null)
                m_closeBox.Update(gameTime);
            if (m_maximizeBox != null)
                m_maximizeBox.Update(gameTime);
            if (m_minimizeBox != null)
                m_minimizeBox.Update(gameTime);
            if (m_cropBox != null)
                m_cropBox.Update(gameTime);

        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            #region Draw Control Boxes
            if (m_closeBox != null)
                m_closeBox.Draw(gameTime);
            if (m_maximizeBox != null)
                m_maximizeBox.Draw(gameTime);
            if (m_minimizeBox != null)
                m_minimizeBox.Draw(gameTime);
            if (m_cropBox != null)
                m_cropBox.Draw(gameTime);
            #endregion

            #region Draw Text
            Color textColor;

            if (!m_disabled)
            {
                textColor = new Color(m_textColor.R, m_textColor.G, m_textColor.B, m_textAlpha);
            }
            else
            {
                textColor = new Color(Control.s_disabledColor.R, Control.s_disabledColor.G,
                    Control.s_disabledColor.B, m_textAlpha);
            }

            // override BitmapFont sprite batch
            BitmapFont.ActiveFonts[Font].SpriteBatchOverride(Client.SpriteBatch);
            // set font alignment
            BitmapFont.ActiveFonts[Font].Alignment = m_textAlignment;
            // draw string in textbox
            Rectangle rect = new Rectangle(m_rectangle.X + m_xTextOffset, m_rectangle.Y + m_yTextOffset, m_rectangle.Width, m_rectangle.Height);
            // change color of text if mouse over text color is not null
            if (m_text != null && m_text.Length > 0)
            {
                BitmapFont.ActiveFonts[Font].TextBox(rect, textColor, m_text);
            }
            #endregion
        }

        protected override void OnMouseDown(MouseState ms)
        {
            Point pt = new Point(ms.X, ms.Y);

            if (m_closeBox != null && m_closeBox.Contains(pt))
                m_closeBox.OnMouseDown(ms);
            else if (m_maximizeBox != null && m_maximizeBox.Contains(pt))
                m_maximizeBox.OnMouseDown(ms);
            else if (m_minimizeBox != null && m_minimizeBox.Contains(pt))
                m_minimizeBox.OnMouseDown(ms);
            else if (m_cropBox != null && m_cropBox.Contains(pt))
                m_cropBox.OnMouseDown(ms);
        }

        protected override void OnMouseRelease(MouseState ms)
        {
            Point pt = new Point(ms.X, ms.Y);

            if (m_closeBox != null && m_closeBox.Contains(pt))
                m_closeBox.OnMouseRelease(ms);
            else if (m_maximizeBox != null && m_maximizeBox.Contains(pt))
                m_maximizeBox.OnMouseRelease(ms);
            else if (m_minimizeBox != null && m_minimizeBox.Contains(pt))
                m_minimizeBox.OnMouseRelease(ms);
            else if (m_cropBox != null && m_cropBox.Contains(pt))
                m_cropBox.OnMouseRelease(ms);
        }

        public bool ControlBoxContains(Point p)
        {
            if (m_closeBox != null && m_closeBox.Contains(p))
                return true;
            else if (m_maximizeBox != null && m_maximizeBox.Contains(p))
                return true;
            else if (m_minimizeBox != null && m_minimizeBox.Contains(p))
                return true;
            else if (m_cropBox != null && m_cropBox.Contains(p))
                return true;

            return false;
        }
    }
}
