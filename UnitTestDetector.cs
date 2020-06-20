using System;
using System.Linq;

namespace qsrv
{
    public static class UnitTestDetector
    {
        static UnitTestDetector()
        {
            const string testAssemblyName = "Microsoft.TestPlatform.CoreUtilities";
            IsInUnitTest = AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.StartsWith(testAssemblyName));
        }

        public static bool IsInUnitTest { get; }
    }
}