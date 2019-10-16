using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
            try
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
                if (!Yuusha.KeyboardHandler.HandleKeyboard())
                    KeyboardHandler(Keyboard.GetState());

                // mouse handler
                MouseHandler(Mouse.GetState());

                // set mouse wheel value after mouse handler
                m_prevScrollWheelValue = Mouse.GetState().ScrollWheelValue;

                // clear cursor override
                m_cursorOverride = "";

                // update background
                if (m_background != null)
                    m_background.Update(gameTime);

                // update controls
                foreach (Control control in new List<Control>(m_controls))
                    control.Update(gameTime);

                // sort controls
                SortControls();

                // update text cues
                //for (int a = m_textCues.Count - 1; a >= 0; a--)
                //    m_textCues[a].Update(gameTime, m_textCues);

                if (Client.RoundDelay)
                    GuiManager.GenericSheet.CursorOverride = "Wait";

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
            catch (Exception e)
            {
                Utils.LogException(e);
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            try
            {
                // draw the background
                if (m_background != null && m_background.IsVisible)
                    m_background.Draw(gameTime);

                // draw controls
                foreach (Control control in new List<Control>(m_controls))
                    control.Draw(gameTime);

                // draw strings
                foreach (TextCue tc in m_textCues)
                    tc.Draw(gameTime);

                // GenericSheet draws the MouseCursor
            }
            catch (Exception e)
            {
                Utils.LogException(e);
            }

            // reset cursor override
            m_cursorOverride = "";
        }

        public Control this[string name]
        {
            get
            {
                try
                {
                    lock (m_controls)
                    {
                        for (int index = 0; index < m_controls.Count; index++)
                        {
                            if (name == m_controls[index].Name)
                                return m_controls[index];

                            if (m_controls[index] is Window)
                            {
                                //for (int index1 = 0; index1 < (m_controls[index] as Window).Controls.Count; index1++)
                                //{
                                //    if (name == (m_controls[index] as Window).Controls[index1].Name)
                                //    {
                                //        return (m_controls[index] as Window).Controls[index1];
                                //    }
                                //    else if ((m_controls[index] as Window).Controls[index1] is Window)
                                //    {
                                //        for (int index2 = 0; index2 < ((m_controls[index] as Window).Controls[index1] as Window).Controls.Count; index2++)
                                //        {
                                //            if (name == ((m_controls[index] as Window).Controls[index1] as Window).Controls[index2].Name)
                                //            {
                                //                return ((m_controls[index] as Window).Controls[index1] as Window).Controls[index2];
                                //            }
                                //            else if (((m_controls[index] as Window).Controls[index1] as Window).Controls[index2] is Window)
                                //            {
                                //                for (int index3 = 0; index3 < (((m_controls[index] as Window).Controls[index1] as Window).Controls[index2] as Window).Controls.Count; index3++)
                                //                {
                                //                    if (name == (((m_controls[index] as Window).Controls[index1] as Window).Controls[index2] as Window).Controls[index3].Name)
                                //                    {
                                //                        return (((m_controls[index] as Window).Controls[index1] as Window).Controls[index2] as Window).Controls[index3];
                                //                    }
                                //                }
                                //            }
                                //        }
                                //    }
                                //}

                                lock ((m_controls[index] as Window).Controls)
                                {
                                    foreach (Control c in new List<Control>((m_controls[index] as Window).Controls))
                                    {
                                        if (c != null && name == c.Name)
                                            return c;

                                        if (c is Window)
                                        {
                                            lock ((c as Window).Controls)
                                            {
                                                foreach (Control c2 in new List<Control>((c as Window).Controls))
                                                {
                                                    if (c2 != null && name == c2.Name)
                                                        return c2;

                                                    if (c2 is Window)
                                                    {
                                                        lock ((c2 as Window).Controls)
                                                        {
                                                            foreach (Control c3 in new List<Control>((c2 as Window).Controls))
                                                            {
                                                                if (name == c3.Name)
                                                                    return c3;

                                                                if (c3 is Window)
                                                                {
                                                                    lock ((c3 as Window).Controls)
                                                                    {
                                                                        foreach (Control c4 in new List<Control>((c3 as Window).Controls))
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
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (GuiManager.GenericSheet != null && !(this == GuiManager.GenericSheet))
                        return GuiManager.GenericSheet[name];

                    return null;
                }
                catch (Exception)
                {
                    //TODO Figure out why there is an Exception here when shutting down.
                    //Utils.LogException(e);
                    //Utils.Log("Exception detail: this[" + name + "] for Sheet: " + Name);
                    return null;
                }
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
                    continue;

                m_controls[i].KeyboardHandler(ks);
            }
        }

        public void MouseHandler(MouseState ms)
        {
            if (!Client.HasFocus) return;

            for (int i = m_controls.Count - 1; i >= 0; i--)
            {
                // No mouse handling if the control is disabled or not visible.
                if (m_controls[i].IsDisabled || !m_controls[i].IsVisible)
                    continue;

                // Always stop dragging if the left mouse button is not pressed.
                if (GuiManager.IsDragging && ms.LeftButton != ButtonState.Pressed)
                    GuiManager.StopDragging();

                //// Always stop dragging if the mouse pointer is not positioned in the dragged control.
                //if (GuiManager.Dragging && !(GuiManager.DraggedControl.Contains(ms.Position)))
                //    GuiManager.StopDragging();

                int numControls = m_controls.Count;

                // not dragging or control is a window and mouse was handled by control
                if ((!GuiManager.IsDragging || (m_controls[i] is Window)) && m_controls[i].MouseHandler(ms))
                {
                    if (numControls != m_controls.Count)
                    {
                        // if we are here then we used a control to delete another control, so 
                        // the index will be missing. Return to prevent IndexOutOfBounds
                        return;
                    }

                    //if ((m_controls[i].ControlState == Enums.EControlState.Down) && !m_controls[i].IsLocked)
                    //{
                    //    GuiManager.StartDragging(m_controls[i], ms);
                    //}

                    // Close any open ComboBox when dragging.
                    if ((m_controls[i] is ComboBox) && (m_controls[i] as ComboBox).IsOpen)
                    {
                        if ((GuiManager.OpenComboBox != "") && GuiManager.OpenComboBox != m_controls[i].Name)
                        {
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

                    // Remove focus from any focus from a TextBox that is not the ActiveTextBox.
                    if ((m_controls[i] is TextBox) && m_controls[i].HasFocus)
                    {
                        if (!string.IsNullOrEmpty(GuiManager.ActiveTextBox) && GuiManager.ActiveTextBox != m_controls[i].Name)
                        {
                            if (this[GuiManager.ActiveTextBox] != null)
                                this[GuiManager.ActiveTextBox].HasFocus = false;
                        }
                        GuiManager.ActiveTextBox = m_controls[i].Name;
                    }

                    // for controls in a window, mouse may have just been released from another 
                    // control's focus, so we need to make sure we reset that control's state
                    if ((m_controls[i].ControlState == Enums.EControlState.Over) && !string.IsNullOrEmpty(m_controls[i].Owner) && !(m_controls[i] is Window))
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

                    // Confirm only one RadioButton of a group is selected.
                    if ((m_controls[i] is RadioButton) && (m_controls[i] as RadioButton).NeedToDeselectOthers)
                    {
                        (m_controls[i] as RadioButton).NeedToDeselectOthers = false;
                        for (int j = 0; j < m_controls.Count; j++)
                        {
                            if (i == j)
                                continue;

                            if ((m_controls[j] is RadioButton) &&
                                ((m_controls[j] as RadioButton).GroupID == (m_controls[i] as RadioButton).GroupID))
                            {
                                (m_controls[j] as RadioButton).Deselect();
                            }
                        }
                    }

                    // May have moved from a back control to a front control; reset over states.
                    for (int j = 0; j < i; j++)
                    {
                        if (m_controls[j].ControlState == Enums.EControlState.Over)
                        {
                            m_controls[j].ControlState = Enums.EControlState.Normal;
                            break;
                        }
                    }

                    // Mouse is down over another control so close any open combobox
                    if (m_controls[i].ControlState == Enums.EControlState.Down)
                    {
                        if ((GuiManager.OpenComboBox != "") && (GuiManager.OpenComboBox != m_controls[i].Name))
                        {
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

                        // Release TextBox Focus.
                        if (!string.IsNullOrEmpty(GuiManager.ActiveTextBox) && (GuiManager.ActiveTextBox != m_controls[i].Name))
                        {
                            // release textbox focus
                            if (this[GuiManager.ActiveTextBox] != null)
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
                            else if (!string.IsNullOrEmpty(m_controls[i].Owner))
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
                        else m_controls[i].ZDepthDateTime = DateTime.Now;
                        #endregion

                    }
                    break;
                }
                else if (ms.LeftButton == ButtonState.Pressed)
                {
                    // clicked off a control so close any open combobox
                    if (GuiManager.OpenComboBox != null && !string.IsNullOrEmpty(GuiManager.OpenComboBox))
                    {
                        ComboBox openComboBox = this[GuiManager.OpenComboBox] as ComboBox;
                        Point cursor = new Point(ms.X, ms.Y);

                        if (!openComboBox.Contains(cursor))
                        {
                            openComboBox.IsOpen = false;
                            GuiManager.OpenComboBox = "";
                        }
                    }

                    if (!string.IsNullOrEmpty(GuiManager.ActiveTextBox) && (GuiManager.ActiveTextBox != m_controls[i].Name))
                    {
                        // release textbox focus
                        if (this[GuiManager.ActiveTextBox] != null)
                            this[GuiManager.ActiveTextBox].HasFocus = false;

                        GuiManager.ActiveTextBox = "";
                    }
                }
            }
        }

        public void AddControl(Control c)
        {
            // set control sheet owner
            c.Sheet = Name;

            // control does not have an owner (Window), find highest z depth
            if (c.Owner == "")
            {
                if(c is PopUpWindow)
                {
                    if (GuiManager.PopUpWindow != null) GuiManager.PopUpWindow.OnClose();
                    GuiManager.PopUpWindow = c as PopUpWindow;
                }

                if(m_controls.Exists(ct => ct.Name == c.Name))
                {
                    if(!c.Name.EndsWith("SpellEffectLabel") && !c.Name.StartsWith("-")) // SpellEffectLabels come in rapidly sometimes. They use GameTime as a Name, however still doesn't give them a unique name.
                        Utils.Log("Attempted to add same Control [" + c.Name + "] to Sheet [" + Name + "].");

                    return;
                }

                // Saved in GUIPositions xml
                if(c is Window && Character.CurrentCharacter != null && Character.GUIPositionSettings != null && Character.GUIPositionSettings.GUIPositionsContains(c, out int index))
                {
                    c.Position = new Point(Character.GUIPositionSettings.GUIPositions[index].Coordinates.X, Character.GUIPositionSettings.GUIPositions[index].Coordinates.Y);
                    //TODO width and height
                }

                if (m_controls.Count > 0)
                    c.ZDepth = m_controls[0].ZDepth + 1;
                else c.ZDepth = 1;

                if (c.TabOrder >= 0)
                    m_currentTabOrder = 0;

                lock (m_controls)
                {
                    m_controls.Add(c);
                }

                SortControls();
            }
            else
            {
                Control owner = this[c.Owner];

                if (c is DropDownMenu)
                    c.ZDepth = 1;

                if (owner != null)
                {
                    if (owner is Window)
                    {
                        AttachControlToWindow(c);
                    }
                    else if (owner is ScrollableTextBox)
                    {
                        if (c is DropDownMenu)
                            (owner as ScrollableTextBox).DropDownMenu = c as DropDownMenu;
                        else if (c is Border)
                            (owner as ScrollableTextBox).Border = c as Border;
                    }
                    else if (owner is TextBox)
                    {
                        if (c is Border)
                            (owner as TextBox).Border = c as Border;
                        else if (c is DropDownMenu)
                            (owner as TextBox).DropDownMenu = c as DropDownMenu;
                    }
                    else if (owner is PercentageBarLabel)
                    {
                        if(c is Border)
                        {
                            if (!c.Name.Contains("Mid") && !c.Name.Contains("Fore") && (owner as PercentageBarLabel).Border == null)
                                (owner as PercentageBarLabel).Border = c as Border;
                            else
                            {
                                if (c.Name.Contains("Mid"))
                                    (owner as PercentageBarLabel).MidBorder = c as Border;
                                else if (c.Name.Contains("Fore"))
                                    (owner as PercentageBarLabel).ForeBorder = c as Border;
                            }
                        }

                        if (c is Label)
                        {
                            if(c.Name.EndsWith("MidLabel"))
                                (owner as PercentageBarLabel).MidLabel = c as Label;
                            else if(c.Name.EndsWith("ForeLabel"))
                                (owner as PercentageBarLabel).ForeLabel = c as Label;
                        }
                    }
                    else if (owner is Label)
                    {
                        if (c is Border)
                            (owner as Label).Border = c as Border;

                        else if (owner is CritterListLabel)
                        {
                            if (c is DropDownMenu)
                                (owner as CritterListLabel).DropDownMenu = c as DropDownMenu;
                        }
                        else if(owner is PercentageBarLabel)
                            (owner as PercentageBarLabel).MidLabel = c as Label;
                    }
                    else if (owner is DropDownMenu)
                    {
                        if (c is Border)
                            (owner as DropDownMenu).Border = c as Border;
                    }
                    else if (owner is DragAndDropButton)
                    {
                        if (c is Border)
                        {
                            (owner as DragAndDropButton).Border = c as Border;
                            (owner as DragAndDropButton).OriginalBorderColor = c.TintColor;
                            (owner as DragAndDropButton).OriginalBorderWidth = c.Width;
                            (owner as DragAndDropButton).HasOriginalBorder = true;
                        }

                        if (c is DropDownMenu)
                            (owner as DragAndDropButton).DropDownMenu = c as DropDownMenu;
                    }
                    else if (owner is Button)
                    {
                        if (c is Border)
                            (owner as Button).Border = c as Border;
                    }
                    else if (owner is CheckboxButton)
                    {
                        if (c is Border)
                            (owner as CheckboxButton).Border = c as Border;
                    }
                    else if (owner is ColorDialogButton)
                    {
                        if (c is Border)
                            (owner as ColorDialogButton).Border = c as Border;
                    }
                }
                //else Utils.Log("Owner is null. " + c.Name + ", Owner: " + c.Owner);
            }
        }

        public void RemoveControl(Control c)
        {
            if (!m_controls.Contains(c))
                return;

            lock (m_controls)
            {
                m_controls.Remove(c);
            }
        }

        public void AttachControlToWindow(Control c, Window w)
        {
            if ((w as Window).Controls.Exists(ct => ct.Name == c.Name))
            {
                Utils.Log("Attempted to add same Control [" + c.Name + "] to Window [" + w.Name + "] in Sheet [" + Name + "].");
                return;
            }

            if (!(w is Window))
            {
                Utils.Log("Exception: Tried to attach Control to a non-Window Control.");
                throw new Exception("Exception: Tried to attach Control to non-Window Control.");
            }

            c.Position = new Point(w.Position.X + c.Position.X, w.Position.Y + c.Position.Y);

            if ((w as Window).Controls.Count > 0)
                c.ZDepth = (w as Window).Controls[0].ZDepth + 1;
            else
                c.ZDepth = 1;

            if (c.TabOrder >= 0)
                (w as Window).CurrentTabOrder = 0;

            // add the control to the Window's list of controls
            lock ((w as Window).Controls)
            {
                (w as Window).Controls.Add(c);
            }

            // sort the controls
            (w as Window).SortControls();

            // set this window's title or border for easy reference later
            if (c is WindowTitle && c.Name == w.Name + "Title")
            {
                c.Width = w.Width;
                (w as Window).WindowTitle = c as WindowTitle;
                if (w is AutoHidingWindow)
                    (w as AutoHidingWindow).SetWindowOrientation();
            }
            else if (c is Border && c.Name == w.Name + "Border")
            {
                (w as Window).WindowBorder = c as Border;
            }
            else if (c is TabControl)
            {
                if ((w as Window).TabControl == null)
                {
                    (w as Window).TabControl = c as TabControl;
                }
                else
                {
                    Utils.Log("Attempted to add a second TabControl to Window [" + c.Name + "].");
                }
            }
            else if (c is TabControlButton)
            {
                if ((w as Window).TabControl != null)
                {
                    (w as Window).TabControl.Add(c as TabControlButton);
                }
                else
                {
                    TabControl tabControl = new TabControl(w.Name + "TabControl", w.Name);
                    AddControl(tabControl);
                    tabControl.Add(c as TabControlButton);
                }
            }
        }

        public void AttachControlToWindow(Control c)
        {
            Control w = this[c.Owner];

            if ((w as Window).Controls.Exists(ct => ct.Name == c.Name))
            {
                Utils.Log("Attempted to add same Control [" + c.Name + "] to Window [" + w.Name + "] in Sheet [" + Name + "].");
                return;
            }

            if (!(w is Window))
            {
                Utils.Log("Exception: Tried to attach Control to a non-Window Control.");
                throw new Exception("Exception: Tried to attach Control to non-Window Control.");
            }

            c.Position = new Point(w.Position.X + c.Position.X, w.Position.Y + c.Position.Y);

            if ((w as Window).Controls.Count > 0)
                c.ZDepth = (w as Window).Controls[0].ZDepth + 1;
            else
                c.ZDepth = 1;

            if (c.TabOrder >= 0)
                (w as Window).CurrentTabOrder = 0;

            // add the control to the Window's list of controls
            lock ((w as Window).Controls)
            {
                (w as Window).Controls.Add(c);
            }

            // sort the controls
            (w as Window).SortControls();

            // set this window's title or border for easy reference later
            if (c is WindowTitle && c.Name == w.Name + "Title")
            {
                c.Width = w.Width;
                (w as Window).WindowTitle = c as WindowTitle;
                if (w is AutoHidingWindow)
                    (w as AutoHidingWindow).SetWindowOrientation();
            }
            else if (c is Border && c.Name == w.Name + "Border")
            {
                (w as Window).WindowBorder = c as Border;
            }
            else if (c is TabControl)
            {
                if ((w as Window).TabControl == null)
                {
                    (w as Window).TabControl = c as TabControl;
                }
                else
                {
                    Utils.Log("Attempted to add a second TabControl to Window [" + c.Name + "].");
                }
            }
            else if (c is TabControlButton)
            {
                if ((w as Window).TabControl != null)
                {
                    (w as Window).TabControl.Add(c as TabControlButton);
                }
                else
                {
                    TabControl tabControl = new TabControl(w.Name + "TabControl", w.Name);
                    AddControl(tabControl);
                    tabControl.Add(c as TabControlButton);
                }
            }
        }

        public void SortControls()
        {
            ControlSorter sorter = new ControlSorter();
            m_controls.Sort(sorter);
        }

        public void OnClientResize(Rectangle prev, Rectangle now)
        {
            foreach (Control control in new List<Control>(m_controls))
                control.OnClientResize(prev, now, false);

            //foreach (TextCue textCue in new List<TextCue>(TextCues))
            //    textCue.OnClientResize(prev, now);
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

                    if (currentFocus != null && currentFocus.TabOrder == tabOrdered.Count - 1)
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
            foreach (Control c in new List<Control>(Controls))
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
        public void CreateAutoHidingWindow(string name, string type, string owner, Rectangle rectangle, bool visible, bool locked, bool disabled,
            string font, VisualKey visualKey, Color tintColor, byte visualAlpha, bool dropShadow, Map.Direction shadowDirection, int shadowDistance,
            List<Enums.EAnchorType> anchors, string cursorOverride, Enums.EAnchorType windowTitleOrientation, int autoHideVisualAlpha,
            bool fadeIn, bool fadeOut, int fadeSpeed)
        {
            AddControl(new AutoHidingWindow(name, owner, rectangle, visible, locked, disabled, font, visualKey, tintColor,
                    visualAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride, windowTitleOrientation,
                    autoHideVisualAlpha, fadeIn, fadeOut, fadeSpeed));
        }
        public void CreateWindow(string name, string type, string owner, Rectangle rectangle, bool visible, bool locked, bool disabled,
            string font, VisualKey visualKey, Color tintColor, byte visualAlpha, bool dropShadow, Map.Direction shadowDirection, int shadowDistance,
            List<Enums.EAnchorType> anchors, string cursorOverride)
        {
            if (type == "Window")
                AddControl(new Window(name, owner, rectangle, visible, locked, disabled, font, visualKey, tintColor,
                    visualAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride));
            else if (type == "HotButtonEditWindow")
            {
                AddControl(new HotButtonEditWindow(name, owner, rectangle, visible, locked, disabled, font, visualKey, tintColor,
                    visualAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride));
            }
            else if(type == "CritterListWindow")
            {
                AddControl(new CritterListWindow(name, owner, rectangle, visible, locked, disabled, font, visualKey, tintColor,
                    visualAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride));
            }
        }

        public void CreateWindowTitle(string name, string owner, string font, string text, Color textColor, Color tintColor, byte visualAlpha,
            BitmapFont.TextAlignment textAlignment, VisualKey visualKey, bool visualTiled, VisualKey closeBoxVisualKey,
            VisualKey maxBoxVisualKey, VisualKey minBoxVisualKey, VisualKey cropBoxVisualKey, VisualKey closeBoxVisualKeyDown,
            VisualKey maxBoxVisualKeyDown, VisualKey minBoxVisualKeyDown, VisualKey cropBoxVisualKeyDown,
            int closeBoxDistanceFromRight, int closeBoxDistanceFromTop, int closeBoxWidth, int closeBoxHeight, int closeBoxVisualAlpha,
            int maxBoxDistanceFromRight, int maxBoxDistanceFromTop, int maxBoxWidth, int maxBoxHeight, int maxBoxVisualAlpha,
            int minBoxDistanceFromRight, int minBoxDistanceFromTop, int minBoxWidth, int minBoxHeight, int minBoxVisualAlpha,
            int cropBoxDistanceFromRight, int cropBoxDistanceFromTop, int cropBoxWidth, int cropBoxHeight, int cropBoxVisualAlpha,
            Color closeBoxTintColor, Color maximizeBoxTintColor, Color minimizeBoxTintColor, Color cropBoxTintColor, int height)
        {
            AddControl(new WindowTitle(name, owner, font, text, textColor, tintColor, visualAlpha,
                textAlignment, visualKey, visualTiled, closeBoxVisualKey,
                maxBoxVisualKey, minBoxVisualKey, cropBoxVisualKey, closeBoxVisualKeyDown, maxBoxVisualKeyDown, minBoxVisualKeyDown,
                cropBoxVisualKeyDown, closeBoxDistanceFromRight, closeBoxDistanceFromTop, closeBoxWidth, closeBoxHeight, closeBoxVisualAlpha,
                maxBoxDistanceFromRight, maxBoxDistanceFromTop, maxBoxWidth, maxBoxHeight, maxBoxVisualAlpha,
                minBoxDistanceFromRight, minBoxDistanceFromTop, minBoxWidth, minBoxHeight, minBoxVisualAlpha,
                cropBoxDistanceFromRight, cropBoxDistanceFromTop, cropBoxWidth, cropBoxHeight, cropBoxVisualAlpha,
                closeBoxTintColor, maximizeBoxTintColor, minimizeBoxTintColor, cropBoxTintColor, height));
        }

        public void CreateSquareBorder(string name, string owner, int width, VisualKey visualKey, bool visualTiled, Color tintColor, byte visualAlpha)
        {
            AddControl(new SquareBorder(name, owner, width, visualKey, visualTiled, tintColor, visualAlpha));
        }

        public void CreateNumericTextBox(string name, string owner, Rectangle rectangle, string text, Color textColor,
            BitmapFont.TextAlignment textAlignment, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha,
            bool editable, int maxLength, bool passwordBox, bool blinkingCursor, Color cursorColor,
            VisualKey visualKeyOver, VisualKey visualKeyDown, VisualKey visualKeyDisabled, int xTextOffset,
            int yTextOffset, string onKeyboardEnter, Color selectionColor, List<Enums.EAnchorType> anchors, int tabOrder, int maxValue, int minValue)
        {
            AddControl(new NumericTextBox(name, owner, rectangle, text, textColor, textAlignment, visible, disabled, font, visualKey, tintColor, visualAlpha,
                textAlpha, editable, maxLength, passwordBox, blinkingCursor, cursorColor, visualKeyOver, visualKeyDown, visualKeyDisabled,
                xTextOffset, yTextOffset, onKeyboardEnter, selectionColor, anchors, tabOrder, maxValue, minValue));
        }

        public void CreateTextBox(string name, string owner, Rectangle rectangle, string text, Color textColor, BitmapFont.TextAlignment textAlignment, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha, bool editable, int maxLength, bool passwordBox, bool blinkingCursor, Color cursorColor,
            VisualKey visualKeyOver, VisualKey visualKeyDown, VisualKey visualKeyDisabled, int xTextOffset,
            int yTextOffset, string onKeyboardEnter, Color selectionColor, List<Enums.EAnchorType> anchors, int tabOrder)
        {
            AddControl(new TextBox(name, owner, rectangle, text, textColor, textAlignment, visible, disabled, font, visualKey, tintColor, visualAlpha,
                textAlpha, editable, maxLength, passwordBox, blinkingCursor, cursorColor, visualKeyOver,
                visualKeyDown, visualKeyDisabled, xTextOffset, yTextOffset, onKeyboardEnter, selectionColor, anchors, tabOrder));
        }

        public void CreateCheckBoxButton(string name, string owner, Rectangle rectangle, bool visible, bool disabled, VisualKey visualKey, Color tintColor, byte visualAlpha,
            VisualKey visualKeyOver, VisualKey visualKeyDown, VisualKey visualKeyDisabled, VisualKey visualKeySelected, Color visualKeySelectedColor, Color tintOverColor, bool hasTintOverColor,
            List<Enums.EAnchorType> anchors, bool dropShadow, Map.Direction shadowDirection, int shadowDistance, string popUpText)
        {
            AddControl(new CheckboxButton(name, owner, rectangle, visible, disabled, this.Font, visualKey, tintColor, visualAlpha, visualKeyOver, visualKeyDown,
                    visualKeyDisabled, visualKeySelected, visualKeySelectedColor, tintOverColor, hasTintOverColor, anchors, dropShadow, shadowDirection, shadowDistance, popUpText));
        }

        public void CreateDragAndDropButton(string type, string name, string owner, Rectangle rectangle, string text, bool textVisible, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha,
            VisualKey visualKeyOver, VisualKey visualKeyDown, VisualKey visualKeyDisabled, string clickEvent,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, Color textOverColor, bool hasTextOverColor, Color tintOverColor, bool hasTintOverColor,
            List<Enums.EAnchorType> anchors, bool dropShadow, Map.Direction shadowDirection, int shadowDistance,
            string command, string popUpText, string tabControlledWindow, string cursorOverride, bool locked, bool acceptingDroppedButtons)
        {
            AddControl(new DragAndDropButton(name, owner, rectangle, text, textVisible, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha,
                    textAlpha, visualKeyOver, visualKeyDown, visualKeyDisabled, clickEvent, textAlignment, xTextOffset, yTextOffset, textOverColor,
                    hasTextOverColor, tintOverColor, hasTintOverColor, anchors, dropShadow, shadowDirection, shadowDistance, popUpText, locked, acceptingDroppedButtons));
        }

        public void CreateButton(string type, string name, string owner, Rectangle rectangle, string text, bool textVisible, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha,
            VisualKey visualKeyOver, VisualKey visualKeyDown, VisualKey visualKeyDisabled, string clickEvent,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, Color textOverColor, bool hasTextOverColor, Color tintOverColor, bool hasTintOverColor,
            List<Enums.EAnchorType> anchors, bool dropShadow, Map.Direction shadowDirection, int shadowDistance,
            string command, string popUpText, string tabControlledWindow, string cursorOverride, bool locked, string clickSound)
        {
            if (type == "HotButton")
            {
                AddControl(new HotButton(name, owner, rectangle, text, textVisible, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha,
                   textAlpha, visualKeyOver, visualKeyDown, visualKeyDisabled, clickEvent, textAlignment, xTextOffset, yTextOffset, textOverColor,
                   hasTextOverColor, tintOverColor, hasTintOverColor, anchors, dropShadow, shadowDirection, shadowDistance, command, popUpText));
            }
            else if (type == "IconImageSelectionButton")
            {
                AddControl(new IconImageSelectionButton(name, owner, rectangle, text, textVisible, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha,
                   textAlpha, visualKeyOver, visualKeyDown, visualKeyDisabled, clickEvent, textAlignment, xTextOffset, yTextOffset, textOverColor,
                   hasTextOverColor, tintOverColor, hasTintOverColor, anchors, dropShadow, shadowDirection,
                   shadowDistance, command));
            }
            else if (type == "MacroButton")
            {
                AddControl(new MacroButton(name, owner, rectangle, text, textVisible, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha,
                   textAlpha, visualKeyOver, visualKeyDown, visualKeyDisabled, clickEvent, textAlignment, xTextOffset, yTextOffset, textOverColor,
                   hasTextOverColor, tintOverColor, hasTintOverColor, anchors, dropShadow, shadowDirection, shadowDistance, command));
            }
            else if(type == "TabControlButton")
            {
                AddControl(new TabControlButton(name, owner, rectangle, text, textVisible, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha,
                   textAlpha, visualKeyOver, visualKeyDown, visualKeyDisabled, textAlignment, xTextOffset, yTextOffset, textOverColor, hasTextOverColor,
                   tintOverColor, hasTintOverColor, anchors, dropShadow, shadowDirection, shadowDistance, tabControlledWindow));
            }
            else if(type == "ColorDialogButton")
            {
                AddControl(new ColorDialogButton(name, owner, rectangle, visible, disabled, this.Font, tintColor, cursorOverride, anchors,
                    dropShadow, shadowDirection, shadowDistance, popUpText));
            }
            else
            {
                AddControl(new Button(name, owner, rectangle, text, textVisible, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha,
                    textAlpha, visualKeyOver, visualKeyDown, visualKeyDisabled, clickEvent, textAlignment,
                    xTextOffset, yTextOffset, textOverColor, hasTextOverColor, tintOverColor, hasTintOverColor, anchors, dropShadow, shadowDirection,
                    shadowDistance, command, popUpText, clickSound));
            }
        }

        public void CreateDropDownMenu(string name, string owner, string title, Rectangle rectangle, bool visible, string font, VisualKey visualKey,
            Color tintColor, int visualAlpha, bool dropShadow, Map.Direction shadowDirection, int shadowDistance)
        {
            AddControl(new DropDownMenu(name, owner, title, rectangle, visible, font, visualKey, tintColor, visualAlpha, dropShadow, shadowDirection, shadowDistance));
        }

        public void CreateLabel(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string onDoubleClickEvent,
            string cursorOverride, List<Enums.EAnchorType> anchors, string popUpText)
        {
            AddControl(new Label(name, owner, rectangle, text, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha,
                textAlpha, textAlignment, xTextOffset, yTextOffset, onDoubleClickEvent, cursorOverride,
                anchors, popUpText));
        }

        public void CreatePercentageBarLabel(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string onDoubleClickEvent,
            string cursorOverride, List<Enums.EAnchorType> anchors, string popUpText, bool segmented)
        {
            AddControl(new PercentageBarLabel(name, owner, rectangle, text, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha,
                textAlpha, textAlignment, xTextOffset, yTextOffset, onDoubleClickEvent, cursorOverride,
                anchors, popUpText, segmented));
        }

        public void CreateCritterListLabel(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string onDoubleClickEvent,
            string cursorOverride, List<Enums.EAnchorType> anchors, string popUpText)
        {
            AddControl(new CritterListLabel(name, owner, rectangle, text, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha,
               textAlpha, textAlignment, xTextOffset, yTextOffset, onDoubleClickEvent, cursorOverride,
               anchors, popUpText));

            SquareBorder border = new SquareBorder(name + "SquareBorder", name, Client.ClientSettings.TargetBorderSize, new VisualKey("WhiteSpace"), false, Client.ClientSettings.TargetBorderColor, visualAlpha);
            border.IsVisible = false;
            AddControl(border);

            //DropDownMenu menu = new DropDownMenu(name + "DropDownMenu", name, "", new Rectangle(rectangle.X, rectangle.Y, 100, 100), false,
            //            this.Font, new VisualKey("WhiteSpace"), Client.ClientSettings.ColorDropDownMenu, visualAlpha, true, Map.Direction.Northwest, 5);
            //AddControl(menu);
        }

        public void CreateIOKTileLabel(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string onDoubleClickEvent,
            string cursorOverride, List<Enums.EAnchorType> anchors, string popUpText)
        {
            AddControl(new IOKTileLabel(name, owner, rectangle, text, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha,
                textAlpha, textAlignment, xTextOffset, yTextOffset, onDoubleClickEvent, cursorOverride, anchors, popUpText));
        }

        public void CreateSpinelTileLabel(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string onDoubleClickEvent,
            string cursorOverride, List<Enums.EAnchorType> anchors, string popUpText)
        {
            AddControl(new SpinelTileLabel(name, owner, rectangle, text, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha,
                textAlpha, textAlignment, xTextOffset, yTextOffset, onDoubleClickEvent, cursorOverride,
                anchors, popUpText));
        }

        public void CreateScrollableTextBox(string name, string owner, Rectangle rectangle, string text, Color textColor,
            bool visible, bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha,
            VisualKey visualKeyOver, VisualKey visualKeyDown, VisualKey visualKeyDisabled, int xTextOffset, int yTextOffset,
            BitmapFont.TextAlignment textAlignment, List<Enums.EAnchorType> anchors, bool trim)
        {
            AddControl(new ScrollableTextBox(name, owner, rectangle, text, textColor, visible, disabled, font, visualKey,
                tintColor, visualAlpha, textAlpha, visualKeyOver, visualKeyDown, visualKeyDisabled, xTextOffset,
                yTextOffset, textAlignment, anchors, trim));
        }

        public void CreateStatusBar(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string onDoubleClickEvent,
            string cursorOverride, List<Enums.EAnchorType> anchors, Enums.ELayoutType layoutType)
        {
            AddControl(new StatusBar(name, owner, rectangle, text, textColor, visible, disabled, font, visualKey, tintColor, visualAlpha,
                textAlpha, textAlignment, xTextOffset, yTextOffset, onDoubleClickEvent, cursorOverride,
                anchors, layoutType));
        }
        #endregion
    }
}