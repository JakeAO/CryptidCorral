using System;
using System.Collections.Generic;

namespace Core.Code.StateMachine
{
    public class Context
    {
        private readonly Dictionary<Type, object> _data = new Dictionary<Type, object>();

        public Context(params object[] initialData)
        {
            foreach (var o in initialData) _data[o.GetType()] = o;
        }

        public T Get<T>()
        {
            return TryGet(out T value) ? value : default;
        }

        public bool TryGet<T>(out T value)
        {
            value = default;

            if (_data.TryGetValue(typeof(T), out var objVal) &&
                objVal is T typedVal)
            {
                value = typedVal;
                return true;
            }

            return false;
        }

        public void Set<T>(T value, bool overwrite = true)
        {
            if (overwrite || !_data.TryGetValue(typeof(T), out var existingValue) || existingValue == default)
                _data[typeof(T)] = value;
        }

        public void Clear()
        {
            _data.Clear();
        }

        public void Clear<T>()
        {
            _data.Remove(typeof(T));
        }
    }
}