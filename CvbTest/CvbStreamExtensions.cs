using System;
using CvbStream = Stemmer.Cvb.Driver.Stream;


namespace CvbConsoleApp
{
    public static class CvbStreamExtensions
    {
        public static IDisposable StartAsDisposable(this CvbStream self)
        {
            self.Start();
            return new DisposableAction(self.Stop);
        }

        private class DisposableAction : IDisposable
        {
            public DisposableAction(Action onDispose)
            {
                _onDispose = onDispose;
            }

            public void Dispose()
            {
                _onDispose();
            }

            private readonly Action _onDispose;
        }
    }
}
