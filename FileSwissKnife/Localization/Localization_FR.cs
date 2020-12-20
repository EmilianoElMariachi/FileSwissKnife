namespace FileSwissKnife.Localization
{
    public class Localization_FR : ILocalization
    {
        public string CultureName => "fr-FR";

        public string DragMeSomeFilesToJoin => "Glisse moi un ou plusieurs fichiers à joindre par ici :)";

        public string DragMeSomeFilesToHash => "Glisse moi un ou plusieurs fichiers à hacher par ici :)";

        public string DragMeSomeFileToSplit => "Glisse moi un fichier à séparer par ici :)";

        public string CancellingHash => "annulation";

        public string CancellingJoin => "Annulation...";

        public string CancelJoin => "Annuler";

        public string ConfirmCloseCancelHash => "Voulez-vous vraiment fermer et annuler ?";

        public string ConfirmCloseCancelHashTitle => "Fermer et annuler ?";

        public string HashInProgress => "en cours...";

        public string HashCanceled => "annulé.";

        public string OperationCanceled => "Annulé";

        public string OperationFinishedIn => "Terminé en {0}";

        public string OperationError => "Erreur!";

        public string JoinInputFiles => "Fichiers à joindre:";

        public string JoinOutputFile => "Fichier de sortie:";

        public string SplitInputFile => "Fichier à séparer:";

        public string SplitOutputFolder => "Répertoire de sortie:";

        public string SplitSize => "Taille:";

        public string TabNameJoin => "Joindre";

        public string TabNameSplit => "Séparer";

        public string TabNameHash => "Hacher";

        public string OutputFileCantBeUndefined => "Veuillez définir le fichier de sortie!";

        public string CanOverrideOutputFile => "Le fichier de sortie «{0}» existe déjà, voulez-vous le remplacer?";

        public string YouChooseNotToOverride => "Vous avez finalement choisi de ne pas écraser le fichier de sortie «{0}».";

        public string Override => "Ecraser?";

        public string HashInputFiles => "Fichier(s) à hacher:";

        public string UnitB => "o";

        public string UnitKB => "ko";

        public string UnitKiB => "Kio";

        public string UnitMB => "Mo";

        public string UnitMiB => "Mio";

        public string UnitGB => "Go";

        public string UnitGiB => "Gio";

        public string SplitSizeShouldBeDefined => "La taille de séparation doit être définie.";

        public string SplitSizeInvalid => "Taille de séparation «{0}» invalide, la valeur doit être un entier strictement positif.";

        public string SplitUnitShouldBeSelected => "Veuillez sélectionner une unité de séparation.";

        public string AllFiles => "Tous les fichiers";

        public string OpenFilesToHashTitle => "Sélectionnez des fichiers.";
    }
}