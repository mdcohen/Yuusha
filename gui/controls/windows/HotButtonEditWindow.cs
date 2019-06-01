using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class HotButtonEditWindow : Window
    {
        public string OriginatingWindow; // which window opened up HotButtonEditMode

        public string SelectedHotButton; // The name of the button being edited
        public Label SelectedIconLabel;
        public string SelectedVisualKey;
        public string IconImagePrefix;
        private bool IconSelectionButtonsCreated = false;

        public HotButtonEditWindow(string name, string owner, Rectangle rectangle, bool visible, bool locked, bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, bool dropShadow, Map.Direction shadowDirection, int shadowDistance, List<Enums.EAnchorType> anchors, string cursorOverride) : base(name, owner, rectangle, visible, locked, disabled, font, visualKey, tintColor, visualAlpha, borderAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride)
        {
        }

        public override void OnClose()
        {
            base.OnClose();

            Events.RegisterEvent(Events.EventName.Set_Game_State, GameHUD.PreviousGameState);

            // Make the Icons Window visible again. It was right clicked to reach this HotButtonEditWindow.

            if (GuiManager.GetControl(OriginatingWindow) is Window iconWindow)
                iconWindow.IsVisible = true;
        }

        public void CreateIconSelectionButtons()
        {
            if (IconSelectionButtonsCreated) return;

            try
            {
                List<string> IconVisualKeys = new List<string>();
                int x = 6;
                int y = 180;
                int width = 48; // 34
                int height = 48; // 34
                int padding = 2;

                foreach (string visualName in GuiManager.Visuals.Keys)
                {
                    if (visualName.StartsWith(IconImagePrefix))
                        IconVisualKeys.Add(visualName);
                }

                //IconVisualKeys.Sort();

                int columnCount = 0;
                int rowCount = 0;
                int a = 0;

                VisualKey emptyKey = new gui.VisualKey("");

                for (a = 0; a < IconVisualKeys.Count; a++)
                {
                    GuiManager.CurrentSheet.CreateButton("IconImageSelectionButton", IconImagePrefix + "_" + a, this.Name,
                        new Rectangle(x, y, width, height), this.Name, false, Color.White, true, false, GuiManager.Sheets[this.Sheet].Font, new VisualKey(IconVisualKeys[a]), Color.White,
                        255, 255, 255, emptyKey, emptyKey, emptyKey, "", BitmapFont.TextAlignment.Center, 0, 0, Color.White, false, Color.White, false, new List<Enums.EAnchorType>() { Enums.EAnchorType.Top, Enums.EAnchorType.Left }, false, Map.Direction.Northwest, 2, "", "", "");

                    columnCount++;
                    x += width + padding;

                    if (columnCount == 31)
                    {
                        columnCount = 0; rowCount++; x = 6; y += height + padding;
                    }
                }

                //// Move down two rows.
                //rowCount++;
                //x = 6; y += height + padding;

                //// Create one more for blank appearance icon.
                //GuiManager.CurrentSheet.CreateButton("IconImageSelectionButton", "WhiteSpace", this.Name,
                //        new Rectangle(x, y, width + 20, height), "No Icon", true, Color.Black, true, false, GuiManager.Sheets[this.Sheet].Font, new VisualKey("WhiteSpace"), Color.FloralWhite,
                //        255, 255, 255, emptyKey, emptyKey, emptyKey, "", BitmapFont.TextAlignment.Center, 0, 0, Color.White, false, new List<Enums.eAnchorType>() { Enums.eAnchorType.Top, Enums.eAnchorType.Left }, false, Map.Direction.Northwest, 2, "");

                IconSelectionButtonsCreated = true;
            }
            catch(Exception e)
            { Utils.LogException(e); }
        }
    }
}
