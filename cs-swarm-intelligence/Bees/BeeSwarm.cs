using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwarmIntelligence.Bees
{
    public class BeeSwarm<Bee> : IBeeSwarm
        where Bee : SimpleBee
    {
        protected int mScoutBeeCount = 60; //number of scout bees (e.g. 40-1000)
        protected int mBestPatchCount = 20; //number of best selected patches (e.g. 10-50)
        protected int mElitePatchCount = 10; //number of elite selected patches (e.g. 10-50)
        protected int mBeeCount_BestPatches = 15; //number of recruited bees around best selected patches (e.g. 10-50)
        protected int mBeeCount_ElitePatches = 30; //number of recruited bees around elite selected patches (e.g. 10-50)
        protected Bee[] mPatches = null;
        private Bee mGlobalBestSolution = null;

        protected double[] mLowerBounds;
        protected double[] mUpperBounds;

        public int LocalSearchIntensity { get; set; } = 10;
        public double[] LocalSearchRegion { get; set; }

        protected object mConstraints = null;

        public object Constraints
        {
            get { return mConstraints; }
            set { mConstraints = value; }
        }

        public double[] LowerBounds
        {
            get { return mLowerBounds; }
            set { mLowerBounds = value; }
        }

        public double[] UpperBounds
        {
            get { return mUpperBounds; }
            set { mUpperBounds = value; }
        }

        public double Evaluate(SimpleBee bee)
        {
            return mEvaluator(bee);
        }

        public delegate Bee CreateBeeMethod();
        protected CreateBeeMethod mBeeGenerator;

        public delegate double CostEvaluationMethod(SimpleBee p);
        protected CostEvaluationMethod mEvaluator;

        public BeeSwarm(int _n, int _n1, int _n2, int _m, int _e, CostEvaluationMethod evaluator, CreateBeeMethod generator)
        {
            mScoutBeeCount = _n;
            mBeeCount_BestPatches = _n1;
            mBeeCount_ElitePatches = _n2;

            mBestPatchCount = _m;
            mElitePatchCount = _e;

            mBeeGenerator = generator;
            mEvaluator = evaluator;
        }

        public BeeSwarm(int _n, int _n1, int _n2, int _m, int _e, CostEvaluationMethod evaluator)
        {
            mScoutBeeCount = _n;
            mBeeCount_BestPatches = _n1;
            mBeeCount_ElitePatches = _n2;

            mBestPatchCount = _m;
            mElitePatchCount = _e;

            mBeeGenerator = () =>
            {
                SimpleBee p = new SimpleBee(this, mLowerBounds.Length, mLowerBounds, mUpperBounds);
                p.Initialize(Constraints);
                return p as Bee;
            };
            mEvaluator = evaluator;
        }

        protected Bee GenerateBee()
        {
            if (mBeeGenerator != null)
            {
                return mBeeGenerator();
            }
            else
            {
                throw new NotImplementedException();                
            }
        }

        public void Initialize()
        {
            mPatches = new Bee[mScoutBeeCount];

            for (int i = 0; i < mScoutBeeCount; i++)
            {
                Bee bee = GenerateBee();
                bee.Initialize(mConstraints);
                mPatches[i] = bee;
            }
            
            Array.Sort(mPatches);
            mGlobalBestSolution = mPatches[0].Clone() as Bee;
        }

        public Bee GlobalBestSolution
        {
            get
            {
                return mGlobalBestSolution;
            }
        }

        public double GlobalBestSolutionCost
        {
            get
            {
                return mGlobalBestSolution.Cost;
            }
        }

        public static double Solve(int population_size, int dimension_count, CostEvaluationMethod evaluator, out Bee global_best_solution, double[] lower_bounds = null, double[] upper_bounds = null, int maxIterations = 100000, int displayEvery = 100, object constraints = null)
        {
            int scoutBeeCount = 60;
            int bestPatch = 15;
            int elitePatch = 30;
            BeeSwarm<Bee> solver = new BeeSwarm<Bee>(scoutBeeCount, bestPatch, elitePatch, 20, 10, evaluator);
            solver.LowerBounds = lower_bounds;
            solver.UpperBounds = upper_bounds;
            solver.Constraints = constraints;
            solver.LocalSearchRegion = new double[dimension_count];
            for(int i=0; i < dimension_count; ++i)
            {
                solver.LocalSearchRegion[i] = (upper_bounds[i] - lower_bounds[i]) / bestPatch;
            }

            solver.Initialize();
            int iteration = 0;
            double global_best_solution_cost = solver.GlobalBestSolutionCost;
            double prev_global_best_solution_cost = global_best_solution_cost;
            while (iteration < maxIterations)
            {
                prev_global_best_solution_cost = global_best_solution_cost;
                solver.Iterate();
                global_best_solution_cost = solver.GlobalBestSolutionCost;
                if (iteration % displayEvery == 0)
                {
                    Console.WriteLine("Generation: {0}, Best Cost: {1}", iteration, global_best_solution_cost);
                }
                iteration++;
            }

            global_best_solution = solver.GlobalBestSolution.Clone() as Bee;

            return global_best_solution_cost;
        }

        public static double Solve(int population_size, int dimension_count, CostEvaluationMethod evaluator, CreateBeeMethod generator, out Bee global_best_solution, double[] lower_bounds = null, double[] upper_bounds = null, object constraints = null, double tolerance = 0.000001, int maxIterations = 100000)
        {
            BeeSwarm<Bee> solver = new BeeSwarm<Bee>(60, 15, 30, 20, 10, evaluator, generator);
            solver.LowerBounds = lower_bounds;
            solver.UpperBounds = upper_bounds;
            solver.Constraints = constraints;
            solver.LocalSearchRegion = new double[dimension_count];
            for (int i = 0; i < dimension_count; ++i)
            {
                solver.LocalSearchRegion[i] = (upper_bounds[i] - lower_bounds[i]) / 1000;
            }

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

            global_best_solution = solver.GlobalBestSolution.Clone() as Bee;

            return global_best_solution_cost;
        }

        public void Iterate()
        {
            //scout at elite selected patches
            for (int j = 0; j < mElitePatchCount; ++j) //number of elite selected patches
            {
                //the following for loop is equivalent to a local search around each solution in the elite solutions
                for (int i = 0; i < mBeeCount_ElitePatches; ++i)
                {
                    mGlobalBestSolution.Dance(mPatches[j], LocalSearchRegion, mConstraints);
                    if (mGlobalBestSolution.IsBetterThan(mPatches[j]))
                    {
                        mPatches[j] = mGlobalBestSolution.Clone() as Bee;
                    }
                }
            }

            //scout at best selected patches next best to the elite (therefore patch count=(_m - _e))
            for (int j = mElitePatchCount; j < mBestPatchCount; ++j)
            {
                //the following for loop is equivalent to a local search around each solution next best to the elite solutions
                for (int i = 0; i < mBeeCount_BestPatches; ++i)
                {
                    mGlobalBestSolution.Dance(mPatches[j], LocalSearchRegion, mConstraints);
                    if (mGlobalBestSolution.IsBetterThan(mPatches[j]))
                    {
                        mPatches[j] = mGlobalBestSolution.Clone() as Bee;
                    }
                }
            }

            //assign remaining bees to search randomly 
            for (int j = mBestPatchCount; j < mScoutBeeCount; ++j)
            {
                mGlobalBestSolution.RandomSearch(mLowerBounds, mUpperBounds, mConstraints);
                mPatches[j] = mGlobalBestSolution.Clone() as Bee;
            }
            Array.Sort(mPatches);

            mGlobalBestSolution = mPatches[0].Clone() as Bee;
        }
    }
}
