using Interpreter.Framework.Extern.Utils;
using Interpreter.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Framework.Extern.Mapping
{
    /// <summary>
    /// Wrapper class for wrapping a language implemented array
    /// </summary>
    public class ArrayAdapter : IArrayAdapter
    {
        private readonly IArrayOperable _srcArrayReference;
        internal protected readonly Type _externType;

        public IArrayOperable OperableReference => _srcArrayReference;
        internal protected IOperable[] InnerArray => _srcArrayReference.GetArray();

        public int Length => _srcArrayReference.GetArray().Length;

        public object this[int index]
        {
            get => TypeMappingHelper.ToExternValue(InnerArray[index], _externType);
            set => InnerArray[index] = TypeMappingHelper.ToClientValue(value);
        }

        internal protected ArrayAdapter(IArrayOperable reference, Type externType)
        {
            _srcArrayReference = reference;
            _externType = externType;
        }

        public IEnumerator GetEnumerator() => new ArrayAdapterEnumerator(InnerArray, _externType);

        public T ElementAt<T>(int index) => (T)TypeMappingHelper.ToExternValue(InnerArray[index], typeof(T));
    }

    /// <summary>
    /// <inheritdoc cref="ArrayAdapter"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ArrayAdapter<T> : ArrayAdapter, IArrayAdapter<T>
    {
        public new T this[int index]
        {
            get => (T)TypeMappingHelper.ToExternValue(InnerArray[index], typeof(T));
            set => InnerArray[index] = TypeMappingHelper.ToClientValue(value);
        }

        internal protected ArrayAdapter(IArrayOperable reference) : base(reference, typeof(T))
        {
        }

        public new IEnumerator<T> GetEnumerator() => new ArrayAdapterEnumerator<T>(InnerArray);
    }

    internal class ArrayAdapterEnumerator : IEnumerator
    {
        private readonly Type _targetType;
        private readonly IEnumerator _src;

        public object Current => TypeMappingHelper.ToExternValue((IOperable)_src.Current, _targetType);

        public ArrayAdapterEnumerator(IOperable[] src, Type targetType)
        {
            _src = src.GetEnumerator();
            _targetType = targetType;
        }

        public bool MoveNext() => _src.MoveNext();

        public void Reset() => _src.Reset();
    }

    internal class ArrayAdapterEnumerator<T> : IEnumerator<T>
    {
        private bool _isDisposed = false;
        private IEnumerator<IOperable> _src;

        public T Current => (T)(this as IEnumerator).Current;

        object IEnumerator.Current => TypeMappingHelper.ToExternValue(_src.Current, typeof(T));

        public ArrayAdapterEnumerator(IOperable[] src)
        {
            _src = (IEnumerator<IOperable>)src.GetEnumerator();
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _src.Dispose();
                _src = null;
                _isDisposed = true;
            }
        }

        public bool MoveNext() => _src.MoveNext();

        public void Reset() => _src.Reset();
    }
}
