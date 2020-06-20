using System;
using System.Collections.Generic;
using System.Text;

namespace qsrv
{
    public class Optional<T>
    {
        public T Result { get; }
        public bool Success { get; }
        public Optional(T result, bool success)
        {
            Result = result;
            Success = success;
        }
    }
}
