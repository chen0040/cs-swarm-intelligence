using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SwarmIntelligence.Bees;

namespace SwarmIntelligence.Bees
{
    public class SimpleBee : BaseSwarmEntity
    {
        protected IBeeSwarm mSwarm;
        private double[] mData;
        private double[] mLowerBounds;
        private double[] mUpperBounds;

        public SimpleBee(IBeeSwarm swarm, int dimension, double[] lowerBounds, double[] upperBounds)
        {
            mSwarm = swarm;
            mData = new double[dimension];
            mLowerBounds = lowerBounds;
            mUpperBounds = upperBounds;
        }

        public IBeeSwarm Swarm
        {
            get
            {
                return mSwarm; 
            }
        }

        public virtual void Dance(SimpleBee rhs, double[] sides, object constraints) //local search around rhs
        {
            double[] localLowerBounds = new double[rhs.Dimension];
            double[] localUpperBounds = new double[rhs.Dimension];

            for(int i=0; i < rhs.Dimension; ++i)
            {
                localLowerBounds[i] = rhs[i] - sides[i];
                localUpperBounds[i] = rhs[i] + sides[i];
                localUpperBounds[i] = Math.Min(mUpperBounds[i], localUpperBounds[i]);
                localLowerBounds[i] = Math.Max(mLowerBounds[i], localLowerBounds[i]);
            }
            RandomSearch(localLowerBounds, localUpperBounds, constraints);
            UpdateCost();
        }

        public void Initialize(object constraints) //initialize random solution
        {
            RandomSearch(mLowerBounds, mUpperBounds, constraints);
            UpdateCost();
        }

        public int Dimension
        {
            get { return mData.Length; }
        }

        public virtual void RandomSearch(double[] lower_bounds, double[] upper_bounds, object constraints) //initialize random solution
        {
            double lower_bound;
            double upper_bound;
            double range;
            for (int d = 0; d < mData.Length; ++d)
            {
                lower_bound = lower_bounds[d];
                upper_bound = upper_bounds[d];
                range = upper_bound - lower_bound;
                mData[d] = lower_bound + range * RandomEngine.NextDouble();
            }
        }
        public virtual SimpleBee Clone() //factory method 
        {
            SimpleBee bee = new SimpleBee(mSwarm, mData.Length, mLowerBounds, mUpperBounds);
            for(int i=0; i < mData.Length; ++i)
            {
                bee[i] = mData[i];
            }
            bee.mCost = mCost;
            bee.mIsCostValid = mIsCostValid;
            return bee;
        }

        public double this[int index]
        {
            get { return mData[index]; }
            set { mData[index] = value; }
        }

        public override void UpdateCost()
        {
            if (mIsCostValid == false)
            {
                mCost = mSwarm.Evaluate(this);
                mIsCostValid = true;
            }
        }
    }
}
