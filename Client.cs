using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        private static ClientSettings m_clientSettings = new ClientSettings();
        private static int m_preferredWindowWidth = 1280;
        private static int m_preferredWindowHeight = 720;
        private static SurfaceFormat m_preferredSurfaceFormat = SurfaceFormat.Color;
        private static DepthFormat m_preferredDepthFormat = DepthFormat.Depth24;
        private static Color m_deviceClearColor = Color.Black;
        private static bool m_isFullScreen = false;
        private static Rectangle m_prevClientBounds;
        private static Rectangle m_nowClientBounds;
        private static Point m_prevClientPosition;
        private static TimeSpan m_lastPing;
        private static TimeSpan m_totalGameTime;

        GraphicsDeviceManager m_graphics;
        ContentManager m_content;
        Yuusha.gui.GuiManager m_guiManager;
        //Yuusha.gui.SplashScreen m_splashScreen;
        #endregion

        #region Public Properties
        public static UserSettings UserSettings
        {
            get { return m_userSettings; }
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
        public static TimeSpan TotalGameTime
        {
            get { return m_totalGameTime; }
        }
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
        #endregion

        public Client()
        {
            m_graphics = new GraphicsDeviceManager(this);
            m_content = new ContentManager(Services);
            m_guiManager = new GuiManager(this);
            Components.Add(m_guiManager);
            //m_splashScreen = new Yuusha.gui.SplashScreen(this);
            //Components.Add(m_splashScreen);
            Deactivated += new EventHandler<EventArgs>(Client_Deactivated);
            Activated += new EventHandler<EventArgs>(Client_Activated);

            Content.RootDirectory = "Content";
        }

        void Client_Activated(object sender, EventArgs e)
        {
            m_hasFocus = true;

            switch (GameState)
            {
                case Enums.EGameState.IOKGame:
                    Control iokTextBox = GuiManager.CurrentSheet[Globals.GAMEINPUTTEXTBOX];
                    if (iokTextBox != null)
                    {
                        iokTextBox.HasFocus = true;
                        GuiManager.ActiveTextBox = iokTextBox.Name;
                    }
                    break;
                case Enums.EGameState.SpinelGame:
                    Control spinelTextBox = GuiManager.CurrentSheet[Globals.GAMEINPUTTEXTBOX];
                    if (spinelTextBox != null)
                    {
                        spinelTextBox.HasFocus = true;
                        GuiManager.ActiveTextBox = spinelTextBox.Name;
                    }
                    break;
            }
        }

        void Client_Deactivated(object sender, EventArgs e)
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

            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 100.0f);
            this.IsFixedTimeStep = false;

            //In some cases you want to call the Draw() method at maximum intensity.
            //m_graphics.SynchronizeWithVerticalRetrace = false;

            // set up the back buffer
            m_graphics.PreferredBackBufferWidth = m_preferredWindowWidth;
            m_graphics.PreferredBackBufferHeight = m_preferredWindowHeight;

            m_graphics.PreferredBackBufferFormat = m_preferredSurfaceFormat;
            m_graphics.PreferredDepthStencilFormat = m_preferredDepthFormat;

            m_graphics.ApplyChanges();
            
            // initialize sound
            Sound.Initialize();

            // create sprite batch
            m_spriteBatch = new SpriteBatch(this.m_graphics.GraphicsDevice);

            // create necessary directories
            Utils.CreateDirectories();

            // load settings
            m_userSettings = UserSettings.Load();
            m_clientSettings = ClientSettings.Load();
            Character.LoadSettings(); // loads default values

            // set initial game state
            //if (false)//(Client.ClientSettings.ShowSplash)
            //{
            //    m_gameState = Enums.eGameState.Splash;
            //    m_preferredWindowWidth = 600;
            //    m_preferredWindowHeight = 406;
            //}
            //else
            //{
                m_gameState = Enums.EGameState.Login;
                //m_splashScreen.Enabled = false;
                m_preferredWindowWidth = 1024;
                m_preferredWindowHeight = 768;
                m_isFullScreen = UserSettings.FullScreen;

                Events.RegisterEvent(Events.EventName.Set_Login_State, Enums.ELoginState.Disconnected);
                Events.RegisterEvent(Events.EventName.Set_Game_State, Enums.EGameState.Login);
            //}
        
            // set initial title
            Client.Title = string.Format("{0} (v{1})", StaticSettings.ClientName, StaticSettings.ClientVersion);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            //spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            this.ClientGameTime = gameTime;

            // update game time
            m_totalGameTime = gameTime.TotalGameTime;

            if ((m_preferredWindowWidth != m_graphics.PreferredBackBufferWidth ||
                m_preferredWindowHeight != m_graphics.PreferredBackBufferHeight) && !m_graphics.IsFullScreen)
            {
                m_graphics.PreferredBackBufferWidth = m_preferredWindowWidth;
                m_graphics.PreferredBackBufferHeight = m_preferredWindowHeight;
                m_graphics.ApplyChanges();
            }

            //if (GameState != Enums.eGameState.Splash && m_isFullScreen != UserSettings.FullScreen)
            //    m_isFullScreen = UserSettings.FullScreen;

            if (m_isFullScreen != m_graphics.IsFullScreen)
                ToggleFullScreen();

            // allows the default game to exit on Xbox 360 and Windows
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            this.Window.Title = m_title;

            m_height = Window.ClientBounds.Height;
            m_width = Window.ClientBounds.Width;

            Sound.Update(gameTime);

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

            //if (false)//(m_gameState == Enums.eGameState.Splash)
            //    //m_splashScreen.Draw();
            //else
                m_guiManager.Draw(gameTime);

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            // save client settings
            ClientSettings.Save();

            // save user settings
            if (Account.Name.Length > 0)
            {
                UserSettings.Save();
            }

            // save character settings
            if (Character.CurrentCharacter != null &&
                Character.CurrentCharacter.Name != null &&
                Character.CurrentCharacter.Name.Length > 0)
            {
                Character.Settings.Save();
            }

            // register disconnect event
            Events.RegisterEvent(Events.EventName.Disconnect);

            // shut down sound
            Sound.Shutdown();

            // exit
            this.Exit();
             
        }

        public void ToggleFullScreen()
        {
            if(Client.IsFullScreen && Client.GameState == Enums.EGameState.HotButtonEditMode)
            {
                TextCue.AddClientInfoTextCue("Please exit Hot Button Edit Mode before switching from full screen.",
                    TextCue.TextCueTag.None, Color.Red, Color.Black, 2000, false, false, true);
                return;
            }

            PresentationParameters presentation = m_graphics.GraphicsDevice.PresentationParameters;

            m_prevClientBounds = Window.ClientBounds;
            

            if (presentation.IsFullScreen)
            {   // going windowed

                m_graphics.PreferredBackBufferWidth = m_preferredWindowWidth;
                m_graphics.PreferredBackBufferHeight = m_preferredWindowHeight;

                m_isFullScreen = false;
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
            }

            m_graphics.ToggleFullScreen();

            if(!m_isFullScreen && m_prevClientPosition != null) // not working properly
                Window.Position = m_prevClientPosition;

            UserSettings.FullScreen = m_graphics.IsFullScreen;

            m_nowClientBounds = Window.ClientBounds;

            OnClientResize();
        }

        public static void OnClientResize()
        {
            foreach (gui.Sheet sheet in gui.GuiManager.Sheets.Values)
                sheet.OnClientResize(m_prevClientBounds, m_nowClientBounds);

            // resize generic sheet
            gui.GuiManager.GenericSheet.OnClientResize(m_prevClientBounds, m_nowClientBounds);
        }
    }
}