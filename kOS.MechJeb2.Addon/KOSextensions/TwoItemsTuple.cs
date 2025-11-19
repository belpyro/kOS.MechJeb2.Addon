using System;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Serialization;
using kOS.Safe.Utilities;

namespace kOS.MechJeb2.Addon.KOSextensions
{
    [KOSNomenclature("TwoItemsTuple", CSharpToKOS = false, KOSToCSharp = false)]
    public abstract class TwoItemsTuple<TItem1, TItem2> : SerializableStructure where TItem1 : Structure where TItem2 : Structure
    {
        private Tuple<TItem1, TItem2> _tuple;

        private TwoItemsTuple()
        {
            RegisterInitializer(InitializeSuffixes);
        }

        protected TwoItemsTuple(TItem1 item1, TItem2 item2) : this()
        {
            _tuple = new Tuple<TItem1, TItem2>(item1, item2);
        }
        
        // Required for all IDumpers for them to work, but can't enforced by the interface because it's static:
        // public static TwoItemsTuple<TItem1,TItem2> CreateFromDump(SafeSharedObjects shared, Dump d)
        // {
        //     var newObj = new TwoItemsTuple();
        //     newObj.LoadDump(d);
        //     return newObj;
        // }

        
        private void InitializeSuffixes()
        {
            AddSuffix("ITEM1", new NoArgsSuffix<TItem1>(() => _tuple.Item1, "Returns 1st item from tupple"));
            AddSuffix("ITEM2", new NoArgsSuffix<TItem2>(() => _tuple.Item2, "Returns 2d item from tupple"));
        }
    }
}