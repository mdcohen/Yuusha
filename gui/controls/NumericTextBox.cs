using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class NumericTextBox : TextBox
    {
        private static readonly List<Keys> NumberKeys = new System.Collections.Generic.List<Keys>()
        {
            Keys.D0,
            Keys.D1,
            Keys.D2,
            Keys.D3,
            Keys.D4,
            Keys.D5,
            Keys.D6,
            Keys.D7,
            Keys.D8,
            Keys.D9,
        };

        private static readonly List<Keys> NumberPadKeys = new System.Collections.Generic.List<Keys>()
        {
            Keys.NumPad0,
            Keys.NumPad1,
            Keys.NumPad2,
            Keys.NumPad3,
            Keys.NumPad4,
            Keys.NumPad5,
            Keys.NumPad6,
            Keys.NumPad7,
            Keys.NumPad8,
            Keys.NumPad9,
        };

        protected override bool OnKeyDown(KeyboardState ks)
        {
            if (!Client.HasFocus || !HasFocus || !m_editable)
                return false;

            Keys[] newKeys = ks.GetPressedKeys();

            if (pressedKeys != null)
            {
                foreach (Keys k in newKeys)
                {
                    bool bFound = false;

                    foreach (Keys k2 in pressedKeys)
                    {
                        if (k == k2)
                        {
                            bFound = true;
                            break;
                        }
                    }

                    if (!bFound)
                    {
                        if(NumberKeys.Contains(k))
                        {
                            return base.OnKeyDown(ks);
                        }

                        // NumLock
                        if ((((ushort)Yuusha.KeyboardHandler.GetKeyState(0x90)) & 0xffff) != 0 && NumberPadKeys.Contains(k))
                        {
                            return base.OnKeyDown(ks);
                        }
                        else
                        {
                            // do nothing
                        }
                    }
                }
            }

            return base.OnKeyDown(ks);
        }
    }
}
