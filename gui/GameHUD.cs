using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class GameHUD : GameComponent
    {
        public static Character CurrentTarget;
        public static string TextSendOverride = "";
        public static Enums.EGameState PreviousGameState;
        private static List<Control> SavedControls;

        // Text is not cleared from scrolling textboxes in these ClientStates (they fill the screen)
        public static List<Enums.EGameState> OverrideDisplayStates = new List<Enums.EGameState>
        {
            Enums.EGameState.CharacterGeneration, Enums.EGameState.HotButtonEditMode
        };

        public GameHUD(Game game): base(game)
        {

        }

        public void MapPortalFade()
        {

        }

        public void Resurrection()
        {

        }
    }
}
