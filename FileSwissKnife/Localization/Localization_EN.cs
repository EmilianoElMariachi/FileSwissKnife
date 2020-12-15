namespace FileSwissKnife.Localization
{
    internal class Localization_EN : ILocalization
    {
        public string CultureName => "en-US";

        public string DragMeSomeFilesToJoin => "Drag me one or more files to join here :)";

        public string DragMeSomeFilesToHash => "Drag me one or more files to hash here :)";

        public string StartJoin => "Join!";

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

        public string TabNameJoin => "Join";

        public string TabNameSplit => "Split";

        public string TabNameHash => "Hash";

        public string OutputFileCantBeUndefined => "Please, specify the output file!";

        public string CanOverrideOutputFile => "Output file «{0}» already exists, do you want to replace it?";

        public string YouChooseNotToOverride => "You finally decided not to override the output file «{0}»...";

        public string Override => "Override?";

        public string HashInputFiles => "File(s) to hash";

    }
}