using TestContext = NUnit.Framework.TestContext;


namespace LDSTest.src.utils
{
    public class Results
    {
        public List<Result> all;
        public List<Result> failed;
        public List<Result> passed;

        public bool HasFailures()
        {
            return this.failed.Count > 0;
        }

        public bool Passed()
        {
            return !HasFailures();
        }

        public void Display()
        {
            int testCaseId = BaseTest.GetTestCaseId();
            Console.WriteLine();
            Console.WriteLine("Summary");
            string passFail = HasFailures() ? "Failed" : "Passed";
            Console.WriteLine($"\tTest Case {TestContext.CurrentContext.Test.FullName} TestRail test case id: {BaseTest.GetTestCaseId()} {passFail}");
            Console.WriteLine($"\tTotal Assertions {this.all.Count}");
            Console.WriteLine($"\tPassed Assertions {this.passed.Count}");
            Console.WriteLine($"\tFailed Assertions {this.failed.Count}");
            Console.WriteLine();

            //TestContext.Progress.WriteLine($"Total Assertions {this.all.Count}");
            //TestContext.Progress.WriteLine($"Passed Assertions {this.passed.Count}");
            //TestContext.Progress.WriteLine($"Failed Assertions {this.failed.Count}");

            if (HasFailures())
            {
                Console.WriteLine();
                Console.WriteLine("Failures");
                foreach (Result result in failed)
                {
                    if (result.testCaseId == testCaseId)
                    {
                        Console.WriteLine($"\t{result.message}");
                        if (BaseTest.verbose)
                        {
                            Console.WriteLine("\t\tStack Trace");

                            foreach (string trace in result.stackTrace)
                            {
                                Console.Write("\t\t\t" + trace);
                            }
                            Console.WriteLine();
                        }
                    }
                }
            }

            if (BaseTest.verbose)
            {
                foreach (Result result in all)
                {
                    if (result.testCaseId == testCaseId)
                    {
                        string status = result.passed ? "Passed" : "Failed";

                        //Console.ForegroundColor = result.passed ? ConsoleColor.Green : ConsoleColor.Red;

                        //Console.Write(status);
                        //Console.ResetColor();
                        Console.WriteLine($"{status}, {result.message}");
                    }
                }
            }
        }

        public void Add(Result result)
        {
            this.all.Add(result);
            if (result.passed) this.passed.Add(result);
            else this.failed.Add(result);
        }

        public string GetErrorMessages()
        {
            string errorMessages = "";

            foreach (Result error in failed)
            {
                errorMessages += error.message + "\n";
            }

            return errorMessages;
        }

        public Results()
        {
            this.all = new List<Result>();
            this.failed = new List<Result>();
            this.passed = new List<Result>();
        }
    }
}