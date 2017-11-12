using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwarmIntelligence.PSO
{
    public class ParticleSwarm<Particle> : ISwarm
        where Particle : SimpleParticle
    {
        protected Particle[] mParticles = null;
        protected Particle[] mLocalBestParticles = null;
        protected Particle mGlobalBestSolution = null;
        protected int mDimensionCount;
        
        protected double[] mLowerBounds;
        protected double[] mUpperBounds;

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

        protected double mC1 = 1;
        protected double mC2 = 2;

        public double C1
        {
            get { return mC1; }
            set { mC1 = value; }
        }

        public double C2
        {
            get { return mC2; }
            set { mC2 = value; }
        }

        public delegate double CostEvaluationMethod(SimpleParticle p, object constraints);
        protected CostEvaluationMethod mEvaluator;

        public delegate Particle CreateParticleMethod(object constraints);
        public CreateParticleMethod mParticleGenerator;

        protected object mConstraints;

        public double Evaluate(SimpleParticle p)
        {
            if (mEvaluator == null)
            {
                throw new ArgumentNullException();
            }
            return mEvaluator(p, mConstraints);
        }

        public ParticleSwarm(int population_size, int dimension_count, CostEvaluationMethod evaluator, double[] lower_bounds, double[] upper_bounds)
        {
            mDimensionCount = dimension_count;

            mLowerBounds = lower_bounds;
            mUpperBounds = upper_bounds;
            
            mParticleGenerator = (constraints) =>
                {
                    SimpleParticle p = new SimpleParticle(this, mDimensionCount, constraints);
                    double lower_bound;
                    double upper_bound;
                    double range;
                    for (int d = 0; d < mDimensionCount; ++d)
                    {
                        lower_bound = lower_bounds[d];
                        upper_bound = upper_bounds[d];
                        range = upper_bound - lower_bound;
                        p[d, ParticleVectorType.Position] = lower_bound + range * RandomEngine.NextDouble();
                        p[d, ParticleVectorType.Velocity] = 0;
                    }
                    Particle cast_p = p as Particle;
                    return cast_p;
                };
    
            mParticles = new Particle[population_size];
            mLocalBestParticles = new Particle[population_size];
            mEvaluator = evaluator;
        }

        public ParticleSwarm(int population_size, int dimension_count, CostEvaluationMethod evaluator, CreateParticleMethod generater = null)
        {
            mDimensionCount = dimension_count;
            mParticleGenerator = generater;
            mEvaluator = evaluator;

            mParticles = new Particle[population_size];
            mLocalBestParticles = new Particle[population_size];
            mEvaluator = evaluator;
        }

        public void Initialize(object info = null)
        {
            mConstraints = info;
            mGlobalBestSolution = GenerateParticle(info);
            for (int i = 0; i < mParticles.Length; ++i)
            {
                mParticles[i] = GenerateParticle(info);
                mLocalBestParticles[i] = mParticles[i].Clone() as Particle;
            }

            UpdateParticleCosts();
            UpdateLocalBestParticles();
            UpdateGlobalBestParticle();
        }

        public void UpdateParticleCosts()
        {
            for (int i = 0; i < mParticles.Length; ++i)
            {
                mParticles[i].UpdateCost();
            }
        }

        public void UpdateLocalBestParticles()
        {
            for(int i=0; i < mParticles.Length; ++i)
            {
                if (mParticles[i].IsBetterThan(mLocalBestParticles[i]))
                {
                    mLocalBestParticles[i].Copy(mParticles[i]);
                }
            }
        }

        public double GlobalBestSolutionCost
        {
            get
            {
                return mGlobalBestSolution.Cost;
            }
        }

        public Particle GlobalBestSolution
        {
            get
            {
                return mGlobalBestSolution;
            }
        }

        public static double Solve(int population_size, int dimension_count, CostEvaluationMethod evaluator, double[] lower_bounds, double[] upper_bounds, out Particle global_best_solution, int maxIterations=100000, int displayEvery = 100)
        {
            ParticleSwarm<Particle> solver = new ParticleSwarm<Particle>(population_size, dimension_count, evaluator, lower_bounds, upper_bounds);
            solver.Initialize();
            int iteration = 0;
            double global_best_solution_cost = solver.GlobalBestSolutionCost;
            double prev_global_best_soution_cost = global_best_solution_cost;
            while (iteration < maxIterations)
            {
                prev_global_best_soution_cost = global_best_solution_cost;
                solver.Iterate();
                global_best_solution_cost = solver.GlobalBestSolutionCost;
                //cost_reduction = prev_global_best_soution_cost - global_best_solution_cost;
                if(iteration % displayEvery == 0)
                {
                    Console.WriteLine("Generation: {0}, Best Cost: {1}", iteration, global_best_solution_cost);
                }
                iteration++;
            }

            global_best_solution = solver.GlobalBestSolution.Clone() as Particle;

            return global_best_solution_cost;
        }

        public static double Solve(int population_size, int dimension_count, CostEvaluationMethod evaluator, CreateParticleMethod generator, out Particle global_best_solution, double[] lower_bounds = null, double[] upper_bounds = null, object constraints = null, double tolerance = 0.000001, int maxIterations = 100000)
        {
            ParticleSwarm<Particle> solver = new ParticleSwarm<Particle>(population_size, dimension_count, evaluator, generator);
            solver.LowerBounds = lower_bounds;
            solver.UpperBounds = upper_bounds;

            solver.Initialize(constraints);
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

            global_best_solution = solver.GlobalBestSolution.Clone() as Particle;

            return global_best_solution_cost;
        }

        public void UpdateParticleVelocity()
        {
            for (int i = 0; i < mParticles.Length; ++i)
            {
                for (int j = 0; j < mDimensionCount; ++j)
                {
                    double oldV = mParticles[i][j, ParticleVectorType.Velocity];
                    double Xj = mParticles[i][j, ParticleVectorType.Position];
                    double X_lbest = mLocalBestParticles[i][j, ParticleVectorType.Position];
                    double X_gbest = mGlobalBestSolution[j, ParticleVectorType.Position];

                    double r1 = RandomEngine.NextDouble();
                    double r2 = RandomEngine.NextDouble();
                    double r3 = RandomEngine.NextDouble();

                    double w = 0.5 + r3 / 2;
                    double newV = w * oldV + mC1 * r1 * (X_lbest - Xj) + mC2 * r2 * (X_gbest - Xj);

                    mParticles[i][j, ParticleVectorType.Velocity] = newV;
                }
            }
        }

        public void UpdateParticlePosition()
        {
            for (int i = 0; i < mParticles.Length; ++i)
            {
                for (int j = 0; j < mDimensionCount; ++j)
                {
                    double Vj = mParticles[i][j, ParticleVectorType.Velocity];
                    double Xj = mParticles[i][j, ParticleVectorType.Position];

                    mParticles[i][j, ParticleVectorType.Position] = Xj + Vj;
                }
            }
        }

        public void UpdateGlobalBestParticle()
        {
            Particle best_particle = null;
            for (int i = 0; i < mParticles.Length; ++i)
            {
                if (mParticles[i].IsBetterThan(mGlobalBestSolution))
                {
                    best_particle = mParticles[i];
                }
            }
            if (best_particle != null)
            {
                mGlobalBestSolution.Copy(best_particle);
            }
        }

        public void Iterate()
        {
            UpdateParticleVelocity();
            UpdateParticlePosition();
            UpdateParticleCosts();
            UpdateLocalBestParticles();
            UpdateGlobalBestParticle();
        }

        public virtual Particle GenerateParticle(object info)
        {
            if (mParticleGenerator != null)
            {
                return mParticleGenerator(info);
            }
            SimpleParticle p = new SimpleParticle(this, mDimensionCount, info);
            if(mLowerBounds != null && mUpperBounds != null)
            {
                double lower_bound;
                double upper_bound;
                double range;
                for (int i = 0; i < mDimensionCount; ++i)
                {
                    lower_bound=mLowerBounds[i];
                    upper_bound=mUpperBounds[i];
                    range=upper_bound-lower_bound;

                    p[i, ParticleVectorType.Position] = lower_bound + range * RandomEngine.NextDouble();
                }
            }
            Particle cast_p = p as Particle;
            if (cast_p == null)
            {
                throw new ArgumentNullException();
            }
            return cast_p;
        }

        public bool IsMinimization
        {
            get { return true; }
        }
    }
}
