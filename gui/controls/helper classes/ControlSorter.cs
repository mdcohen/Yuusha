using System.Collections.Generic;

namespace Yuusha.gui
{
    /// <summary>Sorts Controls by Z Depth</summary>
    public class ControlSorter : IComparer<Control>
    {
        /// <summary>IComparer implementation</summary>
        /// <param name="x">Control 1</param>
        /// <param name="y">Control 2</param>
        public int Compare(Control x, Control y)
        {
            if (x.ZDepth < y.ZDepth)
            {
                return 1;
            }

            if (x.ZDepth == y.ZDepth)
            {
                // DropDownMenus always go on top.
                if (x is DropDownMenu && !(y is DropDownMenu))
                    return 1;
                else if (y is DropDownMenu && !(x is DropDownMenu))
                    return -1;

                // the currently dragged control gets highest z order
                if ((x == GuiManager.DraggedControl) && !(y == GuiManager.DraggedControl))
                {
                    return 1;
                }
                else if (!(x == GuiManager.DraggedControl) && (y == GuiManager.DraggedControl))
                {
                    return -1;
                }

                // window goes underneath its controls
                if ((x is Window) && !(y is Window))
                {
                    return -1;
                }
                else if ((y is Window) && !(x is Window))
                {
                    return 1;
                }
                else
                {
                    if (x.ZDepthDateTime < y.ZDepthDateTime)
                        return -1;
                    if (x.ZDepthDateTime > y.ZDepthDateTime)
                        return 1;
                }

                // border goes above all other controls
                if ((x is Border) && !(y is Border))
                {
                    return 1;
                }
                else if ((y is Border) && !(x is Border))
                {
                    return -1;
                }

                // window title goes above all other window controls
                if ((x is WindowTitle) && !(y is WindowTitle))
                {
                    return 1;
                }
                else if ((y is WindowTitle) && !(x is WindowTitle))
                {
                    return -1;
                }

                // for controls on same window, sort bottom to top
                if (x.Position.Y < y.Position.Y)
                {
                    return 1;
                }
                if (x.Position.Y > y.Position.Y)
                {
                    return -1;
                }

                if (x.ZDepthDateTime < y.ZDepthDateTime)
                    return -1;
                if (x.ZDepthDateTime > y.ZDepthDateTime)
                    return 1;

                return 0;
            }
            return -1;
        }
    }
}
