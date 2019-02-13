using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Yuusha.gui
{
    public class GenericSheet : Sheet
    {
        public GenericSheet(string xmlFile, System.Xml.XmlReader reader)
            : base(xmlFile, reader)
        {
            // empty
        }

        public override void Update(GameTime gameTime)
        {
            // keyboard handler
            KeyboardHandler(Keyboard.GetState());

            // mouse handler
            MouseHandler(Mouse.GetState());

            // set mouse wheel value after mouse handler
            m_prevScrollWheelValue = Mouse.GetState().ScrollWheelValue;

            // cursor used is current sheet cursor
            m_cursor = GuiManager.CurrentSheet.Cursor;

            // override cursor is current sheet override cursor
            m_cursorOverride = GuiManager.CurrentSheet.CursorOverride;       

            // update controls
            foreach (Control control in m_controls)
                control.Update(gameTime);

            // sort controls
            SortControls();

            // update cursor
            if (m_cursorOverride == "")
            {
                if(GuiManager.Cursors.ContainsKey(m_cursor))
                    GuiManager.Cursors[m_cursor].Update(gameTime);
                else Utils.LogOnce("Failed to find cursor visual key [ " + m_cursor + " ]");
            }
            else
            {
                if (GuiManager.Cursors.ContainsKey(m_cursorOverride))
                    GuiManager.Cursors[m_cursorOverride].Update(gameTime);
                else Utils.LogOnce("Failed to find cursor override visual key [ " + m_cursorOverride + " ]");
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // draw controls - these should all be windows
            foreach (Control control in m_controls)
            {
                if (!control.LockoutStates.Contains(Client.GameState))
                    control.Draw(gameTime);
            }

            // draw cursor
            if (GuiManager.MouseCursorVisible)
            {
                if (m_cursorOverride == "")
                {
                    if (GuiManager.Cursors.ContainsKey(m_cursor))
                    {
                        if (GuiManager.Cursors[m_cursor].IsVisible)
                            GuiManager.Cursors[m_cursor].Draw(gameTime);
                    }
                    else Utils.LogOnce("Failed to find cursor visual key [ " + m_cursor + " ] for GUI Sheet [ " + m_name + " ]");
                }
                else
                {
                    if (GuiManager.Cursors.ContainsKey(m_cursorOverride))
                    { 
                        if (GuiManager.Cursors[m_cursorOverride].IsVisible)
                            GuiManager.Cursors[m_cursorOverride].Draw(gameTime);
                    }
                    else Utils.LogOnce("Failed to find cursor override visual key [ " + m_cursorOverride + " ] for GUI Sheet [ " + m_name + " ]");
                }
            }
        }

        public static void LoadMacros()
        {
            if (Character.CurrentCharacter == null) return;

            ClearMacros();

            for (int a = 0; a < 20; a++)
            {
                if (Character.CurrentCharacter.Macros.Count == a) break;
                
                GuiManager.GetControl("Macro" + a + "Button").Text = Character.CurrentCharacter.Macros[a];
            }
        }

        public static void ClearMacros()
        {
            for (int a = 0; a < 20; a++)
            {
                Control control = GuiManager.GetControl("Macro" + a + "Button");
                if (control != null)
                {
                    control.Text = "";
                }
            }
        }
    }
}
