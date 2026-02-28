using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.underdogg.uniext.Runtime.Extensions
{
    public interface IObservable<T>
    {
        event Action<T> OnChanged;
        T Value { get; set; }
        bool SetValue(T value, bool forceNotify = false);
    }

    [Serializable]
    public class ObservableVariable<T> : IObservable<T>
    {
        [SerializeField] private T _value;

        public event Action<T> OnChanged;

        public T Value
        {
            get => _value;
            set => SetValue(value);
        }

        public ObservableVariable() =>
            _value = default;

        public ObservableVariable(T defaultValue) =>
            _value = defaultValue;

        public bool SetValue(T value, bool forceNotify = false)
        {
            var changed = !EqualityComparer<T>.Default.Equals(_value, value);
            if (!changed && !forceNotify)
                return false;

            _value = value;
            OnChanged?.Invoke(value);
            return changed;
        }

        public override string ToString() =>
            _value?.ToString() ?? string.Empty;
    }
}
