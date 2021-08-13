using System;
using System.Threading;

namespace CvbConsoleApp
{
    public static class CancellationHelper
    {
        public static CancellationTokenSource CancelByControlC()
        {
            var cts = new CancellationTokenSource();

            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                Console.WriteLine("Control+C");
                e.Cancel = true;
                try { cts.Cancel(); } catch (ObjectDisposedException) { }
            };

            return cts;
        }
    }
}
