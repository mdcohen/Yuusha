using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    abstract public class Control
    {
        public static Color ColorDisabledStandard = Color.DimGray;

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
        protected VisualKey m_visualKey; // visual key
        protected bool m_visualTiled = false; // visual key is tiled
        protected Dictionary<Enums.EControlState, VisualKey> m_visuals;
        protected string m_font;
        protected TimeSpan m_lastUpdate;
        protected object m_data;
        protected int m_leftClickCount;
        protected int m_rightClickCount;
        protected bool m_dropShadow;
        protected int m_shadowDistance;
        protected Map.Direction m_shadowDirection;
        protected List<Enums.EAnchorType> m_anchors;
        protected string m_onDoubleClickEvent;
        protected string m_cursorOverride;
        protected string m_onMouseDown;

        public BitmapFont.TextAlignment TextAlignment
        { get; set; }
        public int XTextOffset
        { get; set; }
        public int YTextOffset
        { get; set; }

        protected Color m_textOverColor;
        protected bool m_hasTextOverColor;
        protected Color m_tintOverColor;
        protected bool m_hasTintOverColor;
        protected List<Enums.EGameState> m_lockoutStates; // for generic windows, states that this window is not available
        //protected string PopUpText;
        protected System.Timers.Timer m_doubleClickTimer;
        protected int m_tabOrder;
        #endregion

        public string Command // all controls may have an associated command
        { get; set; }

        public string ClickSound
        { get; set; }
        private bool m_clickSoundPlayed = false;

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
            set { m_name = value; }
        }

        public virtual string Name
        {
            get { return m_name; }
            set { m_name = value; }
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
            set
            {
                m_zDepth = value;
                ZDepthDateTime = DateTime.Now;
                if (GuiManager.GetControl(Owner) is Window w)
                    w.SortControls();
            }
        }

        public virtual DateTime ZDepthDateTime
        { get; set; }

        public virtual Point Position
        {
            get { return new Point(m_rectangle.X, m_rectangle.Y); }
            set
            {
                // 6/1/2019 was troubleshooting
                //if(value.X == 0 && value.Y == 0)
                //{
                //    if (Name.StartsWith("SackDragAndDrop"))
                //    {
                //        Utils.Log(this.Name + " x and y set to 0");
                //        return;
                //    }
                //}
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

                if (m_hasFocus)
                {
                    GuiManager.ControlWithFocus = this;
                }
                else if (GuiManager.ControlWithFocus == this)
                    GuiManager.ControlWithFocus = null;

                //if(TabOrder > -1 && !string.IsNullOrEmpty(Owner) && GuiManager.GetControl(Owner) is Window w)
                //{
                //    w.CurrentTabOrder = TabOrder;
                //}
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
            set
            {
                if (value > 255) value = 255;
                else if (value < 0) value = 0;

                m_visualAlpha = value;
            }
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

        public virtual int TabOrder
        {
            get { return m_tabOrder; }
            set { m_tabOrder = value; }
        }

        public virtual string PopUpText
        { get; set; }

        public virtual List<Enums.EAnchorType> Anchors
        {
            get { return m_anchors; }
            set { m_anchors = value; }
        }

        // MouseHandler ignores this control if true.
        public virtual bool MouseInvisible
        { get; set; } = false;
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
            m_visualKey = null;
            m_visuals = new Dictionary<Enums.EControlState, VisualKey>();
            XTextOffset = 0;
            YTextOffset = 0;
            m_font = "";
            m_lastUpdate = new TimeSpan();
            m_dropShadow = false;
            m_shadowDistance = 10;
            m_anchors = new List<Enums.EAnchorType>();
            m_onDoubleClickEvent = "";
            m_cursorOverride = "";
            m_onMouseDown = "";
            TextAlignment = BitmapFont.TextAlignment.Left;
            m_textOverColor = Color.White;
            m_hasTextOverColor = false;
            m_lockoutStates = new List<Enums.EGameState>();
            m_shadowDirection = Map.Direction.Southeast;
            PopUpText = "";
            m_doubleClickTimer = new System.Timers.Timer(Client.ClientSettings.DoubleClickTimerDelay);
            m_doubleClickTimer.Elapsed += new System.Timers.ElapsedEventHandler(DoubleClickTimer_Elapsed);
            m_doubleClickTimer.AutoReset = true;
            m_tabOrder = -1;
        }
        #endregion

        private void DoubleClickTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            m_leftClickCount = 0;
            m_rightClickCount = 0;
            m_doubleClickTimer.Stop();
        }

        public virtual void Update(GameTime gameTime)
        {
            if (m_visuals.ContainsKey(ControlState) && m_visualKey != m_visuals[ControlState])
                m_visualKey = m_visuals[ControlState];

            // update colors using alpha settings
            m_textColor = new Color(m_textColor.R, m_textColor.G, m_textColor.B, TextAlpha);
            m_tintColor = new Color(m_tintColor.R, m_tintColor.G, m_tintColor.B, VisualAlpha);

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

            if (m_rightClickCount == 2)
            {
                m_rightClickCount = 0;
                m_doubleClickTimer.Stop();
                OnDoubleRightClick();
            }
            else if (m_leftClickCount > 2)
            {
                m_rightClickCount = 0;
                m_doubleClickTimer.Stop();
            }

            if(!string.IsNullOrEmpty(PopUpText) && !Contains(GuiManager.MouseState.Position))// && ControlState != Enums.EControlState.Over)
                TextCue.RemoveMouseCursorTextCue(PopUpText);

            m_lastUpdate = gameTime.TotalGameTime;
        }

        public virtual void Draw(GameTime gameTime)
        {
            if (!IsVisible)
                return;

            if (m_visualKey != null && !string.IsNullOrEmpty(m_visualKey.Key) && VisualAlpha > 0)
            {
                if (m_visualKey.Key == "  ")
                {
                    m_visualKey.Key = "WhiteSpace";
                    Utils.Log("Corrected VisualKey for " + Name + ". Sheet: " + Sheet + " GameState: " + Client.GameState.ToString() + " DisplayMode: " + Client.GameDisplayMode.ToString());
                }

                if (!GuiManager.Visuals.ContainsKey(m_visualKey.Key))
                {
                    Utils.LogOnce("Failed to find visual key [ " + m_visualKey + " ] for Control [ " + m_name + " ]");
                    m_visualKey.Key = ""; // clear visual key
                    return;
                }

                VisualInfo vi = GuiManager.Visuals[m_visualKey.Key];

                Color color = new Color(m_tintColor.R, m_tintColor.G, m_tintColor.B, VisualAlpha);

                if (m_hasTintOverColor && ControlState == Enums.EControlState.Over)
                    color = new Color(m_tintOverColor.R, m_tintOverColor.G, m_tintOverColor.B, VisualAlpha);

                if (m_disabled)
                    color = new Color(ColorDisabledStandard.R, ColorDisabledStandard.G, ColorDisabledStandard.B, VisualAlpha);

                if (m_dropShadow)
                {
                    Rectangle shadowRect = new Rectangle(m_rectangle.X + GetXShadow(m_shadowDirection, m_shadowDistance), m_rectangle.Y + GetYShadow(m_shadowDirection, m_shadowDistance), m_rectangle.Width, m_rectangle.Height);
                    Color shadowColor = new Color((int)Color.Black.R, (int)Color.Black.G, (int)Color.Black.B, 50);
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], shadowRect, vi.Rectangle, shadowColor);
                }

                if (!m_visualTiled)
                {
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_rectangle, vi.Rectangle, color);
                }
                else // Tiled visual (borders, window titles)
                {
                    // This code needs some work. 2/18/2017. Have to move on.
                    int desiredWidth = Width / vi.Width;
                    int desiredHeight = Height;

                    // What uses tiled visuals?
                    int xAmount = Width / desiredWidth;
                    int yAmount = Height / desiredHeight;

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
            }

            if (!m_disabled)
            {
                if (!string.IsNullOrEmpty(PopUpText) && m_controlState == Enums.EControlState.Over && GuiManager.ActiveDropDownMenu is null)
                    TextCue.AddMouseCursorTextCue(PopUpText, Client.ClientSettings.ColorDefaultPopUpFore, Client.ClientSettings.ColorDefaultPopUpBack, Client.ClientSettings.DefaultPopUpBackAlpha, Client.ClientSettings.DefaultPopUpFont);
            }
        }

        public virtual bool KeyboardHandler(KeyboardState ks)
        {
            if (!Client.HasFocus) return true; // returns that it was handled so as to not do any other handler calls

            foreach(Keys k in ks.GetPressedKeys())
            {
                if (GuiManager.AwaitKeyRelease.Contains(k))
                    return true;
            }

            GuiManager.AwaitKeyRelease.Clear();

            return OnKeyDown(ks);
        }

        public virtual bool MouseHandler(MouseState ms)
        {
            if (!Client.HasFocus) return true;  // returns that it was handled so as to not do any other handler calls

            if (!IsVisible || IsDisabled) return false;

            // drop down menus take priority for mouse handling
            if (GuiManager.ActiveDropDownMenu != null && !(this is DropDownMenu) && !(this is DropDownMenuItem))
                return false;

            if (Contains(ms.Position) && !m_containsMousePointer && !IsBeneathControl(ms) && !GuiManager.IsDragging)
            {
                m_containsMousePointer = true;
                OnMouseOver(ms);
            }

            if (!Contains(ms.Position) && m_containsMousePointer)
            {
                m_containsMousePointer = false;
                OnMouseLeave(ms);
            }

            if (ms.LeftButton == ButtonState.Pressed || ms.RightButton == ButtonState.Pressed)
            {
                m_mouseLeftDown = ms.LeftButton == ButtonState.Pressed;
                m_mouseRightDown = ms.RightButton == ButtonState.Pressed;
            }

            if (!m_hasTouchDownPoint && (ms.LeftButton == ButtonState.Pressed || ms.RightButton == ButtonState.Pressed) && Contains(ms.Position))
            {
                bool allowTouchDown = true;
                if (Sheet != "Generic")
                {
                    foreach (Control c in GuiManager.GenericSheet.Controls)
                    {
                        //(c.ZDepth > ZDepth || (c.ZDepth == ZDepth && c.ZDepthDateTime > ZDepthDateTime))
                        if (c != this && c.IsVisible && c.m_hasTouchDownPoint && c.Contains(ms.Position))
                        {
                            //TextCue.AddClientInfoTextCue(c.Name + " has touch down point...");
                            allowTouchDown = false;
                            break;
                        }
                    }
                }

                if (allowTouchDown)
                {
                    m_touchDownPoint = new Point(ms.X, ms.Y);
                    m_hasTouchDownPoint = true;
                }
            }
            else if (m_hasTouchDownPoint && ms.LeftButton != ButtonState.Pressed && ms.RightButton != ButtonState.Pressed)
            {
                m_hasTouchDownPoint = false;
                GuiManager.AwaitMouseButtonRelease = false;
                OnMouseRelease(ms);
            }

            // Not a TextBox, contains mouse cursor or has focus.
            // OR
            // Textbox and contains mouse cursor.
            if ((!(this is TextBox) && (Contains(ms.Position) || HasFocus)) || ((this is TextBox) && Contains(ms.Position)))
            {
                // there was a change in scroll wheel values since last MouseHandler
                if ((this is ScrollableTextBox || this is FogOfWarWindow || this is SpinelTileLabel) && GuiManager.CurrentSheet.PreviousScrollWheelValue != ms.ScrollWheelValue)
                {
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

                        if (Contains(ms.Position) && m_hasTouchDownPoint)
                        {
                            GuiManager.AwaitMouseButtonRelease = false;
                            OnMouseRelease(ms);

                            if (OnControl != null && !(this is Window) && (!(this is GridBoxWindow) ||
                                ((this is GridBoxWindow) && (this as GridBoxWindow).HasNewData)))
                            {
                                OnControl(m_name, Data);
                            }
                        }
                    }

                    if(m_mouseRightDown)
                    {
                        m_rightClickCount++;
                        m_doubleClickTimer.Start();
                        m_mouseRightDown = false;

                        if (Contains(ms.Position))
                        {
                            GuiManager.AwaitMouseButtonRelease = false;
                            OnMouseRelease(ms);

                            //if (OnControl != null && !(this is Window) && (!(this is GridBoxWindow) ||
                            //    ((this is GridBoxWindow) && (this as GridBoxWindow).HasNewData)))
                            //{
                            //    OnControl(m_name, Data);
                            //}
                        }
                    }

                    bool result = true;

                    // ListBoxes need to continuously check mouse since it has sub parts that change on mouse over
                    // Mouseover on Windows should be able to reset over state on back controls.
                    if (ControlState == Enums.EControlState.Over && !(this is Window))
                    {
                        result = false;
                    }

                    //if (GuiManager.MouseAbove(this))
                    //    return result;

                    if (!IsBeneathControl(ms) && !GuiManager.IsDragging)
                    {
                        ControlState = Enums.EControlState.Over;
                        OnMouseOver(ms);
                    }

                    return result;
                    #endregion
                }

                if ((ms.LeftButton == ButtonState.Pressed || ms.RightButton == ButtonState.Pressed) && m_hasTouchDownPoint &&
                    Contains(m_touchDownPoint) && Client.HasFocus || HasFocus)
                {
                    if ((!GuiManager.IsDragging || GuiManager.DraggedControl == this) && !GuiManager.AwaitMouseButtonRelease)
                    {
                        if (!IsBeneathControl(ms))
                        {
                            ControlState = Enums.EControlState.Down;
                            OnMouseDown(ms);
                        }
                    }

                    // slider sends data while mouse is down
                    if ((this is Slider) && OnControl != null)
                        OnControl(m_name, Data);

                    HasFocus = true;
                    return true;
                }
            }
            //else if (this is Button && ControlState != Enums.EControlState.Normal)
            else if (!GuiManager.IsDragging && (!Contains(ms.Position) || (this is Button)) && ControlState != Enums.EControlState.Normal)
            {
                if (ms.LeftButton != ButtonState.Pressed && !(this is TextBox))
                    HasFocus = false;

                if (!HasFocus || !Contains(ms.Position))
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

        protected virtual void OnDoubleRightClick()
        {
            // empty
        }

        protected virtual void OnMouseDown(MouseState ms)
        {
            if (m_disabled || !m_visible)
                return;

            if (TabOrder > -1)
            {
                if (string.IsNullOrEmpty(Owner))
                    GuiManager.CurrentSheet.CurrentTabOrder = TabOrder;
                else if (GuiManager.GetControl(Owner) is Window)
                    (GuiManager.GetControl(Owner) as Window).CurrentTabOrder = TabOrder;
            }

            if(!m_clickSoundPlayed)
            {
                if (!string.IsNullOrEmpty(ClickSound))
                    Audio.AudioManager.PlaySoundEffect(ClickSound);
                else if(((this is Button) || GetType().IsSubclassOf(typeof(Button))) && !string.IsNullOrEmpty(m_onMouseDown))
                    Audio.AudioManager.PlaySoundEffect(Client.ClientSettings.DefaultOnClickSound);
                else if(this is DropDownMenuItem)
                    Audio.AudioManager.PlaySoundEffect(Client.ClientSettings.DefaultOnClickSound);

                m_clickSoundPlayed = true;
            }

            //ZDepth = 0;
            //GuiManager.CurrentSheet.SortControls();
        }

        protected virtual void OnMouseOver(MouseState ms)
        {
            // empty
        }

        protected virtual void OnMouseLeave(MouseState ms)
        {
            if (!(this is TextBox))
                HasFocus = false;

            TextCue.RemoveMouseCursorTextCue(PopUpText);
        }

        protected virtual void OnMouseRelease(MouseState ms)
        {
            m_clickSoundPlayed = false;
        }

        protected virtual bool OnKeyDown(KeyboardState ks)
        {
            return false;
        }

        protected virtual bool OnKeyRelease(KeyboardState ks)
        {
            return false;
        }

        protected virtual void OnZDelta(MouseState ms)
        {
            // empty
        }

        public virtual void OnDispose()
        {
            GuiManager.RemoveControl(this);
        }

        public virtual void OnClientResize(Rectangle prev, Rectangle now, bool ownerOverride)
        {
            // if this control is owned by another control then the owner will handle resizing
            if (!string.IsNullOrEmpty(Owner) && !ownerOverride)
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
                    //else if (prev.Y < now.Y)
                    //    y += now.Y - prev.Y;
                }

                // top and bottom
                if (m_anchors.Contains(Enums.EAnchorType.Top) && m_anchors.Contains(Enums.EAnchorType.Bottom))
                {
                    // y stays the same
                    //if (prev.Y > now.Y)
                    //    y -= prev.Y - now.Y;
                    //else if (prev.Y < now.Y)
                    //    y += now.Y - prev.Y;
                    // height is increased or decreased
                    height += now.Height - prev.Height;
                }

                // bottom and no top
                if (m_anchors.Contains(Enums.EAnchorType.Bottom) && !m_anchors.Contains(Enums.EAnchorType.Top))
                {
                    // distance from new x to new bottom should be same as old x to old bottom
                    int oldBottomDist = prev.Bottom - m_rectangle.Y;
                    y = now.Bottom - oldBottomDist;
                    int adjustment = 0;
                    //if (Client.IsFullScreen)
                    //    adjustment = m_rectangle.Height / 2;
                    y += ((now.Height - prev.Height) / 2) - adjustment;
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
                    x += (now.Width - prev.Width) / 2;
                }

                if (m_anchors.Contains(Enums.EAnchorType.Left) && !m_anchors.Contains(Enums.EAnchorType.Right) &&
                    m_anchors.Contains(Enums.EAnchorType.Top) && !m_anchors.Contains(Enums.EAnchorType.Bottom))
                {
                    // This needs some work.
                    //x = now.Left;
                    //y = now.Top;
                }

                if (m_anchors.Contains(Enums.EAnchorType.Center) && m_anchors.Count == 1)
                {
                    //x = now.Width / 2 - Width / 2;
                    //y = now.Height / 2 - Height / 2;

                    x += (now.Width - prev.Width) / 2;
                    y += (now.Height - prev.Height) / 2;
                }
            }
            else // no anchors, just adjust x and y position
            {
                x += m_rectangle.X - prev.X;
                y += m_rectangle.Y - prev.Y;

                if (this is Window)
                {
                    foreach (Control c in (this as Window).Controls)
                    {
                        c.OnClientResize(prev, now, false);
                        if (c is Window)
                        {
                            foreach (Control c2 in (c as Window).Controls)
                                c2.OnClientResize(prev, now, false);
                        }
                    }
                }
            }

            m_rectangle.X = x;
            m_rectangle.Y = y;
            m_rectangle.Width = width;
            m_rectangle.Height = height;
        }

        public virtual bool IsOffscreen()
        {
            if (Position.X > Client.Width)
                return true;

            if (Position.Y > Client.Height)
                return true;

            if (Position.X + Width < 0)
                return true;

            if (Position.Y + Height < 0)
                return true;

            return false;
        }

        protected bool IsBeneathControl(MouseState ms)
        {
            if (GuiManager.Cursors[GuiManager.GenericSheet.Cursor].DraggedControl is DragAndDropButton && !(this is GridBoxWindow) && !(this is DragAndDropButton))
                return true;

            // Drop Down Menu always on top.
            if (this is DropDownMenu || this is DropDownMenuItem)
                return false;
            else if (GuiManager.ActiveDropDownMenu != null)
            {
                if(GuiManager.ActiveDropDownMenu.IsVisible)
                    return true;
                else
                {
                    GuiManager.ActiveDropDownMenu.OnDispose();
                    GuiManager.ActiveDropDownMenu = null;
                }
            }

            // Cheating? Yes. Possibly needs rework in the future. 8/28/2019
            if (Sheet == "Generic") return false;

            if (!string.IsNullOrEmpty(Owner))
            {
                if (GuiManager.GetControl(Owner) is Control c && c.Sheet == "Generic") return false;
            }

            if (Owner == "SpellringWindow") return false;

            if (this is WindowTitle || this is WindowControlBox)
                return false;

            foreach (Control c in GuiManager.GenericSheet.Controls)
            {
                if (c.Sheet != "Generic" && c != this && c.IsVisible && c.Contains(ms.Position) && c.Name != Owner && !c.MouseInvisible)
                {
                    return true;
                }
            }

            if (Sheet != "Generic")
            {
                foreach(Control c in GuiManager.GenericSheet.Controls)
                {
                    if (c.IsVisible && c.Contains(ms.Position))
                        return true;
                }

                foreach (Control c in GuiManager.CurrentSheet.Controls)
                {
                    if (Owner == c.Name || !c.IsVisible || c == this) continue;

                    if ((c.ZDepth < ZDepth || (c.ZDepth == ZDepth && c.ZDepthDateTime > ZDepthDateTime)) && c.Contains(ms.Position) && !c.MouseInvisible)
                        return true;
                }
            }

            return false;
        }

        public static int GetXShadow(Map.Direction direction, int distance)
        {
            switch (direction)
            {
                case Map.Direction.East:
                    return distance;
                case Map.Direction.North:
                    return 0;
                case Map.Direction.Northeast:
                    return distance;
                case Map.Direction.Northwest:
                    return -distance;
                case Map.Direction.South:
                    return 0;
                case Map.Direction.Southeast:
                    return distance;
                case Map.Direction.Southwest:
                    return -distance;
                case Map.Direction.West:
                    return -distance;
                default:
                    return 0;
            }
        }

        public static int GetYShadow(Map.Direction direction, int distance)
        {
            switch (direction)
            {
                case Map.Direction.East:
                    return 0;
                case Map.Direction.North:
                    return -distance;
                case Map.Direction.Northeast:
                    return -distance;
                case Map.Direction.Northwest:
                    return -distance;
                case Map.Direction.South:
                    return distance;
                case Map.Direction.Southeast:
                    return distance;
                case Map.Direction.Southwest:
                    return distance;
                case Map.Direction.West:
                    return 0;
                default:
                    return 0;
            }
        }

        public static bool operator >(Control c1, Control c2)
        {
            if (c2.ZDepth > c1.ZDepth) return true;

            if (c1.ZDepth < c2.ZDepth) return true;

            if (c1.ZDepth == c2.ZDepth && c1.ZDepthDateTime > c2.ZDepthDateTime)
                return true;
            
            return false;
        }

        public static bool operator <(Control c1, Control c2)
        {
            if (c2.ZDepth < c1.ZDepth) return true;

            if (c1.ZDepth > c2.ZDepth) return true;

            if (c1.ZDepth == c2.ZDepth && c1.ZDepthDateTime < c2.ZDepthDateTime)
                return true;

            return false;
        }
    }
}