using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Yuusha.gui;
using Yuusha.Utility.Settings;

namespace Yuusha
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Client : Game
    {
        #region Private Data
        private static bool m_roundDelay = false;
        private static TimeSpan m_roundDelayStart;
        private static bool m_hasFocus;
        private static SpriteBatch m_spriteBatch;
        private static Enums.EGameState m_gameState;
        private static Enums.EGameDisplayMode m_gameDisplayMode;
        private static int m_width;
        private static int m_height;
        private static string m_title;
        private static UserSettings m_userSettings = new UserSettings();
        private static ServerSettings m_serverSettings = new ServerSettings();
        private static ClientSettings m_clientSettings = new ClientSettings();
        private static int m_preferredWindowWidth = 1280;
        private static int m_preferredWindowHeight = 720;
        private static readonly SurfaceFormat m_preferredSurfaceFormat = SurfaceFormat.Color;
        private static readonly DepthFormat m_preferredDepthFormat = DepthFormat.Depth24;
        private static Color m_deviceClearColor = Color.Black;
        private static bool m_isFullScreen = false;
        private static Rectangle m_prevClientBounds;
        private static Rectangle m_nowClientBounds;
        private static Point m_prevClientPosition;
        private static TimeSpan m_lastPing;

        GraphicsDeviceManager m_graphics;
        readonly ContentManager m_contentManager;
        GuiManager m_guiManager;
        readonly GameHUD m_gameHUD;
        readonly Audio.AudioManager m_audioManager;
        bool m_firstFullScreen;
        bool m_noDraw;
        #endregion

        #region Public Properties
        public static UserSettings UserSettings
        {
            get { return m_userSettings; }
        }
        public static ServerSettings ServerSettings
        {
            get { return m_serverSettings; }
        }
        public static ClientSettings ClientSettings
        {
            get { return m_clientSettings; }
        }
        public static SpriteBatch SpriteBatch
        {
            get { return m_spriteBatch; }
        }
        public static Enums.EGameState GameState
        {
            get { return m_gameState; }
            set { m_gameState = value; }
        }
        public static Enums.EGameDisplayMode GameDisplayMode
        {
            get { return m_gameDisplayMode; }
            set { m_gameDisplayMode = value; }
        }
        public static int Width
        {
            get { return m_width; }
        }
        public static int Height
        {
            get { return m_height; }
        }
        public static string Title
        {
            get { return m_title; }
            set { m_title = value; }
        }
        public static int PreferredWindowWidth
        {
            get { return m_preferredWindowWidth; }
            set { m_preferredWindowWidth = value; }
        }
        public static int PreferredWindowHeight
        {
            get { return m_preferredWindowHeight; }
            set { m_preferredWindowHeight = value; }
        }
        public static Color DeviceClearColor
        {
            get { return m_deviceClearColor; }
            set { m_deviceClearColor = value; }
        }
        public static bool IsFullScreen
        {
            get { return m_isFullScreen; }
            set { m_isFullScreen = value; }
        }
        public static Rectangle PrevClientBounds
        {
            get { return m_prevClientBounds; }
        }
        public static Rectangle NowClientBounds
        {
            get { return m_nowClientBounds; }
        }
        public static bool HasFocus
        {
            get { return m_hasFocus; }
        }
        public static bool RoundDelay
        {
            get { return m_roundDelay; }
            set { m_roundDelay = value; }
        }
        public static TimeSpan LastPing
        {
            get { return m_lastPing; }
            set { m_lastPing = value; }
        }
        //public static TimeSpan TotalGameTime
        //{
        //    get { return m_totalGameTime; }
        //}
        public GuiManager GUIManager
        {
            get { return m_guiManager; }
        }
        //public SplashScreen SplashScreen
        //{
        //    get { return m_splashScreen; }
        //}
        public GameTime ClientGameTime
        { get; set; }
        public static bool InGame
        { get { return Client.GameState.ToString().EndsWith("Game"); } }
        #endregion

        public Client()
        {
            m_graphics = new GraphicsDeviceManager(this);
            m_contentManager = new ContentManager(Services);
            m_guiManager = new GuiManager(this);
            Components.Add(m_guiManager);
            m_gameHUD = new GameHUD(this);
            Components.Add(m_gameHUD);
            m_audioManager = new Audio.AudioManager(this);
            Components.Add(m_audioManager);

            Deactivated += new EventHandler<EventArgs>(Client_Deactivated);
            Activated += new EventHandler<EventArgs>(Client_Activated);
            Disposed += Client_Disposed;

            Content.RootDirectory = "Content";
            m_firstFullScreen = true;
            m_noDraw = false;

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += Window_ClientSizeChanged;
        }

        private void Client_Disposed(object sender, EventArgs e)
        {
            OnExiting(sender, e);
        }

        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            //m_prevClientBounds = new Rectangle(Window.Position.X, Window.Position.Y, PreferredWindowWidth, PreferredWindowHeight);
            //m_preferredWindowHeight = Window.ClientBounds.Height;
            //m_preferredWindowWidth = Window.ClientBounds.Width;
            //m_height = Window.ClientBounds.Height;
            //m_width = Window.ClientBounds.Width;
            //m_graphics.PreferredBackBufferHeight = m_preferredWindowHeight;
            //m_graphics.PreferredBackBufferWidth = m_preferredWindowWidth;
            //m_graphics.ApplyChanges();

            //OnClientResize();
            if (Window.ClientBounds.Height < PreferredWindowHeight || Window.ClientBounds.Width < PreferredWindowWidth)
                m_hasFocus = false;
            else m_hasFocus = true;
        }

        private void Client_Activated(object sender, EventArgs e)
        {
            m_hasFocus = true;

            switch (GameState)
            {
                case Enums.EGameState.IOKGame:
                case Enums.EGameState.SpinelGame:
                case Enums.EGameState.YuushaGame:
                    if (GuiManager.CurrentSheet[Globals.GAMEINPUTTEXTBOX] is TextBox inputTextBox)
                    {
                        inputTextBox.HasFocus = true;
                        GuiManager.ActiveTextBox = inputTextBox.Name;
                    }
                    break;
            }
        }

        private void Client_Deactivated(object sender, EventArgs e)
        {
            m_hasFocus = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 100.0f);
            IsFixedTimeStep = false;

            //In some cases you want to call the Draw() method at maximum intensity.
            //m_graphics.SynchronizeWithVerticalRetrace = false;

            // set up the back buffer
            m_graphics.PreferredBackBufferWidth = m_preferredWindowWidth;
            m_graphics.PreferredBackBufferHeight = m_preferredWindowHeight;

            m_graphics.PreferredBackBufferFormat = m_preferredSurfaceFormat;
            m_graphics.PreferredDepthStencilFormat = m_preferredDepthFormat;

            m_graphics.ApplyChanges();

            // create necessary directories
            Utils.CreateDirectories();

            // load settings
            m_userSettings = UserSettings.Load();
            m_clientSettings = ClientSettings.Load();
            Character.LoadSettings();

            m_gameState = Enums.EGameState.Login;
            m_preferredWindowWidth = 1024;
            m_preferredWindowHeight = 768;

            //m_isFullScreen = UserSettings.FullScreen;

            Events.RegisterEvent(Events.EventName.Set_Login_State, Enums.ELoginState.Disconnected);
            Events.RegisterEvent(Events.EventName.Set_Game_State, Enums.EGameState.Login);
        
            Title = string.Format("{0} (v{1})", StaticSettings.ClientName, StaticSettings.ClientVersion);

            //m_graphics.SynchronizeWithVerticalRetrace = true;
            //m_graphics.GraphicsProfile = GraphicsProfile.HiDef;

            //if (ClientSettings.StartupInFullScreen)
            //{
            //    ToggleFullScreen();
            //}
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            //spriteBatch = new SpriteBatch(GraphicsDevice);
            // create sprite batch
            m_spriteBatch = new SpriteBatch(m_graphics.GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            ClientGameTime = gameTime;

            if ((m_preferredWindowWidth != m_graphics.PreferredBackBufferWidth ||
                m_preferredWindowHeight != m_graphics.PreferredBackBufferHeight) && !m_graphics.IsFullScreen)
            {
                m_graphics.PreferredBackBufferWidth = m_preferredWindowWidth;
                m_graphics.PreferredBackBufferHeight = m_preferredWindowHeight;
                m_graphics.ApplyChanges();
            }

            //if (GameState != Enums.eGameState.Splash && m_isFullScreen != UserSettings.FullScreen)
            //    m_isFullScreen = UserSettings.FullScreen;

            //if (m_isFullScreen != m_graphics.IsFullScreen)
            //    ToggleFullScreen();

            // allows the default game to exit on Xbox 360 and Windows
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            Window.Title = m_title;

            m_height = Window.ClientBounds.Height;
            m_width = Window.ClientBounds.Width;

            base.Update(gameTime);

            if (m_roundDelayStart != null && m_roundDelayStart.TotalMilliseconds - gameTime.TotalGameTime.TotalMilliseconds
                >= StaticSettings.RoundDelayLength)
                RoundDelay = false;

            if (RoundDelay)
                m_roundDelayStart = gameTime.TotalGameTime;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            m_graphics.GraphicsDevice.Clear(m_deviceClearColor);

            if (m_noDraw)
                return;

            //m_graphics.GraphicsProfile = GraphicsProfile.Reach;

            m_guiManager.Draw(gameTime);

            //m_graphics.GraphicsProfile = GraphicsProfile.HiDef;
            // draw effects like bloom

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            // remove this code and players who close the client without properly quitting the game are moved to karmaRes location
            if (IO.IsAlive && InGame)
            {
                IO.Send(Protocol.GAME_EXIT);
            }

            Microsoft.Xna.Framework.Media.MediaPlayer.Stop();
            //Microsoft.Xna.Framework.Media.MediaPlayer.Queue.ActiveSong.Dispose();

            // save client settings
            ClientSettings.Save();

            // save user settings
            if (Account.Name.Length > 0)
                UserSettings.Save();

            // save character settings
            if (Character.CurrentCharacter != null &&
                !string.IsNullOrEmpty(Character.CurrentCharacter.Name))
            {
                Character.Settings.Save();
                Character.FogOfWarSettings.Save();
                if (Character.GUIPositionSettings != null)
                    Character.GUIPositionSettings.Save();
            }

            // register disconnect event
            Events.RegisterEvent(Events.EventName.Disconnect);

            UnloadContent();

            Exit();             
        }

        public void ToggleFullScreen()
        {
            if(IsFullScreen && GameState == Enums.EGameState.HotButtonEditMode)
            {
                TextCue.AddClientInfoTextCue("Please exit Hot Button Edit Mode before switching from full screen.",
                    TextCue.TextCueTag.None, Color.Red, Color.Black, 255, 2000, false, false, true);
                return;
            }

            PresentationParameters presentation = m_graphics.GraphicsDevice.PresentationParameters;
            presentation.HardwareModeSwitch = true;

            m_prevClientBounds = Window.ClientBounds;

            if (presentation.IsFullScreen)
            {
                m_graphics.PreferredBackBufferWidth = m_preferredWindowWidth;
                m_graphics.PreferredBackBufferHeight = m_preferredWindowHeight;

                m_isFullScreen = false;

                //try { Window.IsBorderless = true; }
                //catch (NotImplementedException) { }
            }
            else
            {
                // going fullscreen, use desktop resolution to minimize display mode changes
                // this also has the nice effect of working around some displays that lie about 
                // supporting 1280x720
                GraphicsAdapter adapter = m_graphics.GraphicsDevice.Adapter;//CreationParameters.Adapter;

                m_graphics.PreferredBackBufferWidth = adapter.CurrentDisplayMode.Width;
                m_graphics.PreferredBackBufferHeight = adapter.CurrentDisplayMode.Height;

                m_prevClientPosition = Window.Position;

                m_isFullScreen = true;

                //try { Window.IsBorderless = false; }
                //catch (NotImplementedException) { }
            }

            m_graphics.ToggleFullScreen();

            if(!m_isFullScreen && m_prevClientPosition != null) // not working properly
                Window.Position = m_prevClientPosition;

            UserSettings.FullScreen = m_graphics.IsFullScreen;

            m_nowClientBounds = Window.ClientBounds;

            OnClientResize();

            // to correct issue with top 20 pixels inaccessible after first full screen toggle
            if (m_graphics.IsFullScreen && m_firstFullScreen)
            {
                m_noDraw = true;
                m_firstFullScreen = false;
                ToggleFullScreen();
                ToggleFullScreen();
                m_noDraw = false;
            }
        }

        public static void OnClientResize()
        {
            foreach (Sheet sheet in GuiManager.Sheets.Values)
                sheet.OnClientResize(m_prevClientBounds, m_nowClientBounds);

            // resize generic sheet
            GuiManager.GenericSheet.OnClientResize(m_prevClientBounds, m_nowClientBounds);

            foreach (TextCue tc in GuiManager.TextCues)
                tc.OnClientResize(m_prevClientBounds, m_nowClientBounds);
        }
    }
}