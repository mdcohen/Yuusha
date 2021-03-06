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
            Minimize,
            IncreaseSize,
            DecreaseSize,
        }

        public enum EAnchorType
        {
            Top,
            Bottom,
            Left,
            Right,
            Center, // Login Window
            All
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
            Hint,
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
            SpellMiss,
            PrivateMessageReceiver,
            PrivateMessageSender,
            SageAdvice,
            News
        }

        public enum EPlayerUpdate
        {
            Stats,
            Skills,
            RightHand,
            LeftHand,
            Inventory,
            Sack,
            Pouch,
            Belt,
            Rings,
            Locker,
            Spells,
            Talents,
            TalentUse,
            Effects,
            WornEffects,
            Hits,
            Stamina,
            Mana,
            Experience,
            Macros,
            Resists,
            Protections,
            SkillRisk,
            SkillExpChange,
        }

        public enum EGameDisplayMode
        {
            /// <summary>
            /// Island of Kesmai traditional emulation.
            /// </summary>
            IOK,           

            /// <summary>
            /// Spinel emulation.
            /// </summary>
            Spinel,

            /// <summary>
            /// Primary game mode.
            /// </summary>
            Yuusha,
        }

        public enum EGameState
        {
            /// <summary>
            /// Splash screen.
            /// </summary>
            Splash,

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
            /// Spinel game.
            /// </summary>
            SpinelGame,

            HotButtonEditMode,

            YuushaGame
        }

        public enum ECharGenGUIMode
        {
            Text,
            Graphical,
        }

        public enum ECharGenState
        {
            ChooseGender,
            ChooseHomeland,
            ChooseProfession,
            ReviewStats,
            ChooseName,
            CharGenSuccess,
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
            Selected
        }

        public enum ELayoutType
        {
            Horizontal,
            Vertical
        }
    }
}
