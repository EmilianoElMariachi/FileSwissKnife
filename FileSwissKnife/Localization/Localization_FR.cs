namespace FileSwissKnife.Localization
{
    public class Localization_FR : ILocalization
    {
        public string DisplayName => "Français";

        public string CultureName => "fr-FR";

        public string DragMeSomeFilesToJoin => "Glisse moi un ou plusieurs fichiers à joindre par ici :)";

        public string DragMeSomeFilesToHash => "Glisse moi un ou plusieurs fichiers à hacher par ici :)";

        public string DragMeSomeFileToSplit => "Glisse moi un fichier à séparer par ici :)";

        public string CancellingHash => "annulation";

        public string CancelJoin => "Annuler";

        public string ConfirmCloseCancelHash => "Voulez-vous vraiment fermer et annuler ?";

        public string ConfirmCloseCancelHashTitle => "Fermer et annuler ?";

        public string HashInProgress => "en cours...";

        public string HashCanceled => "annulé.";

        public string OperationCanceled => "Annulé";

        public string OperationFinishedIn => "Terminé en {0}";

        public string JoinInputFiles => "Fichiers à joindre:";

        public string JoinOutputFile => "Fichier de sortie:";

        public string SplitInputFile => "Fichier à séparer:";

        public string SplitOutputDir => "Répertoire de sortie:";

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

        public string BrowseFilesToHashTitle => "Sélection des fichiers d'entrée";

        public string CopyErrors => "Copier le(s) message(s) d'erreur";

        public string BrowseSplitOutputDirTitle => "Sélection du répertoire de sortie";

        public string BrowseFileToSplitTitle => "Sélection du fichier d'entrée";

        public string SplitNumPosBeforeBaseName => "Avant le nom";

        public string SplitNumPosAfterBaseName => "Après le nom";

        public string SplitNumPosBeforeExt => "Avant l'extension";

        public string SplitNumPosAfterExt => "Après l'extension";

        public string SplitErrorFileSizeLessThanSplitSize => "La taille de séparation ({0}) est plus grande que la taille du fichier spécifié ({1}).";

        public string SplitNumPosShouldBeSelected => "La position de la numérotation doit être sélectionnée.";

        public string SplitStartNumberShouldBeDefined => "Le numéro de départ doit être défini.";

        public string SplitStartNumberInvalid => "Le numéro de départ ({0}) n'est pas un entier valide.";

        public string SplitInputFileNotFound => "Le fichier «{0}» n'a pas été trouvé.";

        public string SplitNamingOptions => "Options de nommage";

        public string SplitNamingPreview => "Aperçu";

        public string SplitNamingNumPrefix => "Préfixe du numéro:";

        public string SplitNamingStartNum => "Numéro de départ:";

        public string SplitNamingPadWithZeros => "Remplir avec des 0:";

        public string SplitNamingNumSuffix => "Suffixe du numéro:";

        public string SplitNamingNumPos => "Position du numéro:";

        public string BrowseJoinOutputFileTitle => "Emplacement du fichier de sortie";

        public string BrowseJoinInputFilesTitle => "Sélection des fichiers d'entrée";

        public string TabNameSettings => "Paramètres";

        public string Language => "Langue:";

        public string BuffersSize => "Taille des buffers:";

        public string JoinBufferSize => "Joindre";

        public string SplitBufferSize => "Séparer";

        public string HashBufferSize => "Hacher";

        public string Theme => "Thème";
    }
}