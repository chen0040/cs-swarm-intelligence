using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SwarmIntelligence.PSO;

namespace SwarmIntelligence
{
    public class UT_Bees
    {
        public static void RunMain()
        {
            int maxIterations = 2000;
            int dimension = 2;
            int popSize = 200;
            double[] lowerBounds = new double[] { -2.048, -2.048 };
            double[] upperBounds = new double[] { 2.048, 2.048 };
            SimpleParticle finalSolution;

            ParticleSwarm<SimpleParticle>.Solve(popSize, dimension, (solution, constraints) =>
            {
                // this is the Rosenbrock Saddle cost function
                double[] positions = solution.Positions;
                double x0 = positions[0];
                double x1 = positions[1];

                double cost = 100 * Math.Pow(x0 * x0 - x1, 2) + Math.Pow(1 - x0, 2);
                return cost;
            }, lowerBounds, upperBounds, out finalSolution, maxIterations);
        }


    }
}
