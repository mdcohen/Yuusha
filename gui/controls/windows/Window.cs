using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        #endregion

        #region Constructor
        public Window(string name, string owner, Rectangle rectangle, bool visible, bool locked, bool disabled,
            string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, bool dropShadow,
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
            m_borderAlpha = borderAlpha;
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
                for (int index = 0; index < m_controls.Count; index++)
                {
                    if (name == m_controls[index].Name)
                    {
                        return m_controls[index];
                    }
                }
                Utils.LogOnce("GUI Sheet [ " + GuiManager.CurrentSheet.Name + " ], Window [ " + m_name + " ] returned null for Control [ " + name + " ]");
                return null;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (Control control in m_controls)
            {
                if (!m_cropped || ((control is WindowControlBox) || (control is WindowTitle)))
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

                foreach (Control control in m_controls)
                    control.Draw(gameTime);
            }
            else
            {
                if (m_dropShadow && m_visualKey != null && GuiManager.Visuals.ContainsKey(m_visualKey.Key))
                {
                    VisualInfo vi = GuiManager.Visuals[m_visualKey.Key];
                    Rectangle sourceRect = new Rectangle(vi.X, vi.Y, vi.Width, vi.Height);

                    Rectangle shadowRect = new Rectangle(WindowTitle.Position.X + GetXShadow(), WindowTitle.Position.Y + GetYShadow(),
                        WindowTitle.Width, WindowTitle.Height);
                    Color shadowColor = new Color((int)Color.Black.R, (int)Color.Black.G, (int)Color.Black.B, 50);
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], shadowRect, sourceRect, shadowColor);

                }

                WindowTitle.Draw(gameTime);
                if (!m_minimized && !m_cropped)
                    WindowBorder.Draw(gameTime);
            }
        }

        public override bool KeyboardHandler(KeyboardState ks)
        {
            if (this.m_cropped || this.IsMinimized || !this.IsVisible || this.IsDisabled)
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
            if (!this.IsVisible || this.IsDisabled)
                return false;

            if (this.m_cropped || this.IsMinimized)
            {
                foreach (Control c in this.Controls)
                {
                    if (c is WindowTitle || c is WindowControlBox)
                        return c.MouseHandler(ms);
                }
            }

            for (int i = m_controls.Count - 1; i >= 0; i--)
            {
                // skip this control if disabled
                if (m_controls[i].IsDisabled || !m_controls[i].IsVisible)
                    continue;

                // stop dragging if the left button is no longer pressed
                if (m_dragging && ms.LeftButton != ButtonState.Pressed)
                {
                    m_dragging = false;
                }

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

                    if ((m_controls[i].ControlState == Enums.EControlState.Down) &&
                        !m_controls[i].IsLocked)
                    {
                        m_dragging = true;
                    }

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
                            if ((m_controls[i] is Window) && ((m_controls[i] as Window).Controls.Count > 0))
                            {
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

            // do not drag window if mouse is down over a control
            foreach (Control control in m_controls)
            {
                if (control.ControlState == Enums.EControlState.Down && !(control is WindowTitle) &&
                    !(control is Border))
                {
                    GuiManager.Dragging = false;
                    GuiManager.DraggedControl = null;
                    break;
                }
                else if (control.ControlState == Enums.EControlState.Down && (control is WindowTitle) &&
                    (control as WindowTitle).ControlBoxContains(new Point(ms.X, ms.Y)))
                {
                    GuiManager.Dragging = true;
                    GuiManager.DraggedControl = this;
                    break;
                }
            }

            if (this.m_minimized)
            {
                GuiManager.Dragging = false;
                GuiManager.DraggedControl = null;
            }

            if (this.m_maximized)
            {
                GuiManager.Dragging = false;
                GuiManager.DraggedControl = null;
                return;
            }

            m_xOffset = ms.X - m_touchDownPoint.X;
            m_yOffset = ms.Y - m_touchDownPoint.Y;
            m_rectangle.X += m_xOffset;
            m_rectangle.Y += m_yOffset;
            m_touchDownPoint.X += m_xOffset;
            m_touchDownPoint.Y += m_yOffset;

            // window is being dragged, move it's controls with it
            for (int j = Controls.Count - 1; j >= 0; j--)
            {
                Point position = this.Controls[j].Position;
                position.X += this.XOffset;
                position.Y += this.YOffset;
                Controls[j].Position = position;

                // IMPORTANT: (5/29/2019) Currently only one layer of nested windows is dragged. This will need recursiveness if adding more nested windows.
                if (Controls[j] is Window)
                {
                    foreach (Control winControl in (Controls[j] as Window).Controls)
                    {
                        position = winControl.Position;
                        position.X += this.XOffset;
                        position.Y += this.YOffset;
                        winControl.Position = position;
                    }
                }

                if (Controls[j] is WindowTitle)
                {
                    WindowTitle wt = this.Controls[j] as WindowTitle;
                    if (wt.CloseBox != null)
                    {
                        position = wt.CloseBox.Position;
                        position.X += this.XOffset;
                        position.Y += this.YOffset;
                        wt.CloseBox.Position = position;
                    }
                    if (wt.MaximizeBox != null)
                    {
                        position = wt.MaximizeBox.Position;
                        position.X += this.XOffset;
                        position.Y += this.YOffset;
                        wt.MaximizeBox.Position = position;
                    }
                    if (wt.MinimizeBox != null)
                    {
                        position = wt.MinimizeBox.Position;
                        position.X += this.XOffset;
                        position.Y += this.YOffset;
                        wt.MinimizeBox.Position = position;
                    }
                    if (wt.CropBox != null)
                    {
                        position = wt.CropBox.Position;
                        position.X += this.XOffset;
                        position.Y += this.YOffset;
                        wt.CropBox.Position = position;
                    }
                }
            }

            CheckBoundsAndAdjust();
        }

        protected override void OnMouseOver(MouseState ms)
        {
            base.OnMouseOver(ms);

            // return if a control contains the cursor, since the window will not drag
            // unless control is window title or border
            foreach (Control control in m_controls)
            {
                if (!(control is WindowTitle) && !(control is Border) &&
                    control.Contains(new Point(ms.X, ms.Y)))
                    return;
                else if (control is WindowTitle && (control as WindowTitle).ControlBoxContains(new Point(ms.X, ms.Y)))
                    return;
            }

            // show drag cursor if not locked, the window title has the hotspot, the window border has the hotspot
            if (m_cursorOverride != "" && !m_locked &&
                (m_windowTitle != null && m_windowTitle.Contains(new Point(ms.X, ms.Y)) || (m_windowBorder != null && m_windowBorder.Contains(new Point(ms.X, ms.Y)) &&
                !m_minimized && !m_cropped)))
            {
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
            this.IsVisible = false;

            foreach (Control c in this.Controls)
                c.HasFocus = false;
        }

        private void CheckBoundsAndAdjust()
        {
            if (m_owner == "")
            {
                if (m_rectangle.X < 0)
                {
                    int adjustX = Math.Abs(m_rectangle.X);
                    m_rectangle.X += adjustX;
                    m_touchDownPoint.X += adjustX;
                    for (int j = this.Controls.Count - 1; j >= 0; j--)
                    {
                        Point position = this.Controls[j].Position;
                        position.X += adjustX;
                        this.Controls[j].Position = position;
                    }
                }

                if (m_rectangle.Y < 0)
                {
                    int adjustY = Math.Abs(m_rectangle.Y);
                    m_rectangle.Y += adjustY;
                    m_touchDownPoint.Y += adjustY;
                    for (int j = this.Controls.Count - 1; j >= 0; j--)
                    {
                        Point position = this.Controls[j].Position;
                        position.Y += adjustY;
                        this.Controls[j].Position = position;
                    }
                }

                if (m_rectangle.X + m_rectangle.Width > Client.Width)
                {
                    int adjustX = Math.Abs((m_rectangle.X + m_rectangle.Width) - Client.Width);
                    m_rectangle.X -= adjustX;
                    m_touchDownPoint.X -= adjustX;
                    for (int j = this.Controls.Count - 1; j >= 0; j--)
                    {
                        Point position = this.Controls[j].Position;
                        position.X -= adjustX;
                        this.Controls[j].Position = position;
                    }
                }

                int heightCheck = m_rectangle.Y + m_rectangle.Height;

                if (m_cropped && WindowTitle != null)
                    heightCheck = m_rectangle.Y + WindowTitle.Height;

                if (heightCheck > Client.Height)
                {
                    int adjustY = Math.Abs((heightCheck) - Client.Height);
                    m_rectangle.Y -= adjustY;
                    m_touchDownPoint.Y -= adjustY;
                    for (int j = this.Controls.Count - 1; j >= 0; j--)
                    {
                        Point position = this.Controls[j].Position;
                        position.Y -= adjustY;
                        this.Controls[j].Position = position;
                    }
                }
            }
            else
            {
                Sheet sheet = GuiManager.Sheets[Client.GameState.ToString()];

                Window owner = sheet[m_owner] as Window;

                if (owner == null)
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
                    for (int j = this.Controls.Count - 1; j >= 0; j--)
                    {
                        Point position = this.Controls[j].Position;
                        position.X += adjustX;
                        this.Controls[j].Position = position;
                    }
                }

                if (m_rectangle.Y < owner.Position.Y)
                {
                    int adjustY = owner.Position.Y - m_rectangle.Y;
                    m_rectangle.Y += adjustY;
                    m_touchDownPoint.Y += adjustY;
                    for (int j = this.Controls.Count - 1; j >= 0; j--)
                    {
                        Point position = this.Controls[j].Position;
                        position.Y += adjustY;
                        this.Controls[j].Position = position;
                    }
                }

                if (m_rectangle.X + m_rectangle.Width > owner.Position.X + owner.Width)
                {
                    int adjustX = (m_rectangle.X + m_rectangle.Width) - (owner.Position.X + owner.Width);
                    m_rectangle.X -= adjustX;
                    m_touchDownPoint.X -= adjustX;
                    for (int j = this.Controls.Count - 1; j >= 0; j--)
                    {
                        Point position = this.Controls[j].Position;
                        position.X -= adjustX;
                        this.Controls[j].Position = position;
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
                    for (int j = this.Controls.Count - 1; j >= 0; j--)
                    {
                        Point position = this.Controls[j].Position;
                        position.Y -= adjustY;
                        this.Controls[j].Position = position;
                    }
                }
            }
        }

        public override void OnClientResize(Rectangle prev, Rectangle now, bool ownerOverride)
        {
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
                for (int j = this.Controls.Count - 1; j >= 0; j--)
                {
                    Point position = m_controls[j].Position;
                    position.X += (m_rectangle.X - oldWindowRect.X);
                    position.Y += (m_rectangle.Y - oldWindowRect.Y);
                    m_controls[j].Position = position;

                    if (m_controls[j] is ScrollableTextBox && this.Sheet != GuiManager.GenericSheet.Name)
                        m_controls[j].OnClientResize(prev, now, true);
                }
            }
        }

        public void SortControls()
        {
            ControlSorter sorter = new ControlSorter();
            m_controls.Sort(sorter);
        }

        public void ForceMaximize()
        {
            Rectangle prevRect = this.m_rectangle;
            m_maximizeAtRect = new Rectangle(m_rectangle.X, m_rectangle.Y, m_rectangle.Width, m_rectangle.Height);
            m_rectangle = new Rectangle(0, 0, Client.Width, Client.Height);
            m_maximized = true;
            m_minimized = false;

            foreach (Control control in this.Controls)
            {
                Point position = control.Position;
                position.X += (m_rectangle.X - prevRect.X);
                position.Y += (m_rectangle.Y - prevRect.Y);
                control.Position = position;
                control.OnClientResize(prevRect, m_rectangle, true);
            }
        }
    }
}