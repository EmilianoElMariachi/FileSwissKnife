using System.Collections.ObjectModel;

namespace FileSwissKnife.CustomControls.Error
{
    public class ErrorsCollection : ObservableCollection<ErrorViewModel>
    {
        protected override void InsertItem(int index, ErrorViewModel item)
        {
            if (Contains(item))
                return;
            base.InsertItem(index, item);
        }
    }
}