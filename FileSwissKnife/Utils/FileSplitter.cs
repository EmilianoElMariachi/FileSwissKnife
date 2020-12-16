using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileSwissKnife.Utils
{
    public class FileSplitter : ProgressReporterBase
    {

        public Task Split(string inputFile, string outputFolder, long splitSize, CancellationToken cancellationToken)
        {
            var fileInfo = new FileInfo(inputFile);

            if (fileInfo.Length <= splitSize)
                throw new InvalidOperationException($"Split size «{splitSize}» is greater than file size «{fileInfo.Length}»."); //TODO: à localiser

            return Task.Run(() =>
            {
                using var fileStream = File.OpenRead(inputFile);
                //TODO: à continuer
            }, cancellationToken);
        }


    }
}
