using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class Window : Control
    {
        #region Private Data
        protected int m_xOffset = 0;
        protected int m_yOffset = 0;
        protected bool m_dragging = false;
        protected string m_activeTextBox = "";
        protected string m_openComboBox = "";
        protected List<Control> m_controls;
        protected bool m_cropped;
        protected bool m_maximized;
        protected bool m_minimized;
        protected Rectangle m_minimizeAtRect;
        protected Rectangle m_maximizeAtRect;
        protected WindowTitle m_windowTitle;
        protected Border m_windowBorder;
        protected int m_currentTabOrder;
        #endregion

        #region Public Properties
        public List<Control> Controls
        {
            get { return m_controls; }
        }
        public int XOffset
        {
            get { return m_xOffset; }
            set { m_xOffset = value; }
        }
        public int YOffset
        {
            get { return m_yOffset; }
            set { m_yOffset = value; }
        }
        public string ActiveTextBox
        {
            get { return m_activeTextBox; }
            set { m_activeTextBox = value; }
        }
        public string OpenComboBox
        {
            get { return m_openComboBox; }
            set { m_openComboBox = value; }
        }
        public WindowTitle WindowTitle
        {
            get { return m_windowTitle; }
            set { m_windowTitle = value; }
        }
        public string CursorOverride
        {
            get { return m_cursorOverride; }
        }
        public Border WindowBorder
        {
            get { return m_windowBorder; }
            set { m_windowBorder = value; }
        }
        public bool IsMinimized
        {
            get { return m_minimized; }
        }

        public bool IsMaximized
        {
            get { return m_maximized; }
        }

        public int CurrentTabOrder
        {
            get { return m_currentTabOrder; }
            set { m_currentTabOrder = value; }
        }

        public TabControl TabControl
        { get; set; }

        /// <summary>
        /// Window can be dragged from any location in its rectangle. Set to false only within the code.
        /// </summary>
        //public bool DiscreetlyDraggable
        //{ get; set; } = true;

        public bool DiscreetlyDraggable
        { get { return !GameHUD.NonDiscreetlyDraggableWindows.Contains(Name); } }
        #endregion

        #region Constructor
        public Window(string name, string owner, Rectangle rectangle, bool visible, bool locked, bool disabled,
            string font, VisualKey visualKey, Color tintColor, byte visualAlpha, bool dropShadow,
            Map.Direction shadowDirection, int shadowDistance, List<Enums.EAnchorType> anchors, string cursorOverride)
            : base()
        {
            m_name = name;
            m_owner = owner;
            m_rectangle = rectangle;
            m_visible = visible;
            m_locked = locked;
            m_disabled = disabled;
            m_font = font;
            m_visualKey = visualKey;
            m_visuals.Add(Enums.EControlState.Normal, m_visualKey);
            m_tintColor = tintColor;
            m_visualAlpha = visualAlpha;
            m_dropShadow = dropShadow;
            m_shadowDirection = shadowDirection;
            m_shadowDistance = shadowDistance;
            m_anchors = anchors;
            m_cursorOverride = cursorOverride;

            m_controls = new List<Control>();
            m_cropped = false;
            m_maximized = false;
            m_minimized = false;
            m_minimizeAtRect = new Rectangle();
            m_maximizeAtRect = new Rectangle();

            m_currentTabOrder = -1;
        }
        #endregion

        public Control this[string name]
        {
            get
            {
                foreach(Control c in new List<Control>(Controls))
                {
                    if (name == c.Name)
                        return c;

                    if(c is Window)
                    {
                        foreach(Control c2 in (c as Window).Controls)
                        {
                            if (name == c2.Name)
                                return c2;

                            if(c2 is Window)
                            {
                                foreach(Control c3 in (c2 as Window).Controls)
                                {
                                    if (name == c3.Name)
                                        return c3;

                                    if(c3 is Window)
                                    {
                                        foreach(Control c4 in (c3 as Window).Controls)
                                        {
                                            if (name == c4.Name)
                                                return c4;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //for (int index = 0; index < m_controls.Count; index++)
                //{
                //    if (name == m_controls[index].Name)
                //    {
                //        return m_controls[index];
                //    }
                //}
                Utils.LogOnce("GUI Sheet [ " + GuiManager.CurrentSheet.Name + " ] Window [ " + m_name + " ] returned null for Control [ " + name + " ]");
                return null;
            }
        }

        public override void Update(GameTime gameTime)
        {
            MouseState ms = GuiManager.MouseState;

            // Handle dragging.
            if (GuiManager.IsDragging && GuiManager.DraggedControl == this)
            {
                m_xOffset = ms.X - m_touchDownPoint.X;
                m_yOffset = ms.Y - m_touchDownPoint.Y;
                m_rectangle.X += m_xOffset;
                m_rectangle.Y += m_yOffset;
                m_touchDownPoint.X += m_xOffset;
                m_touchDownPoint.Y += m_yOffset;

                // window is being dragged, move it's controls with it
                foreach (Control control in new List<Control>(Controls))
                {
                    Point position = control.Position;
                    position.X += XOffset;
                    position.Y += YOffset;
                    control.Position = position;

                    // make a call here

                    if (control is Window)
                    {
                        foreach (Control winControl in new List<Control>((control as Window).Controls))
                        {
                            position = winControl.Position;
                            position.X += XOffset;
                            position.Y += YOffset;
                            winControl.Position = position;
                        }
                    }

                    if (control is WindowTitle)
                    {
                        WindowTitle wt = control as WindowTitle;
                        if (wt.CloseBox != null)
                        {
                            position = wt.CloseBox.Position;
                            position.X += m_xOffset;
                            position.Y += m_yOffset;
                            wt.CloseBox.Position = position;
                        }
                        if (wt.MaximizeBox != null)
                        {
                            position = wt.MaximizeBox.Position;
                            position.X += m_xOffset;
                            position.Y += m_yOffset;
                            wt.MaximizeBox.Position = position;
                        }
                        if (wt.MinimizeBox != null)
                        {
                            position = wt.MinimizeBox.Position;
                            position.X += m_xOffset;
                            position.Y += m_yOffset;
                            wt.MinimizeBox.Position = position;
                        }
                        if (wt.CropBox != null)
                        {
                            position = wt.CropBox.Position;
                            position.X += m_xOffset;
                            position.Y += m_yOffset;
                            wt.CropBox.Position = position;
                        }
                    }
                }
                CheckBoundsAndAdjust();
            }

            base.Update(gameTime);

            foreach (Control control in new List<Control>(m_controls))
            {
                if (!m_cropped || (control is WindowControlBox) || (control is WindowTitle))
                    control.IsDisabled = m_disabled; // disabled

                control.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (!m_visible)
                return;

            if (!m_minimized && !m_cropped)
            {
                base.Draw(gameTime);

                if (WindowBorder != null)
                    WindowBorder.Draw(gameTime);

                foreach (Control control in new List<Control>(m_controls))
                {
                    if(control != WindowBorder && control != WindowTitle)
                        control.Draw(gameTime);
                }

                if (WindowTitle != null)
                    WindowTitle.Draw(gameTime);
            }
            else
            {
                if (m_dropShadow && m_visualKey != null && GuiManager.Visuals.ContainsKey(m_visualKey.Key))
                {
                    VisualInfo vi = GuiManager.Visuals[m_visualKey.Key];
                    Rectangle sourceRect = new Rectangle(vi.X, vi.Y, vi.Width, vi.Height);

                    Rectangle shadowRect = new Rectangle(WindowTitle.Position.X + GetXShadow(m_shadowDirection, m_shadowDistance), WindowTitle.Position.Y + GetYShadow(m_shadowDirection, m_shadowDistance),
                        WindowTitle.Width, WindowTitle.Height);
                    Color shadowColor = new Color((int)Color.Black.R, (int)Color.Black.G, (int)Color.Black.B, 50);
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], shadowRect, sourceRect, shadowColor);

                }

                if(WindowTitle != null)
                    WindowTitle.Draw(gameTime);

                if (!m_minimized && !m_cropped)
                    WindowBorder.Draw(gameTime);
            }
        }

        public override bool KeyboardHandler(KeyboardState ks)
        {
            if (m_cropped || IsMinimized || !IsVisible || IsDisabled)
                return false;

            // front to back
            for (int i = m_controls.Count - 1; i >= 0; i--)
            {
                // skip this control if disabled
                if (m_controls[i].IsDisabled)
                {
                    continue;
                }

                if (m_controls[i].KeyboardHandler(ks))
                    return true;
            }

            return base.KeyboardHandler(ks);
        }

        public override bool MouseHandler(MouseState ms)
        {
            if (!IsVisible || IsDisabled)
                return false;

            if (m_cropped || IsMinimized)
            {
                foreach (Control c in Controls)
                {
                    if (c is WindowTitle || c is WindowControlBox)
                        return c.MouseHandler(ms);
                }
            }

            for (int i = m_controls.Count - 1; i >= 0; i--)
            {
                // skip this control if disabled
                try
                {
                    if (m_controls[i].IsDisabled || !m_controls[i].IsVisible)
                        continue;
                }
                catch(ArgumentOutOfRangeException)
                {
                    break;
                }

                // stop dragging if the left button is no longer pressed
                if (GuiManager.DraggedControl == this && ms.LeftButton != ButtonState.Pressed)
                    m_dragging = false;

                int numControls = m_controls.Count;

                // not dragging or control is a window and mouse was handled by control
                if ((!m_dragging || (m_controls[i] is Window)) && m_controls[i].MouseHandler(ms))
                {
                    if (numControls != m_controls.Count)
                    {
                        // if we're here, we used a control to delete another control, so 
                        // the index will be missing. Return to prevent IndexOutOfBounds
                        return false;
                    }

                    //if ((m_controls[i].ControlState == Enums.EControlState.Down) &&
                    //    !m_controls[i].IsLocked)
                    //{
                    //    m_dragging = true;
                    //}

                    #region ComboBox
                    if ((m_controls[i] is ComboBox) && (m_controls[i] as ComboBox).IsOpen)
                    {
                        if ((m_openComboBox != "") && m_openComboBox != m_controls[i].Name)
                        {
                            // close any open ComboBox
                            for (int j = 0; j < m_controls.Count; j++)
                            {
                                if (m_controls[j].Name == m_openComboBox)
                                {
                                    (m_controls[j] as ComboBox).IsOpen = false;
                                    break;
                                }
                            }
                        }
                        m_openComboBox = m_controls[i].Name;
                    }
                    #endregion

                    if ((m_controls[i] is TextBox) && m_controls[i].HasFocus)
                    {
                        if (m_activeTextBox != "" && m_activeTextBox != m_controls[i].Name)
                            if (this[m_activeTextBox] != null)
                                this[m_activeTextBox].HasFocus = false;

                        m_activeTextBox = m_controls[i].Name;
                    }

                    // for controls in a window, mouse may have just been released from another 
                    // control's focus, so we need to make sure we reset that control's state
                    if ((m_controls[i].ControlState == Enums.EControlState.Over) && (m_controls[i].Owner != "")
                        && !(m_controls[i] is Window))
                    {
                        for (int j = m_controls.Count - 1; j >= 0 && m_controls[j].ZDepth == 1; j--)
                        {
                            if (m_controls[j].ControlState == Enums.EControlState.Down)
                            {
                                m_controls[j].ControlState = Enums.EControlState.Normal;
                                break;
                            }
                        }
                    }

                    // if new radiobutton was selected, deselect radiobuttons of same group ID
                    if ((m_controls[i] is RadioButton) && ((m_controls[i] as RadioButton).NeedToDeselectOthers))
                    {
                        (m_controls[i] as RadioButton).NeedToDeselectOthers = false;
                        for (int j = 0; j < m_controls.Count; j++)
                        {
                            if (i == j)
                            {
                                continue;
                            }
                            if ((m_controls[j] is RadioButton) &&
                                ((m_controls[j] as RadioButton).GroupID == (m_controls[i] as RadioButton).GroupID))
                            {
                                (m_controls[j] as RadioButton).Deselect();
                            }
                        }
                    }

                    // may have moved from a back control to a front control so reset over states
                    for (int j = 0; j < i; j++)
                    {
                        if (m_controls[j].ControlState == Enums.EControlState.Over)
                        {
                            m_controls[j].ControlState = Enums.EControlState.Normal;
                            break;
                        }
                    }

                    if (m_controls[i].ControlState == Enums.EControlState.Down)
                    {
                        // mouse is down over another control so close any open combobox
                        if ((m_openComboBox != "") && (m_openComboBox != m_controls[i].Name))
                        {
                            // close the open combobox
                            for (int j = 0; j < m_controls.Count; j++)
                            {
                                if (m_controls[j].Name == m_openComboBox)
                                {
                                    (m_controls[j] as ComboBox).IsOpen = false;
                                    m_openComboBox = "";
                                    break;
                                }
                            }
                        }

                        if ((m_activeTextBox != "") && (m_activeTextBox != m_controls[i].Name))
                        {
                            // release textbox focus
                            if (this[m_activeTextBox] != null)
                                this[m_activeTextBox].HasFocus = false;
                            m_activeTextBox = "";
                        }

                        Control c = m_controls[i];

                        #region Adjust Z Depth
                        if (m_controls[i].ZDepth != 1)
                        {
                            if (m_controls[i] is Window && ((m_controls[i] as Window).Controls.Count > 0))
                            {
                                if(!m_controls[i].IsLocked) // REMEMBER THIS
                                    m_controls[i].ZDepth = 1;
                            }
                            else
                            {
                                // control is either independent or a window
                                for (int j = 0; j < m_controls.Count; j++)
                                {
                                    if (m_controls[j].ZDepth < m_controls[i].ZDepth)
                                    {
                                        m_controls[j].ZDepth++;
                                    }
                                }
                                m_controls[i].ZDepth = 1;
                            }

                            // sort controls
                            SortControls();
                        }
                        #endregion
                    }
                    break;
                }
                else if (ms.LeftButton == ButtonState.Pressed)
                {
                    // clicked off a control so close any open ComboBox
                    if (m_openComboBox != "")
                    {
                        ComboBox openComboBox;
                        if (this[m_openComboBox] != null)
                        {
                            openComboBox = this[m_openComboBox] as ComboBox;
                            Point mousePointer = new Point(ms.X, ms.Y);
                            // close the open combobox
                            if (!openComboBox.Contains(mousePointer))
                            {
                                openComboBox.IsOpen = false;
                                m_openComboBox = "";
                            }
                        }
                    }

                    if ((m_activeTextBox != "") && (m_activeTextBox != m_controls[i].Name))
                    {
                        // release EditBox focus
                        if (this[m_activeTextBox] != null)
                            this[m_activeTextBox].HasFocus = false;
                        m_activeTextBox = "";
                    }
                }
            }

            return base.MouseHandler(ms);
        }

        protected override void OnMouseDown(MouseState ms)
        {
            // return if window is locked or disabled
            if (m_locked || m_disabled || !Client.HasFocus)
                return;

            if (((WindowTitle != null && WindowTitle.ControlState == Enums.EControlState.Down) || this is FogOfWarWindow) && GuiManager.DraggedControl != this)
            {
                bool startDragging = true;

                foreach (Control box in Controls)
                    if(box is WindowControlBox && box.Contains(new Point(ms.X, ms.Y))) startDragging = false;

                if (startDragging)
                    GuiManager.StartDragging(this, ms);
            }

            if (!DiscreetlyDraggable && ms.LeftButton == ButtonState.Pressed && GuiManager.DraggedControl != this)
                GuiManager.StartDragging(this, ms);

            // Stop dragging if minimized or maximized.
            if (m_minimized || m_maximized)
                GuiManager.StopDragging();
        }

        protected override void OnMouseOver(MouseState ms)
        {
            base.OnMouseOver(ms);

            // show drag cursor if not locked, the window title has the hotspot, the window border has the hotspot
            if (m_cursorOverride != "" && !m_locked && (!DiscreetlyDraggable || WindowTitle != null && WindowTitle.Contains(new Point(ms.X, ms.Y))))
            {
                foreach (Control box in Controls)
                {
                    if (box is WindowControlBox && box.Contains(new Point(ms.X, ms.Y)))
                        return;
                }

                GuiManager.CurrentSheet.CursorOverride = m_cursorOverride;
            }
        }

        public virtual void OnCrop()
        {
            if (!m_minimized)
            {
                m_cropped = !m_cropped;
                CheckBoundsAndAdjust();

                // disable all controls in the window except the control boxes and title
                foreach (Control ctrl in this.Controls)
                {
                    if (!(ctrl is WindowControlBox) && !(ctrl is WindowTitle) && (ctrl != WindowBorder))
                        ctrl.IsDisabled = m_cropped;
                }
            }
        }

        public virtual void OnMinimize()
        {
            if (!m_minimized)
            {
                m_minimizeAtRect = new Rectangle(m_rectangle.X, m_rectangle.Y, m_rectangle.Width, m_rectangle.Height);
                if (m_maximized)
                {
                    m_rectangle = new Rectangle(m_maximizeAtRect.X, m_maximizeAtRect.Y, m_maximizeAtRect.Width, m_maximizeAtRect.Height);
                }
                m_maximized = false;
                m_minimized = true;
                m_cropped = true;
                Position = new Point(0, Client.Height - WindowTitle.Height);
                CheckBoundsAndAdjust();
            }
            else OnMaximize();
        }

        public virtual void OnMaximize()
        {
            m_cropped = false;

            if (m_minimized)
            {
                m_rectangle = new Rectangle(m_minimizeAtRect.X, m_minimizeAtRect.Y, m_minimizeAtRect.Width, m_minimizeAtRect.Height);
                m_maximizeAtRect = new Rectangle(m_minimizeAtRect.X, m_minimizeAtRect.Y, m_minimizeAtRect.Width, m_minimizeAtRect.Height);
                m_minimized = false;
                m_maximized = false;
            }
            else if (m_maximized)
            {
                m_rectangle = new Rectangle(m_maximizeAtRect.X, m_maximizeAtRect.Y, m_maximizeAtRect.Width, m_maximizeAtRect.Height);
                m_maximized = false;
                m_minimized = false;
            }
            else
            {
                m_maximizeAtRect = new Rectangle(m_rectangle.X, m_rectangle.Y, m_rectangle.Width, m_rectangle.Height);
                m_rectangle = new Rectangle(0, 0, Client.Width, Client.Height);
                m_maximized = true;
                m_minimized = false;
            }
        }

        public virtual void OnClose()
        {
            IsVisible = false;
            HasFocus = false;

            foreach (Control c in Controls)
                c.HasFocus = false;

            if (Name == "OptionsWindow" || Name.EndsWith("PrivateMessageWindow") || Name.EndsWith("HotButtonEditWindow"))
            {
                if (Client.InGame)
                    GuiManager.CurrentSheet[Globals.GAMEINPUTTEXTBOX].HasFocus = true;
                else if (Client.GameState == Enums.EGameState.Conference)
                    GuiManager.CurrentSheet[Globals.CONFINPUTTEXTBOX].HasFocus = true;
            }
        }

        /// <summary>
        /// Be aware when adding a window within a window within a window within a window. ;)
        /// </summary>
        public void CheckBoundsAndAdjust()
        {
            if (this is FogOfWarWindow) return;

            if (m_owner == "")
            {
                try
                {
                    // X
                    if (m_rectangle.X < 0)
                    {
                        int adjustX = Math.Abs(m_rectangle.X);
                        m_rectangle.X += adjustX;
                        m_touchDownPoint.X += adjustX;
                        for (int j = Controls.Count - 1; j >= 0; j--)
                        {
                            Point position = Controls[j].Position;
                            position.X += adjustX;
                            Controls[j].Position = position;

                            if (Controls[j] is Window)
                            {
                                for (int k = (Controls[j] as Window).Controls.Count - 1; k >= 0; k--)
                                {
                                    position = ((this as Window).Controls[j] as Window).Controls[k].Position;
                                    position.X += adjustX;
                                    ((this as Window).Controls[j] as Window).Controls[k].Position = position;

                                    if (((this as Window).Controls[j] as Window).Controls[k] is Window)
                                    {
                                        for (int l = (((this as Window).Controls[j] as Window).Controls[k] as Window).Controls.Count - 1; l >= 0; l--)
                                        {
                                            position = (((this as Window).Controls[j] as Window).Controls[k] as Window).Controls[l].Position;
                                            position.X += adjustX;
                                            (((this as Window).Controls[j] as Window).Controls[k] as Window).Controls[l].Position = position;

                                            if ((((this as Window).Controls[j] as Window).Controls[k] as Window).Controls[l] is Window)
                                            {
                                                for (int m = ((((this as Window).Controls[j] as Window).Controls[k] as Window).Controls[l] as Window).Controls.Count - 1; m >= 0; m--)
                                                {
                                                    position = ((((this as Window).Controls[j] as Window).Controls[k] as Window).Controls[l] as Window).Controls[m].Position;
                                                    position.X += adjustX;
                                                    ((((this as Window).Controls[j] as Window).Controls[k] as Window).Controls[l] as Window).Controls[m].Position = position;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Y
                    if (m_rectangle.Y < 0)
                    {
                        int adjustY = Math.Abs(m_rectangle.Y);
                        m_rectangle.Y += adjustY;
                        m_touchDownPoint.Y += adjustY;
                        for (int j = Controls.Count - 1; j >= 0; j--)
                        {
                            Point position = Controls[j].Position;
                            position.Y += adjustY;
                            Controls[j].Position = position;

                            if (Controls[j] is Window)
                            {
                                for (int k = (Controls[j] as Window).Controls.Count - 1; k >= 0; k--)
                                {
                                    position = ((this as Window).Controls[j] as Window).Controls[k].Position;
                                    position.Y += adjustY;
                                    ((this as Window).Controls[j] as Window).Controls[k].Position = position;

                                    if (((this as Window).Controls[j] as Window).Controls[k] is Window)
                                    {
                                        for (int l = (((this as Window).Controls[j] as Window).Controls[k] as Window).Controls.Count - 1; l >= 0; l--)
                                        {
                                            position = (((this as Window).Controls[j] as Window).Controls[k] as Window).Controls[l].Position;
                                            position.Y += adjustY;
                                            (((this as Window).Controls[j] as Window).Controls[k] as Window).Controls[l].Position = position;

                                            if ((((this as Window).Controls[j] as Window).Controls[k] as Window).Controls[l] is Window)
                                            {
                                                for (int m = ((((this as Window).Controls[j] as Window).Controls[k] as Window).Controls[l] as Window).Controls.Count - 1; m >= 0; m--)
                                                {
                                                    position = ((((this as Window).Controls[j] as Window).Controls[k] as Window).Controls[l] as Window).Controls[m].Position;
                                                    position.Y += adjustY;
                                                    ((((this as Window).Controls[j] as Window).Controls[k] as Window).Controls[l] as Window).Controls[m].Position = position;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Width -- MapWindow allowed to exceed bounds
                    if (m_rectangle.X + m_rectangle.Width > Client.Width && !(this is FogOfWarWindow))
                    {
                        int adjustX = Math.Abs((m_rectangle.X + m_rectangle.Width) - Client.Width);
                        m_rectangle.X -= adjustX;
                        m_touchDownPoint.X -= adjustX;
                        for (int j = Controls.Count - 1; j >= 0; j--)
                        {
                            Point position = Controls[j].Position;
                            position.X -= adjustX;
                            Controls[j].Position = position;

                            if (Controls[j] is Window)
                            {
                                for (int k = (Controls[j] as Window).Controls.Count - 1; k >= 0; k--)
                                {
                                    position = ((this as Window).Controls[j] as Window).Controls[k].Position;
                                    position.X -= adjustX;
                                    ((this as Window).Controls[j] as Window).Controls[k].Position = position;

                                    if (((this as Window).Controls[j] as Window).Controls[k] is Window)
                                    {
                                        for (int l = (((this as Window).Controls[j] as Window).Controls[k] as Window).Controls.Count - 1; l >= 0; l--)
                                        {
                                            position = (((this as Window).Controls[j] as Window).Controls[k] as Window).Controls[l].Position;
                                            position.X -= adjustX;
                                            (((this as Window).Controls[j] as Window).Controls[k] as Window).Controls[l].Position = position;

                                            if ((((this as Window).Controls[j] as Window).Controls[k] as Window).Controls[l] is Window)
                                            {
                                                for (int m = ((((this as Window).Controls[j] as Window).Controls[k] as Window).Controls[l] as Window).Controls.Count - 1; m >= 0; m--)
                                                {
                                                    position = ((((this as Window).Controls[j] as Window).Controls[k] as Window).Controls[l] as Window).Controls[m].Position;
                                                    position.X -= adjustX;
                                                    ((((this as Window).Controls[j] as Window).Controls[k] as Window).Controls[l] as Window).Controls[m].Position = position;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Height -- MapWindow allowed to exceed bounds
                    int heightCheck = m_rectangle.Y + m_rectangle.Height;

                    if (m_cropped && WindowTitle != null)
                        heightCheck = m_rectangle.Y + WindowTitle.Height;

                    if (heightCheck > Client.Height && !(this is FogOfWarWindow))
                    {
                        int adjustY = Math.Abs((heightCheck) - Client.Height);
                        m_rectangle.Y -= adjustY;
                        m_touchDownPoint.Y -= adjustY;

                        for (int j = Controls.Count - 1; j >= 0; j--)
                        {
                            Point position = Controls[j].Position;
                            position.Y -= adjustY;
                            this.Controls[j].Position = position;

                            if (Controls[j] is Window)
                            {
                                for (int k = (Controls[j] as Window).Controls.Count - 1; k >= 0; k--)
                                {
                                    position = ((this as Window).Controls[j] as Window).Controls[k].Position;
                                    position.Y -= adjustY;
                                    ((this as Window).Controls[j] as Window).Controls[k].Position = position;

                                    if (((this as Window).Controls[j] as Window).Controls[k] is Window)
                                    {
                                        for (int l = (((this as Window).Controls[j] as Window).Controls[k] as Window).Controls.Count - 1; l >= 0; l--)
                                        {
                                            position = (((this as Window).Controls[j] as Window).Controls[k] as Window).Controls[l].Position;
                                            position.Y -= adjustY;
                                            (((this as Window).Controls[j] as Window).Controls[k] as Window).Controls[l].Position = position;

                                            if ((((this as Window).Controls[j] as Window).Controls[k] as Window).Controls[l] is Window)
                                            {
                                                for (int m = ((((this as Window).Controls[j] as Window).Controls[k] as Window).Controls[l] as Window).Controls.Count - 1; m >= 0; m--)
                                                {
                                                    position = ((((this as Window).Controls[j] as Window).Controls[k] as Window).Controls[l] as Window).Controls[m].Position;
                                                    position.Y -= adjustY;
                                                    ((((this as Window).Controls[j] as Window).Controls[k] as Window).Controls[l] as Window).Controls[m].Position = position;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Utils.LogException(e);
                }
            }
            else
            {
                Sheet sheet = GuiManager.Sheets[Client.GameState.ToString()];

                try
                {
                    if (!(sheet[m_owner] is Window owner))
                    {
                        owner = sheet[(sheet[m_owner] as Window).Owner] as Window;
                    }

                    if (owner == null)
                    {
                        sheet = GuiManager.GenericSheet;
                        owner = sheet[m_owner] as Window;
                        if (owner == null)
                            owner = sheet[(sheet[m_owner] as Window).Owner] as Window;
                    }

                    if (owner == null)
                        return;

                    if (m_rectangle.X < owner.Position.X)
                    {
                        int adjustX = owner.Position.X - m_rectangle.X;
                        m_rectangle.X += adjustX;
                        m_touchDownPoint.X += adjustX;
                        for (int j = Controls.Count - 1; j >= 0; j--)
                        {
                            Point position = Controls[j].Position;
                            position.X += adjustX;
                            Controls[j].Position = position;
                        }
                    }

                    if (m_rectangle.Y < owner.Position.Y)
                    {
                        int adjustY = owner.Position.Y - m_rectangle.Y;
                        m_rectangle.Y += adjustY;
                        m_touchDownPoint.Y += adjustY;
                        for (int j = Controls.Count - 1; j >= 0; j--)
                        {
                            Point position = Controls[j].Position;
                            position.Y += adjustY;
                            Controls[j].Position = position;
                        }
                    }

                    if (m_rectangle.X + m_rectangle.Width > owner.Position.X + owner.Width)
                    {
                        int adjustX = (m_rectangle.X + m_rectangle.Width) - (owner.Position.X + owner.Width);
                        m_rectangle.X -= adjustX;
                        m_touchDownPoint.X -= adjustX;
                        for (int j = this.Controls.Count - 1; j >= 0; j--)
                        {
                            Point position = Controls[j].Position;
                            position.X -= adjustX;
                            Controls[j].Position = position;
                        }
                    }

                    int heightCheck = m_rectangle.Y + m_rectangle.Height;

                    if (m_cropped && WindowTitle != null)
                        heightCheck = m_rectangle.Y + WindowTitle.Height;

                    if (heightCheck > owner.Position.Y + owner.Height)
                    {
                        int adjustY = (heightCheck) - (owner.Position.Y + owner.Height);
                        m_rectangle.Y -= adjustY;
                        m_touchDownPoint.Y -= adjustY;
                        for (int j = Controls.Count - 1; j >= 0; j--)
                        {
                            Point position = Controls[j].Position;
                            position.Y -= adjustY;
                            Controls[j].Position = position;
                        }
                    }
                }
                catch(Exception e)
                {
                    Utils.LogException(e);
                }
            }
        }

        public override void OnClientResize(Rectangle prev, Rectangle now, bool ownerOverride)
        {
            if (GuiManager.DraggedControl == this)
                GuiManager.StopDragging();

            Rectangle oldWindowRect = m_rectangle;

            if (m_anchors.Count == 0)
            {
                Rectangle prevClientBounds = Client.PrevClientBounds;

                int howFarFromTop = m_rectangle.Y;
                int howFarFromBottom = prevClientBounds.Height - (m_rectangle.Y + m_rectangle.Height);
                int howFarFromLeft = m_rectangle.X;
                int howFarFromRight = prevClientBounds.Width - (m_rectangle.X + m_rectangle.Width);

                // anchor to top
                if (howFarFromTop < howFarFromBottom)
                    m_anchors.Add(Enums.EAnchorType.Top);
                else if (howFarFromBottom < howFarFromTop) m_anchors.Add(Enums.EAnchorType.Bottom);

                if (howFarFromRight < howFarFromLeft)
                    m_anchors.Add(Enums.EAnchorType.Right);
                else if (howFarFromLeft < howFarFromRight) m_anchors.Add(Enums.EAnchorType.Left);

                // If equal it's dead center and should stay here. (Login Window)
            }

            base.OnClientResize(prev, now, ownerOverride);

            // TODO: anchors here

            // rectangle has been adjusted on resize
            if (oldWindowRect != m_rectangle)
            {
                for (int j = Controls.Count - 1; j >= 0; j--)
                {
                    //m_controls[j].OnClientResize(oldWindowRect, m_rectangle, true);
                    Point position = m_controls[j].Position;
                    Point oldPosition = new Point(position.X, position.Y);
                    position.X += m_rectangle.X - oldWindowRect.X;
                    position.Y += m_rectangle.Y - oldWindowRect.Y;
                    m_controls[j].Position = position;

                    // TODO: fix this. get anchors working or whatever, this is ridiculous. 6/16/2019 Eb
                    //if (m_controls[j] is ScrollableTextBox && Sheet != GuiManager.GenericSheet.Name && Client.GameState != Enums.EGameState.YuushaGame)
                    //if(!m_controls[j].Owner.Contains("FogOfWar"))
                    if(oldWindowRect.Width != m_rectangle.Width || oldWindowRect.Height != m_rectangle.Height)
                        m_controls[j].OnClientResize(oldWindowRect, m_rectangle, true);

                    if (WindowTitle != null)
                    {
                        WindowTitle.Width = Width;

                        if (WindowTitle.CloseBox != null)
                        {
                            position = WindowTitle.CloseBox.Position;
                            position.X += m_controls[j].Position.X - oldPosition.X;
                            position.Y += m_controls[j].Position.Y - oldPosition.Y;
                            WindowTitle.CloseBox.Position = position;
                        }

                        if (WindowTitle.MaximizeBox != null)
                        {
                            position = WindowTitle.MaximizeBox.Position;
                            position.X += m_controls[j].Position.X - oldPosition.X;
                            position.Y += m_controls[j].Position.Y - oldPosition.Y;
                            WindowTitle.MaximizeBox.Position = position;
                        }

                        if (WindowTitle.MinimizeBox != null)
                        {
                            position = WindowTitle.MinimizeBox.Position;
                            position.X += m_controls[j].Position.X - oldPosition.X;
                            position.Y += m_controls[j].Position.Y - oldPosition.Y;
                            WindowTitle.MinimizeBox.Position = position;
                        }

                        if (WindowTitle.CropBox != null)
                        {
                            position = WindowTitle.CropBox.Position;
                            position.X += m_controls[j].Position.X - oldPosition.X;
                            position.Y += m_controls[j].Position.Y - oldPosition.Y;
                            WindowTitle.CropBox.Position = position;
                        }
                    }

                    if (m_controls[j] is Window w)
                    {
                        Rectangle oldWindowRect2 = new Rectangle(m_controls[j].Position.X, m_controls[j].Position.Y, m_controls[j].Width, m_controls[j].Height);

                        for(int k = (m_controls[j] as Window).Controls.Count - 1; k >= 0; k--)
                        {
                            try
                            {
                                position = m_controls[k].Position;
                                // oldPosition -- for a window inside a window inside a window (doesn't exist yet as of 7/16/2019)
                                position.X += m_controls[j].Position.X - oldPosition.X;
                                position.Y += m_controls[j].Position.Y - oldPosition.Y;
                                m_controls[k].Position = position;

                                // TODO: fix this. get anchors working or whatever, this is ridiculous. 6/16/2019 Eb
                                //if (m_controls[k] is ScrollableTextBox && Sheet != GuiManager.GenericSheet.Name && Client.GameDisplayMode != Enums.EGameDisplayMode.Yuusha)
                                if (oldWindowRect2.Width != m_controls[j].Width || oldWindowRect2.Height != m_controls[j].Height)
                                    m_controls[k].OnClientResize(prev, now, true);
                            }
                            catch(ArgumentOutOfRangeException aoorE)
                            {
                                Utils.LogException(aoorE);
                                Utils.Log("INDEX: k=" + k + " j=" + j);
                                continue;
                            }
                            catch(Exception e)
                            {
                                Utils.LogException(e);
                                continue;
                            }
                        }

                        if(w.WindowTitle != null)
                        {
                            w.WindowTitle.Width = w.Width;

                            if(w.WindowTitle.CloseBox != null)
                            {
                                position = w.WindowTitle.CloseBox.Position;
                                position.X += m_controls[j].Position.X - oldPosition.X;
                                position.Y += m_controls[j].Position.Y - oldPosition.Y;
                                w.WindowTitle.CloseBox.Position = position;
                            }

                            if(w.WindowTitle.MaximizeBox != null)
                            {
                                position = w.WindowTitle.MaximizeBox.Position;
                                position.X += m_controls[j].Position.X - oldPosition.X;
                                position.Y += m_controls[j].Position.Y - oldPosition.Y;
                                w.WindowTitle.MaximizeBox.Position = position;
                            }

                            if(w.WindowTitle.MinimizeBox != null)
                            {
                                position = w.WindowTitle.MinimizeBox.Position;
                                position.X += m_controls[j].Position.X - oldPosition.X;
                                position.Y += m_controls[j].Position.Y - oldPosition.Y;
                                w.WindowTitle.MinimizeBox.Position = position;
                            }

                            if(w.WindowTitle.CropBox != null)
                            {
                                position = w.WindowTitle.CropBox.Position;
                                position.X += m_controls[j].Position.X - oldPosition.X;
                                position.Y += m_controls[j].Position.Y - oldPosition.Y;
                                w.WindowTitle.CropBox.Position = position;
                            }
                        }
                    }
                }
            }
        }

        public void SortControls()
        {
            var sorter = new ControlSorter();
            m_controls.Sort(sorter);
        }

        public void ForceMaximize()
        {
            var prevRect = m_rectangle;
            m_maximizeAtRect = new Rectangle(m_rectangle.X, m_rectangle.Y, m_rectangle.Width, m_rectangle.Height);
            m_rectangle = new Rectangle(0, 0, Client.Width, Client.Height);
            m_maximized = true;
            m_minimized = false;

            foreach (Control control in Controls)
            {
                Point position = control.Position;
                position.X += m_rectangle.X - prevRect.X;
                position.Y += m_rectangle.Y - prevRect.Y;
                control.Position = position;
                control.OnClientResize(prevRect, m_rectangle, true);
            }
        }

        public void ForcePosition(Point p, bool checkBounds)
        {
            if (Position.X == p.X && Position.Y == p.Y) return;
            //if (IsLocked && !GameHUD.NonDiscreetlyDraggableWindows.Contains(Name)) return;

            m_xOffset = p.X - Position.X;
            m_yOffset = p.Y - Position.Y;
            m_rectangle.X += m_xOffset;
            m_rectangle.Y += m_yOffset;

            // window is being dragged, move it's controls with it
            foreach (Control control in new List<Control>(Controls))
            {
                Point position = control.Position;
                position.X += XOffset;
                position.Y += YOffset;
                control.Position = position;

                // make a call here

                if (control is Window)
                {
                    foreach (Control winControl in new List<Control>((control as Window).Controls))
                    {
                        position = winControl.Position;
                        position.X += XOffset;
                        position.Y += YOffset;
                        winControl.Position = position;
                    }
                }

                if (control is WindowTitle)
                {
                    WindowTitle wt = control as WindowTitle;
                    if (wt.CloseBox != null)
                    {
                        position = wt.CloseBox.Position;
                        position.X += m_xOffset;
                        position.Y += m_yOffset;
                        wt.CloseBox.Position = position;
                    }
                    if (wt.MaximizeBox != null)
                    {
                        position = wt.MaximizeBox.Position;
                        position.X += m_xOffset;
                        position.Y += m_yOffset;
                        wt.MaximizeBox.Position = position;
                    }
                    if (wt.MinimizeBox != null)
                    {
                        position = wt.MinimizeBox.Position;
                        position.X += m_xOffset;
                        position.Y += m_yOffset;
                        wt.MinimizeBox.Position = position;
                    }
                    if (wt.CropBox != null)
                    {
                        position = wt.CropBox.Position;
                        position.X += m_xOffset;
                        position.Y += m_yOffset;
                        wt.CropBox.Position = position;
                    }
                }
            }

            if(checkBounds)
                CheckBoundsAndAdjust();
        }
    }
}