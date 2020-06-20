using System;

namespace qsrv.ApiResponses
{
    public class TargetSite
    {
        public readonly string Name;
        public readonly string FileName;
        public readonly int LineNumber;
        public readonly string StackTrace;

        public TargetSite(string name, string fileName, int lineNumber)
        {
            Name = name;
            FileName = fileName;
            LineNumber = lineNumber;
            StackTrace = Environment.StackTrace;
        }
    }
}