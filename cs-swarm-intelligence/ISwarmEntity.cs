using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwarmIntelligence
{
    public interface ISwarmEntity
    {
        void InvalidateCost();

        double Cost
        {
            get;
        }

        bool IsCostValid
        {
            get;
        }

        void UpdateCost();
    }
}
