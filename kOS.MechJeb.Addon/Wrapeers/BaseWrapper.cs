using System;
using kOS.MechJeb.Addon.Core;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Utilities;

namespace kOS.MechJeb.Addon.Wrapeers
{
    [KOSNomenclature("BaseWrapper")]
    public abstract class BaseWrapper : Structure, IBaseWrapper
    {
        protected object CoreInstance { get; private set; }
        public bool Initialized { get; protected set; }

        public virtual void Initialize(object coreInstance)
        {
            if (Initialized) return;
            CoreInstance = coreInstance;
            Initialized = true;
        }
        
        protected void AddSufixInternal(string name, Func<object, double> getter, string description,
            params string[] aliases)
        {
            var suffix = new NoArgsSuffix<ScalarDoubleValue>(() => getter(CoreInstance), description);

            if (aliases != null && aliases.Length > 0)
            {
                var names = new string[1 + aliases.Length];
                names[0] = name;
                for (int i = 0; i < aliases.Length; i++)
                    names[i + 1] = aliases[i];

                AddSuffix(names, suffix);
            }
            else
            {
                AddSuffix(name, suffix);
            }
        }
        
        protected void AddSufixInternal(string name, Delegate getter, string description, params string[] aliases)
        {
            ISuffix suffix;

            if (getter is Func<object, double> gd)
                suffix = new NoArgsSuffix<ScalarDoubleValue>(() => gd(CoreInstance), description);
            else if (getter is Func<object, int> gi)
                suffix = new NoArgsSuffix<ScalarIntValue>(() => gi(CoreInstance), description);
            else if (getter is Func<object, float> gf)
                suffix = new NoArgsSuffix<ScalarDoubleValue>(() => gf(CoreInstance),
                    description); // float → double
            else if (getter is Func<object, string> gs)
                suffix = new NoArgsSuffix<StringValue>(() => gs(CoreInstance), description);
            else
                return;

            if (aliases != null && aliases.Length > 0)
            {
                var names = new string[aliases.Length + 1];
                names[0] = name;
                for (int i = 0; i < aliases.Length; i++)
                    names[i + 1] = aliases[i];

                AddSuffix(names, suffix);
            }
            else
            {
                AddSuffix(name, suffix);
            }
        }

    }
}