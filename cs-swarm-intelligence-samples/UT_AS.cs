using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwarmIntelligence.ACO;
using SwarmIntelligence.Benchmarks.Tsp;

namespace SwarmIntelligence
{
    public class UT_AS
    {
        public static void RunMain()
        {
            int populationSize = 100;
            
            SimpleAnt bestSolution;
            TspBenchmark tsp = Tsp.get(Tsp.Instance.bayg29);
            int problemSize = tsp.ProblemSize();
            int displayEvery = 10;
            int maxIterations = 1000;
            AntSystem<SimpleAnt>.SolveByAntSystem(populationSize, problemSize, solution =>
            {
                double cost = 0;
                for(int i=0; i < solution.Length; ++i)
                {
                    int j = (i + 1) % solution.Length;
                    int v = solution[i];
                    int w = solution[j];
                    cost += tsp.Distance(v, w);
                }
                return cost;
            }, (state1, state2) =>
            {
                return 1.0 / (1.0 + tsp.Distance(state1, state2));
            }, displayEvery, out bestSolution, null, maxIterations);
        }
    }
}
