using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Yuusha
{
    public class CharGen
    {
        public const int ACCOUNT_MIN_LENGTH = 5;
        public const int ACCOUNT_MAX_LENGTH = 12;
        public const int PASSWORD_MIN_LENGTH = 6;
        public const int PASSWORD_MAX_LENGTH = 20;
        private const int MAX_STAT_ROLL = 18;
        private const int MIN_STAT_ROLL = 3;

        public static Enums.ECharGenState CharGenState = Enums.ECharGenState.ChooseHomeland;
        public static Enums.ECharGenGUIMode CharGenGUIMode = Enums.ECharGenGUIMode.Text;

        public static DateTime AutoRollerStartTime = new DateTime();

        public static string NewAccountName = "";
        public static string NewAccountPassword = "";
        public static string NewAccountEmail = "";

        public static bool AutoRollerEnabled = false;

        private static int RolledStrength = 15;
        private static int RolledDexterity = 15;
        private static int RolledIntelligence = 15;
        private static int RolledWisdom = 15;
        private static int RolledConstitution = 15;
        private static int RolledCharisma = 15;
        private static int RolledHits = 60;
        private static int RolledStamina = 10;
        private static int RolledMana = 10;

        private static int DesiredStrength = 15;
        private static int DesiredDexterity = 15;
        private static int DesiredIntelligence = 15;
        private static int DesiredWisdom = 15;
        private static int DesiredConstitution = 15;
        private static int DesiredCharisma = 15;
        private static int DesiredHits = 60;
        private static int DesiredStamina = 10;
        private static int DesiredMana = 10;

        private static int HighestHits = 0;
        private static int HighestStamina = 0;
        private static int HighestMana = 0;

        private static bool ManaUser = false;

        public static bool FirstCharacter = false;

        private static string CharacterAge = "very young";
        private static string SelectedGender = "Male";
        public static string SelectedHomeland = "Barbarian";
        public static string SelectedProfession = "Fighter";

        public static int RollNumber = 0;
        private static bool SetCharacterInfoText = false;

        public static readonly Dictionary<string, string> Homelands = Lore.GetAllHomelandLore();
        public static readonly Dictionary<string, string> Professions = Lore.GetAllProfessionsLore();

        public static readonly Dictionary<string, string> CommandsToSend = new Dictionary<string, string>()
        {
            {"Barbarian", "b" },
            {"Illyria", "i" },
            {"Mu", "m" },
            {"Lemuria", "l" },
            {"Leng", "lg" },
            {"Draznia", "d" },
            {"Hovath", "h" },
            {"Mnar", "mn" },
            {"Fighter", "fi" },
            {"Thaumaturge", "th" },
            {"Wizard", "wi" },
            {"Martial Artist", "ma" },
            {"Thief", "tf" },
            {"Sorcerer", "sr" }
        };

        public static void ResetCharGen()
        {
            NewAccountName = "";
            NewAccountPassword = "";
            NewAccountEmail = "";

            RolledStrength = 15;
            RolledDexterity = 15;
            RolledIntelligence = 15;
            RolledWisdom = 15;
            RolledConstitution = 15;
            RolledCharisma = 15;
            RolledHits = 60;
            RolledStamina = 10;
            RolledMana = 10;

            DesiredStrength = 15;
            DesiredDexterity = 15;
            DesiredIntelligence = 15;
            DesiredWisdom = 15;
            DesiredConstitution = 15;
            DesiredCharisma = 15;
            DesiredHits = 60;
            DesiredStamina = 10;
            DesiredMana = 10;

            HighestHits = 0;
            HighestStamina = 0;
            HighestMana = 0;

            FirstCharacter = false;

            CharacterAge = "very young";
            SelectedGender = "Male";
            SelectedHomeland = "Barbarian";
            SelectedProfession = "Fighter";

            RollNumber = 0;
            SetCharacterInfoText = false;

            Program.Client.GUIManager.LoadSheet(gui.GuiManager.GenericSheet.FilePath);
            Program.Client.GUIManager.LoadSheet(gui.GuiManager.CurrentSheet.FilePath);

            if (Client.IsFullScreen)
            {
               gui.GuiManager.GenericSheet.OnClientResize(Client.PrevClientBounds, Client.NowClientBounds);
               gui.GuiManager.CurrentSheet.OnClientResize(Client.PrevClientBounds, Client.NowClientBounds);
            }
        }

        public static void ChooseGender()
        {
            gui.GuiManager.CurrentSheet["CharGenToggleAutoRollerButton"].IsVisible = false;
            gui.GuiManager.CurrentSheet["AutoRollerWindow"].IsVisible = false;
            gui.GuiManager.CurrentSheet["RollerStatsWindow"].IsVisible = false;

            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).Clear();
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("Please select a gender:", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("1 - Male", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("2 - Female", Enums.ETextType.Default);
        }

        public static void ChooseHomeland()
        {
            RemoveCharGenSelectionButtons(new List<string>(Professions.Keys));

            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).Clear();
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("Please select a homeland:", Enums.ETextType.Default);
            
            Point point = new Point(64, 111);
            int zDepth = 30;
            List<string> homelandsList = new List<string>(Homelands.Keys);
            homelandsList.Sort();
            foreach(string homeland in homelandsList)
            {
                AddSelectionButton(homeland, point, zDepth);
                point.Y += 23;
                zDepth++;
            }

            Events.RegisterEvent(Events.EventName.CharGen_Lore, gui.GuiManager.CurrentSheet["BarbarianButton"]);
        }

        public static void ChooseProfession()
        {
            RemoveCharGenSelectionButtons(new List<string>(Homelands.Keys));

            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).Clear();
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("Please select a profession:", Enums.ETextType.Default);

            Point point = new Point(64, 111);
            int zDepth = 30;
            List<string> professionsList = new List<string>(Professions.Keys);
            professionsList.Sort();
            foreach (string profession in professionsList)
            {
                AddSelectionButton(profession, point, zDepth);
                point.Y += 23;
                zDepth++;
            }

            Events.RegisterEvent(Events.EventName.CharGen_Lore, gui.GuiManager.CurrentSheet["FighterButton"]);
        }

        public static void AddSelectionButton(string selection, Point point, int zDepth)
        {
            if (gui.GuiManager.GetControl(selection + "Button") != null)
                return;

            gui.Button button = new gui.Button(selection + "Button", "CharacterGenerationWindow",
                new Rectangle(point.X, point.Y, BitmapFont.ActiveFonts[gui.GuiManager.CurrentSheet.Font].MeasureString(selection), 23),
                selection, true, Color.White, true, false, gui.GuiManager.CurrentSheet.Font, new gui.VisualKey("WhiteSpace"),
                Color.Black, 0, 255, 255, new gui.VisualKey("WhiteSpace"), new gui.VisualKey(""), new gui.VisualKey(""),
                Events.EventName.CharGen_Lore.ToString(), BitmapFont.TextAlignment.Left, 0, 0, Color.Lime, true, Color.DarkGray, true,
                new List<Enums.EAnchorType>() { Enums.EAnchorType.Left, Enums.EAnchorType.Top }, false, Map.Direction.Northwest, 5, "", "");

            button.ZDepth = zDepth;

            gui.GuiManager.CurrentSheet.AddControl(button);
        }

        public static void ChooseName()
        {
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).Clear();
            string description = CharacterAge.ToLower() + " " + SelectedGender.ToLower() + " " + SelectedProfession.ToLower() + " from " + SelectedHomeland;
            (gui.GuiManager.CurrentSheet["CharGenInfoScrollableTextBox"] as gui.ScrollableTextBox).AddLine("You are a " + description + ".", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("Please enter a name for your character:", Enums.ETextType.Default);
        }

        public static void ReviewStats(string inData)
        {
            // Remove all buttons in window.
            RemoveCharGenSelectionButtons(new List<string>(Professions.Keys));

            // Increase roll counter.
            RollNumber++;

            // Update roll counter label.
            (gui.GuiManager.CurrentSheet["RollsCounterLabel"] as gui.Label).Text = "Roll Count: " + RollNumber;

            // Determine mana info visibility for casters/non-casters.
            if(!inData.Contains("Mana:"))
            {
                (gui.GuiManager.CurrentSheet["ManaLabel"] as gui.Label).IsVisible = false;
                (gui.GuiManager.CurrentSheet["ManaRollLabel"] as gui.Label).IsVisible = false;
                (gui.GuiManager.CurrentSheet["HighestManaLabel"] as gui.Label).IsVisible = false;
                ManaUser = false;
            }
            else
            {
                (gui.GuiManager.CurrentSheet["ManaLabel"] as gui.Label).IsVisible = true;
                (gui.GuiManager.CurrentSheet["ManaRollLabel"] as gui.Label).IsVisible = true;
                (gui.GuiManager.CurrentSheet["HighestManaLabel"] as gui.Label).IsVisible = true;
                ManaUser = true;
            }

            // Enable auto roller toggle button.
            gui.GuiManager.CurrentSheet["CharGenToggleAutoRollerButton"].IsVisible = true;
            gui.GuiManager.CurrentSheet["AutoRollerWindow"].IsVisible = true;
            gui.GuiManager.CurrentSheet["RollerStatsWindow"].IsVisible = true;            

            // Why?
            if (!CharGen.SetCharacterInfoText)
            {
                (gui.GuiManager.CurrentSheet["CharGenInfoScrollableTextBox"] as gui.ScrollableTextBox).Clear();
                (gui.GuiManager.CurrentSheet["CharGenInfoScrollableTextBox"] as gui.ScrollableTextBox).IsVisible = false;
                CharGen.SetCharacterInfoText = true;
            }
            
            // display stat rolls received
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).Clear();
            string description = CharacterAge.ToLower() + " " + SelectedGender.ToLower() + " " + SelectedProfession.ToLower() + " from " + SelectedHomeland;
            (gui.GuiManager.CurrentSheet["CharGenInfoScrollableTextBox"] as gui.ScrollableTextBox).AddLine("You are a " + description + ".", Enums.ETextType.Default);
            // add story here
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("", Enums.ETextType.Default);
            string[] lines = inData.Split("\n".ToCharArray());
            for (int a = 1; a < lines.Length; a++)
            {
                if (lines[a].ToLower().Contains("roll again") && AutoRollerEnabled)
                    (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("Auto rolling...", Enums.ETextType.Default);
                else
                {
                    if (!lines[a].Contains("["))
                        (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine(lines[a].TrimStart(), Enums.ETextType.Default);
                }
            }

            // parse stat rolls
            foreach (string line in lines)
            {
                if (line.Contains("Strength:"))
                {
                    string newLine = line.Remove(0, line.IndexOf(":") + 1).TrimStart();
                    int.TryParse(newLine.Substring(0, 2).TrimStart(), out RolledStrength);
                    (gui.GuiManager.CurrentSheet["StrengthRollLabel"] as gui.Label).Text = RolledStrength.ToString().PadLeft(2);
                }
                if (line.Contains("Dexterity:"))
                {
                    string newLine = line.Remove(0, line.IndexOf(":") + 1).TrimStart();
                    int.TryParse(newLine.Substring(0, 2).TrimStart(), out RolledDexterity);
                    (gui.GuiManager.CurrentSheet["DexterityRollLabel"] as gui.Label).Text = RolledDexterity.ToString().PadLeft(2);
                }
                if (line.Contains("Intelligence:"))
                {
                    string newLine = line.Remove(0, line.IndexOf(":") + 1).TrimStart();
                    int.TryParse(newLine.Substring(0, 2).TrimStart(), out RolledIntelligence);
                    (gui.GuiManager.CurrentSheet["IntelligenceRollLabel"] as gui.Label).Text = RolledIntelligence.ToString().PadLeft(2);
                }
                if (line.Contains("Wisdom:"))
                {
                    string newLine = line.Remove(0, line.IndexOf(":") + 1).TrimStart();
                    int.TryParse(newLine.Substring(0, 2).TrimStart(), out RolledWisdom);
                    (gui.GuiManager.CurrentSheet["WisdomRollLabel"] as gui.Label).Text = RolledWisdom.ToString().PadLeft(2);
                }
                if (line.Contains("Constitution:"))
                {
                    string newLine = line.Remove(0, line.IndexOf(":") + 1).TrimStart();
                    int.TryParse(newLine.Substring(0, 2).TrimStart(), out RolledConstitution);
                    (gui.GuiManager.CurrentSheet["ConstitutionRollLabel"] as gui.Label).Text = RolledConstitution.ToString().PadLeft(2);
                }
                if (line.Contains("Charisma:"))
                {
                    string newLine = line.Remove(0, line.IndexOf(":") + 1).TrimStart();
                    int.TryParse(newLine.Substring(0, 2).TrimStart(), out RolledCharisma);
                    (gui.GuiManager.CurrentSheet["CharismaRollLabel"] as gui.Label).Text = RolledCharisma.ToString().PadLeft(2);
                }
                if (line.Contains("Hits:"))
                {
                    string newLine = line.Remove(0, line.IndexOf("Hits:") + 5).TrimStart();
                    int.TryParse(newLine.Substring(0, 2).TrimStart(), out RolledHits);
                    (gui.GuiManager.CurrentSheet["HitsRollLabel"] as gui.Label).Text = RolledHits.ToString().PadLeft(2);

                    if (RolledHits > HighestHits)
                        HighestHits = RolledHits;

                    (gui.GuiManager.CurrentSheet["HighestHitsLabel"] as gui.Label).Text = "Highest Hits: " + HighestHits;
                }
                if (line.Contains("Stamina:"))
                {
                    string newLine = line.Remove(0, line.IndexOf("Stamina:") + 8).TrimStart();
                    int.TryParse(newLine.Substring(0, 2).TrimStart(), out RolledStamina);
                    (gui.GuiManager.CurrentSheet["StaminaRollLabel"] as gui.Label).Text = RolledStamina.ToString().PadLeft(2);

                    if (RolledStamina > HighestStamina)
                        HighestStamina = RolledStamina;

                    (gui.GuiManager.CurrentSheet["HighestStaminaLabel"] as gui.Label).Text = "Highest Stamina: " + HighestStamina;
                }
                if (line.Contains("Mana:"))
                {
                    string newLine = line.Remove(0, line.IndexOf("Mana:") + 5).TrimStart();
                    int.TryParse(newLine.Substring(0, 2).TrimStart(), out RolledMana);
                    (gui.GuiManager.CurrentSheet["ManaRollLabel"] as gui.Label).Text = RolledMana.ToString().PadLeft(2);

                    if (RolledMana > HighestMana)
                        HighestMana = RolledMana;

                    (gui.GuiManager.CurrentSheet["HighestManaLabel"] as gui.Label).Text = "Highest Mana: " + HighestMana;
                }
            }

            // check auto roller
            if (!AutoRollerEnabled)
            {
                (gui.GuiManager.CurrentSheet["AutoRollerTimeLabel"] as gui.Label).Text = "Auto Roller Time: N/A";
                return;
            }
            else
            {
                if (!DesiredStatsAchieved())
                {
                    TimeSpan elapsedTime = DateTime.Now - AutoRollerStartTime;
                    (gui.GuiManager.CurrentSheet["AutoRollerTimeLabel"] as gui.Label).Text = "Auto Roller Time: " + elapsedTime.Minutes + "m " + elapsedTime.Seconds + "s";
                    IO.Send("y");
                }
                else
                {
                    Events.RegisterEvent(Events.EventName.Toggle_AutoRoller);

                    (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("", Enums.ETextType.Default);
                    (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("Auto Roller Success!", Enums.ETextType.Default);
                    (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("", Enums.ETextType.Default);
                    (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("Roll again?  (y,n)", Enums.ETextType.Default);
                    Audio.AudioManager.PlaySoundEffect("KSND0220");
                }
            }
        }

        public static bool DesiredStatsAchieved()
        {
            // strength check
            DesiredStrength = Convert.ToInt32((gui.GuiManager.CurrentSheet["StrengthDesiredTextBox"] as gui.NumericTextBox).Text.Trim());
            if (RolledStrength < DesiredStrength)
                return false;
            // dexterity check
            DesiredDexterity = Convert.ToInt32((gui.GuiManager.CurrentSheet["DexterityDesiredTextBox"] as gui.NumericTextBox).Text.Trim());
            if (RolledDexterity < DesiredDexterity)
                return false;
            // intelligence check
            DesiredIntelligence = Convert.ToInt32((gui.GuiManager.CurrentSheet["IntelligenceDesiredTextBox"] as gui.NumericTextBox).Text.Trim());
            if (RolledIntelligence < DesiredIntelligence)
                return false;
            // wisdom check
            DesiredWisdom = Convert.ToInt32((gui.GuiManager.CurrentSheet["WisdomDesiredTextBox"] as gui.NumericTextBox).Text.Trim());
            if (RolledWisdom < DesiredWisdom)
                return false;
            // constitution check
            DesiredConstitution = Convert.ToInt32((gui.GuiManager.CurrentSheet["ConstitutionDesiredTextBox"] as gui.NumericTextBox).Text.Trim());
            if (RolledConstitution < DesiredConstitution)
                return false;
            // charisma check
            DesiredCharisma = Convert.ToInt32((gui.GuiManager.CurrentSheet["CharismaDesiredTextBox"] as gui.NumericTextBox).Text.Trim());
            if (RolledCharisma < DesiredCharisma)
                return false;
            // hits check
            DesiredHits = Convert.ToInt32((gui.GuiManager.CurrentSheet["HitsDesiredTextBox"] as gui.NumericTextBox).Text.Trim());
            if (RolledHits < DesiredHits)
                return false;
            // stamina check
            DesiredStamina = Convert.ToInt32((gui.GuiManager.CurrentSheet["StaminaDesiredTextBox"] as gui.NumericTextBox).Text.Trim());
            if (RolledStamina < DesiredStamina)
                return false;

            if (ManaUser)
            {
                DesiredMana = Convert.ToInt32((gui.GuiManager.CurrentSheet["ManaDesiredTextBox"] as gui.NumericTextBox).Text.Trim());
                if (RolledMana < DesiredMana)
                    return false;
            }

            return true;
        }

        public static void OnSend(string text)
        {
            switch(CharGen.CharGenState)
            {
                case Enums.ECharGenState.ChooseGender:
                    if (text == "1")
                        SelectedGender = "Male";
                    else if (text == "2")
                        SelectedGender = "Female";
                    break;
                case Enums.ECharGenState.ChooseHomeland:
                    switch(text.ToLower())
                    {
                        case "i":
                            SelectedHomeland = "Illyria";
                            break;
                        case "m":
                            SelectedHomeland = "Mu";
                            break;
                        case "l":
                            SelectedHomeland = "Lemuria";
                            break;
                        case "lg":
                            SelectedHomeland = "Leng";
                            break;
                        case "d":
                            SelectedHomeland = "Draznia";
                            break;
                        case "h":
                            SelectedHomeland = "Hovath";
                            break;
                        case "mn":
                            SelectedHomeland = "Mnar";
                            break;
                        case "b":
                            SelectedHomeland = "the barbarian plains";
                            break;
                    }
                    break;
                case Enums.ECharGenState.ChooseProfession:
                    switch (text.ToLower())
                    {
                        case "fi":
                            SelectedProfession = "Fighter";
                            break;
                        case "th":
                            SelectedProfession = "Thaumaturge";
                            break;
                        case "wi":
                            SelectedProfession = "Wizard";
                            break;
                        case "ma":
                            SelectedProfession = "Martial Artist";
                            break;
                        case "tf":
                            SelectedProfession = "Thief";
                            break;
                        case "sr":
                            SelectedProfession = "Sorcerer";
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        public static void PopulateInfoTextBox(string info, Dictionary<string, string> dict)
        {
            (gui.GuiManager.CurrentSheet["CharGenInfoScrollableTextBox"] as gui.ScrollableTextBox).Clear();
            int prevAlpha = (gui.GuiManager.CurrentSheet["CharGenInfoScrollableTextBox"] as gui.ScrollableTextBox).TextAlpha;
            (gui.GuiManager.CurrentSheet["CharGenInfoScrollableTextBox"] as gui.ScrollableTextBox).TextAlpha = 0;
            (gui.GuiManager.CurrentSheet["CharGenInfoScrollableTextBox"] as gui.ScrollableTextBox).AddLine(info, Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenInfoScrollableTextBox"] as gui.ScrollableTextBox).AddLine("", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenInfoScrollableTextBox"] as gui.ScrollableTextBox).AddLine(" " + dict[info], Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenInfoScrollableTextBox"] as gui.ScrollableTextBox).ScrollToTop();
            (gui.GuiManager.CurrentSheet["CharGenInfoScrollableTextBox"] as gui.ScrollableTextBox).TextAlpha = prevAlpha;
            //(gui.GuiManager.CurrentSheet["CharGenSelectionButton"] as gui.Button).Text = "Select: " + info;
            //(gui.GuiManager.CurrentSheet["CharGenSelectionButton"] as gui.Button).Command = CharGen.CommandsToSend[info];
            (gui.GuiManager.CurrentSheet["CharGenInputTextBox"] as gui.TextBox).Text = CharGen.CommandsToSend[info];
            (gui.GuiManager.CurrentSheet["CharGenInputTextBox"] as gui.TextBox).SelectAll();
            (gui.GuiManager.CurrentSheet["CharGenInputTextBox"] as gui.TextBox).HasFocus = true;
        }

        public static void RemoveCharGenSelectionButtons(List<string> keysList)
        {
            foreach (gui.Control c in new List<gui.Control>((gui.GuiManager.CurrentSheet["CharacterGenerationWindow"] as gui.Window).Controls))
            {
                foreach (string key in keysList)
                {
                    if (c.Name == key + "Button")
                    {
                        (gui.GuiManager.CurrentSheet["CharacterGenerationWindow"] as gui.Window).Controls.Remove(c);
                    }
                }
            }
        }
    }
}