using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Yuusha
{
    public class Cell
    {
        public short xCord = 0; // 16 bits
        public short yCord = 0; // 16 bits
        public int zCord = 0;
        public short map = 0; // 16 bits
        public short land = 0; // 16 bits

        public string displayGraphic = "  ";
        public string cellGraphic = "  ";
        public string visual0 = ""; // base visual key
        public string visual1 = ""; // first layer 2D visual key
        public string visual2 = ""; // second layer 2D visual key

        public bool lair = false; // true if the janitor will ignore this cell
        public bool lockers = false; // true if this is a lockers cell
        public bool portal = false;	// true if this is a map portal cell
        public bool secretDoor = false; // true if this cell is a secret door
        public bool singleCustomer = false;	// true if only one player is allowed in this cell at a time
        public bool teleport = false; // true if this is a teleport cell (uses this cell's CellLock for access)
        public bool magicDead = false; // true if magic does not work
        public bool townLimits = false;	// true if within town limits (terrain spells cast here by lawfuls make you neutral)
        public bool noRecall = false; // true if cell does not allow setting recall or recall from
        public bool underworldPortal = false; // true if cell allows access to the Underworld
        public bool ancestorStart = false; // true if cell begins ancestoring process
        public bool ancestorFinish = false;	// true if cell allows completion of ancestoring process
        public bool oneHandClimbUp = false;
        public bool oneHandClimbDown = false;
        public bool twoHandClimbUp = false;
        public bool twoHandClimbDown = false;
        public bool pvpEnabled = false; // true if PvP enabled (no mark or karma penalties)

        public bool visible = false; // is this cell visible from player's current position

        private List<Effect> m_effects;
        private List<Character> m_characters; // collection of Character objects in this cell
        private List<Item> m_items; // collection of Item objects in this cell
        public List<Item> Items
        { get { return m_items; } }

        public bool hasItems = false;

        #region Public Properties
        public List<Character> Characters
        {
            get { return this.m_characters; }
        }

        public List<Effect> Effects
        {
            get { return this.m_effects; }
        }

        public bool IsLootVisible
        {
            get
            {
                switch (this.displayGraphic)
                {
                    case "~~": // water
                    case "**": // fire
                    case "[]": // wall
                    case "  ": // empty
                    case "??": // darkness
                        return false;
                    default:
                        return true;
                }
            }
        }
        #endregion

        #region Constructors (2)
        public Cell()
        {
            visible = true;
            land = 0;
            map = 0;
            xCord = 0;
            yCord = 0;
            cellGraphic = "  ";
            displayGraphic = "  ";
            lockers = false;
            portal = false;
            m_effects = new List<Effect>();
            m_characters = new List<Character>();
            m_items = new List<Item>();
        }

        public Cell(string info)
        {
            string[] cellInfo = info.Split(Protocol.ISPLIT.ToCharArray());

            try
            {
                this.visible = true;
                this.land = Convert.ToInt16(cellInfo[0]);
                this.map = Convert.ToInt16(cellInfo[1]);
                this.xCord = Convert.ToInt16(cellInfo[2]);
                this.yCord = Convert.ToInt16(cellInfo[3]);
                this.zCord = Convert.ToInt32(cellInfo[4]);
                this.cellGraphic = cellInfo[5];
                this.displayGraphic = cellInfo[6];
                this.lockers = Convert.ToBoolean(cellInfo[7]);
                this.portal = Convert.ToBoolean(cellInfo[8]);
                this.hasItems = Convert.ToBoolean(cellInfo[9]);
                m_effects = new List<Effect>();
                m_characters = new List<Character>();
                m_items = new List<Item>();
            }
            catch (Exception e)
            {
                Utils.LogException(e);
            }
        } 
        #endregion

        #region Add Methods
        public void Add(Character ch)
        {
            this.m_characters.Add(ch);
        }

        public void Add(Item item)
        {
            this.m_items.Add(item);
        }

        public void Add(Effect effect)
        {
            this.m_effects.Add(effect);
        } 
        #endregion

        public static void SendCellItemsRequest(Cell cell)
        {
            gui.GameHUD.ExaminedCell = cell;
            string cellCoords = cell.xCord + Protocol.VSPLIT + cell.yCord + Protocol.VSPLIT + cell.zCord;
            IO.Send(Protocol.REQUEST_CELLITEMS + " " + cellCoords);
        }
    }
}
