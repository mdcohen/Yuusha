namespace Yuusha
{
    public class CharGen
    {
        public const int ACCOUNT_MIN_LENGTH = 5;
        public const int ACCOUNT_MAX_LENGTH = 12;
        public const int PASSWORD_MIN_LENGTH = 6;
        public const int PASSWORD_MAX_LENGTH = 20;

        public static Enums.ECharGenState CharGenState = Enums.ECharGenState.ChooseGender;
        public static Enums.ECharGenGUIMode CharGenGUIMode = Enums.ECharGenGUIMode.Text;

        public static string NewAccountName = "";
        public static string NewAccountPassword = "";
        public static string NewAccountEmail = "";

        public static bool AutoRollerEnabled = false;

        public static int DesiredStrength = 15;
        public static int DesiredDexterity = 15;
        public static int DesiredIntelligence = 15;
        public static int DesiredWisdom = 15;
        public static int DesiredConstitution = 15;
        public static int DesiredCharisma = 15;

        public static bool FirstCharacter = false;

        public static void ChooseGender()
        {
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).Clear();
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("Please select a gender:", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("1 - Male", Enums.ETextType.Default);
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("2 - Female", Enums.ETextType.Default);
        }

        public static void ChooseHomeland()
        {
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).Clear();
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
            (gui.GuiManager.CurrentSheet["CharGenScrollableTextBox"] as gui.ScrollableTextBox).AddLine("Please enter a name for your character:", Enums.ETextType.Default);
        }

        public static void ReviewStats(string inData)
        {
            if (!AutoRollerEnabled) return;
        }
    }
}