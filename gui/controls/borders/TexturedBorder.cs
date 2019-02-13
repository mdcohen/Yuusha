using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Yuusha.gui
{
    public class TexturedBorder : Border
    {
        private Dictionary<Enums.EBorderLocation, VisualInfo> m_borderVisuals;

        public TexturedBorder()
        {
            m_borderVisuals = new Dictionary<Enums.EBorderLocation, VisualInfo>();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
