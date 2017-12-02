


using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
/**
* Created by xschen on 14/6/2017.
*/
namespace SwarmIntelligence.Benchmarks.Tsp {
    public class TspBenchmark {
   private Dictionary<int, Vector2D> points = new Dictionary<int, Vector2D>();
        private double[][] distances;
        private int N;
        private List<int> optTour = new List<int>();
        private double bestCost;

        public TspBenchmark(string topo, string opt) {
            
            string[] lines = topo.Split('\n');
            
            foreach(var line in lines)
            {
                string[] comps = line.Split(',');
                int v = int.Parse(comps[0]) - 1;
                double x = double.Parse(comps[1]);
                double y = double.Parse(comps[2]);
                Vector2D pos = new Vector2D(x, y);
                points[v] = pos;
            }


            N = points.Count;

            distances = new double[N][];
            for (int i = 0; i < N; ++i) {
                distances[i] = new double[N];
            }

            for (int i = 0; i < N; ++i) {
                for (int j = i + 1; j < N; ++j) {
                    double distance = points[i].distance(points[j]);
                    distances[i][j] = distance;
                    distances[j][i] = distance;
                }
            }
            
            lines = opt.Split('\n');
            foreach (var line in lines)
            {
                int v = int.Parse(line) - 1;
                optTour.Add(v);
            }


            double cost = 0;
            for (int i = 0; i < optTour.Count; ++i) {
                int j = (i + 1) % optTour.Count;

                int v = optTour[i];
                int w = optTour[j];

                double distance = distances[v][w];
                cost += distance;
            }

            bestCost = cost;

        }

        public int ProblemSize() {
            return N;
        }

        public double OptCost() {
            return bestCost;
        }

        public List<int> OptTour() {
            return optTour;
        }

        public double Distance(int v, int w) {
            return distances[v][w];
        }

        public Vector2D Position(int v) {
            return points[v];
        }
    }
}