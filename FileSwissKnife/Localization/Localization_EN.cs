namespace FileSwissKnife.Localization
{
    internal class Localization_EN : ILocalization
    {
        public string CultureName => "en-US";

        public string DragMeSomeFilesToJoin => "Drag me one or more files to join here :)";

        public string DragMeSomeFilesToHash => "Drag me one or more files to hash here :)";

        public string DragMeSomeFileToSplit => "Drag me some file to split :)";

        public string CancellingHash => "cancelling...";

        public string CancellingJoin => "Cancelling...";

        public string CancelJoin => "Cancel";

        public string ConfirmCloseCancelHash => "Do you really want to close and cancel?";

        public string ConfirmCloseCancelHashTitle => "Close and cancel?";

        public string HashInProgress => "in progress...";

        public string HashCanceled => "canceled.";

        public string OperationCanceled => "Canceled";

        public string OperationFinishedIn => "Finished in {0}";

        public string OperationError => "Error!";

        public string JoinInputFiles => "Files to join:";

        public string JoinOutputFile => "Output file:";

        public string SplitInputFile => "File to split:";

        public string SplitOutputDir => "Output directory:";

        public string SplitSize => "Size:";

        public string TabNameJoin => "Join";

        public string TabNameSplit => "Split";

        public string TabNameHash => "Hash";

        public string OutputFileCantBeUndefined => "Please, specify the output file!";

        public string CanOverrideOutputFile => "Output file «{0}» already exists, do you want to replace it?";

        public string YouChooseNotToOverride => "You finally decided not to override the output file «{0}»...";

        public string Override => "Override?";

        public string HashInputFiles => "File(s) to hash";

        public string UnitB => "B";

        public string UnitKB => "kB";

        public string UnitKiB => "KiB";

        public string UnitMB => "MB";

        public string UnitMiB => "MiB";

        public string UnitGB => "GB";

        public string UnitGiB => "GiB";

        public string SplitSizeShouldBeDefined => "Split size should be defined.";

        public string SplitSizeInvalid => "Invalid split size «{0}», value should be a strictly positive integer.";

        public string SplitUnitShouldBeSelected => "Please select a split unit.";

        public string AllFiles => "All files";

        public string BrowseFilesToHashTitle => "Input files selection";

        public string CopyErrors => "Copy error messages";

        public string BrowseSplitOutputDirTitle => "Output directory selection";

        public string BrowseFileToSplitTitle => "Input file selection";

        public string SplitNumPosBeforeBaseName => "Before name";

        public string SplitNumPosAfterBaseName => "After name";

        public string SplitNumPosBeforeExt => "Before extension";

        public string SplitNumPosAfterExt => "After extension";

        public string SplitErrorFileSizeLessThanSplitSize => "The split size ({0}) is greater than the specified file size ({1}).";

        public string SplitNumPosShouldBeSelected => "The numbering position should be selected.";

        public string SplitStartNumberShouldBeDefined => "The start number should be defined.";

        public string SplitStartNumberInvalid => "The start number ({0}) is not a valid integer.";

        public string SplitInputFileNotFound => "Input file «{0}» not found.";

    }
}