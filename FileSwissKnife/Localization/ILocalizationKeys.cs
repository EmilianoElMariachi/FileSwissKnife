namespace FileSwissKnife.Localization
{
    public interface ILocalizationKeys
    {
        string DragMeSomeFilesToJoin { get; }

        string DragMeSomeFilesToHash { get; }

        public string StartJoin { get; }

        string Cancelling { get; }

        string CancelJoin { get; }

        string OperationCanceled { get; }

        string OperationFinishedIn { get; }

        string OperationError { get; }

        string JoinInputFiles { get; }

        string JoinOutputFile { get; }

        string TabNameJoin { get; }

        string TabNameSplit { get; }

        string TabNameHash { get; }

        string OutputFileCantBeUndefined { get; }

        string CanOverrideOutputFile { get; }

        string YouChooseNotToOverride { get; }

        string Override { get; }

        string HashInputFiles { get; }

    }
}
