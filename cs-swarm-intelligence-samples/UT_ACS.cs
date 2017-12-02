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
    public class UT_ACS
    {
        public static void RunMain()
        {
            int populationSize = 100;
            int problemSize = 29;
            SimpleAnt bestSolution;
            TspBenchmark tsp = Tsp.get(Tsp.Instance.bayg29);
            int displayEvery = 10;
            AntColonySystem<SimpleAnt>.SolveByAntColonySystem(populationSize, problemSize, solution =>
            {
                double cost = 0;
                Debug.Assert(solution.Length > 1);
                for(int i=0; i < solution.Length; ++i)
                {
                    int j = (i + 1) % solution.Length;
                    int v = solution[i];
                    int w = solution[j];
                    cost += tsp.distance(v, w);
                }
                return cost;
            }, displayEvery, out bestSolution);
        }
    }
}
