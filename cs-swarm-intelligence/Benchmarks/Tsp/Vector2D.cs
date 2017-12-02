using System;


/**
 * Created by xschen on 14/6/2017.
 */
 namespace SwarmIntelligence.Benchmarks.Tsp
{
    public class Vector2D
    {
        public double x;
        public double y;

        public Vector2D(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double distanceSq(Vector2D that)
        {
            double dx = x - that.x;
            double dy = y - that.y;
            return dx * dx + dy * dy;
        }

        public double distance(Vector2D that)
        {
            return Math.Sqrt(distanceSq(that));
        }


        public override bool Equals(object o)
        {
            if (this == o)
                return true;
            if (o == null)
                return false;

            Vector2D vector2D = (Vector2D)o;

            if (vector2D.x != x)
                return false;
            return vector2D.y == y;
        }

        public override int GetHashCode()
        {
            int result;
            int temp;
            temp = x.GetHashCode();
            result = temp;
            temp = y.GetHashCode();
            result = 31 * result + temp;
            return result;
        }


        public override string ToString()
        {
            return "(" + "x=" + x + ", y=" + y + ')';
        }
    }
}

