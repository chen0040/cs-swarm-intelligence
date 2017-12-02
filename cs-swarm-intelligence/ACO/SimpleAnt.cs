using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwarmIntelligence.ACO
{
    public class SimpleAnt : BaseSwarmEntity
    {
        protected List<int> mData = new List<int>();
        protected IAntColony mAntColony = null;
        protected object mAntGeneratorContext;

        public int CurrentState
        {
            get
            {
                if (mData.Count == 0)
                {
                    return -1;
                }
                return mData[mData.Count - 1];
            }
        }

        public List<Tuple<int, int>> FindTrasitionPath()
        {
            if (mData.Count == 0) return null;

            List<Tuple<int, int>> path = new List<Tuple<int, int>>();
            for(int i=0; i < mData.Count-1; ++i)
            {
                int state1_id = mData[i];
                int state2_id = mData[i + 1];
                path.Add(Tuple.Create(state1_id, state2_id));
            }

            return path;
        }

        public SimpleAnt(IAntColony colony, object info)
        {
            mAntColony = colony;
            mAntGeneratorContext = info;
        }

        public override void UpdateCost()
        {
            mCost = mAntColony.Evaluate(this);
        }

        public void Add(int state)
        {
            mData.Add(state);
        }

        public void Reset(int antIndex, int stateCount)
        {
            mData.Clear();
            int firstState = (antIndex) % stateCount;
            mData.Add(firstState);
        }

        public int Length
        {
            get { return mData.Count; }
        }
        

        public virtual bool HasTraversedState(int state_id)
        {
            return mData.Contains(state_id);
        }

        public int this[int index]
        {
            get
            {
                if (index < 0 || index >= mData.Count)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return mData[index];
            }
            set
            {
                if (index < 0 || index >= mData.Count)
                {
                    throw new ArgumentOutOfRangeException();
                }
                if(mData[index] != value)
                {
                    mData[index] = value;
                }
            }
        }

        public SimpleAnt Clone()
        {
            SimpleAnt clone = new SimpleAnt(mAntColony, mAntGeneratorContext);
            clone.Copy(this);
            return clone;
        }

        public virtual void Copy(SimpleAnt rhs)
        {
            mData.Clear();
            for (int i = 0; i < rhs.mData.Count; ++i)
            {
                mData.Add(rhs.mData[i]);
            }
            mCost = rhs.mCost;
        }
    }
}
