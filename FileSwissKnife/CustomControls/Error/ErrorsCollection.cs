using System.Collections.ObjectModel;

namespace FileSwissKnife.CustomControls.Error
{
    public class ErrorsCollection : ObservableCollection<ErrorViewModel>
    {
        protected override void InsertItem(int index, ErrorViewModel item)
        {
            if (this.Contains(item))
                return;
            base.InsertItem(index, item);
        }
    }
}