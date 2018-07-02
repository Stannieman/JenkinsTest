using System;
using System.Collections.Generic;

namespace Stannieman.DI.UnitTests.TestTypes
{
    public interface INonDisposableType { }

    public class NonDisposableType : INonDisposableType
    {
        public INonDisposableType_2 NonDisposableType_2 { get; set; }

        public IDisposableType_1 DisposableType_1 { get; }

        public NonDisposableType(IDisposableType_1 _disposableType_1)
        {
            DisposableType_1 = _disposableType_1;
        }
    }


    public interface IDisposableType { }

    public class DisposableType : IDisposableType, IDisposable
    {
        public INonDisposableType_2 NonDisposableType_2 { get; set; }

        public IDisposableType_1 DisposableType_1 { get; }

        public bool Disposed { get; private set; } = false;

        public DisposableType(IDisposableType_1 _disposableType_1)
        {
            DisposableType_1 = _disposableType_1;
        }

        public void Dispose()
        {
            Disposed = true;
        }
    }


    public interface IDisposableType_1 : IDisposable { }

    public class DisposableType_1 : IDisposableType_1
    {
        public bool Disposed { get; private set; } = false;

        public void Dispose()
        {
            Disposed = true;
        }
    }


    public interface INonDisposableType_2 { }

    public class NonDisposableType_2 : INonDisposableType_2
    {
        public IEnumerable<IDisposableType_2_2> DisposableType_2_2s { get; set; }

        public IDisposableType_2_1 DisposableType_2_1 { get; }

        public NonDisposableType_2(IDisposableType_2_1 disposableType_2_1)
        {
            DisposableType_2_1 = disposableType_2_1;
        }
    }


    public interface IDisposableType_2_1 { }

    public class DisposableType_2_1 : IDisposableType_2_1, IDisposable
    {
        public bool Disposed { get; private set; } = false;

        public void Dispose()
        {
            Disposed = true;
        }
    }


    public interface IDisposableType_2_2 : IDisposable { }

    public class DisposableType_2_2Impl1 : IDisposableType_2_2
    {
        public bool Disposed { get; private set; } = false;

        public void Dispose()
        {
            Disposed = true;
        }
    }

    public class DisposableType_2_2Impl2 : IDisposableType_2_2
    {
        public bool Disposed { get; private set; } = false;

        public void Dispose()
        {
            Disposed = true;
        }
    }
}
