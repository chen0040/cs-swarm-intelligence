using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwarmIntelligence.ACO
{
    public class AntColonySystem<Ant> : AntSystem<Ant>
        where Ant : SimpleAnt
    {
        public AntColonySystem(int population_size, int state_count, CostEvaluationMethod evaluator, CreateAntMethod generator = null)
            : base(population_size, state_count, evaluator, generator)
        {

        }

        public override void DepositPheromone()
        {
            List<Tuple<int, int>> path = mGlobalBestAnt.FindTrasitionPath();
            int segment_count = path.Count;
            for (int i = 0; i < segment_count; ++i)
            {
                Tuple<int, int> state_transition = path[i];
                int state1_id = state_transition.Item1;
                int state2_id = state_transition.Item2;
                double pheromone = mPheromones[state1_id, state2_id];
                double p_delta = GetRewardPerStateTransition(mGlobalBestAnt);
                pheromone += m_alpha * p_delta;

                mPheromones[state1_id, state2_id] = pheromone;
                if (mSymmetric)
                {
                    mPheromones[state2_id, state1_id] = pheromone;
                }
            }
        }

        public override void TransiteState(Ant ant, int state_index)
        {
            int current_state_id = ant.CurrentState;
            List<int> candidate_states = GetCandidateNextStates(ant, current_state_id);

            if (candidate_states.Count == 0) return;

            int selected_state_id = -1;
            double[] acc_prob = new double[candidate_states.Count];
            double product_sum = 0;
            double max_product = double.MinValue;

            int state_id_with_max_prob = -1;

            for (int i = 0; i < candidate_states.Count; ++i)
            {
                int candidate_state_id = candidate_states[i];
                double pheromone = mPheromones[current_state_id, candidate_state_id];
                double heuristic_value = GetHeuristicValue(current_state_id, candidate_state_id);

                double product = System.Math.Pow(pheromone, m_alpha) * System.Math.Pow(heuristic_value, m_beta);

                product_sum += product;
                acc_prob[i] = product_sum;

                if (product > max_product)
                {
                    max_product = product;
                    state_id_with_max_prob = candidate_state_id;
                }
            }

            double r = RandomEngine.NextDouble();
            if (r <= m_Q)
            {
                selected_state_id = state_id_with_max_prob;
            }
            else
            {
                r = RandomEngine.NextDouble();
                for (int i = 0; i < candidate_states.Count; ++i)
                {
                    acc_prob[i] /= product_sum;
                    if (r <= acc_prob[i])
                    {
                        selected_state_id = candidate_states[i];
                        break;
                    }
                }
            }
            
            if (selected_state_id != -1)
            {
                ant.Add(selected_state_id);
                LocalPheromoneUpdate(current_state_id, selected_state_id);
            }
        }

        protected void LocalPheromoneUpdate(int state1_id, int state2_id)
        {
            double pheromone = mPheromones[state1_id, state2_id];

            pheromone = (1 - m_rho) * pheromone + m_rho * mTau0;
            if (pheromone <= mTau0)
            {
                pheromone = mTau0;
            }

            mPheromones[state1_id, state2_id] = pheromone;
            if (mSymmetric)
            {
                mPheromones[state2_id, state1_id] = pheromone;
            }
        }

        public static double SolveByAntColonySystem(int population_size, int state_count, CostEvaluationMethod evaluator, HeuristicValueEvaluationMethod heuristicValueEvaulator, int displayEvery, out Ant global_best_solution, CreateAntMethod generator = null, int maxIterations = 1000)
        {
            AntColonySystem<Ant> solver = new AntColonySystem<Ant>(population_size, state_count, evaluator, generator);
            solver.HeuristicValueEvaluator = heuristicValueEvaulator;
            solver.Initialize();
            int iteration = 0;
            double global_best_solution_cost = solver.GlobalBestSolutionCost;
            double prev_global_best_solution_cost = global_best_solution_cost;
            while (iteration < maxIterations)
            {
                prev_global_best_solution_cost = global_best_solution_cost;
                solver.Iterate();
                global_best_solution_cost = solver.GlobalBestSolutionCost;
                
                iteration++;
                if(displayEvery > 0 && iteration % displayEvery == 0)
                {
                    Console.WriteLine("Iteration: {0}, Cost: {1}", iteration, global_best_solution_cost);
                }
            }

            global_best_solution = solver.GlobalBestSolution.Clone() as Ant;

            return global_best_solution_cost;
        }

    }
}
