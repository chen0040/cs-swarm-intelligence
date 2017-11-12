using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SwarmIntelligence.Bees;

namespace SwarmIntelligence
{
    public class UT_Bees
    {
        public static void RunMain()
        {
            int maxIterations = 2000;
            int dimension = 2;
            int popSize = 200;
            int displayEvery = 100;
            double[] lowerBounds = new double[] { -2.048, -2.048 };
            double[] upperBounds = new double[] { 2.048, 2.048 };
            SimpleBee finalSolution;

            BeeSwarm<SimpleBee>.Solve(popSize, dimension, (solution) =>
            {
                // this is the Rosenbrock Saddle cost function
                
                double x0 = solution[0];
                double x1 = solution[1];

                double cost = 100 * Math.Pow(x0 * x0 - x1, 2) + Math.Pow(1 - x0, 2);
                return cost;
            }, out finalSolution, lowerBounds, upperBounds, maxIterations, displayEvery);
        }


    }
}
