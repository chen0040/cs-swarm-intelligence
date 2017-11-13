# cs-swarm-intelligence

Swam intelligence for numerical optimization implemented in .NET

# Features 

The current library support optimization problems in which solutions are either discrete or continuous vectors. The algorithms implemented for swarm-intelligence are listed below:

* Particle Swarm Optimization (PSO)
* Bees Algorithm
* Ant Colony System

# Usage

## Running PSO

The sample codes below shows how to solve the "Rosenbrock Saddle" continuous optmization problem using PSO:

```cs
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
```

## Running Bees Algorithm 

The sample codes below shows how to solve the "Rosenbrock Saddle" continuous optmization problem using Bees Algorithm:

```cs
int maxIterations = 2000;
int dimension = 2;
int displayEvery = 10;
double[] lowerBounds = new double[] { -2.048, -2.048 };
double[] upperBounds = new double[] { 2.048, 2.048 };
SimpleBee finalSolution;

BeeSwarm<SimpleBee>.Solve(dimension, (solution) =>
{
	// this is the Rosenbrock Saddle cost function
	
	double x0 = solution[0];
	double x1 = solution[1];

	double cost = 100 * Math.Pow(x0 * x0 - x1, 2) + Math.Pow(1 - x0, 2);
	return cost;
}, out finalSolution, lowerBounds, upperBounds, maxIterations, displayEvery);
```

