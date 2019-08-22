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
        public string IconImagePrefix = "";
        private bool IconSelectionButtonsCreated = false;

        public HotButtonEditWindow(string name, string owner, Rectangle rectangle, bool visible, bool locked, bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, bool dropShadow, Map.Direction shadowDirection, int shadowDistance, List<Enums.EAnchorType> anchors, string cursorOverride) : base(name, owner, rectangle, visible, locked, disabled, font, visualKey, tintColor, visualAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride)
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
            foreach (Control c in new System.Collections.Concurrent.ConcurrentBag<Control>(Controls))
            {
                if (c is IconImageSelectionButton)
                {
                    Controls.Remove(c);
                }
            }

            try
            {
                List<string> IconVisualKeys = new List<string>();
                int x = 6;
                int y = 180;
                int width = 48; // 34
                int height = 48; // 34
                int padding = 3;

                if (!Client.IsFullScreen)
                {
                    width = 31;
                    height = 31;
                    padding = 1;
                }

                foreach (string visualName in GuiManager.Visuals.Keys)
                {
                    if (IconImagePrefix != "" && visualName.StartsWith(IconImagePrefix))
                        IconVisualKeys.Add(visualName);
                }

                int columnCount = 0;
                int rowCount = 0;
                int a = 0;

                VisualKey emptyKey = new VisualKey("");

                for (a = 0; a < IconVisualKeys.Count; a++)
                {
                    GuiManager.CurrentSheet.CreateButton("IconImageSelectionButton", IconImagePrefix + "_" + a, Name,
                        new Rectangle(x, y, width, height), a.ToString(), false, Color.White, true, false, GuiManager.Sheets[Sheet].Font, new VisualKey(IconVisualKeys[a]), Color.White,
                        255, 255, emptyKey, emptyKey, emptyKey, "", BitmapFont.TextAlignment.Center, 0, 30, Color.White, false, Color.White, false, new List<Enums.EAnchorType>(),
                        false, Map.Direction.Northwest, 2, "", "", "", "", false, Client.ClientSettings.DefaultOnClickSound);

                    columnCount++;
                    x += width + padding;

                    if (columnCount == 31)
                    {
                        columnCount = 0;
                        rowCount++;
                        x = 6;
                        y += height + padding;
                    }
                }

                //IconSelectionButtonsCreated = true;
            }
            catch(Exception e)
            { Utils.LogException(e); }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(GuiManager.KeyboardState.IsKeyDown(Keys.LeftAlt))
            {
                foreach(Control c in Controls)
                {
                    if(c is IconImageSelectionButton b)
                    {
                        b.IsTextVisible = true;
                    }
                }
            }
            else
            {
                foreach (Control c in Controls)
                {
                    if (c is IconImageSelectionButton b)
                    {
                        b.IsTextVisible = false;
                    }
                }
            }
        }

        //public override void OnClientResize(Rectangle prev, Rectangle now, bool ownerOverride)
        //{
        //    base.OnClientResize(prev, now, false);
            

        //    //base.OnClientResize(prev, now, ownerOverride);

        //    //IconSelectionButtonsCreated = false;
        //    //CreateIconSelectionButtons();
        //}
    }
}
