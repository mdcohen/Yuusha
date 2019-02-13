using System;
using System.Collections.Generic;
using System.Text;
using Yuusha.gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;

namespace Yuusha
{
    public static class Events
    {
        public enum EventName
        {
            Attack_Critter,
            Connect,
            Disconnect,
            Display_Conference_Text,
            Display_Game_Text,
            End_Game_Round,
            Format_Cell,
            Goto_Conf,
            Goto_Game,
            Goto_Menu,
            LoadLOKMap,
            New_Game_Round,
            NextLOKTile,
            Next_Visual,
            CreateNewAccount,
            None,
            SaveLOKMap,
            Send_Account_Name,
            Send_Macro,
            Send_Password,
            Send_Text,
            Set_Client_Mode,
            Set_Game_State,
            Set_Login_State,
            Set_Login_Status_Label,
            Switch_Character_Next,
            Switch_Character_Back,
            Client_Settings_Changed,
            User_Settings_Changed,
            Character_Settings_Changed,
            Target_Cleared,
            Target_Select,
            Save_Character_Settings,
            Load_Character_Settings,
            Load_Client_Settings,
            Character_Death,
            Send_Command
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
                                Control control = (Control) args[0];

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
                            int id = 0;
                            if (Int32.TryParse(args[0].ToString(), out id))
                            {
                                // currently just clear target
                                if (GameHUD.CurrentTarget != null && GameHUD.CurrentTarget.ID == id)
                                    Events.RegisterEvent(EventName.Target_Cleared, null);
                            }
                        }
                        break; 
                    #endregion
                    case EventName.Connect:
                        #region Connect

                        // TEMPORARY ?? Why is/was this temporary? (10/27/11 -MDC)
                        if ((GuiManager.CurrentSheet["LoginWindow"] as gui.Window)["AccountTextBox"].Text.Length > 0 &&
                            (GuiManager.CurrentSheet["LoginWindow"] as gui.Window)["PasswordTextBox"].Text.Length > 0)
                        {
                            TextBox serverHostTextBox =
                                (GuiManager.CurrentSheet["LoginWindow"] as gui.Window)["ServerHostTextBox"] as TextBox;

                            string serverHostAddress = Client.ClientSettings.ServerHost;

                            if (serverHostTextBox != null)
                            {
                                if (serverHostTextBox.Text != Client.ClientSettings.ServerHost)
                                {
                                    Client.ClientSettings.ServerHost = serverHostTextBox.Text;
                                }
                            }

                            //gui.TextCue.AddClientInfoTextCue("Connect", "", Color.Red, Color.Transparent, 5000, false, false, true);

                            if (IO.Connect())
                            {
                                RegisterEvent(EventName.Set_Login_Status_Label,
                                              "Connecting to " + Client.ClientSettings.ServerName + "...", "LimeGreen");
                                RegisterEvent(EventName.Set_Login_State, Enums.ELoginState.Connected);

                                // connected to another host address, save the setting
                                if (serverHostAddress != Client.ClientSettings.ServerHost)
                                {
                                    Events.RegisterEvent(EventName.Client_Settings_Changed);
                                }
                            }
                            else
                            {
                                RegisterEvent(EventName.Set_Login_Status_Label, "Failed to connect.", "Red");
                            }
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
                    case EventName.Display_Conference_Text:
                        #region Display Conference Text

                        (GuiManager.CurrentSheet["ConfScrollableTextBox"] as gui.ScrollableTextBox).AddLine(
                            (string) args[0], (Enums.ETextType) args[1]);
                        break;

                        #endregion
                    case EventName.Display_Game_Text:
                        #region Display Game Text

                        switch (Client.GameDisplayMode)
                        {
                            case Enums.EGameDisplayMode.Spinel:
                                gui.SpinelMode.DisplayGameText((string) args[0], Enums.ETextType.Default);
                                if ((string) args[0] == TextManager.YOU_ARE_STUNNED)
                                    gui.TextCue.AddPromptStateTextCue(Protocol.PromptStates.Stunned);
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
                                                TextCue.AddChantingTextCue(spell.Incantation);
                                                break;
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
                                if (Client.ClientSettings.DisplayChantingTextCue)
                                {
                                    if (args[0].ToString().StartsWith("You warm the spell "))
                                    {
                                        string findSpellName = args[0].ToString().Replace("You warm the spell ", "");
                                        findSpellName = findSpellName.Replace(".", "");
                                        foreach (Spell spell in Character.CurrentCharacter.Spells)
                                            if (spell.Name == findSpellName)
                                                TextCue.AddChantingTextCue(spell.Incantation);
                                    }
                                }
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
                    case EventName.LoadLOKMap:
                        break;
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
                                int currentNum = 0;
                                if (Int32.TryParse(visualName, out currentNum))
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
                                int currentNum = 0;
                                if (Int32.TryParse(visualName, out currentNum))
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

                            TextCue.AddCursorTextCue(VisualKeyWindowButton.VisualKey, Color.Yellow, "courier16");
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
                    case EventName.SaveLOKMap:
                        break;
                    case EventName.Send_Account_Name:
                        IO.Send((GuiManager.CurrentSheet["LoginWindow"] as gui.Window)["AccountTextBox"].Text);
                        break;
                    case EventName.Send_Macro:
                        break;
                    case EventName.Send_Password:
                        IO.Send((GuiManager.CurrentSheet["LoginWindow"] as gui.Window)["PasswordTextBox"].Text);
                        break;
                    case EventName.Send_Text:
                        #region Send Text

                        {
                            string textToSend = (args[0] as Control).Text;
                            if (Client.GameState.ToString().EndsWith("Game") && GameHUD.CurrentTarget != null)
                            {
                                textToSend = textToSend.Replace("%t", GameHUD.CurrentTarget.ID.ToString());
                            }

                            if (textToSend != "")
                            {
                                IO.Send(textToSend);
                            }
                            break;
                        }
                    #endregion
                    case EventName.Send_Command:
                        if ((args[0] as Control) is Button)
                        {
                            IO.Send((args[0] as Button).Command);
                        }
                        break;
                    case EventName.Set_Client_Mode:
                        Client.GameDisplayMode = (Enums.EGameDisplayMode) args[0];
                        RegisterEvent(EventName.Set_Game_State, Enums.EGameState.Game);
                        break;
                    case EventName.Set_Game_State:

                        #region Set Game State

                        //if (Client.GameState == (Enums.eGameState)args[0])
                        //    break;

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

                            if (Client.GameState.ToString().EndsWith("Game"))
                                Events.RegisterEvent(EventName.Target_Cleared, null);

                            if (Client.GameState == Enums.EGameState.Login)
                                ResetLoginGUI();
                        }
                        break;

                        #endregion

                    case EventName.Set_Login_State:
                        IO.LoginState = (Enums.ELoginState) args[0];
                        if(IO.LoginState == Enums.ELoginState.NewAccount)
                        {
                            var loginWindow = GuiManager.GetControl("LoginWindow");
                            if (loginWindow != null) loginWindow.IsVisible = false;
                            var createNewAccountCreationWindow = GuiManager.GetControl("CreateNewAccountWindow");
                            if (createNewAccountCreationWindow != null) createNewAccountCreationWindow.IsVisible = true;
                        }
                        break;
                    case EventName.Set_Login_Status_Label:
                        #region Set Login Status Label

                        GuiManager.CurrentSheet["LoginStatusLabel"].Text = (string) args[0];
                        GuiManager.CurrentSheet["LoginStatusLabel"].TextColor = Utils.GetColor((string) args[1]);
                        if (args.Length >= 3)
                        {
                            GuiManager.CurrentSheet["LoginStatusLabel"].IsVisible = (bool) args[2];
                        }
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
                                    TextBox tbx = optionsWindow["DoubleLeftClickNearbyTargetTextBox"] as TextBox;
                                    if (tbx != null)
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

                                    TextCue.AddClientInfoTextCue("Character Options Saved", TextCue.TextCueTag.None, Color.Lime, Color.Black, 1000, false, false, true);

                                }
                            }
                            else
                            {
                                if (Client.GameState == Enums.EGameState.HotButtonEditMode)
                                {
                                    #region Hot Button Edit Mode

                                    if (GuiManager.GetControl("HotButtonEditWindow") is HotButtonEditWindow hotButtonEditWindow)
                                    {
                                        // Change icon and text. --- Commented out 1/16/2019. No text makes the hot button not draw.
                                        //if (hotButtonEditWindow["HotButtonEditWindowTextBox"].Text.Length <= 0)
                                        //{
                                        //    TextCue.AddClientInfoTextCue("Invalid Text for HotButton", TextCue.TextCueTag.None, Color.Red, Color.Black, 1000, false, false, true);
                                        //    return;
                                        //}

                                        bool isHorizontal = hotButtonEditWindow.OriginatingWindow.ToLower().Contains("horizontal"); // horizontal or vertical hot buttons only right now

                                        if (!isHorizontal)
                                        {
                                            if (GuiManager.GenericSheet["VerticalHotButtonWindow"] is Window verticalHotButtonWindow)
                                            {
                                                verticalHotButtonWindow[hotButtonEditWindow.SelectedHotButton].VisualKey = hotButtonEditWindow.SelectedVisualKey;
                                                verticalHotButtonWindow[hotButtonEditWindow.SelectedHotButton].Text = hotButtonEditWindow["HotButtonEditWindowTextBox"].Text;

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

                                                hotButtonEditWindow.OnClose();

                                                TextCue.AddClientInfoTextCue("Hot Button Saved", TextCue.TextCueTag.None, Color.Lime, Color.Black, 1000, false, false, true);
                                            }
                                        }
                                        else // horizontal
                                        {
                                            if (GuiManager.GenericSheet["HorizontalHotButtonWindow"] is Window horizontallHotButtonWindow)
                                            {
                                                horizontallHotButtonWindow[hotButtonEditWindow.SelectedHotButton].VisualKey = hotButtonEditWindow.SelectedVisualKey;
                                                horizontallHotButtonWindow[hotButtonEditWindow.SelectedHotButton].Text = hotButtonEditWindow["HotButtonEditWindowTextBox"].Text;

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

                                                hotButtonEditWindow.OnClose();

                                                TextCue.AddClientInfoTextCue("Hot Button Saved", TextCue.TextCueTag.None, Color.Lime, Color.Black, 1000, false, false, true);
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
                        //Events.RegisterEvent(EventName.Client_Settings_Changed);
                        //Events.RegisterEvent(EventName.User_Settings_Changed);
                        Events.RegisterEvent(EventName.Character_Settings_Changed);
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
                    case EventName.Load_Character_Settings:
                        if (Character.Settings != null)
                        {
                            #region Options Window
                            Window optionsWindow = GuiManager.GenericSheet["OptionsWindow"] as Window;

                            if (optionsWindow != null)
                            {
                                TextBox tbx = optionsWindow["DoubleLeftClickNearbyTargetTextBox"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.DoubleLeftClickNearbyTarget;
                                tbx = optionsWindow["DoubleLeftClickDistantTargetTextBox"] as TextBox;
                                if (tbx != null)
                                    tbx.Text = Character.Settings.DoubleLeftClickDistantTarget;
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
                            }
                            #endregion

                            #region Vertical Hot Buttons
                            Window verticalHotButtonWindow = GuiManager.GenericSheet["VerticalHotButtonWindow"] as Window;

                            if (verticalHotButtonWindow != null)
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
                            }
                            #endregion

                            #region Horizontal Hot Buttons
                            Window horizontalHotButtonWindow = GuiManager.GenericSheet["HorizontalHotButtonWindow"] as Window;

                            if (horizontalHotButtonWindow != null)
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
                            if (na != null)
                                na.IsVisible = false;
                        }
                        else
                        {
                            if (at != null)
                                at.IsDisabled = false;
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
                                Control w = GuiManager.GetControl("OptionsWindow");
                                if (w != null && !w.HasFocus)
                                {
                                    sheet["ConfInputTextBox"].HasFocus = true;
                                    GuiManager.ActiveTextBox = "ConfInputTextBox";
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
                        gui.SpinelMode.UpdateGUI(gameTime, sheet);
                        break;
                    case Enums.EGameState.IOKGame:
                        gui.IOKMode.UpdateGUI(gameTime, sheet);
                        break;
                    case Enums.EGameState.LOKGame:
                        break;
                    case Enums.EGameState.HotButtonEditMode:
                        break;
                }
            }
        }

        public static void ResetLoginGUI()
        {
            foreach(Control c in GuiManager.GenericSheet.Controls)
            {
                if (c is Window)
                    (c as Window).OnClose();
            }

            try
            {
                Sheet sheet = GuiManager.Sheets["Login"];

                if (sheet != null)
                {

                    if (sheet["LoginWindow"] is gui.Window wi)
                    {
                        var sl = sheet["LoginStatusLabel"] as TextBox;
                        var at = wi["AccountTextBox"] as TextBox;
                        var pt = wi["PasswordTextBox"] as TextBox;
                        var sh = wi["ServerHostTextBox"] as TextBox;
                        var cna = wi["CreateNewAccountButton"] as Button;

                        if (at != null)
                        {
                            at.Clear();
                            at.IsCursorVisible = true;
                            at.HasFocus = true;
                        }
                        if (pt != null)
                        {
                            pt.Clear();
                            pt.IsCursorVisible = false;
                            pt.HasFocus = false;
                        }
                        if (sh != null)
                        {
                            sh.Clear();
                            sh.AddText(Client.ClientSettings.ServerHost);
                            sh.SelectAll();
                        }
                        if (sl != null)
                        {
                            sl.Clear();
                            sl.TextColor = Color.White;
                        }
                        if(cna != null)
                        {
                            cna.IsVisible = true;
                        }

                        if (sheet["CreateNewAccountWindow"] is gui.Window cnaw)
                            cnaw.IsVisible = false;

                        wi.IsVisible = true;
                    }
                }

                // Hide some windows.
                gui.Window macrosWindow = gui.GuiManager.GenericSheet["MacrosWindow"] as gui.Window;
                if (macrosWindow != null && macrosWindow.IsVisible)
                    macrosWindow.IsVisible = false;
                gui.Window optionsWindow = gui.GuiManager.GenericSheet["OptionsWindow"] as gui.Window;
                if (optionsWindow != null && optionsWindow.IsVisible)
                    optionsWindow.IsVisible = false;
                gui.Window helpWindow = gui.GuiManager.GenericSheet["HelpWindow"] as gui.Window;
                if (helpWindow != null && helpWindow.IsVisible)
                    helpWindow.IsVisible = false;
            }
            catch (Exception e)
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
                    case Enums.EGameState.CharacterGeneration:
                        break;
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
                        {
                            gui.SpinelMode.UpdateGUI(Program.Client.ClientGameTime, GuiManager.Sheets[newGameState.ToString()]);
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
                        TextCue.ClearCursorTextCue();
                        break;
                    case Enums.EGameState.Login:
                        if(GameHUD.PreviousGameState != Enums.EGameState.Login)
                            Events.ResetLoginGUI();
                        break;
                    case Enums.EGameState.LOKGame:
                        GuiManager.TextCues.Clear();
                        break;
                    case Enums.EGameState.HotButtonEditMode:
                        //GameHUD.PreviousGameState = currGameState; // remember what game state we were in when HUD took control
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
                        TextCue.ClearCursorTextCue();
                        break;
                    //case Enums.eGameState.Splash:
                    //    Events.ResetLoginGUI();
                        //break;
                }

                if(currGameState != Enums.EGameState.HotButtonEditMode)
                {
                    GameHUD.PreviousGameState = currGameState;
                }
            }
            catch (Exception e)
            {
                Utils.LogException(e);
            }
        }
    }
}
