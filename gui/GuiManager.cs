using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public partial class GuiManager : GameComponent
    {
        public const string PASSWORDCHAR = "*";
        public static bool LoggingRequested = false;
        public static bool MouseCursorVisible = true;

        #region Private Data
        static Dictionary<string, Texture2D> m_textures; // master textures
        static Dictionary<string, Sheet> m_sheets; // gui sheets
        static Dictionary<string, VisualInfo> m_visuals; // visual information for each texture piece
        static Dictionary<string, MouseCursor> m_cursors; // cursors
        static Control m_controlWithFocus = null;
        static Control m_draggedControl = null;
        static string m_activeTextBox = "";
        static string m_openComboBox = "";
        static GenericSheet m_genericSheet = null; // generic sheet
        static List<TextCue> m_textCues = new List<TextCue>();
        static List<Window> m_minimizeTrayList = new List<Window>();
        static int m_dragingXOffset; // holds where mouse cursor X coord is on a control being dragged
        static int m_draggingYOffset; // holds where mouse cursor Y coord is on a control being dragged
        #endregion

        public delegate void ControlDelegate(string controlName, object data);

        #region Public Properties
        public static Dictionary<string, Texture2D> Textures
        {
            get { return m_textures; }
        }
        public static Dictionary<string, Sheet> Sheets
        {
            get { return m_sheets; }
        }
        public static Dictionary<string, VisualInfo> Visuals
        {
            get { return m_visuals; }
        }
        public static Dictionary<string, MouseCursor> Cursors
        {
            get { return m_cursors; }
        }
        public static Sheet CurrentSheet
        {
            get { return m_sheets[Client.GameState.ToString()]; }
        }
        public static GenericSheet GenericSheet
        {
            get { return m_genericSheet; }
        }
        public static Control DraggedControl
        {
            get { return m_draggedControl; }
        }
        public static bool Dragging
        { get; set; }
        //public static int DraggingXOffset
        //{
        //    get { return m_dragingXOffset; }
        //}
        //public static int DraggingYOffset
        //{
        //    get { return m_draggingYOffset; }
        //    set { m_draggingYOffset = value; }
        //}
        public static Control ControlWithFocus
        {
            get { return m_controlWithFocus; }
            set { if (!(value is Window)) m_controlWithFocus = value; }
        }
        public static string ActiveTextBox
        { get; set; }
        public static string OpenComboBox
        { get; set;}
        public static KeyboardState KeyboardState
        {
            get { return Keyboard.GetState(); }
        }
        public static MouseState MouseState
        {
            get { return Mouse.GetState(); }
        }
        public static List<TextCue> TextCues
        {
            get { return m_textCues; }
        }

        public static Control MouseOverDropAcceptingControl
        {
            get; set;
        }
        #endregion

        #region Constructor
        public GuiManager(Game game)
            : base(game)
        {
            m_textures = new Dictionary<string, Texture2D>();
            m_sheets = new Dictionary<string, Sheet>();
            m_visuals = new Dictionary<string, VisualInfo>();
            m_cursors = new Dictionary<string, MouseCursor>();
        } 
        #endregion

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            if (!ParseXML("index.xml"))
            {
                Utils.Log("Fatal Error: Failed to load master index of XML files, verify index.xml.");
                throw new Exception("Failed to load the master index of XML files, verify index.xml.");
            }
        }

        /// <summary>
        /// Reload the IOK tile XML file.
        /// </summary>
        public void ReloadIOKTiles()
        {
            ParseXML(IOKMode.TileXMLFile);
        }

        /// <summary>
        /// Load a single GUI sheet.
        /// </summary>
        /// <param name="name">The defined name of the sheet to load.</param>
        public void LoadSheet(string filePath)
        {
            ParseXML(filePath);
        }

        private bool ParseXML(string xmlFile)
        {
            GraphicsDevice device = (Game.Services.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService).GraphicsDevice;
            string file = Utils.GetMediaFile(xmlFile);

            if (file == "")
            {
                Utils.LogOnce("XML File does not exist [ " + xmlFile + " ]");
                return false;
            }

            XmlTextReader reader = new XmlTextReader(file)
            {
                WhitespaceHandling = WhitespaceHandling.None
            };

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "File")
                    {
                        #region File
                        string fileName = "";

                        for (int i = 0; i < reader.AttributeCount; i++)
                        {
                            reader.MoveToAttribute(i);
                            if (reader.Name == "Name")
                                fileName = reader.Value;
                        }

                        if (fileName.ToLower().EndsWith(".xml"))
                        {
                            if (!ParseXML(fileName))
                            {
                                Utils.Log("Failed to load and parse XML file [ " + fileName + " ] in file [ " + xmlFile + " ]");
                                if (xmlFile == "index.xml")
                                    return false;
                            }
                        }
                        #endregion
                    }
                    else if (reader.Name == "Texture")
                    {
                        #region Texture
                        string name = "";
                        string path = "";

                        for (int i = 0; i < reader.AttributeCount; i++)
                        {
                            reader.MoveToAttribute(i);
                            if (reader.Name == "Name")
                                name = reader.Value;
                            else if (reader.Name == "Path")
                                path = reader.Value;
                        }

                        if (Textures.ContainsKey(name))
                        {
                            Utils.Log("Textures Dictionary already contains Key [ " + name + " ] from Texture Index [ " + path + " ]");
                        }
                        else
                        {
                            Textures.Add(name, Texture2D.FromStream(device, new System.IO.FileStream(Utils.GetMediaFile(path), System.IO.FileMode.Open)));
                        }
                        #endregion
                    }
                    else if (reader.Name == "Font")
                    {
                        #region Font
                        for (int i = 0; i < reader.AttributeCount; i++)
                        {
                            reader.MoveToAttribute(i);
                            if (reader.Name == "Path")
                            {
                                BitmapFont font = new BitmapFont(reader.Value);
                                font.Reset(device);
                                font.KernEnable = true;

                                if (!BitmapFont.ActiveFonts.ContainsKey("") && font.Name == Client.ClientSettings.DefaultFont)
                                    BitmapFont.ActiveFonts.Add("", font);
                            }
                        }
                        #endregion
                    }
                    else if (reader.Name == "Visual")
                    {
                        #region Visual
                        VisualInfo vi = new VisualInfo(reader);
                        if (Visuals.ContainsKey(vi.Name))
                            Utils.Log("Visuals dictionary already contains Key [ " + vi.Name + " ] from Visual XML File [ " + xmlFile + " ]");
                        else
                            Visuals.Add(vi.Name, vi);
                        #endregion
                    }
                    else if(reader.Name == "AutoVisuals")
                    {
                        #region AutoVisuals
                        string parentTexture = "";
                        string startName = "";
                        int counter = 0;
                        int numColumns = 0;
                        int numRows = 0;
                        int width = 0;
                        int height = 0;

                        for (int i = 0; i < reader.AttributeCount; i++)
                        {
                            reader.MoveToAttribute(i);
                            if (reader.Name == "ParentTexture")
                                parentTexture = reader.Value;
                            else if (reader.Name == "StartName")
                                startName = reader.Value;
                            else if (reader.Name == "StartCounter")
                                counter = reader.ReadContentAsInt();
                            else if (reader.Name == "Columns")
                                numColumns = reader.ReadContentAsInt();
                            else if (reader.Name == "Rows")
                                numRows = reader.ReadContentAsInt();
                            else if (reader.Name == "Width")
                                width = reader.ReadContentAsInt();
                            else if (reader.Name == "Height")
                                height = reader.ReadContentAsInt();
                        }

                        for (int rows = 0, y = 0; rows < numRows; rows++, y += height)
                        {
                            for (int cols = 0, x = 0; cols < numColumns; cols++, x += width)
                            {
                                VisualInfo vi = new VisualInfo(parentTexture, startName + counter.ToString(), x, y, width, height);
                                counter++;
                                if (Visuals.ContainsKey(vi.Name))
                                    Utils.Log("Visuals dictionary already contains Key [ " + vi.Name + " ] from Visual XML File [ " + xmlFile + " ]");
                                else
                                {
                                    //Utils.LogOnceToFile("AutoVisual Created: Key [ " + vi.Name + " ] from Visual XML File [ " + xmlFile + " ]", "AutoVisuals");
                                    Visuals.Add(vi.Name, vi);
                                }
                            }
                        } 
                        #endregion
                    }
                    else if (reader.Name == "Cursor")
                    {
                        MouseCursor cursor = new MouseCursor(reader);
                        Cursors.Add(cursor.Name, cursor);
                    }
                    else if (reader.Name == "IOKTileDef")
                    {
                        if (IOKMode.TileXMLFile != xmlFile) IOKMode.TileXMLFile = xmlFile;
                        IOKTileDefinition iokTileDef = new IOKTileDefinition(reader);
                        if (!IOKMode.Tiles.ContainsKey(iokTileDef.Graphic))
                            IOKMode.Tiles.Add(iokTileDef.Graphic, iokTileDef);
                        else Utils.Log("IOKTileDef dictionary already contains Graphic [ " + iokTileDef.Graphic + " ]");
                    }
                    else if (reader.Name == "SpinelTileDef")
                    {
                        if (SpinelMode.TileXMLFile != xmlFile) SpinelMode.TileXMLFile = xmlFile;
                        SpinelTileDefinition spinelTileDef = new SpinelTileDefinition(reader);
                        if (!SpinelMode.Tiles.ContainsKey(spinelTileDef.Graphic))
                            SpinelMode.Tiles.Add(spinelTileDef.Graphic, spinelTileDef);
                        else Utils.Log("SpinelTileDef dictionary already contains Graphic [ " + spinelTileDef.Graphic + " ]");
                    }
                    else if (reader.Name == "LOKTileDef")
                    {
                        if (LOKMode.TileXMLFile != xmlFile) LOKMode.TileXMLFile = xmlFile;
                        LOKTileDefinition lokTileDef = new LOKTileDefinition(reader);
                        if (!LOKMode.Tiles.ContainsKey(lokTileDef.Code))
                        {
                            LOKMode.Tiles.Add(lokTileDef.Code, lokTileDef);
                            LOKMode.Codes.Add(lokTileDef.Code);
                        }
                        else Utils.Log("LOKTileDef dictionary already contains Code [ " + lokTileDef.Code + " ]");
                    }
                    else if (reader.Name == "Sheet" || reader.Name == "GenericSheet")
                    {
                        #region Sheet
                        Sheet sheet = null;

                        if (reader.Name == "Sheet")
                        {
                            sheet = new Sheet(xmlFile, reader);
                            // sheet might be reloading on request, remove from the dictionary if it exists
                            if (Sheets.ContainsKey(sheet.Name))
                                Sheets.Remove(sheet.Name);

                            Sheets.Add(sheet.Name, sheet);

                        }
                        else if (reader.Name == "GenericSheet")
                        {
                            m_genericSheet = new GenericSheet(xmlFile, reader);
                            sheet = GuiManager.GenericSheet;
                        }
                        else break;

                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                string name = "";
                                string font = "";
                                string visualKey = ""; // used for background, control
                                bool visualTiled = false; // draw the visualKey tiled
                                string type = "";
                                string owner = "";
                                int x = 0;
                                int y = 0;
                                int width = 0;
                                int height = 0;
                                bool visible = false;
                                bool locked = true;
                                bool disabled = false;
                                byte visualAlpha = 255;
                                byte borderAlpha = 255;
                                byte textAlpha = 255;
                                string text = "";
                                bool textVisible = true;
                                bool editable = true; // textbox
                                string textColor = "White";
                                string tintColor = "White";
                                string selectionColor = "Blue";
                                int maxLength = 5; // textbox
                                bool passwordBox = false; // textbox
                                bool blinkingCursor = false; // textbox
                                string cursorColor = "LightBlue"; // textbox
                                int xTextOffset = 0;
                                int yTextOffset = 0;
                                bool dropShadow = false; // window, button
                                int shadowDistance = 10; // window, button
                                Map.Direction shadowDirection = Map.Direction.Southeast; // window, button
                                bool trim = false; // scrollable text box
                                Enums.ELayoutType layoutType = Enums.ELayoutType.Vertical;

                                string visualKeyOver = "";
                                string visualKeyDown = "";
                                string visualKeyDisabled = "";
                                string visualKeySelected = "";
                                string visualKeySelectedColor = "White";

                                string onMouseDown = "";
                                string onKeyboardEnter = "";
                                string onDoubleClick = "";
                                BitmapFont.TextAlignment textAlignment = BitmapFont.TextAlignment.Left;

                                bool hasTextOverColor = false;
                                string textOverColor = "White";

                                bool hasTintOverColor = false;
                                string tintOverColor = "White";

                                string popUpText = "";

                                string closeBoxVisualKey = ""; // WindowTitle (WindowControlBox)
                                string closeBoxVisualKeyDown = ""; // WindowTitle (WindowControlBox)
                                string maxBoxVisualKey = ""; // WindowTitle (WindowControlBox)
                                string maxBoxVisualKeyDown = ""; // WindowTitle (WindowControlBox)
                                string minBoxVisualKey = ""; // WindowTitle (WindowControlBox)
                                string minBoxVisualKeyDown = ""; // WindowTitle (WindowControlBox)
                                string cropBoxVisualKey = ""; // WindowTitle (WindowControlBox)
                                string cropBoxVisualKeyDown = ""; // WindowTitle (WindowControlBox)
                                int closeBoxDistanceFromTop = 0; // WindowTitle (WindowControlBox)
                                int closeBoxDistanceFromRight = 0; // WindowTitle (WindowControlBox)
                                int maxBoxDistanceFromTop = 0; // WindowTitle (WindowControlBox)
                                int maxBoxDistanceFromRight = 0; // WindowTitle (WindowControlBox)
                                int minBoxDistanceFromTop = 0; // WindowTitle (WindowControlBox)
                                int minBoxDistanceFromRight = 0; // WindowTitle (WindowControlBox)
                                int cropBoxDistanceFromTop = 0; // WindowTitle (WindowControlBox)
                                int cropBoxDistanceFromRight = 0; // WindowTitle (WindowControlBox)
                                int closeBoxWidth = 0; // WindowTitle (WindowControlBox)
                                int closeBoxHeight = 0; // WindowTitle (WindowControlBox)
                                int maxBoxWidth = 0; // WindowTitle (WindowControlBox)
                                int maxBoxHeight = 0; // WindowTitle (WindowControlBox)
                                int minBoxWidth = 0; // WindowTitle (WindowControlBox)
                                int minBoxHeight = 0; // WindowTitle (WindowControlBox)
                                int cropBoxWidth = 0; // WindowTitle (WindowControlBox)
                                int cropBoxHeight = 0; // WindowTitle (WindowControlBox)
                                int closeBoxVisualAlpha = 255; // WindowTitle (WindowControlBox)
                                int maxBoxVisualAlpha = 255; // WindowTitle (WindowControlBox)
                                int minBoxVisualAlpha = 255; // WindowTitle (WindowControlBox)
                                int cropBoxVisualAlpha = 255; // WindowTitle (WindowControlBox)
                                string closeBoxTintColor = "White"; // WindowTitle (WindowControlBox)
                                string minBoxTintColor = "White"; // WindowTitle (WindowControlBox)
                                string maxBoxTintColor = "White"; // WindowTitle (WindowControlBox)
                                string cropBoxTintColor = "White"; // WindowTitle (WindowControlBox)
                                string shortcut = ""; // used for HotButtons -- not currently implemented in XML 2/18/2017
                                string command = ""; // used for buttons and other click components to send text
                                int tabOrder = -1; // used for tabOrder, currently (4/9/2019) only TextBoxes have these. Tab Order is handled by owner/sheet
                                int maxValue = 18;
                                int minValue = 3;

                                List<Enums.EAnchorType> anchors = new List<Enums.EAnchorType>();
                                Enums.EAnchorType windowTitleOrientation = Enums.EAnchorType.Top;
                                int autoHideVisualAlpha = 0;
                                bool fadeIn = false;
                                bool fadeOut = true;
                                int fadeSpeed = 2; // how quickly a fading AutoHidingWindow fades in and out
                                // used for generic sheet controls to limit visibility
                                List<Enums.EGameState> lockoutStates = new List<Enums.EGameState>();

                                string cursorOnDrag = ""; // cursor on mouse drag (Window)
                                string cursorOnOver = ""; // cursor on mouse over (Label)

                                string tabControlledWindow = "";

                                if (reader.Name == "Background")
                                {
                                    sheet.Background = new Background(reader);                                    
                                }
                                else if (reader.Name == "Control")
                                {
                                    #region Control
                                    for (int i = 0; i < reader.AttributeCount; i++)
                                    {
                                        reader.MoveToAttribute(i);
                                        if (reader.Name == "Type")
                                            type = reader.Value;
                                        else if (reader.Name == "Name")
                                            name = reader.Value;
                                        else if (reader.Name == "Owner")
                                            owner = reader.Value;
                                        else if (reader.Name == "X")
                                            x = reader.ReadContentAsInt();
                                        else if (reader.Name == "Y")
                                            y = reader.ReadContentAsInt();
                                        else if (reader.Name == "Width")
                                            width = reader.ReadContentAsInt();
                                        else if (reader.Name == "Height")
                                            height = reader.ReadContentAsInt();
                                        else if (reader.Name == "IsVisible")
                                            visible = reader.ReadContentAsBoolean();
                                        else if (reader.Name == "IsLocked")
                                            locked = reader.ReadContentAsBoolean();
                                        else if (reader.Name == "IsDisabled")
                                            disabled = reader.ReadContentAsBoolean();
                                        else if (reader.Name == "Font")
                                            font = reader.Value;
                                        else if (reader.Name == "VisualKey")
                                            visualKey = reader.Value;
                                        else if (reader.Name == "VisualTiled")
                                            visualTiled = reader.ReadContentAsBoolean();
                                        else if (reader.Name == "TintColor")
                                            tintColor = reader.Value;
                                        else if (reader.Name == "VisualAlpha")
                                            visualAlpha = Convert.ToByte(reader.Value);
                                        else if (reader.Name == "BorderAlpha")
                                            borderAlpha = Convert.ToByte(reader.Value);
                                        else if (reader.Name == "TextAlpha")
                                            textAlpha = Convert.ToByte(reader.Value);
                                        else if (reader.Name == "Text")
                                            text = reader.Value;
                                        else if (reader.Name == "TextVisible")
                                            textVisible = reader.ReadContentAsBoolean();
                                        else if (reader.Name == "IsEditable")
                                            editable = reader.ReadContentAsBoolean();
                                        else if (reader.Name == "TextColor")
                                            textColor = reader.Value;
                                        else if (reader.Name == "MaxLength")
                                            maxLength = reader.ReadContentAsInt();
                                        else if (reader.Name == "IsPasswordBox")
                                            passwordBox = reader.ReadContentAsBoolean();
                                        else if (reader.Name == "HasBlinkingCursor")
                                            blinkingCursor = reader.ReadContentAsBoolean();
                                        else if (reader.Name == "CursorColor")
                                            cursorColor = reader.Value;
                                        else if (reader.Name == "VisualKeyOver")
                                            visualKeyOver = reader.Value;
                                        else if (reader.Name == "VisualKeyDown")
                                            visualKeyDown = reader.Value;
                                        else if (reader.Name == "VisualKeyDisabled")
                                            visualKeyDisabled = reader.Value;
                                        else if (reader.Name == "VisualKeySelected")
                                            visualKeySelected = reader.Value;
                                        else if (reader.Name == "VisualKeySelectedColor")
                                            visualKeySelectedColor = reader.Value;
                                        else if (reader.Name == "OnMouseDown")
                                            onMouseDown = reader.Value;
                                        else if (reader.Name == "TextAlignment")
                                            textAlignment = (BitmapFont.TextAlignment)Enum.Parse(typeof(BitmapFont.TextAlignment), reader.Value, true);
                                        else if (reader.Name == "XTextOffset")
                                            xTextOffset = reader.ReadContentAsInt();
                                        else if (reader.Name == "YTextOffset")
                                            yTextOffset = reader.ReadContentAsInt();
                                        else if (reader.Name == "OnKeyboardEnter")
                                            onKeyboardEnter = reader.Value;
                                        else if (reader.Name == "TextOverColor")
                                        {
                                            textOverColor = reader.Value;
                                            hasTextOverColor = true;
                                        }
                                        else if (reader.Name == "TintOverColor")
                                        {
                                            tintOverColor = reader.Value;
                                            hasTintOverColor = true;
                                        }
                                        else if (reader.Name == "SelectionColor")
                                            selectionColor = reader.Value;
                                        else if (reader.Name == "OnDoubleClick")
                                            onDoubleClick = reader.Value;
                                        else if (reader.Name == "DropShadow")
                                            dropShadow = reader.ReadContentAsBoolean();
                                        else if (reader.Name == "ShadowDistance")
                                            shadowDistance = reader.ReadContentAsInt();
                                        else if (reader.Name == "ShadowDirection")
                                            shadowDirection = (Map.Direction)Enum.Parse(typeof(Map.Direction), reader.Value, true);
                                        else if (reader.Name == "AnchorTop")
                                        {
                                            if (reader.ReadContentAsBoolean())
                                                anchors.Add(Enums.EAnchorType.Top);
                                        }
                                        else if (reader.Name == "AnchorBottom")
                                        {
                                            if (reader.ReadContentAsBoolean())
                                                anchors.Add(Enums.EAnchorType.Bottom);
                                        }
                                        else if (reader.Name == "AnchorLeft")
                                        {
                                            if (reader.ReadContentAsBoolean())
                                                anchors.Add(Enums.EAnchorType.Left);
                                        }
                                        else if (reader.Name == "AnchorRight")
                                        {
                                            if (reader.ReadContentAsBoolean())
                                                anchors.Add(Enums.EAnchorType.Right);
                                        }
                                        else if (reader.Name == "AnchorCenter")
                                        {
                                            if (reader.ReadContentAsBoolean())
                                                anchors.Add(Enums.EAnchorType.Center);
                                        }
                                        else if (reader.Name == "LayoutType")
                                            layoutType = (Enums.ELayoutType)Enum.Parse(typeof(Enums.ELayoutType), reader.Value, true);
                                        else if (reader.Name == "LockoutState")
                                            lockoutStates.Add((Enums.EGameState)Enum.Parse(typeof(Enums.EGameState), reader.Value, true));
                                        else if (reader.Name == "PopUpText")
                                            popUpText = reader.Value;
                                        // window titles
                                        else if (reader.Name == "CloseBoxVisualKey")
                                            closeBoxVisualKey = reader.Value;
                                        else if (reader.Name == "MaximizeBoxVisualKey")
                                            maxBoxVisualKey = reader.Value;
                                        else if (reader.Name == "MinimizeBoxVisualKey")
                                            minBoxVisualKey = reader.Value;
                                        else if (reader.Name == "CropBoxVisualKey")
                                            cropBoxVisualKey = reader.Value;
                                        else if (reader.Name == "CloseBoxVisualKeyDown")
                                            closeBoxVisualKeyDown = reader.Value;
                                        else if (reader.Name == "MaximizeBoxVisualKeyDown")
                                            maxBoxVisualKeyDown = reader.Value;
                                        else if (reader.Name == "MinimizeBoxVisualKeyDown")
                                            minBoxVisualKeyDown = reader.Value;
                                        else if (reader.Name == "CropBoxVisualKeyDown")
                                            cropBoxVisualKeyDown = reader.Value;
                                        else if (reader.Name == "CloseBoxDistanceFromRight")
                                            closeBoxDistanceFromRight = reader.ReadContentAsInt();
                                        else if (reader.Name == "CloseBoxDistanceFromTop")
                                            closeBoxDistanceFromTop = reader.ReadContentAsInt();
                                        else if (reader.Name == "MaximizeBoxDistanceFromRight")
                                            maxBoxDistanceFromRight = reader.ReadContentAsInt();
                                        else if (reader.Name == "MaximizeBoxDistanceFromTop")
                                            maxBoxDistanceFromTop = reader.ReadContentAsInt();
                                        else if (reader.Name == "MinimizeBoxDistanceFromRight")
                                            minBoxDistanceFromRight = reader.ReadContentAsInt();
                                        else if (reader.Name == "MinimizeBoxDistanceFromTop")
                                            minBoxDistanceFromTop = reader.ReadContentAsInt();
                                        else if (reader.Name == "CropBoxDistanceFromRight")
                                            cropBoxDistanceFromRight = reader.ReadContentAsInt();
                                        else if (reader.Name == "CropBoxDistanceFromTop")
                                            cropBoxDistanceFromTop = reader.ReadContentAsInt();
                                        else if (reader.Name == "CloseBoxWidth")
                                            closeBoxWidth = reader.ReadContentAsInt();
                                        else if (reader.Name == "CloseBoxHeight")
                                            closeBoxHeight = reader.ReadContentAsInt();
                                        else if (reader.Name == "CloseBoxTintColor")
                                            closeBoxTintColor = reader.Value;
                                        else if (reader.Name == "MaximizeBoxWidth")
                                            maxBoxWidth = reader.ReadContentAsInt();
                                        else if (reader.Name == "MaximizeBoxHeight")
                                            maxBoxHeight = reader.ReadContentAsInt();
                                        else if (reader.Name == "MaximizeBoxTintColor")
                                            maxBoxTintColor = reader.Value;
                                        else if (reader.Name == "MinimizeBoxWidth")
                                            minBoxWidth = reader.ReadContentAsInt();
                                        else if (reader.Name == "MinimizeBoxHeight")
                                            minBoxHeight = reader.ReadContentAsInt();
                                        else if (reader.Name == "MinimizeBoxTintColor")
                                            minBoxTintColor = reader.Value;
                                        else if (reader.Name == "CropBoxWidth")
                                            cropBoxWidth = reader.ReadContentAsInt();
                                        else if (reader.Name == "CropBoxHeight")
                                            cropBoxHeight = reader.ReadContentAsInt();
                                        else if (reader.Name == "CropBoxTintColor")
                                            cropBoxTintColor = reader.Value;
                                        else if (reader.Name == "CloseBoxVisualAlpha")
                                            closeBoxVisualAlpha = reader.ReadContentAsInt();
                                        else if (reader.Name == "MaximizeBoxVisualAlpha")
                                            maxBoxVisualAlpha = reader.ReadContentAsInt();
                                        else if (reader.Name == "MinimizeCloseBoxVisualAlpha")
                                            minBoxVisualAlpha = reader.ReadContentAsInt();
                                        else if (reader.Name == "CropBoxVisualAlpha")
                                            cropBoxVisualAlpha = reader.ReadContentAsInt();
                                        // end window titles
                                        else if (reader.Name == "CursorOnDrag")
                                            cursorOnDrag = reader.Value;
                                        else if (reader.Name == "CursorOnOver")
                                            cursorOnOver = reader.Value;
                                        else if (reader.Name == "Trim")
                                            trim = reader.ReadContentAsBoolean();
                                        else if (reader.Name == "Shortcut")
                                            shortcut = reader.Value;
                                        else if (reader.Name == "Command")
                                            command = reader.Value;
                                        else if (reader.Name == "TabOrder")
                                            tabOrder = reader.ReadContentAsInt();
                                        else if (reader.Name == "MaxValue")
                                            maxValue = reader.ReadContentAsInt();
                                        else if (reader.Name == "MinValue")
                                            minValue = reader.ReadContentAsInt();
                                        else if (reader.Name == "TabControlledWindow")
                                            tabControlledWindow = reader.Value;
                                        else if (reader.Name == "WindowTitleOrientation")
                                            windowTitleOrientation = (Enums.EAnchorType)Enum.Parse(typeof(Enums.EAnchorType), reader.Value, true);
                                        else if (reader.Name == "AutoHideVisualAlpha")
                                            autoHideVisualAlpha = reader.ReadContentAsInt();
                                        else if (reader.Name == "FadeIn")
                                            fadeIn = reader.ReadContentAsBoolean();
                                        else if (reader.Name == "FadeOut")
                                            fadeOut = reader.ReadContentAsBoolean();
                                        else if (reader.Name == "FadeSpeed")
                                            fadeSpeed = reader.ReadContentAsInt();
                                    }
                                    #endregion
                                }

                                switch (type)
                                {
                                    case "AutoHidingWindow":
                                        sheet.CreateAutoHidingWindow(name, type, owner, new Rectangle(x, y, width, height), visible,
                                            locked, disabled, font, new VisualKey(visualKey), Utils.GetColor(tintColor),
                                            visualAlpha, borderAlpha, dropShadow, shadowDirection, shadowDistance, anchors,
                                            cursorOnDrag, windowTitleOrientation, autoHideVisualAlpha, fadeIn, fadeOut, fadeSpeed);
                                        break;
                                    case "Window":
                                    case "HotButtonEditWindow":
                                        sheet.CreateWindow(name, type, owner, new Rectangle(x, y, width, height), visible,
                                            locked, disabled, font, new VisualKey(visualKey), Utils.GetColor(tintColor),
                                            visualAlpha, borderAlpha, dropShadow, shadowDirection, shadowDistance, anchors,
                                            cursorOnDrag);
                                        break;
                                    case "WindowTitle":
                                        sheet.CreateWindowTitle(name, owner, font, text, Utils.GetColor(textColor), Utils.GetColor(tintColor), visualAlpha,
                                            textAlignment, new VisualKey(visualKey), visualTiled, new VisualKey(closeBoxVisualKey), new VisualKey(maxBoxVisualKey),
                                            new VisualKey(minBoxVisualKey), new VisualKey(cropBoxVisualKey), new VisualKey(closeBoxVisualKeyDown),
                                            new VisualKey(maxBoxVisualKeyDown), new VisualKey(minBoxVisualKeyDown), new VisualKey(cropBoxVisualKeyDown),
                                            closeBoxDistanceFromRight, closeBoxDistanceFromTop, closeBoxWidth, closeBoxHeight, closeBoxVisualAlpha,
                                            maxBoxDistanceFromRight, maxBoxDistanceFromTop, maxBoxWidth, maxBoxHeight, maxBoxVisualAlpha,
                                            minBoxDistanceFromRight, minBoxDistanceFromTop, minBoxWidth, minBoxHeight, minBoxVisualAlpha,
                                            cropBoxDistanceFromRight, cropBoxDistanceFromTop, cropBoxWidth, cropBoxHeight, cropBoxVisualAlpha,
                                            Utils.GetColor(closeBoxTintColor), Utils.GetColor(maxBoxTintColor), Utils.GetColor(minBoxTintColor),
                                            Utils.GetColor(cropBoxTintColor), height);
                                        break;
                                    case "TexturedBorder":
                                        break;
                                    case "SquareBorder":
                                        sheet.CreateSquareBorder(name, owner, width, new VisualKey(visualKey), visualTiled, Utils.GetColor(tintColor), visualAlpha);
                                        break;
                                    case "NumericTextBox":
                                        sheet.CreateNumericTextBox(name, owner, new Rectangle(x, y, width, height), text, Utils.GetColor(textColor),
                                            textAlignment, visible, disabled, font, new VisualKey(visualKey), Utils.GetColor(tintColor), visualAlpha, borderAlpha,
                                            textAlpha, editable, maxLength, passwordBox, blinkingCursor, Utils.GetColor(cursorColor),
                                            new VisualKey(visualKeyOver), new VisualKey(visualKeyDown), new VisualKey(visualKeyDisabled),
                                            xTextOffset, yTextOffset, onKeyboardEnter, Utils.GetColor(selectionColor), anchors, tabOrder, maxValue, minValue);
                                        break;
                                    case "TextBox":
                                        sheet.CreateTextBox(name, owner, new Rectangle(x, y, width, height), text, Utils.GetColor(textColor), textAlignment,
                                            visible, disabled, font, new VisualKey(visualKey), Utils.GetColor(tintColor), visualAlpha, borderAlpha,
                                            textAlpha, editable, maxLength, passwordBox, blinkingCursor, Utils.GetColor(cursorColor),
                                            new VisualKey(visualKeyOver), new VisualKey(visualKeyDown), new VisualKey(visualKeyDisabled),
                                            xTextOffset, yTextOffset, onKeyboardEnter, Utils.GetColor(selectionColor), anchors, tabOrder);
                                        break;
                                    case "Button":
                                    case "HotButton":
                                    case "MacroButton":
                                    case "TabControlButton":
                                    case "ColorDialogButton":
                                    case "DragAndDropButton":
                                        sheet.CreateButton(type, name, owner, new Rectangle(x, y, width, height), text, textVisible, Utils.GetColor(textColor),
                                            visible, disabled, font, new VisualKey(visualKey), Utils.GetColor(tintColor), visualAlpha, borderAlpha,
                                            textAlpha, new VisualKey(visualKeyOver), new VisualKey(visualKeyDown), new VisualKey(visualKeyDisabled),
                                            onMouseDown, textAlignment, xTextOffset, yTextOffset, Utils.GetColor(textOverColor), hasTextOverColor,
                                            Utils.GetColor(tintOverColor), hasTintOverColor, anchors, dropShadow, shadowDirection, shadowDistance,
                                            command, popUpText, tabControlledWindow, cursorOnOver);
                                        break;
                                    case "CheckboxButton":
                                        sheet.CreateCheckBoxButton(name, owner, new Rectangle(x, y, width, height), visible, disabled, new VisualKey(visualKey), Utils.GetColor(tintColor), visualAlpha, borderAlpha, new VisualKey(visualKeyOver),
                                            new VisualKey(visualKeyDown), new VisualKey(visualKeyDisabled), new VisualKey(visualKeySelected), Utils.GetColor(visualKeySelectedColor), Utils.GetColor(tintOverColor), hasTintOverColor, anchors, dropShadow,
                                            shadowDirection, shadowDistance, popUpText);
                                        break;
                                    case "Label":
                                        sheet.CreateLabel(name, owner, new Rectangle(x, y, width, height), text, Utils.GetColor(textColor),
                                            visible, disabled, font, new VisualKey(visualKey), Utils.GetColor(tintColor), visualAlpha,
                                            borderAlpha, textAlpha, textAlignment, xTextOffset, yTextOffset, onDoubleClick, cursorOnOver, anchors,
                                            popUpText);
                                        break;
                                    case "CritterListLabel":
                                        sheet.CreateCritterListLabel(name, owner, new Rectangle(x, y, width, height), text, Utils.GetColor(textColor),
                                            visible, disabled, font, new VisualKey(visualKey), Utils.GetColor(tintColor), visualAlpha,
                                            borderAlpha, textAlpha, textAlignment, xTextOffset, yTextOffset, onDoubleClick, cursorOnOver, anchors,
                                            popUpText);
                                        break;
                                    case "IOKTileLabel":
                                        sheet.CreateIOKTileLabel(name, owner, new Rectangle(x, y, width, height), text, Utils.GetColor(textColor),
                                            visible, disabled, font, new VisualKey(visualKey), Utils.GetColor(tintColor), visualAlpha,
                                            borderAlpha, textAlpha, textAlignment, xTextOffset, yTextOffset, onDoubleClick, cursorOnOver, anchors,
                                            popUpText);
                                        break;
                                    case "SpinelTileLabel":
                                        sheet.CreateSpinelTileLabel(name, owner, new Rectangle(x, y, width, height), text, Utils.GetColor(textColor),
                                            visible, disabled, font, new VisualKey(visualKey), Utils.GetColor(tintColor), visualAlpha,
                                            borderAlpha, textAlpha, textAlignment, xTextOffset, yTextOffset, onDoubleClick, cursorOnOver, anchors,
                                            popUpText);
                                        break;
                                    case "ScrollableTextBox":
                                        sheet.CreateScrollableTextBox(name, owner, new Rectangle(x, y, width, height), text, Utils.GetColor(textColor),
                                            visible, disabled, font, new VisualKey(visualKey), Utils.GetColor(tintColor), visualAlpha,
                                            borderAlpha, textAlpha, new VisualKey(visualKeyOver), new VisualKey(visualKeyDown),
                                            new VisualKey(visualKeyDisabled), xTextOffset, yTextOffset, textAlignment, anchors, trim);
                                        break;
                                    case "StatusBar":
                                        sheet.CreateStatusBar(name, owner, new Rectangle(x, y, width, height), text, Utils.GetColor(textColor),
                                            visible, disabled, font, new VisualKey(visualKey), Utils.GetColor(tintColor), visualAlpha,
                                            borderAlpha, textAlpha, textAlignment, xTextOffset, yTextOffset, onDoubleClick, cursorOnOver, anchors,
                                            layoutType);
                                        break;
                                    default:
                                        //Utils.Log("GuiManager has no method defined for creating control of Type [" + type + "] for Sheet [" + sheet.Name + "] -- Control Name [" + name + "].");
                                        //Utils.Log("Reader Info: " + reader.Name + ", " + reader.LocalName);
                                        break;
                                }

                                // only generic controls use lockout states, so if this is a generic sheet set lockout states
                                if (sheet.Name == "Generic")
                                {
                                    if (owner.Length > 0)
                                    {
                                        Window w = (sheet[owner] as Window);
                                        if (w != null)
                                        {
                                            Control c = w[name];
                                            if (c != null)
                                                c.LockoutStates = lockoutStates;
                                        }
                                    }
                                    else
                                    {
                                        Control c = sheet[name];
                                        if(c != null)
                                            c.LockoutStates = lockoutStates;
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    else if (reader.Name == "Lore")
                    {
                        if (Lore.LoreXMLFile != xmlFile) Lore.LoreXMLFile = xmlFile;
                    }
                }
            }
            reader.Close();
            return true;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            string state = Client.GameState.ToString();

            //if (state == Enums.eGameState.Splash.ToString())

            base.Update(gameTime);

            // Overridden states girst (eg: Hot Button Edit Mode)
            if (GameHUD.OverrideDisplayStates.Contains(Client.GameState))
                m_sheets[state].Update(gameTime);

            if (m_genericSheet != null)
                m_genericSheet.Update(gameTime);

            if (Client.GameState.ToString().Contains("Game"))
            {
                if (GuiManager.GetControl("GameAutoHidingWindow") is AutoHidingWindow gameAutoHidingWindow)
                {
                    gameAutoHidingWindow.IsVisible = true;
                }

                #region DamageFogSkullsLabel
                if (Character.CurrentCharacter != null && GuiManager.GetControl("DamageFogSkullsLabel") is Label fogLabel)
                {
                    double pct = (double)((Character.CurrentCharacter.Hits * 100) / Character.CurrentCharacter.HitsFull);

                    if (pct <= 50)
                    {
                        fogLabel.TintColor = Color.LemonChiffon;
                        fogLabel.VisualAlpha = 65;
                        fogLabel.IsVisible = true;

                        if (pct <= 25)
                        {
                            fogLabel.TintColor = Color.Crimson;
                            fogLabel.VisualAlpha = 125;

                            if (Character.CurrentCharacter.Hits <= 0)
                            {
                                fogLabel.TintColor = Color.Crimson;
                                fogLabel.VisualAlpha = 190;
                            }
                        }
                    }
                    else fogLabel.IsVisible = false;
                }
                #endregion
            }
            else
            {
                if (GuiManager.GetControl("GameAutoHidingWindow") is AutoHidingWindow gameAutoHidingWindow)
                    gameAutoHidingWindow.IsVisible = false;

                if (GuiManager.GetControl("DamageFogSkullsLabel") is Label fogLabel)
                    fogLabel.IsVisible = false;
            }

            if (!GameHUD.OverrideDisplayStates.Contains(Client.GameState) && m_sheets.ContainsKey(state))
                m_sheets[state].Update(gameTime);            

            Events.UpdateGUI(gameTime);

            for (int a = m_textCues.Count - 1; a >= 0; a--)
                m_textCues[a].Update(gameTime, m_textCues);

            for (int a = 0; a < m_textCues.Count; a++)
            {
                if (m_textCues[a].IsCentered)
                {
                    m_textCues[a].X = (int)((Client.Width / 2) - (BitmapFont.ActiveFonts[m_textCues[a].Font].MeasureString(m_textCues[a].Text) / 2));
                    m_textCues[a].Y = (int)(((m_textCues.Count + 1) * BitmapFont.ActiveFonts[CurrentSheet.Font].LineHeight) / 2) +
                        (a * BitmapFont.ActiveFonts[CurrentSheet.Font].LineHeight);
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            string state = Client.GameState.ToString();

            Client.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            //Client.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend); // Before MonoGame conversion.

            // Overridden states girst (eg: Hot Button Edit Mode)
            if (GameHUD.OverrideDisplayStates.Contains(Client.GameState))
                m_sheets[state].Draw(gameTime);

            if (!GameHUD.OverrideDisplayStates.Contains(Client.GameState) && m_sheets.ContainsKey(state))
                m_sheets[state].Draw(gameTime);

            if (m_genericSheet != null)
                m_genericSheet.Draw(gameTime);

            foreach (TextCue tc in m_textCues)
                tc.Draw(gameTime);

            Client.SpriteBatch.End();
        }

        public static Control GetControl(string sheet, string name)
        {
            try
            {
                foreach (Control c1 in new List<Control>(Sheets[sheet].Controls))
                {
                    if (c1.Name == name)
                        return c1;

                    if (c1 is Window)
                    {
                        foreach (Control c2 in new List<Control>((c1 as Window).Controls))
                        {
                            if (c2.Name == name)
                                return c2;
                            else if (c2 is Window)
                            {
                                foreach (Control c3 in new List<Control>((c2 as Window).Controls))
                                {
                                    if (c3.Name == name)
                                        return c3;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utils.LogException(e);
                Utils.Log("Error in GuiManager.GetControl - Searching current sheet for a control named " + name + ".");
                return null;
            }
            return null;
        }

        public static Control GetControl(string name)
        {
            if (CurrentSheet[name] != null)
            {
                return CurrentSheet[name];
            }

            if (GenericSheet[name] != null)
            {
                return GenericSheet[name];
            }

            // Check CurrentSheet controls.
            try
            {
                foreach (Control c1 in new List<Control>(CurrentSheet.Controls))
                {
                    if (c1 is Window)
                    {
                        foreach (Control c2 in new List<Control>((c1 as Window).Controls))
                        {
                            if (c2.Name == name)
                                return c2;
                            else if (c2 is Window)
                            {
                                foreach (Control c3 in new List<Control>((c2 as Window).Controls))
                                {
                                    if (c3.Name == name)
                                        return c3;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utils.LogException(e);
                Utils.Log("Error in GuiManager.GetControl - Searching current sheet for a control named " + name + ".");
            }

            // Check GenericSheet controls.
            try
            {
                foreach (Control c1 in new List<Control>(GenericSheet.Controls))
                {
                    if (c1 is Window)
                    {
                        foreach (Control c2 in new List<Control>((c1 as Window).Controls))
                        {
                            if (c2.Name == name)
                                return c2;
                            else if (c2 is Window)
                            {
                                foreach (Control c3 in new List<Control>((c2 as Window).Controls))
                                    if (c3.Name == name)
                                        return c3;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utils.LogException(e);
                Utils.Log("Error in GuiManager.GetControl - Searching generic sheet for a control named " + name + ".");
            }

            return null;
        }

        public static void Dispose(Control c)
        {
            if(c.Owner != "")
            {
                if(GuiManager.GetControl(c.Owner) is Window w)
                {
                    w.Controls.Remove(c);
                }
            }

            if (c.Sheet != "Generic")
                CurrentSheet.RemoveControl(c);
            else GenericSheet.RemoveControl(c);
        }

        public static void StartDragging(Control control, MouseState ms)
        {
            if (Dragging) return;

            m_draggedControl = control;
            Dragging = true;
            m_dragingXOffset = ms.X - control.Position.X;
            m_draggingYOffset = ms.Y - control.Position.Y;
        }

        public static void StopDragging()
        {
            m_draggedControl = null;
            Dragging = false;
        }
    }
}


