using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwarmIntelligence.Bees
{
    public interface IBeeSwarm
    {
        double Evaluate(SimpleBee p);
    }
}
