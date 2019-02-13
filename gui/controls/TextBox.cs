using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Yuusha.gui
{
    public class TextBox : Control
    {
        private static System.Collections.Generic.List<Keys> DoNothingKeys = new System.Collections.Generic.List<Keys>()
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

        #region Private Data
        protected int m_cursorPosition;
        protected int m_maxLength;
        protected bool m_editable;
        protected bool m_passwordBox;
        protected bool m_cursorVisible;
        protected bool m_blinkingCursor;
        protected Color m_cursorColor;
        protected Color m_selectionColor;
        private Keys[] pressedKeys;
        private TimeSpan m_previousBlink;
        private string m_onKeyboardEnter;
        private int m_selectionStart;
        private int m_selectionLength;
        private Border m_border;
        #endregion

        public bool IsCursorVisible
        {
            get { return m_cursorVisible; }
            set { m_cursorVisible = value; }
        }

        public Border Border
        {
            get { return m_border; }
            set { m_border = value; }
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
        }

        public TextBox(string name, string owner, Rectangle rectangle, string text, Color textColor,
            bool visible, bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha,
            byte borderAlpha, byte textAlpha, bool editable, int maxLength, bool passwordBox,
            bool blinkingCursor, Color cursorColor, VisualKey visualKeyOver, VisualKey visualKeyDown,
            VisualKey visualKeyDisabled, int xTextOffset, int yTextOffset, string onKeyboardEnter,
            Color selectionColor, System.Collections.Generic.List<Enums.EAnchorType> anchors)
            : base()
        {
            m_name = name;
            m_owner = owner;
            m_rectangle = rectangle;
            m_text = text;
            m_cursorPosition = m_text.Length;
            m_textColor = textColor;
            m_visible = visible;
            m_disabled = disabled;
            m_font = font;
            m_visualKey = visualKey;
            m_tintColor = tintColor;
            m_visualAlpha = visualAlpha;
            m_borderAlpha = borderAlpha;
            m_textAlpha = textAlpha;
            m_editable = editable;
            m_maxLength = maxLength;
            m_passwordBox = passwordBox;
            m_blinkingCursor = blinkingCursor;
            m_cursorColor = cursorColor;
            m_xTextOffset = xTextOffset;
            m_yTextOffset = yTextOffset;
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
        }
        #endregion

        protected override bool OnKeyDown(KeyboardState ks)
        {
            if (!Client.HasFocus || !HasFocus || !m_editable)
                return false;

            // If typing in Options, don't echo the text to game or conference input text boxes.
            if ((this.Name == Globals.GAMEINPUTTEXTBOX || this.Name == Globals.CONFINPUTTEXTBOX)
                && GuiManager.GenericSheet["OptionsWindow"] != null && GuiManager.GenericSheet["OptionsWindow"].HasFocus)
                return false;

            bool controlPressed = (ks.IsKeyDown(Keys.LeftControl) || ks.IsKeyDown(Keys.RightControl));
            bool shiftPressed = (ks.IsKeyDown(Keys.LeftShift) || ks.IsKeyDown(Keys.RightShift));
            bool altPressed = (ks.IsKeyDown(Keys.LeftAlt) || ks.IsKeyDown(Keys.RightAlt));
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
                            {
                                    Events.RegisterEvent(Events.EventName.Target_Cleared, null);
                            }
                            
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
                                    if (this.Name == Globals.GAMEINPUTTEXTBOX && numLock) // overrides for numpad keys in a game input text box
                                    {
                                        #region NumLock and Number Pad Keys
                                        if (numLock)
                                        {
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
                                        } 
                                        #endregion
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
                                        else if (ks.IsKeyDown(Keys.C) && !this.m_passwordBox)
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
            #region Determine Cursor Visibility
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

            if (!HasFocus)
                m_cursorVisible = false;
            #endregion

            base.Update(gameTime);

            if (m_border != null) m_border.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!m_visible)
                return;

            base.Draw(gameTime);

            Color textColor = m_textColor;

            if (m_disabled)
            {
                textColor = new Color(Control.s_disabledColor.R, Control.s_disabledColor.G, Control.s_disabledColor.B, m_textAlpha);
            }

            if (m_border != null) m_border.Draw(gameTime);

            // difference between lineheight and this height / 2

            int rectX = m_rectangle.X + m_xTextOffset; // used for selection rectangle, cursor and text
            int rectY = m_rectangle.Y + m_yTextOffset; // used for selection rectangle, cursor and text

            if (m_border != null && m_border is SquareBorder)
            {
                rectX += (m_border as SquareBorder).BorderWidth;
                rectY += (m_border as SquareBorder).BorderWidth;
            }

            // draw selection color if text is selected
            if (HasFocus && m_selectionLength != 0)
            {
                BitmapFont bmf = BitmapFont.ActiveFonts[Font];
                Rectangle selRect = new Rectangle(rectX + bmf.MeasureString(m_text.Substring(0, m_selectionStart)), rectY,
                    bmf.MeasureString(m_text.Substring(m_selectionStart, m_selectionLength)), bmf.LineHeight);
                VisualInfo vi = GuiManager.Visuals["WhiteSpace"];
                Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], selRect, vi.Rectangle, m_selectionColor);

            }

            // override BitmapFont sprite batch
            BitmapFont.ActiveFonts[Font].SpriteBatchOverride(Client.SpriteBatch);
            
            // draw text if not password box, password characters if otherwise
            if (!m_passwordBox)
            {
                BitmapFont.ActiveFonts[Font].DrawString(rectX, rectY, textColor, m_text);
            }
            else
            {
                string password = "";

                for (int i = 0; i < m_text.Length; i++)
                    password += GuiManager.PASSWORDCHAR;

                BitmapFont.ActiveFonts[Font].DrawString(rectX, rectY, textColor, password);
            }

            // draw the cursor if cursor is visible, control has focus and control is not disabled
            if (m_cursorVisible && HasFocus && !m_disabled)
            {
                int cursorWidth = BitmapFont.ActiveFonts[Font].MeasureString("_");
                int cursorHeight = BitmapFont.ActiveFonts[Font].LineHeight;

                VisualInfo cursorVisual = GuiManager.Visuals["WhiteSpace"];
                
                if (!m_passwordBox)
                {
                    Rectangle cursorRectangle = new Rectangle(rectX + BitmapFont.ActiveFonts[Font].MeasureString(m_text.Substring(0, m_cursorPosition)), rectY + 1, cursorWidth, cursorHeight);
                    Client.SpriteBatch.Draw(GuiManager.Textures[cursorVisual.ParentTexture], cursorRectangle, cursorVisual.Rectangle, m_cursorColor);
                }
                else
                {
                    string password = "";

                    for (int i = 0; i < m_text.Length; i++)
                        password += GuiManager.PASSWORDCHAR;

                    Rectangle cursorRectangle = new Rectangle(rectX + BitmapFont.ActiveFonts[Font].MeasureString(password.Substring(0, m_cursorPosition)), rectY + 1, cursorWidth, cursorHeight);
                    Client.SpriteBatch.Draw(GuiManager.Textures[cursorVisual.ParentTexture], cursorRectangle, cursorVisual.Rectangle, m_cursorColor);
                }
            }
        }

        protected override void OnMouseDown(MouseState ms)
        {
            if (!Client.HasFocus) return;

            m_cursorVisible = true;

            base.OnMouseDown(ms);
        }

        protected override void OnDoubleLeftClick()
        {
            if (!Client.HasFocus) return;

            SelectAll();
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

        private void ReplaceSelectedText(string replacementText)
        {
            m_text = m_text.Remove(m_selectionStart, m_selectionLength);
            m_text = m_text.Insert(m_selectionStart, replacementText);
            SelectAll();
        }

        private void InsertClipboardText()
        {
            string clipboardText = Utils.GetClipboardText();
            m_text = m_text.Insert(m_cursorPosition, clipboardText);
            m_cursorPosition = m_cursorPosition + clipboardText.Length;
        }
    }
}
