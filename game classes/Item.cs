using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Yuusha
{
    public class Item
    {
        private static ArrayList itemCatalog = new ArrayList();

        public enum ItemType { Weapon, Wearable, Container, Miscellaneous, Edible, Potable, Corpse, Coin }

        public enum BaseType
        {
            Unknown, // 0
            Bow,
            Dagger,
            Flail,
            Halberd,
            Mace, // 5
            Rapier,
            Shuriken,
            Staff,
            Sword,
            Threestaff, // 10
            TwoHanded,
            Thievery,
            Magic,
            Unarmed,
            Armor, // 15
            Helm,
            Bracer,
            Shield,
            Boots,
            Bottle, // 20
            Amulet,
            Ring,
            Book,
            Food,
            Gem, // 25
            Figurine,
            Scroll
        }

        public enum LootType { Very_Common, Common, Rare, Very_Rare, Lair }

        public enum AttuneType { None, Attack, Slain, Take, Wear }

        public enum ArmorType { Leather, Chain, Plate }

        public enum AttackType { None, Pierce, Slash, Blunt }

        public enum Size { Belt_Only, Sack_Only, Belt_Or_Sack, No_Container, Belt_Large_Slot_Only }

        public int ID // the item id
        { get; set; }
        public long WorldItemID
        { get; set; }// world item id
        public string notes;
        //public ItemType itemType; // weapon, wearable, container, miscellaneous
        //public BaseType baseType; // the base type of the item
        //public Character.SkillType skillType; // skill used to wield this item
        public string Name
        { get; set; }
        //public string shortDesc; // short description of the item (extended name)
        //public string longDesc; // long description of the item
        public string VisualKey
        { get; set; }
        //public double weight; // weight of the item
        //public Size size; // size of the item
        //public double coinValue; // coin value
        //public string effectType; // magical effect if worn or drink
        //public string effectAmount; // magical effect if worn or drink
        //public long figExp; // figurine experience
        //public int vRandLow; //if vRandLow > 0, random cValue is between vRandLow and vRandHigh
        //public int vRandHigh;
        //public int venom; // if > 0, the amount of poison damage on a successful hit...then reset to 0
        //public bool recall;    //does the item recall
        //public bool wasRecall; //used for recall reset
        //public int recallLand;
        //public int recallMap;
        //public int recallX; // recall values X Y and Map
        //public int recallY;
        //public World.Alignment alignment;
        //public int dropRound; // round item was dropped
        //public string key; // control for keys/locks
        //public int charges; // charges remaining in an item -1 = never has charges, 0 = had charges,
        //public int spell; // spell ID of the spell this item contains
        //public int currentPage;
        //public AttuneType attuneType;
        //public int attunedID; // if an attuned item, this will hold playerID
        //public string special;
        //public int blockRank; // blocking ability of this item
        //public int combatAdds; // attacking ability of this item
        //public double armorClass;
        //public ArmorType armorType;
        //public AttackType attackType;
        //public int minDamage; // minimum damage this weapon can inflict
        //public int maxDamage; // maximum damage this weapon can inflict
        //public string unidentifiedName;
        //public string identifiedName;

        public Character.WearLocation wearLocation;
        public Character.WearOrientation wearOrientation;

        public bool returning; // true if this item will stay in hand if thrown
        public bool flammable; // true if this item will be destroyed by fire
        public bool nocked; // true if this bow is nocked

        public string creationTime;
        public string creationWho;

        public Item()
        {
            // empty constructor
        }

        public Item(string info)
        {
            try
            {
                string[] itemInfo = info.Split(Protocol.VSPLIT.ToCharArray());
                ID = Convert.ToInt32(itemInfo[0]);
                WorldItemID = Convert.ToInt32(itemInfo[1]);
                //this.itemType = (ItemType)Convert.ToInt32(itemInfo[2]);
                //this.baseType = (BaseType)Convert.ToInt32(itemInfo[3]);
                //this.skillType = (Character.SkillType)Convert.ToInt32(itemInfo[4]);
                Name = itemInfo[2];
                VisualKey = itemInfo[3];
                //this.shortDesc = itemInfo[6];
                //this.longDesc = itemInfo[7];
                //this.weight = Convert.ToDouble(itemInfo[8]);
                //this.size = (Size)Convert.ToInt32(itemInfo[9]);
                //this.coinValue = Convert.ToDouble(itemInfo[10]);
                //this.effectType = itemInfo[11];
                //this.effectAmount = itemInfo[12];
                //this.figExp = Convert.ToInt64(itemInfo[13]);
                //this.vRandLow = Convert.ToInt32(itemInfo[14]);
                //this.vRandHigh = Convert.ToInt32(itemInfo[15]);
                //this.venom = Convert.ToInt32(itemInfo[16]);
                //this.recall = Convert.ToBoolean(itemInfo[17]);
                //this.wasRecall = Convert.ToBoolean(itemInfo[18]);
                //this.recallLand = Convert.ToInt32(itemInfo[19]);
                //this.recallMap = Convert.ToInt32(itemInfo[20]);
                //this.recallX = Convert.ToInt32(itemInfo[21]);
                //this.recallY = Convert.ToInt32(itemInfo[22]);
                //this.alignment = (World.Alignment)Convert.ToInt32(itemInfo[23]);
                //this.dropRound = Convert.ToInt32(itemInfo[24]);
                //this.key = itemInfo[25];
                //this.charges = Convert.ToInt32(itemInfo[26]);
                //this.spell = Convert.ToInt32(itemInfo[27]);
                //this.attuneType = (AttuneType)Convert.ToInt32(itemInfo[28]);
                //this.attunedID = Convert.ToInt32(itemInfo[29]);
                //this.special = itemInfo[30];
                //this.combatAdds = Convert.ToInt32(itemInfo[31]);
                //this.armorClass = Convert.ToDouble(itemInfo[32]);
                //this.minDamage = Convert.ToInt32(itemInfo[33]);
                //this.maxDamage = Convert.ToInt32(itemInfo[34]);
                //this.wearLocation = (Character.WearLocation)Convert.ToInt32(itemInfo[35]);
                //this.wearOrientation = (Character.WearOrientation)Convert.ToInt32(itemInfo[36]);
                //this.returning = Convert.ToBoolean(itemInfo[37]);
                //this.flammable = Convert.ToBoolean(itemInfo[38]);
                //this.creationTime = itemInfo[39];
                //this.creationWho = itemInfo[40];
                //this.visualKey = itemInfo[41];
                //this.nocked = Convert.ToBoolean(itemInfo[42]);
                //this.unidentifiedName = itemInfo[43];
                //this.identifiedName = itemInfo[44];
            }
            catch (Exception e)
            {
                Utils.LogException(e);
            }
        }

        public static void addToCatalog(Item item)
        {
            Item.itemCatalog.Add(item);
        }

        public static int getItemCatalogCount()
        {
            return Item.itemCatalog.Count;
        }
    }
}
