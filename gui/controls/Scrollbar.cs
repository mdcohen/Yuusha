using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Yuusha.gui
{
    public class Scrollbar : Control
    {
        #region Private Data
        private int m_scrollValue;
        private int m_upperAdjust;
        private int m_lowerAdjust;
        private int m_maxValue;
        private int m_minValue;
        private ScrollbarControlBox m_upperBox;
        private ScrollbarControlBox m_lowerBox;
        private ScrollbarControlBox m_thumbBox;
        #endregion

        #region Public Properties
        public int ScrollValue
        {
            get { return m_scrollValue; }
            set { m_scrollValue = value; }
        }
        public int MaxValue
        {
            get { return m_maxValue; }
            set { m_maxValue = value; }
        }
        public int MinValue
        {
            get { return m_minValue; }
            set { m_minValue = value; }
        }
        #endregion

        public Scrollbar()
            : base()
        {
            m_scrollValue = 0;
            m_upperAdjust = 1;
            m_lowerAdjust = 1;
            m_maxValue = 100;
            m_minValue = 0;
            m_visualKey = null;
        }

        public Scrollbar(string name, int scrollValue, int upperAdjust, int lowerAdjust, int maxValue,
            int minValue, bool disabled)
            : base()
        {
            m_name = name;
            m_scrollValue = scrollValue;
            m_upperAdjust = upperAdjust;
            m_lowerAdjust = lowerAdjust;
            m_maxValue = maxValue;
            m_minValue = minValue;
            m_disabled = disabled;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (m_scrollValue > m_maxValue)
                m_scrollValue = m_maxValue;
            if (m_scrollValue < m_minValue)
                m_scrollValue = m_minValue;

            base.Update(gameTime);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (!m_visible)
                return;

            base.Draw(gameTime);

            //Rectangle trackRectangle = new Rectangle(m_rectangle.X, m_rectangle.Y, m_rectangle.Width, m_rectangle.Height);

        }

        public void Reset()
        {
            m_scrollValue = 0;
            m_upperAdjust = 1;
            m_lowerAdjust = 1;
            m_maxValue = 100;
            m_minValue = 0;
        }
    }
}
