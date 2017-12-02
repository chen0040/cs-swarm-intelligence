using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SwarmIntelligence.ACO
{
    public class AntSystem<Ant> : IAntColony
        where Ant : SimpleAnt
    {
        protected Ant[] mAnts;
        protected Ant mGlobalBestAnt;

        public delegate double CostEvaluationMethod(SimpleAnt agent);
        protected CostEvaluationMethod mEvaluator;

        public delegate Ant CreateAntMethod(object info);
        protected CreateAntMethod mAntGenerator;
        protected object mAntGeneratorContext;

        protected double m_alpha=1;
	    protected double m_beta=2;
	    protected double m_Q=0.9;
		protected double m_rho=0.1;
        protected bool mSymmetric = true;
        protected double[,] mPheromones;
        protected int mStateCount;

        protected double mTau0;

        public delegate List<int> NextCandidateStateLookupMethod(Ant ant, int state_id);
        protected NextCandidateStateLookupMethod mNextStateCandidateStateLookup;

        public delegate double HeuristicCostEvaluationMethod(int state1_id, int state2_id);
        protected HeuristicCostEvaluationMethod mHeuristicCostEvaluator;

        public delegate double RewardPerStateTransitionEvaluationMethod(Ant ant);
        protected RewardPerStateTransitionEvaluationMethod mRewardPerStateTransitionEvaluator;

        public RewardPerStateTransitionEvaluationMethod RewardPerStateTransitionEvaluator
        {
            get { return mRewardPerStateTransitionEvaluator; }
            set { mRewardPerStateTransitionEvaluator = value; }
        }

        public HeuristicCostEvaluationMethod HeuristicCostEvaluator
        {
            get { return mHeuristicCostEvaluator; }
            set { mHeuristicCostEvaluator = value; }
        }

        public NextCandidateStateLookupMethod CandidateStateLookup
        {
            get { return mNextStateCandidateStateLookup; }
            set { mNextStateCandidateStateLookup = value; }
        }

        public bool Symmetric
        {
            get { return mSymmetric; }
            set { mSymmetric = value; }
        }

        public double Alpha
        {
            get { return m_alpha; }
            set { m_alpha = value; }
        }

        public double Beta
        {
            get { return m_beta; }
            set { m_beta = value; }
        }

        public double Q
        {
            get { return m_Q; }
            set { m_Q = value; }
        }

        public double Rho
        {
            get { return m_rho; }
            set { m_rho = value; }
        }

        protected virtual double GetRewardPerStateTransition(Ant ant)
        {
            if (mRewardPerStateTransitionEvaluator != null)
            {
                return mRewardPerStateTransitionEvaluator(ant);
            }
            return 1.0 / ant.Cost;
        }

        public Ant GenerateAnt(object info)
        {
            if (mAntGenerator != null)
            {
                return mAntGenerator(info);
            }
            SimpleAnt p = new SimpleAnt(this, info);
            Ant cast_p = p as Ant;
            if (cast_p == null)
            {
                throw new ArgumentNullException();
            }
            return cast_p;
        }

        public virtual double Evaluate(SimpleAnt agent)
        {
            if (mEvaluator != null)
            {
                return mEvaluator(agent);
            }
            throw new ArgumentNullException();
        }

        public bool IsMaximization
        {
            get { return true; }
        }

        public AntSystem(int population_size, int state_count, CostEvaluationMethod evaluator, CreateAntMethod generator = null)
        {
            mEvaluator = evaluator;
            mStateCount = state_count;
            if (generator == null)
            {
                mAntGenerator = (info) =>
                    {
                        SimpleAnt p = new SimpleAnt(this, info);
                        Ant cast_p = p as Ant;
                        return cast_p;
                    };
            }
    
            mAnts = new Ant[population_size];
            mPheromones = new double[mStateCount, mStateCount];
            mTau0 = 1.0 / state_count;
        }

        public void Initialize(object info = null)
        {
            mAntGeneratorContext = info;
            mGlobalBestAnt = GenerateAnt(info);
            for (int i = 0; i < mAnts.Length; ++i)
            {
                mAnts[i] = GenerateAnt(info);
            }
            for (int i = 0; i < mStateCount; ++i)
            {
                for (int j = 0; j < mStateCount; ++j)
                {
                    mPheromones[i, j] = mTau0;
                }
            }
        }

        public void UpdateAntCost()
        {
            for (int i = 0; i < mAnts.Length; ++i)
            {
                mAnts[i].UpdateCost();
            }
        }

        public double GlobalBestSolutionCost
        {
            get
            {
                return mGlobalBestAnt.Cost;
            }
        }

        public Ant GlobalBestSolution
        {
            get
            {
                return mGlobalBestAnt;
            }
        }

        public static double SolveByAntSystem(int population_size, int state_count, CostEvaluationMethod evaluator,out Ant global_best_solution, CreateAntMethod generator = null, double tolerance = 0.000001, int maxIterations = 100000)
        {
            AntSystem<Ant> solver = new AntSystem<Ant>(population_size, state_count, evaluator, generator);
            solver.Initialize();
            int iteration = 0;
            double cost_reduction = tolerance;
            double global_best_solution_cost = solver.GlobalBestSolutionCost;
            double prev_global_best_solution_cost = global_best_solution_cost;
            while (cost_reduction >= tolerance && iteration < maxIterations)
            {
                prev_global_best_solution_cost = global_best_solution_cost;
                solver.Iterate();
                global_best_solution_cost = solver.GlobalBestSolutionCost;
                cost_reduction = prev_global_best_solution_cost - global_best_solution_cost;
                iteration++;
            }

            global_best_solution = solver.GlobalBestSolution.Clone() as Ant;

            return global_best_solution_cost;
        }

        public void UpdateGlobalBestAnt()
        {
            Ant best_particle = null;
            for (int i = 0; i < mAnts.Length; ++i)
            {
                if (mAnts[i].IsBetterThan(mGlobalBestAnt))
                {
                    best_particle = mAnts[i];
                }
            }
            if (best_particle != null)
            {
                mGlobalBestAnt.Copy(best_particle);
            }
        }

        public void Iterate()
        {
            AntTraverse();
            UpdateAntCost();
            EvaporatePheromone();
            UpdateGlobalBestAnt();
            DepositPheromone();
        }

        public virtual void AntTraverse()
        {
            int ant_count = mAnts.Length;


            for (int state_index = 0; state_index < mStateCount; ++state_index)
            {
                if (state_index == 0)
                {

                    for (int i = 0; i < ant_count; ++i)
                    {
                        mAnts[i].Reset(i, mStateCount);
                    }
                }
                else
                {
                    for (int i = 0; i < ant_count; ++i)
                    {
                        Ant ant = mAnts[i];
                        TransiteState(ant, state_index);
                    }
                }
            }
        }

        public virtual void DepositPheromone()
        {
            for (int ant_index = 0; ant_index < mAnts.Length; ++ant_index)
            {
                Ant ant = mAnts[ant_index];

                List<Tuple<int, int>> path = ant.FindTrasitionPath();
                int segment_count = path.Count;
                for (int i = 0; i < segment_count; ++i)
                {
                    Tuple<int, int> state_transition = path[i];
                    int state1_id = state_transition.Item1;
                    int state2_id = state_transition.Item2;
                    double pheromone = mPheromones[state1_id, state2_id];
                    double p_delta = GetRewardPerStateTransition(ant);
                    pheromone += m_alpha * p_delta;

                    mPheromones[state1_id, state2_id] = pheromone;
                    if (mSymmetric)
                    {
                        mPheromones[state2_id, state1_id] = pheromone;
                    }
                }
            }
        }

        public virtual void EvaporatePheromone()
        {
            double pheromone = 0;
            for (int i = 0; i < mStateCount; ++i)
            {
                for (int j = 0; j < mStateCount; ++j)
                {
                    pheromone = mPheromones[i, j];
                    pheromone = (1 - m_alpha) * pheromone;
                    if (pheromone < mTau0)
                    {
                        pheromone = mTau0;
                    }
					mPheromones[i, j] = pheromone;
                }
            }
        }

        public virtual List<int> GetCandidateNextStates(Ant ant, int state_id)
        {
            if (mNextStateCandidateStateLookup != null)
            {
                return mNextStateCandidateStateLookup(ant, state_id);
            }
            List<int> candidate_states = new List<int>();
            for (int i = 0; i < mStateCount; ++i)
            {
                if (!ant.HasTraversedState(i))
                {
                    candidate_states.Add(i);
                }
            }
            return candidate_states;
        }

        public virtual double GetHeuristicValue(int state1_id, int state2_id)
        {
            if (mHeuristicCostEvaluator != null)
            {
                return mHeuristicCostEvaluator(state1_id, state2_id);
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        public virtual void TransiteState(Ant ant, int state_index)
        {
            int current_state_id = ant.CurrentState;
            List<int> candidate_states = GetCandidateNextStates(ant, current_state_id);

            if (candidate_states.Count == 0) return;

            int selected_state_id = -1;
            double[] acc_prob = new double[candidate_states.Count];
            double product_sum = 0;
            
            for (int i = 0; i < candidate_states.Count; ++i)
            {
                int candidate_state_id=candidate_states[i];
                double pheromone = mPheromones[current_state_id, candidate_state_id];
                double heuristic_cost = GetHeuristicValue(current_state_id, candidate_state_id);

                double product = System.Math.Pow(pheromone, m_alpha) * System.Math.Pow(heuristic_cost, m_beta);
                
                product_sum += product;
                acc_prob[i] = product_sum;
            }

            double r = RandomEngine.NextDouble();
            for (int i = 0; i < candidate_states.Count; ++i)
            {
                acc_prob[i] /= product_sum;
                if (r <= acc_prob[i])
                {
                    selected_state_id = candidate_states[i];
                    break;
                }
            }
            if (selected_state_id != -1)
            {
                ant.Add(selected_state_id);
            }
        }


    }
}
