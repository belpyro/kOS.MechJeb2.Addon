using System;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;
using kOS.Safe.Utilities;

namespace kOS.MechJeb2.Addon.kOS
{
    public class PredicatedSetSuffix<TValue> : SetSuffix<TValue> where TValue : Structure
    {
        private readonly Predicate<object> _predicate;
        private readonly object _instance;

        public PredicatedSetSuffix(Predicate<object> predicate, object instance, SuffixGetDlg<TValue> getter,
            SuffixSetDlg<TValue> setter, string description = "") : base(getter, setter, description)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            _instance = instance;
        }

        public override ISuffixResult Get()
        {
            return !_predicate(_instance) ? throw new KOSException("Predicate must be valid") : base.Get();
        }

        public override void Set(object value)
        {
            if (!_predicate(_instance)) throw new KOSException("Predicate must be valid");
            base.Set(value);
        }
    }
}