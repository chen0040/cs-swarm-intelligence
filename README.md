# cs-swarm-intelligence

Swam intelligence for numerical optimization implemented in .NET

# Features 

The current library support optimization problems in which solutions are either discrete or continuous vectors. The algorithms implemented for swarm-intelligence are listed below:

* Particle Swarm Optimization (PSO)
* Bees Algorithm
* Ant Colony System

# Install 

```bash
Install-Package cs-swarm-intelligence -Version 1.0.1
```

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

## Ant Colony System

The sample codes belows show to solve the Travelling Salesman Problem (TSP) using Ant Colony System:

```cs 
int populationSize = 100;

SimpleAnt bestSolution;
TspBenchmark tsp = Tsp.get(Tsp.Instance.bayg29);
int problemSize = tsp.ProblemSize();
int displayEvery = 10;
int maxIterations = 1000;
AntColonySystem<SimpleAnt>.SolveByAntColonySystem(populationSize, problemSize
, solution => // this returns the cost of the solution which in the case of the TSP is the total distance of visiting every cities exactly once using the route represented by the solution 
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
}, (state1, state2) => // this returns the heuristic value for a move from state1 to state2
{ 
	return 1.0 / (1.0 + tsp.Distance(state1, state2));
}, displayEvery, out bestSolution, null, maxIterations);
```

## Ant System 

The sample codes belows show to solve the Travelling Salesman Problem (TSP) using Ant System:

```cs 

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
```

