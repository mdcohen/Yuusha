using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuusha.gui
{
    public class TabControl : Control
    {
        private List<TabControlButton> TabButtonsList;

        public TabControl(string name, string owner) : base()
        {
            m_name = name;
            m_owner = owner;

            TabButtonsList = new List<TabControlButton>();
        }

        public void Add(TabControlButton button)
        {
            if (!TabButtonsList.Contains(button))
                TabButtonsList.Add(button);
            else
            {
                //log it and return;
                return;
            }

            button.TabControl = this;
        }

        public void ConfirmOneTabWindowVisible(TabControlButton button)
        {
            try
            {
                Window w = GuiManager.GetControl(button.TabControlledWindow) as Window;

                foreach (TabControlButton tabControlButton in TabButtonsList)
                {
                    if (tabControlButton.TabControlledWindow != w.Name)
                    {
                        GuiManager.GetControl(tabControlButton.TabControlledWindow).IsVisible = false;
                    }
                }

                w.IsVisible = true;
            }
            catch(Exception e)
            {
                Utils.LogException(e);
            }
        }
    }
}
