using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Yuusha.gui
{
    public class SpinelTileLabel : IOKTileLabel
    {
        #region Private Data
        private VisualKey m_foreVisual;
        private byte m_foreAlpha;
        private Color m_foreTint;
        private VisualKey m_lootVisual;
        private byte m_lootVisualAlpha = 255;
        private List<VisualKey> m_critterVisuals;
        private List<string> m_effectNames;
        private VisualKey m_pathingVisual; // footsteps and ?
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
        public byte LootVisualAlpha
        { set { m_lootVisualAlpha = value; } }
        public string FogVisual // FogOfWar
        { get; set; }
        public List<string> EffectNames
        {
            get { return m_effectNames; }
        }
        public List<VisualKey> CritterVisuals
        {
            get { return m_critterVisuals; }
        }
        public FogOfWarWindow.FogOfWarDetail FogOfWarDetail
        { get; set; }
        public string PathingVisual
        {
            get { return m_pathingVisual.Key; }
            set { m_pathingVisual.Key = value; }
        }
        public List<Tuple<string, AnimatedVisual>> EffectAnimatedVisuals
        { get; set; }
        public List<AnimatedVisual> GraphicAnimatedVisuals
        { get; set; }
        #endregion

        #region Constructor
        public SpinelTileLabel(string name, string owner, Rectangle rectangle, string text, Color textColor, bool visible,
            bool disabled, string font, VisualKey visualKey, Color tintColor, byte visualAlpha, byte textAlpha,
            BitmapFont.TextAlignment textAlignment, int xTextOffset, int yTextOffset, string onDoubleClickEvent,
            string cursorOverride, List<Enums.EAnchorType> anchors, string popUpText)
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
            m_textAlpha = textAlpha;
            TextAlignment = textAlignment;
            XTextOffset = xTextOffset;
            YTextOffset = yTextOffset;
            m_onDoubleClickEvent = onDoubleClickEvent;
            m_cursorOverride = cursorOverride;
            m_anchors = anchors;
            PopUpText = popUpText;

            m_foreVisual = new VisualKey("");
            m_foreAlpha = 255;
            m_foreTint = Color.White;
            m_lootVisual = new VisualKey("");
            m_pathingVisual = new VisualKey("");
            m_critterVisuals = new List<VisualKey>();
            m_effectNames = new List<string>();
            
            LootText = "";
            CreatureText = "";

            FogOfWarDetail = new FogOfWarWindow.FogOfWarDetail(0, 0, 0, 0, "");

            EffectAnimatedVisuals = new List<Tuple<string, AnimatedVisual>>();
            GraphicAnimatedVisuals = new List<AnimatedVisual>();
        } 
        #endregion

        public override void Update(GameTime gameTime)
        {
            m_textRectangle = m_rectangle; // TODO:

            foreach (AnimatedVisual animation in GraphicAnimatedVisuals)
            {
                animation.SetPosition(Position);
                animation.Update(gameTime);
            }

            foreach (Tuple<string, AnimatedVisual> animation in EffectAnimatedVisuals)
            {
                animation.Item2.SetPosition(Position);
                animation.Item2.Update(gameTime);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!m_visible || IsOffscreen())
                return;

            base.Draw(gameTime);

            if (m_foreVisual != null && m_foreVisual.Key != "")
            {
                if (!GuiManager.Visuals.ContainsKey(m_foreVisual.Key))
                {
                    Utils.LogOnce("Failed to find SpinelTileLabel foreVisual key [ " + m_foreVisual + " ] for Control [ " + m_name + " ]");
                    m_foreVisual.Key = ""; // clear visual key
                    return;
                }

                VisualInfo vi = GuiManager.Visuals[m_foreVisual.Key];

                if (!m_disabled)
                {
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_rectangle, vi.Rectangle, new Color(m_foreTint, m_foreAlpha));
                }
                else
                {
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_rectangle, vi.Rectangle, new Color(ColorDisabledStandard.R, ColorDisabledStandard.G, ColorDisabledStandard.B, m_visualAlpha));
                }
            }

            if (FogVisual != null && FogVisual != "")
            {
                if (!GuiManager.Visuals.ContainsKey(FogVisual))
                {
                    Utils.LogOnce("Failed to find SpinelTileLabel fogVisual key [ " + FogVisual + " ] for Control [ " + m_name + " ]");
                    FogVisual = ""; // clear visual key
                    return;
                }

                VisualInfo vi = GuiManager.Visuals[FogVisual];
                Color fogColor = new Color(FogOfWarWindow.FogColor, FogOfWarWindow.FogAlpha);
                Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_rectangle, vi.Rectangle, fogColor);
            }

            // Draw loot.
            if (m_lootVisual != null && m_lootVisual.Key != "")
            {
                if (!GuiManager.Visuals.ContainsKey(m_lootVisual.Key))
                {
                    Utils.LogOnce("Failed to find SpinelTileLabel lootVisual key [ " + m_lootVisual + " ] for Control [ " + m_name + " ]");
                    m_lootVisual.Key = ""; // clear visual key
                    return;
                }

                VisualInfo vi = GuiManager.Visuals[m_lootVisual.Key];

                if (!m_disabled)
                {
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_rectangle, vi.Rectangle, new Color(Color.White, m_lootVisualAlpha));
                }
                else
                {
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_rectangle, vi.Rectangle, new Color(ColorDisabledStandard.R, ColorDisabledStandard.G, ColorDisabledStandard.B, m_visualAlpha));
                }
            }

            foreach (AnimatedVisual animation in GraphicAnimatedVisuals)
                animation.Draw(gameTime);

            // Draw effects.
            if (m_effectNames.Count > 0)
            {
                for (int a = 0; a < m_effectNames.Count; a++)
                {
                    VisualKey vk = new VisualKey("unknown");
                    Color tintColor = new Color(Color.White, 200);

                    if (Effect.CellEffectsDictionary.ContainsKey(m_effectNames[a]) && !Effect.CellEffectsDictionary[m_effectNames[a]].Item4)
                    {
                        if (GuiManager.Visuals.ContainsKey(Effect.CellEffectsDictionary[m_effectNames[a]].Item1))
                            vk = new VisualKey(Effect.CellEffectsDictionary[m_effectNames[a]].Item1);

                        tintColor = new Color(Effect.CellEffectsDictionary[m_effectNames[a]].Item2, Effect.CellEffectsDictionary[m_effectNames[a]].Item3);
                    }
                    else Utils.LogOnce("CellEffectsDictionary does not contain a key for effect: " + m_effectNames[a]);

                    VisualInfo vi = GuiManager.Visuals[vk.Key];
                    Rectangle sourceRect = new Rectangle(vi.X, vi.Y, vi.Width, vi.Height);

                    if (!m_disabled)
                    {
                        Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_rectangle, sourceRect, tintColor);
                    }
                    else
                    {
                        Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_rectangle, sourceRect, new Color(ColorDisabledStandard.R, ColorDisabledStandard.G, ColorDisabledStandard.B, tintColor.A));
                    }
                }
            }

            foreach (Tuple<string, AnimatedVisual> animation in EffectAnimatedVisuals)
                animation.Item2.Draw(gameTime);

            // Draw critters.
            if (m_critterVisuals.Count > 0)
            {
                for(int a = 0; a < m_critterVisuals.Count; a++)
                {
                    VisualKey vk = m_critterVisuals[a];
                    if (!GuiManager.Visuals.ContainsKey(vk.Key))
                    {
                        string vkString = vk.ToString();
                        if (vkString.Length > 0 && char.IsDigit(vkString[0]) && vkString.Contains(" ") && vkString.EndsWith("s"))
                        {
                            string newVisual = vkString.Substring(vkString.IndexOf(" ") + 1, vkString.Length - (vkString.IndexOf(" ") + 2));
                            if (GuiManager.Visuals.ContainsKey(newVisual))
                                vk = new VisualKey(newVisual);
                            else if (GuiManager.Visuals.ContainsKey(TextManager.ConvertPluralToSingular(newVisual)))
                                vk = new VisualKey(TextManager.ConvertPluralToSingular(newVisual));
                            else
                            {
                                Utils.LogOnce("Failed to find Critter visual key [ " + vk + " ] for SpinelTileLabel after visual key string filtering.");
                                vk.Key = "question_mark";
                            }
                        }
                        else
                        {
                            if (GuiManager.Visuals.ContainsKey(vkString.Replace("_female", "")))
                            {
                                Utils.LogOnce("Failed to find Critter visual key [ " + vk + " ] for SpinelTileLabel, found when removing '_female' from visual key.");
                                vk.Key = vkString.Replace("_female", "");
                            }
                            else
                            {
                                Utils.LogOnce("Failed to find Critter visual key [ " + vk + " ] for SpinelTileLabel.");
                                vk.Key = "question_mark"; // draw a question mark
                            }
                        }
                    }

                    VisualInfo vi = GuiManager.Visuals[vk.Key];
                    Rectangle sourceRect = new Rectangle(vi.X, vi.Y, vi.Width, vi.Height);
                    Color tintColor = vk.OverrideTintColor;
                    
                    if (!m_disabled)
                    {
                        Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_rectangle, sourceRect, tintColor);
                    }
                    else
                    {
                        Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_rectangle, sourceRect, new Color(ColorDisabledStandard.R, ColorDisabledStandard.G, ColorDisabledStandard.B, m_visualAlpha));
                    }
                }
            }

            // Draw pathing such as movement footsteps or spell casting direction.
            if (m_pathingVisual != null && !string.IsNullOrEmpty(m_pathingVisual.Key))
            {
                if (!GuiManager.Visuals.ContainsKey(m_pathingVisual.Key))
                {
                    Utils.LogOnce("Failed to find SpinelTileLabel pathingVisual key [ " + m_pathingVisual + " ] for Control [ " + m_name + " ]");
                    m_pathingVisual.Key = ""; // clear visual key
                    return;
                }

                VisualInfo vi = GuiManager.Visuals[m_pathingVisual.Key];

                if (!m_disabled)
                {
                    int visualAlpha = 255;

                    if (Character.HasEffect("Hide in Shadows"))
                        visualAlpha = 80;

                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_rectangle, vi.Rectangle, new Color(Client.ClientSettings.MovementFootstepsColor, visualAlpha));
                }
                else
                {
                    Client.SpriteBatch.Draw(GuiManager.Textures[vi.ParentTexture], m_rectangle, vi.Rectangle, new Color(ColorDisabledStandard.R, ColorDisabledStandard.G, ColorDisabledStandard.B, m_visualAlpha));
                }
            }

            // Draw border.
            if (Border != null) Border.Draw(gameTime); // draws the border again

            // Draw text (critter letters).
            if (BitmapFont.ActiveFonts.ContainsKey(Font))
            {
                // override BitmapFont sprite batch
                BitmapFont.ActiveFonts[Font].SpriteBatchOverride(Client.SpriteBatch);
                // set font alignment
                BitmapFont.ActiveFonts[Font].Alignment = TextAlignment;
                // draw string in textbox
                Rectangle rect = new Rectangle(m_textRectangle.X + XTextOffset, m_textRectangle.Y + YTextOffset, m_textRectangle.Width, m_textRectangle.Height);

                if (CreatureText != null && CreatureText != "")
                {
                    BitmapFont.ActiveFonts[Font].TextBox(rect, Client.ClientSettings.Color_Gui_CreatureLetter_Fore, CreatureText);
                }
            }
            else Utils.LogOnce("BitmapFont.ActiveFonts does not contain the Font [ " + Font + " ] for SpinelTileLabel of Sheet [ " + GuiManager.CurrentSheet.Name + " ]");
        }

        protected override void OnMouseOver(MouseState ms)
        {
            base.OnMouseOver(ms);

            if (Owner == "MapDisplayWindow")
            {
                var border = new SquareBorder(Name + "Border", Name, 1, new VisualKey("WhiteSpace"), false, Color.White, 255);
                Border = border;
                ZDepth = 1;

                (GuiManager.GetControl("MapDisplayWindow") as Window).Controls.ForEach(c => { if (c is SpinelTileLabel && c.Name != Name) { (c as SpinelTileLabel).Border = null; } });
            }
        }

        protected override void OnMouseDown(MouseState ms)
        {
            if (!string.IsNullOrEmpty(Owner) && Owner == "MapDisplayWindow")
            {
                if (!m_onMouseDownSent && ms.LeftButton == ButtonState.Pressed)
                {
                    foreach (Cell cell in GameHUD.MovementChoices)
                    {
                        if(GuiManager.CurrentSheet["Tile" + GameHUD.Cells.IndexOf(cell)] is SpinelTileLabel spLabel) spLabel.PathingVisual = "";
                    }

                    GameHUD.MovementChoices.Clear();

                    string movement = GetMovementDirections();

                    Cell currentCell = Character.CurrentCharacter.Cell;
                    foreach (Cell cell in GameHUD.MovementChoices)
                    {
                        if (GuiManager.CurrentSheet["Tile" + GameHUD.Cells.IndexOf(cell)] is SpinelTileLabel spLabel &&
                            Map.GetDirection(currentCell, cell) is Map.Direction direction && direction != Map.Direction.None)
                        {
                            spLabel.PathingVisual = direction.ToString() + "Footsteps";
                        }

                        currentCell = cell;
                    }
                }
            }
                
            base.OnMouseDown(ms);
        }

        protected override void OnZDelta(MouseState ms)
        {
            // Currently only the MapDisplayWindow in YuushaMode may be adjusted.
            if (Owner != "MapDisplayWindow" || Client.GameState != Enums.EGameState.YuushaGame)
                return;

            if(ms.ScrollWheelValue > GuiManager.CurrentSheet.PreviousScrollWheelValue)
            {
                Events.RegisterEvent(Events.EventName.MapDisplay_Increase);
            }
            else if(ms.ScrollWheelValue < GuiManager.CurrentSheet.PreviousScrollWheelValue)
            {
                Events.RegisterEvent(Events.EventName.MapDisplay_Decrease);
            }
        }
    }
}
