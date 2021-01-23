﻿// MSIX Hero
// Copyright (C) 2021 Marcin Otorowski
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// Full notice:
// https://github.com/marcinotorowski/msix-hero/blob/develop/LICENSE.md

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Otor.MsixHero.App.Mvvm.Changeable
{
    public class ChangeableCollection<T> : ObservableCollection<T>, IChangeableValue
    {
        private bool isTouched, isDirty;

        private bool monitorChildren = true;

        private List<T> originalItems = new List<T>();
        
        public ChangeableCollection()
        {
            this.AssertType();
        }

        public ChangeableCollection(IEnumerable<T> collection)
        {
            this.AssertType();
            try
            {
                this.monitorChildren = false;
                this.AddRange(collection);
                this.originalItems = this.ToList();
            }
            finally
            {
                this.monitorChildren = true;
            }
        }

        public ChangeableCollection(params T[] collection) : this((IEnumerable<T>)collection)
        {
        }

        public event EventHandler<EventArgs> Changed;

        public event EventHandler<ValueChangedEventArgs> ValueChanged;
        
        public event EventHandler<ValueChangingEventArgs> ValueChanging;

        protected override void ClearItems()
        {
            base.ClearItems();
            this.IsTouched = true;
            this.IsDirty = !this.originalItems.SequenceEqual(this);
            this.Changed?.Invoke(this, new EventArgs());
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            this.Changed?.Invoke(this, new EventArgs());

            if (item is IChangeableValue newChangeableValue)
            {
                newChangeableValue.ValueChanged -= this.OnItemValueChanged;
                newChangeableValue.ValueChanged += this.OnItemValueChanged;
            }

            if (item is IChangeable newChangeable)
            {
                newChangeable.IsTouchedChanged -= this.OnIsTouchedChanged;
                newChangeable.IsTouchedChanged += this.OnIsTouchedChanged;
            }

            if (!this.monitorChildren)
            {
                return;
            }

            this.IsTouched = true;

            if (item is IChangeable changeableItem)
            {
                var isNewDirty = changeableItem.IsDirty;
                this.IsDirty = isNewDirty || !this.originalItems.SequenceEqual(this);
            }
            else
            {
                this.IsDirty = !this.originalItems.SequenceEqual(this);
            }
        }

        private void OnIsTouchedChanged(object sender, ValueChangedEventArgs<bool> e)
        {
            if (e.NewValue)
            {
                this.IsTouched = true;
            }
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            base.MoveItem(oldIndex, newIndex);
            this.Changed?.Invoke(this, new EventArgs());

            if (!this.monitorChildren)
            {
                return;
            }

            this.IsTouched = true;
            this.IsDirty = !this.originalItems.SequenceEqual(this);
        }

        protected override void RemoveItem(int index)
        {
            var item = this[index];
            base.RemoveItem(index);
            this.Changed?.Invoke(this, new EventArgs());

            if (item is IChangeableValue oldChangeableValue)
            {
                oldChangeableValue.ValueChanged -= this.OnItemValueChanged;
            }

            if (item is IChangeable oldChangeable)
            {
                oldChangeable.IsTouchedChanged -= this.OnIsTouchedChanged;
            }

            if (!this.monitorChildren)
            {
                return;
            }

            this.IsTouched = true;

            if (item is IChangeable)
            {
                this.IsDirty = !this.originalItems.SequenceEqual(this) || this.OfType<IChangeable>().Any(x => x.IsDirty);
            }
            else
            {
                this.IsDirty = !this.originalItems.SequenceEqual(this);
            }
        }

        protected override void SetItem(int index, T item)
        {
            var oldItem = this[index];
            base.SetItem(index, item);
            this.Changed?.Invoke(this, new EventArgs());

            if (oldItem is IChangeableValue oldChangeableValue)
            {
                oldChangeableValue.ValueChanged -= this.OnItemValueChanged;
            }

            if (item is IChangeableValue newChangeableValue)
            {
                newChangeableValue.ValueChanged -= this.OnItemValueChanged;
            }

            if (oldItem is IChangeableValue oldChangeable)
            {
                oldChangeable.IsTouchedChanged -= this.OnIsTouchedChanged;
            }

            if (item is IChangeableValue newChangeable)
            {
                newChangeable.IsTouchedChanged += this.OnIsTouchedChanged;
            }

            if (!this.monitorChildren)
            {
                return;
            }

            this.IsTouched = true;

            if (item is IChangeable changeableItem)
            {
                var oldIsDirty = ((IChangeable) oldItem).IsDirty;
                var newIsDirty = changeableItem.IsDirty;
                if (oldIsDirty != newIsDirty)
                {
                    this.IsDirty = newIsDirty || !this.originalItems.SequenceEqual(this);
                }
            }
            else
            {
                this.IsDirty = !this.originalItems.SequenceEqual(this);
            }
        }

        private void OnItemValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (!this.monitorChildren)
            {
                return;
            }

            this.IsTouched = true;
            var actualSender = (IChangeable) sender;
            if (actualSender != null)
            {
                if (actualSender.IsDirty)
                {
                    this.IsDirty = true;
                }
                else
                {
                    this.IsDirty = !this.originalItems.SequenceEqual(this) || this.OfType<IChangeable>().Any(i => i.IsDirty);
                }
            }

            this.Changed?.Invoke(this, new EventArgs());
        }

        public bool IsDirty
        {
            get => this.isDirty;
            private set
            {
                if (!this.SetField(ref this.isDirty, value))
                {
                    return;
                }

                var isDirtyChanged = this.IsDirtyChanged;
                if (isDirtyChanged != null)
                {
                    isDirtyChanged(this, new ValueChangedEventArgs<bool>(value));
                }
            }
        }

        public bool IsTouched
        {
            get => this.isTouched;
            private set
            {
                if (!this.SetField(ref this.isTouched, value))
                {
                    return;
                }

                var isTouchedChanged = this.IsTouchedChanged;
                if (isTouchedChanged != null)
                {
                    isTouchedChanged(this, new ValueChangedEventArgs<bool>(value));
                }
            }
        }

        public void Commit()
        {
            try
            {
                this.monitorChildren = false;

                foreach (var item in this.OfType<IChangeable>())
                {
                    item.Commit();
                }
            }
            finally
            {
                this.monitorChildren = true;
                this.IsDirty = false;
                this.IsTouched = false;
                this.originalItems = this.ToList();
            }
        }

        public void Reset(ValueResetType resetType = ValueResetType.Hard)
        {
            try
            {
                this.monitorChildren = false;

                this.ClearItems();
                if (this.originalItems?.Any() == true)
                {
                    this.AddRange(this.originalItems);

                    foreach (var item in this.originalItems.OfType<IChangeable>())
                    {
                        item.Reset();
                    }
                }
            }
            finally
            {
                this.monitorChildren = true;
            }

            this.IsDirty = false;

            if (resetType == ValueResetType.Hard)
            {
                this.IsTouched = false;
            }
        }

        public void Touch()
        {
            this.IsTouched = true;
            this.Changed?.Invoke(this, new EventArgs());
        }

        public event EventHandler<ValueChangedEventArgs<bool>> IsDirtyChanged;

        public event EventHandler<ValueChangedEventArgs<bool>> IsTouchedChanged;
        
        protected bool SetField<T1>(ref T1 field, T1 value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T1>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            return true;
        }
        private void AssertType()
        {
            if (typeof(IChangeable).IsAssignableFrom(typeof(T)))
            {
                return;
            }

            if (typeof(T) == typeof(string))
            {
                return;
            }

            if (typeof(T).IsPrimitive || typeof(T).IsValueType)
            {
                return;
            }

            throw new NotSupportedException("Objects supported by this class must be strings, value types or they must implement IChangeable interface.");
        }
    }
}
