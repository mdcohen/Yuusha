using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;

namespace Yuusha.gui
{
    public class GameHUD : GameComponent
    {
        
        public static Dictionary<string, string> GameIconsDictionary = new Dictionary<string, string>()
        {
            // skills
            {"bow", "yuushaicon_0" },
            {"dagger", "yuushaicon_1" },
            {"flail", "yuushaicon_2" },
            {"polearm", "yuushaicon_3" },
            {"mace", "yuushaicon_4" },
            {"rapier", "yuushaicon_5" },
            {"shuriken", "yuushaicon_6" },
            {"staff", "yuushaicon_7" },
            {"sword", "yuushaicon_8" },
            {"threestaff", "yuushaicon_9" },
            {"two handed", "yuushaicon_10" },
            {"unarmed", "yuushaicon_11" },
            {"magic", "yuushaicon_12" },
            {"thievery", "yuushaicon_13" },
            {"bash", "yuushaicon_14" },
            {"poisoned", "yuushaicon_15" },
            {"throwing", "yuushaicon_16" },
            {"stunned", "yuushaicon_17" },
            {"resting", "yuushaicon_18" },
            {"meditating", "yuushaicon_19" },
            {"thirdeye", "yuushaicon_20" },
            {"swap", "yuushaicon_21" },
            {"upgrade", "yuushaicon_22" },
            {"downgrade", "yuushaicon_23" },
            {"talent_assassinate", "yuushaicon_24" },
            {"talent_backstab", "yuushaicon_25" },
            {"talent_battlecharge", "yuushaicon_26" },
            {"talent_blindfighting", "yuushaicon_27" },
            {"talent_cleave", "yuushaicon_28" },
            {"talent_daggerstorm", "yuushaicon_29" },
            {"talent_dualwield", "yuushaicon_30" },
            {"talent_flyingfury", "yuushaicon_31" },
            {"talent_gage", "yuushaicon_32" },
            {"talent_legsweep", "yuushaicon_33" },
            {"talent_memorize", "yuushaicon_34" },
            {"talent_snoop", "yuushaicon_35" },
            {"talent_picklocks", "yuushaicon_36" },
            {"talent_rapidkicks", "yuushaicon_37" },
            {"talent_riposte", "yuushaicon_38" },
            {"talent_roundhousekick", "yuushaicon_39" },
            {"talent_shieldbash", "yuushaicon_40" },
            {"talent_steal", "yuushaicon_41" },
            {"talent_doubleattack", "yuushaicon_42" },
        };
        /// <summary>
        /// When a Character enters the game for the first time a Request_Spells event is triggered.
        /// From then on talents are updated when a change to the Character's SpellDictionary server-side occurs.
        /// </summary>
        public static List<int> InitialSpellbookUpdated = new List<int>();
        /// <summary>
        /// When a Character enters the game for the first time a Request_Talents event is triggered.
        /// From then on talents are updated when a change to the Character's TalentsDictionary server-side occurs.
        /// </summary>
        public static List<int> InitialTalentbookUpdated = new List<int>();
        /// <summary>
        /// Used when discerning pathing direction choices double clicking on MapTileLabels/SpinelTileLabels.
        /// </summary>
        public static Cell MovementClickedCell;
        /// <summary>
        /// Windows that may be dragged from any spot the mouse cursor touches down.
        /// </summary>
        public static List<string> NonDiscreetlyDraggableWindows = new List<string>()
        {
            "CharacterStatsWindow",
            "EffectsWindow",
            //"FogOfWarWindow",
            //"HorizontalHotButtonWindow",
            //"InventoryWindow",
            "RingsWindow",
            "ScrollableTextBoxWindow",
            "SpellbookWindow",
            "SpellringWindow",
            "SpellWarmingWindow",
            //"VerticalHotButtonWindow",
            "VitalsWindow",
            "VolumeControlPopUpWindow",
        };

        private static Window SlideScreenWindow;
        private static Window SlideScreenGenericWindow;

        private static Window SplitSlideScreenWindow1;
        //private static Window SplitSlideScreenGenericWindow1;
        private static Window SplitSlideScreenWindow2;
        //private static Window SplitSlideScreenGenericWindow2;

        private static bool SplitSlideScreenVertically;
        private static Map.Direction SlideDirection = Map.Direction.None;
        private static int SlideSpeed = 15;
        public static bool ReturnSlidScreen = false;
        public static bool ReturnSplitSlidScreen = false;

        const int MAP_GRID_MINIMUM = 30;
        const int MAP_GRID_MAXIMUM = 100;

        public static List<Tuple<ScrollableTextBox, DateTime>> ConversationBubbles = new List<Tuple<ScrollableTextBox, DateTime>>();

        /// <summary>
        /// Control to be shaken. Original point on screen. DateTime of start. TimeSpan how long to shake. int = How much to shake.
        /// </summary>
        private static List<Tuple<Control, Point, DateTime, TimeSpan, int>> RandomlyShakingControls = new List<Tuple<Control, Point, DateTime, TimeSpan, int>>();

        /// <summary>
        /// Control to be shaken. Original point on screen. DateTime of start. TimeSpan how long to shake. SyncronizedShakingAmount static int is modified in method call.
        /// </summary>
        private static List<Tuple<Control, Point, DateTime, TimeSpan>> SynchronizedShakingControls = new List<Tuple<Control, Point, DateTime, TimeSpan>>();
        private static int SynchronizedShakingAmount = 3;

        /// <summary>
        /// Control (Label) that will be created to black out the screen. DateTime of creation, TimeSpan to remain, int = fade speed, bool = fade in upon expiration
        /// </summary>
        public static Tuple<Control, DateTime, TimeSpan, int, bool> FadeToBlackTuple;

        private static List<Tuple<Control, Point, Map.Direction, int>> CrumblingControls = new List<Tuple<Control, Point, Map.Direction, int>>();
        
        private static List<Tuple<Control, Point, Map.Direction, int>> ReturningCrumbledControls = new List<Tuple<Control, Point, Map.Direction, int>>();

        private static List<Tuple<Control, DateTime, TimeSpan>> HiddenControls = new List<Tuple<Control, DateTime, TimeSpan>>();

        public static List<Cell> MovementChoices = new List<Cell>();

        public static bool ChangingMapDisplaySize = false; // used in Events.UpdateGUI to prevent issues while changing map display size

        public static List<Control> AchievementLabelList = new List<Control>(); // level up label also goes here to prevent achievements from showing until it's done

        public static Character CurrentTarget;
        public static string TextSendOverride = "";
        public static bool VitalsTextMode = false;

        public static Spell DraggedSpell { get; set; }
        public static Enums.EGameState PreviousGameState { get; set; }
        public static Cell ExaminedCell { get; set; }

        public static List<Cell> Cells = new List<Cell>();
        public static Dictionary<int, Character> CharactersInView = new Dictionary<int, Character>();

        private static int FramesQuantity = 0;
        private static float CurrentAvgFPS = 0;

        public static GridBoxWindow.GridBoxPurpose GridBoxWindowRequestUpdate
        { get; set; } = GridBoxWindow.GridBoxPurpose.None;

        public static bool NextGridBoxUpdateIsSilent
        { get; set; } = true;

        // Text is not cleared from scrolling textboxes in these ClientStates (they fill the screen)
        public static List<Enums.EGameState> OverrideDisplayStates = new List<Enums.EGameState>
        {
            Enums.EGameState.CharacterGeneration, Enums.EGameState.HotButtonEditMode
        };

        public GameHUD(Game game) : base(game)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Fade to black. Or fading back in from black.
            if (FadeToBlackTuple != null)
            {
                // Time is up. Remove control or fade "in"
                if (DateTime.Now - FadeToBlackTuple.Item2 >= FadeToBlackTuple.Item3)
                {
                    if (!FadeToBlackTuple.Item5) // not fading in
                    {
                        GuiManager.Dispose(FadeToBlackTuple.Item1);
                        FadeToBlackTuple = null;
                    }
                    else // fading in
                    {
                        FadeToBlackTuple.Item1.VisualAlpha -= FadeToBlackTuple.Item4;

                        if (FadeToBlackTuple.Item1.VisualAlpha <= 0)
                        {
                            GuiManager.Dispose(FadeToBlackTuple.Item1);
                            FadeToBlackTuple = null;
                        }
                    }
                }
                else
                {
                    FadeToBlackTuple.Item1.VisualAlpha += FadeToBlackTuple.Item4;
                    if (FadeToBlackTuple.Item1.VisualAlpha > 255) FadeToBlackTuple.Item1.VisualAlpha = 255;
                }
            }

            // Temporarily hidden controls.
            //foreach (Tuple<Control, DateTime, TimeSpan> tuple in new List<Tuple<Control, DateTime, TimeSpan>>(HiddenControls))
            //{
            //    if (DateTime.Now - tuple.Item2 >= tuple.Item3)
            //    {
            //        tuple.Item1.IsVisible = true;
            //    }
            //}

            if (!ReturnSlidScreen)
            {
                if (SlideScreenWindow != null)
                {
                    Point pt = Map.DirectionCoordinates[SlideDirection];
                    Rectangle client = new Rectangle(0, 0, Client.Width, Client.Height);

                    if (new Rectangle(SlideScreenWindow.Position.X, SlideScreenWindow.Position.Y, SlideScreenWindow.Width, SlideScreenWindow.Height).Intersects(client))
                        SlideScreenWindow.ForcePosition(new Point(SlideScreenWindow.Position.X + (SlideSpeed * pt.X), SlideScreenWindow.Position.Y + (SlideSpeed * pt.Y)), false);
                }

                if (SlideScreenGenericWindow != null)
                {
                    Point pt = Map.DirectionCoordinates[SlideDirection];
                    Rectangle client = new Rectangle(0, 0, Client.Width, Client.Height);

                    if (new Rectangle(SlideScreenGenericWindow.Position.X, SlideScreenGenericWindow.Position.Y, SlideScreenGenericWindow.Width, SlideScreenGenericWindow.Height).Intersects(client))
                        SlideScreenGenericWindow.ForcePosition(new Point(SlideScreenGenericWindow.Position.X + (SlideSpeed * pt.X), SlideScreenGenericWindow.Position.Y + (SlideSpeed * pt.Y)), false);
                }
            }
            else
            {
                if (SlideScreenWindow != null && SlideScreenWindow.Position != new Point(0, 0))
                {
                    Point pt = Map.DirectionCoordinates[Map.GetOppositeDirection(SlideDirection)];
                    SlideScreenWindow.ForcePosition(new Point(SlideScreenWindow.Position.X + (SlideSpeed * pt.X), SlideScreenWindow.Position.Y + (SlideSpeed * pt.Y)), false);

                    if (SlideScreenWindow.Position == new Point(0, 0))
                        SlideScreenWindow = null;
                }

                if (SlideScreenGenericWindow != null && SlideScreenGenericWindow.Position != new Point(0, 0))
                {
                    Point pt = Map.DirectionCoordinates[Map.GetOppositeDirection(SlideDirection)];
                    SlideScreenGenericWindow.ForcePosition(new Point(SlideScreenGenericWindow.Position.X + (SlideSpeed * pt.X), SlideScreenGenericWindow.Position.Y + (SlideSpeed * pt.Y)), false);

                    if (SlideScreenGenericWindow.Position == new Point(0, 0))
                        SlideScreenGenericWindow = null;
                }

                if (SlideScreenWindow == null && SlideScreenGenericWindow == null)
                    ReturnSlidScreen = false;
            }

            if(!ReturnSplitSlidScreen)
            {
                if (SplitSlideScreenWindow1 != null)
                {
                    Point pt = SplitSlideScreenVertically ? Map.DirectionCoordinates[Map.Direction.North] : Map.DirectionCoordinates[Map.Direction.West];
                    Rectangle client = new Rectangle(0, 0, Client.Width, Client.Height);

                    if (new Rectangle(SplitSlideScreenWindow1.Position.X, SplitSlideScreenWindow1.Position.Y, SplitSlideScreenWindow1.Width, SplitSlideScreenWindow1.Height).Intersects(client))
                        SplitSlideScreenWindow1.ForcePosition(new Point(SplitSlideScreenWindow1.Position.X + (SlideSpeed * pt.X), SplitSlideScreenWindow1.Position.Y + (SlideSpeed * pt.Y)), false);
                }

                if (SplitSlideScreenWindow2 != null)
                {
                    Point pt = SplitSlideScreenVertically ? Map.DirectionCoordinates[Map.Direction.South] : Map.DirectionCoordinates[Map.Direction.East];
                    Rectangle client = new Rectangle(0, 0, Client.Width, Client.Height);

                    if (new Rectangle(SplitSlideScreenWindow2.Position.X, SplitSlideScreenWindow2.Position.Y, SplitSlideScreenWindow2.Width, SplitSlideScreenWindow2.Height).Intersects(client))
                        SplitSlideScreenWindow2.ForcePosition(new Point(SplitSlideScreenWindow2.Position.X + (SlideSpeed * pt.X), SplitSlideScreenWindow2.Position.Y + (SlideSpeed * pt.Y)), false);
                }
            }
            else
            {
                if (SplitSlideScreenWindow1 != null && SplitSlideScreenWindow1.Position != new Point(0, 0))
                {
                    Point pt = SplitSlideScreenVertically ? Map.DirectionCoordinates[Map.Direction.South] : Map.DirectionCoordinates[Map.Direction.East];

                    SplitSlideScreenWindow1.ForcePosition(new Point(SplitSlideScreenWindow1.Position.X + (SlideSpeed * pt.X), SplitSlideScreenWindow1.Position.Y + (SlideSpeed * pt.Y)), false);

                    if (SplitSlideScreenWindow1.Position == new Point(0, 0))
                        SplitSlideScreenWindow1 = null;
                }

                if (SplitSlideScreenWindow2 != null && SplitSlideScreenWindow2.Position != new Point(0, 0))
                {
                        Point pt = SplitSlideScreenVertically ? Map.DirectionCoordinates[Map.Direction.North] : Map.DirectionCoordinates[Map.Direction.West];

                        SplitSlideScreenWindow2.ForcePosition(new Point(SplitSlideScreenWindow2.Position.X + (SlideSpeed * pt.X), SplitSlideScreenWindow2.Position.Y + (SlideSpeed * pt.Y)), false);

                    if (SplitSlideScreenWindow2.Position == new Point(0, 0))
                        SplitSlideScreenWindow2 = null;
                }

                if (SplitSlideScreenWindow1 == null && SplitSlideScreenWindow2 == null)
                    ReturnSplitSlidScreen = false;
            }

            // Crumbling controls.
            foreach (Tuple<Control, Point, Map.Direction, int> tuple in new List <Tuple<Control, Point, Map.Direction, int>>(CrumblingControls))
            {
                Point pt = Map.DirectionCoordinates[tuple.Item3];
                Rectangle client = new Rectangle(0, 0, Client.Width, Client.Height);
                //int crumbleAmount = new Random(Guid.NewGuid().GetHashCode()).Next(tuple.Item4 / 2, tuple.Item4);

                if (tuple.Item1 is Window w && new Rectangle(w.Position.X, w.Position.Y, w.Width, w.Height).Intersects(client) && new Random(Guid.NewGuid().GetHashCode()).Next(1 , 100) > 50)
                {
                    w.ForcePosition(new Point(tuple.Item1.Position.X + (new Random(Guid.NewGuid().GetHashCode()).Next(1, tuple.Item4) * pt.X), tuple.Item1.Position.Y + (new Random(Guid.NewGuid().GetHashCode()).Next(1, tuple.Item4) * pt.Y)), false);
                }
                else if (!(tuple.Item1 is Window) && new Rectangle(tuple.Item1.Position.X, tuple.Item1.Position.Y, tuple.Item1.Width, tuple.Item1.Height).Intersects(client) && new Random(Guid.NewGuid().GetHashCode()).Next(1, 100) > 50)
                {
                    tuple.Item1.Position = new Point(tuple.Item1.Position.X + (new Random(Guid.NewGuid().GetHashCode()).Next(1, tuple.Item4) * pt.X), tuple.Item1.Position.Y + (new Random(Guid.NewGuid().GetHashCode()).Next(1, tuple.Item4) * pt.Y));
                }
            }

            // Returning crumbled controls.
            foreach (Tuple<Control, Point, Map.Direction, int> tuple in new List<Tuple<Control, Point, Map.Direction, int>>(ReturningCrumbledControls))
            {
                Point pt = Map.DirectionCoordinates[tuple.Item3];
                Rectangle client = new Rectangle(0, 0, Client.Width, Client.Height);

                if (tuple.Item1.Position != tuple.Item2)
                {
                    if (tuple.Item1 is Window w && new Random(Guid.NewGuid().GetHashCode()).Next(1, 100) > 50)
                        w.ForcePosition(new Point(tuple.Item1.Position.X + (tuple.Item4 * pt.X), tuple.Item1.Position.Y + (tuple.Item4 * pt.Y)), false);
                    else if(!(tuple.Item1 is Window) && new Random(Guid.NewGuid().GetHashCode()).Next(1, 100) > 50)
                        tuple.Item1.Position = new Point(tuple.Item1.Position.X + (tuple.Item4 * pt.X), tuple.Item1.Position.Y + (tuple.Item4 * pt.Y));
                }
                else
                {
                    ReturningCrumbledControls.Remove(tuple);
                }
            }

            // Conversation bubbles.
            foreach (Tuple<ScrollableTextBox, DateTime> tuple in new List<Tuple<ScrollableTextBox, DateTime>>(ConversationBubbles))
            {
                tuple.Item1.ZDepth = 0;

                if (DateTime.Now - tuple.Item2 >= TimeSpan.FromSeconds(2))
                {
                    tuple.Item1.VisualAlpha -= Client.ClientSettings.ConversationBubbleFadeOutSpeed;
                    tuple.Item1.TextAlpha -= Client.ClientSettings.ConversationBubbleFadeOutSpeed;
                }
                
                if(DateTime.Now - tuple.Item2 >= TimeSpan.FromSeconds(Utility.Settings.StaticSettings.RoundDelayLength * 1.3) || tuple.Item1.VisualAlpha <= 0)
                {
                    GuiManager.RemoveControl(tuple.Item1);
                    ConversationBubbles.Remove(tuple);
                }
            }

            // Randomly shaking controls.
            foreach (Tuple<Control, Point, DateTime, TimeSpan, int> tuple in new List<Tuple<Control, Point, DateTime, TimeSpan, int>>(RandomlyShakingControls))
            {
                if (DateTime.Now - tuple.Item3 >= tuple.Item4)
                {
                    if (tuple.Item1 is Window w)
                        w.ForcePosition(tuple.Item2, true);
                    else
                        tuple.Item1.Position = tuple.Item2;

                    RandomlyShakingControls.Remove(tuple);
                }
                else
                {
                    if (tuple.Item1 is Window w)
                        w.ForcePosition(new Point(tuple.Item1.Position.X + new Random(Guid.NewGuid().GetHashCode()).Next(-tuple.Item5, tuple.Item5), tuple.Item1.Position.Y + new Random(Guid.NewGuid().GetHashCode()).Next(-tuple.Item5, tuple.Item5)), false);
                    else tuple.Item1.Position = new Point(tuple.Item1.Position.X + new Random(Guid.NewGuid().GetHashCode()).Next(-tuple.Item5, tuple.Item5), tuple.Item1.Position.Y + new Random(Guid.NewGuid().GetHashCode()).Next(-tuple.Item5, tuple.Item5));
                }
            }

            // Synchronized shaking controls.
            if (SynchronizedShakingControls.Count > 0)
            {
                int shakeX = new Random(Guid.NewGuid().GetHashCode()).Next(-SynchronizedShakingAmount, SynchronizedShakingAmount);
                int shakeY = new Random(Guid.NewGuid().GetHashCode()).Next(-SynchronizedShakingAmount, SynchronizedShakingAmount);

                foreach (Tuple<Control, Point, DateTime, TimeSpan> tuple in new List<Tuple<Control, Point, DateTime, TimeSpan>>(SynchronizedShakingControls))
                {
                    if (DateTime.Now - tuple.Item3 >= tuple.Item4)
                    {
                        if (tuple.Item1 is Window w)
                            w.ForcePosition(tuple.Item2, true);
                        else
                            tuple.Item1.Position = tuple.Item2;

                        SynchronizedShakingControls.Remove(tuple);
                    }
                    else
                    {
                        if (tuple.Item1 is Window w)
                            w.ForcePosition(new Point(tuple.Item1.Position.X + shakeX, tuple.Item1.Position.Y + shakeY), false);
                        else tuple.Item1.Position = new Point(tuple.Item1.Position.X + shakeX, tuple.Item1.Position.Y + shakeY);
                    }
                }
            }
        }

        public static void AddRandomlyShakingControl(Tuple<Control, Point, DateTime, TimeSpan, int> tuple)
        {
            foreach(Tuple<Control, Point, DateTime, TimeSpan, int> t in RandomlyShakingControls)
            {
                if (t.Item1.Name == tuple.Item1.Name && t.Item1.Sheet == tuple.Item1.Sheet)
                    return;
            }

            RandomlyShakingControls.Add(tuple);
        }
        public static void RandomlyShakeScreen(int milliseconds, int amount, bool includeGenericSheet)
        {
            ReleaseAllDraggedControls();

            foreach (Control c in new List<Control>(GuiManager.CurrentSheet.Controls))
            {
                AddRandomlyShakingControl(Tuple.Create(c, c.Position, DateTime.Now, TimeSpan.FromMilliseconds(milliseconds), amount));
            }

            if (includeGenericSheet)
            {
                foreach (Control c in new List<Control>(GuiManager.GenericSheet.Controls))
                {
                    AddRandomlyShakingControl(Tuple.Create(c, c.Position, DateTime.Now, TimeSpan.FromMilliseconds(milliseconds), amount));
                }
            }
        }

        public static void AddSynchronizedShakingControl(Tuple<Control, Point, DateTime, TimeSpan> tuple)
        {
            foreach (Tuple<Control, Point, DateTime, TimeSpan> t in SynchronizedShakingControls)
            {
                if (t.Item1.Name == tuple.Item1.Name && t.Item1.Sheet == tuple.Item1.Sheet)
                    return;
            }

            SynchronizedShakingControls.Add(tuple);
        }
        public static void SynchronouslyShakeScreen(int milliseconds, int shakeAmount, bool includeGenericSheet)
        {
            ReleaseAllDraggedControls();
            SynchronizedShakingAmount = shakeAmount;

            foreach (Control c in GuiManager.CurrentSheet.Controls)
            {
                AddSynchronizedShakingControl(Tuple.Create(c, c.Position, DateTime.Now, TimeSpan.FromMilliseconds(milliseconds)));
            }

            if (includeGenericSheet)
            {
                foreach (Control c in GuiManager.GenericSheet.Controls)
                {
                    AddSynchronizedShakingControl(Tuple.Create(c, c.Position, DateTime.Now, TimeSpan.FromMilliseconds(milliseconds)));
                }
            }
        }

        public static void SplitSlideScreen(bool vertically, int slideSpeed, bool includeGenericSheet)
        {
            ReleaseAllDraggedControls();
            SlideSpeed = slideSpeed;
            SplitSlideScreenVertically = vertically;

            SplitSlideScreenWindow1 = new Window("SplitSlideScreenWindow1", "", new Rectangle(0, 0, Client.Width, Client.Height), true, true, false, GuiManager.CurrentSheet.Font,
                new VisualKey("WhiteSpace"), Color.Black, 0, false, Map.Direction.None, 0, new List<Enums.EAnchorType>(), "");
            SplitSlideScreenWindow2 = new Window("SplitSlideScreenWindow2", "", new Rectangle(0, 0, Client.Width, Client.Height), true, true, false, GuiManager.CurrentSheet.Font,
                new VisualKey("WhiteSpace"), Color.Black, 0, false, Map.Direction.None, 0, new List<Enums.EAnchorType>(), "");

            foreach (Control c in GuiManager.CurrentSheet.Controls)
            {
                if (c is Background)
                    continue;

                if(!vertically) // going horizontal
                {
                    if(c.Position.X <= Client.Width / 2)
                        GuiManager.CurrentSheet.AttachControlToWindow(c, SplitSlideScreenWindow1);
                    else GuiManager.CurrentSheet.AttachControlToWindow(c, SplitSlideScreenWindow2);
                }
                else
                {
                    if (c.Position.Y <= Client.Height / 2)
                        GuiManager.CurrentSheet.AttachControlToWindow(c, SplitSlideScreenWindow1);
                    else GuiManager.CurrentSheet.AttachControlToWindow(c, SplitSlideScreenWindow2);
                }
            }

            if (includeGenericSheet)
            {
                foreach (Control c in GuiManager.GenericSheet.Controls)
                {
                    if (!vertically)
                    {
                        if (c.Position.X <= Client.Width / 2)
                            GuiManager.GenericSheet.AttachControlToWindow(c, SplitSlideScreenWindow1);
                        else GuiManager.GenericSheet.AttachControlToWindow(c, SplitSlideScreenWindow2);
                    }
                    else
                    {
                        if (c.Position.Y <= Client.Height / 2)
                            GuiManager.GenericSheet.AttachControlToWindow(c, SplitSlideScreenWindow1);
                        else GuiManager.GenericSheet.AttachControlToWindow(c, SplitSlideScreenWindow2);
                    }
                }
            }
        }

        public static void SynchronouslySlideScreen(Map.Direction direction, int slideSpeed, bool includeBackground, bool includeGenericSheet)
        {
            ReleaseAllDraggedControls();
            SlideDirection = direction;
            SlideSpeed = slideSpeed;

            SlideScreenWindow = new Window("SlideScreenWindow", "", new Rectangle(0, 0, Client.Width, Client.Height), true, true, false, GuiManager.CurrentSheet.Font,
                new VisualKey("WhiteSpace"), Color.Black, 0, false, Map.Direction.None, 0, new List<Enums.EAnchorType>(), "");

            foreach (Control c in GuiManager.CurrentSheet.Controls)
            {
                if (c is Background && !includeBackground)
                    continue;

                GuiManager.CurrentSheet.AttachControlToWindow(c, SlideScreenWindow);
            }

            if (includeGenericSheet)
            {
                SlideScreenGenericWindow = new Window("SlideScreenWindow", "", new Rectangle(0, 0, Client.Width, Client.Height), true, true, false, GuiManager.CurrentSheet.Font,
                new VisualKey("WhiteSpace"), Color.Black, 0, false, Map.Direction.None, 0, new List<Enums.EAnchorType>(), "");

                foreach (Control c in GuiManager.GenericSheet.Controls)
                {
                    GuiManager.GenericSheet.AttachControlToWindow(c, SlideScreenGenericWindow);
                }
            }
        }

        public static void AddCrumblingControl(Tuple<Control, Point, Map.Direction, int> tuple)
        {
            foreach (Tuple<Control, Point, Map.Direction, int> t in CrumblingControls)
            {
                if (t.Item1.Name == tuple.Item1.Name && t.Item1.Sheet == tuple.Item1.Sheet)
                    return;
            }

            CrumblingControls.Add(tuple);
        }
        public static void CrumbleScreen(Map.Direction direction, int crumbleSpeed, bool includeBackground, bool includeGenericSheet)
        {
            ReleaseAllDraggedControls();

            foreach (Control c in GuiManager.CurrentSheet.Controls)
            {
                if (c is Background && !includeBackground)
                    continue;

                AddCrumblingControl(Tuple.Create(c, c.Position, direction, crumbleSpeed));
            }

            if (includeGenericSheet)
            {
                foreach (Control c in GuiManager.GenericSheet.Controls)
                {
                    AddCrumblingControl(Tuple.Create(c, c.Position, direction, crumbleSpeed));
                }
            }
        }
        public static void ReturnCrumbledScreen(bool immediately)
        {
            if (immediately)
            {
                foreach (Tuple<Control, Point, Map.Direction, int> tuple in CrumblingControls)
                {
                    if (tuple.Item1 is Window w)
                        w.ForcePosition(tuple.Item2, true);
                    else
                        tuple.Item1.Position = tuple.Item2;

                    CrumblingControls.Remove(tuple);
                }
            }
            else
            {
                foreach (Tuple<Control, Point, Map.Direction, int> tuple in new List<Tuple<Control, Point, Map.Direction, int>>(CrumblingControls))
                {
                    ReturningCrumbledControls.Add(Tuple.Create(tuple.Item1, tuple.Item2, Map.GetOppositeDirection(CrumblingControls[0].Item3), tuple.Item4));
                    CrumblingControls.Remove(tuple);
                }
            }
        }

        public static void FadeToColor(Color fadeColor, int milliseconds, int fadeAmount, bool fadeIn)
        {
            FadeToVisualKey("WhiteSpace", fadeColor, milliseconds, fadeAmount, fadeIn, false);
        }
        /// <summary>
        /// The screen goes black immediately.
        /// </summary>
        /// <param name="milliseconds">Time from start when the screen fades in or is no longer black.</param>
        /// <param name="fadeAmount"></param>
        /// <param name="fadeIn"></param>
        public static void GoBlack(int milliseconds, int fadeAmount, bool fadeIn)
        {
            FadeToVisualKey("WhiteSpace", Color.Black, milliseconds, fadeAmount, fadeIn, true);
        }

        public static void LightningFlash()
        {
            FadeToVisualKey("WhiteSpace", Color.Azure, 100, 6, true, true);
        }

        public static void FadeToVisualKey(string visualKey, Color tintColor, int milliseconds, int fadeAmount, bool fadeIn, bool immediate)
        {
            if(GuiManager.GetControl("GameHUDFadeToVisualKeyLabel") is Label l)
                GuiManager.Dispose(l);

            Control fadeToBlackLabel = new Label("GameHUDFadeToVisualKeyLabel", "", new Rectangle(0, 0, Client.Width, Client.Height), "", Color.White, true, false,
                GuiManager.GenericSheet.Font, new VisualKey(visualKey), tintColor, immediate ? (byte)255 : (byte)0, 0, BitmapFont.TextAlignment.Center, 0, 0, "", "",
                new List<Enums.EAnchorType>(), "");
            GuiManager.GenericSheet.AddControl(fadeToBlackLabel);
            fadeToBlackLabel.ZDepth = 0;

            FadeToBlackTuple = Tuple.Create(fadeToBlackLabel, DateTime.Now, TimeSpan.FromMilliseconds(milliseconds), fadeAmount, fadeIn);
        }

        public static float UpdateCumulativeMovingAverageFPS(float newFPS)
        {
            ++FramesQuantity;
            CurrentAvgFPS += (newFPS - CurrentAvgFPS) / FramesQuantity;

            return CurrentAvgFPS;
        }

        private static void ReleaseAllDraggedControls()
        {
            GuiManager.StopDragging();
            if (GuiManager.Cursors[GuiManager.GenericSheet.Cursor].DraggedControl is DragAndDropButton dadButton)
                dadButton.StopDragging();
        }

        public static void DisplayLogoutOptionScreen()
        {
            if (GuiManager.GenericSheet["LogoutOptionWindow"] is Window w)
                return;

            Window logoutOptionWindow = new Window("LogoutOptionWindow", "", new Rectangle(0, 0, Client.Width, Client.Height), true, true, false, Client.ClientSettings.DefaultFont,
                new VisualKey("WhiteSpace"), Color.Black, 190, false, Map.Direction.Northwest, 5, new List<Enums.EAnchorType>() { Enums.EAnchorType.All }, "");

            VisualInfo vi = GuiManager.Visuals["GoldDragonLogo"];

            Label backgroundDragonLabel = new Label("LogoutDragonLabel", logoutOptionWindow.Name, new Rectangle(Client.Width / 2 - vi.Width / 2, Client.Height / 2 - vi.Height / 2, vi.Width, vi.Height),
                "", Color.GhostWhite, true, false, Client.ClientSettings.DefaultFont, new VisualKey("GoldDragonLogo"), Color.White, 255, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>(), "")
            {
                MouseInvisible = true
            };

            string questionToDisplay = "Press L to logout, X to exit";

            if (Client.InGame)
                questionToDisplay = "Press Q to quit, L to logout, X to exit";

            // change question depending on game state

            Label logoutQuestionLabel = new Label("LogoutQuestionLabel", logoutOptionWindow.Name, new Rectangle(0, Client.Height / 2 - BitmapFont.ActiveFonts[Client.ClientSettings.DefaultFont].LineHeight, Client.Width, BitmapFont.ActiveFonts[Client.ClientSettings.DefaultFont].LineHeight),
                questionToDisplay, Color.GhostWhite, true, false, Client.ClientSettings.DefaultFont, new VisualKey("WhiteSpace"), Color.Black, 90, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>(), "");

            Label logoutQuestion2Label = new Label("LogoutQuestion2Label", logoutOptionWindow.Name, new Rectangle(0, logoutQuestionLabel.Position.Y + logoutQuestionLabel.Height, Client.Width, BitmapFont.ActiveFonts[Client.ClientSettings.DefaultFont].LineHeight),
                "R to return", Color.GhostWhite, true, false, Client.ClientSettings.DefaultFont, new VisualKey("WhiteSpace"), Color.Black, 90, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>(), "");

            GuiManager.GenericSheet.AddControl(logoutOptionWindow);
            GuiManager.GenericSheet.AddControl(backgroundDragonLabel);
            GuiManager.GenericSheet.AddControl(logoutQuestionLabel);
            GuiManager.GenericSheet.AddControl(logoutQuestion2Label);

            backgroundDragonLabel.ZDepth = 100;
        }

        /// <summary>
        /// Handles dropping from a drag and drop area to a drag and drop area.
        /// </summary>
        /// <param name="b">The drag and drop area where an item is being manipulated FROM.</param>
        public static void DragAndDropLogic(DragAndDropButton b)
        {
            // Right hand or left hand items
            if (b.Name.StartsWith("RH") || b.Name.StartsWith("LH"))
            {
                #region Right and Left Hand Items
                string rightOrLeft = b.Name.StartsWith("RH") ? "right" : "left";
                if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Sack"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command, "put " + rightOrLeft + " in sack");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Sack);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Belt"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command, "belt " + rightOrLeft);
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Belt);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Pouch"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command, "put " + rightOrLeft + " in pouch");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Pouch);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Locker"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command, "put " + rightOrLeft + " in locker");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Locker);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH") || GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                {
                    // when returning to original position prevent a swap command
                    if(GuiManager.MouseOverDropAcceptingControl.Name != b.Name)
                        Events.RegisterEvent(Events.EventName.Send_Command, "swap");
                }
                else if (AcceptingGridBoxIsLocation(GuiManager.MouseOverDropAcceptingControl.Name))
                {
                    GridBoxWindow window = GuiManager.MouseOverDropAcceptingControl as GridBoxWindow;

                    if (window.WindowTitle != null)
                    {
                        switch (window.WindowTitle.Text.ToLower())
                        {
                            case "altar":
                                Events.RegisterEvent(Events.EventName.Send_Command, "put " + rightOrLeft + " on altar");
                                break;
                            case "counter":
                                Events.RegisterEvent(Events.EventName.Send_Command, "put " + rightOrLeft + " on counter");
                                break;
                            default:
                                Events.RegisterEvent(Events.EventName.Send_Command, "drop " + rightOrLeft);
                                break;
                        }

                        GridBoxWindow.RequestUpdateFromServer((GridBoxWindow.GridBoxPurpose)Enum.Parse(typeof(GridBoxWindow.GridBoxPurpose), window.WindowTitle.Text, true));
                    }
                }
                else if(GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Inventory"))
                {
                    //TODO: wearOrientation
                    Events.RegisterEvent(Events.EventName.Send_Command, "wear " + rightOrLeft);
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Inventory);
                }
                else if(GuiManager.MouseOverDropAcceptingControl.Name.Contains("Ring"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command, GetRingPutCommand(GuiManager.MouseOverDropAcceptingControl.Name, rightOrLeft));
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Rings);
                }
                #endregion
            }
            else if (b.Name.StartsWith("Belt"))
            {
                if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH") || GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command, "wield " + b.RepresentedItem.Name);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Locker"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command, "wield " + b.RepresentedItem.Name + ";put " + b.RepresentedItem.Name + " in locker");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Locker);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Sack"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command,"wield " + b.RepresentedItem.Name + ";put " + b.RepresentedItem.Name + " in sack");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Sack);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Pouch"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command,"wield " + b.RepresentedItem.Name + ";put " + b.RepresentedItem.Name + " in pouch");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Pouch);
                }
                else if (AcceptingGridBoxIsLocation(GuiManager.MouseOverDropAcceptingControl.Name))
                {
                    GridBoxWindow window = GuiManager.MouseOverDropAcceptingControl as GridBoxWindow;
                    if (window.WindowTitle != null)
                    {
                        switch (window.WindowTitle.Text.ToLower())
                        {
                            case "altar":
                                Events.RegisterEvent(Events.EventName.Send_Command,"wield " + b.RepresentedItem.Name + ";put " + b.RepresentedItem.Name + " on altar");
                                break;
                            case "counter":
                                Events.RegisterEvent(Events.EventName.Send_Command,"wield " + b.RepresentedItem.Name + ";put " + b.RepresentedItem.Name + " on counter");
                                break;
                            default:
                                Events.RegisterEvent(Events.EventName.Send_Command,"wield " + b.RepresentedItem.Name + ";drop " + b.RepresentedItem.Name);
                                break;
                        }

                        GridBoxWindow.RequestUpdateFromServer((GridBoxWindow.GridBoxPurpose)Enum.Parse(typeof(GridBoxWindow.GridBoxPurpose), window.WindowTitle.Text, true));
                    }
                }

                GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Belt);
            }
            else if (b.Name.StartsWith("Sack"))
            {
                if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH") || GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                {
                    string swapafter = "";
                    string swapbefore = "";

                    if (Character.CurrentCharacter.RightHand == null && GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                        swapafter = ";swap";
                    else if (Character.CurrentCharacter.LeftHand == null && GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH"))
                        swapbefore = "swap;";

                    Events.RegisterEvent(Events.EventName.Send_Command,swapbefore + "take " + b.GetNItemName(b) + " from sack" + swapafter);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Locker"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + " from sack; put " + b.RepresentedItem.Name + " in locker");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Locker);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Belt"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + " from sack;belt " + b.RepresentedItem.Name);
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Belt);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Pouch"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + " from sack;put " + b.RepresentedItem.Name + " in pouch");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Pouch);
                }
                else if (AcceptingGridBoxIsLocation(GuiManager.MouseOverDropAcceptingControl.Name))
                {
                    GridBoxWindow window = GuiManager.MouseOverDropAcceptingControl as GridBoxWindow;
                    if (window.WindowTitle != null)
                    {
                        switch (window.WindowTitle.Text.ToLower())
                        {
                            case "altar":
                                Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + " from sack;put " + b.RepresentedItem.Name + " on altar");
                                break;
                            case "counter":
                                Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + " from sack;put " + b.RepresentedItem.Name + " on counter");
                                break;
                            default:
                                Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + " from sack;drop " + b.RepresentedItem.Name);
                                break;
                        }

                        GridBoxWindow.RequestUpdateFromServer((GridBoxWindow.GridBoxPurpose)Enum.Parse(typeof(GridBoxWindow.GridBoxPurpose), window.WindowTitle.Text, true));
                    }
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.Contains("Ring"))
                {
                    string rightOrLeft = Character.CurrentCharacter.RightHand == null ? "right" : "left";

                    Events.RegisterEvent(Events.EventName.Send_Command, "take " + b.GetNItemName(b) + " from sack;" + GetRingPutCommand(GuiManager.MouseOverDropAcceptingControl.Name, rightOrLeft));
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Rings);
                }

                GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Sack);
            }
            else if (b.Name.StartsWith("Pouch"))
            {
                if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH") || GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                {
                    string swapafter = "";
                    string swapbefore = "";

                    if (Character.CurrentCharacter.RightHand == null && GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                        swapafter = ";swap";
                    else if (Character.CurrentCharacter.LeftHand == null && GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH"))
                        swapbefore = "swap;";

                    Events.RegisterEvent(Events.EventName.Send_Command,swapbefore + "take " + b.GetNItemName(b) + " from pouch" + swapafter);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Locker"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + " from pouch put " + b.RepresentedItem.Name + " in locker");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Locker);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Belt"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + " from pouch;belt " + b.RepresentedItem.Name);
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Belt);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Sack"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + " from pouch;put " + b.RepresentedItem.Name + " in sack");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Sack);
                }
                else if (AcceptingGridBoxIsLocation(GuiManager.MouseOverDropAcceptingControl.Name))
                {
                    GridBoxWindow window = GuiManager.MouseOverDropAcceptingControl as GridBoxWindow;
                    if (window.WindowTitle != null)
                    {
                        switch (window.WindowTitle.Text.ToLower())
                        {
                            case "altar":
                                Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + " from pouch;put " + b.RepresentedItem.Name + " on altar");
                                break;
                            case "counter":
                                Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + " from pouch;put " + b.RepresentedItem.Name + " on counter");
                                break;
                            default:
                                Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + " from pouch;drop " + b.RepresentedItem.Name);
                                break;
                        }

                        GridBoxWindow.RequestUpdateFromServer((GridBoxWindow.GridBoxPurpose)Enum.Parse(typeof(GridBoxWindow.GridBoxPurpose), window.WindowTitle.Text, true));
                    }
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.Contains("Ring"))
                {
                    string rightOrLeft = Character.CurrentCharacter.RightHand == null ? "right" : "left";

                    Events.RegisterEvent(Events.EventName.Send_Command, "take " + b.GetNItemName(b) + " from pouch;" + GetRingPutCommand(GuiManager.MouseOverDropAcceptingControl.Name, rightOrLeft));
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Rings);
                }

                GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Pouch);
            }
            else if(b.Name.StartsWith("Locker"))
            {
                if (Character.CurrentCharacter.Cell != null && Character.CurrentCharacter.Cell.IsLockers)
                {
                    if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH") || GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                    {
                        string swapafter = "";
                        string swapbefore = "";

                        if (Character.CurrentCharacter.RightHand == null && GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                            swapafter = ";swap";
                        else if (Character.CurrentCharacter.LeftHand == null && GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH"))
                            swapbefore = "swap;";

                        Events.RegisterEvent(Events.EventName.Send_Command, swapbefore + "take " + b.GetNItemName(b) + " from locker" + swapafter);
                    }
                    else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Pouch"))
                    {
                        Events.RegisterEvent(Events.EventName.Send_Command, "take " + b.GetNItemName(b) + " from locker;put " + b.RepresentedItem.Name + " in pouch");
                        GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Locker);
                    }
                    else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Belt"))
                    {
                        Events.RegisterEvent(Events.EventName.Send_Command, "take " + b.GetNItemName(b) + " from locker;belt " + b.RepresentedItem.Name);
                        GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Belt);
                    }
                    else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Sack"))
                    {
                        Events.RegisterEvent(Events.EventName.Send_Command, "take " + b.GetNItemName(b) + " from locker;put " + b.RepresentedItem.Name + " in sack");
                        GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Sack);
                    }
                    else if (AcceptingGridBoxIsLocation(GuiManager.MouseOverDropAcceptingControl.Name))
                    {
                        GridBoxWindow window = GuiManager.MouseOverDropAcceptingControl as GridBoxWindow;
                        if (window.WindowTitle != null)
                        {
                            switch (window.WindowTitle.Text.ToLower())
                            {
                                case "altar":
                                    Events.RegisterEvent(Events.EventName.Send_Command, "take " + b.GetNItemName(b) + " from locker;put " + b.RepresentedItem.Name + " on altar");
                                    break;
                                case "counter":
                                    Events.RegisterEvent(Events.EventName.Send_Command, "take " + b.GetNItemName(b) + " from locker;put " + b.RepresentedItem.Name + " on counter");
                                    break;
                                default:
                                    Events.RegisterEvent(Events.EventName.Send_Command, "take " + b.GetNItemName(b) + " from locker;drop " + b.RepresentedItem.Name);
                                    break;
                            }

                            GridBoxWindow.RequestUpdateFromServer((GridBoxWindow.GridBoxPurpose)Enum.Parse(typeof(GridBoxWindow.GridBoxPurpose), window.WindowTitle.Text, true));
                        }
                    }
                    else if (GuiManager.MouseOverDropAcceptingControl.Name.Contains("Ring"))
                    {
                        string rightOrLeft = Character.CurrentCharacter.RightHand == null ? "right" : "left";

                        Events.RegisterEvent(Events.EventName.Send_Command, "take " + b.GetNItemName(b) + " from locker;" + GetRingPutCommand(GuiManager.MouseOverDropAcceptingControl.Name, rightOrLeft));
                        GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Rings);
                    }

                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Locker);
                }
            }
            else if (AcceptingGridBoxIsLocation(b.Name))
            {
                string fromLocation = "";
                if (b.Name.StartsWith("Altar"))
                    fromLocation = " from altar";
                else if (b.Name.StartsWith("Counter"))
                    fromLocation = " from counter";

                if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH") || GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                {
                    string swapafter = "";
                    string swapbefore = "";

                    if (Character.CurrentCharacter.RightHand == null && GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                        swapafter = ";swap";
                    else if (Character.CurrentCharacter.LeftHand == null && GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH"))
                        swapbefore = "swap;";

                    Events.RegisterEvent(Events.EventName.Send_Command,swapbefore + "take " + b.GetNItemName(b) + fromLocation + swapafter);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Locker"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + fromLocation + "; put " + b.RepresentedItem.Name + " in locker");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Locker);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Belt"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + fromLocation + ";belt " + b.RepresentedItem.Name);
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Belt);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Sack"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command,"take " + b.GetNItemName(b) + fromLocation + ";put " + b.RepresentedItem.Name + " in sack");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Sack);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Pouch"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command, "take " + b.GetNItemName(b) + fromLocation + ";put " + b.RepresentedItem.Name + " in pouch");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Pouch);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Ground") || GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Altar") ||
                    GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Counter"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command, "take " + b.GetNItemName(b) + fromLocation + ";drop " + b.RepresentedItem.Name);
                    GridBoxWindow.RequestUpdateFromServer((GridBoxWindow.GridBoxPurpose)Enum.Parse(typeof(GridBoxWindow.GridBoxPurpose), GuiManager.MouseOverDropAcceptingControl.Name.Replace("GridBoxWindow",""), true));
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.Contains("Ring"))
                {
                    string rightOrLeft = Character.CurrentCharacter.RightHand == null ? "right" : "left";

                    Events.RegisterEvent(Events.EventName.Send_Command, "take " + b.GetNItemName(b) + ";" + GetRingPutCommand(GuiManager.MouseOverDropAcceptingControl.Name, rightOrLeft));
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Rings);
                }

                GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Ground);
            }
            else if(b.Name.Contains("Inventory"))
            {
                Character.WearOrientation wearOrientation = b.RepresentedItem.WearOrientation;
                if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH") || GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                {
                    string swapafter = "";
                    string swapbefore = "";

                    if (Character.CurrentCharacter.RightHand == null && GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                        swapafter = ";swap";
                    else if (Character.CurrentCharacter.LeftHand == null && GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH"))
                        swapbefore = "swap;";

                    Events.RegisterEvent(Events.EventName.Send_Command, swapbefore + "remove" + (wearOrientation != Character.WearOrientation.None ? wearOrientation.ToString() + " " : " ") + b.RepresentedItem.Name + swapafter);
                }
            }
            else if(b.Name.StartsWith("LeftRing") || b.Name.StartsWith("RightRing"))
            {
                if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("RH") || GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("LH"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command, GetRingRemoveCommand(b.Name, out string goingToRightOrLeft));
                }
                else if(GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Sack"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command, GetRingRemoveCommand(b.Name, out string goingToRightOrLeft) + "; put " + goingToRightOrLeft + " in sack");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Sack);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Pouch"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command, GetRingRemoveCommand(b.Name, out string goingToRightOrLeft) + "; put " + goingToRightOrLeft + " in pouch");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Pouch);
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.StartsWith("Locker"))
                {
                    Events.RegisterEvent(Events.EventName.Send_Command, GetRingRemoveCommand(b.Name, out string goingToRightOrLeft) + "; put " + goingToRightOrLeft + " in locker");
                    GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Locker);
                }
                else if (AcceptingGridBoxIsLocation(GuiManager.MouseOverDropAcceptingControl.Name))
                {
                    GridBoxWindow window = GuiManager.MouseOverDropAcceptingControl as GridBoxWindow;
                    if (window.WindowTitle != null)
                    {
                        string goingToRightOrLeft = "";
                        switch (window.WindowTitle.Text.ToLower())
                        {
                            case "altar":
                                Events.RegisterEvent(Events.EventName.Send_Command, GetRingRemoveCommand(b.Name, out goingToRightOrLeft) + "; put " + goingToRightOrLeft + " on altar");
                                break;
                            case "counter":
                                Events.RegisterEvent(Events.EventName.Send_Command, GetRingRemoveCommand(b.Name, out goingToRightOrLeft) + "; put " + goingToRightOrLeft + " on counter");
                                break;
                            default:
                                Events.RegisterEvent(Events.EventName.Send_Command, GetRingRemoveCommand(b.Name, out goingToRightOrLeft) + "; drop " + goingToRightOrLeft);
                                break;
                        }

                        GridBoxWindow.RequestUpdateFromServer((GridBoxWindow.GridBoxPurpose)Enum.Parse(typeof(GridBoxWindow.GridBoxPurpose), window.WindowTitle.Text, true));
                    }
                }
                else if (GuiManager.MouseOverDropAcceptingControl.Name.Contains("Ring"))
                {
                    string rightOrLeft = Character.CurrentCharacter.RightHand == null ? "right" : "left";

                    Events.RegisterEvent(Events.EventName.Send_Command, GetRingRemoveCommand(b.Name, out string goingToRightOrLeft) + ";" + GetRingPutCommand(GuiManager.MouseOverDropAcceptingControl.Name, goingToRightOrLeft.ToLower() == "right" ? "left" : "right"));
                }

                GridBoxWindow.RequestUpdateFromServer(GridBoxWindow.GridBoxPurpose.Rings);
            }
        }

        private static string GetRingRemoveCommand(string buttonName, out string goingToRightOrLeft)
        {
            string whichRing = buttonName.Replace("DragAndDropButton", "");

            if (whichRing.StartsWith("Left"))
                goingToRightOrLeft = "right";
            else goingToRightOrLeft = "left";

            string number = whichRing.Substring(whichRing.Length - 1, 1);

            return "remove " + number + " ring from " + whichRing.Replace("Ring" + number, "").ToLower();
        }

        private static string GetRingPutCommand(string controlName, string rightOrLeft)
        {
            string whichRing = controlName.Replace("DragAndDropButton", "");
            string number = whichRing.Substring(whichRing.Length - 1, 1);

            return "put " + rightOrLeft + " on " + number + " " + whichRing.Replace("Ring" + number, "").ToLower();
        }

        public static void UpdateStatDetailsWindow()
        {
            if (GuiManager.GetControl("StatDetailsWindow") is Window w)
            {
                bool wasVisible = w.IsVisible;

                w.IsVisible = false;

                w.Controls.RemoveAll(c => c is Label);

                List<Tuple<string, string>> StatsLeftColumn = new List<Tuple<string, string>>()
                {
                    Tuple.Create("Race", "Race"), // Age                    
                    Tuple.Create("Gender", "Gender"), // Deity
                    Tuple.Create("Prof", "Profession"), // Specialty
                    Tuple.Create("Exp", "Experience"),
                    Tuple.Create("BLANK", "BLANK"),
                    Tuple.Create("Health","Hits"),
                    Tuple.Create("Health Adj", "HitsAdjustment"),
                    Tuple.Create("Health Full", "HitsFull"),
                    Tuple.Create("BLANK", "BLANK"),
                    Tuple.Create("Stamina", "Stamina"),
                    Tuple.Create("Stamina Adj", "StaminaAdjustment"),
                    Tuple.Create("BLANK", "BLANK"),
                    Tuple.Create("Mana", "Mana"),
                    Tuple.Create("Mana Adj", "ManaAdjustment"),
                    Tuple.Create("BLANK", "BLANK"),
                    Tuple.Create("Strength", "Strength"),
                    Tuple.Create("Dexterity", "Dexterity"),
                    Tuple.Create("Intelligence", "Intelligence"),
                    Tuple.Create("Wisdom", "Wisdom"),
                    Tuple.Create("Constitution", "Constitution"),
                    Tuple.Create("Charisma", "Charisma"),
                    Tuple.Create("BLANK", "BLANK"),
                    Tuple.Create("Karma", "Karma"),
                };

                List<Tuple<string, string>> StatsRightColumn = new List<Tuple<string, string>>()
                {
                    Tuple.Create("Age", "AgeDescription"),
                    Tuple.Create("Alignment", "Alignment"),
                    Tuple.Create("Spec", "ClassFullName"),
                    Tuple.Create("Level", "Level"),
                    Tuple.Create("BLANK", "BLANK"),
                    Tuple.Create("Health Max", "HitsMax"),
                    Tuple.Create("Health Doctored", "HitsDoctored"),
                    Tuple.Create("BLANK", "BLANK"),
                    Tuple.Create("BLANK", "BLANK"),
                    Tuple.Create("BLANK", "BLANK"),
                    Tuple.Create("Stamina Max", "StaminaMax"),
                    Tuple.Create("Stamina Full", "StaminaFull"),
                    Tuple.Create("BLANK", "BLANK"),
                    Tuple.Create("Mana Max", "ManaMax"),
                    Tuple.Create("Mana Full", "ManaFull"),
                    Tuple.Create("BLANK", "BLANK"),
                    Tuple.Create("Strength Add", "StrengthAdd"),
                    Tuple.Create("Dexterity Add", "DexterityAdd"),
                    Tuple.Create("BLANK", "BLANK"),
                    Tuple.Create("Encumbrance", "Encumbrance"),
                    Tuple.Create("Birthday", "Birthday"),
                    Tuple.Create("Rounds", "RoundsPlayed"),
                    Tuple.Create("# Kills", "NumKills"),
                    Tuple.Create("# Deaths", "NumDeaths"),
                };

                PropertyInfo[] properties = Character.CurrentCharacter.GetType().GetProperties(BindingFlags.Public |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Instance);

                Dictionary<string, string> PropertyValues = new Dictionary<string, string>();
                List<string> TypesDesired = new List<string>()
                {
                    "System.String", "System.Int32", "System.Int64", "System.Boolean", "System.Decimal",
                    "Yuusha.World+Alignment", "Yuusha.Character+GenderType", "Yuusha.Character+ClassType"
                };

                foreach (PropertyInfo p in properties)
                {
                    try
                    {
                        object value = p.GetValue(Character.CurrentCharacter);

                        if (value != null && TypesDesired.Contains(value.GetType().ToString()))
                            PropertyValues.Add(p.Name, value == null ? "" : value.ToString());
                    }
                    catch (Exception e)
                    {
                        Utils.LogException(e);
                        continue;
                    }
                }

                int x = 2;
                int y = 2;

                foreach (Tuple<string, string> t in StatsLeftColumn)
                {
                    if (t.Item1.ToLower() != "blank")
                    {
                        if (!Character.CurrentCharacter.IsManaUser && t.Item1.StartsWith("Mana"))
                            continue;

                        PropertyValues.TryGetValue(t.Item2, out string value);
                        if (int.TryParse(value, out int i)) // add commas to numerical values
                            value = string.Format("{0:n0}", i);
                        Label label = new Label(t.Item2 + "StatsLabel", w.Name, new Rectangle(x, y, 200, 20), t.Item1 + ": " + value, Color.White, true, false, w.Font, new VisualKey("WhiteSpace"), Color.White, 0, 255, BitmapFont.TextAlignment.Left, 0, 0, "", "", new List<Enums.EAnchorType>(), "");
                        GuiManager.GenericSheet.AddControl(label);
                        y += 18;
                    }
                    else y += 9;
                }

                x = 212;
                y = 2;

                foreach (Tuple<string, string> t in StatsRightColumn)
                {
                    if (!Character.CurrentCharacter.IsManaUser && t.Item1.StartsWith("Mana"))
                        continue;

                    if (t.Item1.ToLower() != "blank")
                    {
                        PropertyValues.TryGetValue(t.Item2, out string value);
                        if (int.TryParse(value, out int i)) // add commas to numerical values
                            value = string.Format("{0:n0}", i);
                        Label label = new Label(t.Item2 + "StatsLabel", w.Name, new Rectangle(x, y, 200, 20), t.Item1 + ": " + value, Color.White, true, false, w.Font, new VisualKey("WhiteSpace"), Color.White, 0, 255, BitmapFont.TextAlignment.Left, 0, 0, "", "", new List<Enums.EAnchorType>(), "");
                        GuiManager.GenericSheet.AddControl(label);
                        y += 18;
                    }
                    else y += 9;
                }

                w.IsVisible = wasVisible;
            }
        }

        /// <summary>
        /// Currently contains resists and protections information.
        /// </summary>
        public static void UpdateFurtherStatDetailsWindow()
        {
            if (GuiManager.GetControl("FurtherStatDetailsWindow") is Window w)
            {
                bool wasVisible = w.IsVisible;
                w.IsVisible = false;

                w.Controls.RemoveAll(c => c is ScrollableTextBox);

                if (!string.IsNullOrEmpty(Character.CurrentCharacter.ResistsData))
                {
                    string[] resists = Character.CurrentCharacter.ResistsData.Split(Protocol.VSPLIT.ToCharArray());

                    // create resists scrollable text box
                    ScrollableTextBox resistsSTB = new ScrollableTextBox("ResistsScrollableTextBox", w.Name,
                                                   new Rectangle(2, 2, 405, (resists.Length + 2) * BitmapFont.ActiveFonts[w.Font].LineHeight), "", Color.White, true, false, w.Font,
                                                   new VisualKey("WhiteSpace"), Color.DarkMagenta, 0, 255, new VisualKey(""), new VisualKey(""), new VisualKey(""), 0, 0,
                                                   BitmapFont.TextAlignment.Left, new List<Enums.EAnchorType>() { }, true)
                    {

                        Colorize = false,
                    };
                    resistsSTB.AddLine("Resists (Saving Throw Adjustments)", Enums.ETextType.Default);
                    resistsSTB.AddLine("", Enums.ETextType.Default);
                    foreach (string line in resists)
                    {
                        string[] resist = line.Split(Protocol.ISPLIT.ToCharArray());
                        string withColon = resist[0].Trim() + ":";
                        resistsSTB.AddLine(withColon.PadRight(13) + resist[1], Enums.ETextType.Default);
                    }
                    GuiManager.GenericSheet.AddControl(resistsSTB);

                    if (!string.IsNullOrEmpty(Character.CurrentCharacter.ProtectionsData))
                    {
                        string[] protections = Character.CurrentCharacter.ProtectionsData.Split(Protocol.VSPLIT.ToCharArray());

                        // create protections scrollable text box
                        ScrollableTextBox protectionsSTB = new ScrollableTextBox("ProtectionsScrollableTextBox", w.Name,
                                                       new Rectangle(2, resistsSTB.Height + 20, 405, (protections.Length + 2) * BitmapFont.ActiveFonts[w.Font].LineHeight), "", Color.White, true, false, w.Font,
                                                       new VisualKey("WhiteSpace"), Color.DarkMagenta, 0, 255, new VisualKey(""), new VisualKey(""), new VisualKey(""), 0, 0,
                                                       BitmapFont.TextAlignment.Left, new List<Enums.EAnchorType>() { }, true)
                        {

                            Colorize = false,
                        };

                        protectionsSTB.AddLine("Protections (Damage Absorption)", Enums.ETextType.Default);
                        protectionsSTB.AddLine("", Enums.ETextType.Default);
                        foreach (string line in protections)
                        {
                            string[] protection = line.Split(Protocol.ISPLIT.ToCharArray());
                            string withColon = protection[0].Trim() + ":";
                            protectionsSTB.AddLine(withColon.PadRight(13) + protection[1], Enums.ETextType.Default);
                        }
                        GuiManager.GenericSheet.AddControl(protectionsSTB);
                    }
                }

                w.IsVisible = wasVisible;
            }
        }

        public static void UpdateSkillDetailsWindow()
        {
            if (GuiManager.GetControl("SkillDetailsWindow") is Window w)
            {
                bool wasVisible = w.IsVisible;
                w.IsVisible = false;

                foreach (Control c in new List<Control>(w.Controls))
                {
                    GuiManager.RemoveControl(c);
                }

                w.Controls.Clear(); // just in case

                if (!string.IsNullOrEmpty(Character.CurrentCharacter.SkillsData))
                {
                    string[] skills = Character.CurrentCharacter.SkillsData.Split(Protocol.VSPLIT.ToCharArray());

                    ScrollableTextBox skillsSTB = new ScrollableTextBox("SkillsScrollableTextBox", w.Name,
                                                   new Rectangle(2, 2, 405, (skills.Length + 2) * BitmapFont.ActiveFonts[w.Font].LineHeight), "", Color.White, true, false, w.Font,
                                                   new VisualKey("WhiteSpace"), Color.DarkMagenta, 0, 255, new VisualKey(""), new VisualKey(""), new VisualKey(""), 0, 0,
                                                   BitmapFont.TextAlignment.Left, new List<Enums.EAnchorType>() { }, true)
                    {

                        Colorize = false,
                    };

                    skillsSTB.AddLine("Skills", Enums.ETextType.Default);
                    skillsSTB.AddLine("", Enums.ETextType.Default);

                    foreach (string line in skills)
                    {
                        if(line.StartsWith("Alchemy"))
                        {
                            UpdateTradesDetailsWindow();
                            break;
                        }

                        string[] skillInfo = line.Split(Protocol.ISPLIT.ToCharArray());
                        string withColon = TextManager.FormatEnumString(skillInfo[0]) + ":";
                        skillsSTB.AddLine(withColon.PadRight(20) + skillInfo[2] + " (" + skillInfo[1] + ")", Enums.ETextType.Default);
                    }

                    GuiManager.GenericSheet.AddControl(skillsSTB);
                }

                w.IsVisible = wasVisible;
            }
        }

        public static void UpdateTradesDetailsWindow()
        {
            if (GuiManager.GetControl("TradesDetailsWindow") is Window w)
            {
                bool wasVisible = w.IsVisible;
                w.IsVisible = false;
                bool beginTrades = false;

                foreach (Control c in new List<Control>(w.Controls))
                {
                    GuiManager.RemoveControl(c);
                }

                w.Controls.Clear(); // just in case

                if (!string.IsNullOrEmpty(Character.CurrentCharacter.SkillsData))
                {
                    string[] skills = Character.CurrentCharacter.SkillsData.Split(Protocol.VSPLIT.ToCharArray());

                    ScrollableTextBox tradesSTB = new ScrollableTextBox("TradesScrollableTextBox", w.Name,
                                                   new Rectangle(2, 2, 405, (skills.Length + 2) * BitmapFont.ActiveFonts[w.Font].LineHeight), "", Color.White, true, false, w.Font,
                                                   new VisualKey("WhiteSpace"), Color.DarkMagenta, 0, 255, new VisualKey(""), new VisualKey(""), new VisualKey(""), 0, 0,
                                                   BitmapFont.TextAlignment.Left, new List<Enums.EAnchorType>() { }, true)
                    {

                        Colorize = false,
                    };

                    tradesSTB.AddLine("Trades", Enums.ETextType.Default);
                    tradesSTB.AddLine("", Enums.ETextType.Default);

                    foreach (string line in skills)
                    {
                        if (line.StartsWith("Alchemy"))
                        {
                            beginTrades = true;
                        }

                        if (!beginTrades)
                            continue;

                        string[] tradeInfo = line.Split(Protocol.ISPLIT.ToCharArray());
                        string withColon = TextManager.FormatEnumString(tradeInfo[0]) + ":";
                        tradesSTB.AddLine(withColon.PadRight(20) + tradeInfo[2] + " (" + tradeInfo[1] + ")", Enums.ETextType.Default);
                    }

                    GuiManager.GenericSheet.AddControl(tradesSTB);
                }

                w.IsVisible = wasVisible;
            }
        }

        public static void UpdateInventoryWindow()
        {
            if(GuiManager.GenericSheet["InventoryWindow"] is Window w)
            {
                if (Character.CurrentCharacter.Gender == Character.GenderType.Female)
                    w.VisualKey = "Inventory_Female";
                else w.VisualKey = "Inventory_Male";

                Item item = null;

                foreach(Control c in w.Controls)
                {
                    if(c is DragAndDropButton b)
                    {
                        string slot = b.Name.Replace("DragAndDropButton", "");

                        switch(slot)
                        {
                            case "Neck":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Neck, Character.WearOrientation.None);
                                break;
                            case "Head":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Head, Character.WearOrientation.None);
                                break;
                            case "Shoulders":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Shoulders, Character.WearOrientation.None);
                                break;
                            case "Back":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Back, Character.WearOrientation.None);
                                break;
                            case "Face":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Face, Character.WearOrientation.None);
                                break;
                            case "Torso":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Torso, Character.WearOrientation.None);
                                break;
                            case "LeftBicep":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Bicep, Character.WearOrientation.Left);
                                break;
                            case "RightBicep":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Bicep, Character.WearOrientation.Right);
                                break;
                            case "LeftWrist":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Wrist, Character.WearOrientation.Left);
                                break;
                            case "RightWrist":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Wrist, Character.WearOrientation.Right);
                                break;
                            case "Waist":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Waist, Character.WearOrientation.None);
                                break;
                            case "Legs":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Legs, Character.WearOrientation.None);
                                break;
                            case "Feet":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Feet, Character.WearOrientation.None);
                                break;
                            case "Hands":
                                item = Character.CurrentCharacter.GetInventoryItem(Character.WearLocation.Hands, Character.WearOrientation.None);
                                break;
                        }

                        if(item != null)
                        {
                            b.VisualKey = item.VisualKey;
                            b.IsLocked = false;
                            b.AcceptingDroppedButtons = false;
                            b.PopUpText = item.Name;
                            b.RepresentedItem = item;
                        }
                        else
                        {
                            b.AcceptingDroppedButtons = true;
                            b.VisualKey = "";
                            b.IsLocked = true;
                            b.PopUpText = "";
                            b.RepresentedItem = null;
                        }
                    }
                }
            }
        }

        public static void UpdateRingsWindow()
        {
            if (GuiManager.GenericSheet["RingsWindow"] is Window w)
            {
                Item item = null;

                foreach (Control c in w.Controls)
                {
                    if (c is DragAndDropButton b)
                    {
                        string slot = b.Name.Replace("DragAndDropButton", "");

                        switch (slot)
                        {
                            case "LeftRing1":
                                item = Character.CurrentCharacter.GetRing(Character.WearOrientation.LeftRing1);
                                break;
                            case "LeftRing2":
                                item = Character.CurrentCharacter.GetRing(Character.WearOrientation.LeftRing2);
                                break;
                            case "LeftRing3":
                                item = Character.CurrentCharacter.GetRing(Character.WearOrientation.LeftRing3);
                                break;
                            case "LeftRing4":
                                item = Character.CurrentCharacter.GetRing(Character.WearOrientation.LeftRing4);
                                break;
                            case "RightRing1":
                                item = Character.CurrentCharacter.GetRing(Character.WearOrientation.RightRing1);
                                break;
                            case "RightRing2":
                                item = Character.CurrentCharacter.GetRing(Character.WearOrientation.RightRing2);
                                break;
                            case "RightRing3":
                                item = Character.CurrentCharacter.GetRing(Character.WearOrientation.RightRing3);
                                break;
                            case "RightRing4":
                                item = Character.CurrentCharacter.GetRing(Character.WearOrientation.RightRing4);
                                break;
                        }

                        if (item != null)
                        {
                            b.VisualKey = item.VisualKey;
                            b.IsLocked = false;
                            b.AcceptingDroppedButtons = false;
                            b.PopUpText = item.Name;
                            b.RepresentedItem = item;
                        }
                        else
                        {
                            b.AcceptingDroppedButtons = true;
                            b.VisualKey = "";
                            b.IsLocked = true;
                            b.PopUpText = "";
                            b.RepresentedItem = null;
                        }
                    }
                }
            }
        }

        public static void UpdateEffectsWindow(Sheet sheet)
        {
            try
            {
                if (sheet["EffectsWindow"] is Window w)
                {
                    w.Controls.RemoveAll(c => c is Label);
                    int size = 40;
                    int spacing = 1;
                    int borderWidth = w.WindowBorder != null ? w.WindowBorder.Width : 0;
                    int labelsPerRow = 8;

                    if (Character.CurrentCharacter.Effects.Count > 0)
                    {
                        int x = borderWidth + spacing;
                        int y = (w.WindowTitle != null ? w.WindowTitle.Height + spacing : 0) + borderWidth + spacing;

                        foreach (Effect effect in Character.CurrentCharacter.Effects)
                        {
                            VisualKey visual = new VisualKey("WhiteSpace");
                            string text = effect.Name[0].ToString();
                            string effectPopUp = TextManager.FormatEnumString(effect.Name);
                            Color tintColor = Color.Transparent;

                            // time remaining
                            if (effect.Duration > 0)
                            {
                                TimeSpan timeRemaining = Utils.RoundsToTimeSpan(effect.Duration);
                                if (timeRemaining < TimeSpan.FromMinutes(60))
                                    effectPopUp += " [" + string.Format("{0:D2}", timeRemaining.Minutes) + ":" + string.Format("{0:D2}", timeRemaining.Seconds) + "]";
                                else effectPopUp += " [" + timeRemaining.ToString() + "]";
                            }

                            // visual key of effect/spell if it exists
                            if (Effect.IconsDictionary.ContainsKey(TextManager.FormatEnumString(effect.Name)))
                            {
                                visual = new VisualKey(Effect.IconsDictionary[TextManager.FormatEnumString(effect.Name)]);
                                text = "";
                                tintColor = Color.White;

                                if (Effect.IconsTintDictionary.ContainsKey(TextManager.FormatEnumString(effect.Name)))
                                    tintColor = Effect.IconsTintDictionary[TextManager.FormatEnumString(effect.Name)];
                            }

                            EffectLabel label = new EffectLabel(effect.Name + "Label", w.Name, new Rectangle(x, y, size, size), text, Color.White, true, false, w.Font,
                                visual, tintColor, 255, 255, BitmapFont.TextAlignment.Center, 0, 0, "", "", new List<Enums.EAnchorType>(), effectPopUp)
                            {
                                EffectName = effect.Name,
                                TimeCreated = System.DateTime.Now,
                                Duration = effect.Duration,
                                Timeless = effect.Duration <= 0,
                            };

                            GuiManager.CurrentSheet.AddControl(label);

                            Color borderColor = Color.DimGray;

                            if (Effect.NegativeEffects.Contains(TextManager.FormatEnumString(effect.Name)))
                                borderColor = Color.Red;

                            if (Effect.ShortTermPositiveEffects.Contains(TextManager.FormatEnumString(effect.Name)))
                                borderColor = Color.Lime;

                            SquareBorder border = new SquareBorder(label.Name + "Border", label.Name, 1, new VisualKey("WhiteSpace"), false, borderColor, 175);
                            GuiManager.CurrentSheet.AddControl(border);

                            x += size + spacing;

                            if (Client.GameDisplayMode == Enums.EGameDisplayMode.Yuusha)
                            {
                                if (x >= labelsPerRow * (size + spacing) + borderWidth + 1)
                                {
                                    x = borderWidth + 1;
                                    y += size + spacing;
                                }
                            }
                        }
                    }

                    int rowCount = 1;
                    if (Client.GameDisplayMode == Enums.EGameDisplayMode.Yuusha && Character.CurrentCharacter.Effects.Count > labelsPerRow)
                    {
                        rowCount = (int)Math.Ceiling(Character.CurrentCharacter.Effects.Count / (decimal)labelsPerRow);
                        w.Width = labelsPerRow * (size + spacing) + spacing + (borderWidth * 2);
                    }
                    else w.Width = Character.CurrentCharacter.Effects.Count * (size + spacing) + spacing + 1;
                    if (w.WindowTitle != null) w.WindowTitle.Width = w.Width;
                    w.Height = (w.WindowTitle != null ? w.WindowTitle.Height : 0) + (rowCount * (size + spacing)) + (borderWidth * 2);

                    if (Client.InGame && Character.CurrentCharacter.Effects.Count > 0)
                        w.IsVisible = true;
                    else w.IsVisible = false;
                }
            }
            catch(Exception e)
            {
                Utils.LogException(e);
            }
        }

        public static void UpdateWornEffectsWindow()
        {
            try
            {
                if (GuiManager.GenericSheet["WornEffectsWindow"] is Window w)
                {
                    w.IsVisible = false;

                    w.Controls.RemoveAll(c => c is Label);
                    int size = 40;
                    int spacing = 1;
                    int borderWidth = w.WindowBorder != null ? w.WindowBorder.Width : 0;
                    int labelsPerRow = GuiManager.GetControl(w.Owner).Width / size;

                    if (Character.CurrentCharacter.WornEffects.Count > 0)
                    {
                        int x = borderWidth + spacing;
                        int y = (w.WindowTitle != null ? w.WindowTitle.Height + spacing : 0) + borderWidth + spacing;
                        
                        List<string> AddedEffects = new List<string>();

                        foreach (Effect effect in Character.CurrentCharacter.WornEffects)
                        {
                            if (!AddedEffects.Contains(effect.Name))
                            {
                                VisualKey visual = new VisualKey("WhiteSpace");
                                string text = effect.Amount.ToString();
                                if (effect.Amount <= 0) text = "";
                                string effectPopUp = TextManager.FormatEnumString(effect.Name);
                                Color tintColor = Color.Transparent;

                                // time remaining
                                //if (effect.Duration > 0)
                                //{
                                //    TimeSpan timeRemaining = Utils.RoundsToTimeSpan(effect.Duration);
                                //    if (timeRemaining < System.TimeSpan.FromMinutes(60))
                                //        effectPopUp += " [" + string.Format("{0:D2}", timeRemaining.Minutes) + ":" + string.Format("{0:D2}", timeRemaining.Seconds) + "]";
                                //    else effectPopUp += " [" + timeRemaining.ToString() + "]";
                                //}

                                // visual key of effect/spell if it exists
                                if (Effect.IconsDictionary.ContainsKey(TextManager.FormatEnumString(effect.Name)))
                                {
                                    visual = new VisualKey(Effect.IconsDictionary[TextManager.FormatEnumString(effect.Name)]);
                                    if (effect.Amount > 0)
                                        text = effect.Amount.ToString();
                                    tintColor = Color.White;

                                    if (Effect.IconsTintDictionary.ContainsKey(TextManager.FormatEnumString(effect.Name)))
                                        tintColor = Effect.IconsTintDictionary[TextManager.FormatEnumString(effect.Name)];
                                }

                                EffectLabel label = new EffectLabel(effect.Name + "Label", w.Name, new Rectangle(x, y, size, size), text, Color.White, true, false, w.Font,
                                    visual, tintColor, 255, 255, BitmapFont.TextAlignment.Right, 0, 10, "", "", new List<Enums.EAnchorType>(), effectPopUp)
                                {
                                    EffectName = effect.Name,
                                    TimeCreated = DateTime.Now,
                                    Duration = effect.Duration,
                                    Timeless = effect.Duration <= 0,
                                    ZDepth = 0,
                                };

                                GuiManager.CurrentSheet.AddControl(label);

                                AddedEffects.Add(effect.Name);

                                Color borderColor = Color.DimGray;

                                if (Effect.NegativeEffects.Contains(effect.Name))
                                    borderColor = Color.Red;

                                SquareBorder border = new SquareBorder(label.Name + "Border", label.Name, 1, new VisualKey("WhiteSpace"), false, borderColor, 175);
                                GuiManager.CurrentSheet.AddControl(border);

                                x += size + spacing;

                                if (Client.GameDisplayMode == Enums.EGameDisplayMode.Yuusha)
                                {
                                    if (x >= labelsPerRow * (size + spacing) + borderWidth + 1)
                                    {
                                        x = borderWidth + 1;
                                        y += size + spacing;
                                    }
                                }
                            }
                            else
                            {
                                if (effect.Amount > 0)
                                {
                                    if (w[effect.Name + "Label"] is Control c)
                                    {
                                        int currentAmount = Convert.ToInt32(c.Text);
                                        c.Text = (currentAmount + effect.Amount).ToString();
                                    }
                                }
                            }
                        }
                    }

                    int rowCount = 1;
                    if (Client.GameDisplayMode == Enums.EGameDisplayMode.Yuusha && Character.CurrentCharacter.WornEffects.Count > labelsPerRow)
                    {
                        rowCount = (int)Math.Ceiling(Character.CurrentCharacter.WornEffects.Count / (decimal)labelsPerRow);
                        w.Width = labelsPerRow * (size + spacing) + spacing + (borderWidth * 2);
                    }
                    else w.Width = Character.CurrentCharacter.WornEffects.Count * (size + spacing) + spacing + 1;
                    if (w.WindowTitle != null) w.WindowTitle.Width = w.Width;
                    w.Height = (w.WindowTitle != null ? w.WindowTitle.Height : 0) + (rowCount * (size + spacing)) + (borderWidth * 2);

                    if (!string.IsNullOrEmpty(w.Owner))
                    {
                        w.Width = GuiManager.GetControl(w.Owner).Width;
                        if (w.WindowTitle != null) w.WindowTitle.Width = w.Width;
                    }

                    w.IsVisible = true;
                }
            }
            catch(Exception e)
            {
                Utils.LogException(e);
            }
        }

        //public void MapPortalFade()
        //{

        //}

        //public void Resurrection()
        //{

        //}

        private static bool AcceptingGridBoxIsLocation(string name)
        {
            return name.StartsWith("Ground") || name.StartsWith("Altar") || name.StartsWith("Counter");
        }

        public static void ChangeMapDisplayWindowSize(int tileSizeChange)
        {
            if (GuiManager.GetControl("MapDisplayWindow") is Window w)
            {
                int currentSize = w["Tile0"].Width;

                // Maximum or minimum size decided here...
                if (currentSize + tileSizeChange < MAP_GRID_MINIMUM || currentSize + tileSizeChange > MAP_GRID_MAXIMUM)
                    return;

                List<Control> copiedControls = new List<Control>(w.Controls);

                ChangingMapDisplaySize = true;

                w.Controls.RemoveAll(c => c is SpinelTileLabel);

                w.Position = new Point(w.Position.X - (tileSizeChange / 2 * 7), w.Position.Y - (tileSizeChange / 2 * 7));
                w.Width = ((currentSize + tileSizeChange) * 7) + 4; // 2 pixel padding on each side
                w.Height = ((currentSize + tileSizeChange) * 7) + 4;

                int x = 2;
                int y = 2;
                int count = 0;
                for (int a = 0; a < 49; a++, count++, x += currentSize + tileSizeChange)
                {
                    if (count == 7) // new row, reset column
                    {
                        x = 2;
                        y += currentSize + tileSizeChange;
                        count = 0;
                    }

                    GuiManager.Sheets[w.Sheet].CreateSpinelTileLabel("Tile" + a, w.Name, new Rectangle(x, y, currentSize + tileSizeChange, currentSize + tileSizeChange), "", copiedControls[a].TextColor, copiedControls[a].IsVisible,
                        copiedControls[a].IsDisabled, copiedControls[a].Font, new VisualKey(copiedControls[a].VisualKey), copiedControls[a].TintColor, 255, 255, copiedControls[a].TextAlignment, 0, 0, "", "", new List<Enums.EAnchorType>(), "");
                }

                if (w.WindowBorder != null)
                    w.WindowBorder.Width = w.Width;
                w.WindowBorder.Height = w.Height;

                if (w.WindowTitle != null)
                    w.WindowTitle.Width = w.Width;

                YuushaMode.BuildMap();

                if(GuiManager.GetControl("FogOfWarMapWindow") is FogOfWarWindow m)
                {
                    GuiManager.RemoveControl(GuiManager.GetControl("FogOfWarMapWindow"));
                    System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(() => Events.RegisterEvent(Events.EventName.Toggle_FogOfWar));
                    t.Start();
                    t.Wait();
                }

                ChangingMapDisplaySize = false;
            }
        }

        public static void DisplayCastSpell(int spellID)
        {
            Spell spell = World.GetSpellByID(spellID);

            if (spell != null)
            {
                switch (spell.Name)
                {
                    case "Summon Hellhound":
                    case "Summon Nature's Ally":
                    case "Summon Humanoid":
                    case "Summon Phantasm":
                        SpellEffectLabel.CreateSpellEffectLabel(spell.Name);
                        break;
                }
            }
        }
    }
}
