using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    abstract public class Control
    {
        public static Color s_disabledColor = Color.Gray;

        #region Private Data
        protected string m_sheet; // name of the sheet this control belongs to
        protected string m_owner; // owner of this control (Window)
        protected string m_name; // name of this control
        protected Enums.EControlState m_controlState; // control state
        protected string m_text; // control text
        protected Rectangle m_rectangle; // rectangle
        protected int m_zDepth; // z depth
        protected bool m_disabled; // disabled
        protected bool m_locked; // locked (prevents dragging)
        protected bool m_visible; // visible
        protected bool m_hasFocus; // has focus
        protected bool m_mouseLeftDown;
        protected bool m_mouseRightDown;
        protected bool m_hasTouchDownPoint; // has touch down point
        protected bool m_containsMousePointer;
        protected Point m_touchDownPoint; // touch down point
        protected Color m_textColor; // text color
        protected int m_textAlpha; // text alpha
        protected Color m_tintColor; // visual tint color
        protected int m_visualAlpha; // visual alpha
        protected int m_borderAlpha; // border alpha
        protected VisualKey m_visualKey; // visual key
        protected bool m_visualTiled = false; // visual key is tiled
        protected Dictionary<Enums.EControlState, VisualKey> m_visuals;
        protected int m_xTextOffset;
        protected int m_yTextOffset;
        protected string m_font;
        protected TimeSpan m_lastUpdate;
        protected object m_data;
        protected int m_leftClickCount;
        protected bool m_dropShadow;
        protected int m_shadowDistance;
        protected Map.Direction m_shadowDirection;
        protected List<Enums.EAnchorType> m_anchors;
        protected string m_onDoubleClickEvent;
        protected string m_cursorOverride;
        protected string m_onMouseDown;
        protected BitmapFont.TextAlignment m_textAlignment;
        protected Color m_textOverColor;
        protected bool m_hasTextOverColor;
        protected List<Enums.EGameState> m_lockoutStates; // for generic windows, states that this window is not available
        protected string m_popUpText;
        protected System.Timers.Timer m_doubleClickTimer;
        #endregion

        public event GuiManager.ControlDelegate OnControl;

        #region Public Properties
        public virtual string Sheet
        {
            get { return m_sheet; }
            set { m_sheet = value; }
        }

        public virtual string Owner
        {
            get { return m_owner; }
        }

        public virtual string Name
        {
            get { return m_name; }
        }

        public virtual object Data
        {
            get { return m_data; }
            set { m_data = value; }
        }

        public virtual Enums.EControlState ControlState
        {
            get { return m_controlState; }
            set { this.m_controlState = value; }
        }

        public virtual string Text
        {
            get { return m_text; }
            set { m_text = value; }
        }

        public virtual Color TextColor
        {
            get { return m_textColor; }
            set { m_textColor = value; }
        }

        public virtual Color TintColor
        {
            get { return m_tintColor; }
            set { m_tintColor = value; }
        }

        public virtual int ZDepth
        {
            get { return m_zDepth; }
            set { m_zDepth = value; }
        }

        public virtual Point Position
        {
            get { return new Point(m_rectangle.X, m_rectangle.Y); }
            set
            {
                m_rectangle.X = value.X;
                m_rectangle.Y = value.Y;
            }
        }

        public virtual int Height
        {
            get { return m_rectangle.Height; }
            set { m_rectangle.Height = value; }
        }

        public virtual int Width
        {
            get { return m_rectangle.Width; }
            set { m_rectangle.Width = value; }
        }

        public virtual bool IsDisabled
        {
            get { return m_disabled; }
            set { m_disabled = value; }
        }

        public virtual bool IsLocked
        {
            get { return m_locked; }
            set { m_locked = value; }
        }

        public virtual bool IsVisible
        {
            get { return m_visible; }
            set { m_visible = value; }
        }

        public virtual bool HasFocus
        {
            get { return m_hasFocus; }
            set
            {
                m_hasFocus = value;

                if (m_hasFocus && GuiManager.ControlWithFocus != this)
                    GuiManager.ControlWithFocus = this;

                //if (this is TextBox)
                //    GuiManager.ActiveTextBox = this.Name;
            }
        }

        public virtual string Font
        {
            get { return m_font; }
            set { m_font = value; }
        }

        public virtual string VisualKey
        {
            get { return m_visualKey.Key; }
            set { m_visualKey.Key = value; }
        }

        public virtual int VisualAlpha
        {
            get { return m_visualAlpha; }
            set { m_visualAlpha = value; }
        }

        public virtual int TextAlpha
        {
            get { return m_textAlpha; }
            set { m_textAlpha = value; }
        }

        public virtual List<Enums.EGameState> LockoutStates
        {
            get { return m_lockoutStates; }
            set { m_lockoutStates = value; }
        }
        #endregion

        #region Constructor
        public Control()
        {
            m_sheet = "";
            m_owner = "";
            m_name = "Unknown";
            m_controlState = Enums.EControlState.Normal;
            m_text = "";
            m_rectangle = new Rectangle();
            m_zDepth = 0;
            m_disabled = false;
            m_locked = true; // control cannot be dragged by default
            m_visible = false;
            m_hasFocus = false;
            m_hasTouchDownPoint = false;
            m_containsMousePointer = false;
            m_touchDownPoint = new Point();
            m_textColor = Color.White;
            m_tintColor = Color.White;
            m_visualAlpha = 255;
            m_textAlpha = 255;
            m_borderAlpha = 255;
            m_visualKey = null;
            m_visuals = new Dictionary<Enums.EControlState, VisualKey>();
            m_xTextOffset = 0;
            m_yTextOffset = 0;
            m_font = "";
            m_lastUpdate = new TimeSpan();
            m_dropShadow = false;
            m_shadowDistance = 10;
            m_anchors = new List<Enums.EAnchorType>();
            m_onDoubleClickEvent = "";
            m_cursorOverride = "";
            m_onMouseDown = "";
            m_textAlignment = BitmapFont.TextAlignment.Left;
            m_textOverColor = Color.White;
            m_hasTextOverColor = false;
            m_lockoutStates = new List<Enums.EGameState>();
            m_shadowDirection = Map.Direction.Southeast;
            m_popUpText = "";
            m_doubleClickTimer = new System.Timers.Timer(800);
            m_doubleClickTimer.Elapsed += new System.Timers.ElapsedEventHandler(DoubleClickTimer_Elapsed);
            m_doubleClickTimer.AutoReset = true;
        }
        #endregion

        private void DoubleClickTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            m_leftClickCount = 0;
            m_doubleClickTimer.Stop();
        }

        public virtual void Update(GameTime gameTime)
        {
            if (m_visuals.ContainsKey(ControlState) && m_visualKey != m_visuals[ControlState])
                m_visualKey = m_visuals[ControlState];

            // update colors using alpha settings
            m_textColor = new Color(m_textColor.R, m_textColor.G, m_textColor.B, this.TextAlpha);
            m_tintColor = new Color(m_tintColor.R, m_tintColor.G, m_tintColor.B, this.VisualAlpha);

            if (m_leftClickCount == 2)
            {
                m_leftClickCount = 0;
                m_doubleClickTimer.Stop();
                OnDoubleLeftClick();
            }
            else if (m_leftClickCount > 2)
            {
                m_leftClickCount = 0;
                m_doubleClickTimer.Stop();
            }

            m_lastUpdate = gameTime.TotalGameTime;
        }

        public virtual void Draw(GameTime gameTime)
        {
            if (!this.IsVisible)
                return;

            if (m_visualKey != null && m_visualKey.Key != "")
            {
                if (!GuiManager.Visuals.ContainsKey(m_visualKey.Key))
                {
                    Utils.LogOnce("Failed to find visual key [ " + m_visualKey + " ] for Control [ " + m_name + " ]");
                    m_visualKey.Key = ""; // clear visual key
                    return;
                }

                VisualInfo vi = GuiManager.Visuals[m_visualKey.Key];

                Color color = new Color(m_tintColor.R, m_tintColor.G, m_tintColor.B, this.VisualAlpha);

                if (m_disabled)
                    color = new Color(s_disabledColor.R, s_disabledColor.G, s_disabledColor.B, this.VisualAlpha);

                if (m_dropShadow)
                {
                    Rectangle shadowRect = new Rectangle(m_rectangle.X + GetXShadow(), m_rectangle.Y + GetYShadow(), m_rectangle.Width, m_rectangle.Height);
                    Color shadowColor = new Color((int)Color.Black.R, (int)Color.Black.G, (int)Color.Black.B, 50);
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], shadowRect, vi.Rectangle, shadowColor);
                }

                // Drawing.
                if (!m_visualTiled)
                {
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_rectangle, vi.Rectangle, color);
                    //Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], new Rectangle(0, 0, Client.Width, Client.Height), m_tintColor);
                }
                else // Tiled visual (borders, window titles)
                {
                    // This code needs some work. 2/18/2017. Have to move on.
                    int desiredWidth = (int)(this.Width / vi.Width);
                    int desiredHeight = this.Height;

                    // What uses tiled visuals?
                    int xAmount = (int)(this.Width / desiredWidth);
                    int yAmount = (int)(this.Height / desiredHeight);

                    int countWidth = 0;
                    int countHeight = 0;

                    // this goes columns first, then rows.
                    // may have to modify for things such as vertical borders/titles

                    for (int x = 0; countWidth <= this.Width; x++, countWidth += desiredWidth)
                    {
                        for (int y = 0; countHeight <= this.Height; y++, countHeight += desiredHeight)
                        {
                            Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], new Rectangle(x * desiredWidth, y * desiredHeight, desiredWidth, desiredHeight), vi.Rectangle, color);
                        }
                    }
                }

                //Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_rectangle, vi.Rectangle, color);
            }
        }

        public virtual bool KeyboardHandler(KeyboardState ks)
        {
            if (!Client.HasFocus) return false;

            return OnKeyDown(ks);
        }

        public virtual bool MouseHandler(MouseState ms)
        {
            if (!Client.HasFocus) return false;

            Point mousePointer = new Point(ms.X, ms.Y); // point of the mouse

            if (Contains(mousePointer) && !m_containsMousePointer)
            {
                m_containsMousePointer = true;
                OnMouseOver(ms);
            }

            if (!Contains(mousePointer) && m_containsMousePointer)
            {
                m_containsMousePointer = false;
                OnMouseLeave(ms);
            }

            // current sheet only (excludes generic sheet)
            if (this.Sheet == GuiManager.CurrentSheet.Name)
            {
                foreach (Control c in GuiManager.GenericSheet.Controls)
                {
                    if (c.IsVisible && c.Contains(mousePointer))
                        return false;
                }
            }

            if (ms.LeftButton == ButtonState.Pressed || ms.RightButton == ButtonState.Pressed)
            {
                m_mouseLeftDown = ms.LeftButton == ButtonState.Pressed;
                m_mouseRightDown = ms.RightButton == ButtonState.Pressed;
            }

            if (!m_hasTouchDownPoint && (ms.LeftButton == ButtonState.Pressed || ms.RightButton == ButtonState.Pressed))
            {
                m_touchDownPoint = new Point(ms.X, ms.Y);
                m_hasTouchDownPoint = true;
            }
            else if (m_hasTouchDownPoint && (ms.LeftButton != ButtonState.Pressed && ms.RightButton != ButtonState.Pressed))
            {
                m_hasTouchDownPoint = false;
            }

            // Not a TextBox, contains mouse cursor or has focus.
            // OR
            // Textbox and contains mouse cursor.
            if ((!(this is TextBox) && (Contains(mousePointer) || HasFocus)) || ((this is TextBox) && Contains(mousePointer)))
            {
                //m_containsCursor = true;

                // there was a change in scroll wheel values since last MouseHandler
                if (GuiManager.CurrentSheet.PreviousScrollWheelValue != ms.ScrollWheelValue)
                {
                    if (this is ScrollableTextBox)
                        OnZDelta(ms);
                }

                // to send message, mouse must have been pressed and released over the hotspot
                if (ms.LeftButton != ButtonState.Pressed && ms.RightButton != ButtonState.Pressed)
                {
                    #region No mouse buttons are pressed. Check for double left click and do OnMouseOver.
                    if (m_mouseLeftDown)
                    {
                        m_leftClickCount++; // start timer here
                        m_doubleClickTimer.Start();
                        m_mouseLeftDown = false;

                        if (!(this is TextBox))
                            HasFocus = false;

                        if (Contains(mousePointer))
                        {
                            OnMouseRelease(ms);
                            if (OnControl != null && !(this is Window) && (!(this is ListBox) ||
                                ((this is ListBox) && (this as ListBox).HasNewData)))
                            {
                                OnControl(m_name, Data);
                            }
                        }
                    }

                    bool result = true;

                    // ListBoxes need to continuously check mouse since it has sub parts that change on mouse over
                    // Mouseover on Windows should be able to reset over state on back controls.
                    if (ControlState == Enums.EControlState.Over && !(this is ListBox) && !(this is Window))
                    {
                        // state is already over
                        result = false;
                    }

                    ControlState = Enums.EControlState.Over;
                    OnMouseOver(ms);
                    return result; 
                    #endregion
                }

                if ((ms.LeftButton == ButtonState.Pressed || ms.RightButton == ButtonState.Pressed) && m_hasTouchDownPoint &&
                    Contains(m_touchDownPoint) && Client.HasFocus || this.HasFocus)
                {
                    ControlState = Enums.EControlState.Down;
                    OnMouseDown(ms);

                    // slider sends data while mouse is down
                    if ((this is Slider) && OnControl != null)
                        OnControl(m_name, Data);
                    HasFocus = true;
                    return true;
                }

                //if(ms.LeftButton != ButtonState.Pressed && m_mouseLeftDown)
                //{
                //    m_mouseLeftDown = false;
                //    ControlState = Enums.eControlState.Normal;
                //    m_hasTouchDownPoint = false;
                //    OnMouseRelease(ms);
                //    return true;
                //}

                //if (ms.RightButton != ButtonState.Pressed && m_mouseRightDown)
                //{
                //    m_mouseRightDown = false;
                //    ControlState = Enums.eControlState.Normal;
                //    m_hasTouchDownPoint = false;
                //    OnMouseRelease(ms);
                //    return true;
                //}
            }
            else if (this is Button && ControlState != Enums.EControlState.Normal)
            //else if ((!Contains(mousePointer) || (this is Button)) && ControlState != Enums.eControlState.Normal)
            {
                if (ms.LeftButton != ButtonState.Pressed && !(this is TextBox))
                    HasFocus = false;

                if (!HasFocus)
                {
                    ControlState = Enums.EControlState.Normal;
                    return true;
                }

                OnMouseLeave(ms);
            }
            return false;
        }

        public virtual bool Contains(Point point)
        {
            if (m_rectangle.Top <= point.Y && m_rectangle.Bottom >= point.Y &&
                m_rectangle.Left <= point.X && m_rectangle.Right >= point.X)
                return true;

            return false;
        }

        protected virtual void OnDoubleLeftClick()
        {
            // empty
        }

        protected virtual void OnMouseDown(MouseState ms)
        {
            // empty
        }

        protected virtual void OnMouseOver(MouseState ms)
        {
            // empty
        }

        protected virtual void OnMouseLeave(MouseState ms)
        {
            if (!(this is TextBox))
                this.HasFocus = false;
        }

        protected virtual void OnMouseRelease(MouseState ms)
        {
            // empty
        }

        protected virtual bool OnKeyDown(KeyboardState ks)
        {
            return false;
        }

        protected virtual void OnZDelta(MouseState ms)
        {
            // empty
        }

        public virtual void OnDestroy()
        {
            if (GuiManager.Sheets[this.Sheet][this.Owner] is Window)
                (GuiManager.Sheets[this.Sheet][this.Owner] as Window).Controls.Remove(this);

            GuiManager.Sheets[this.Sheet].RemoveControl(this);
        }

        public virtual void OnClientResize(Rectangle prev, Rectangle now, bool ownerOverride)
        {
            // if this control is owned by another control then the owner will handle resizing
            if (Owner != "" && !ownerOverride)
                return;

            int x = m_rectangle.X;
            int y = m_rectangle.Y;
            int width = m_rectangle.Width;
            int height = m_rectangle.Height;

            if (m_anchors.Count > 0)
            {
                // top and no bottom
                if (m_anchors.Contains(Enums.EAnchorType.Top) && !m_anchors.Contains(Enums.EAnchorType.Bottom))
                {
                    // y stays the same
                    //if (prev.Y > now.Y)
                    //    y -= prev.Y - now.Y;
                    //else if (prev.X < now.X)
                    //    y += now.Y - prev.Y;
                }

                if (m_anchors.Contains(Enums.EAnchorType.Top) && m_anchors.Contains(Enums.EAnchorType.Bottom))
                {
                    // y stays the same
                    //if (prev.Y > now.Y)
                    //    y -= prev.Y - now.Y;
                    //else if (prev.X < now.X)
                    //    y += now.Y - prev.Y;
                    // height is increased or decreased
                    height += now.Height - prev.Height;
                }

                if (m_anchors.Contains(Enums.EAnchorType.Bottom) && !m_anchors.Contains(Enums.EAnchorType.Top))
                {
                    // distance from new x to new bottom should be same as old x to old bottom
                    int oldBottomDist = prev.Bottom - m_rectangle.Y;
                    y = now.Bottom - oldBottomDist;
                    y += (int)((now.Height - prev.Height) / 2);
                }

                if (m_anchors.Contains(Enums.EAnchorType.Left) && !m_anchors.Contains(Enums.EAnchorType.Right))
                {
                    // x stays the same
                    //if (prev.X > now.X)
                    //    x -= prev.X - now.X;
                    //else if (prev.X < now.X)
                    //    x += now.X - prev.X;
                }

                if (m_anchors.Contains(Enums.EAnchorType.Left) && m_anchors.Contains(Enums.EAnchorType.Right))
                {
                    // x stays the same
                    //if (prev.X > now.X)
                    //    x -= prev.X - now.X;
                    //else if (prev.X < now.X)
                    //    x += now.X - prev.X;
                    // width is increased or decreased
                    width += now.Width - prev.Width;
                }

                if (m_anchors.Contains(Enums.EAnchorType.Right) && !m_anchors.Contains(Enums.EAnchorType.Left))
                {
                    int oldRightDist = prev.Right - m_rectangle.X;
                    x = now.Right - oldRightDist;
                    x += (int)((now.Width - prev.Width) / 2);
                }

                if (m_anchors.Contains(Enums.EAnchorType.Left) && !m_anchors.Contains(Enums.EAnchorType.Right) &&
                    m_anchors.Contains(Enums.EAnchorType.Top) && !m_anchors.Contains(Enums.EAnchorType.Bottom))
                {
                    // This needs some work.
                    //x = now.Left;
                    //y = now.Top;
                }

                if (m_anchors.Contains(Enums.EAnchorType.Center))
                {
                    x += (int)((now.Width - prev.Width) / 2);
                    y += (int)((now.Height - prev.Height) / 2);
                }
            }
            else // no anchors, just adjust x and y position
            {
                x += (int)((now.Width - prev.Width) / 2);
                y += (int)((now.Height - prev.Height) / 2);
            }

            m_rectangle.X = x;
            m_rectangle.Y = y;
            m_rectangle.Width = width;
            m_rectangle.Height = height;
        }

        public int GetXShadow()
        {
            switch (m_shadowDirection)
            {
                case Map.Direction.East:
                    return m_shadowDistance;
                case Map.Direction.North:
                    return 0;
                case Map.Direction.Northeast:
                    return m_shadowDistance;
                case Map.Direction.Northwest:
                    return -(m_shadowDistance);
                case Map.Direction.South:
                    return 0;
                case Map.Direction.Southeast:
                    return m_shadowDistance;
                case Map.Direction.Southwest:
                    return -(m_shadowDistance);
                case Map.Direction.West:
                    return -(m_shadowDistance);
                default:
                    return 0;
            }
        }

        public int GetYShadow()
        {
            switch (m_shadowDirection)
            {
                case Map.Direction.East:
                    return 0;
                case Map.Direction.North:
                    return -(m_shadowDistance);
                case Map.Direction.Northeast:
                    return -(m_shadowDistance);
                case Map.Direction.Northwest:
                    return -(m_shadowDistance);
                case Map.Direction.South:
                    return m_shadowDistance;
                case Map.Direction.Southeast:
                    return m_shadowDistance;
                case Map.Direction.Southwest:
                    return m_shadowDistance;
                case Map.Direction.West:
                    return 0;
                default:
                    return 0;
            }
        }
    }

    /// <summary>Sorts Controls by Z Depth</summary>
    public class ControlSorter : IComparer<Control>
    {
        /// <summary>IComparer implementation</summary>
        /// <param name="x">Control 1</param>
        /// <param name="y">Control 2</param>
        public int Compare(Control x, Control y)
        {
            if (x.ZDepth < y.ZDepth)
            {
                return 1;
            }

            if (x.ZDepth == y.ZDepth)
            {
                // DropDownMenus always go on top.
                if (x is DropDownMenu && !(y is DropDownMenu))
                    return 1;
                else if (y is DropDownMenu && !(x is DropDownMenu))
                    return -1;

                // the currently dragged control gets highest z order
                if((x == GuiManager.DraggedControl) && !(y == GuiManager.DraggedControl))
                {
                    return 1;
                }
                else if (!(x == GuiManager.DraggedControl) && (y == GuiManager.DraggedControl))
                {
                    return -1;
                }

                // window goes underneath its controls
                if ((x is Window) && !(y is Window))
                {
                    return -1;
                }
                else if ((y is Window) && !(x is Window))
                {
                    return 1;
                }

                // border goes underneath all other controls
                if ((x is Border) && !(y is Border))
                {
                    return -1;
                }
                else if ((y is Border) && !(x is Border))
                {
                    return 1;
                }

                // window title goes above all other window controls
                if ((x is WindowTitle) && !(y is WindowTitle))
                {
                    return 1;
                }
                else if ((y is WindowTitle) && !(x is WindowTitle))
                {
                    return -1;
                }

                // for controls on same window, sort bottom to top
                if (x.Position.Y < y.Position.Y)
                {
                    return 1;
                }
                if (x.Position.Y > y.Position.Y)
                {
                    return -1;
                }
                return 0;
            }
            return -1;
        }
    }
}
