namespace FileSwissKnife
{
    public abstract class ProgressReporterBase : IProgressReporter
    {
        public event ProgressHandler? OnProgress;

        protected void NotifyProgressChanged(double percent)
        {
            OnProgress?.Invoke(this, new ProgressHandlerArgs
            {
                Percent = percent
            });
        }
    }
}