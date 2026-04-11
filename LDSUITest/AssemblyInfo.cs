using NUnit.Framework;

// Enable parallel execution at fixture level (test classes run in parallel)
[assembly: Parallelizable(ParallelScope.Fixtures)]

// Set number of workers (0 run sequentially, or specify a number like 4)
[assembly: LevelOfParallelism(2)]
