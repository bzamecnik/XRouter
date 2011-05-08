using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace XRouter.Utils
{
    public class GuardedCollection<T> : Collection<T>
    {
        private Action<T> ItemAdding { get; set; }
        private Action<T> ItemRemoving { get; set; }

        public GuardedCollection(Action<T> itemAdding = null, Action<T> itemRemoving = null)
        {
            ItemAdding = itemAdding;
            ItemRemoving = itemRemoving;
        }

        protected override void InsertItem(int index, T item)
        {
            OnItemAdding(item);
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, T item)
        {
            OnItemRemoving(Items[index]);
            OnItemAdding(item);
            base.SetItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            OnItemRemoving(Items[index]);
            base.RemoveItem(index);
        }

        protected override void ClearItems()
        {
            foreach (var item in Items) {
                OnItemRemoving(item);
            }
            base.ClearItems();
        }

        protected virtual void OnItemAdding(T item)
        {
            if (ItemAdding != null) {
                ItemAdding(item);
            }
        }

        protected virtual void OnItemRemoving(T item)
        {
            if (ItemRemoving != null) {
                ItemRemoving(item);
            }
        }
    }
}
