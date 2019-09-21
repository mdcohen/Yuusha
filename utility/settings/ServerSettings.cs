using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuusha.Utility.Settings
{
    public class ServerSettings
    {
        public bool Anonymous;
        public string EmailAddress;
        public bool FilterProfanity = false;
        public List<int> FriendsList = new List<int>();
        public bool FriendNotify;
        public List<int> IgnoreList = new List<int>();
        public bool Immortal;
        public bool Invisible;
        public bool ReceiveGroupInvites;
        public bool ReceivePages;
        public bool ReceiveTells;
        public bool ShowStaffTitle;
        public bool DisplayCombatDamage;
        public bool DisplayPetDamage;
        public bool DisplayPetMessages;
        public bool DisplayDamageShield;

        public ServerSettings()
        {

        }

        public ServerSettings(string inData)
        {

        }
    }
}
