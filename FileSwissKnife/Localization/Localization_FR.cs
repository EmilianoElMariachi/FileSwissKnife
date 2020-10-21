namespace FileSwissKnife.Localization
{
    public class Localization_FR : ILocalization
    {
        public string CultureName => "fr-FR";

        public string DragMeSomeFilesToJoin => "Glisse moi un ou plusieurs fichiers à joindre par ici :)";

        public string Start => "Go!";

        public string Cancelling => "Annulation...";

        public string Cancel => "Annuler";

        public string OperationCanceled => "Annulé";

        public string OperationFinishedIn => "Terminé en {0}";

        public string OperationError => "Erreur!";

        public string JoinInputFiles => "Fichiers à joindre:";

        public string JoinOutputFile => "Fichier de sortie:";

        public string TabNameJoin => "Joindre";

        public string TabNameSplit => "Séparer";

        public string TabNameHash => "Hacher";

        public string HideError => "Masquer";

        public string OutputFileCantBeUndefined => "Veuillez définir le fichier de sortie!";

        public string CanOverrideOutputFile => "Le fichier de sortie «{0}» existe déjà, voulez-vous le remplacer?";

        public string YouChooseNotToOverride => "Vous avez finalement choisi de ne pas écraser le fichier de sortie «{0}».";

        public string Override => "Ecraser?";
    }
}