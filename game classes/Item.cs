using System;
using System.Collections;

namespace Yuusha
{
    public class Item
    {
        public enum EItemType
        {
            Weapon,
            Wearable,
            Container,
            Miscellaneous,
            Edible,
            Potable,
            Corpse,
            Bauble,
            Literature,
            Coin
        }

        //public enum LootType { Very_Common, Common, Rare, Very_Rare, Lair }

        //public enum AttuneType { None, Attack, Slain, Take, Wear }

        //public enum ArmorType { Leather, Chain, Plate }

        //public enum AttackType { None, Pierce, Slash, Blunt }

        //public enum Size { Belt_Only, Sack_Only, Belt_Or_Sack, No_Container, Belt_Large_Slot_Only }

        public int CatalogID
        { get; set; }
        public long WorldItemID
        { get; set; }
        public string Notes
        { get; set; }
        public string Name
        { get; set; }
        public string VisualKey
        { get; set; }
        public string IdentifiedName
        { get; set; }
        public EItemType ItemType
        { get; set; }
        public bool IsNocked
        { get; set; }
        public Character.WearLocation WearLocation
        { get; set; } = Character.WearLocation.None;
        public Character.WearOrientation WearOrientation
        { get; set; } = Character.WearOrientation.None;

        public bool Nocked
        { get; set; }

        public Item()
        {
            // empty constructor
        }

        public Item(string info)
        {
            try
            {
                string[] itemInfo = info.Split(Protocol.VSPLIT.ToCharArray());

                CatalogID = Convert.ToInt32(itemInfo[0]);
                WorldItemID = Convert.ToInt32(itemInfo[1]);
                Name = itemInfo[2];
                VisualKey = itemInfo[3];
                if (itemInfo.Length > 4)
                    WearLocation = (Character.WearLocation)Convert.ToInt32(itemInfo[4]);
                if (itemInfo.Length > 5)
                    WearOrientation = (Character.WearOrientation)Convert.ToInt32(itemInfo[5]);
                if (itemInfo.Length > 6)
                    IdentifiedName = itemInfo[6];
                if (itemInfo.Length > 7)
                    ItemType = (EItemType)Convert.ToInt32(itemInfo[7]);
                if (itemInfo.Length > 8)
                    IsNocked = Convert.ToBoolean(itemInfo[8]);
            }
            catch (Exception e)
            {
                Utils.LogException(e);
            }
        }

        //public static void addToCatalog(Item item)
        //{
        //    itemCatalog.Add(item);
        //}
    }
}
