using System;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;

namespace kOS.MechJeb2.Addon.kOS
{
    /// <summary>
    /// Custom four-argument suffix since kOS only provides up to ThreeArgsSuffix.
    /// Used for operations like ELLIPTICIZETIMED(pe, ap, timeRef, seconds).
    /// </summary>
    public class FourArgsSuffix<TReturn, TParam1, TParam2, TParam3, TParam4> : SuffixBase
        where TReturn : Structure
        where TParam1 : Structure
        where TParam2 : Structure
        where TParam3 : Structure
        where TParam4 : Structure
    {
        private readonly Del<TReturn, TParam1, TParam2, TParam3, TParam4> del;

        public delegate TInnerReturn Del<out TInnerReturn, in TInnerParam1, in TInnerParam2, in TInnerParam3, in TInnerParam4>(
            TInnerParam1 one, TInnerParam2 two, TInnerParam3 three, TInnerParam4 four);

        public FourArgsSuffix(Del<TReturn, TParam1, TParam2, TParam3, TParam4> del, string description = "")
            : base(description)
        {
            this.del = del;
        }

        protected override object Call(object[] args)
        {
            return (TReturn)del((TParam1)args[0], (TParam2)args[1], (TParam3)args[2], (TParam4)args[3]);
        }

        protected override Delegate Delegate
        {
            get { return del; }
        }
    }

    /// <summary>
    /// Void-returning version of FourArgsSuffix.
    /// </summary>
    public class FourArgsSuffix<TParam1, TParam2, TParam3, TParam4> : SuffixBase
        where TParam1 : Structure
        where TParam2 : Structure
        where TParam3 : Structure
        where TParam4 : Structure
    {
        private readonly Del<TParam1, TParam2, TParam3, TParam4> del;

        public delegate void Del<in TInnerParam1, in TInnerParam2, in TInnerParam3, in TInnerParam4>(
            TInnerParam1 one, TInnerParam2 two, TInnerParam3 three, TInnerParam4 four);

        public FourArgsSuffix(Del<TParam1, TParam2, TParam3, TParam4> del, string description = "")
            : base(description)
        {
            this.del = del;
        }

        protected override object Call(object[] args)
        {
            del((TParam1)args[0], (TParam2)args[1], (TParam3)args[2], (TParam4)args[3]);
            return null;
        }

        protected override Delegate Delegate
        {
            get { return del; }
        }
    }
}
