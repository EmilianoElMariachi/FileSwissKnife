using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using FileSwissKnife.Utils;

namespace FileSwissKnife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Task? _joinTask;
        private CancellationTokenSource? _cts;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
        }

        private void HashFiles()
        {
            //TODO: a effacer

            var hashNames = new[] { "SHA1", "MD5", "SHA256", "SHA384", "SHA512" };

            foreach (var file in FilesToHash.Text.Split(Environment.NewLine))
            {
                var computeHashes = HashUtil.ComputeHashes(file, hashNames);
                for (var i = 0; i < hashNames.Length; i++)
                {
                    var hashName = hashNames[i];
                    FilesToHash.AppendText(Environment.NewLine + hashName + "=" + computeHashes[i]);
                }
            }
        }


        private void TextBoxFilesToJoin_OnDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            // Note that you can have more than one file.
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length > 1)
                TextBoxFilesToJoin.Text = string.Join(Environment.NewLine, files);
            else
            {
                var filesToJoin = FileJoiner.GuessFilesToJoin(files[0], out var outputFile);

                TextBoxFilesToJoin.Text = string.Join(Environment.NewLine, filesToJoin);
                JoinOutputFilePath.Text = outputFile;
            }
        }

        private void FilesToJoin_OnPreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

    }
}
