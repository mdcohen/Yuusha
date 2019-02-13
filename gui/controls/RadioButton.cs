using Microsoft.Xna.Framework;

namespace Yuusha.gui
{
    public class RadioButton : Control
    {
        private int m_groupID;
        private bool m_deselectOthers;
        private bool m_checked;
        private string m_onChecked;
        private string m_onUnchecked;
        private bool m_onCheckedSent;

        #region Public Properties
        public virtual int GroupID
        {
            get { return m_groupID; }
        }
        public virtual bool NeedToDeselectOthers
        {
            get { return m_deselectOthers; }
            set { m_deselectOthers = value; }
        } 
        #endregion

        public RadioButton()
        {
            m_onCheckedSent = false;
        }

        public void Deselect()
        {
            m_checked = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void OnMouseDown(Microsoft.Xna.Framework.Input.MouseState ms)
        {
            if (m_disabled)
                return;

            m_checked = true;

            if (!m_onCheckedSent)
            {
                Events.RegisterEvent((Events.EventName)System.Enum.Parse(typeof(Events.EventName), m_onChecked, true));
                m_onCheckedSent = true;
            }
        }

        protected override void OnMouseRelease(Microsoft.Xna.Framework.Input.MouseState ms)
        {
            m_onCheckedSent = false;
        }
    }
}
