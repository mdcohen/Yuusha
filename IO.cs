using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Color = Microsoft.Xna.Framework.Color;

namespace Yuusha
{
    public static class IO
    {
        #region Private Data
        static TcpClient m_TcpClient = null;
        static string m_InData = null;
        static char[] m_buffer = null;
        static StreamReader m_StreamReader = null;
        static StreamWriter m_StreamWriter = null;
        static Enums.ELoginState m_loginState;
        static TimeSpan m_pingScore;
        #endregion

        #region Public Properties
        public static Enums.ELoginState LoginState
        {
            get { return m_loginState; }
            set
            {
                m_loginState = value;
            }
        }

        public static bool IsAlive
        {
            get
            {
                if (m_TcpClient != null && m_TcpClient.Client.Connected)
                {
                    return true;
                }
                return false;
            }
        } 
        #endregion

        public static void Run()
        {
            while (IO.IsAlive)
            {
                if (m_TcpClient.Available > 0)
                {
                    m_buffer = new char[m_TcpClient.Available];

                    m_StreamReader.Read(m_buffer, 0, m_buffer.Length);

                    for (int a = 0; a < m_buffer.Length; a++)
                    {
                        m_InData += m_buffer[a].ToString();

                        if (LoginState > Enums.ELoginState.VerifyPassword)
                        {
                            if (m_InData.Contains(Convert.ToString((char)27)))
                            {
                                if (CaptureInput(m_InData))
                                {
                                    m_InData = "";

                                    //if (LoginState == Enums.eLoginState.LoggedIn)
                                    //    IO.SendPing(Client.TotalGameTime);
                                }
                            }
                        }
                        else
                        {
                            if (CaptureInput(m_InData))
                            {
                                m_InData = "";
                            }
                        }
                    }
                }

                System.Threading.Thread.Sleep(100);
            }

            Events.RegisterEvent(Events.EventName.Set_Login_State, Enums.ELoginState.Disconnected);
            Events.RegisterEvent(Events.EventName.Set_Game_State, Enums.EGameState.Login);

        }

        private static void ConnectCallback(IAsyncResult asyncResult)
        {
            try
            {
                NetworkStream socketStream = m_TcpClient.GetStream();

                m_StreamReader = new StreamReader(socketStream);
                m_StreamWriter = new StreamWriter(socketStream)
                {
                    AutoFlush = true
                };
            }
            catch
            {
                return;
            }
            
            IO.Run();
        }

        /// <summary>
        /// Tests internet connectivity.
        /// </summary>
        /// <returns>False if not connected or exception.</returns>
        private static bool TestConnection()
        {
            try
            {
                IPHostEntry ipHe = Dns.GetHostEntry("www.google.com");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool Connect()
        {
            if (!TestConnection())
                return false;

            try
            {
                m_TcpClient = new TcpClient();
                m_TcpClient.BeginConnect(Client.ClientSettings.ServerHost, Client.ClientSettings.ServerPort, new AsyncCallback(ConnectCallback), m_TcpClient);

                return true;
            }
            catch (SocketException)
            {
                return false;
            }
            catch (Exception e)
            {
                Utils.LogException(e);
                return false;
            }
        }

        public static void Disconnect()
        {
            if (m_TcpClient != null)
            {
                try
                {
                    m_TcpClient.Client.Disconnect(false);
                }
                catch { }
                m_TcpClient = null;
            }
        }

        public static void Send(string outData)
        {
            if (outData == null || outData == "") return;

            if (IO.IsAlive)
            {
                outData = outData + "\r"; // add a carriage return to outbound data
                try
                {
                    if(!Client.RoundDelay)
                        m_StreamWriter.Write(outData);
                    if (Client.GameState.ToString().EndsWith("Game") && Utility.Settings.StaticSettings.RoundDelayEnabled)
                        Client.RoundDelay = true;
                }
                catch
                {
                    // nothing
                }
            }
            else
            {
                Events.RegisterEvent(Events.EventName.Set_Login_State, Enums.ELoginState.Disconnected);
                Events.RegisterEvent(Events.EventName.Set_Game_State, Enums.EGameState.Login);
            }
        }

        public static void SendPing(TimeSpan totalGameTime)
        {
            Client.LastPing = totalGameTime;
            IO.Send(Protocol.PING);
        }

        public static bool CaptureInput(string inData)
        {
            #region Detect Logout
            if (inData.IndexOf(Protocol.LOGOUT) != -1)
            {
                Events.RegisterEvent(Events.EventName.Disconnect);
                return true;
            }
            #endregion

            #region Detect Ping Response
            //else if (inData.IndexOf(Protocol.PING) != -1)
            //{
            //    m_pingScore = Client.TotalGameTime - Client.LastPing;
            //    if (gui.GuiManager.GenericSheet["PingResponseLabel"] != null)
            //        gui.GuiManager.GenericSheet["PingResponseLabel"].Text = string.Format("{0}ms", m_pingScore.TotalMilliseconds.ToString());
            //} 
            #endregion

            #region Detect Main Menu
            else if (inData.IndexOf(Protocol.MENU_MAIN) != -1)
            {
                Events.RegisterEvent(Events.EventName.Set_Login_State, Enums.ELoginState.LoggedIn);
                Events.RegisterEvent(Events.EventName.Set_Game_State, Enums.EGameState.Menu);
                return true;
            }
            #endregion

            #region Detect Game Enter
            else if (inData.IndexOf(Protocol.GAME_ENTER) != -1)
            {
                Events.RegisterEvent(Events.EventName.Set_Game_State, Enums.EGameState.Game);
                return true;
            }
            #endregion

            #region Detect Conference Enter
            else if (inData.IndexOf(Protocol.CONF_ENTER) != -1)
            {
                Events.RegisterEvent(Events.EventName.Set_Game_State, Enums.EGameState.Conference);
                return true;
            }
            #endregion

            #region Detect Message Box
            else if (inData.IndexOf(Protocol.MESSAGEBOX_END) != -1)
            {
                Protocol.DisplayMessageBox(Protocol.GetProtoInfoFromString(inData, Protocol.MESSAGEBOX, Protocol.MESSAGEBOX_END));
                return true;
            }
            #endregion

            #region Detect News
            else if (inData.IndexOf(Protocol.NEWS_END) != -1)
            {
                World.AddNews(Protocol.GetProtoInfoFromString(inData, Protocol.NEWS, Protocol.NEWS_END));
                return true;
            }
            #endregion

            #region Detect Sound
            else if (inData.IndexOf(Protocol.SOUND_END) != -1)
            {
                if (Client.UserSettings.SoundEffects)
                {
                    string[] soundInfo = Protocol.GetProtoInfoFromString(inData, Protocol.SOUND, Protocol.SOUND_END).Split(Protocol.VSPLIT.ToCharArray());
                    Sound.Play(soundInfo);
                }
                return true;
            }
            #endregion

            #region Detect CharGen Enter
            else if (inData.IndexOf(Protocol.CHARGEN_ENTER) != -1)
            {
                if(IO.LoginState != Enums.ELoginState.LoggedIn)
                    Events.RegisterEvent(Events.EventName.Set_Login_State, Enums.ELoginState.LoggedIn);
                if (Client.GameState != Enums.EGameState.CharacterGeneration)
                    Events.RegisterEvent(Events.EventName.Set_Game_State, Enums.EGameState.CharacterGeneration);
                return true;
            }
            #endregion

            #region Detect Users List
            else if (inData.IndexOf(Protocol.WORLD_USERS_END) != -1)
            {
                World.GatherWorldData(Protocol.GetProtoInfoFromString(inData, Protocol.WORLD_USERS, Protocol.WORLD_USERS_END), World.WorldUpdate.Users);
                return true;
            }
            #endregion

            #region Detect Character List
            else if (inData.IndexOf(Protocol.CHARACTER_LIST_END) != -1)
            {
                Character.GatherCharacterList(Protocol.GetProtoInfoFromString(inData, Protocol.CHARACTER_LIST, Protocol.CHARACTER_LIST_END));
                return true;
            }
            #endregion

            #region Detect Account Info
            else if (inData.IndexOf(Protocol.ACCOUNT_INFO_END) != -1)
            {
                Account.SetAccountInfo(Protocol.GetProtoInfoFromString(inData, Protocol.ACCOUNT_INFO, Protocol.ACCOUNT_INFO_END));
                return true;
            } 
            #endregion

            #region Detect Current Character ID
            else if (inData.IndexOf(Protocol.CHARACTER_CURRENT_END) != -1)
            {
                int id = Convert.ToInt32(Protocol.GetProtoInfoFromString(inData, Protocol.CHARACTER_CURRENT, Protocol.CHARACTER_CURRENT_END));

                if (Character.CurrentCharacter == null || id != Character.CurrentCharacter.ID)
                {
                    Character.CurrentCharacter = Account.GetCharacterByID(Convert.ToInt32(Protocol.GetProtoInfoFromString(inData, Protocol.CHARACTER_CURRENT, Protocol.CHARACTER_CURRENT_END)));
                }

                Character.LoadSettings();
                gui.GenericSheet.LoadMacros();

                return true;
            }
            #endregion

            #region Detect Scores
            else if (inData.IndexOf(Protocol.WORLD_SCORES_END) != -1)
            {
                World.GatherWorldData(Protocol.GetProtoInfoFromString(inData, Protocol.WORLD_SCORES, Protocol.WORLD_SCORES_END), World.WorldUpdate.Scores);
                return true;
            }
            #endregion

            #region Detect Implementor Information
            else if (inData.IndexOf(Protocol.IMP_CHARACTERFIELDS_END) != -1)
            {
                //Implementor.gatherImplementorData(Protocol.GetProtoInfoFromString(inData, Protocol.IMP_CHARACTERFIELDS, Protocol.IMP_CHARACTERFIELDS_END), Implementor.ImplementorUpdate.CharacterFields);
                return true;
            }
            #endregion

            #region Detect Macros -- Gather this data only if it is not coming from a character list.
            else if (inData.IndexOf(Protocol.CHARACTER_LIST) == -1 && inData.IndexOf(Protocol.CHARACTER_MACROS_END) != -1 && Character.CurrentCharacter != null)
            {
                Character.GatherCharacterData(Protocol.GetProtoInfoFromString(inData, Protocol.CHARACTER_MACROS, Protocol.CHARACTER_MACROS_END), Enums.EPlayerUpdate.Macros);
                return true;
            } 
            #endregion

            else
            {
                if (IO.LoginState != Enums.ELoginState.LoggedIn)
                {
                    #region Detect Login Information
                    switch (IO.LoginState)
                    {
                        case Enums.ELoginState.Disconnected:
                            break;
                        case Enums.ELoginState.NewAccount:                            
                            #region NewAccount -- All steps to verify new account here. Next is CharGen.
                            // Enter "new"
                            if (inData.ToLower().IndexOf("login:") != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Set_Login_Status_Label, "Setting up your new account...", "White");
                                IO.Send("new");
                                IO.Send(CharGen.NewAccountName);
                                IO.Send(CharGen.NewAccountEmail);
                                IO.Send(CharGen.NewAccountEmail);
                                IO.Send(CharGen.NewAccountPassword);
                                IO.Send(CharGen.NewAccountPassword);
                                return true;
                            }
                            // PROBLEMS
                            // That name is invalid.
                            if (inData.IndexOf("That name is invalid.") != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Set_Login_Status_Label, "Account name is invalid. It may already be in use.", "Red");
                                return true;
                            }
                            // That password is invalid.
                            if (inData.IndexOf("That password is invalid.") != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Set_Login_Status_Label, "Please use a different password.", "Red");
                                return true;
                            }
                            // CHARGEN
                            // "Welcome to the character generator."
                            if (inData.IndexOf("Welcome to the character generator.") != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Set_Login_Status_Label, "Welcome to the character generator.", "Lime");
                                Events.RegisterEvent(Events.EventName.Set_Login_State, Enums.ELoginState.LoggedIn);
                                Events.RegisterEvent(Events.EventName.Set_Game_State, Enums.EGameState.CharacterGeneration);
                                return true;
                            }
                            break;
                        #endregion
                        case Enums.ELoginState.Connected:
                            #region Connected
                            if (inData.ToLower().IndexOf("login:") != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Set_Login_State, Enums.ELoginState.VerifyAccount);
                                Events.RegisterEvent(Events.EventName.Set_Login_Status_Label, "Verifying account information...", "White");
                                Events.RegisterEvent(Events.EventName.Send_Account_Name);
                                return true;
                            }
                            break;
                            #endregion
                        case Enums.ELoginState.VerifyAccount:
                            #region VerifyAccount
                            if (inData.ToLower().IndexOf("password:") != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Set_Login_State, Enums.ELoginState.VerifyPassword);
                                Events.RegisterEvent(Events.EventName.Send_Password);
                                return true;
                            }
                            else if (inData.IndexOf("That account name was not found") != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Disconnect);
                                Events.RegisterEvent(Events.EventName.Set_Login_Status_Label, "Account does not exist.", "Red");
                                return true;
                            }
                            break;
                            #endregion
                        case Enums.ELoginState.VerifyPassword:
                            #region VerifyPassword
                            if (inData.IndexOf("Invalid password.") != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Disconnect);
                                Events.RegisterEvent(Events.EventName.Set_Login_Status_Label, "Invalid password.", "Red");
                                return true;
                            }
                            else if (inData.IndexOf("That account is already logged in") != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Disconnect);
                                Events.RegisterEvent(Events.EventName.Set_Login_Status_Label, "Account already logged in.", "Yellow");
                                return true;
                            }
                            else if (inData.IndexOf(Protocol.DETECT_CLIENT) != -1)
                            {
                                IO.Send(Protocol.SET_CLIENT);
                                return true;
                            }
                            else if (inData.IndexOf(Protocol.DETECT_PROTOCOL) != -1)
                            {
                                IO.Send(Protocol.SET_PROTOCOL);
                                return true;
                            }
                            else if (inData.IndexOf(Protocol.WORLD_INFORMATION) != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Set_Login_Status_Label, "Receiving world data...", "White");
                                Events.RegisterEvent(Events.EventName.Set_Login_State, Enums.ELoginState.WorldInformation);
                                return true;
                            }
                            // "Welcome to the character generator."
                            else if (inData.IndexOf("There are no characters presently on this account") != -1)
                            {
                                CharGen.FirstCharacter = true;
                                if (IO.LoginState != Enums.ELoginState.LoggedIn)
                                    Events.RegisterEvent(Events.EventName.Set_Login_State, Enums.ELoginState.LoggedIn);
                                if (Client.GameState != Enums.EGameState.CharacterGeneration)
                                    Events.RegisterEvent(Events.EventName.Set_Game_State, Enums.EGameState.CharacterGeneration);                                
                                return true;
                            }
                            else if (inData.IndexOf("Welcome to the character generator.") != -1)
                            {
                                if(IO.LoginState != Enums.ELoginState.LoggedIn)
                                    Events.RegisterEvent(Events.EventName.Set_Login_State, Enums.ELoginState.LoggedIn);
                                if(Client.GameState != Enums.EGameState.CharacterGeneration)
                                    Events.RegisterEvent(Events.EventName.Set_Game_State, Enums.EGameState.CharacterGeneration);                                
                                return true;
                            }
                            break;
                            #endregion
                        case Enums.ELoginState.WorldInformation:
                            try
                            {
                                #region WorldInformation
                                if (inData.IndexOf(Protocol.VERSION_SERVER_END) != -1)
                                {
                                    Utility.Settings.StaticSettings.ServerVersion = Protocol.GetProtoInfoFromString(inData, Protocol.VERSION_SERVER, Protocol.VERSION_SERVER_END);
                                    return true;
                                }

                                // capture world lands
                                else if (inData.IndexOf(Protocol.WORLD_LANDS_END) != -1)
                                {
                                    World.GatherWorldData(Protocol.GetProtoInfoFromString(inData, Protocol.WORLD_LANDS, Protocol.WORLD_LANDS_END), World.WorldUpdate.Lands);
                                    return true;
                                }
                                // capture world maps
                                else if (inData.IndexOf(Protocol.WORLD_MAPS_END) != -1)
                                {
                                    World.GatherWorldData(Protocol.GetProtoInfoFromString(inData, Protocol.WORLD_MAPS, Protocol.WORLD_MAPS_END), World.WorldUpdate.Maps);
                                    return true;
                                }
                                // capture world spells
                                else if (inData.IndexOf(Protocol.WORLD_SPELLS_END) != -1)
                                {
                                    World.GatherWorldData(Protocol.GetProtoInfoFromString(inData, Protocol.WORLD_SPELLS, Protocol.WORLD_SPELLS_END), World.WorldUpdate.Spells);
                                    return true;
                                }
                                // capture newbie chargen info
                                else if (inData.IndexOf(Protocol.WORLD_CHARGEN_INFO_END) != -1)
                                {
                                    World.GatherWorldData(Protocol.GetProtoInfoFromString(inData, Protocol.WORLD_CHARGEN_INFO, Protocol.WORLD_CHARGEN_INFO_END), World.WorldUpdate.CharGen);
                                    return true;
                                }
                                #endregion
                            }
                            catch (Exception e)
                            { Utils.LogException(e); }
                            break;
                    }
                    #endregion
                }
                else
                {
                    switch (Client.GameState)
                    {
                        case Enums.EGameState.Menu:
                            break;
                        case Enums.EGameState.CharacterGeneration:
                            #region CharGen
                            if(inData.ToLower().IndexOf("that was not an option") != -1)
                            {
                                gui.TextCue.AddClientInfoTextCue("That was not an option.", gui.TextCue.TextCueTag.None, Color.Tomato, Color.Black, 1500, false, false, true);
                                return true;
                            }
                            else if(inData.ToLower().IndexOf("please select a gender:") != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Set_CharGen_State, Enums.ECharGenState.ChooseGender);
                                return true;
                            }
                            else if(inData.ToLower().IndexOf("please select a race:") != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Set_CharGen_State, Enums.ECharGenState.ChooseHomeland);
                                return true;
                            }
                            else if(inData.ToLower().IndexOf("select a character class:") != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Set_CharGen_State, Enums.ECharGenState.ChooseProfession);
                                return true;
                            }
                            else if(inData.ToLower().IndexOf("roll again? (y,n):") != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Set_CharGen_State, Enums.ECharGenState.ReviewStats, inData);
                                return true;
                            }
                            else if(inData.ToLower().IndexOf("please enter a name for your character:") != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Set_CharGen_State, Enums.ECharGenState.ChooseName);
                                return true;
                            }
                            //else if(inData.ToLower().IndexOf("a character with the name you have chosen already exists") != -1)
                            //{

                            //}
                            else if(inData.ToLower().IndexOf("that name is invalid.") != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Set_CharGen_State, Enums.ECharGenState.ChooseName);
                                return true;
                            }
                            //Events.RegisterEvent(Events.EventName.Display_CharGen_Text, inData);
                            //if (inData.IndexOf(Protocol.CHARGEN_ROLLER_RESULTS_END) != -1)
                            //{
                            //    //Essence.client.chargen.ParseRollerResults(Protocol.GetProtoInfoFromString(inData, Protocol.CHARGEN_ROLLER_RESULTS, Protocol.CHARGEN_ROLLER_RESULTS_END));
                            //    return true;
                            //}
                            //else if (inData.IndexOf(Protocol.CHARGEN_ACCEPTED) != -1)
                            //{
                            //    //Essence.client.chargen.AcceptStepOne();
                            //    return true;
                            //}
                            //else if (inData.IndexOf(Protocol.CHARGEN_ERROR) != -1)
                            //{
                            //    //Essence.client.chargen.sendCharGenInfo();
                            //    return true;
                            //}
                            //else if (inData.IndexOf(Protocol.CHARGEN_INVALIDNAME) != -1)
                            //{
                            //    //Essence.client.chargen.DenyStepOne();
                            //    return true;
                            //}
                            break;
                            #endregion
                        case Enums.EGameState.Conference:
                            #region Conference
                            // conference information
                            if (inData.IndexOf(Protocol.CONF_INFO_END) != -1)
                            {
                                //string[] info = Protocol.GetProtoInfoFromString(inData, Protocol.CONF_INFO, Protocol.CONF_INFO_END).Split(Protocol.ISPLIT.ToCharArray());
                                return true;
                            }
                            // header
                            else if (inData.IndexOf(Protocol.TEXT_HEADER_END) != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Display_Conference_Text, Protocol.GetProtoInfoFromString(inData, Protocol.TEXT_HEADER, Protocol.TEXT_HEADER_END), Enums.ETextType.Header);
                                return true;
                            }
                            // chat text
                            else if (inData.IndexOf(Protocol.TEXT_PLAYERCHAT_END) != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Display_Conference_Text, Protocol.GetProtoInfoFromString(inData, Protocol.TEXT_PLAYERCHAT, Protocol.TEXT_PLAYERCHAT_END), Enums.ETextType.PlayerChat);
                                return true;
                            }
                            // status text
                            else if (inData.IndexOf(Protocol.TEXT_STATUS_END) != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Display_Conference_Text, Protocol.GetProtoInfoFromString(inData, Protocol.TEXT_STATUS, Protocol.TEXT_STATUS_END), Enums.ETextType.Status);
                                return true;
                            }
                            // private messages
                            else if (inData.IndexOf(Protocol.TEXT_PRIVATE_END) != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Display_Conference_Text, Protocol.GetProtoInfoFromString(inData, Protocol.TEXT_PRIVATE, Protocol.TEXT_PRIVATE_END), Enums.ETextType.Private);
                                return true;
                            }
                            // enter messages
                            else if (inData.IndexOf(Protocol.TEXT_ENTER_END) != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Display_Conference_Text, Protocol.GetProtoInfoFromString(inData, Protocol.TEXT_ENTER, Protocol.TEXT_ENTER_END), Enums.ETextType.Enter);
                                return true;
                            }
                            // exit messages
                            else if (inData.IndexOf(Protocol.TEXT_EXIT_END) != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Display_Conference_Text, Protocol.GetProtoInfoFromString(inData, Protocol.TEXT_EXIT, Protocol.TEXT_EXIT_END), Enums.ETextType.Exit);
                                return true;
                            }
                            // system messages
                            else if (inData.IndexOf(Protocol.TEXT_SYSTEM_END) != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Display_Conference_Text, Protocol.GetProtoInfoFromString(inData, Protocol.TEXT_SYSTEM, Protocol.TEXT_SYSTEM_END), Enums.ETextType.System);
                                return true;
                            }
                            // help messages
                            else if (inData.IndexOf(Protocol.TEXT_HELP_END) != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Display_Conference_Text, Protocol.GetProtoInfoFromString(inData, Protocol.TEXT_HELP, Protocol.TEXT_HELP_END), Enums.ETextType.Help);
                                return true;
                            }
                            // list messages
                            else if (inData.IndexOf(Protocol.TEXT_LISTING_END) != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Display_Conference_Text, Protocol.GetProtoInfoFromString(inData, Protocol.TEXT_LISTING, Protocol.TEXT_LISTING_END), Enums.ETextType.Listing);
                                return true;
                            }
                            // error messages
                            else if (inData.IndexOf(Protocol.TEXT_ERROR_END) != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Display_Conference_Text, Protocol.GetProtoInfoFromString(inData, Protocol.TEXT_ERROR, Protocol.TEXT_ERROR_END), Enums.ETextType.Error);
                                return true;
                            }
                            // friend messages
                            else if (inData.IndexOf(Protocol.TEXT_FRIEND_END) != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Display_Conference_Text, Protocol.GetProtoInfoFromString(inData, Protocol.TEXT_FRIEND, Protocol.TEXT_FRIEND_END), Enums.ETextType.Friend);
                                return true;
                            }
                            // page
                            else if (inData.IndexOf(Protocol.TEXT_PAGE_END) != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Display_Conference_Text, Protocol.GetProtoInfoFromString(inData, Protocol.TEXT_PAGE, Protocol.TEXT_PAGE_END), Enums.ETextType.Page);
                                return true;
                            }
                            // character stats update from conference
                            else if (inData.IndexOf(Protocol.CHARACTER_STATS_END) != -1)
                            {
                                Character.GatherCharacterData(Protocol.GetProtoInfoFromString(inData, Protocol.CHARACTER_STATS, Protocol.CHARACTER_STATS_END), Enums.EPlayerUpdate.Stats);
                                return true;
                            }
                            break;
                            #endregion
                        case Enums.EGameState.SpinelGame:
                        case Enums.EGameState.IOKGame:
                        case Enums.EGameState.LOKGame:
                        case Enums.EGameState.Game:
                            #region Game
                            #region GAME_NEW_ROUND
                            if (inData.IndexOf(Protocol.GAME_NEW_ROUND) != -1)
                            {
                                Events.RegisterEvent(Events.EventName.New_Game_Round);
                                return true;
                            }
                            #endregion
                            else if (inData.IndexOf(Protocol.GAME_CHARACTER_DEATH) != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Character_Death, inData.Substring(0, inData.IndexOf(Protocol.GAME_CHARACTER_DEATH)));
                            }
                            #region GAME_POINTER_UPDATE
                            else if (inData.IndexOf(Protocol.GAME_POINTER_UPDATE) != -1)
                            {
                                Character.CurrentCharacter.directionPointer = inData.Substring(inData.IndexOf(Protocol.GAME_POINTER_UPDATE) - 1, 1);
                                return true;
                            }
                            #endregion
                            #region GAME_END_ROUND
                            else if (inData.IndexOf(Protocol.GAME_END_ROUND) != -1)
                            {
                                Events.RegisterEvent(Events.EventName.End_Game_Round);
                                return true;
                            }
                            #endregion
                            #region GAME_TEXT_END
                            else if (inData.IndexOf(Protocol.GAME_TEXT_END) != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Display_Game_Text, Protocol.GetProtoInfoFromString(inData, Protocol.GAME_TEXT, Protocol.GAME_TEXT_END));
                                return true;
                            }
                            #endregion
                            #region GAME_CELL_END
                            else if (inData.IndexOf(Protocol.GAME_CELL_END) != -1)
                            {
                                Events.RegisterEvent(Events.EventName.Format_Cell, Protocol.GetProtoInfoFromString(inData, Protocol.GAME_CELL, Protocol.GAME_CELL_END));
                                return true;
                            }
                            #endregion
                            #region CHARACTER_HITS_UPDATE_END
                            else if (inData.IndexOf(Protocol.CHARACTER_HITS_UPDATE_END) != -1)
                            {
                                Character.GatherCharacterData(Protocol.GetProtoInfoFromString(inData, Protocol.CHARACTER_HITS_UPDATE, Protocol.CHARACTER_HITS_UPDATE_END), Enums.EPlayerUpdate.Hits);
                                return true;
                            }
                            #endregion
                            #region CHARACTER_STAMINA_UPDATE_END
                            else if (inData.IndexOf(Protocol.CHARACTER_STAMINA_UPDATE_END) != -1)
                            {
                                Character.GatherCharacterData(Protocol.GetProtoInfoFromString(inData, Protocol.CHARACTER_STAMINA_UPDATE, Protocol.CHARACTER_STAMINA_UPDATE_END), Enums.EPlayerUpdate.Stamina);
                                return true;
                            }
                            #endregion
                            #region CHARACTER_MANA_UPDATE_END
                            else if (inData.IndexOf(Protocol.CHARACTER_MANA_UPDATE_END) != -1)
                            {
                                Character.GatherCharacterData(Protocol.GetProtoInfoFromString(inData, Protocol.CHARACTER_MANA_UPDATE, Protocol.CHARACTER_MANA_UPDATE_END), Enums.EPlayerUpdate.Mana);
                                return true;
                            }
                            #endregion
                            else if (inData.IndexOf(Protocol.CHARACTER_EXPERIENCE_END) != -1)
                            {
                                Character.GatherCharacterData(Protocol.GetProtoInfoFromString(inData, Protocol.CHARACTER_EXPERIENCE, Protocol.CHARACTER_EXPERIENCE_END), Enums.EPlayerUpdate.Experience);
                                return true;
                            }
                            else if (inData.IndexOf(Protocol.CHARACTER_PROMPT_STATE_END) != -1)
                            {
                                string promptStateInfo = Protocol.GetProtoInfoFromString(inData, Protocol.CHARACTER_PROMPT_STATE, Protocol.CHARACTER_PROMPT_STATE_END);

                                if (promptStateInfo.ToLower() == Protocol.PromptStates.Normal.ToString().ToLower())
                                {
                                    foreach (Yuusha.gui.TextCue tc in new System.Collections.Generic.List<gui.TextCue>(gui.GuiManager.TextCues))
                                    {
                                        if (tc.Tag == gui.TextCue.TextCueTag.PromptState)
                                        {
                                            gui.GuiManager.TextCues.Remove(tc);
                                        }
                                    }
                                    return true;
                                }

                                Microsoft.Xna.Framework.Color cueColor = Microsoft.Xna.Framework.Color.Gold;
                                Microsoft.Xna.Framework.Color backgroundColor = Microsoft.Xna.Framework.Color.Transparent;
                                Protocol.PromptStates promptState = (Protocol.PromptStates)Enum.Parse(typeof(Protocol.PromptStates), promptStateInfo, true);

                                gui.TextCue.AddPromptStateTextCue(promptState);
                            }
                            #region CHARACTER_STATS_END
                            else if (inData.IndexOf(Protocol.CHARACTER_STATS_END) != -1)
                            {
                                Character.GatherCharacterData(Protocol.GetProtoInfoFromString(inData, Protocol.CHARACTER_STATS, Protocol.CHARACTER_STATS_END), Enums.EPlayerUpdate.Stats);
                                return true;
                            }
                            #endregion
                            #region CHARACTER_RIGHTHAND_END
                            else if (inData.IndexOf(Protocol.CHARACTER_RIGHTHAND_END) != -1)
                            {
                                Character.GatherCharacterData(Protocol.GetProtoInfoFromString(inData, Protocol.CHARACTER_RIGHTHAND, Protocol.CHARACTER_RIGHTHAND_END), Enums.EPlayerUpdate.RightHand);
                                return true;
                            }
                            #endregion
                            #region CHARACTER_LEFTHAND_END
                            else if (inData.IndexOf(Protocol.CHARACTER_LEFTHAND_END) != -1)
                            {
                                Character.GatherCharacterData(Protocol.GetProtoInfoFromString(inData, Protocol.CHARACTER_LEFTHAND, Protocol.CHARACTER_LEFTHAND_END), Enums.EPlayerUpdate.LeftHand);
                                return true;
                            }
                            #endregion
                            #region CHARACTER_INVENTORY_END
                            else if (inData.IndexOf(Protocol.CHARACTER_INVENTORY_END) != -1)
                            {
                                Character.GatherCharacterData(Protocol.GetProtoInfoFromString(inData, Protocol.CHARACTER_INVENTORY, Protocol.CHARACTER_INVENTORY_END), Enums.EPlayerUpdate.Inventory);
                                return true;
                            }
                            #endregion
                            #region CHARACTER_SACK_END
                            else if (inData.IndexOf(Protocol.CHARACTER_SACK_END) != -1)
                            {
                                Character.GatherCharacterData(Protocol.GetProtoInfoFromString(inData, Protocol.CHARACTER_SACK, Protocol.CHARACTER_SACK_END), Enums.EPlayerUpdate.Sack);
                                return true;
                            }
                            #endregion
                            #region CHARACTER_BELT_END
                            else if (inData.IndexOf(Protocol.CHARACTER_BELT_END) != -1)
                            {
                                Character.GatherCharacterData(Protocol.GetProtoInfoFromString(inData, Protocol.CHARACTER_BELT, Protocol.CHARACTER_BELT_END), Enums.EPlayerUpdate.Belt);
                                return true;
                            }
                            #endregion
                            #region CHARACTER_RINGS_END
                            else if (inData.IndexOf(Protocol.CHARACTER_RINGS_END) != -1)
                            {
                                Character.GatherCharacterData(Protocol.GetProtoInfoFromString(inData, Protocol.CHARACTER_RINGS, Protocol.CHARACTER_RINGS_END), Enums.EPlayerUpdate.Rings);
                                return true;
                            }
                            #endregion
                            #region CHARACTER_LOCKER_END
                            else if (inData.IndexOf(Protocol.CHARACTER_LOCKER_END) != -1)
                            {
                                Character.GatherCharacterData(Protocol.GetProtoInfoFromString(inData, Protocol.CHARACTER_LOCKER, Protocol.CHARACTER_LOCKER_END), Enums.EPlayerUpdate.Locker);
                                return true;
                            }
                            #endregion
                            #region CHARACTER_SPELLS_END
                            else if (inData.IndexOf(Protocol.CHARACTER_SPELLS_END) != -1)
                            {
                                Character.GatherCharacterData(Protocol.GetProtoInfoFromString(inData, Protocol.CHARACTER_SPELLS, Protocol.CHARACTER_SPELLS_END), Enums.EPlayerUpdate.Spells);
                                return true;
                            }
                            #endregion
                            #region CHARACTER_EFFECTS_END
                            else if (inData.IndexOf(Protocol.CHARACTER_EFFECTS_END) != -1)
                            {
                                Character.GatherCharacterData(Protocol.GetProtoInfoFromString(inData, Protocol.CHARACTER_EFFECTS, Protocol.CHARACTER_EFFECTS_END), Enums.EPlayerUpdate.Effects);
                                return true;
                            }
                            #endregion
                            break;
                            #endregion
                    }
                }
            }
            return false;
        }
    }
}
