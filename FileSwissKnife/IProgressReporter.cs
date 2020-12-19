namespace FileSwissKnife
{
    public interface IProgressReporter
    {
        public event ProgressHandler OnProgress;
    }

    public delegate void ProgressHandler(object sender, ProgressHandlerArgs args);

    public class ProgressHandlerArgs
    {
        public string? Message { get; set; }

        public double Percent { get; set; }
    }
}