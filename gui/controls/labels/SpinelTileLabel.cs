using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Yuusha.gui
{
    public class SpinelTileLabel : IOKTileLabel
    {
        #region Private Data
        private VisualKey m_foreVisual;
        private byte m_foreAlpha;
        private Color m_foreTint;
        private VisualKey m_lootVisual;
        private List<VisualKey> m_critterVisuals; 
        #endregion

        #region Public Properties
        public string ForeVisual
        {
            get { return m_foreVisual.Key; }
            set { m_foreVisual.Key = value; }
        }
        public byte ForeAlpha
        {
            get { return m_foreAlpha; }
            set { m_foreAlpha = value; }
        }
        public Color ForeColor
        {
            get { return m_foreTint; }
            set { m_foreTint = value; }
        }
        public string LootVisual
        {
            get { return m_lootVisual.Key; }
            set { m_lootVisual.Key = value; }
        }
        public List<VisualKey> CritterVisuals
        {
            get { return m_critterVisuals; }
        } 
        #endregion

        #region Constructor
        public SpinelTileLabel(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
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
            m_textAlignment = textAlignment;
            m_xTextOffset = xTextOffset;
            m_yTextOffset = yTextOffset;
            m_onDoubleClickEvent = onDoubleClickEvent;
            m_cursorOverride = cursorOverride;
            m_anchors = anchors;
            m_popUpText = popUpText;

            m_foreVisual = new VisualKey("");
            m_foreAlpha = 255;
            m_foreTint = Color.White;
            m_lootVisual = new VisualKey("");
            m_critterVisuals = new List<VisualKey>();

            
            LootText = "";
            CreatureText = "";
        } 
        #endregion

        public override void Update(GameTime gameTime)
        {
            m_textRectangle = m_rectangle; // TODO:

            if (m_popUpText != "" && m_controlState == Enums.EControlState.Over)
                TextCue.AddMouseCursorTextCue(m_popUpText, Utils.GetColor(Client.ClientSettings.DefaultPopUpColor), m_font);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!m_visible)
                return;

            base.Draw(gameTime);
            
            if (m_foreVisual != null && m_foreVisual.Key != "")
            {
                if (!GuiManager.Visuals.ContainsKey(m_foreVisual.Key))
                {
                    Utils.LogOnce("Failed to find visual key [ " + m_foreVisual + " ] for Control [ " + m_name + " ]");
                    m_foreVisual.Key = ""; // clear visual key
                    return;
                }

                VisualInfo vi = GuiManager.Visuals[m_foreVisual.Key];

                if (!m_disabled)
                {
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_rectangle, vi.Rectangle, m_foreTint);
                }
                else
                {
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_rectangle, vi.Rectangle, new Color(s_disabledColor.R, s_disabledColor.G, s_disabledColor.B, m_visualAlpha));
                }
            }

            if (m_lootVisual != null && m_lootVisual.Key != "")
            {
                if (!GuiManager.Visuals.ContainsKey(m_lootVisual.Key))
                {
                    Utils.LogOnce("Failed to find visual key [ " + m_lootVisual + " ] for Control [ " + m_name + " ]");
                    m_lootVisual.Key = ""; // clear visual key
                    return;
                }

                VisualInfo vi = GuiManager.Visuals[m_lootVisual.Key];

                if (!m_disabled)
                {
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_rectangle, vi.Rectangle, Color.White);
                }
                else
                {
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_rectangle, vi.Rectangle, new Color(s_disabledColor.R, s_disabledColor.G, s_disabledColor.B, m_visualAlpha));
                }
            }

            if (m_critterVisuals.Count > 0)
            {
                for(int a = 0; a < m_critterVisuals.Count; a++)
                {
                    VisualKey vk = m_critterVisuals[a];
                    if (!GuiManager.Visuals.ContainsKey(vk.Key))
                    {
                        Utils.LogOnce("Failed to find visual key [ " + vk + " ] for Control [ " + m_name + " ]");
                        vk.Key = "question_mark"; // draw a question mark
                    }

                    VisualInfo vi = GuiManager.Visuals[vk.Key];
                    Rectangle sourceRect = new Rectangle(vi.X, vi.Y, vi.Width, vi.Height);
                    
                    if (!m_disabled)
                    {
                        Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_rectangle, sourceRect, Color.White);
                    }
                    else
                    {
                        Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_rectangle, sourceRect, new Color(s_disabledColor.R, s_disabledColor.G, s_disabledColor.B, m_visualAlpha));
                    }
                }
            }
            if (BitmapFont.ActiveFonts.ContainsKey(Font))
            {
                // override BitmapFont sprite batch
                BitmapFont.ActiveFonts[Font].SpriteBatchOverride(Client.SpriteBatch);
                // set font alignment
                BitmapFont.ActiveFonts[Font].Alignment = m_textAlignment;
                // draw string in textbox
                Rectangle rect = new Rectangle(m_textRectangle.X + m_xTextOffset, m_textRectangle.Y + m_yTextOffset, m_textRectangle.Width, m_textRectangle.Height);

                if (CreatureText != null && CreatureText != "")
                {
                    BitmapFont.ActiveFonts[Font].TextBox(rect, Client.UserSettings.Color_Gui_CreatureLetter_Fore, CreatureText);
                }
            }
            else Utils.LogOnce("BitmapFont.ActiveFonts does not contain the Font [ " + Font + " ] for SpinelTileLabel [ " + m_name + " ] of Sheet [ " + GuiManager.CurrentSheet.Name + " ]");
        }
    }
}
