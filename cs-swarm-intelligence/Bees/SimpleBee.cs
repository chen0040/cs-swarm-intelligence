using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwarmIntelligence.Bees
{
    public abstract class SimpleBee : BaseSwarmEntity
    {
        protected IBeeSwarm mSwarm;

        public SimpleBee(IBeeSwarm swarm)
        {
            mSwarm = swarm;
        }

        public IBeeSwarm Swarm
        {
            get
            {
                return mSwarm; 
            }
        }

        public abstract void Dance(SimpleBee rhs, double[] lower_bounds, double[] upper_bounds, object constraints); //local search around rhs
        public void Initialize(double[] lower_bounds, double[] upper_bounds, object constraints) //initialize random solution
        {
            RandomSearch(lower_bounds, upper_bounds, constraints);
        }
        public abstract void RandomSearch(double[] lower_bounds, double[] upper_bounds, object constraints); //initialize random solution
        public abstract SimpleBee Clone(); //factory method

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
