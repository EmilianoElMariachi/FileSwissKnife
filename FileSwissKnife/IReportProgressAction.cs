using System.Threading;

namespace FileSwissKnife
{

    public delegate void ProgressHandler(object sender, ProgressHandlerArgs args);

    public class ProgressHandlerArgs
    {
        public double Percent { get; set; }
    }

    internal interface IReportProgressAction
    {
        public event ProgressHandler OnProgress;


        void Run(CancellationToken cancellationToken);
    }
}