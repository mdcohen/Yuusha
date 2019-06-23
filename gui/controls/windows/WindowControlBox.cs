using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Yuusha.gui
{
    public class WindowControlBox : Control
    {
        private readonly Enums.EWindowControlBoxType m_controlBoxType; // Close, Crop, Maximize, Minimize
        private int m_distanceFromRight; // distance from right of WindowTitle
        private int m_distanceFromTop; // distance from top of WindowTitle

        public WindowControlBox(string owner, Enums.EWindowControlBoxType controlBoxType,
            int distanceFromRight, int distanceFromTop, int width, int height, VisualKey visualKey,
            VisualKey visualKeyDown, Color tintColor, int visualAlpha)
            : base()
        {
            m_owner = owner; // the Window this control box belongs to
            m_controlBoxType = controlBoxType;
            m_distanceFromRight = distanceFromRight;
            m_distanceFromTop = distanceFromTop;
            m_rectangle = new Rectangle(0, 0, width, height); // x and y are set with SetRectangle()
            m_visualKey = visualKey;
            m_visuals.Add(Enums.EControlState.Normal, visualKey);
            m_visuals.Add(Enums.EControlState.Down, visualKeyDown);
            m_tintColor = tintColor;
            m_visualAlpha = visualAlpha;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            UpdateRectangle();

            if (!Contains(new Point(GuiManager.MouseState.X, GuiManager.MouseState.Y)))
                m_controlState = Enums.EControlState.Normal;
        }

        public override void Draw(GameTime gameTime)
        {
            if (m_visuals.ContainsKey(m_controlState) && m_visuals[m_controlState].Key != "")
            {
                if (!GuiManager.Visuals.ContainsKey(m_visuals[m_controlState].Key))
                {
                    Utils.LogOnce("Failed to find visual key [ " + m_visuals[m_controlState] + " ] for WindowControlBox [ " + m_controlBoxType.ToString() + " ] of Window [ " + m_owner + "]");
                    m_visuals[m_controlState] = new VisualKey(""); // clear visual key
                    return;
                }

                VisualInfo vi = GuiManager.Visuals[m_visuals[m_controlState].Key];
                Rectangle sourceRect = new Rectangle(vi.X, vi.Y, vi.Width, vi.Height);

                if (m_dropShadow)
                {
                    Rectangle shadowRect = new Rectangle(m_rectangle.X + GetXShadow(), m_rectangle.Y + GetYShadow(), m_rectangle.Width, m_rectangle.Height);
                    Color shadowColor = new Color((int)Color.Black.R, (int)Color.Black.G, (int)Color.Black.B, 50);
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], shadowRect, sourceRect, shadowColor);
                }

                try
                {
                    if (!m_disabled)
                    {

                        Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_rectangle, sourceRect, m_tintColor);
                    }
                    else
                    {
                        Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_rectangle, sourceRect, new Color(ColorDisabledStandard.R, ColorDisabledStandard.G, ColorDisabledStandard.B, m_visualAlpha));
                    }
                }
                catch
                {
                    Utils.LogOnce("Failed to SpriteBatch.Draw texture [ " + vi.ParentTexture + "]");
                }
            }
            else
            {
                base.Draw(gameTime);
            }
        }

        public new void OnMouseDown(MouseState ms)
        {
            if(!GuiManager.Dragging)
                m_controlState = Enums.EControlState.Down;
        }

        public new void OnMouseRelease(MouseState ms)
        {
            if (m_controlState == Enums.EControlState.Down)
            {
                if (GuiManager.GetControl(m_owner) is Window window)
                {
                    switch (m_controlBoxType)
                    {
                        case Enums.EWindowControlBoxType.Close:
                            window.OnClose();
                            break;
                        case Enums.EWindowControlBoxType.Maximize:
                            window.OnMaximize();
                            break;
                        case Enums.EWindowControlBoxType.Minimize:
                            window.OnMinimize();
                            break;
                        case Enums.EWindowControlBoxType.Crop:
                            window.OnCrop();
                            break;
                    }
                }
            }

            m_controlState = Enums.EControlState.Normal;
        }

        private void UpdateRectangle()
        {
            Control control = GuiManager.GetControl(m_owner); // the Window this control box belongs to

            if (control != null)
            {
                if ((control as Window).WindowTitle != null)
                {
                    Point p = (control as Window).WindowTitle.Position;
                    int w = (control as Window).WindowTitle.Width;
                    m_rectangle = new Rectangle(p.X + w - m_distanceFromRight, p.Y + m_distanceFromTop, m_rectangle.Width, m_rectangle.Height);
                }
            }
        }
    }
}
