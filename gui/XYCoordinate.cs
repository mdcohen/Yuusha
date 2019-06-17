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

        public static bool operator ==(XYCoordinate lhs, XYCoordinate rhs)
        {
            if (lhs is null && rhs == null)
                return true;

            if (lhs == null || rhs == null)
                return false;

            if (lhs.X == rhs.X && lhs.Y == rhs.Y)
                return true;
            else
                return false;
        }

        public static bool operator !=(XYCoordinate lhs, XYCoordinate rhs)
        {
            if (lhs == null && rhs == null)
                return false;

            if (lhs == null || rhs == null)
                return true;

            if (lhs.X == rhs.X && lhs.Y == rhs.Y)
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