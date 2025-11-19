using System;
using kOS.Safe;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Utilities;

namespace kOS.MechJeb2.Addon.KOSextensions
{
    public class FourArgsSuffix<TReturn, TParam1, TParam2, TParam3, TParam4> : SuffixBase
        where TReturn : Structure
        where TParam1 : Structure
        where TParam2 : Structure
        where TParam3 : Structure
        where TParam4 : Structure
    {
        public delegate TInnerReturn Del<out TInnerReturn, in TInnerParam, in TInnerParam2, in TInnerParam3,
            in TInnerParam4>(TInnerParam one, TInnerParam2 two, TInnerParam3 three, TInnerParam4 four);

        private Del<TReturn, TParam1, TParam2, TParam3, TParam4> _del;

        public FourArgsSuffix(Del<TReturn, TParam1, TParam2, TParam3, TParam4> del, string description) : base(
            description)
        {
            _del = del;
        }

        protected override object Call(object[] args) =>
            (TReturn)_del((TParam1)args[0], (TParam2)args[1], (TParam3)args[2], (TParam4)args[3]);


        protected override Delegate Delegate => _del;
    }

    public class FourArgsSuffix<TParam1, TParam2, TParam3, TParam4> : SuffixBase
        where TParam1 : Structure
        where TParam2 : Structure
        where TParam3 : Structure
        where TParam4 : Structure
    {
        public delegate void Del<in TInnerParam, in TInnerParam2, in TInnerParam3,
            in TInnerParam4>(TInnerParam one, TInnerParam2 two, TInnerParam3 three, TInnerParam4 four);

        private Del<TParam1, TParam2, TParam3, TParam4> _del;

        public FourArgsSuffix(string description, Del<TParam1, TParam2, TParam3, TParam4> del) : base(description)
        {
            _del = del;
        }

        protected override object Call(object[] args)
        {
            _del((TParam1)args[0], (TParam2)args[1], (TParam3)args[2], (TParam4)args[3]);
            return null;
        }

        protected override Delegate Delegate => _del;
    }
}