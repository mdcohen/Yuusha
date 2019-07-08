using System;

namespace Yuusha
{
    public static class Protocol
    {
        public enum PromptStates
        {
            Normal, Stunned, Blind, Feared, Resting, Meditating
        }

        public static string USPLIT = "?";
        public static string ASPLIT = " "; // attribute delimiter
        public static string ISPLIT = "^"; // item delimiter
        public static string VSPLIT = "~"; // variable delimiter (if multiple items in proto line)
        public static string TEXT_RETURN = (char)27 + "UU" + (char)27;

        #region Commands
        public static string PING = (char)27 + "88" + (char)27;
        public static string DELETE_CHARACTER = (char)27 + "89" + (char)27;
        public static string CHARGEN_RECEIVE = (char)27 + "90" + (char)27;
        public static string GET_SCORES = (char)27 + "91" + (char)27;
        public static string GOTO_GAME = (char)27 + "92" + (char)27;
        public static string GOTO_CHARGEN = (char)27 + "93" + (char)27;
        public static string GOTO_MENU = (char)27 + "94" + (char)27;
        public static string GOTO_CONFERENCE = (char)27 + "95" + (char)27;
        public static string LOGOUT = (char)27 + "96" + (char)27;
        public static string SWITCH_CHARACTER = (char)27 + "97" + (char)27;
        public static string SET_PROTOCOL = (char)27 + "98" + (char)27;
        public static string SET_CLIENT = (char)27 + "99" + (char)27;
        #endregion

        #region Version Information
        public static string VERSION_SERVER = (char)27 + "V0" + (char)27;
        public static string VERSION_SERVER_END = (char)27 + "V1" + (char)27;
        public static string VERSION_CLIENT = (char)27 + "V2" + (char)27;
        public static string VERSION_CLIENT_END = (char)27 + "V3" + (char)27;
        public static string VERSION_MASTERROUNDINTERVAL = (char)27 + "V4" + (char)27;
        public static string VERSION_MASTERROUNDINTERVAL_END = (char)27 + "V5" + (char)27;
        #endregion

        public static string ACCOUNT_INFO = (char)27 + "A0" + (char)27;
        public static string ACCOUNT_INFO_END = (char)27 + "A1" + (char)27;

        #region Character Information
        public static string CHARACTER_LIST = (char)27 + "C0" + (char)27;
        public static string CHARACTER_LIST_END = (char)27 + "C1" + (char)27;
        public static string CHARACTER_STATS = (char)27 + "C2" + (char)27;
        public static string CHARACTER_STATS_END = (char)27 + "C3" + (char)27;
        public static string CHARACTER_RIGHTHAND = (char)27 + "C4" + (char)27;
        public static string CHARACTER_RIGHTHAND_END = (char)27 + "C5" + (char)27;
        public static string CHARACTER_LEFTHAND = (char)27 + "C6" + (char)27;
        public static string CHARACTER_LEFTHAND_END = (char)27 + "C7" + (char)27;
        public static string CHARACTER_INVENTORY = (char)27 + "C8" + (char)27;
        public static string CHARACTER_INVENTORY_END = (char)27 + "C9" + (char)27;
        public static string CHARACTER_SACK = (char)27 + "CA" + (char)27;
        public static string CHARACTER_SACK_END = (char)27 + "CB" + (char)27;
        public static string CHARACTER_BELT = (char)27 + "CC" + (char)27;
        public static string CHARACTER_BELT_END = (char)27 + "CD" + (char)27;
        public static string CHARACTER_RINGS = (char)27 + "CE" + (char)27;
        public static string CHARACTER_RINGS_END = (char)27 + "CF" + (char)27;
        public static string CHARACTER_LOCKER = (char)27 + "CG" + (char)27;
        public static string CHARACTER_LOCKER_END = (char)27 + "CH" + (char)27;
        public static string CHARACTER_SPELLS = (char)27 + "CI" + (char)27;
        public static string CHARACTER_SPELLS_END = (char)27 + "CJ" + (char)27;
        public static string CHARACTER_EFFECTS = (char)27 + "CK" + (char)27;
        public static string CHARACTER_EFFECTS_END = (char)27 + "CL" + (char)27;
        public static string CHARACTER_CURRENT = (char)27 + "CM" + (char)27;
        public static string CHARACTER_CURRENT_END = (char)27 + "CN" + (char)27;
        public static string CHARACTER_LIST_SPLIT = (char)27 + "CZ" + (char)27;
        public static string CHARACTER_HITS_UPDATE = (char)27 + "C00" + (char)27;
        public static string CHARACTER_HITS_UPDATE_END = (char)27 + "C01" + (char)27;
        public static string CHARACTER_STAMINA_UPDATE = (char)27 + "C02" + (char)27;
        public static string CHARACTER_STAMINA_UPDATE_END = (char)27 + "C03" + (char)27;
        public static string CHARACTER_MANA_UPDATE = (char)27 + "C04" + (char)27;
        public static string CHARACTER_MANA_UPDATE_END = (char)27 + "C05" + (char)27;
        public static string CHARACTER_EXPERIENCE = (char)27 + "C06" + (char)27;
        public static string CHARACTER_EXPERIENCE_END = (char)27 + "C07" + (char)27;
        public static string CHARACTER_MACROS = (char)27 + "C08" + (char)27;
        public static string CHARACTER_MACROS_END = (char)27 + "C09" + (char)27;
        public static string CHARACTER_PROMPT_STATE = (char)27 + "C10" + (char)27;
        public static string CHARACTER_PROMPT_STATE_END = (char)27 + "C11" + (char)27;
        public static string CHARACTER_POUCH = (char)27 + "C12" + (char)27;
        public static string CHARACTER_POUCH_END = (char)27 + "C13" + (char)27;
        public static string CHARACTER_TALENTS = (char)27 + "C14" + (char)27;
        public static string CHARACTER_TALENTS_END = (char)27 + "C15" + (char)27;
        public static string CHARACTER_MAIL = (char)27 + "C16" + (char)27;
        public static string CHARACTER_MAIL_END = (char)27 + "C17" + (char)27;

        #endregion

        #region Main Menu, News, Detect Protocol/Client
        public static string MENU_MAIN = (char)27 + "M0" + (char)27;
        public static string NEWS = (char)27 + "M1" + (char)27;
        public static string NEWS_END = (char)27 + "M2" + (char)27;
        public static string DETECT_PROTOCOL = (char)27 + "M3" + (char)27;
        public static string DETECT_CLIENT = (char)27 + "M4" + (char)27;
        public static string MESSAGEBOX = (char)27 + "M5" + (char)27;
        public static string MESSAGEBOX_END = (char)27 + "M6" + (char)27;
        #endregion

        #region Text
        public static string TEXT_PLAYERCHAT = (char)27 + "T00" + (char)27;
        public static string TEXT_PLAYERCHAT_END = (char)27 + "T01" + (char)27;
        public static string TEXT_HEADER = (char)27 + "T02" + (char)27;
        public static string TEXT_HEADER_END = (char)27 + "T03" + (char)27;
        public static string TEXT_STATUS = (char)27 + "T04" + (char)27;
        public static string TEXT_STATUS_END = (char)27 + "T05" + (char)27;
        public static string TEXT_PRIVATE = (char)27 + "T06" + (char)27;
        public static string TEXT_PRIVATE_END = (char)27 + "T07" + (char)27;
        public static string TEXT_ENTER = (char)27 + "T08" + (char)27;
        public static string TEXT_ENTER_END = (char)27 + "T09" + (char)27;
        public static string TEXT_EXIT = (char)27 + "T10" + (char)27;
        public static string TEXT_EXIT_END = (char)27 + "T11" + (char)27;
        public static string TEXT_SYSTEM = (char)27 + "T12" + (char)27;
        public static string TEXT_SYSTEM_END = (char)27 + "T13" + (char)27;
        public static string TEXT_HELP = (char)27 + "T14" + (char)27;
        public static string TEXT_HELP_END = (char)27 + "T15" + (char)27;
        public static string TEXT_LISTING = (char)27 + "T16" + (char)27;
        public static string TEXT_LISTING_END = (char)27 + "T17" + (char)27;
        public static string TEXT_ERROR = (char)27 + "T18" + (char)27;
        public static string TEXT_ERROR_END = (char)27 + "T19" + (char)27;
        public static string TEXT_FRIEND = (char)27 + "T20" + (char)27;
        public static string TEXT_FRIEND_END = (char)27 + "T21" + (char)27;
        public static string TEXT_PAGE = (char)27 + "T22" + (char)27;
        public static string TEXT_PAGE_END = (char)27 + "T23" + (char)27;
        public static string TEXT_CREATURECHAT = (char)27 + "T24" + (char)27;
        public static string TEXT_CREATURECHAT_END = (char)27 + "T25" + (char)27;
        #endregion

        // Command prefix is 'I'
        public static string IMP_CHARACTERFIELDS = (char)27 + "I0" + (char)27;
        public static string IMP_CHARACTERFIELDS_END = (char)27 + "I1" + (char)27;

        // Command prefix is 'S'
        public static string SOUND = (char)27 + "S0" + (char)27;
        public static string SOUND_END = (char)27 + "S1" + (char)27;
        public static string SOUND_FROM_CLIENT = (char)27 + "S2" + (char)27;

        // Command prefix is 'CG'
        public static string CHARGEN_ENTER = (char)27 + "CG0" + (char)27;
        public static string CHARGEN_ROLLER_RESULTS = (char)27 + "CG1" + (char)27;
        public static string CHARGEN_ROLLER_RESULTS_END = (char)27 + "CG2" + (char)27;
        public static string CHARGEN_ERROR = (char)27 + "CG3" + (char)27;
        public static string CHARGEN_INVALIDNAME = (char)27 + "CG4" + (char)27;
        public static string CHARGEN_ACCEPTED = (char)27 + "CG5" + (char)27;

        #region Requests
        public static string REQUEST_CHARACTER_INVENTORY = (char)27 + "R8" + (char)27;
        public static string REQUEST_CHARACTER_SACK = (char)27 + "RA" + (char)27;
        public static string REQUEST_CHARACTER_POUCH = (char)27 + "RB" + (char)27;
        public static string REQUEST_CHARACTER_BELT = (char)27 + "RC" + (char)27;
        public static string REQUEST_CELLITEMS = (char)27 + "RD" + (char)27;
        public static string REQUEST_CHARACTER_RINGS = (char)27 + "RE" + (char)27;
        public static string REQUEST_CHARACTER_STATS = (char)27 + "RF" + (char)27;
        public static string REQUEST_CHARACTER_LOCKER = (char)27 + "RG" + (char)27;
        public static string REQUEST_CHARACTER_SPELLS = (char)27 + "RI" + (char)27;
        public static string REQUEST_CHARACTER_EFFECTS = (char)27 + "RK" + (char)27;
        public static string REQUEST_CHARACTER_SKILLS = (char)27 + "RO" + (char)27;
        public static string REQUEST_CHARACTER_TALENTS = (char)27 + "RP" + (char)27;
        #endregion

        #region Conference Room
        public static string CONF_ENTER = (char)27 + "F0" + (char)27;
        public static string CONF_INFO = (char)27 + "F1" + (char)27;
        public static string CONF_INFO_END = (char)27 + "F2" + (char)27;
        #endregion

        #region Game Information
        public static string GAME_CELL = (char)27 + "G0" + (char)27;
        public static string GAME_CELL_END = (char)27 + "G1" + (char)27;
        public static string GAME_CELL_INFO = (char)27 + "G2" + (char)27;
        public static string GAME_CELL_INFO_END = (char)27 + "G3" + (char)27;
        public static string GAME_CELL_CRITTERS = (char)27 + "G4" + (char)27;
        public static string GAME_CELL_CRITTERS_END = (char)27 + "G5" + (char)27;
        public static string GAME_CELL_ITEMS = (char)27 + "G6" + (char)27;
        public static string GAME_CELL_ITEMS_END = (char)27 + "G7" + (char)27;
        public static string GAME_CRITTER_INFO = (char)27 + "G8" + (char)27;
        public static string GAME_CRITTER_INFO_END = (char)27 + "G9" + (char)27;
        public static string GAME_CRITTER_INVENTORY = (char)27 + "GA" + (char)27;
        public static string GAME_CRITTER_INVENTORY_END = (char)27 + "GB" + (char)27;
        public static string GAME_CELL_EFFECTS = (char)27 + "GC" + (char)27;
        public static string GAME_CELL_EFFECTS_END = (char)27 + "GD" + (char)27;
        public static string GAME_WORLD_INFO = (char)27 + "GE" + (char)27;
        public static string GAME_WORLD_INFO_END = (char)27 + "GF" + (char)27;
        public static string GAME_EXIT = (char)27 + "GG" + (char)27;
        public static string GAME_TEXT = (char)27 + "GH" + (char)27;
        public static string GAME_TEXT_END = (char)27 + "GI" + (char)27;
        public static string GAME_NEW_ROUND = (char)27 + "GJ" + (char)27;
        public static string GAME_END_ROUND = (char)27 + "GK" + (char)27;
        public static string GAME_ROUND_DELAY = (char)27 + "GL" + (char)27;
        public static string GAME_ENTER = (char)27 + "GM" + (char)27;
        public static string GAME_POINTER_UPDATE = (char)27 + "GN" + (char)27;
        public static string GAME_CHARACTER_DEATH = (char)27 + "GO" + (char)27; // followed by ID number
        #endregion

        #region World Information
        public static string WORLD_SPELLS = (char)27 + "W0" + (char)27;
        public static string WORLD_SPELLS_END = (char)27 + "W1" + (char)27;
        public static string WORLD_LANDS = (char)27 + "W2" + (char)27;
        public static string WORLD_LANDS_END = (char)27 + "W3" + (char)27;
        public static string WORLD_MAPS = (char)27 + "W4" + (char)27;
        public static string WORLD_MAPS_END = (char)27 + "W5" + (char)27;
        public static string WORLD_SCORES = (char)27 + "W6" + (char)27;
        public static string WORLD_SCORES_END = (char)27 + "W7" + (char)27;
        public static string WORLD_USERS = (char)27 + "W8" + (char)27;
        public static string WORLD_USERS_END = (char)27 + " W9" + (char)27;
        public static string WORLD_INFORMATION = (char)27 + "WA" + (char)27;
        public static string WORLD_CHARGEN_INFO = (char)27 + "WB" + (char)27;
        public static string WORLD_CHARGEN_INFO_END = (char)27 + "WC" + (char)27;
        public static string WORLD_CELL_INFO = (char)27 + "WD" + (char)27;
        public static string WORLD_CELL_INFO_END = (char)27 + "WE" + (char)27;
        public static string WORLD_ITEMS = (char)27 + "WF" + (char)27;
        public static string WORLD_ITEMS_END = (char)27 + "WG" + (char)27;
        #endregion

        public static string GetProtoInfoFromString(string inData, string startProto, string endProto)
        {
            string protoInfo = "";

            try
            {
                protoInfo = inData.Substring(inData.IndexOf(startProto) + startProto.Length, inData.IndexOf(endProto) - (inData.IndexOf(startProto) + startProto.Length));
            }
            catch (Exception e)
            {                
                Utils.Log("Failure at GetProtoInfoFromString(info, startProto, endProto)");
                Utils.LogOnce("InData = " + inData);
                Utils.LogOnce("STARTPROTO = " + startProto);
                Utils.LogOnce("ENDPROTO = " + endProto);//" + info + ", " + startProto + ", " + endProto + ")");
                Utils.LogException(e);
            }

            return protoInfo;
        }

        public static void DisplayMessageBox(string info)
        {
            string[] boxArgs = info.Split(Protocol.VSPLIT.ToCharArray());

            //Utility.Log("boxArgs Length = " + boxArgs.Length + "  info = " + info);

            if (boxArgs.Length == 4)
            {
                //MessageBox.Show(boxArgs[0], boxArgs[1], (MessageBoxButtons)Enum.Parse(typeof(MessageBoxButtons), boxArgs[2]), (MessageBoxIcon)Enum.Parse(typeof(MessageBoxIcon), boxArgs[3]));
            }
            else if (boxArgs.Length == 2)
            {
                //MessageBox.Show(boxArgs[0], boxArgs[1]);
            }
            else
            {
                //MessageBox.Show(info);
            }
        }

    }
}
