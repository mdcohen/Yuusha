using System;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class TextBox : Control
    {
        private static readonly System.Collections.Generic.List<Keys> DoNothingKeys = new System.Collections.Generic.List<Keys>()
        {
            Keys.LeftShift,
            Keys.RightShift,
            Keys.CapsLock,
            Keys.RightControl,
            Keys.LeftControl,
            Keys.RightAlt,
            Keys.LeftAlt,
            Keys.NumLock,
            Keys.VolumeDown,
            Keys.VolumeMute,
            Keys.VolumeUp,
            Keys.MediaNextTrack,
            Keys.MediaPlayPause,
            Keys.MediaPreviousTrack,
            Keys.MediaStop,
            Keys.SelectMedia,
            Keys.LeftWindows,
            Keys.RightWindows,
        };

        private static readonly System.Collections.Generic.List<Keys> RepeatableKeys = new System.Collections.Generic.List<Keys>()
        {
            Keys.Back, Keys.Delete
        };

        private static readonly System.Collections.Generic.List<Keys> FunctionKeys = new System.Collections.Generic.List<Keys>()
        {
            Keys.F1 ,Keys.F2, Keys.F3, Keys.F4, Keys.F5, Keys.F6, Keys.F7, Keys.F8, Keys.F9, Keys.F10, Keys.F11, Keys.F12
        };

        #region Private Data
        protected int m_cursorPosition;
        protected int m_maxLength;
        protected bool m_editable;
        protected bool m_passwordBox;
        protected bool m_cursorVisible;
        protected bool m_blinkingCursor;
        protected Color m_cursorColor;
        protected Color m_selectionColor;
        protected Keys[] pressedKeys;
        protected TimeSpan m_previousBlink;
        protected string m_onKeyboardEnter;
        protected int m_selectionStart;
        protected int m_selectionLength;

        protected Timer m_repeatingKeyTimer;
        protected bool m_allowRepeatingKey;
        protected Keys m_lastKeyPressed;
        protected int m_repeatedKeyCount;
        #endregion

        public bool IsCursorVisible
        {
            get { return m_cursorVisible; }
            set { m_cursorVisible = value; }
        }

        public DropDownMenu DropDownMenu
        { get; set; }

        public Border Border { get; set; }

        public int SelectionStart
        {
            get { return m_selectionStart; }
        }

        public int SelectionLength
        {
            get { return m_selectionLength; }
        }

        #region Constructors (2)
        public TextBox()
            : base()
        {
            m_cursorPosition = 0;
            m_maxLength = 20;
            m_editable = true;
            m_passwordBox = false;
            m_cursorVisible = false;
            m_blinkingCursor = false;
            m_cursorColor = Color.White;
            m_selectionColor = Color.Blue;
            m_previousBlink = new TimeSpan();
            m_onKeyboardEnter = "";
            m_selectionLength = 0;
            m_selectionStart = 0;

            m_repeatingKeyTimer = new System.Timers.Timer(500);
            m_repeatingKeyTimer.Elapsed += new System.Timers.ElapsedEventHandler(RepeatingKeyTimer_Elapsed);
            m_repeatingKeyTimer.AutoReset = true;
            m_allowRepeatingKey = true;
            m_repeatedKeyCount = 0;
        }

        public TextBox(string name, string owner, Rectangle rectangle, string text, Color textColor, BitmapFont.TextAlignment textAlignment,
            bool visible, bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha,
            byte textAlpha, bool editable, int maxLength, bool passwordBox,
            bool blinkingCursor, Color cursorColor, VisualKey visualKeyOver, VisualKey visualKeyDown,
            VisualKey visualKeyDisabled, int xTextOffset, int yTextOffset, string onKeyboardEnter,
            Color selectionColor, System.Collections.Generic.List<Enums.EAnchorType> anchors, int tabOrder)
            : base()
        {
            m_name = name;
            m_owner = owner;
            m_rectangle = rectangle;
            m_text = text;
            m_cursorPosition = m_text.Length;
            m_textColor = textColor;
            TextAlignment = textAlignment;
            m_visible = visible;
            m_disabled = disabled;
            m_font = font;
            m_visualKey = visualKey;
            m_tintColor = tintColor;
            m_visualAlpha = visualAlpha;
            m_textAlpha = textAlpha;
            m_editable = editable;
            m_maxLength = maxLength;
            m_passwordBox = passwordBox;
            m_blinkingCursor = blinkingCursor;
            m_cursorColor = cursorColor;
            XTextOffset = xTextOffset;
            YTextOffset = yTextOffset;
            m_onKeyboardEnter = onKeyboardEnter;
            m_selectionColor = selectionColor;
            m_anchors = anchors;

            if (m_visualKey.Key != "")
                m_visuals.Add(Enums.EControlState.Normal, m_visualKey);
            if (visualKeyOver.Key != "")
                m_visuals.Add(Enums.EControlState.Over, visualKeyOver);
            if (visualKeyDown.Key != "")
                m_visuals.Add(Enums.EControlState.Down, visualKeyDown);
            if (visualKeyDisabled.Key != "")
                m_visuals.Add(Enums.EControlState.Disabled, visualKeyDisabled);

            m_previousBlink = new TimeSpan();
            m_selectionStart = 0;
            m_selectionLength = 0;
            m_tabOrder = tabOrder;

            m_repeatingKeyTimer = new System.Timers.Timer(800);
            m_repeatingKeyTimer.Elapsed += new System.Timers.ElapsedEventHandler(RepeatingKeyTimer_Elapsed);
            m_repeatingKeyTimer.AutoReset = true;

            m_allowRepeatingKey = true;
            m_repeatedKeyCount = 0;
        }
        #endregion

        protected override bool OnKeyDown(KeyboardState ks)
        {
            if (!Client.HasFocus || !HasFocus || !m_editable)
                return false;

            // If typing in Options, don't echo the text to game or conference input text boxes.
            if ((Name == Globals.GAMEINPUTTEXTBOX || Name == Globals.CONFINPUTTEXTBOX)
                && GuiManager.GenericSheet["OptionsWindow"] != null && GuiManager.GenericSheet["OptionsWindow"].HasFocus)
                return false;

            bool controlPressed = ks.IsKeyDown(Keys.LeftControl) || ks.IsKeyDown(Keys.RightControl);
            bool shiftPressed = ks.IsKeyDown(Keys.LeftShift) || ks.IsKeyDown(Keys.RightShift);
            bool altPressed = ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt);
            bool capsLock = (((ushort)Yuusha.KeyboardHandler.GetKeyState(0x14)) & 0xffff) != 0;
            bool numLock = (((ushort)Yuusha.KeyboardHandler.GetKeyState(0x90)) & 0xffff) != 0;

            Keys[] newKeys = ks.GetPressedKeys();

            if (pressedKeys != null)
            {
                foreach (Keys k in newKeys)
                {
                    bool bFound = false;

                    foreach (Keys k2 in pressedKeys)
                    {
                        if (k == k2)
                        {
                            bFound = true;
                            //m_lastKeyPressed = k2;
                            //if (m_lastKeyPressed == k2 && m_allowRepeatingKey && RepeatableKeys.Contains(k2))
                            //{
                            //    bFound = false;
                            //    m_allowRepeatingKey = false;
                            //    m_repeatedKeyCount++;
                            //    if (m_repeatedKeyCount >= 2)
                            //        m_repeatingKeyTimer.Interval = 200;
                            //    m_repeatingKeyTimer.Start();
                            //}
                            //else
                            //{
                            //    m_repeatingKeyTimer.Interval = 500;
                            //    m_allowRepeatingKey = true;
                            //    m_repeatedKeyCount = 0;
                            //}
                            break;
                        }
                    }

                    if (!bFound)
                    {
                        if (DoNothingKeys.Contains(k))
                        {
                            // do nothing
                        }
                        #region Non-Displayable Keys
                        else if (k == Keys.Back)
                        {
                            if (m_selectionLength != 0)
                            {
                                m_text = m_text.Remove(m_selectionStart, m_selectionLength);
                                m_cursorPosition = m_selectionStart;
                                DeselectText();
                            }
                            else if (m_cursorPosition > 0)
                            {
                                m_cursorPosition--;
                                m_text = m_text.Remove(m_cursorPosition, 1);
                            }
                            else if (m_cursorPosition < m_text.Length) // Backspace on cursor position 0 will delete first character
                            {
                                m_text = m_text.Remove(m_cursorPosition, 1);
                            }
                        }
                        else if (k == Keys.Delete)
                        {
                            if (m_selectionLength != 0)
                            {
                                m_text = m_text.Remove(m_selectionStart, m_selectionLength);
                                m_cursorPosition = m_selectionStart;
                                DeselectText();
                            }
                            else if (m_cursorPosition < m_text.Length)
                            {
                                m_text = m_text.Remove(m_cursorPosition, 1);
                            }
                        }
                        else if (k == Keys.Left)
                        {
                            DeselectText();

                            if (m_cursorPosition <= 0)
                            {
                                pressedKeys = newKeys;
                                return false;
                            }
                            if (controlPressed)
                            {
                                if (m_text[m_cursorPosition - 1] == ' ')
                                {
                                    while (m_cursorPosition != 0 && m_text[m_cursorPosition - 1] == ' ')
                                    {
                                        m_cursorPosition--;
                                    }
                                }
                                else
                                {
                                    while (m_cursorPosition != 0 && m_text[m_cursorPosition - 1] != ' ')
                                    {
                                        m_cursorPosition--;
                                    }
                                }
                            }
                            else
                            {
                                m_cursorPosition--;
                            }
                        }
                        else if (k == Keys.Right)
                        {
                            DeselectText();

                            if (m_cursorPosition >= m_text.Length)
                            {
                                pressedKeys = newKeys;
                                return false;
                            }
                            if (controlPressed)
                            {
                                if (m_text[m_cursorPosition] == ' ')
                                {
                                    while (m_cursorPosition < m_text.Length && m_text[m_cursorPosition] == ' ')
                                    {
                                        m_cursorPosition++;
                                    }
                                }
                                else
                                {
                                    while (m_cursorPosition < m_text.Length && m_text[m_cursorPosition] != ' ')
                                    {
                                        m_cursorPosition++;
                                    }
                                }
                            }
                            else
                            {
                                m_cursorPosition++;
                            }
                            pressedKeys = newKeys;
                            return true;
                        }
                        else if (k == Keys.Up || k == Keys.Down)
                        {
                            if (shiftPressed)
                            {
                                #region Shift is pressed.
                                switch (Client.GameState)
                                {
                                    case Enums.EGameState.YuushaGame:
                                        if (k == Keys.Up)
                                        {
                                            if (YuushaMode.BufferPreview > 0)
                                            {
                                                YuushaMode.BufferPreview--;
                                                m_text = YuushaMode.BufferedCommands[YuushaMode.BufferPreview];
                                                SelectAll();
                                            }
                                        }
                                        else if (k == Keys.Down)
                                        {
                                            if (YuushaMode.BufferPreview < YuushaMode.BufferedCommands.Count - 1)
                                            {
                                                YuushaMode.BufferPreview++;
                                                m_text = YuushaMode.BufferedCommands[YuushaMode.BufferPreview];
                                                SelectAll();
                                            }
                                        }
                                        break;
                                    case Enums.EGameState.SpinelGame:
                                        if (k == Keys.Up)
                                        {
                                            if (SpinelMode.BufferPreview > 0)
                                            {
                                                SpinelMode.BufferPreview--;
                                                m_text = SpinelMode.BufferedCommands[SpinelMode.BufferPreview];
                                                SelectAll();
                                            }
                                        }
                                        else if (k == Keys.Down)
                                        {
                                            if (SpinelMode.BufferPreview < SpinelMode.BufferedCommands.Count - 1)
                                            {
                                                SpinelMode.BufferPreview++;
                                                m_text = SpinelMode.BufferedCommands[SpinelMode.BufferPreview];
                                                SelectAll();
                                            }
                                        }
                                        break;
                                    case Enums.EGameState.IOKGame:
                                        if (k == Keys.Up)
                                        {
                                            if (IOKMode.BufferPreview > 0)
                                            {
                                                IOKMode.BufferPreview--;
                                                m_text = IOKMode.BufferedCommands[IOKMode.BufferPreview];
                                                SelectAll();
                                            }
                                        }
                                        else if (k == Keys.Down)
                                        {
                                            if (IOKMode.BufferPreview < IOKMode.BufferedCommands.Count - 1)
                                            {
                                                IOKMode.BufferPreview++;
                                                m_text = IOKMode.BufferedCommands[IOKMode.BufferPreview];
                                                SelectAll();
                                            }
                                        }
                                        break;
                                } 
                                #endregion
                            }
                        }
                        else if (k == Keys.End)
                        {
                            DeselectText();
                            m_cursorPosition = m_text.Length;
                        }
                        else if (k == Keys.Home)
                        {
                            DeselectText();
                            m_cursorPosition = 0;
                        }
                        else if (k == Keys.Escape)
                        {
                            if (m_selectionLength != 0)
                            {
                                DeselectText();
                            }
                            else
                            {
                                m_text = "";
                                m_cursorPosition = 0;
                            }

                            if (Client.GameState.ToString().EndsWith("Game"))
                                Events.RegisterEvent(Events.EventName.Target_Cleared, null);

                            if(GuiManager.Cursors[GuiManager.GenericSheet.Cursor].DraggedControl is DragAndDropButton dadButton)
                                dadButton.StopDragging();
                            
                        }
                        else if (k == Keys.Tab)
                        {
                            pressedKeys = newKeys;
                            return false;
                        }
                        else if (k == Keys.PageDown)
                        {
                        }
                        else if (k == Keys.PageUp)
                        {
                        }
                        else if (k == Keys.Insert)
                        {
                        }
                        else if (k == Keys.PrintScreen)
                        { }
                        else if (k == Keys.Enter)
                        {
                            if (m_onKeyboardEnter != "")
                            {
                                Events.EventName eventName = (Events.EventName)Enum.Parse(typeof(Events.EventName), m_onKeyboardEnter, true);
                                Events.RegisterEvent(eventName, this);
                                // clear the text
                                if (eventName == Events.EventName.Send_Text)
                                {
                                    SelectAll();

                                    if (Client.GameState == Enums.EGameState.IOKGame)
                                        IOKMode.AddBufferedCommand(m_text);
                                    else if (Client.GameState == Enums.EGameState.SpinelGame)
                                        SpinelMode.AddBufferedCommand(m_text);
                                    else if (Client.GameState == Enums.EGameState.YuushaGame)
                                        YuushaMode.AddBufferedCommand(m_text);
                                }
                                pressedKeys = newKeys;
                                return true;
                            }
                            pressedKeys = newKeys;
                            return false;
                        }
                        else if (!Char.IsLetterOrDigit(k.ToString(), 0) && !Char.IsPunctuation(k.ToString(), 0))
                        {
                            // do nothing
                            Utils.Log("Do nothing for this key: " + k.ToString());
                        }
                        #endregion
                        else
                        {
                            if (!string.IsNullOrEmpty(GuiManager.ActiveDropDownMenu))
                                return false;

                            // eliminate F (function) keys
                            if (k.ToString().StartsWith("F") && k.ToString().Length > 1)
                                return false;

                            if (k.ToString().ToLower().StartsWith("oem"))
                            {
                                #region OEM Keys
                                switch (k)
                                {
                                    case Keys.OemBackslash:
                                        if (!shiftPressed)
                                            AddText("\\");
                                        else AddText("|");
                                        break;
                                    case Keys.OemCloseBrackets:
                                        if (!shiftPressed)
                                            AddText("]");
                                        //else AddText("}"); this causes crash in BitmapFont class
                                        break;
                                    case Keys.OemComma:
                                        if (!shiftPressed)
                                            AddText(",");
                                        else AddText("<");
                                        break;
                                    case Keys.OemMinus:
                                        if (!shiftPressed)
                                            AddText("-");
                                        else AddText("_");
                                        break;
                                    case Keys.OemOpenBrackets:
                                        if (!shiftPressed)
                                            AddText("[");
                                        //else AddText("{");
                                        break;
                                    case Keys.OemPlus:
                                        if (!shiftPressed)
                                            AddText("=");
                                        else AddText("+");
                                        break;
                                    case Keys.OemQuestion:
                                        if (!shiftPressed)
                                            AddText("/");
                                        else AddText("?");
                                        break;
                                    case Keys.OemQuotes:
                                        if (!shiftPressed)
                                            AddText("'");
                                        else AddText("\"");
                                        break;
                                    case Keys.OemSemicolon:
                                        if (!shiftPressed)
                                            AddText(";");
                                        else AddText(":");
                                        break;
                                    case Keys.OemTilde:
                                        if (!shiftPressed)
                                            AddText("`");
                                        else AddText("~");
                                        break;
                                    case Keys.OemPeriod:
                                        if (!shiftPressed)
                                            AddText(".");
                                        else AddText(">");
                                        break;
                                    case Keys.OemPipe:
                                        if (!shiftPressed)
                                            AddText("|");
                                        else AddText("\\");
                                        break;
                                }
                                #endregion
                            }
                            else
                            {
                                string textToAdd = ((char)k).ToString();

                                if (!shiftPressed && !controlPressed && !altPressed)
                                {
                                    if (Name == Globals.GAMEINPUTTEXTBOX && numLock) // overrides for numpad keys in a game input text box
                                    {
                                        #region NumLock and Number Pad Keys
                                        switch (k)
                                        {
                                            case Keys.NumPad1:
                                                textToAdd = Character.Settings.NumLock1 + " ";
                                                break;
                                            case Keys.NumPad2:
                                                textToAdd = Character.Settings.NumLock2 + " ";
                                                break;
                                            case Keys.NumPad3:
                                                textToAdd = Character.Settings.NumLock3 + " ";
                                                break;
                                            case Keys.NumPad4:
                                                textToAdd = Character.Settings.NumLock4 + " ";
                                                break;
                                            case Keys.NumPad5:
                                                textToAdd = Character.Settings.NumLock5 + " ";
                                                break;
                                            case Keys.NumPad6:
                                                textToAdd = Character.Settings.NumLock6 + " ";
                                                break;
                                            case Keys.NumPad7:
                                                textToAdd = Character.Settings.NumLock7 + " ";
                                                break;
                                            case Keys.NumPad8:
                                                textToAdd = Character.Settings.NumLock8 + " ";
                                                break;
                                            case Keys.NumPad9:
                                                textToAdd = Character.Settings.NumLock9 + " ";
                                                break;
                                            case Keys.Divide:
                                                textToAdd = Character.Settings.NumPadDivide + " ";
                                                break;
                                            case Keys.Multiply:
                                                textToAdd = Character.Settings.NumPadMultiply + " ";
                                                break;
                                            case Keys.Subtract:
                                                textToAdd = Character.Settings.NumPadSubtract + " ";
                                                break;
                                            case Keys.Add:
                                                textToAdd = Character.Settings.NumPadAdd + " ";
                                                break;
                                            case Keys.Delete:
                                                textToAdd = Character.Settings.NumPadDelete + " ";
                                                break;
                                        }
                                        #endregion
                                    }
                                    else if(Name == Globals.GAMEINPUTTEXTBOX && FunctionKeys.Contains(k))
                                    {
                                        switch(k)
                                        {
                                            case Keys.F1:
                                                textToAdd = Character.Settings.FunctionKey1;
                                                break;
                                            case Keys.F2:
                                                textToAdd = Character.Settings.FunctionKey2;
                                                break;
                                            case Keys.F3:
                                                textToAdd = Character.Settings.FunctionKey3;
                                                break;
                                            case Keys.F4:
                                                textToAdd = Character.Settings.FunctionKey4;
                                                break;
                                            case Keys.F5:
                                                textToAdd = Character.Settings.FunctionKey5;
                                                break;
                                            case Keys.F6:
                                                textToAdd = Character.Settings.FunctionKey6;
                                                break;
                                            case Keys.F7:
                                                textToAdd = Character.Settings.FunctionKey7;
                                                break;
                                            case Keys.F8:
                                                textToAdd = Character.Settings.FunctionKey8;
                                                break;
                                            case Keys.F9:
                                                textToAdd = Character.Settings.FunctionKey9;
                                                break;
                                            case Keys.F10:
                                                textToAdd = Character.Settings.FunctionKey10;
                                                break;
                                            case Keys.F11:
                                                textToAdd = Character.Settings.FunctionKey11;
                                                break;
                                            case Keys.F12:
                                                textToAdd = Character.Settings.FunctionKey12;
                                                break;
                                        }
                                    }
                                    else if (k.ToString().StartsWith("NumPad")) // text is numbers
                                    {
                                        textToAdd = k.ToString().Substring(k.ToString().Length - 1, 1);
                                    }
                                    else
                                    {
                                        switch (k)
                                        {
                                            case Keys.Multiply:
                                                textToAdd = "*";
                                                break;
                                            case Keys.Divide:
                                                textToAdd = "/";
                                                break;
                                            case Keys.Subtract:
                                                textToAdd = "-";
                                                break;
                                            case Keys.Decimal:
                                                textToAdd = ".";
                                                break;
                                        }
                                    }

                                    if (textToAdd.Length > 0)
                                    {
                                        if (capsLock)
                                            textToAdd = textToAdd.ToUpper();
                                        else textToAdd = textToAdd.ToLower();

                                        AddText(textToAdd);
                                    }
                                }
                                else
                                {
                                    if (shiftPressed)
                                    {
                                        #region Shift key is pressed.
                                        switch (k)
                                        {
                                            case Keys.D1:
                                                textToAdd = "!";
                                                break;
                                            case Keys.D2:
                                                textToAdd = "@";
                                                break;
                                            case Keys.D3:
                                                textToAdd = "#";
                                                break;
                                            case Keys.D4:
                                                textToAdd = "$";
                                                break;
                                            case Keys.D5:
                                                textToAdd = "%";
                                                break;
                                            case Keys.D6:
                                                textToAdd = "^";
                                                break;
                                            case Keys.D7:
                                                textToAdd = "&";
                                                break;
                                            case Keys.D8:
                                                textToAdd = "*";
                                                break;
                                            case Keys.D9:
                                                textToAdd = "(";
                                                break;
                                            case Keys.D0:
                                                textToAdd = ")";
                                                break;
                                            default:
                                                textToAdd = textToAdd.ToUpper();
                                                break;
                                        }
                                        #endregion
                                    }

                                    if (controlPressed)
                                    {
                                        #region Control key is pressed.
                                        if (ks.IsKeyDown(Keys.A))
                                        {
                                            SelectAll(); // use ESC to clear selected text
                                            pressedKeys = newKeys;
                                            return true;
                                        }
                                        else if (ks.IsKeyDown(Keys.C) && !m_passwordBox)
                                        {
                                            if (m_text.Length > 0 && m_selectionLength > 0)
                                            {
                                                Utils.SetClipboardText(m_text.Substring(m_selectionStart, m_selectionLength));
                                            }
                                            pressedKeys = newKeys;
                                            return true;
                                        }
                                        else if (ks.IsKeyDown(Keys.V))
                                        {
                                            if (m_selectionLength > 0)
                                            {
                                                ReplaceSelectedText(Utils.GetClipboardText());
                                            }
                                            else InsertClipboardText();
                                            pressedKeys = newKeys;
                                            return true;
                                        } 
                                        #endregion
                                    }

                                    if (textToAdd.Length > 0)
                                    {
                                        if (capsLock)
                                            textToAdd = (shiftPressed ? textToAdd.ToLower() : textToAdd.ToUpper());

                                        AddText(textToAdd);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            pressedKeys = newKeys;
            return true;
        }

        private void RepeatingKeyTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            m_allowRepeatingKey = true;
            m_repeatingKeyTimer.Stop();
        }

        public virtual void AddText(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                // letter or digit, punctuation, white space, symbol
                if (char.IsLetterOrDigit(text, i) || char.IsPunctuation(text, i) ||
                    char.IsWhiteSpace(text, i) || char.IsSymbol(text, i))
                {
                    if (m_text.Length < m_maxLength || m_selectionLength == m_maxLength)
                    {
                        if (m_selectionLength != 0)
                        {
                            m_text = m_text.Remove(m_selectionStart, m_selectionLength);
                            m_text = m_text.Insert(m_selectionStart, text.Substring(i, 1));
                            m_cursorPosition = m_selectionStart + 1;
                            m_selectionStart = 0;
                            m_selectionLength = 0;
                        }
                        else
                        {
                            if (m_cursorPosition == m_text.Length)
                            {
                                m_text += text.Substring(i, 1);
                            }
                            else
                            {
                                m_text = m_text.Insert(m_cursorPosition, text.Substring(i, 1));
                            }
                            m_cursorPosition += 1;
                        }
                    }
                }
            }
        }

        public void SelectAll()
        {
            m_selectionStart = 0;
            m_selectionLength = m_text.Length;
            m_cursorPosition = m_text.Length;
        }

        public void DeselectText()
        {
            m_selectionStart = 0;
            m_selectionLength = 0;
        }

        public override void Update(GameTime gameTime)
        {
            if (GuiManager.ControlWithFocus != this) HasFocus = false;
            if (GuiManager.ActiveTextBox == Name) HasFocus = true;

            if (!IsVisible || IsDisabled)
                DropDownMenu = null;

            #region Cursor
            if (m_blinkingCursor)
            {
                if (m_previousBlink == new TimeSpan() || gameTime.TotalGameTime - m_previousBlink >= TimeSpan.FromSeconds(.75))
                {
                    if (m_previousBlink == new TimeSpan())
                        m_cursorVisible = true;
                    else
                        m_cursorVisible = !m_cursorVisible;

                    m_previousBlink = gameTime.TotalGameTime;
                }
            }
            else m_cursorVisible = true;

            if (m_cursorPosition > (m_maxLength - 1)) m_cursorPosition = m_maxLength - 1;

            //if (Sheet == GuiManager.CurrentSheet.Name)
            //{
            //    if (Owner != "" && GuiManager.GetControl(Owner).IsVisible)
            //        TextCue.AddClientInfoTextCue(Name + " cursor position: " + m_cursorPosition.ToString());
            //    else if (Owner == "")
            //        TextCue.AddClientInfoTextCue(Name + " cursor position: " + m_cursorPosition.ToString());
            //}

            if (!HasFocus)
                m_cursorVisible = false;
            #endregion

            base.Update(gameTime);

            if (Border != null) Border.Update(gameTime);

            if (DropDownMenu != null) DropDownMenu.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!m_visible)
                return;

            base.Draw(gameTime);

            Color textColor = m_textColor;

            if (m_disabled)
            {
                textColor = new Color(Control.ColorDisabledStandard.R, Control.ColorDisabledStandard.G, Control.ColorDisabledStandard.B, m_textAlpha);
            }

            if (Border != null) Border.Draw(gameTime);

            // difference between lineheight and this height / 2

            int rectX = m_rectangle.X + XTextOffset; // used for selection rectangle, cursor and text
            int rectY = m_rectangle.Y + YTextOffset; // used for selection rectangle, cursor and text

            if (Border != null && Border is SquareBorder)
            {
                rectX += (Border as SquareBorder).BorderWidth;
                rectY += (Border as SquareBorder).BorderWidth;
            }

            // draw selection color if text is selected
            if (HasFocus && m_selectionLength != 0)
            {
                BitmapFont bmf = BitmapFont.ActiveFonts[Font];
                Rectangle selRect = new Rectangle(rectX + bmf.MeasureString(m_text.Substring(0, m_selectionStart)), rectY,
                    bmf.MeasureString(m_text.Substring(m_selectionStart, m_selectionLength)), bmf.LineHeight);

                //if (m_textAlignment == BitmapFont.TextAlignment.Right)
                //    selRect.X += Width - (BitmapFont.ActiveFonts[Font].MeasureString(m_text.Substring(0, m_cursorPosition)) * m_text.Length);
                //else if (m_textAlignment == BitmapFont.TextAlignment.Center)
                //    selRect.X = (Width / 2) - (BitmapFont.ActiveFonts[Font].MeasureString(m_text.Substring(0, m_cursorPosition)) * (m_text.Length / 2));

                //if (Border != null)
                //    selRect.Y += Border.Width;

                VisualInfo vi = GuiManager.Visuals["WhiteSpace"];
                Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], selRect, vi.Rectangle, m_selectionColor);

            }

            // override BitmapFont sprite batch
            BitmapFont.ActiveFonts[Font].SpriteBatchOverride(Client.SpriteBatch);

            // set font alignment
            BitmapFont.ActiveFonts[Font].Alignment = TextAlignment;

            // code below needs work 6/23/2019
            string viewableText = m_text;

            if (BitmapFont.ActiveFonts[Font].MeasureString(viewableText) > Width)
            {
                int count = 1;
                while (BitmapFont.ActiveFonts[Font].MeasureString(viewableText) > Width)
                {
                    viewableText = viewableText.Substring(count);
                    count++;
                }
            }

            // draw text if not password box, password characters if otherwise
            if (!m_passwordBox)
            {
                if (Border != null && Border is SquareBorder && TextAlignment == BitmapFont.TextAlignment.Right)
                    rectY -= (Border as SquareBorder).BorderWidth;

                BitmapFont.ActiveFonts[Font].TextBox(new Rectangle(rectX, rectY, m_rectangle.Width - 1, m_rectangle.Height - 1), textColor, viewableText);
            }
            else
            {
                string password = "";

                for (int i = 0; i < m_text.Length; i++)
                    password += GuiManager.PASSWORDCHAR;

                if (Border != null && Border is SquareBorder && TextAlignment == BitmapFont.TextAlignment.Right)
                    rectY -= (Border as SquareBorder).BorderWidth;

                BitmapFont.ActiveFonts[Font].TextBox(new Rectangle(rectX, rectY, m_rectangle.Width - 1, m_rectangle.Height - 1), textColor, password);
            }

            // draw the cursor if cursor is visible, control has focus and control is not disabled -- also if Client has focus/is activated
            if (m_cursorVisible && HasFocus && !m_disabled && Client.HasFocus)
            {
                int cursorWidth = BitmapFont.ActiveFonts[Font].MeasureString("_");
                int cursorHeight = BitmapFont.ActiveFonts[Font].LineHeight;

                VisualInfo cursorVisual = GuiManager.Visuals["WhiteSpace"];

                // automatically reduce alpha of cursor
                if (m_cursorPosition < Text.Length)
                    m_cursorColor = new Color(m_cursorColor, 130);
                else m_cursorColor = new Color(m_cursorColor, 255);

                //if (m_textAlignment == BitmapFont.TextAlignment.Right)
                //    rectX += BitmapFont.ActiveFonts[Font].MeasureString(m_text.Substring(0, m_cursorPosition)) * m_text.Length;
                //else if (m_textAlignment == BitmapFont.TextAlignment.Center)
                //    rectX = (Width / 2) - (BitmapFont.ActiveFonts[Font].MeasureString(m_text.Substring(0, m_cursorPosition)) / 2);

                if (!m_passwordBox)
                {
                    Rectangle cursorRectangle = new Rectangle();
                    // ocassional argument out of range exception when selecting a text box with saved information and text is empty
                    try
                    {
                        cursorRectangle = new Rectangle(rectX + BitmapFont.ActiveFonts[Font].MeasureString(m_text.Substring(0, m_cursorPosition)), rectY, cursorWidth, cursorHeight);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        SelectAll();
                        cursorRectangle = new Rectangle(rectX + BitmapFont.ActiveFonts[Font].MeasureString(m_text.Substring(0, m_cursorPosition)), rectY, cursorWidth, cursorHeight);
                    }
                    Client.SpriteBatch.Draw(GuiManager.Textures[cursorVisual.ParentTexture], cursorRectangle, cursorVisual.Rectangle, m_cursorColor);
                }
                else
                {
                    string password = "";

                    for (int i = 0; i < m_text.Length; i++)
                        password += GuiManager.PASSWORDCHAR;

                    Rectangle cursorRectangle = new Rectangle(rectX + BitmapFont.ActiveFonts[Font].MeasureString(password.Substring(0, m_cursorPosition)), rectY, cursorWidth, cursorHeight);
                    Client.SpriteBatch.Draw(GuiManager.Textures[cursorVisual.ParentTexture], cursorRectangle, cursorVisual.Rectangle, m_cursorColor);
                }
            }

            if (DropDownMenu != null)
                DropDownMenu.Draw(gameTime);
        }

        protected override void OnMouseDown(MouseState ms)
        {
            if (!Client.HasFocus) return;
            if (IsDisabled) return;

            m_hasFocus = true;
            
            m_cursorVisible = true;

            if(ms.RightButton == ButtonState.Pressed)
            {
                if (DropDownMenu == null && (Text.Length > 0 || Utils.GetClipboardText().Length > 0))
                {
                    try
                    {
                        Rectangle dropDownRectangle = new Rectangle(ms.X - 10, ms.Y - 10, 100, 100); // default height for 5 drop down menu items

                        // readjust Y if out of client width bounds
                        //if (dropDownRectangle.Y + dropDownRectangle.Width > Client.Width)
                        //    dropDownRectangle.Y = Client.Width - dropDownRectangle.Width - 5;

                        if (Sheet != "Generic")
                        {
                            GuiManager.Sheets[Sheet].CreateDropDownMenu(Name + "DropDownMenu", Name, "", dropDownRectangle, true,
                                Client.ClientSettings.DefaultDropDownMenuFont, new VisualKey("WhiteSpace"), Client.ClientSettings.ColorDropDownMenu, 255, true, Map.Direction.Northwest, 5);
                        }
                        else
                        {
                            GuiManager.GenericSheet.CreateDropDownMenu(Name + "DropDownMenu", Name, "", dropDownRectangle, true,
                                Client.ClientSettings.DefaultDropDownMenuFont, new VisualKey("WhiteSpace"), Client.ClientSettings.ColorDropDownMenu, 255, true, Map.Direction.Northwest, 5);
                        }

                        DropDownMenu.Border = new SquareBorder(DropDownMenu.Name + "Border", DropDownMenu.Name, Client.ClientSettings.DropDownMenuBorderWidth, new VisualKey("WhiteSpace"), false, Client.ClientSettings.ColorDropDownMenuBorder, 255)
                        {
                            IsVisible = true,
                        };

                        DropDownMenu.HasFocus = true;
                        int height = 0;
                        if (Text.Length > 0 && !m_passwordBox)
                        {
                            if (m_editable)
                            {
                                height += 20;
                                DropDownMenu.AddDropDownMenuItem("cut", Name + "DropDownMenu", new VisualKey("WhiteSpace"), "TextBox_DropDown", "", false);
                            }

                            height += 20;
                            DropDownMenu.AddDropDownMenuItem("copy", Name + "DropDownMenu", new VisualKey("WhiteSpace"), "TextBox_DropDown", "", false);
                        }
                        if (Utils.GetClipboardText().Length > 0 && m_editable)
                        {
                            height += 20;
                            DropDownMenu.AddDropDownMenuItem("paste", Name + "DropDownMenu", new VisualKey("WhiteSpace"), "TextBox_DropDown", "", Utils.GetClipboardText().Length > 0 ? false : true);
                        }
                        if (this.Text.Length > 0 && m_editable)
                        {
                            height += 20;
                            DropDownMenu.AddDropDownMenuItem("delete", Name + "DropDownMenu", new VisualKey("WhiteSpace"), "TextBox_DropDown", "", false);
                        }

                        DropDownMenu.Height = height;
                    }
                    catch(Exception e)
                    {
                        Utils.LogException(e);
                    }
                }
            }

            base.OnMouseDown(ms);
        }

        protected override void OnDoubleLeftClick()
        {
            if (!Client.HasFocus) return;

            if (m_selectionLength > 0)
                DeselectText();
            else SelectAll();
        }

        public void Clear()
        {
            m_text = "";
            m_cursorPosition = 0;
            m_selectionLength = 0;
            m_selectionStart = 0;
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                if (value.Length > m_maxLength)
                    value = value.Substring(0, m_maxLength);

                base.Text = value;
            }
        }

        public void ReplaceSelectedText(string replacementText)
        {
            if (replacementText.Length > m_maxLength)
                TextCue.AddClientInfoTextCue("Pasted text longer than textbox maximum text length.");

            m_text = m_text.Remove(m_selectionStart, m_selectionLength);
            m_text = m_text.Insert(m_selectionStart, replacementText);
            SelectAll();
        }

        public void InsertClipboardText()
        {
            string clipboardText = Utils.GetClipboardText();
            m_text = m_text.Insert(m_cursorPosition, clipboardText);
            m_cursorPosition = m_cursorPosition + clipboardText.Length;
        }

        public override bool MouseHandler(MouseState ms)
        {
            if (DropDownMenu != null)
                DropDownMenu.MouseHandler(ms);

            return base.MouseHandler(ms);
        }

        public override void OnClientResize(Rectangle prev, Rectangle now, bool ownerOverride)
        {
            if(DropDownMenu != null)
                GuiManager.Dispose(DropDownMenu);

            base.OnClientResize(prev, now, ownerOverride);
        }
    }
}
