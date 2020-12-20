using System;
using System.Collections.Specialized;
using System.Windows.Controls;

namespace FileSwissKnife.CustomControls
{
    public class ItemsControlEx : ItemsControl
    {

        public event EventHandler<NotifyCollectionChangedEventArgs>? ItemsChanged;

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            ItemsChanged?.Invoke(this, e);
        }

    }
}
