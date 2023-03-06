using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Data;

namespace Common.UI.WPF.Charts
{
    public class DataPointsList<T> : ListBase<T> where T : DataPoint
    {
        private Hashtable boundPoints;
        private BindingsList<BindingInfo> bindingsList;
        private IEnumerable itemsSource;
        private ItemsCollectionView itemsCollectionView;

        private bool isUsingItemsSource;
        internal bool IsUsingItemsSource
        {
            get
            {
                return isUsingItemsSource;
            }
        }

        public DataPointsList()
        {
        }

        public override T this[int index]
        {
            get
            {
                T item = default(T);
                if (!IsUsingItemsSource)
                {
                    item = base[index];
                }
                else
                {
                    object itemAt = itemsCollectionView.GetItemAt(index);
                    if (boundPoints[itemAt] == null)
                    {
                        var dataPoint = new DataPoint();
                        if (itemAt != null)
                        {
                            for (int i = 0; i < bindingsList.Count; i++)
                            {
                                var bindingInfo = bindingsList[i];
                                if (bindingInfo != null && bindingInfo.Binding != null)
                                {
                                    var binding = CopyBinding(bindingInfo.Binding);
                                    binding.Source = itemAt;
                                    switch (bindingInfo.PropertyName)
                                    {
                                        case DataPointPropertyName.X:
                                            {
                                                BindingOperations.SetBinding(dataPoint, DataPoint.XProperty, binding);
                                                break;
                                            }
                                        case DataPointPropertyName.Y:
                                            {
                                                BindingOperations.SetBinding(dataPoint, DataPoint.YProperty, binding);
                                                break;
                                            }
                                        case DataPointPropertyName.Label:
                                            {
                                                BindingOperations.SetBinding(dataPoint, DataPoint.LabelProperty, binding);
                                                break;
                                            }
                                    }
                                }
                            }
                            dataPoint.Content = itemAt;
                        }
                        boundPoints[itemAt] = dataPoint;
                    }
                    item = (T)boundPoints[itemAt];
                }
                return item;
            }
            set
            {
                if (IsUsingItemsSource)
                {
                    throw new InvalidOperationException("List is DataBound. Please, use ItemsSource instead.");
                }
                base[index] = value;
            }
        }

        internal void ClearItemsSource()
        {
            bindingsList = null;
            itemsSource = null;
            isUsingItemsSource = false;
            SetCollectionView(null);
        }

        private Binding CopyBinding(Binding original)
        {
            var binding = new Binding()
            {
                Converter = original.Converter,
                ConverterParameter = original.ConverterParameter,
                ConverterCulture = original.ConverterCulture,
                FallbackValue = original.FallbackValue,
                IsAsync = original.IsAsync,
                Mode = original.Mode,
                NotifyOnSourceUpdated = original.NotifyOnSourceUpdated,
                NotifyOnTargetUpdated = original.NotifyOnTargetUpdated,
                NotifyOnValidationError = original.NotifyOnValidationError,
                Path = original.Path,
                UpdateSourceExceptionFilter = original.UpdateSourceExceptionFilter,
                UpdateSourceTrigger = original.UpdateSourceTrigger,
                XPath = original.XPath
            };

            foreach (ValidationRule validationRule in original.ValidationRules)
            {
                binding.ValidationRules.Add(validationRule);
            }

            return binding;
        }

        protected override int GetCount()
        {
            return (IsUsingItemsSource ? itemsCollectionView.Count : base.GetCount());
        }

        public override IEnumerator<T> GetEnumerator()
        {
            return new DataPointEnumerator<T>(this);
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        private void OnCollectionViewChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChanged(e);
        }

        internal void SetBindings(BindingsList<BindingInfo> bindings)
        {
            bindingsList = bindings;
        }

        internal void SetCollectionView(ItemsCollectionView view)
        {
            if (itemsCollectionView != view)
            {
                if (itemsCollectionView != null)
                {
                    boundPoints = null;
                    if (view == null)
                    {
                        foreach (T t in this)
                        {
                            DataPoint dataPoint = t;
                            BindingOperations.ClearBinding(dataPoint, DataPoint.XProperty);
                            BindingOperations.ClearBinding(dataPoint, DataPoint.YProperty);
                            BindingOperations.ClearBinding(dataPoint, DataPoint.LabelProperty);
                        }
                    }
                    itemsCollectionView.ChangedEvent -= new NotifyCollectionChangedEventHandler(OnCollectionViewChanged);
                }
                if (view != null)
                {
                    boundPoints = new Hashtable();
                    view.ChangedEvent += new NotifyCollectionChangedEventHandler(OnCollectionViewChanged);
                }
                itemsCollectionView = view;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        internal void SetItemsSource(IEnumerable value)
        {
            if (!IsUsingItemsSource && itemsSource != null)
            {
                throw new InvalidOperationException("value");
            }

            itemsSource = value;
            isUsingItemsSource = true;
            SetCollectionView(new ItemsCollectionView(itemsSource));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
