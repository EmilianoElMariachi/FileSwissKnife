namespace FileSwissKnife.CustomControls.Error
{
    public class StaticErrorViewModel : ErrorViewModel
    {
        private readonly ErrorsCollection _errors;

        public StaticErrorViewModel(ErrorsCollection errors)
        {
            _errors = errors;
        }

        public bool IsFixed { get; set; }

        public void Show(string newMessage)
        {
            _errors.Add(this);
            this.Message = newMessage;
        }        
        
        public void Show()
        {
            _errors.Add(this);
        }

        public void Hide()
        {
            _errors.Remove(this);
        }

    }
}