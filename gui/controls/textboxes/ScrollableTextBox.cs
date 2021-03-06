using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class ScrollableTextBox : Control
    {
        #region Private Data

        private List<string> m_allLines; // all lines received by this scrollable text box
        private List<string> m_formattedLines; // formatted lines
        private List<string> m_visibleLines; // lines that are visible in the viewport

        private List<Enums.ETextType> m_allTextTypes;
        private List<Enums.ETextType> m_formattedTextTypes;
        private List<Enums.ETextType> m_visibleTextTypes;

        private List<Color> m_allTextColors;
        private List<Color> m_visibleTextColors;
        private List<Color> m_formattedTextColors;
       
        private Scrollbar m_scrollbar;
        private readonly int m_maxLineBuffer;
        private int m_prevScrollWheelValue;
        private readonly bool m_trim;

        public bool Colorize
        { get; set; } = true;

        #endregion

        #region Public Properties
        public int PreviousScrollWheelValue
        {
            get { return m_prevScrollWheelValue; }
            set { m_prevScrollWheelValue = value; }
        }
        public Scrollbar Scrollbar
        {
            get { return m_scrollbar; }
            set { m_scrollbar = value; }
        }
        private int ViewportLines
        {
            get { return (int)((m_rectangle.Height - XTextOffset) / BitmapFont.ActiveFonts[Font].LineHeight); }
        }
        public int LinesCount
        {
            get { return m_allLines.Count; }
        }
        public int LongestFormattedLine()
        {
            int length = 0;

            foreach(string line in m_formattedLines)
                if (BitmapFont.ActiveFonts[Font].MeasureString(line) > length) length = BitmapFont.ActiveFonts[Font].MeasureString(line);

            return length;
        }
        public int FormattedLinesCount
        { get { return m_formattedLines.Count; } }
        public DropDownMenu DropDownMenu
        { get; set; }
        public Border Border
        { get; set; }
        public override string Font
        {
            get => base.Font;
            set
            {
                if (value != base.Font)
                {
                    base.Font = value;
                    OnFontChange();
                }
                else base.Font = value;
            }
        }
        #endregion

        #region Constructors (2)
        public ScrollableTextBox()
            : base()
        {
            m_allLines = new List<string>();
            m_formattedLines = new List<string>();
            m_visibleLines = new List<string>();
            m_allTextTypes = new List<Enums.ETextType>();
            m_allTextColors = new List<Color>();
            m_formattedTextTypes = new List<Enums.ETextType>();
            m_visibleTextTypes = new List<Enums.ETextType>();
            m_visibleTextColors = new List<Color>();
            m_formattedTextColors = new List<Color>();
            TextAlignment = BitmapFont.TextAlignment.Left;
            m_maxLineBuffer = 100;
            m_prevScrollWheelValue = 0;
            m_trim = false;
        }

        public ScrollableTextBox(string name, string owner, Rectangle rectangle, string text, Color textColor,
            bool visible, bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha,
            byte textAlpha, VisualKey visualKeyOver, VisualKey visualKeyDown,
            VisualKey visualKeyDisabled, int xTextOffset, int yTextOffset, BitmapFont.TextAlignment textAlignment,
            List<Enums.EAnchorType> anchors, bool trim)
            : base()
        {
            m_name = name;
            m_owner = owner;
            m_rectangle = rectangle;
            m_text = text;
            m_textColor = textColor;
            m_visible = visible;
            m_disabled = disabled;
            m_font = font;
            m_visualKey = visualKey;
            m_visuals.Add(Enums.EControlState.Normal, m_visualKey);
            m_tintColor = tintColor;
            m_visualAlpha = visualAlpha;
            m_textAlpha = textAlpha;
            XTextOffset = xTextOffset;
            YTextOffset = yTextOffset;
            TextAlignment = textAlignment;
            m_anchors = anchors;
            m_trim = trim;

            if (visualKeyOver.Key != "")
                m_visuals.Add(Enums.EControlState.Over, visualKeyOver);
            if (visualKeyDown.Key != "")
                m_visuals.Add(Enums.EControlState.Down, visualKeyDown);
            if (visualKeyDisabled.Key != "")
                m_visuals.Add(Enums.EControlState.Disabled, visualKeyDisabled);

            m_allLines = new List<string>();
            m_formattedLines = new List<string>();
            m_visibleLines = new List<string>();

            m_allTextTypes = new List<Enums.ETextType>();
            m_formattedTextTypes = new List<Enums.ETextType>();
            m_visibleTextTypes = new List<Enums.ETextType>();

            m_allTextColors = new List<Color>();
            m_formattedTextColors = new List<Color>();
            m_visibleTextColors = new List<Color>();

            m_maxLineBuffer = 200; // TODO: temporary?
            m_formattedLines.Capacity = m_maxLineBuffer;

            // TODO: temporary - scrollbar info should be set in xml
            m_scrollbar = new Scrollbar(m_name + "Scrollbar", 0, 1, 1, m_formattedLines.Count, 0, true);

            m_prevScrollWheelValue = 0;
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            if (m_scrollbar != null)
            {
                // enable or disable the scrollbar
                if (BitmapFont.ActiveFonts.ContainsKey(Font))
                {
                    if (m_formattedLines.Count * BitmapFont.ActiveFonts[Font].LineHeight > m_rectangle.Height - XTextOffset)
                        m_scrollbar.IsDisabled = false;
                    else m_scrollbar.IsDisabled = true;
                }
                else
                {
                    Utils.LogOnce("BitmapFont.ActiveFonts does not contain the Font [ " + Font + " ] for ScrollableTextBox [ " + m_name + " ] of Sheet [ " + GuiManager.CurrentSheet.Name + " ]");
                    m_scrollbar.IsDisabled = true;
                }

                // clear visible lines
                m_visibleLines.Clear();
                m_visibleTextTypes.Clear();
                m_visibleTextColors.Clear();

                // update visible lines capacity
                m_visibleLines.Capacity = ViewportLines;
                m_visibleTextTypes.Capacity = ViewportLines;
                m_visibleTextColors.Capacity = ViewportLines;

                m_scrollbar.MaxValue = m_formattedLines.Count - ViewportLines;
                m_scrollbar.MinValue = 0;

                // update scrollbar
                m_scrollbar.Update(gameTime);

                if (m_formattedLines.Count > 0)
                {
                    if (!m_scrollbar.IsDisabled)
                    {
                        for (int a = m_formattedLines.Count - (ViewportLines) - m_scrollbar.ScrollValue, b = 1; b <= ViewportLines;
                            a++, b++)
                        {
                            try
                            {
                                m_visibleLines.Add(m_formattedLines[a]);
                                m_visibleTextTypes.Add(m_formattedTextTypes[a]);
                                m_visibleTextColors.Add(m_formattedTextColors[a]);
                            }
                            catch { continue; }
                        }
                    }
                    else
                    {

                        for (int a = 0; a < m_formattedLines.Count; a++)
                            try
                            {
                                m_visibleLines.Add(m_formattedLines[a]);
                            }
                            catch { continue; }
                        for (int a = 0; a < m_formattedTextTypes.Count; a++)
                            try
                            {
                                m_visibleTextTypes.Add(m_formattedTextTypes[a]);
                            }
                            catch { continue; }
                        for (int a = 0; a < m_formattedTextColors.Count; a++)
                            try
                            {
                                m_visibleTextColors.Add(m_formattedTextColors[a]);
                            }
                            catch { continue; }
                    }       
                }
            }
            else
            {
                m_visibleLines = m_formattedLines;
                m_visibleTextTypes = m_formattedTextTypes;
                m_visibleTextColors = m_formattedTextColors;
            }

            if (Border != null) Border.Update(gameTime);
            if (DropDownMenu != null) DropDownMenu.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!m_visible)
                return;

            base.Draw(gameTime);

            if (BitmapFont.ActiveFonts.ContainsKey(Font))
            {
                // override BitmapFont sprite batch
                BitmapFont.ActiveFonts[Font].SpriteBatchOverride(Client.SpriteBatch);

                // replace text color using text color and text alpha
                Color textColor = new Color(m_textColor.R, m_textColor.G, m_textColor.B, m_textAlpha);

                // if disabled use static disabled color
                if (m_disabled)
                    textColor = new Color(ColorDisabledStandard.R, ColorDisabledStandard.G, ColorDisabledStandard.B, m_textAlpha);

                int lineHeight = 0;

                string[] lines = new string[m_visibleLines.Count];
                Enums.ETextType[] textTypes = new Enums.ETextType[m_visibleLines.Count];
                Color[] textColors = new Color[m_visibleLines.Count];

                try
                {
                    m_visibleLines.CopyTo(lines);
                    m_visibleTextTypes.CopyTo(textTypes);
                    m_visibleTextColors.CopyTo(textColors);
                }
                catch(Exception e)
                {
                    Utils.LogException(e);
                }

                // set text alignment
                BitmapFont.ActiveFonts[Font].Alignment = TextAlignment;
                Color drawTextColor = textColor;

                for (int a = 0; a < lines.Length; a++)
                {
                    if(Client.UserSettings.TextColorFiltering)
                        drawTextColor = new Color(textColors[a].R, textColors[a].G, textColors[a].B, m_textAlpha);
                    else if(lines.Length == textTypes.Length)
                        drawTextColor = Utils.GetTextTypeColor(textTypes[a]);

                    // switch to draw color based on preferred colors
                    int rectX = m_rectangle.X;
                    if (TextAlignment == BitmapFont.TextAlignment.Center)
                        rectX = (m_rectangle.X + Width - BitmapFont.ActiveFonts[Font].MeasureString(lines[a])) / 2;
                    else if (TextAlignment == BitmapFont.TextAlignment.Right)
                        rectX = m_rectangle.X + Width - BitmapFont.ActiveFonts[Font].MeasureString(lines[a]);

                    //BitmapFont.ActiveFonts[Font].TextBox(new Rectangle(m_rectangle.X + XTextOffset, m_rectangle.Y + YTextOffset + lineHeight, Width, lineHeight), drawTextColor, 
                    BitmapFont.ActiveFonts[Font].DrawString(rectX + XTextOffset, m_rectangle.Y + YTextOffset + lineHeight, drawTextColor, lines[a]);
                    lineHeight += BitmapFont.ActiveFonts[Font].LineHeight;
                }
            }
            else Utils.LogOnce("BitmapFont.ActiveFonts does not contain the Font [ " + Font + " ] for ScrollableTextBox [ " + m_name + " ] of Sheet [ " + GuiManager.CurrentSheet.Name + " ]");

            if (Border != null) Border.Draw(gameTime);
            if (DropDownMenu != null) DropDownMenu.Draw(gameTime);
        }

        public override bool MouseHandler(MouseState ms)
        {
            if (DropDownMenu != null)
                DropDownMenu.MouseHandler(ms);

            return base.MouseHandler(ms);
        }

        protected override void OnMouseDown(MouseState ms)
        {
            if (ms.RightButton == ButtonState.Pressed)
            {
                if (DropDownMenu == null)
                {
                    try
                    {
                        Rectangle dropDownRectangle = new Rectangle(ms.X - 10, ms.Y - 10, 100, 100); // default height for 5 drop down menu items

                        // readjust Y if out of client width bounds
                        //if (dropDownRectangle.Y + dropDownRectangle.Width > Client.Width)
                        //    dropDownRectangle.Y = Client.Width - dropDownRectangle.Width - 5;

                        if (Sheet != "Generic")
                        {
                            GuiManager.Sheets[Sheet].CreateDropDownMenu(Name + "DropDownMenu", Name, " Font Options ", dropDownRectangle, true,
                                Client.ClientSettings.DefaultDropDownMenuFont, new VisualKey("WhiteSpace"), Client.ClientSettings.ColorDropDownMenu, 255, true, Map.Direction.Northwest, 5);
                        }
                        else
                        {
                            GuiManager.GenericSheet.CreateDropDownMenu(Name + "DropDownMenu", Name, " Font Options ", dropDownRectangle, true,
                                Client.ClientSettings.DefaultDropDownMenuFont, new VisualKey("WhiteSpace"), Client.ClientSettings.ColorDropDownMenu, 255, true, Map.Direction.Northwest, 5);
                        }

                        // Uncomment below line to change border for this button while drop down menu is visible
                        //Border = new SquareBorder(Name + "Border", Name, 1, new VisualKey("WhiteSpace"), false, Client.ClientSettings.ColorDropDownMenuBorder, 255);
                        DropDownMenu.Border = new SquareBorder(Name + "Border", Name, 1, new VisualKey("WhiteSpace"), false, Client.ClientSettings.ColorDropDownMenuBorder, 255);

                        DropDownMenu.HasFocus = true;
                        int height = DropDownMenu.Title == "" ? 0 : BitmapFont.ActiveFonts[Font].LineHeight;
                        foreach (string _font in BitmapFont.ActiveFonts.Keys)
                        {
                            if (BitmapFont.ActiveFonts[_font].IsMonospace)
                            {
                                height += 20;
                                DropDownMenu.AddDropDownMenuItem(BitmapFont.ActiveFonts[_font].Name, Name + "DropDownMenu", new VisualKey("WhiteSpace"), "ScrollableTextBox_DropDown", _font, Font == _font ? true : false);
                            }
                        }

                        DropDownMenu.Height = height;
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                    }
                }
            }

            base.OnMouseDown(ms);
        }

        protected override void OnZDelta(MouseState ms)
        {
            if (m_scrollbar != null && !m_scrollbar.IsDisabled)
            {
                int prev = GuiManager.CurrentSheet.PreviousScrollWheelValue;
                int curr = ms.ScrollWheelValue;
                int diff = Math.Max(prev, curr) - Math.Min(prev, curr);

                if (Math.Max(diff, m_prevScrollWheelValue) -
                    Math.Min(diff, m_prevScrollWheelValue) == 0)
                    m_scrollbar.ScrollValue = 0;
                else if (prev < curr)
                    m_scrollbar.ScrollValue += (int)(diff / 120);
                else if (prev > curr)
                    m_scrollbar.ScrollValue -= (int)(diff / 120);

                // save scroll wheel value
                m_prevScrollWheelValue = ms.ScrollWheelValue;
            }
        }

        public void ScrollToTop()
        {
            m_scrollbar.ScrollValue = m_formattedLines.Count;
        }

        public override void OnClientResize(Rectangle prev, Rectangle now, bool ownerOverride)
        {
            if (DropDownMenu != null) GuiManager.Dispose(DropDownMenu);

            base.OnClientResize(prev, now, ownerOverride);

            m_formattedLines.Clear();
            m_formattedTextTypes.Clear();
            m_formattedTextColors.Clear();

            for (int a = 0; a < m_allLines.Count; a++)
            {
                FormatLine(m_allLines[a], m_allTextTypes[a], m_allTextColors[a]);
            }
        }

        public void AddLine(string line, Enums.ETextType textType)
        {
            // TODO: fix this. either server side or create a filter method
            if (line.StartsWith("You don't see a -"))
                line = "You don't see your target.";

            if (line.Contains("\r\n"))
            {
                string[] newLines = line.Split("\r\n".ToCharArray());
                foreach (string newLine in newLines)
                    AddLine(newLine, textType);
                return;
            }

            // buffer capacity
            if (m_allLines.Count >= m_maxLineBuffer)
            {
                m_allLines.RemoveAt(0);
                m_allTextTypes.RemoveAt(0);
                m_allTextColors.RemoveAt(0);
            }

            try
            {
                m_allLines.Add(line);

                if(GuiManager.LoggingRequested)
                    Utils.LogRequest(line);

                m_allTextTypes.Add(textType);

                // This is where line colors are decided.
                Color lineColor = TextColor;

                if (Colorize)
                {
                    lineColor = TextManager.GetTextFilteredColor(Client.GameState, line, true);

                    if (Client.InGame)
                    {
                        if (lineColor == TextColor && line.EndsWith("!") && !line.Contains(": "))
                            lineColor = TextManager.GetTextFilteredColor(Client.GameState, " hits with ", true);
                        else if (line.Contains(":") && line.IndexOf(" ") == line.IndexOf(":") + 1)
                        {
                            string[] f = line.Split(":".ToCharArray());
                            string[] g = f[1].Trim().Split(" ".ToCharArray());

                            bool magicChant = g.Length >= 4 && TextManager.MagicWords.Contains(g[0]) && TextManager.MagicWords.Contains(g[1]) && TextManager.MagicWords.Contains(g[2]) && TextManager.MagicWords.Contains(g[3]);

                            if (magicChant)
                                lineColor = Color.Khaki;
                            else lineColor = Color.BurlyWood; // speech/spell warming
                        }
                    }
                }

                m_allTextColors.Add(lineColor);

                FormatLine(line, textType, lineColor);
            }
            catch (Exception e)
            {
                Utils.Log("EXCEPTION: ScrollableTextBox.AddLine(" + line + ", " + textType.ToString());
                Utils.LogException(e);
            }

            if (Scrollbar.ScrollLocked) Scrollbar.ScrollValue = m_formattedLines.Count;
        }

        private void OnFontChange()
        {
            m_formattedLines.Clear();
            m_formattedTextTypes.Clear();
            m_formattedTextColors.Clear();

            for (int a = 0; a < m_allLines.Count; a++)
                FormatLine(m_allLines[a], m_allTextTypes[a], m_allTextColors[a]);
        }

        public void Clear()
        {
            m_allLines.Clear();
            m_allTextTypes.Clear();
            m_allTextColors.Clear();

            m_formattedLines.Clear();
            m_formattedTextTypes.Clear();
            m_formattedTextColors.Clear();

            m_visibleLines.Clear();
            m_visibleTextTypes.Clear();
            m_visibleTextColors.Clear();

            if (m_scrollbar != null)
                m_scrollbar.Reset();
        }

        public void FormatLine(string line, Enums.ETextType textType, Color textColor)
        {
            try
            {
                bool lineHandled = false;
                int lineLength = BitmapFont.ActiveFonts[Font].MeasureString(line);
                string newLine = "";

                if (lineLength > m_rectangle.Width - XTextOffset && line.StartsWith(" ") && !line.Substring(1, line.Length - 1).Contains(" "))
                    goto lineHandling;
                else if (lineLength > m_rectangle.Width - XTextOffset && !line.Contains(" "))
                    goto lineHandling;

                try
                {
                    if (lineLength > m_rectangle.Width - XTextOffset && line.Contains(" "))
                    {
                        int loopCount = 0;
                        while (lineLength > m_rectangle.Width - XTextOffset && loopCount < 2400)
                        {
                            try
                            {
                                newLine = newLine.Insert(0, line.Substring(line.LastIndexOf(' '), line.Length - line.LastIndexOf(' ')));
                                line = line.Remove(line.LastIndexOf(' '), line.Length - line.LastIndexOf(' '));
                                lineLength = BitmapFont.ActiveFonts[Font].MeasureString(line);
                                loopCount++;
                            }
                            catch { lineHandled = false; goto lineHandling; }
                        }
                        lineHandled = true;
                    }
                }
                catch { lineHandled = false; goto lineHandling; }

                lineHandling:
                if (!lineHandled) // splits the line in half
                {
                    int loopCount = 0;
                    while (lineLength > m_rectangle.Width - XTextOffset && loopCount < 200)
                    {
                        newLine = line.Substring(line.Length / 2);
                        line = line.Substring(0, line.Length / 2);
                        lineLength = BitmapFont.ActiveFonts[Font].MeasureString(line);
                        loopCount++;
                    }
                }

                // buffer capacity
                if (m_formattedLines.Count >= m_maxLineBuffer)
                {
                    m_formattedLines.RemoveAt(0);
                    m_formattedTextTypes.RemoveAt(0);
                    m_formattedTextColors.RemoveAt(0);
                }

                // trim spaces from start and end
                line = line.TrimEnd(Convert.ToChar(" "));

                if (m_trim)
                    line = line.TrimStart(Convert.ToChar(" "));

                m_formattedLines.Add(line);
                m_formattedTextTypes.Add(textType);
                m_formattedTextColors.Add(textColor);

                if (newLine.Length > 0)
                    FormatLine(newLine, textType, textColor);
            }
            catch (Exception e)
            {
                Utils.Log("EXCEPTION: ScrollableTextBox.FormatLine(" + line + ", " + textType.ToString());
                Utils.LogException(e);
            }
        }
    }
}
