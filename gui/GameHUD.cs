using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Yuusha.gui
{
    public static class GameHUD
    {
        public static Character CurrentTarget;
        public static Cell CurrentCell;
        public static string TextSendOverride = "";
        public static Enums.EGameState PreviousGameState;

        // Text is not cleared from scrolling textboxes in these ClientStates (they fill the screen)
        public static List<Enums.EGameState> OverrideDisplayStates = new List<Enums.EGameState>
        {
            Enums.EGameState.CharacterGeneration, Enums.EGameState.HotButtonEditMode
        };

        public static void KeyboardHandler(KeyboardState ks)
        {
            if (!Client.HasFocus) return;
        }
    }
}
