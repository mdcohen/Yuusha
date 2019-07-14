using System;

namespace Yuusha
{
    [Serializable]
    public class XYCoordinate
    {
        public int X;
        public int Y;

        public XYCoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return "{" + this.X.ToString() + "," + this.Y.ToString() + "}";
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is XYCoordinate)) return false;

            return this == (XYCoordinate)obj;
        }

        public static bool operator ==(XYCoordinate xy1, XYCoordinate xy2)
        {
            if (xy1 is null && !(xy2 is null))
                return false;

            if (!(xy1 is null) && xy2 is null)
                return false;

            if (xy1 is null && xy2 is null)
                return false;

            if (xy1.X == xy2.X && xy1.Y == xy2.Y)
                return true;
            else
                return false;
        }

        public static bool operator !=(XYCoordinate xy1, XYCoordinate xy2)
        {
            if (xy1 is null && !(xy2 is null))
                return true;

            if (!(xy1 is null) && xy2 is null)
                return true;

            if (xy1 is null && xy2 is null)
                return false;

            if (xy1.X == xy2.X && xy1.Y == xy2.Y)
                return false;
            else
                return true;
        }

        public static XYCoordinate operator -(XYCoordinate lhs, XYCoordinate rhs)
        {
            return new XYCoordinate(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }
    }
}