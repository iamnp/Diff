using System.Collections.Generic;

namespace Diff.Expressions.LowLevel
{
    public class Variable
    {
        private Data _data;

        public double AsDouble => _data.DoubleValue.Value;

        public bool AsBool => _data.BoolValue.Value;

        public bool IsEmpty => !IsDouble && !IsBool && !IsArray;

        public string Name { get; private set; }

        public Variable Parent { get; private set; }

        public bool IsDouble => _data.DoubleValue != null;
        public bool IsBool => _data.BoolValue != null;

        public bool IsArray => _data.Array != null;

        public Variable NthItem(int n)
        {
            if (_data.Array == null)
            {
                _data.Array = new List<Variable>();
            }

            while (_data.Array.Count < n + 1)
            {
                _data.Array.Add(null);
            }

            if (n < 0)
            {
                return this;
            }

            return _data.Array[n] == null ? _data.Array[n] = Empty(null, this) : _data.Array[n];
        }

        public static Variable Const(double constant)
        {
            return new Variable {_data = new Data {DoubleValue = constant}};
        }

        public static Variable Const(bool constant)
        {
            return new Variable {_data = new Data {BoolValue = constant}};
        }

        public static Variable Empty(string name = null, Variable parent = null)
        {
            return new Variable {Name = name, Parent = parent, _data = new Data()};
        }

        public Variable CopyValue(Variable another)
        {
            _data.CopyValue(another._data);
            return this;
        }

        private class Data
        {
            public List<Variable> Array;
            public bool? BoolValue;
            public double? DoubleValue;

            public void CopyValue(Data another)
            {
                DoubleValue = another.DoubleValue;
                BoolValue = another.BoolValue;
                Array = another.Array;
            }
        }
    }
}