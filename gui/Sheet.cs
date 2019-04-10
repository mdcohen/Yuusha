using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Yuusha.gui
{
    public class Sheet
    {
        #region Private Data
        protected string m_name;
        protected string m_description;
        protected string m_filePath;
        protected Background m_background;
        protected string m_font;
        protected string m_cursor;
        protected string m_cursorOverride;
        protected int m_prevScrollWheelValue;
        protected int m_preferredWidth;
        protected int m_preferredHeight;
        protected Color m_deviceClearColor;
        protected bool m_allowFullScreen;
        protected List<Control> m_controls;
        protected List<TextCue> m_textCues;
        protected int m_currentTabOrder;
        #endregion

        #region Public Properties
        public string Name
        {
            get { return m_name; }
        }
        public string Description
        {
            get { return m_description; }
        }
        public List<Control> Controls
        {
            get { return m_controls; }
        }
        public Background Background
        {
            get { return m_background; }
            set { m_background = value; }
        }
        public string FilePath
        {
            get { return m_filePath; }
        }
        public string Font
        {
            get { return m_font; }
        }
        public string CursorOverride
        {
            get { return m_cursorOverride; }
            set { m_cursorOverride = value; }
        }
        public string Cursor
        {
            get { return m_cursor; }
        }
        public List<TextCue> TextCues
        {
            get { return m_textCues; }
        }
        public int PreviousScrollWheelValue
        {
            get { return m_prevScrollWheelValue; }
        }
        public bool AllowFullScreen
        {
            get { return m_allowFullScreen; }
        }
        public int CurrentTabOrder
        {
            get; set;
        }
        #endregion

        #region Constructor
        public Sheet(string filePath, System.Xml.XmlReader reader)
        {
            m_name = "";
            m_description = "";
            m_filePath = "";
            m_background = null;
            m_font = "";
            m_cursor = "";
            m_cursorOverride = "";
            m_prevScrollWheelValue = 0;
            m_preferredWidth = 1024;
            m_preferredHeight = 768;
            m_deviceClearColor = Color.Black;
            m_allowFullScreen = false;
            m_controls = new List<Control>();
            m_textCues = new List<TextCue>();
            m_currentTabOrder = -1;

            m_filePath = filePath;

            for (int i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);
                if (reader.Name == "Name")
                    m_name = reader.Value;
                else if (reader.Name == "Description")
                    m_description = reader.Value;
                else if (reader.Name == "Font")
                    m_font = reader.Value;
                else if (reader.Name == "Cursor")
                    m_cursor = reader.Value;
                else if (reader.Name == "PreferredWidth")
                    m_preferredWidth = reader.ReadContentAsInt();
                else if (reader.Name == "PreferredHeight")
                    m_preferredHeight = reader.ReadContentAsInt();
                else if (reader.Name == "DeviceClearColor")
                    m_deviceClearColor = Utils.GetColor(reader.Value);
                else if (reader.Name == "AllowFullScreen")
                    m_allowFullScreen = reader.ReadContentAsBoolean();
            }
        }
        #endregion

        public virtual void Update(GameTime gameTime)
        {
            if (Client.PreferredWindowWidth != m_preferredWidth || Client.PreferredWindowHeight != m_preferredHeight)
            {
                Client.PreferredWindowWidth = m_preferredWidth;
                Client.PreferredWindowHeight = m_preferredHeight;
            }

            if (Client.DeviceClearColor != m_deviceClearColor)
                Client.DeviceClearColor = m_deviceClearColor;

            if (Client.IsFullScreen && !m_allowFullScreen)
                Client.IsFullScreen = false;

            // keyboard handler
            if(!Yuusha.KeyboardHandler.HandleKeyboard())
                KeyboardHandler(Keyboard.GetState());

            // mouse handler
            MouseHandler(Mouse.GetState());

            // set mouse wheel value after mouse handler
            m_prevScrollWheelValue = Mouse.GetState().ScrollWheelValue;

            // clear cursor override
            m_cursorOverride = "";

            // update background
            if(m_background != null)
                m_background.Update(gameTime);

            // update controls
            foreach (Control control in m_controls)
                control.Update(gameTime);            

            // sort controls
            SortControls();

            // update text cues
            for (int a = m_textCues.Count - 1; a >= 0; a--)
                m_textCues[a].Update(gameTime, m_textCues);

            if (Client.RoundDelay)
                gui.GuiManager.CurrentSheet.CursorOverride = "Wait";

            // update cursor
            if (m_cursorOverride == "")
            {
                if (GuiManager.Cursors.ContainsKey(m_cursor))
                    GuiManager.Cursors[m_cursor].Update(gameTime);
                else Utils.LogOnce("Failed to find cursor visual key [ " + m_cursor + " ] for GUI Sheet [ " + m_name + " ]");
            }
            else
            {
                if (GuiManager.Cursors.ContainsKey(m_cursorOverride))
                    GuiManager.Cursors[m_cursorOverride].Update(gameTime);
                else Utils.LogOnce("Failed to find cursor override visual key [ " + m_cursorOverride + " ] for GUI Sheet [ " + m_name + " ]");
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            // draw the background
            if(m_background != null)
                m_background.Draw(gameTime);

            // draw controls
            foreach (Control control in m_controls)
                control.Draw(gameTime);

            // draw strings
            foreach (TextCue tc in m_textCues)
                tc.Draw(gameTime);

            // draw cursor
            if (m_cursorOverride == "")
            {
                if (GuiManager.Cursors.ContainsKey(m_cursor))
                    GuiManager.Cursors[m_cursor].Draw(gameTime);
                else Utils.LogOnce("Failed to find cursor visual key [ " + m_cursor + " ] for GUI Sheet [ " + m_name + " ]");
                
            }
            else
            {
                if (GuiManager.Cursors.ContainsKey(m_cursorOverride))
                    GuiManager.Cursors[m_cursorOverride].Draw(gameTime);
                else Utils.LogOnce("Failed to find cursor override visual key [ " + m_cursorOverride + " ] for GUI Sheet [ " + m_name + " ]");
            }

            // reset cursor override
            m_cursorOverride = "";
        }

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
                    if (m_controls[index] is Window)
                    {
                        foreach (Control c in (m_controls[index] as Window).Controls)
                        {
                            if (name == c.Name)
                                return c;
                            if (c is Window)
                            {
                                foreach (Control c2 in (c as Window).Controls)
                                {
                                    if (name == c2.Name)
                                        return c2;
                                }
                            }
                        }
                    }
                }

                if (!(this == GuiManager.GenericSheet))
                {
                    return GuiManager.GenericSheet[name];
                }

                return null;
            }
        }

        public void KeyboardHandler(KeyboardState ks)
        {
            if (!Client.HasFocus) return;

            // front to back
            for (int i = m_controls.Count - 1; i >= 0; i--)
            {
                // skip this control if disabled
                if (m_controls[i].IsDisabled)
                {
                    continue;
                }

                m_controls[i].KeyboardHandler(ks);
            }
        }

        public void MouseHandler(MouseState ms)
        {
            for (int i = m_controls.Count - 1; i >= 0; i--)
            {
                // No mouse handling if the control is disabled or not visible.
                if (m_controls[i].IsDisabled || !m_controls[i].IsVisible)
                    continue;

                // Always stop dragging if the left mouse button is not pressed.
                if (GuiManager.Dragging && ms.LeftButton != ButtonState.Pressed)
                    GuiManager.Dragging = false;

                // Always stop dragging if the moust pointer is not positioned in the dragged control.
                if (GuiManager.Dragging && !(GuiManager.DraggedControl.Contains(ms.Position)))
                    GuiManager.Dragging = false;

                int numControls = m_controls.Count;

                // not dragging or control is a window and mouse was handled by control
                if ((!GuiManager.Dragging || (m_controls[i] is Window)) && m_controls[i].MouseHandler(ms))
                {
                    if (numControls != m_controls.Count)
                    {
                        // if we are here then we used a control to delete another control, so 
                        // the index will be missing. Return to prevent IndexOutOfBounds
                        return;
                    }

                    if ((m_controls[i].ControlState == Enums.EControlState.Down) &&
                        !m_controls[i].IsLocked)
                    {
                        // start dragging, unless it is a minimized window
                        //if (!((m_controls[i] is WindowTitle) && (GuiManager.GetControl(m_controls[i].Owner) as Window).IsMinimized))
                        //{
                            GuiManager.Dragging = true;
                            GuiManager.DraggedControl = m_controls[i];
                            GuiManager.DraggingXOffset = ms.X - m_controls[i].Position.X;
                            GuiManager.DraggingYOffset = ms.Y - m_controls[i].Position.Y;
                        //}
                    }

                    if ((m_controls[i] is ComboBox) && (m_controls[i] as ComboBox).IsOpen)
                    {
                        if ((GuiManager.OpenComboBox != "") && GuiManager.OpenComboBox != m_controls[i].Name)
                        {
                            // close any open ComboBox
                            for (int j = 0; j < m_controls.Count; j++)
                            {
                                if (m_controls[j].Name == GuiManager.OpenComboBox)
                                {
                                    (m_controls[j] as ComboBox).IsOpen = false;
                                    break;
                                }
                            }
                        }
                        GuiManager.OpenComboBox = m_controls[i].Name;
                    }

                    if ((m_controls[i] is TextBox) && m_controls[i].HasFocus)
                    {
                        if (GuiManager.ActiveTextBox != "" && GuiManager.ActiveTextBox != m_controls[i].Name)
                        {
                            if(this[GuiManager.ActiveTextBox] != null)
                                this[GuiManager.ActiveTextBox].HasFocus = false;
                        }
                        GuiManager.ActiveTextBox = m_controls[i].Name;
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
                        //result = true;

                        // mouse is down over another control so close any open combobox
                        if ((GuiManager.OpenComboBox != "") && (GuiManager.OpenComboBox != m_controls[i].Name))
                        {
                            // close the open combobox
                            for (int j = 0; j < m_controls.Count; j++)
                            {
                                if (m_controls[j].Name == GuiManager.OpenComboBox)
                                {
                                    (m_controls[j] as ComboBox).IsOpen = false;
                                    GuiManager.OpenComboBox = "";
                                    break;
                                }
                            }
                        }

                        if ((GuiManager.ActiveTextBox != "") && (GuiManager.ActiveTextBox != m_controls[i].Name))
                        {
                            // release textbox focus
                            if(this[GuiManager.ActiveTextBox] != null)
                                this[GuiManager.ActiveTextBox].HasFocus = false;

                            GuiManager.ActiveTextBox = "";
                        }

                        Control c = m_controls[i];

                        #region Adjust Z Depth
                        if (m_controls[i].ZDepth != 1)
                        {
                            if (m_controls[i] == GuiManager.DraggedControl)
                            {
                                m_controls[i].ZDepth = 1;
                            }
                            else if (m_controls[i].Owner != "")
                            {
                                // control is inside a window, move window and controls to the front
                                this[m_controls[i].Owner].ZDepth = 1;
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
                    // clicked off a control so close any open combobox
                    if (GuiManager.OpenComboBox != "")
                    {
                        ComboBox openComboBox = this[GuiManager.OpenComboBox] as ComboBox;
                        Point cursor = new Point(ms.X, ms.Y);

                        if (!openComboBox.Contains(cursor))
                        {
                            openComboBox.IsOpen = false;
                            GuiManager.OpenComboBox = "";
                        }
                    }

                    if ((GuiManager.ActiveTextBox != "") && (GuiManager.ActiveTextBox != m_controls[i].Name))
                    {
                        // release textbox focus
                        if(this[GuiManager.ActiveTextBox] != null)
                            this[GuiManager.ActiveTextBox].HasFocus = false;

                        GuiManager.ActiveTextBox = "";
                    }
                }
            }
        }

        public void AddControl(Control c)
        {
            // set control sheet owner
            c.Sheet = this.Name;

            // control does not have an owner (Window), find highest z depth
            if (c.Owner == "")
            {
                if (m_controls.Count > 0)
                    c.ZDepth = m_controls[0].ZDepth + 1;
                else c.ZDepth = 1;

                if (c.TabOrder >= 0)
                    m_currentTabOrder = 0;

                m_controls.Add(c);
                SortControls();
            }
            else
            {
                Control owner = this[c.Owner];

                if (owner != null)
                {
                    if (owner is Window) AttachControlToWindow(c);
                    else if (owner is TextBox)
                    {
                        if (c is Border)
                            (owner as TextBox).Border = c as Border;
                    }
                    else if (owner is Label)
                    {
                        if (c is Border)
                            (owner as Label).Border = c as Border;

                        if (owner is CritterListLabel)
                            if (c is DropDownMenu)
                                (owner as CritterListLabel).DropDownMenu = c as DropDownMenu;
                    }
                    else if (owner is DropDownMenu)
                    {
                        if (c is Border)
                            (owner as DropDownMenu).Border = c as Border;
                        //TODO if c is DropDownMenuItem ?
                    }
                    else if (owner is HotButton)
                    {
                        if (c is SquareBorder)
                            (owner as HotButton).Border = c as SquareBorder;
                    }
                }
            }
        }

        public void RemoveControl(Control c)
        {
            if (m_controls.Contains(c))
                m_controls.Remove(c);
        }

        public void AttachControlToWindow(Control c)
        {
            Control w = this[c.Owner];

            if (!(w is Window))
            {
                Utils.Log("Exception: Tried to attach Control to a non-Window Control.");
                throw new Exception("Exception: Tried to attach Control to non-Window Control.");
            }

            c.Position = new Point(w.Position.X + c.Position.X, w.Position.Y + c.Position.Y);

            if ((w as Window).Controls.Count > 0)
            {
                c.ZDepth = (w as Window).Controls[0].ZDepth + 1;
            }
            else
            {
                c.ZDepth = 1;
            }

            if (c.TabOrder >= 0)
                (w as Window).CurrentTabOrder = 0;

            // add the control to the Window's list of controls
            (w as Window).Controls.Add(c);

            // sort the controls
            (w as Window).SortControls();

            // set this window's title or border for easy reference later
            if (c is WindowTitle && c.Name == w.Name + "Title")
            {
                (w as Window).WindowTitle = c as WindowTitle;
            }
            else if (c is Border && c.Name == w.Name + "Border")
            {
                (w as Window).WindowBorder = c as Border;
            }
            
        }

        public void SortControls()
        {
            ControlSorter sorter = new ControlSorter();
            m_controls.Sort(sorter);
        }

        public void OnClientResize(Rectangle prev, Rectangle now)
        {
            foreach (Control control in m_controls)
                control.OnClientResize(prev, now, false);
        }

        /// <summary>
        /// Handles all actions when tabbing forward through controls with a TabOrder > -1.
        /// </summary>
        public void HandleTabOrderForward()
        {
            Control currentFocus = GuiManager.ControlWithFocus;

            //TextCue.AddClientInfoTextCue(currentFocus != null ? currentFocus.Name : "null", TextCue.TextCueTag.None, Color.Red, Color.Black, 1500, false, false, true);

            #region This sheet has a tab order.
            if (CurrentTabOrder >= 0)
            {
                var tabOrdered = new List<Control>();
                Controls.ForEach(x => { if (x.TabOrder >= 0 && x.IsVisible && !x.IsDisabled) tabOrdered.Add(x); }); // all the controls in the tab order collection

                if (tabOrdered.Count > 1)
                {
                    tabOrdered.Sort((s1, s2) => s1.TabOrder.CompareTo(s2.TabOrder)); // ascending numerical sort by tab order

                    if (currentFocus != null && currentFocus.TabOrder > tabOrdered.Count - 1)
                    {
                        currentFocus.HasFocus = false;
                        tabOrdered[0].HasFocus = true;
                        CurrentTabOrder = tabOrdered[0].TabOrder;
                    }

                    foreach (Control c in tabOrdered)
                    {
                        if (c.TabOrder > CurrentTabOrder)
                        {
                            c.HasFocus = true;
                            CurrentTabOrder = c.TabOrder;
                            break;
                        }
                    }
                }
            }
            #endregion

            // Iterate through controls of this sheet. Currently only checks Windows that are visible and have a tab order.
            foreach (Control c in this.Controls)
            {
                if (c is Window && c.IsVisible && (c as Window).CurrentTabOrder > -1)
                {
                    var tabOrdered = new List<Control>();

                    (c as Window).Controls.ForEach(x => { if (x.TabOrder >= 0 && x.IsVisible && !x.IsDisabled) tabOrdered.Add(x); });

                    tabOrdered.Sort((s1, s2) => s1.TabOrder.CompareTo(s2.TabOrder)); // numerical sort by tab order

                    if (currentFocus != null && currentFocus.TabOrder == tabOrdered.Count - 1)
                    {
                        currentFocus.HasFocus = false;
                        tabOrdered[0].HasFocus = true;
                        (c as Window).CurrentTabOrder = tabOrdered[0].TabOrder; break;
                    }

                    if (tabOrdered.Count > 1)
                    {
                        foreach (Control c2 in tabOrdered)
                        {
                            if (c2.TabOrder > (c as Window).CurrentTabOrder)
                            {
                                c2.HasFocus = true;
                                (c as Window).CurrentTabOrder = c2.TabOrder;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles all actions when tabbing in reverse through controls with a TabOrder > -1.
        /// </summary>
        public void HandleTabOrderReverse()
        {
            Control currentFocus = GuiManager.ControlWithFocus;

            //TextCue.AddClientInfoTextCue(currentFocus != null ? currentFocus.Name : "null", TextCue.TextCueTag.None, Color.Red, Color.Black, 1500, false, false, true);

            #region This sheet has a tab order.
            if (CurrentTabOrder >= 0)
            {
                var tabOrdered = new List<Control>();
                Controls.ForEach(x => { if (x.TabOrder >= 0 && x.IsVisible && !x.IsDisabled) tabOrdered.Add(x); }); // all the controls in the tab order collection

                if (tabOrdered.Count > 1)
                {
                    tabOrdered.Sort((s1, s2) => s1.TabOrder.CompareTo(s2.TabOrder)); // ascending numerical sort by tab order
                    tabOrdered.Reverse();

                    if (currentFocus != null && currentFocus.TabOrder == 0 && tabOrdered.Count > 1)
                    {
                        currentFocus.HasFocus = false;
                        tabOrdered[tabOrdered.Count - 1].HasFocus = true;
                        CurrentTabOrder = tabOrdered[tabOrdered.Count - 1].TabOrder;
                    }
                    else
                    {
                        foreach (Control c in tabOrdered)
                        {
                            if (c.TabOrder < CurrentTabOrder)
                            {
                                c.HasFocus = true;
                                CurrentTabOrder = c.TabOrder;
                                break;
                            }
                        }
                    }
                }
            }
            #endregion

            // Iterate through controls of this sheet. Currently only checks Windows that are visible and have a tab order.
            foreach (Control c in this.Controls)
            {
                if (c is Window && c.IsVisible && (c as Window).CurrentTabOrder > -1)
                {
                    var tabOrdered = new List<Control>();

                    (c as Window).Controls.ForEach(x => { if (x.TabOrder >= 0 && x.IsVisible && !x.IsDisabled) tabOrdered.Add(x); });

                    tabOrdered.Sort((s1, s2) => s1.TabOrder.CompareTo(s2.TabOrder)); // numerical sort by tab order

                    tabOrdered.Reverse();

                    if (currentFocus != null && currentFocus.TabOrder == 0 && tabOrdered.Count > 1)
                    {
                        currentFocus.HasFocus = false;
                        tabOrdered[tabOrdered.Count - 1].HasFocus = true;
                        (c as Window).CurrentTabOrder = tabOrdered[tabOrdered.Count - 1].TabOrder; break;
                    }
                    else
                    {
                        if (tabOrdered.Count > 1)
                        {
                            foreach (Control c2 in tabOrdered)
                            {
                                if (c2.TabOrder < (c as Window).CurrentTabOrder)
                                {
                                    c2.HasFocus = true;
                                    (c as Window).CurrentTabOrder = c2.TabOrder;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        #region Create Controls
        public void CreateWindow(string name, string type, string owner, Rectangle rectangle, bool visible, bool locked, bool disabled,
            string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, bool dropShadow,
            Map.Direction shadowDirection, int shadowDistance, List<Enums.EAnchorType> anchors, string cursorOverride)
        {
            if (type == "Window")
                AddControl(new Window(name, owner, rectangle, visible, locked, disabled, font, visualKey, tintColor,
                    visualAlpha, borderAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride));
            else if (type == "HotButtonEditWindow")
            {
                AddControl(new HotButtonEditWindow(name, owner, rectangle, visible, locked, disabled, font, visualKey, tintColor,
                    visualAlpha, borderAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride));
            }
        }

        public void CreateWindowTitle(string name, string owner, string font, string text, Color textColor, Color tintColor,
            BitmapFont.TextAlignment textAlignment, VisualKey visualKey, bool visualTiled, VisualKey closeBoxVisualKey,
            VisualKey maxBoxVisualKey, VisualKey minBoxVisualKey, VisualKey cropBoxVisualKey, VisualKey closeBoxVisualKeyDown,
            VisualKey maxBoxVisualKeyDown, VisualKey minBoxVisualKeyDown, VisualKey cropBoxVisualKeyDown,
            int closeBoxDistanceFromRight, int closeBoxDistanceFromTop, int closeBoxWidth, int closeBoxHeight,
            int maxBoxDistanceFromRight, int maxBoxDistanceFromTop, int maxBoxWidth, int maxBoxHeight,
            int minBoxDistanceFromRight, int minBoxDistanceFromTop, int minBoxWidth, int minBoxHeight,
            int cropBoxDistanceFromRight, int cropBoxDistanceFromTop, int cropBoxWidth, int cropBoxHeight)
        {
            AddControl(new WindowTitle(name, owner, font, text, textColor, tintColor, textAlignment, visualKey, visualTiled, closeBoxVisualKey,
                maxBoxVisualKey, minBoxVisualKey, cropBoxVisualKey, closeBoxVisualKeyDown, maxBoxVisualKeyDown, minBoxVisualKeyDown,
                cropBoxVisualKeyDown, closeBoxDistanceFromRight, closeBoxDistanceFromTop, closeBoxWidth, closeBoxHeight, maxBoxDistanceFromRight,
                maxBoxDistanceFromTop, maxBoxWidth, maxBoxHeight, minBoxDistanceFromRight, minBoxDistanceFromTop, minBoxWidth,
                minBoxHeight, cropBoxDistanceFromRight, cropBoxDistanceFromTop, cropBoxWidth, cropBoxHeight));
        }

        public void CreateSquareBorder(string name, string owner, int width, VisualKey visualKey, bool visualTiled, Color tintColor)
        {
            AddControl(new SquareBorder(name, owner, width, visualKey, visualTiled, tintColor));
        }

        public void CreateTextBox(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, byte textAlpha,
            bool editable, int maxLength, bool passwordBox, bool blinkingCursor, Color cursorColor,
            VisualKey visualKeyOver, VisualKey visualKeyDown, VisualKey visualKeyDisabled, int xTextOffset,
            int yTextOffset, string onKeyboardEnter, Color selectionColor, List<Enums.EAnchorType> anchors, int tabOrder)
        {
            AddControl(new TextBox(name, owner, rectangle, text, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha,
                borderAlpha, textAlpha, editable, maxLength, passwordBox, blinkingCursor, cursorColor, visualKeyOver,
                visualKeyDown, visualKeyDisabled, xTextOffset, yTextOffset, onKeyboardEnter, selectionColor, anchors, tabOrder));
        }

        public void CreateButton(string type, string name, string owner, Rectangle rectangle, string text, bool textVisible, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, byte textAlpha,
            VisualKey visualKeyOver, VisualKey visualKeyDown, VisualKey visualKeyDisabled, string clickEvent,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, Color textOverColor, bool hasTextOverColor,
            List<Enums.EAnchorType> anchors, bool dropShadow, Map.Direction shadowDirection, int shadowDistance, string command)
        {
            if (type == "HotButton")
            {
                AddControl(new HotButton(name, owner, rectangle, text, textVisible, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha,
                   borderAlpha, textAlpha, visualKeyOver, visualKeyDown, visualKeyDisabled, clickEvent, textAlignment,
                   xTextOffset, yTextOffset, textOverColor, hasTextOverColor, anchors, dropShadow, shadowDirection,
                   shadowDistance, command));
            }
            else if (type == "IconImageSelectionButton")
            {
                AddControl(new IconImageSelectionButton(name, owner, rectangle, text, textVisible, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha,
                   borderAlpha, textAlpha, visualKeyOver, visualKeyDown, visualKeyDisabled, clickEvent, textAlignment,
                   xTextOffset, yTextOffset, textOverColor, hasTextOverColor, anchors, dropShadow, shadowDirection,
                   shadowDistance, command));
            }
            else if(type == "MacroButton")
            {
                AddControl(new MacroButton(name, owner, rectangle, text, textVisible, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha,
                   borderAlpha, textAlpha, visualKeyOver, visualKeyDown, visualKeyDisabled, clickEvent, textAlignment,
                   xTextOffset, yTextOffset, textOverColor, hasTextOverColor, anchors, dropShadow, shadowDirection,
                   shadowDistance, command));
            }
            else
            {
                AddControl(new Button(name, owner, rectangle, text, textVisible, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha,
                    borderAlpha, textAlpha, visualKeyOver, visualKeyDown, visualKeyDisabled, clickEvent, textAlignment,
                    xTextOffset, yTextOffset, textOverColor, hasTextOverColor, anchors, dropShadow, shadowDirection,
                    shadowDistance, command));
            }
        }

        public void CreateDropDownMenu(string name, Control owner, string title, Rectangle rectangle, bool visible, string font, VisualKey visualKey,
            Color tintColor, int visualAlpha, bool dropShadow, Map.Direction shadowDirection, int shadowDistance)
        {
            AddControl(new DropDownMenu(name, owner, title, rectangle, visible, font, visualKey, tintColor, visualAlpha, dropShadow, shadowDirection, shadowDistance));
        }

        public void CreateLabel(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, byte textAlpha,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string onDoubleClickEvent,
            string cursorOverride, List<Enums.EAnchorType> anchors, string popUpText)
        {
            AddControl(new Label(name, owner, rectangle, text, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha,
                borderAlpha, textAlpha, textAlignment, xTextOffset, yTextOffset, onDoubleClickEvent, cursorOverride,
                anchors, popUpText));
        }

        public void CreateCritterListLabel(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, byte textAlpha,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string onDoubleClickEvent,
            string cursorOverride, List<Enums.EAnchorType> anchors, string popUpText)
        {
            AddControl(new CritterListLabel(name, owner, rectangle, text, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha,
               borderAlpha, textAlpha, textAlignment, xTextOffset, yTextOffset, onDoubleClickEvent, cursorOverride,
               anchors, popUpText));

            SquareBorder border = new SquareBorder(name + "SquareBorder", name, Client.UserSettings.TargetBorderSize, new VisualKey("WhiteSpace"), false, Client.UserSettings.ColorTargetBorder);
            border.IsVisible = false;
            AddControl(border);

            //DropDownMenu menu = new DropDownMenu(name + "DropDownMenu", name, "", new Rectangle(rectangle.X, rectangle.Y, 100, 100), false,
            //            this.Font, new VisualKey("WhiteSpace"), Client.ClientSettings.ColorDropDownMenu, visualAlpha, true, Map.Direction.Northwest, 5);
            //AddControl(menu);
        }

        public void CreateIOKTileLabel(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, byte textAlpha,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string onDoubleClickEvent,
            string cursorOverride, List<Enums.EAnchorType> anchors, string popUpText)
        {
            AddControl(new IOKTileLabel(name, owner, rectangle, text, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha,
                borderAlpha, textAlpha, textAlignment, xTextOffset, yTextOffset, onDoubleClickEvent, cursorOverride,
                anchors, popUpText));
        }

        public void CreateSpinelTileLabel(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, byte textAlpha,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string onDoubleClickEvent,
            string cursorOverride, List<Enums.EAnchorType> anchors, string popUpText)
        {
            AddControl(new SpinelTileLabel(name, owner, rectangle, text, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha,
                borderAlpha, textAlpha, textAlignment, xTextOffset, yTextOffset, onDoubleClickEvent, cursorOverride,
                anchors, popUpText));
        }

        public void CreateScrollableTextBox(string name, string owner, Rectangle rectangle, string text, Color textColor,
            bool visible, bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha,
            byte borderAlpha, byte textAlpha, VisualKey visualKeyOver, VisualKey visualKeyDown,
            VisualKey visualKeyDisabled, int xTextOffset, int yTextOffset, BitmapFont.TextAlignment textAlignment,
            List<Enums.EAnchorType> anchors, bool trim)
        {
            AddControl(new ScrollableTextBox(name, owner, rectangle, text, textColor, visible, disabled, font, visualKey,
                tintColor, visualAlpha, borderAlpha, textAlpha, visualKeyOver, visualKeyDown, visualKeyDisabled, xTextOffset,
                yTextOffset, textAlignment, anchors, trim));
        }

        public void CreateStatusBar(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, byte textAlpha,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string onDoubleClickEvent,
            string cursorOverride, List<Enums.EAnchorType> anchors, Enums.ELayoutType layoutType)
        {
            AddControl(new StatusBar(name, owner, rectangle, text, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha,
                borderAlpha, textAlpha, textAlignment, xTextOffset, yTextOffset, onDoubleClickEvent, cursorOverride,
                anchors, layoutType));
        }
        #endregion

        /// <summary>
        /// As of 4/9/2019 this is never referenced.
        /// </summary>
        /// <param name="name"></param>
        public void DestroyControl(string name)
        {
            Control c = this[name];

            if (c != null)
            {
                c.OnDestroy();
            }
        }
    }
}