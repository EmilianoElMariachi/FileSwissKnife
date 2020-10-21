using System.Threading;

namespace FileSwissKnife
{

    internal interface IReportProgressAction
    {
        public event ProgressHandler OnProgress;

        /// <summary>
        /// Should throw if some requirements are not met
        /// </summary>
        public void CheckPrerequisites();

        void Run(CancellationToken cancellationToken);
    }

    public delegate void ProgressHandler(object sender, ProgressHandlerArgs args);

    public class ProgressHandlerArgs
    {
        public double Percent { get; set; }
    }
}