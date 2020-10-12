using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FileSwissKnife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Task _joinTask;
        private CancellationTokenSource _cts;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartCancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (_cts != null)
            {
                _cts.Cancel(false);
                return;
            }

            switch (GetCurrentMode())
            {
                case Mode.Join:
                    JoinFiles();
                    break;
                case Mode.Split:
                    break;
                case Mode.Hash:
                    HashFiles();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }



        }

        private void HashFiles()
        {
            var hashNames = new[] { "SHA1", "MD5", "SHA256", "SHA384", "SHA512" };

            var dateTime = DateTime.Now;

            foreach (var file in FilesToHash.Text.Split(Environment.NewLine))
            {
                var computeHashes = HashUtil.ComputeHashes(file, hashNames);
                for (var i = 0; i < hashNames.Length; i++)
                {
                    var hashName = hashNames[i];
                    FilesToHash.AppendText(Environment.NewLine + hashName + "=" + computeHashes[i]);
                }

            }
            var timeSpan = DateTime.Now - dateTime;
        }


        private void MainWindow_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                switch (GetCurrentMode())
                {
                    case Mode.Join:
                        if (files.Length > 1)
                            FilesToJoin.Text = string.Join(Environment.NewLine, files);
                        else
                        {

                            var filesToJoin = FileUtil.GuessFilesToJoin(files[0], out var outputFile);

                            FilesToJoin.Text = string.Join(Environment.NewLine, filesToJoin);
                            JoinOutputFilePath.Text = outputFile;
                        }
                        break;
                    case Mode.Split:
                        break;
                    case Mode.Hash:
                        FilesToHash.Text = string.Join(Environment.NewLine, files);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void JoinFiles()
        {
            var inputFiles = FilesToJoin.Text.Split(Environment.NewLine);
            var outputFile = JoinOutputFilePath.Text;

            _cts = new CancellationTokenSource();
            _joinTask = Task.Run(() =>
            {
                FileUtil.Join(inputFiles, outputFile, _cts.Token, percent =>
                {
                    this.Dispatcher.Invoke(new Action<double>(OnProgress), DispatcherPriority.Background, percent);
                });
            }, _cts.Token).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    ProgressLabel.Content = "Canceled!";
                }
                else if (task.IsCompletedSuccessfully)
                {
                    ProgressLabel.Content = "Finished!";
                }

                _cts = null;

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OnProgress(double percent)
        {
            var percentStr = $"{percent:0.00}%";

            ProgressBar.Value = percent;
            ProgressLabel.Content = percentStr;
        }

        private void FilesToJoin_OnPreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        private Mode GetCurrentMode()
        {
            if (Equals(TabControl.SelectedItem, TabItemJoin))
                return Mode.Join;
            if (Equals(TabControl.SelectedItem, TabItemSplit))
                return Mode.Split;
            if (Equals(TabControl.SelectedItem, TabItemHash))
                return Mode.Hash;
            throw new NotSupportedException("Unknown tab item.");
        }


        private enum Mode
        {
            Join,
            Split,
            Hash
        }

    }
}
