namespace FileSwissKnife
{
    public abstract class ProgressReporterBase : IProgressReporter
    {
        public event ProgressHandler? OnProgress;

        protected void NotifyProgressChanged(double percent, string? message = null)
        {
            OnProgress?.Invoke(this, new ProgressHandlerArgs
            {
                Message = message,
                Percent = percent
            });
        }
    }
}