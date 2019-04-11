using System;
using System.Collections.Generic;
using System.Text;

namespace Yuusha
{
    public static class Enums
    {
        public enum EGraphicalEffectType
        {
            Heartbeat,
        }

        public enum EScrollbarControlType
        {
            Thumb,
            Upper,
            Lower
        }

        public enum EWindowControlBoxType
        {
            Close,
            Crop,
            Maximize,
            Minimize
        }

        public enum EAnchorType
        {
            Top,
            Bottom,
            Left,
            Right,
            Center // Login Window
        }

        public enum ETextType
        {
            Default,
            PlayerChat,
            Enter,
            Exit,
            Header,
            Status,
            System,
            Help,
            Private,
            Listing,
            Error,
            Friend,
            Page,
            Attuned,
            NPCChat,
            Death,
            SpellCast,
            SpellWarm,
            CombatHit,
            CombatMiss,
            SpellHit,
            SpellMiss
        }

        public enum EPlayerUpdate
        {
            Stats,
            Skills,
            RightHand,
            LeftHand,
            Inventory,
            Sack,
            Belt,
            Rings,
            Locker,
            Spells,
            Effects,
            Hits,
            Stamina,
            Mana,
            Experience,
            Macros
        }

        public enum EGameDisplayMode
        {
            /// <summary>
            /// Normal client mode.
            /// </summary>
            Normal,

            /// <summary>
            /// Island of Kesmai traditional emulation.
            /// </summary>
            IOK,

            /// <summary>
            /// Legends of Kesmai emulation.
            /// </summary>
            LOK,

            /// <summary>
            /// Spinel emulation.
            /// </summary>
            Spinel,
        }

        public enum EGameState
        {
            /// <summary>
            /// Splash screen.
            /// </summary>
            //Splash,

            /// <summary>
            /// Login.
            /// </summary>
            Login,

            /// <summary>
            /// Main menu.
            /// </summary>
            Menu,

            /// <summary>
            /// Character generation.
            /// </summary>
            CharacterGeneration,

            /// <summary>
            /// Conference room.
            /// </summary>
            Conference,

            /// <summary>
            /// Normal Game.
            /// </summary>
            Game,

            /// <summary>
            /// Island of Kesmai game.
            /// </summary>
            IOKGame,

            /// <summary>
            /// Legends of Kesmai game.
            /// </summary>
            LOKGame,

            /// <summary>
            /// Spinel game.
            /// </summary>
            SpinelGame,

            HotButtonEditMode,
        }

        public enum ECharGenState
        {

        }

        public enum ELoginState
        {
            Disconnected,
            Connected,
            VerifyAccount,
            VerifyPassword,
            WorldInformation,
            NewAccount, // all steps of account creation are verified in this login state
            LoggedIn
        }

        public enum EBorderLocation
        {
            TopLeft,
            Top,
            TopRight,
            Left,
            Background,
            Right,
            BotLeft,
            Bot,
            BotRight
        }

        public enum EControlState
        {
            Normal,
            Over,
            Down,
            Disabled,
        }

        public enum ELayoutType
        {
            Horizontal,
            Vertical
        }
    }
}
