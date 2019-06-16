using Microsoft.Xna.Framework;

namespace Yuusha.gui
{
    public class IOKTileLabel : MapTileLabel
    {
        private string m_lootText;
        private string m_creatureText;

        #region Public Properties
        public string LootText
        {
            get { return m_lootText; }
            set { m_lootText = value; }
        }
        public string CreatureText
        {
            get { return m_creatureText; }
            set { m_creatureText = value; }
        }
        #endregion

        public IOKTileLabel()
            : base()
        {
        }

        public IOKTileLabel(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte borderAlpha, byte textAlpha,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string onDoubleClickEvent,
            string cursorOverride, System.Collections.Generic.List<Enums.EAnchorType> anchors, string popUpText)
            : base()
        {
            m_name = name;
            m_owner = owner;
            m_rectangle = rectangle;
            m_textRectangle = rectangle; // TODO:
            m_text = text;
            m_textColor = textColor;
            m_visible = visible;
            m_disabled = disabled;
            m_font = font;
            m_visualKey = visualKey;
            m_tintColor = tintColor;
            m_visualAlpha = visualAlpha;
            m_borderAlpha = borderAlpha;
            m_textAlpha = textAlpha;
            TextAlignment = textAlignment;
            XTextOffset = xTextOffset;
            YTextOffset = yTextOffset;
            m_onDoubleClickEvent = onDoubleClickEvent;
            m_cursorOverride = cursorOverride;
            m_anchors = anchors;
            m_popUpText = popUpText;

            m_lootText = "";
            m_creatureText = "";
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // draw string in textbox
            Rectangle rect = new Rectangle(m_textRectangle.X + XTextOffset, m_textRectangle.Y + YTextOffset, m_textRectangle.Width, m_textRectangle.Height);

            if (m_lootText != null && m_lootText != "")
            {
                BitmapFont.ActiveFonts[Font].TextBox(rect, Client.ClientSettings.Color_Gui_Loot_Fore, m_lootText);
            }

            // SpinelTileLabel will draw it's own creature text
            if (m_creatureText != null && m_creatureText != "" && !(this is SpinelTileLabel))
            {
                BitmapFont.ActiveFonts[Font].TextBox(rect, Client.ClientSettings.Color_Gui_CreatureLetter_Fore, m_creatureText);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
