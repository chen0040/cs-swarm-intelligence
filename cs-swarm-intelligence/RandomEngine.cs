using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Statistics;

namespace SwarmIntelligence
{
    public class RandomEngine
    {
        public static double NextDouble()
        {
            return DistributionModel.GetUniform();
        }
    }
}
