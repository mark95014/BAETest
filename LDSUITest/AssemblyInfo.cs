using NUnit.Framework;

[assembly: Parallelizable(ParallelScope.None)]

// Set number of workers (0 run sequentially, or specify a number like 4)
[assembly: LevelOfParallelism(1)]
