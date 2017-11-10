using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwarmIntelligence.ACO
{
    public interface IAntColony 
    {
        double Evaluate(SimpleAnt agent);
    }
}
