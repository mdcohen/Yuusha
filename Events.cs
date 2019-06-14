using System;
using System.Collections.Generic;
using Yuusha.gui;
using Microsoft.Xna.Framework;

namespace Yuusha
{
    public static class Events
    {
        public enum EventName
        {
            Attack_Critter,
            Character_Death,
            Character_Settings_Changed,
            CharGen_Lore,
            Client_Settings_Changed,
            Close_Window,
            Connect,
            CreateNewAccount,
            Disconnect,
            Display_CharGen_Text,
            Display_Conference_Text,
            Display_Game_Text,
            End_Game_Round,
            Format_Cell,
            Goto_CharGen,
            Goto_Conf,
            Goto_Game,
            Goto_Menu,
            Load_Character_Settings,
            Load_Client_Settings,
            Logout,
            New_Game_Round,
            NextLOKTile,
            Next_Visual,
            None,
            Request_Belt,
            Request_Effects,
            Request_Inventory,
            Request_Locker,
            Request_Pouch,
            Request_Rings,
            Request_Sack,
            Request_Skills,
            Request_Spells,
            Save_Character_Settings,
            Send_Command,
            Send_Account_Name,
            Send_Password,
            Send_Tell,
            Send_Text,
            Set_CharGen_State,
            Set_Client_Mode,
            Set_Game_State,
            Set_Login_State,
            Set_Login_Status_Label,
            Switch_Character_Next,
            Switch_Character_Back,
            TabControl,
            Target_Cleared,
            Target_Select,
            TextBox_DropDown,
            Toggle_AutoRoller,
            Toggle_HorizontalHotbar,
            Toggle_Macros,
            Toggle_OptionsWindow,
            Toggle_VerticalHotbar,
            User_Settings_Changed,
        }

        public static void RegisterEvent(EventName name, params object[] args)
        {
            //if (Client.GameState == Enums.eGameState.Splash) return;

            try
            {
                switch (name)
                {
                    case EventName.Attack_Critter:
                        #region Attack_Critter
                        {
                            if (args.Length >= 1 && args[0] is Control)
                            {
                                Control control = (Control)args[0];

                                if (control != null)
                                {
                                    if (control is CritterListLabel)
                                    {
                                        CritterListLabel label = control as CritterListLabel;

                                        // TODO: currently only %t is used to put in target ID
                                        GameHUD.TextSendOverride =
                                            Character.Settings.DoubleLeftClickNearbyTarget.Replace("%t",
                                                                                                   label.Critter.ID.
                                                                                                       ToString());

                                        if (!label.CenterCell) // what to do if ranged attack
                                            GameHUD.TextSendOverride =
                                                Character.Settings.DoubleLeftClickDistantTarget.Replace("%t",
                                                                                                        label.Critter.ID
                                                                                                            .ToString());

                                        IO.Send(GameHUD.TextSendOverride);

                                        control = GuiManager.GetControl(Globals.GAMEINPUTTEXTBOX);

                                        if (control != null && control is TextBox)
                                        {
                                            string targetName = label.Critter.Name;

                                            if (Char.IsDigit(targetName[0]))
                                            {
                                                targetName = targetName.Substring(targetName.IndexOf(" ") + 1);
                                                // remove the digit and space for grouped critters
                                                targetName = targetName.Substring(0, targetName.Length - 1);
                                                // remove the s
                                            }

                                            control.Text = GameHUD.TextSendOverride.Replace(
                                                label.Critter.ID.ToString(), targetName);
                                            (control as TextBox).SelectAll();
                                            control.HasFocus = true;
                                        }
                                    }
                                    else if (control is DropDownMenuItem)
                                    {
                                        GameHUD.TextSendOverride = control.Text;

                                        DropDownMenu menu = (control as DropDownMenuItem).DropDownMenu;

                                        bool targetSelected = false;

                                        if (menu != null && menu.DropDownMenuOwner != null &&
                                            menu.DropDownMenuOwner is CritterListLabel)
                                        {
                                            if (control.Text.Contains("%t"))
                                            {
                                                Events.RegisterEvent(Events.EventName.Target_Select,
                                                                     (menu.DropDownMenuOwner as CritterListLabel).
                                                                         Critter);
                                                GameHUD.TextSendOverride = control.Text.Replace("%t",
                                                                                                GameHUD.CurrentTarget.ID
                                                                                                    .ToString());
                                                targetSelected = true;
                                            }

                                            (menu.DropDownMenuOwner as CritterListLabel).DropDownMenu.IsDisabled = true;
                                            (menu.DropDownMenuOwner as CritterListLabel).DropDownMenu.IsVisible = false;
                                            GuiManager.GenericSheet.RemoveControl((menu.DropDownMenuOwner as CritterListLabel).DropDownMenu);
                                            (menu.DropDownMenuOwner as CritterListLabel).DropDownMenu = null;
                                        }

                                        IO.Send(GameHUD.TextSendOverride);

                                        control = GuiManager.GetControl(Globals.GAMEINPUTTEXTBOX);

                                        if (control != null && control is TextBox)
                                        {
                                            if (targetSelected)
                                            {
                                                string targetName = GameHUD.CurrentTarget.Name;

                                                if (Char.IsDigit(targetName[0]))
                                                {
                                                    targetName = targetName.Substring(targetName.IndexOf(" ") + 1);
                                                    // remove the digit and space for grouped critters
                                                    targetName = targetName.Substring(0, targetName.Length - 1);
                                                    // remove the s
                                                }

                                                control.Text =
                                                    GameHUD.TextSendOverride.Replace(
                                                        GameHUD.CurrentTarget.ID.ToString(), targetName);
                                            }
                                            else control.Text = GameHUD.TextSendOverride;

                                            (control as TextBox).SelectAll();
                                            control.HasFocus = true;
                                        }
                                    }
                                }
                            }
                            break;
                        }

                    #endregion
                    case EventName.Character_Death:
                        #region Character Death
                        {
                            if (Int32.TryParse(args[0].ToString(), out int id))
                            {
                                // currently just clear target
                                if (GameHUD.CurrentTarget != null && GameHUD.CurrentTarget.ID == id)
                                    Events.RegisterEvent(EventName.Target_Cleared, null);
                            }
                        }
                        break;
                    #endregion
                    case EventName.CharGen_Lore:
                        #region Character Generation Lore
                        Dictionary<string, string> dictionaryInfo = new Dictionary<string, string>();
                        switch (CharGen.CharGenState)
                        {
                            case Enums.ECharGenState.ChooseHomeland:
                                CharGen.SelectedHomeland = (args[0] as Button).Text;
                                dictionaryInfo = CharGen.Homelands;
                                break;
                            case Enums.ECharGenState.ChooseProfession:
                                CharGen.SelectedProfession = (args[0] as Button).Text;
                                dictionaryInfo = CharGen.Professions;
                                break;
                        }
                        (gui.GuiManager.CurrentSheet["CharGenSelectionButton"] as gui.Button).Text = "Select: " + (args[0] as Button).Text;
                        (gui.GuiManager.CurrentSheet["CharGenSelectionButton"] as gui.Button).Command = CharGen.CommandsToSend[(args[0] as Button).Text];
                        System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(() => CharGen.PopulateInfoTextBox((args[0] as Button).Text, dictionaryInfo));
                        task.Start();
                        task.Wait();
                        break; 
                    #endregion
                    case EventName.Close_Window:
                        Window w = GuiManager.GetControl((args[0] as Control).Owner) as Window;
                        if (w != null)
                            w.OnClose();
                        break;
                    case EventName.Connect:
                        // TODO: play modem connect sound??
                        #region Connect
                        if (IO.LoginState != Enums.ELoginState.NewAccount)
                        {
                            #region Not in new account creation mode.
                            // TEMPORARY ?? Why is/was this temporary? (10/27/11 -MDC)
                            if ((GuiManager.CurrentSheet["LoginWindow"] as Window)["AccountTextBox"].Text.Length > 0 &&
                                (GuiManager.CurrentSheet["LoginWindow"] as Window)["PasswordTextBox"].Text.Length > 0)
                            {
                                string serverHostAddress = Client.ClientSettings.ServerHost;

                                if ((GuiManager.CurrentSheet["LoginWindow"] as Window)["ServerHostTextBox"] is TextBox serverHostTextBox)
                                {
                                    if (serverHostTextBox.Text != Client.ClientSettings.ServerHost)
                                        Client.ClientSettings.ServerHost = serverHostTextBox.Text;
                                }

                                if (IO.Connect())
                                {
                                    RegisterEvent(EventName.Set_Login_Status_Label,
                                                  "Connecting to " + Client.ClientSettings.ServerName + "...", "Lime");
                                    RegisterEvent(EventName.Set_Login_State, Enums.ELoginState.Connected);

                                    bool rememberPassword = (GuiManager.CurrentSheet["RememberPasswordCheckboxButton"] as CheckboxButton).IsChecked;
                                    string accountName = (GuiManager.CurrentSheet["LoginWindow"] as Window)["AccountTextBox"].Text;
                                    string encryptedAccountName = Utility.Encrypt.EncryptString(accountName, Utility.Settings.StaticSettings.DecryptionPassPhrase1);

                                    if (!rememberPassword)
                                    {
                                        if (Client.ClientSettings.ContainsStoredAccount(encryptedAccountName, out Utility.Encrypt.EncryptedKeyValuePair<string, string> kvPair))
                                            Client.ClientSettings.StoredAccounts.Remove(kvPair);

                                        Client.ClientSettings.MostRecentStoredAccount = "";
                                    }
                                    else
                                    {
                                        string password = (GuiManager.CurrentSheet["LoginWindow"] as Window)["PasswordTextBox"].Text;
                                        string encryptedPassword = Utility.Encrypt.EncryptString(password, Utility.Settings.StaticSettings.DecryptionPassPhrase2);

                                        Client.ClientSettings.MostRecentStoredAccount = Utility.Encrypt.EncryptString(encryptedAccountName, Utility.Settings.StaticSettings.DecryptionPassPhrase3);

                                        if (!Client.ClientSettings.ContainsStoredAccount(encryptedAccountName, out Utility.Encrypt.EncryptedKeyValuePair<string, string> kvPair))
                                            Client.ClientSettings.StoredAccounts.Add(new Utility.Encrypt.EncryptedKeyValuePair<string, string>(encryptedAccountName, encryptedPassword));
                                    }

                                    RegisterEvent(EventName.Client_Settings_Changed); // always save client settings?
                                }
                                else
                                {
                                    RegisterEvent(EventName.Set_Login_Status_Label, "Failed to connect. Check internet connection.", "Red");
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            #region Creating a new account.
                            var newAccountTextBox = (GuiManager.CurrentSheet["CreateNewAccountWindow"] as Window)["CreateNewAccountTextBox"] as TextBox;
                            var newAccountPasswordTextBox = (GuiManager.CurrentSheet["CreateNewAccountWindow"] as Window)["CreateNewAccountPasswordTextBox"] as TextBox;
                            var newAccountEmailTextBox = (GuiManager.CurrentSheet["CreateNewAccountWindow"] as Window)["CreateNewAccountEmailTextBox"] as TextBox;

                            if (newAccountTextBox.Text.Length == 0 || newAccountPasswordTextBox.Text.Length == 0 ||
                                newAccountEmailTextBox.Text.Length == 0)
                            {
                                newAccountTextBox.HasFocus = true;
                                return;
                            }

                            // Check account name length.
                            if (newAccountTextBox.Text.Length < CharGen.ACCOUNT_MIN_LENGTH || newAccountTextBox.Text.Length > CharGen.ACCOUNT_MAX_LENGTH)
                            {
                                TextCue.AddClientInfoTextCue("Account name must be between " + CharGen.ACCOUNT_MIN_LENGTH + " and " + CharGen.ACCOUNT_MAX_LENGTH + " characters in length.",
                                    TextCue.TextCueTag.None, Color.OrangeRed, Color.Transparent, 10000, false, true, true);

                                newAccountTextBox.HasFocus = true;
                                return;
                            }
                            else CharGen.NewAccountName = newAccountTextBox.Text;

                            // Check password lenth.
                            if (newAccountPasswordTextBox.Text.Length < CharGen.PASSWORD_MIN_LENGTH || newAccountPasswordTextBox.Text.Length > CharGen.PASSWORD_MAX_LENGTH)
                            {
                                TextCue.AddClientInfoTextCue("Password must be between " + CharGen.PASSWORD_MIN_LENGTH + " and " + CharGen.PASSWORD_MAX_LENGTH + " characters in length.",
                                    TextCue.TextCueTag.None, Color.OrangeRed, Color.Transparent, 10000, false, true, true);

                                newAccountPasswordTextBox.HasFocus = true;
                                return;
                            }
                            else if ((GuiManager.CurrentSheet["CreateNewAccountWindow"] as gui.Window)["CreateNewAccountConfirmPasswordTextBox"].Text != newAccountPasswordTextBox.Text)
                            {
                                TextCue.AddClientInfoTextCue("Passwords do not match.", TextCue.TextCueTag.None, Color.OrangeRed, Color.Transparent, 10000, false, true, true);
                                newAccountPasswordTextBox.HasFocus = true;
                                return;
                            }
                            else CharGen.NewAccountPassword = newAccountPasswordTextBox.Text;

                            try
                            {
                                System.Net.Mail.MailAddress email = new System.Net.Mail.MailAddress(newAccountEmailTextBox.Text);
                            }
                            catch
                            {
                                TextCue.AddClientInfoTextCue("Invalid email address.", TextCue.TextCueTag.None, Color.OrangeRed, Color.Transparent, 10000, false, true, true);
                                newAccountEmailTextBox.HasFocus = true;
                                return;
                            }

                            if ((GuiManager.CurrentSheet["CreateNewAccountWindow"] as gui.Window)["CreateNewAccountConfirmEmailTextBox"].Text != newAccountEmailTextBox.Text)
                            {
                                TextCue.AddClientInfoTextCue("Email addresses do not match.", TextCue.TextCueTag.None, Color.OrangeRed, Color.Transparent, 10000, false, true, true);
                                newAccountEmailTextBox.HasFocus = true;
                                return;
                            }
                            else CharGen.NewAccountEmail = newAccountEmailTextBox.Text;

                            // Checks complete. Let's sign in and attempt to create the account. Then on to CharGen.

                            if (IO.Connect())
                            {
                                RegisterEvent(EventName.Set_Login_Status_Label,
                                              "Connecting to " + Client.ClientSettings.ServerName + "...", "Lime");
                                RegisterEvent(EventName.Set_Login_State, Enums.ELoginState.Connected);
                                RegisterEvent(EventName.Set_Login_State, Enums.ELoginState.NewAccount);
                                //if (IO.LoginState != Enums.ELoginState.NewAccount)
                                //    RegisterEvent(EventName.Set_Login_State, Enums.ELoginState.Connected);
                            }
                            else
                            {
                                RegisterEvent(EventName.Set_Login_Status_Label, "Failed to connect. Check internet connection.", "Red");
                            } 
                            #endregion
                        }
                        break;
                    #endregion
                    case EventName.CreateNewAccount:
                        if(GuiManager.GetControl("CreateNewAccountWindow") is Window cnaw)
                        {
                            if (GuiManager.GetControl("LoginWindow") is Window loginWindow)
                            {
                                loginWindow.IsVisible = false;
                                cnaw.IsVisible = true;
                                RegisterEvent(EventName.Set_Login_State, Enums.ELoginState.NewAccount); // set state
                            }
                        }
                        break;
                    case EventName.Disconnect:
                        #region Disconnect
                        {
                            IO.Send(Protocol.LOGOUT); // send logout if connected
                            IO.Disconnect(); // clean up the connection
                            Program.Client.GUIManager.LoadSheet(
                                gui.GuiManager.Sheets[Enums.EGameState.IOKGame.ToString()].FilePath);
                            Program.Client.GUIManager.LoadSheet(
                                gui.GuiManager.Sheets[Enums.EGameState.SpinelGame.ToString()].FilePath);
                            Program.Client.GUIManager.LoadSheet(
                                gui.GuiManager.Sheets[Enums.EGameState.Conference.ToString()].FilePath);
                            RegisterEvent(EventName.Set_Login_State, Enums.ELoginState.Disconnected); // set state
                            RegisterEvent(EventName.Set_Game_State, Enums.EGameState.Login);
                        }
                        break;
                    #endregion
                    case EventName.Display_CharGen_Text:
                        #region Display CharGen Text
                        (GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine(
                            (string)args[0], Enums.ETextType.Default);
                        break;
                    #endregion
                    case EventName.Display_Conference_Text:
                        #region Display Conference Text
                        
                        string message = (string)args[0];
                        // detect tells
                        // decide above if message will also be put into conference scrollable box
                        bool privateMessage = false;
                        if (Client.ClientSettings.DisplayPrivateMessageWindows)
                        {
                            if (message.IndexOf("tells you,") != -1 && message.Split(" ".ToCharArray())[1] == "tells")
                            {
                                PrivateMessageWindow pmWindow2 = PrivateMessageWindow.CreateNewPrivateMessageWindow(message.Split(" ".ToCharArray())[0]);
                                pmWindow2.ReceivedMessage(message);
                                privateMessage = true;
                            }
                            else if (message.StartsWith("You tell"))
                            {
                                string[] pm = message.Split(" ".ToCharArray());

                                PrivateMessageWindow pmWindow2 = PrivateMessageWindow.CreateNewPrivateMessageWindow(pm[2].Substring(0, pm[2].Length - 1));
                                pmWindow2.SentMessage(message);
                                privateMessage = true;
                            }
                        }

                        if(!privateMessage || (privateMessage && Client.ClientSettings.EchoPrivateMessagesToConference))
                        {
                            (GuiManager.CurrentSheet["ConfScrollableTextBox"] as gui.ScrollableTextBox).AddLine((string)args[0], (Enums.ETextType)args[1]);
                        }
                        break;
                        #endregion
                    case EventName.Display_Game_Text:
                        #region Display Game Text

                        switch (Client.GameDisplayMode)
                        {
                            case Enums.EGameDisplayMode.Spinel:
                                message = (string)args[0];
                                privateMessage = false;
                                if (Client.ClientSettings.DisplayPrivateMessageWindows)
                                {
                                    if (message.IndexOf("tells you,") != -1 && message.Split(" ".ToCharArray())[1] == "tells")
                                    {
                                        PrivateMessageWindow pmWindow2 = PrivateMessageWindow.CreateNewPrivateMessageWindow(message.Split(" ".ToCharArray())[0]);
                                        pmWindow2.ReceivedMessage(message);
                                        privateMessage = true;
                                    }
                                    else if (message.StartsWith("You tell"))
                                    {
                                        string[] pm = message.Split(" ".ToCharArray());

                                        PrivateMessageWindow pmWindow2 = PrivateMessageWindow.CreateNewPrivateMessageWindow(pm[2].Substring(0, pm[2].Length - 1));
                                        pmWindow2.SentMessage(message);
                                        privateMessage = true;
                                    }
                                }

                                if (!privateMessage || (privateMessage && Client.ClientSettings.EchoPrivateMessagesToConference))
                                {
                                    SpinelMode.DisplayGameText((string)args[0], Enums.ETextType.Default);

                                    if ((string)args[0] == TextManager.YOU_ARE_STUNNED)
                                        TextCue.AddPromptStateTextCue(Protocol.PromptStates.Stunned);

                                    if (Client.ClientSettings.DisplayChantingTextCue)
                                    {
                                        if (args[0].ToString().StartsWith("You warm the spell "))
                                        {
                                            string findSpellName = args[0].ToString().Replace("You warm the spell ", "");
                                            findSpellName = findSpellName.Replace(".", "");
                                            //Utils.Log("Spells Count: " + Character.CurrentCharacter.Spells.Count + " Looking for: " + findSpellName);
                                            foreach (Spell spell in Character.CurrentCharacter.Spells)
                                            {
                                                Utils.Log("Spell: " + spell.Name);
                                                if (spell.Name == findSpellName)
                                                {
                                                    //Utils.Log("Found spell: " + findSpellName);
                                                    //TextCue.AddChantingTextCue(spell.Incantation);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            case Enums.EGameDisplayMode.IOK:
                                gui.IOKMode.DisplayGameText((string) args[0], Enums.ETextType.Default);
                                    // TODO: temporary until texttypes done
                                if ((string) args[0] == TextManager.YOU_ARE_STUNNED)
                                    gui.TextCue.AddPromptStateTextCue(Protocol.PromptStates.Stunned);
                                break;
                            case Enums.EGameDisplayMode.LOK:
                                break;
                            case Enums.EGameDisplayMode.Normal:
                                break;
                        }
                        break;

                        #endregion
                    case EventName.End_Game_Round:
                        #region End Game Round

                        switch (Client.GameDisplayMode)
                        {
                            case Enums.EGameDisplayMode.IOK:
                            case Enums.EGameDisplayMode.Spinel:
                                gui.IOKMode.EndGameRound();
                                gui.SpinelMode.EndGameRound();
                                break;
                            case Enums.EGameDisplayMode.LOK:
                                break;
                            default:
                                break;
                        }
                        break;

                        #endregion
                    case EventName.Format_Cell:
                        #region Format Cell

                        switch (Client.GameDisplayMode)
                        {
                            case Enums.EGameDisplayMode.IOK:
                            case Enums.EGameDisplayMode.Spinel:
                                gui.IOKMode.FormatCells((string) args[0]);
                                gui.SpinelMode.FormatCell((string) args[0]);
                                break;
                            case Enums.EGameDisplayMode.LOK:
                                break;
                            case Enums.EGameDisplayMode.Normal:
                                break;
                        }
                        break;

                    #endregion
                    case EventName.Goto_CharGen:
                        #region Goto CharGen
                        if (Client.GameState == Enums.EGameState.Conference)
                        {
                            IO.SwitchingToCharGen = true;
                            IO.Send("/menu");
                        }
                        IO.Send("6");
                        IO.Send("1");
                        //RegisterEvent(EventName.Set_Game_State, Enums.EGameState.CharacterGeneration);
                        break;
                    #endregion
                    case EventName.Goto_Conf:
                        #region Goto Conf
                        IO.Send(Protocol.GOTO_CONFERENCE);
                        RegisterEvent(EventName.Set_Game_State, Enums.EGameState.Conference);
                        break;
                        #endregion
                    case EventName.Goto_Game:
                        #region Goto Game

                        IO.Send(Protocol.GOTO_GAME);
                        Enums.EGameState gstate = Enums.EGameState.Game;
                        switch (Client.GameDisplayMode)
                        {
                            case Enums.EGameDisplayMode.Spinel:
                                gstate = Enums.EGameState.SpinelGame;
                                break;
                            case Enums.EGameDisplayMode.IOK:
                                gstate = Enums.EGameState.IOKGame;
                                break;
                            case Enums.EGameDisplayMode.LOK:
                                gstate = Enums.EGameState.LOKGame;
                                break;
                            default:
                                break;
                        }
                        RegisterEvent(EventName.Set_Game_State, gstate);
                        break;

                        #endregion
                    case EventName.Goto_Menu:
                        #region Goto Menu
                        IO.Send(Protocol.GOTO_MENU);
                        RegisterEvent(EventName.Set_Game_State, Enums.EGameState.Menu);
                        break; 
                    #endregion
                    case EventName.Logout:
                        #region Logout
                        switch (Client.GameState)
                        {
                            case Enums.EGameState.Menu:
                                IO.Send("3");
                                break;
                            case Enums.EGameState.Conference:
                                IO.Send("/exit");
                                break;
                        }
                        ResetLoginGUI();
                        RegisterEvent(Events.EventName.Set_Login_State, Enums.ELoginState.Disconnected);
                        RegisterEvent(Events.EventName.Set_Game_State, Enums.EGameState.Login);
                        break; 
                    #endregion
                    case EventName.Next_Visual:
                        #region Next Visual
                        Window VisualKeyWindow = GuiManager.GenericSheet["VisualKeyWindow"] as Window;

                        if (VisualKeyWindow != null)
                        {
                            Button VisualKeyWindowButton = VisualKeyWindow["VisualKeyWindowButton"] as Button;

                            string visualName = VisualKeyWindowButton.VisualKey;

                            if (visualName.StartsWith("hotbuttonicon_"))
                            {
                                visualName = visualName.Replace("hotbuttonicon_", "");
                                if (Int32.TryParse(visualName, out int currentNum))
                                    currentNum = currentNum + 1;

                                if (GuiManager.Visuals.ContainsKey("hotbuttonicon_" + currentNum))
                                {
                                    VisualKeyWindowButton.VisualKey = "hotbuttonicon_" + currentNum;
                                }
                                else
                                {
                                    VisualKeyWindowButton.VisualKey = "hotbuttonicon_0";
                                }
                            }
                            else if (visualName.StartsWith("spinelspells_"))
                            {
                                visualName = visualName.Replace("spinelspells_", "");
                                if (Int32.TryParse(visualName, out int currentNum))
                                    currentNum = currentNum + 1;

                                if (GuiManager.Visuals.ContainsKey("spinelspells_" + currentNum))
                                {
                                    VisualKeyWindowButton.VisualKey = "spinelspells_" + currentNum;
                                }
                                else
                                {
                                    VisualKeyWindowButton.VisualKey = "spinelspells_0";
                                }
                            }

                            TextCue.AddMouseCursorTextCue(VisualKeyWindowButton.VisualKey, Color.Yellow, "courier16");
                        }
                        break; 
                    #endregion
                    case EventName.New_Game_Round:
                        #region New Game Round

                        switch (Client.GameDisplayMode)
                        {
                            case Enums.EGameDisplayMode.IOK:
                            case Enums.EGameDisplayMode.Spinel:
                                gui.IOKMode.NewGameRound();
                                gui.SpinelMode.NewGameRound();
                                break;
                            case Enums.EGameDisplayMode.LOK:
                                break;
                            case Enums.EGameDisplayMode.Normal:
                                break;
                        }
                        break;

                        #endregion
                    case EventName.NextLOKTile:
                        #region NextLOKTile
                        {
                            Control control = (Control)args[0];
                            string code = gui.LOKMode.GetCodeByVisualKey(control.VisualKey);
                            int index = gui.LOKMode.Codes.IndexOf(code);
                            if (index + 1 > gui.LOKMode.Codes.Count)
                                control.VisualKey = gui.LOKMode.Tiles[gui.LOKMode.Codes[0]].VisualKey;
                            else control.VisualKey = gui.LOKMode.Tiles[gui.LOKMode.Codes[index]].VisualKey;
                            break;
                        }
                    #endregion
                    case EventName.Request_Belt:
                        // get belt grid box, if not visible send request
                        Control gridBoxWindow = GuiManager.GetControl("BeltGridBoxWindow");
                        if (gridBoxWindow == null || !gridBoxWindow.IsVisible)
                            IO.Send(Protocol.REQUEST_CHARACTER_BELT);
                        else if (gridBoxWindow.IsVisible)
                            gridBoxWindow.IsVisible = false;
                        break;
                    case EventName.Request_Effects:
                        IO.Send(Protocol.REQUEST_CHARACTER_EFFECTS);
                        break;
                    case EventName.Request_Inventory:
                        gridBoxWindow = GuiManager.GetControl("InventoryWindow");
                        gridBoxWindow.IsVisible = !gridBoxWindow.IsVisible;
                        if(gridBoxWindow.IsVisible) gridBoxWindow.ZDepth = 1;
                        //IO.Send(Protocol.REQUEST_CHARACTER_INVENTORY);
                        break;
                    case EventName.Request_Locker:
                        gridBoxWindow = GuiManager.GetControl("LockerGridBoxWindow");
                        if (gridBoxWindow == null || !gridBoxWindow.IsVisible)
                            IO.Send(Protocol.REQUEST_CHARACTER_LOCKER);
                        else if (gridBoxWindow.IsVisible)
                            gridBoxWindow.IsVisible = false;
                        break;
                    case EventName.Request_Pouch:
                        gridBoxWindow = GuiManager.GetControl("PouchGridBoxWindow");
                        if (gridBoxWindow == null || !gridBoxWindow.IsVisible)
                            IO.Send(Protocol.REQUEST_CHARACTER_POUCH);
                        else if (gridBoxWindow.IsVisible)
                            gridBoxWindow.IsVisible = false;
                        break;
                    case EventName.Request_Rings:
                        gridBoxWindow = GuiManager.GetControl("RingsGridBoxWindow");
                        if (gridBoxWindow == null || !gridBoxWindow.IsVisible)
                            IO.Send(Protocol.REQUEST_CHARACTER_RINGS);
                        else if (gridBoxWindow.IsVisible)
                            gridBoxWindow.IsVisible = false;
                        break;
                    case EventName.Request_Sack:
                        gridBoxWindow = GuiManager.GetControl("SackGridBoxWindow");
                        if (gridBoxWindow == null || !gridBoxWindow.IsVisible)
                            IO.Send(Protocol.REQUEST_CHARACTER_SACK);
                        else if (gridBoxWindow.IsVisible)
                            gridBoxWindow.IsVisible = false;
                        break;
                    case EventName.Request_Skills:
                        IO.Send(Protocol.REQUEST_CHARACTER_SKILLS);
                        break;
                    case EventName.Request_Spells:
                        IO.Send(Protocol.REQUEST_CHARACTER_SPELLS);
                        break;
                    case EventName.Send_Account_Name:
                        IO.Send((GuiManager.CurrentSheet["LoginWindow"] as gui.Window)["AccountTextBox"].Text);
                        break;
                    case EventName.Send_Password:
                        IO.Send((GuiManager.CurrentSheet["LoginWindow"] as gui.Window)["PasswordTextBox"].Text);
                        break;
                    case EventName.Send_Tell:
                        #region Send Tell
                        string tellToSend = (args[0] as Control).Text;
                        PrivateMessageWindow pmWindow = GuiManager.GenericSheet[(args[0] as Control).Owner] as PrivateMessageWindow;
                        tellToSend = "tell " + pmWindow.RecipientName + " " + tellToSend;
                        if (Client.GameState == Enums.EGameState.Conference)
                            tellToSend = "/" + tellToSend;
                        IO.Send(tellToSend);
                        (args[0] as TextBox).SelectAll();
                        break; 
                    #endregion
                    case EventName.Send_Text:
                        #region Send Text
                        {
                            string textToSend = (args[0] as Control).Text;
                            if (Client.GameState.ToString().EndsWith("Game") && GameHUD.CurrentTarget != null)
                                textToSend = textToSend.Replace("%t", GameHUD.CurrentTarget.ID.ToString());

                            if (textToSend != "")
                                IO.Send(textToSend);
                            break;
                        }
                    #endregion
                    case EventName.Send_Command:
                        if (args[0] is Control)
                        {
                            IO.Send((args[0] as Control).Command);
                            TextCue.AddClientInfoTextCue((args[0] as Control).Command);

                        }
                        //if(args[0] is DropDownMenuItem sendCommandMenuItem)
                        //{
                        //    if(sendCommandMenuItem.DropDownMenu != null && sendCommandMenuItem.DropDownMenu.DropDownMenuOwner is DragAndDropButton)
                        //    {
                        //        TextCue.AddClientInfoTextCue("sendCommandMenuItem.DropDownMenu.DropDownMenuOwner.Owner: " + sendCommandMenuItem.DropDownMenu.DropDownMenuOwner.Owner);

                        //        if (sendCommandMenuItem.DropDownMenu.DropDownMenuOwner.Owner.Contains("GridBoxWindow"))
                        //        {
                        //            TextCue.AddClientInfoTextCue("...and here.");
                        //            GridBoxWindow.GridBoxPurpose purpose = (sendCommandMenuItem.DropDownMenu.DropDownMenuOwner as DragAndDropButton).GridBoxUpdateRequests[sendCommandMenuItem.DropDownMenu.MenuItems.IndexOf(sendCommandMenuItem)];
                        //            GridBoxWindow.GridBoxPurpose ownerPurpose = (GuiManager.GetControl(sendCommandMenuItem.DropDownMenu.DropDownMenuOwner.Owner) as GridBoxWindow).GridBoxPurposeType;

                        //            GridBoxWindow.RequestUpdateFromServer(purpose);
                        //            GridBoxWindow.RequestUpdateFromServer(ownerPurpose);
                        //        }
                        //    }
                        //}
                        break;
                    case EventName.Set_CharGen_State:
                        CharGen.CharGenState = (Enums.ECharGenState)args[0];
                        switch(CharGen.CharGenState)
                        {
                            case Enums.ECharGenState.ChooseGender:
                                CharGen.ChooseGender();
                                (gui.GuiManager.CurrentSheet["CharGenInputTextBox"] as gui.TextBox).HasFocus = true;
                                break;
                            case Enums.ECharGenState.ChooseHomeland:
                                (gui.GuiManager.CurrentSheet["CharGenSelectionButton"] as gui.Button).IsVisible = true;
                                CharGen.ChooseHomeland();
                                break;
                            case Enums.ECharGenState.ChooseProfession:
                                CharGen.ChooseProfession();
                                break;
                            case Enums.ECharGenState.ChooseName:
                                CharGen.ChooseName();
                                (gui.GuiManager.CurrentSheet["CharGenInputTextBox"] as gui.TextBox).HasFocus = true;
                                break;
                            case Enums.ECharGenState.ReviewStats:
                                (gui.GuiManager.CurrentSheet["CharGenSelectionButton"] as gui.Button).IsVisible = false;
                                CharGen.RemoveCharGenSelectionButtons(new List<string>(CharGen.Professions.Keys));
                                (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).Height = 357;
                                CharGen.ReviewStats((string)args[1]);
                                break;
                        }
                        break;
                    case EventName.Set_Client_Mode:
                        Client.GameDisplayMode = (Enums.EGameDisplayMode) args[0];
                        if(Client.GameState.ToString().EndsWith("Game"))
                            RegisterEvent(EventName.Set_Game_State, Enums.EGameState.Game);
                        break;
                    case EventName.Set_Game_State:
                        #region Set Game State
                        OnSwitchGameState(Client.GameState, (Enums.EGameState)args[0]);

                        if ((Enums.EGameState) args[0] == Enums.EGameState.Game)
                        {
                            switch (Client.GameDisplayMode)
                            {
                                case Enums.EGameDisplayMode.Spinel:
                                    Client.GameState = Enums.EGameState.SpinelGame;
                                    break;
                                case Enums.EGameDisplayMode.IOK:
                                    Client.GameState = Enums.EGameState.IOKGame;
                                    break;
                                case Enums.EGameDisplayMode.LOK:
                                    Client.GameState = Enums.EGameState.LOKGame;
                                    break;
                                default:
                                    Client.GameState = Enums.EGameState.Game;
                                    break;
                            }
                        }
                        else
                        {
                            Client.GameState = (Enums.EGameState) args[0];

                            if(Client.GameState == Enums.EGameState.CharacterGeneration)
                            {
                                if (CharGen.RollNumber > 0) CharGen.ResetCharGen();

                                if(CharGen.FirstCharacter)
                                {
                                    TextCue.AddClientInfoTextCue("There are currently no characters on your account. Please create one.", Color.Yellow, Color.Black, 3500);
                                }

                                TextCue.AddClientInfoTextCue("Welcome to the character generator.", Color.Yellow, Color.Black, 3500);

                                RegisterEvent(EventName.Set_CharGen_State, Enums.ECharGenState.ChooseGender);

                                (gui.GuiManager.CurrentSheet["CharGenInputTextBox"] as gui.TextBox).HasFocus = true;
                            }

                            if (Client.GameState.ToString().EndsWith("Game"))
                                Events.RegisterEvent(EventName.Target_Cleared, null);

                            if (Client.GameState == Enums.EGameState.Login)
                                ResetLoginGUI();
                        }
                        break;
                        #endregion
                    case EventName.Set_Login_State:
                        IO.LoginState = (Enums.ELoginState) args[0];
                        break;
                    case EventName.Set_Login_Status_Label:
                        #region Set Login Status Label
                        Label loginLabel = GuiManager.CurrentSheet["LoginStatusLabel"] as Label;
                        loginLabel.Text = (string) args[0];
                        loginLabel.TextColor = Utils.GetColor((string) args[1]);
                        if (args.Length >= 3)
                            GuiManager.CurrentSheet["LoginStatusLabel"].IsVisible = (bool) args[2];
                        break;
                        #endregion
                    case EventName.Switch_Character_Back:
                        Character.Settings.Save();
                        Character.CurrentCharacter = Account.GetPreviousCharacter();
                        IO.Send(Protocol.SWITCH_CHARACTER + " " + Character.CurrentCharacter.ID);
                        break;
                    case EventName.Switch_Character_Next:
                        Character.Settings.Save();
                        Character.CurrentCharacter = Account.GetNextCharacter();
                        IO.Send(Protocol.SWITCH_CHARACTER + " " + Character.CurrentCharacter.ID);
                        break;
                    case EventName.TextBox_DropDown:
                        #region TextBox_DropDown
                        if (args.Length >= 1 && args[0] is Control)
                        {
                            Control control = (Control)args[0];

                            if (control != null)
                            {
                                if (control is DropDownMenuItem)
                                {
                                    DropDownMenu menu = (control as DropDownMenuItem).DropDownMenu;
                                    TextBox textBox = GuiManager.GetControl(menu.Owner) as TextBox;

                                    switch (control.Text.ToLower())
                                    {
                                        case "cut":
                                            textBox.SelectAll();
                                            Utils.SetClipboardText(textBox.Text.Substring(textBox.SelectionStart, textBox.SelectionLength));
                                            textBox.Clear();
                                            break;
                                        case "copy":
                                            textBox.SelectAll();
                                            Utils.SetClipboardText(textBox.Text.Substring(textBox.SelectionStart, textBox.SelectionLength));
                                            break;
                                        case "paste":
                                            if (textBox.SelectionLength > 0)
                                                textBox.ReplaceSelectedText(Utils.GetClipboardText());
                                            else textBox.InsertClipboardText();
                                            break;
                                        case "delete":
                                            textBox.SelectAll();
                                            textBox.Clear();
                                            break;
                                    }

                                    GuiManager.CurrentSheet.RemoveControl(menu);
                                    textBox.DropDownMenu = null;
                                    menu = null;
                                }
                            }
                        }
                        break;
                    #endregion
                    case EventName.Toggle_AutoRoller:
                        #region Toggle_AutoRoller
                        if (CharGen.CharGenState != Enums.ECharGenState.ReviewStats)
                        {
                            TextCue.AddClientInfoTextCue("Auto roller may be enabled when reviewing your first stats roll.", 4000);
                            return;
                        }
                        CharGen.AutoRollerEnabled = !CharGen.AutoRollerEnabled;
                        (GuiManager.CurrentSheet["CharGenToggleAutoRollerButton"] as Button).Text = "Toggle Auto Roller " + (CharGen.AutoRollerEnabled ? "Off" : "On");
                        if (CharGen.AutoRollerEnabled && CharGen.CharGenState == Enums.ECharGenState.ReviewStats && !CharGen.DesiredStatsAchieved())
                        {
                            CharGen.AutoRollerStartTime = DateTime.Now;
                            IO.Send("y");
                        }
                        break; 
                    #endregion
                    case EventName.Client_Settings_Changed:
                        Client.ClientSettings.Save();
                        break;
                    case EventName.User_Settings_Changed:
                        Client.UserSettings.Save();
                        break;
                    case EventName.Character_Settings_Changed:
                        {
                            //TODO: use Reflection to save, where ****TextBox should be variable in CharacterSettings
                            if (!GameHUD.OverrideDisplayStates.Contains(Client.GameState))
                            {
                                if (GuiManager.GenericSheet["OptionsWindow"] is Window optionsWindow && optionsWindow.IsVisible)
                                {
                                    #region Options Window
                                    if (optionsWindow["DoubleLeftClickNearbyTargetTextBox"] is TextBox tbx)
                                        Character.Settings.DoubleLeftClickNearbyTarget = tbx.Text;
                                    tbx = optionsWindow["DoubleLeftClickDistantTargetTextBox"] as TextBox;
                                    if (tbx != null)
                                        Character.Settings.DoubleLeftClickDistantTarget = tbx.Text;
                                    tbx = optionsWindow["OptionsNumPadDivideTextBox"] as TextBox;
                                    if (tbx != null)
                                        Character.Settings.NumPadDivide = tbx.Text;
                                    tbx = optionsWindow["OptionsNumPadMultiplyTextBox"] as TextBox;
                                    if (tbx != null)
                                        Character.Settings.NumPadMultiply = tbx.Text;
                                    tbx = optionsWindow["OptionsNumPadSubtractTextBox"] as TextBox;
                                    if (tbx != null)
                                        Character.Settings.NumPadSubtract = tbx.Text;
                                    tbx = optionsWindow["OptionsNumPadAddTextBox"] as TextBox;
                                    if (tbx != null)
                                        Character.Settings.NumPadAdd = tbx.Text;
                                    tbx = optionsWindow["OptionsNumPadZeroTextBox"] as TextBox;
                                    if (tbx != null)
                                        Character.Settings.NumLock0 = tbx.Text;
                                    tbx = optionsWindow["OptionsNumPadFiveTextBox"] as TextBox;
                                    if (tbx != null)
                                        Character.Settings.NumLock5 = tbx.Text;
                                    tbx = optionsWindow["OptionsNumPadDeleteTextBox"] as TextBox;
                                    if (tbx != null)
                                        Character.Settings.NumPadDelete = tbx.Text;
                                    tbx = optionsWindow["OptionsCritterDropDownTextBox1"] as TextBox;
                                    if (tbx != null)
                                        Character.Settings.CritterListDropDownMenuItem1 = tbx.Text;
                                    tbx = optionsWindow["OptionsCritterDropDownTextBox2"] as TextBox;
                                    if (tbx != null)
                                        Character.Settings.CritterListDropDownMenuItem2 = tbx.Text;
                                    tbx = optionsWindow["OptionsCritterDropDownTextBox3"] as TextBox;
                                    if (tbx != null)
                                        Character.Settings.CritterListDropDownMenuItem3 = tbx.Text;
                                    tbx = optionsWindow["OptionsCritterDropDownTextBox4"] as TextBox;
                                    if (tbx != null)
                                        Character.Settings.CritterListDropDownMenuItem4 = tbx.Text;
                                    tbx = optionsWindow["OptionsCritterDropDownTextBox5"] as TextBox;
                                    if (tbx != null)
                                        Character.Settings.CritterListDropDownMenuItem5 = tbx.Text;
                                    #endregion

                                    TextCue.AddClientInfoTextCue("Character Options Saved", Color.Lime, Color.Black, 1000);

                                }
                            }
                            else
                            {
                                if (Client.GameState == Enums.EGameState.HotButtonEditMode)
                                {
                                    #region Hot Button Edit Mode
                                    if (GuiManager.GetControl("HotButtonEditWindow") is HotButtonEditWindow hotButtonEditWindow)
                                    {
                                        bool isHorizontal = hotButtonEditWindow.OriginatingWindow.ToLower().Contains("horizontal"); // horizontal or vertical hot buttons only right now

                                        if (!isHorizontal)
                                        {
                                            if (GuiManager.GenericSheet["VerticalHotButtonWindow"] is Window verticalHotButtonWindow)
                                            {
                                                verticalHotButtonWindow[hotButtonEditWindow.SelectedHotButton].VisualKey = hotButtonEditWindow.SelectedVisualKey;
                                                verticalHotButtonWindow[hotButtonEditWindow.SelectedHotButton].Text = hotButtonEditWindow["HotButtonEditWindowTextBox"].Text;
                                                (verticalHotButtonWindow[hotButtonEditWindow.SelectedHotButton] as HotButton).Command = hotButtonEditWindow["HotButtonEditWindowCommandTextBox"].Text;

                                                Character.Settings.VerticalHotButtonVisualKey0 = verticalHotButtonWindow["VerticalHBWindowHotButton0"].VisualKey;
                                                Character.Settings.VerticalHotButtonVisualKey1 = verticalHotButtonWindow["VerticalHBWindowHotButton1"].VisualKey;
                                                Character.Settings.VerticalHotButtonVisualKey2 = verticalHotButtonWindow["VerticalHBWindowHotButton2"].VisualKey;
                                                Character.Settings.VerticalHotButtonVisualKey3 = verticalHotButtonWindow["VerticalHBWindowHotButton3"].VisualKey;
                                                Character.Settings.VerticalHotButtonVisualKey4 = verticalHotButtonWindow["VerticalHBWindowHotButton4"].VisualKey;
                                                Character.Settings.VerticalHotButtonVisualKey5 = verticalHotButtonWindow["VerticalHBWindowHotButton5"].VisualKey;
                                                Character.Settings.VerticalHotButtonVisualKey6 = verticalHotButtonWindow["VerticalHBWindowHotButton6"].VisualKey;
                                                Character.Settings.VerticalHotButtonVisualKey7 = verticalHotButtonWindow["VerticalHBWindowHotButton7"].VisualKey;
                                                Character.Settings.VerticalHotButtonVisualKey8 = verticalHotButtonWindow["VerticalHBWindowHotButton8"].VisualKey;
                                                Character.Settings.VerticalHotButtonVisualKey9 = verticalHotButtonWindow["VerticalHBWindowHotButton9"].VisualKey;
                                                Character.Settings.VerticalHotButtonVisualKey10 = verticalHotButtonWindow["VerticalHBWindowHotButton10"].VisualKey;
                                                Character.Settings.VerticalHotButtonVisualKey11 = verticalHotButtonWindow["VerticalHBWindowHotButton11"].VisualKey;
                                                Character.Settings.VerticalHotButtonVisualKey12 = verticalHotButtonWindow["VerticalHBWindowHotButton12"].VisualKey;
                                                Character.Settings.VerticalHotButtonVisualKey13 = verticalHotButtonWindow["VerticalHBWindowHotButton13"].VisualKey;
                                                Character.Settings.VerticalHotButtonVisualKey14 = verticalHotButtonWindow["VerticalHBWindowHotButton14"].VisualKey;
                                                Character.Settings.VerticalHotButtonVisualKey15 = verticalHotButtonWindow["VerticalHBWindowHotButton15"].VisualKey;
                                                Character.Settings.VerticalHotButtonVisualKey16 = verticalHotButtonWindow["VerticalHBWindowHotButton16"].VisualKey;
                                                Character.Settings.VerticalHotButtonVisualKey17 = verticalHotButtonWindow["VerticalHBWindowHotButton17"].VisualKey;
                                                Character.Settings.VerticalHotButtonVisualKey18 = verticalHotButtonWindow["VerticalHBWindowHotButton18"].VisualKey;
                                                Character.Settings.VerticalHotButtonVisualKey19 = verticalHotButtonWindow["VerticalHBWindowHotButton19"].VisualKey;

                                                Character.Settings.VerticalHotButtonText0 = verticalHotButtonWindow["VerticalHBWindowHotButton0"].Text;
                                                Character.Settings.VerticalHotButtonText1 = verticalHotButtonWindow["VerticalHBWindowHotButton1"].Text;
                                                Character.Settings.VerticalHotButtonText2 = verticalHotButtonWindow["VerticalHBWindowHotButton2"].Text;
                                                Character.Settings.VerticalHotButtonText3 = verticalHotButtonWindow["VerticalHBWindowHotButton3"].Text;
                                                Character.Settings.VerticalHotButtonText4 = verticalHotButtonWindow["VerticalHBWindowHotButton4"].Text;
                                                Character.Settings.VerticalHotButtonText5 = verticalHotButtonWindow["VerticalHBWindowHotButton5"].Text;
                                                Character.Settings.VerticalHotButtonText6 = verticalHotButtonWindow["VerticalHBWindowHotButton6"].Text;
                                                Character.Settings.VerticalHotButtonText7 = verticalHotButtonWindow["VerticalHBWindowHotButton7"].Text;
                                                Character.Settings.VerticalHotButtonText8 = verticalHotButtonWindow["VerticalHBWindowHotButton8"].Text;
                                                Character.Settings.VerticalHotButtonText9 = verticalHotButtonWindow["VerticalHBWindowHotButton9"].Text;
                                                Character.Settings.VerticalHotButtonText10 = verticalHotButtonWindow["VerticalHBWindowHotButton10"].Text;
                                                Character.Settings.VerticalHotButtonText11 = verticalHotButtonWindow["VerticalHBWindowHotButton11"].Text;
                                                Character.Settings.VerticalHotButtonText12 = verticalHotButtonWindow["VerticalHBWindowHotButton12"].Text;
                                                Character.Settings.VerticalHotButtonText13 = verticalHotButtonWindow["VerticalHBWindowHotButton13"].Text;
                                                Character.Settings.VerticalHotButtonText14 = verticalHotButtonWindow["VerticalHBWindowHotButton14"].Text;
                                                Character.Settings.VerticalHotButtonText15 = verticalHotButtonWindow["VerticalHBWindowHotButton15"].Text;
                                                Character.Settings.VerticalHotButtonText16 = verticalHotButtonWindow["VerticalHBWindowHotButton16"].Text;
                                                Character.Settings.VerticalHotButtonText17 = verticalHotButtonWindow["VerticalHBWindowHotButton17"].Text;
                                                Character.Settings.VerticalHotButtonText18 = verticalHotButtonWindow["VerticalHBWindowHotButton18"].Text;
                                                Character.Settings.VerticalHotButtonText19 = verticalHotButtonWindow["VerticalHBWindowHotButton19"].Text;

                                                Character.Settings.VerticalHotButtonCommand0 = (verticalHotButtonWindow["VerticalHBWindowHotButton0"] as HotButton).Command;
                                                Character.Settings.VerticalHotButtonCommand1 = (verticalHotButtonWindow["VerticalHBWindowHotButton1"] as HotButton).Command;
                                                Character.Settings.VerticalHotButtonCommand2 = (verticalHotButtonWindow["VerticalHBWindowHotButton2"] as HotButton).Command;
                                                Character.Settings.VerticalHotButtonCommand3 = (verticalHotButtonWindow["VerticalHBWindowHotButton3"] as HotButton).Command;
                                                Character.Settings.VerticalHotButtonCommand4 = (verticalHotButtonWindow["VerticalHBWindowHotButton4"] as HotButton).Command;
                                                Character.Settings.VerticalHotButtonCommand5 = (verticalHotButtonWindow["VerticalHBWindowHotButton5"] as HotButton).Command;
                                                Character.Settings.VerticalHotButtonCommand6 = (verticalHotButtonWindow["VerticalHBWindowHotButton6"] as HotButton).Command;
                                                Character.Settings.VerticalHotButtonCommand7 = (verticalHotButtonWindow["VerticalHBWindowHotButton7"] as HotButton).Command;
                                                Character.Settings.VerticalHotButtonCommand8 = (verticalHotButtonWindow["VerticalHBWindowHotButton8"] as HotButton).Command;
                                                Character.Settings.VerticalHotButtonCommand9 = (verticalHotButtonWindow["VerticalHBWindowHotButton9"] as HotButton).Command;
                                                Character.Settings.VerticalHotButtonCommand10 = (verticalHotButtonWindow["VerticalHBWindowHotButton10"] as HotButton).Command;
                                                Character.Settings.VerticalHotButtonCommand11 = (verticalHotButtonWindow["VerticalHBWindowHotButton11"] as HotButton).Command;
                                                Character.Settings.VerticalHotButtonCommand12 = (verticalHotButtonWindow["VerticalHBWindowHotButton12"] as HotButton).Command;
                                                Character.Settings.VerticalHotButtonCommand13 = (verticalHotButtonWindow["VerticalHBWindowHotButton13"] as HotButton).Command;
                                                Character.Settings.VerticalHotButtonCommand14 = (verticalHotButtonWindow["VerticalHBWindowHotButton14"] as HotButton).Command;
                                                Character.Settings.VerticalHotButtonCommand15 = (verticalHotButtonWindow["VerticalHBWindowHotButton15"] as HotButton).Command;
                                                Character.Settings.VerticalHotButtonCommand16 = (verticalHotButtonWindow["VerticalHBWindowHotButton16"] as HotButton).Command;
                                                Character.Settings.VerticalHotButtonCommand17 = (verticalHotButtonWindow["VerticalHBWindowHotButton17"] as HotButton).Command;
                                                Character.Settings.VerticalHotButtonCommand18 = (verticalHotButtonWindow["VerticalHBWindowHotButton18"] as HotButton).Command;
                                                Character.Settings.VerticalHotButtonCommand19 = (verticalHotButtonWindow["VerticalHBWindowHotButton19"] as HotButton).Command;

                                                hotButtonEditWindow.OnClose();

                                                TextCue.AddClientInfoTextCue("Hot Button Saved", Color.Lime, Color.Black, 1000);
                                            }
                                        }
                                        else // horizontal
                                        {
                                            if (GuiManager.GenericSheet["HorizontalHotButtonWindow"] is Window horizontallHotButtonWindow)
                                            {
                                                horizontallHotButtonWindow[hotButtonEditWindow.SelectedHotButton].VisualKey = hotButtonEditWindow.SelectedVisualKey;
                                                horizontallHotButtonWindow[hotButtonEditWindow.SelectedHotButton].Text = hotButtonEditWindow["HotButtonEditWindowTextBox"].Text;
                                                (horizontallHotButtonWindow[hotButtonEditWindow.SelectedHotButton] as HotButton).Command = hotButtonEditWindow["HotButtonEditWindowCommandTextBox"].Text;

                                                Character.Settings.HorizontalHotButtonVisualKey0 = horizontallHotButtonWindow["HorizontalHBWindowHotButton0"].VisualKey;
                                                Character.Settings.HorizontalHotButtonVisualKey1 = horizontallHotButtonWindow["HorizontalHBWindowHotButton1"].VisualKey;
                                                Character.Settings.HorizontalHotButtonVisualKey2 = horizontallHotButtonWindow["HorizontalHBWindowHotButton2"].VisualKey;
                                                Character.Settings.HorizontalHotButtonVisualKey3 = horizontallHotButtonWindow["HorizontalHBWindowHotButton3"].VisualKey;
                                                Character.Settings.HorizontalHotButtonVisualKey4 = horizontallHotButtonWindow["HorizontalHBWindowHotButton4"].VisualKey;
                                                Character.Settings.HorizontalHotButtonVisualKey5 = horizontallHotButtonWindow["HorizontalHBWindowHotButton5"].VisualKey;
                                                Character.Settings.HorizontalHotButtonVisualKey6 = horizontallHotButtonWindow["HorizontalHBWindowHotButton6"].VisualKey;
                                                Character.Settings.HorizontalHotButtonVisualKey7 = horizontallHotButtonWindow["HorizontalHBWindowHotButton7"].VisualKey;
                                                Character.Settings.HorizontalHotButtonVisualKey8 = horizontallHotButtonWindow["HorizontalHBWindowHotButton8"].VisualKey;
                                                Character.Settings.HorizontalHotButtonVisualKey9 = horizontallHotButtonWindow["HorizontalHBWindowHotButton9"].VisualKey;
                                                Character.Settings.HorizontalHotButtonVisualKey10 = horizontallHotButtonWindow["HorizontalHBWindowHotButton10"].VisualKey;
                                                Character.Settings.HorizontalHotButtonVisualKey11 = horizontallHotButtonWindow["HorizontalHBWindowHotButton11"].VisualKey;
                                                Character.Settings.HorizontalHotButtonVisualKey12 = horizontallHotButtonWindow["HorizontalHBWindowHotButton12"].VisualKey;
                                                Character.Settings.HorizontalHotButtonVisualKey13 = horizontallHotButtonWindow["HorizontalHBWindowHotButton13"].VisualKey;
                                                Character.Settings.HorizontalHotButtonVisualKey14 = horizontallHotButtonWindow["HorizontalHBWindowHotButton14"].VisualKey;

                                                Character.Settings.HorizontalHotButtonText0 = horizontallHotButtonWindow["HorizontalHBWindowHotButton0"].Text;
                                                Character.Settings.HorizontalHotButtonText1 = horizontallHotButtonWindow["HorizontalHBWindowHotButton1"].Text;
                                                Character.Settings.HorizontalHotButtonText2 = horizontallHotButtonWindow["HorizontalHBWindowHotButton2"].Text;
                                                Character.Settings.HorizontalHotButtonText3 = horizontallHotButtonWindow["HorizontalHBWindowHotButton3"].Text;
                                                Character.Settings.HorizontalHotButtonText4 = horizontallHotButtonWindow["HorizontalHBWindowHotButton4"].Text;
                                                Character.Settings.HorizontalHotButtonText5 = horizontallHotButtonWindow["HorizontalHBWindowHotButton5"].Text;
                                                Character.Settings.HorizontalHotButtonText6 = horizontallHotButtonWindow["HorizontalHBWindowHotButton6"].Text;
                                                Character.Settings.HorizontalHotButtonText7 = horizontallHotButtonWindow["HorizontalHBWindowHotButton7"].Text;
                                                Character.Settings.HorizontalHotButtonText8 = horizontallHotButtonWindow["HorizontalHBWindowHotButton8"].Text;
                                                Character.Settings.HorizontalHotButtonText9 = horizontallHotButtonWindow["HorizontalHBWindowHotButton9"].Text;
                                                Character.Settings.HorizontalHotButtonText10 = horizontallHotButtonWindow["HorizontalHBWindowHotButton10"].Text;
                                                Character.Settings.HorizontalHotButtonText11 = horizontallHotButtonWindow["HorizontalHBWindowHotButton11"].Text;
                                                Character.Settings.HorizontalHotButtonText12 = horizontallHotButtonWindow["HorizontalHBWindowHotButton12"].Text;
                                                Character.Settings.HorizontalHotButtonText13 = horizontallHotButtonWindow["HorizontalHBWindowHotButton13"].Text;
                                                Character.Settings.HorizontalHotButtonText14 = horizontallHotButtonWindow["HorizontalHBWindowHotButton14"].Text;

                                                Character.Settings.HorizontalHotButtonCommand0 = (horizontallHotButtonWindow["HorizontalHBWindowHotButton0"] as HotButton).Command;
                                                Character.Settings.HorizontalHotButtonCommand1 = (horizontallHotButtonWindow["HorizontalHBWindowHotButton1"] as HotButton).Command;
                                                Character.Settings.HorizontalHotButtonCommand2 = (horizontallHotButtonWindow["HorizontalHBWindowHotButton2"] as HotButton).Command;
                                                Character.Settings.HorizontalHotButtonCommand3 = (horizontallHotButtonWindow["HorizontalHBWindowHotButton3"] as HotButton).Command;
                                                Character.Settings.HorizontalHotButtonCommand4 = (horizontallHotButtonWindow["HorizontalHBWindowHotButton4"] as HotButton).Command;
                                                Character.Settings.HorizontalHotButtonCommand5 = (horizontallHotButtonWindow["HorizontalHBWindowHotButton5"] as HotButton).Command;
                                                Character.Settings.HorizontalHotButtonCommand6 = (horizontallHotButtonWindow["HorizontalHBWindowHotButton6"] as HotButton).Command;
                                                Character.Settings.HorizontalHotButtonCommand7 = (horizontallHotButtonWindow["HorizontalHBWindowHotButton7"] as HotButton).Command;
                                                Character.Settings.HorizontalHotButtonCommand8 = (horizontallHotButtonWindow["HorizontalHBWindowHotButton8"] as HotButton).Command;
                                                Character.Settings.HorizontalHotButtonCommand9 = (horizontallHotButtonWindow["HorizontalHBWindowHotButton9"] as HotButton).Command;
                                                Character.Settings.HorizontalHotButtonCommand10 = (horizontallHotButtonWindow["HorizontalHBWindowHotButton10"] as HotButton).Command;
                                                Character.Settings.HorizontalHotButtonCommand11 = (horizontallHotButtonWindow["HorizontalHBWindowHotButton11"] as HotButton).Command;
                                                Character.Settings.HorizontalHotButtonCommand12 = (horizontallHotButtonWindow["HorizontalHBWindowHotButton12"] as HotButton).Command;
                                                Character.Settings.HorizontalHotButtonCommand13 = (horizontallHotButtonWindow["HorizontalHBWindowHotButton13"] as HotButton).Command;
                                                Character.Settings.HorizontalHotButtonCommand14 = (horizontallHotButtonWindow["HorizontalHBWindowHotButton14"] as HotButton).Command;

                                                hotButtonEditWindow.OnClose();

                                                TextCue.AddClientInfoTextCue("Hot Button Saved", Color.Lime, Color.Black, 1000);
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }

                            Character.Settings.Save();
                        }
                        break;
                    case EventName.Save_Character_Settings:
                        RegisterEvent(EventName.Character_Settings_Changed);
                        break;
                    case EventName.TabControl:
                        try
                        {
                            TabControlButton button = (args[0] as TabControlButton);
                            if (button.TabControl != null)
                                button.TabControl.ConfirmOneTabWindowVisible(args[0] as TabControlButton);
                        }
                        catch(Exception e)
                        {
                            Utils.LogException(e);
                        }
                        break;
                    case EventName.Target_Cleared:
                        GameHUD.CurrentTarget = null;
                        if (Client.GameState.ToString().EndsWith("Game") && GameHUD.CurrentTarget != null)
                        {
                            TextCue.AddClientInfoTextCue("No Target", TextCue.TextCueTag.None, Color.Red,
                               Color.Black, 2000, false, false, false);
                            GameHUD.CurrentTarget = null;
                        }
                        break;
                    case EventName.Target_Select:
                        GameHUD.CurrentTarget = (Character) args[0];
                        if (GameHUD.CurrentTarget != null && Client.GameState.ToString().EndsWith("Game"))
                        {
                            string targetName = GameHUD.CurrentTarget.Name;

                            if (Char.IsDigit(targetName[0]))
                            {
                                targetName = targetName.Substring(targetName.IndexOf(" ") + 1);
                                    // remove the digit and space for grouped critters
                                targetName = targetName.Substring(0, targetName.Length - 1); // remove the s
                            }

                            TextCue.AddClientInfoTextCue("Target: " + targetName, TextCue.TextCueTag.None, Color.Red,
                                                         Color.Transparent, 2000, false, false, false);
                        }
                        break;
                    case EventName.Toggle_HorizontalHotbar: // ALT + K
                        if(GuiManager.GenericSheet["HorizontalHotButtonWindow"]  is Window horizontalHotbar)
                        {
                            horizontalHotbar.IsVisible = !horizontalHotbar.IsVisible;
                            if (horizontalHotbar.IsVisible) horizontalHotbar.ZDepth = 1;
                        }
                        break;
                    case EventName.Toggle_Macros: // ALT + M
                        if(GuiManager.GenericSheet["MacrosWindow"] is Window macrosWindow)                        
                        {
                            macrosWindow.IsVisible = !macrosWindow.IsVisible;
                            // TODO: functionality to edit macros with point and click
                            if (macrosWindow.IsVisible) macrosWindow.ZDepth = 1;
                        }
                        break;
                    case EventName.Toggle_OptionsWindow:
                        if (GuiManager.GenericSheet["OptionsWindow"] is Window optWindow)
                        {
                            if (!optWindow.IsVisible)
                                Events.RegisterEvent(Events.EventName.Load_Character_Settings);
                            optWindow.IsVisible = !optWindow.IsVisible;
                            optWindow.HasFocus = optWindow.IsVisible;
                            if (optWindow.IsVisible) optWindow.ZDepth = 1;
                        }
                        break;
                    case EventName.Toggle_VerticalHotbar:
                        if (GuiManager.GenericSheet["VerticalHotButtonWindow"] is Window verticalHotbar)
                        {
                            verticalHotbar.IsVisible = !verticalHotbar.IsVisible;
                            if (verticalHotbar.IsVisible) verticalHotbar.ZDepth = 1;
                        }
                        break;
                    case EventName.Load_Character_Settings:
                        if (Character.Settings != null)
                        {
                            #region Options Window
                            if (GuiManager.GenericSheet["OptionsWindow"] is Window optionsWindow)
                            {
                                if (optionsWindow["DoubleLeftClickNearbyTargetTextBox"] is TextBox tbx)
                                    tbx.Text = Character.Settings.DoubleLeftClickNearbyTarget;
                                tbx = optionsWindow["DoubleLeftClickDistantTargetTextBox"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.DoubleLeftClickDistantTarget;
                                // Number Pad Keyboard Mapping
                                tbx = optionsWindow["OptionsNumPadDivideTextBox"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.NumPadDivide;
                                tbx = optionsWindow["OptionsNumPadMultiplyTextBox"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.NumPadMultiply;
                                tbx = optionsWindow["OptionsNumPadSubtractTextBox"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.NumPadSubtract;
                                tbx = optionsWindow["OptionsNumPadAddTextBox"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.NumPadAdd;
                                tbx = optionsWindow["OptionsNumPadZeroTextBox"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.NumLock0;
                                tbx = optionsWindow["OptionsNumPadFiveTextBox"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.NumLock5;
                                tbx = optionsWindow["OptionsNumPadDeleteTextBox"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.NumPadDelete;
                                tbx = optionsWindow["OptionsCritterDropDownTextBox1"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.CritterListDropDownMenuItem1;
                                tbx = optionsWindow["OptionsCritterDropDownTextBox2"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.CritterListDropDownMenuItem2;
                                tbx = optionsWindow["OptionsCritterDropDownTextBox3"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.CritterListDropDownMenuItem3;
                                tbx = optionsWindow["OptionsCritterDropDownTextBox4"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.CritterListDropDownMenuItem4;
                                tbx = optionsWindow["OptionsCritterDropDownTextBox5"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.CritterListDropDownMenuItem5;
                                // Function Key Mapping
                                tbx = optionsWindow["OptionsFunctionKey1TextBox"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.FunctionKey1;
                                tbx = optionsWindow["OptionsFunctionKey2TextBox"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.FunctionKey2;
                                tbx = optionsWindow["OptionsFunctionKey3TextBox"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.FunctionKey3;
                                tbx = optionsWindow["OptionsFunctionKey4TextBox"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.FunctionKey4;
                                tbx = optionsWindow["OptionsFunctionKey5TextBox"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.FunctionKey5;
                                tbx = optionsWindow["OptionsFunctionKey6TextBox"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.FunctionKey6;
                                tbx = optionsWindow["OptionsFunctionKey7TextBox"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.FunctionKey7;
                                tbx = optionsWindow["OptionsFunctionKey8TextBox"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.FunctionKey8;
                                tbx = optionsWindow["OptionsFunctionKey9TextBox"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.FunctionKey9;
                                tbx = optionsWindow["OptionsFunctionKey10TextBox"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.FunctionKey10;
                                tbx = optionsWindow["OptionsFunctionKey11TextBox"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.FunctionKey11;
                                tbx = optionsWindow["OptionsFunctionKey12TextBox"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.FunctionKey12;
                            }

                            #endregion

                            #region Vertical Hot Buttons
                            if (GuiManager.GenericSheet["VerticalHotButtonWindow"] is Window verticalHotButtonWindow)
                            {
                                verticalHotButtonWindow["VerticalHBWindowHotButton0"].VisualKey = Character.Settings.VerticalHotButtonVisualKey0;
                                verticalHotButtonWindow["VerticalHBWindowHotButton1"].VisualKey = Character.Settings.VerticalHotButtonVisualKey1;
                                verticalHotButtonWindow["VerticalHBWindowHotButton2"].VisualKey = Character.Settings.VerticalHotButtonVisualKey2;
                                verticalHotButtonWindow["VerticalHBWindowHotButton3"].VisualKey = Character.Settings.VerticalHotButtonVisualKey3;
                                verticalHotButtonWindow["VerticalHBWindowHotButton4"].VisualKey = Character.Settings.VerticalHotButtonVisualKey4;
                                verticalHotButtonWindow["VerticalHBWindowHotButton5"].VisualKey = Character.Settings.VerticalHotButtonVisualKey5;
                                verticalHotButtonWindow["VerticalHBWindowHotButton6"].VisualKey = Character.Settings.VerticalHotButtonVisualKey6;
                                verticalHotButtonWindow["VerticalHBWindowHotButton7"].VisualKey = Character.Settings.VerticalHotButtonVisualKey7;
                                verticalHotButtonWindow["VerticalHBWindowHotButton8"].VisualKey = Character.Settings.VerticalHotButtonVisualKey8;
                                verticalHotButtonWindow["VerticalHBWindowHotButton9"].VisualKey = Character.Settings.VerticalHotButtonVisualKey9;
                                verticalHotButtonWindow["VerticalHBWindowHotButton10"].VisualKey = Character.Settings.VerticalHotButtonVisualKey10;
                                verticalHotButtonWindow["VerticalHBWindowHotButton11"].VisualKey = Character.Settings.VerticalHotButtonVisualKey11;
                                verticalHotButtonWindow["VerticalHBWindowHotButton12"].VisualKey = Character.Settings.VerticalHotButtonVisualKey12;
                                verticalHotButtonWindow["VerticalHBWindowHotButton13"].VisualKey = Character.Settings.VerticalHotButtonVisualKey13;
                                verticalHotButtonWindow["VerticalHBWindowHotButton14"].VisualKey = Character.Settings.VerticalHotButtonVisualKey14;
                                verticalHotButtonWindow["VerticalHBWindowHotButton15"].VisualKey = Character.Settings.VerticalHotButtonVisualKey15;
                                verticalHotButtonWindow["VerticalHBWindowHotButton16"].VisualKey = Character.Settings.VerticalHotButtonVisualKey16;
                                verticalHotButtonWindow["VerticalHBWindowHotButton17"].VisualKey = Character.Settings.VerticalHotButtonVisualKey17;
                                verticalHotButtonWindow["VerticalHBWindowHotButton18"].VisualKey = Character.Settings.VerticalHotButtonVisualKey18;
                                verticalHotButtonWindow["VerticalHBWindowHotButton19"].VisualKey = Character.Settings.VerticalHotButtonVisualKey19;

                                verticalHotButtonWindow["VerticalHBWindowHotButton0"].Text = Character.Settings.VerticalHotButtonText0;
                                verticalHotButtonWindow["VerticalHBWindowHotButton1"].Text = Character.Settings.VerticalHotButtonText1;
                                verticalHotButtonWindow["VerticalHBWindowHotButton2"].Text = Character.Settings.VerticalHotButtonText2;
                                verticalHotButtonWindow["VerticalHBWindowHotButton3"].Text = Character.Settings.VerticalHotButtonText3;
                                verticalHotButtonWindow["VerticalHBWindowHotButton4"].Text = Character.Settings.VerticalHotButtonText4;
                                verticalHotButtonWindow["VerticalHBWindowHotButton5"].Text = Character.Settings.VerticalHotButtonText5;
                                verticalHotButtonWindow["VerticalHBWindowHotButton6"].Text = Character.Settings.VerticalHotButtonText6;
                                verticalHotButtonWindow["VerticalHBWindowHotButton7"].Text = Character.Settings.VerticalHotButtonText7;
                                verticalHotButtonWindow["VerticalHBWindowHotButton8"].Text = Character.Settings.VerticalHotButtonText8;
                                verticalHotButtonWindow["VerticalHBWindowHotButton9"].Text = Character.Settings.VerticalHotButtonText9;
                                verticalHotButtonWindow["VerticalHBWindowHotButton10"].Text = Character.Settings.VerticalHotButtonText10;
                                verticalHotButtonWindow["VerticalHBWindowHotButton11"].Text = Character.Settings.VerticalHotButtonText11;
                                verticalHotButtonWindow["VerticalHBWindowHotButton12"].Text = Character.Settings.VerticalHotButtonText12;
                                verticalHotButtonWindow["VerticalHBWindowHotButton13"].Text = Character.Settings.VerticalHotButtonText13;
                                verticalHotButtonWindow["VerticalHBWindowHotButton14"].Text = Character.Settings.VerticalHotButtonText14;
                                verticalHotButtonWindow["VerticalHBWindowHotButton15"].Text = Character.Settings.VerticalHotButtonText15;
                                verticalHotButtonWindow["VerticalHBWindowHotButton16"].Text = Character.Settings.VerticalHotButtonText16;
                                verticalHotButtonWindow["VerticalHBWindowHotButton17"].Text = Character.Settings.VerticalHotButtonText17;
                                verticalHotButtonWindow["VerticalHBWindowHotButton18"].Text = Character.Settings.VerticalHotButtonText18;
                                verticalHotButtonWindow["VerticalHBWindowHotButton19"].Text = Character.Settings.VerticalHotButtonText19;

                                (verticalHotButtonWindow["VerticalHBWindowHotButton0"] as HotButton).Command = Character.Settings.VerticalHotButtonCommand0;
                                (verticalHotButtonWindow["VerticalHBWindowHotButton1"] as HotButton).Command = Character.Settings.VerticalHotButtonCommand1;
                                (verticalHotButtonWindow["VerticalHBWindowHotButton2"] as HotButton).Command = Character.Settings.VerticalHotButtonCommand2;
                                (verticalHotButtonWindow["VerticalHBWindowHotButton3"] as HotButton).Command = Character.Settings.VerticalHotButtonCommand3;
                                (verticalHotButtonWindow["VerticalHBWindowHotButton4"] as HotButton).Command = Character.Settings.VerticalHotButtonCommand4;
                                (verticalHotButtonWindow["VerticalHBWindowHotButton5"] as HotButton).Command = Character.Settings.VerticalHotButtonCommand5;
                                (verticalHotButtonWindow["VerticalHBWindowHotButton6"] as HotButton).Command = Character.Settings.VerticalHotButtonCommand6;
                                (verticalHotButtonWindow["VerticalHBWindowHotButton7"] as HotButton).Command = Character.Settings.VerticalHotButtonCommand7;
                                (verticalHotButtonWindow["VerticalHBWindowHotButton8"] as HotButton).Command = Character.Settings.VerticalHotButtonCommand8;
                                (verticalHotButtonWindow["VerticalHBWindowHotButton9"] as HotButton).Command = Character.Settings.VerticalHotButtonCommand9;
                                (verticalHotButtonWindow["VerticalHBWindowHotButton10"] as HotButton).Command = Character.Settings.VerticalHotButtonCommand10;
                                (verticalHotButtonWindow["VerticalHBWindowHotButton11"] as HotButton).Command = Character.Settings.VerticalHotButtonCommand11;
                                (verticalHotButtonWindow["VerticalHBWindowHotButton12"] as HotButton).Command = Character.Settings.VerticalHotButtonCommand12;
                                (verticalHotButtonWindow["VerticalHBWindowHotButton13"] as HotButton).Command = Character.Settings.VerticalHotButtonCommand13;
                                (verticalHotButtonWindow["VerticalHBWindowHotButton14"] as HotButton).Command = Character.Settings.VerticalHotButtonCommand14;
                                (verticalHotButtonWindow["VerticalHBWindowHotButton15"] as HotButton).Command = Character.Settings.VerticalHotButtonCommand15;
                                (verticalHotButtonWindow["VerticalHBWindowHotButton16"] as HotButton).Command = Character.Settings.VerticalHotButtonCommand16;
                                (verticalHotButtonWindow["VerticalHBWindowHotButton17"] as HotButton).Command = Character.Settings.VerticalHotButtonCommand17;
                                (verticalHotButtonWindow["VerticalHBWindowHotButton18"] as HotButton).Command = Character.Settings.VerticalHotButtonCommand18;
                                (verticalHotButtonWindow["VerticalHBWindowHotButton19"] as HotButton).Command = Character.Settings.VerticalHotButtonCommand19;
                            }
                            #endregion

                            #region Horizontal Hot Buttons
                            if (GuiManager.GenericSheet["HorizontalHotButtonWindow"] is Window horizontalHotButtonWindow)
                            {
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton0"].VisualKey = Character.Settings.HorizontalHotButtonVisualKey0;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton1"].VisualKey = Character.Settings.HorizontalHotButtonVisualKey1;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton2"].VisualKey = Character.Settings.HorizontalHotButtonVisualKey2;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton3"].VisualKey = Character.Settings.HorizontalHotButtonVisualKey3;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton4"].VisualKey = Character.Settings.HorizontalHotButtonVisualKey4;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton5"].VisualKey = Character.Settings.HorizontalHotButtonVisualKey5;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton6"].VisualKey = Character.Settings.HorizontalHotButtonVisualKey6;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton7"].VisualKey = Character.Settings.HorizontalHotButtonVisualKey7;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton8"].VisualKey = Character.Settings.HorizontalHotButtonVisualKey8;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton9"].VisualKey = Character.Settings.HorizontalHotButtonVisualKey9;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton10"].VisualKey = Character.Settings.HorizontalHotButtonVisualKey10;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton11"].VisualKey = Character.Settings.HorizontalHotButtonVisualKey11;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton12"].VisualKey = Character.Settings.HorizontalHotButtonVisualKey12;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton13"].VisualKey = Character.Settings.HorizontalHotButtonVisualKey13;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton14"].VisualKey = Character.Settings.HorizontalHotButtonVisualKey14;

                                horizontalHotButtonWindow["HorizontalHBWindowHotButton0"].Text = Character.Settings.HorizontalHotButtonText0;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton1"].Text = Character.Settings.HorizontalHotButtonText1;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton2"].Text = Character.Settings.HorizontalHotButtonText2;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton3"].Text = Character.Settings.HorizontalHotButtonText3;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton4"].Text = Character.Settings.HorizontalHotButtonText4;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton5"].Text = Character.Settings.HorizontalHotButtonText5;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton6"].Text = Character.Settings.HorizontalHotButtonText6;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton7"].Text = Character.Settings.HorizontalHotButtonText7;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton8"].Text = Character.Settings.HorizontalHotButtonText8;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton9"].Text = Character.Settings.HorizontalHotButtonText9;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton10"].Text = Character.Settings.HorizontalHotButtonText10;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton11"].Text = Character.Settings.HorizontalHotButtonText11;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton12"].Text = Character.Settings.HorizontalHotButtonText12;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton13"].Text = Character.Settings.HorizontalHotButtonText13;
                                horizontalHotButtonWindow["HorizontalHBWindowHotButton14"].Text = Character.Settings.HorizontalHotButtonText14;

                                (horizontalHotButtonWindow["HorizontalHBWindowHotButton0"] as HotButton).Command = Character.Settings.HorizontalHotButtonCommand0;
                                (horizontalHotButtonWindow["HorizontalHBWindowHotButton1"] as HotButton).Command = Character.Settings.HorizontalHotButtonCommand1;
                                (horizontalHotButtonWindow["HorizontalHBWindowHotButton2"] as HotButton).Command = Character.Settings.HorizontalHotButtonCommand2;
                                (horizontalHotButtonWindow["HorizontalHBWindowHotButton3"] as HotButton).Command = Character.Settings.HorizontalHotButtonCommand3;
                                (horizontalHotButtonWindow["HorizontalHBWindowHotButton4"] as HotButton).Command = Character.Settings.HorizontalHotButtonCommand4;
                                (horizontalHotButtonWindow["HorizontalHBWindowHotButton5"] as HotButton).Command = Character.Settings.HorizontalHotButtonCommand5;
                                (horizontalHotButtonWindow["HorizontalHBWindowHotButton6"] as HotButton).Command = Character.Settings.HorizontalHotButtonCommand6;
                                (horizontalHotButtonWindow["HorizontalHBWindowHotButton7"] as HotButton).Command = Character.Settings.HorizontalHotButtonCommand7;
                                (horizontalHotButtonWindow["HorizontalHBWindowHotButton8"] as HotButton).Command = Character.Settings.HorizontalHotButtonCommand8;
                                (horizontalHotButtonWindow["HorizontalHBWindowHotButton9"] as HotButton).Command = Character.Settings.HorizontalHotButtonCommand9;
                                (horizontalHotButtonWindow["HorizontalHBWindowHotButton10"] as HotButton).Command = Character.Settings.HorizontalHotButtonCommand10;
                                (horizontalHotButtonWindow["HorizontalHBWindowHotButton11"] as HotButton).Command = Character.Settings.HorizontalHotButtonCommand11;
                                (horizontalHotButtonWindow["HorizontalHBWindowHotButton12"] as HotButton).Command = Character.Settings.HorizontalHotButtonCommand12;
                                (horizontalHotButtonWindow["HorizontalHBWindowHotButton13"] as HotButton).Command = Character.Settings.HorizontalHotButtonCommand13;
                                (horizontalHotButtonWindow["HorizontalHBWindowHotButton14"] as HotButton).Command = Character.Settings.HorizontalHotButtonCommand14;
                            }
                            #endregion
                        }
                        break;
                    case EventName.Load_Client_Settings:
                        break;
                }
            }
            catch (Exception e)
            {
                Utils.LogException(e);
            }
        }

        public static void UpdateGUI(GameTime gameTime)
        {
            Character chr = Character.CurrentCharacter;
            Enums.EGameState state = Client.GameState;
            Sheet sheet = GuiManager.Sheets[state.ToString()];

            // set client title
            Client.Title = string.Format("{0} (v{1})", Utility.Settings.StaticSettings.ClientName, Utility.Settings.StaticSettings.ClientVersion);

            // add character info to title if logged in and character is active
            if (chr != null && Client.GameState != Enums.EGameState.Login)
                Client.Title.Insert(0, chr.Name + ": ");

            if (sheet != null)
            {
                switch (state)
                {
                    case Enums.EGameState.Login:
                        #region Login

                        Button cb = (sheet["LoginWindow"] as gui.Window)["ConnectButton"] as Button;
                        TextBox at = (sheet["LoginWindow"] as gui.Window)["AccountTextBox"] as TextBox;
                        TextBox pt = (sheet["LoginWindow"] as gui.Window)["PasswordTextBox"] as TextBox;
                        TextBox sh = (sheet["LoginWindow"] as gui.Window)["ServerHostTextBox"] as TextBox;
                        Button na = (sheet["LoginWindow"] as gui.Window)["CreateNewAccountButton"] as Button;
                        CheckboxButton rcheck = (sheet["LoginWindow"] as gui.Window)["RememberPasswordCheckboxButton"] as CheckboxButton;

                        if (IO.LoginState != Enums.ELoginState.Disconnected)
                        {
                            if (cb != null)
                                cb.IsVisible = false;
                            if (at != null)
                                at.IsDisabled = true;
                            if (pt != null)
                                pt.IsDisabled = true;
                            if(sh != null)
                                sh.IsDisabled = true;
                            if (rcheck != null)
                                rcheck.IsDisabled = true;
                            if (na != null)
                                na.IsVisible = false;
                        }
                        else
                        {
                            if (at != null)
                            {
                                at.IsDisabled = false;

                                if (Client.ClientSettings.ContainsStoredAccount(Utility.Encrypt.EncryptString(at.Text, Utility.Settings.StaticSettings.DecryptionPassPhrase1), out Utility.Encrypt.EncryptedKeyValuePair<string, string> kvPair))
                                {
                                    string unencryptedPassword = Utility.Encrypt.DecryptString(kvPair.Value, Utility.Settings.StaticSettings.DecryptionPassPhrase2);
                                    if (pt.Text != unencryptedPassword)
                                    {
                                        at.SelectAll();
                                        pt.Text = Utility.Encrypt.DecryptString(kvPair.Value, Utility.Settings.StaticSettings.DecryptionPassPhrase2);
                                        pt.SelectAll();
                                        pt.HasFocus = true;
                                        if (rcheck != null) rcheck.IsChecked = true;
                                    }
                                }
                            }
                            if (pt != null)
                                pt.IsDisabled = false;
                            if(sh != null)
                                sh.IsDisabled = false;
                            if (cb != null && !cb.IsVisible && na != null) na.IsVisible = true;

                            if (cb != null && at != null && pt != null && sh != null)
                            {
                                // there is text in all text boxes, make the connect button visible
                                if (at.Text.Length > 0 && pt.Text.Length > 0 && sh.Text.Length > 0)
                                {
                                    na.IsVisible = false;
                                    cb.IsVisible = true;
                                }
                                else
                                {
                                    cb.IsVisible = false;
                                    if (IO.LoginState == Enums.ELoginState.Disconnected)
                                        na.IsVisible = true;
                                }
                            }

                            if (rcheck != null)
                                rcheck.IsDisabled = false;

                            if (GuiManager.CurrentSheet["LoginStatusLabel"] is Label lsl && lsl.TextColor != Color.Red)
                                lsl.Text = "";
                        }
                        break; 
                        #endregion
                    case Enums.EGameState.Menu:
                        #region Menu
                        if (Character.CurrentCharacter != null)
                        {
                            if (sheet["CurrentPictureLabel"] != null)
                                sheet["CurrentPictureLabel"].VisualKey = chr.VisualKey;
                            if (sheet["CurrentNameLabel"] != null)
                                sheet["CurrentNameLabel"].Text = chr.Name;
                            if (sheet["CurrentLevelLabel"] != null)
                                sheet["CurrentLevelLabel"].Text = string.Format("Level {0} {1}", chr.Level, chr.ClassFullName);
                            if (sheet["CurrentLocationLabel"] != null)
                                sheet["CurrentLocationLabel"].Text = chr.MapName;
                        }
                        break;
                    #endregion
                    case Enums.EGameState.CharacterGeneration:
                        #region Character Generation
                        break; 
                        #endregion
                    case Enums.EGameState.Conference:
                        #region Conference
                        if (chr != null)
                        {
                            if (sheet["CurrentPictureLabel"] != null)
                                sheet["CurrentPictureLabel"].VisualKey = chr.VisualKey;
                            if (sheet["CurrentNameLabel"] != null)
                                sheet["CurrentNameLabel"].Text = chr.Name;
                            if (sheet["CurrentLevelLabel"] != null)
                                sheet["CurrentLevelLabel"].Text = string.Format("Level {0} {1}", chr.Level, chr.ClassFullName);
                            if (sheet["CurrentLocationLabel"] != null)
                                sheet["CurrentLocationLabel"].Text = chr.MapName;
                        }

                        if (sheet["ConfInputTextBox"] != null)
                        {
                            if (Client.HasFocus)
                            {
                                // Place focus on the input text box.
                                if (sheet[Globals.CONFINPUTTEXTBOX] != null)
                                {
                                    // Overrides to focus on input text box.
                                    // Options window and private messages have focus priority.
                                    if (!GuiManager.GenericSheet["OptionsWindow"].IsVisible && !GuiManager.ControlWithFocus.Name.Contains("PrivateMessage"))
                                        sheet[Globals.CONFINPUTTEXTBOX].HasFocus = true;
                                }
                            }
                            else
                            {
                                sheet["ConfInputTextBox"].HasFocus = false;
                            }
                        }

                        break; 
                        #endregion
                    case Enums.EGameState.Game:
                        break;
                    case Enums.EGameState.SpinelGame:
                        SpinelMode.UpdateGUI(gameTime, sheet);
                        break;
                    case Enums.EGameState.IOKGame:
                        IOKMode.UpdateGUI(gameTime, sheet);
                        break;
                }
            }
        }

        public static void ResetLoginGUI()
        {
            try
            {
                foreach (Control c in new List<Control>(GuiManager.GenericSheet.Controls))
                {
                    if (c is Window)
                        (c as Window).OnClose();
                }

                Program.Client.GUIManager.LoadSheet(gui.GuiManager.GenericSheet.FilePath);
                Program.Client.GUIManager.LoadSheet(gui.GuiManager.CurrentSheet.FilePath);

                if (Client.IsFullScreen)
                {
                    GuiManager.GenericSheet.OnClientResize(Client.PrevClientBounds, Client.NowClientBounds);
                    GuiManager.CurrentSheet.OnClientResize(Client.PrevClientBounds, Client.NowClientBounds);
                }

                if (GuiManager.Sheets["Login"]["ServerHostTextBox"] is TextBox serverHostTextBox)
                {
                    serverHostTextBox.Clear();
                    serverHostTextBox.AddText(Client.ClientSettings.ServerHost);
                    serverHostTextBox.SelectAll();
                }

                if (Client.ClientSettings.MostRecentStoredAccount != "")
                {
                    if (Client.ClientSettings.ContainsStoredAccount(Utility.Encrypt.DecryptString(Client.ClientSettings.MostRecentStoredAccount, Utility.Settings.StaticSettings.DecryptionPassPhrase3), out Utility.Encrypt.EncryptedKeyValuePair<string, string> kvPair))
                    {
                        TextBox at = GuiManager.Sheets["Login"]["AccountTextBox"] as TextBox;
                        TextBox pt = GuiManager.Sheets["Login"]["PasswordTextBox"] as TextBox;
                        at.Text = Utility.Encrypt.DecryptString(kvPair.Key, Utility.Settings.StaticSettings.DecryptionPassPhrase1);
                        at.SelectAll();
                        pt.Text = Utility.Encrypt.DecryptString(kvPair.Value, Utility.Settings.StaticSettings.DecryptionPassPhrase2);
                        pt.SelectAll();

                        (GuiManager.Sheets["Login"]["RememberPasswordCheckboxButton"] as CheckboxButton).IsChecked = true;

                        //GuiManager.Sheets["Login"]["ConnectButton"].HasFocus = true;
                    }
                }
                else GuiManager.Sheets["Login"]["AccountTextBox"].HasFocus = true;
            }
            catch(Exception e)
            {
                Utils.LogException(e);
            }
        }

        /// <summary>
        /// Called when setting a new game state.
        /// </summary>
        /// <param name="currGameState">The current game state.</param>
        public static void OnSwitchGameState(Enums.EGameState currGameState, Enums.EGameState newGameState)
        {
            Control control = null;

            try
            {
                switch (currGameState)
                {
                    case Enums.EGameState.Conference:
                        if (!GameHUD.OverrideDisplayStates.Contains(newGameState))
                        {
                            control = GuiManager.GetControl("ConfScrollableTextBox");
                            if (control != null)
                                (control as ScrollableTextBox).Clear();
                        }
                        control = GuiManager.GetControl("ConfInputTextBox");
                        if (control != null)
                            (control as TextBox).Clear();
                        break;
                    case Enums.EGameState.Game:
                        GuiManager.TextCues.Clear();
                        break;
                    case Enums.EGameState.IOKGame:
                        if (newGameState == Enums.EGameState.SpinelGame)
                            gui.SpinelMode.UpdateGUI(Program.Client.ClientGameTime, GuiManager.Sheets[newGameState.ToString()]);
                        else
                        {
                            control = GuiManager.GetControl(Globals.GAMEINPUTTEXTBOX);
                            if (control != null)
                            {
                                (control as TextBox).Clear();
                                control.HasFocus = true;
                            }
                        }
                        GuiManager.TextCues.Clear();
                        TextCue.ClearMouseCursorTextCue();
                        break;
                    case Enums.EGameState.Login:
                        if(GameHUD.PreviousGameState != Enums.EGameState.Login)
                            Events.ResetLoginGUI();
                        break;
                    case Enums.EGameState.LOKGame:
                        GuiManager.TextCues.Clear();
                        break;
                    case Enums.EGameState.HotButtonEditMode:
                        break;
                    case Enums.EGameState.Menu:
                        break;
                    case Enums.EGameState.SpinelGame:
                        if (newGameState == Enums.EGameState.IOKGame)
                        {
                            gui.IOKMode.UpdateGUI(Program.Client.ClientGameTime, GuiManager.Sheets[newGameState.ToString()]);
                        }
                        else
                        {
                            control = GuiManager.GetControl(Globals.GAMEINPUTTEXTBOX);
                            if (control != null)
                            {
                                (control as TextBox).Clear();
                                control.HasFocus = true;
                            }
                        }
                        GuiManager.TextCues.Clear();
                        TextCue.ClearMouseCursorTextCue();
                        break;
                    //case Enums.eGameState.Splash:
                    //    Events.ResetLoginGUI();
                    //break;
                    default:
                        break;
                }

                if(currGameState != Enums.EGameState.HotButtonEditMode)
                    GameHUD.PreviousGameState = currGameState;
            }
            catch (Exception e)
            {
                Utils.LogException(e);
            }
        }
    }
}
