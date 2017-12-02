

using System.Collections.Generic;
/**
* Created by xschen on 14/6/2017.
*/
namespace SwarmIntelligence.Benchmarks.Tsp
{
    public class Tsp
    {
        public enum Instance
        {
            a280,
            att48,
            bayg29,
            berlin52
        }

        private static Dictionary<Instance, TspBenchmark> cache = new Dictionary<Instance, TspBenchmark>();

        public static TspBenchmark get(Instance instance)
        {
            TspBenchmark benchmark;
            if (cache.ContainsKey(instance))
            {
                benchmark = cache[instance];
            }
            else
            {
                string tsp_topo = "";
                string tsp_opt = "";
                if(instance == Instance.bayg29)
                {
                    tsp_topo = Properties.Resources.bayg29_tsp_3c;
                    tsp_opt = Properties.Resources.bays29_opt_tour;
                }
                benchmark = new TspBenchmark(tsp_topo, tsp_opt);
                cache[instance] = benchmark;
            }
            return benchmark;

        }
    }
}
