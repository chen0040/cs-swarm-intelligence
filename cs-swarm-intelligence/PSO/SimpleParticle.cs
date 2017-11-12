using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwarmIntelligence.PSO
{
    public enum ParticleVectorType
    {
        Position,
        Velocity
    }

    public class SimpleParticle : BaseSwarmEntity
    {
        protected double[,] mData = null;
        protected ISwarm mSwarm;
        protected object mParticleGeneratorContext;

        public override void UpdateCost()
        {
            if (mIsCostValid == false)
            {
                mCost = mSwarm.Evaluate(this);
                mIsCostValid = true;
            }
        }
        
        public SimpleParticle(ISwarm swarm, int dimensions, object info)
        {
            mSwarm = swarm;
            mData = new double[dimensions, 2];
            mParticleGeneratorContext = info;
        }

        public double[] Positions
        {
            get
            {
                double[] result = new double[mData.GetLength(0)];
                for(int i=0; i < result.Length; ++i)
                {
                    result[i] = mData[i, 0];
                }
                return result;
            }
        }

        public double[] Velocity
        {
            get
            {
                double[] result = new double[mData.GetLength(0)];
                for (int i = 0; i < result.Length; ++i)
                {
                    result[i] = mData[i, 1];
                }
                return result;
            }
        }

        public int DimensionCount
        {
            get { return mData.GetLength(0); }
        }

        public virtual SimpleParticle Clone()
        {
            SimpleParticle clone = new SimpleParticle(mSwarm, DimensionCount, mParticleGeneratorContext);
            clone.Copy(this);
            return clone;
        }

        public double this[int index, ParticleVectorType vector_type]
        {
            get
            {
                if (index >= DimensionCount || index < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return mData[index, vector_type==ParticleVectorType.Position ? 0 : 1];
            }
            set
            {
                if (index >= DimensionCount || index < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                int column_index = vector_type==ParticleVectorType.Position ? 0 : 1;
                if (mData[index, column_index] != value)
                {
                    mData[index, column_index] = value;
                    if (column_index == 0)
                    {
                        mIsCostValid = false;
                    }
                }
            }
        }

        public virtual void Copy(SimpleParticle rhs)
        {
            if (rhs.DimensionCount != DimensionCount)
            {
                throw new ArgumentOutOfRangeException();
            }
            for (int i = 0; i < DimensionCount; ++i)
            {
                for (int j = 0; j < 2; ++j)
                {
                    mData[i, j] = rhs.mData[i, j];
                }
            }
            mIsCostValid = rhs.mIsCostValid;
            mCost = rhs.mCost;
        }
    }
}
