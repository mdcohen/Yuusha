using System;

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

        public static Enums.ECharGenState CharGenState = Enums.ECharGenState.ChooseGender;
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
        private static string CharacterGender = "Male";
        private static string CharacterHomeland = "the plains";
        private static string CharacterProfession = "Fighter";

        private static int RollNumber = 0;

        public static void ChooseGender()
        {
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
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).Clear();
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("Please select a homeland:", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("I  - Illyria", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("M  - Mu", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("L  - Lemuria", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("LG - Leng", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("D  - Draznia", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("H  - Hovath", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("MN - Mnar", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("B  - Barbarian", Enums.ETextType.Default);
        }

        public static void ChooseProfession()
        {
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).Clear();
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("Please select a character class:", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("FI - Fighter", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("TH - Thaumaturge", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("WI - Wizard", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("MA - Martial Artist", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("TF - Thief (neutral)", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("SR - Sorcerer (evil)", Enums.ETextType.Default);
            //(gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("DR - Druid (not available)", Enums.ETextType.Default);
            //(gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("RA - Ranger (not available)", Enums.ETextType.Default);
            //(gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("BE - Berserker (not available)", Enums.ETextType.Default);
        }

        public static void ChooseName()
        {
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).Clear();
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("Please enter a name for your character:", Enums.ETextType.Default);
        }

        public static void ReviewStats(string inData)
        {
            RollNumber++;

            (gui.GuiManager.CurrentSheet["RollsCounterLabel"] as gui.Label).Text = "Roll Count: " + RollNumber;

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

            // display lines
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).Clear();
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("", Enums.ETextType.Default);
            string description = CharacterAge.ToLower() + " " + CharacterGender.ToLower() + " " + CharacterProfession.ToLower() + " from " + CharacterHomeland;
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("You are a " + description + ".", Enums.ETextType.Default);
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
                    Sound.Play("0220");
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
                        CharacterGender = "Male";
                    else if (text == "2")
                        CharacterGender = "Female";
                    break;
                case Enums.ECharGenState.ChooseHomeland:
                    switch(text.ToLower())
                    {
                        case "i":
                            CharacterHomeland = "Illyria";
                            break;
                        case "m":
                            CharacterHomeland = "Mu";
                            break;
                        case "l":
                            CharacterHomeland = "Lemuria";
                            break;
                        case "lg":
                            CharacterHomeland = "Leng";
                            break;
                        case "d":
                            CharacterHomeland = "Draznia";
                            break;
                        case "h":
                            CharacterHomeland = "Hovath";
                            break;
                        case "mn":
                            CharacterHomeland = "Mnar";
                            break;
                        case "b":
                            CharacterHomeland = "the barbarian plains";
                            break;
                    }
                    break;
                case Enums.ECharGenState.ChooseProfession:
                    switch (text.ToLower())
                    {
                        case "fi":
                            CharacterProfession = "Fighter";
                            break;
                        case "th":
                            CharacterProfession = "Thaumaturge";
                            break;
                        case "wi":
                            CharacterProfession = "Wizard";
                            break;
                        case "ma":
                            CharacterProfession = "Martial Artist";
                            break;
                        case "tf":
                            CharacterProfession = "Thief";
                            break;
                        case "sr":
                            CharacterProfession = "Sorcerer";
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}