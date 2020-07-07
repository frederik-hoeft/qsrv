using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace qsrv.ApiRequests
{
    public class ProcessThreadHelper : IDisposable
    {
        private readonly ManualResetEventSlim manualResetEvent;
        public ProcessThreadHelper(ManualResetEventSlim manualResetEvent)
        {
            this.manualResetEvent = manualResetEvent;
        }
        public void Dispose()
        {
            manualResetEvent.Set();
        }
    }
}
