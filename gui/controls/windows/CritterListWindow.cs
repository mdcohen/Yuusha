using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Yuusha.gui
{
    public class CritterListWindow : Window
    {
        

        public CritterListWindow(string name, string owner, Rectangle rectangle, bool visible, bool locked, bool disabled, string font,
            VisualKey visualKey, Color tintColor, byte visualAlpha, bool dropShadow, Map.Direction shadowDirection, int shadowDistance,
            List<Enums.EAnchorType> anchors, string cursorOverride) : base(name, owner, rectangle, visible, locked, disabled, font, visualKey, tintColor, visualAlpha, dropShadow, shadowDirection, shadowDistance, anchors, cursorOverride)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            int height = 20;
            int count = 0;
            int spacing = 1;
            if (Client.GameDisplayMode != Enums.EGameDisplayMode.Yuusha)
                spacing = 0;

            foreach(Control c in Controls)
            {
                if(c is CritterListLabel && c.IsVisible)
                {
                    height = c.Height;
                    if ((c as CritterListLabel).HealthBar != null) height += (c as CritterListLabel).HealthBar.Height;
                    count++;
                }
            }

            int titleHeight = WindowTitle != null ? WindowTitle.Height : 0;

            Height = titleHeight + ((height + spacing) * count);
        }
    }
}
