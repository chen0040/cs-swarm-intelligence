using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwarmIntelligence.PSO
{
    public interface ISwarm 
    {
        double Evaluate(SimpleParticle p);
    }
}
