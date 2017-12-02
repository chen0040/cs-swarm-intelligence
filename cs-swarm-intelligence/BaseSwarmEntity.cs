using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwarmIntelligence
{
    public abstract class BaseSwarmEntity : ISwarmEntity, IComparable
    {
        protected double mCost = double.MaxValue;

        public int CompareTo(object rhs) //when used in Sort, array[0] will be best and array[last] will be worst
        {
            BaseSwarmEntity rhs_cast = rhs as BaseSwarmEntity;
            if (this.IsBetterThan(rhs_cast)) return -1;
            else if (rhs_cast.IsBetterThan(this)) return 1;
            else return 0;
        }


        public virtual bool IsBetterThan(ISwarmEntity rhs)
        {
            return Cost < rhs.Cost;
        }

        public virtual double Cost
        {
            get { return mCost; }
        }

        public abstract void UpdateCost();
    }
}
