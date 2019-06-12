namespace Yuusha
{
    public class XYCoordinate
    {
        public int x;
        public int y;

        public XYCoordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return "{" + this.x.ToString() + "," + this.y.ToString() + "}";
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
            if ((object)lhs == null && (object)rhs == null)
                return true;

            if ((object)lhs == null || (object)rhs == null)
                return false;

            if (lhs.x == rhs.x && lhs.y == rhs.y)
                return true;
            else
                return false;
        }

        public static bool operator !=(XYCoordinate lhs, XYCoordinate rhs)
        {
            if ((object)lhs == null && (object)rhs == null)
                return false;

            if ((object)lhs == null || (object)rhs == null)
                return true;

            if (lhs.x == rhs.x && lhs.y == rhs.y)
                return false;
            else
                return true;
        }

        public static XYCoordinate operator -(XYCoordinate lhs, XYCoordinate rhs)
        {
            return new XYCoordinate(lhs.x - rhs.x, lhs.y - rhs.y);
        }
    }
}