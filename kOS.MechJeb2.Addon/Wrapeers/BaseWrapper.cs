using System;
using kOS.MechJeb2.Addon.Core;
using kOS.MechJeb2.Addon.Utils;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;
using kOS.Safe.Utilities;
using KSPBuildTools;

namespace kOS.MechJeb2.Addon.Wrapeers
{
    [KOSNomenclature("BaseWrapper")]
    public abstract class BaseWrapper : Structure, IBaseWrapper, ILogContextProvider
    {
        protected PartModule MasterMechJeb => FlightGlobals.fetch.activeVessel.GetMasterMJ();

        public void Initialize()
        {
            BindObject();
            RegisterInitializer(InitializeSuffixes);
            Initialized = true;
        }

        public bool Initialized { get; protected set; }

        protected void AddSufixInternal(string name, Func<object, double> getter, object o, string description,
            params string[] aliases)
        {
            var suffix = new NoArgsSuffix<ScalarDoubleValue>(() => getter(o), description);

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

        protected void AddSufixInternal(string name, Delegate getter, object o, string description,
            params string[] aliases)
        {
            ISuffix suffix;

            if (getter is Func<object, double> gd)
                suffix = new NoArgsSuffix<ScalarDoubleValue>(() => gd(o), description);
            else if (getter is Func<object, int> gi)
                suffix = new NoArgsSuffix<ScalarIntValue>(() => gi(o), description);
            else if (getter is Func<object, float> gf)
                suffix = new NoArgsSuffix<ScalarDoubleValue>(() => gf(o),
                    description); // float → double
            else if (getter is Func<object, string> gs)
                suffix = new NoArgsSuffix<StringValue>(() => gs(o), description);
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

        protected virtual void BindObject()
        {
        }

        protected abstract void InitializeSuffixes();
        public abstract string context();

        protected MemberBinder Member(object target, string name)
            => new MemberBinder(target, name);

        /// <summary>
        /// Runs <paramref name="bind"/>, and if the MechJeb2 member it needs does not exist, logs a warning and
        /// returns <paramref name="missing"/> built with an error message instead. Suffixes bound to the fallback
        /// fail with that message when used, so a member renamed or removed in another MechJeb2 version disables
        /// only the suffixes that use it instead of the whole wrapper.
        /// </summary>
        protected static TResult BindOrMissing<TResult>(object target, string name, Func<TResult> bind,
            Func<string, TResult> missing)
        {
            try
            {
                return bind();
            }
            catch (MissingMemberException)
            {
                var member = $"{target.GetType().FullName}.{name}";
                Log.Warning($"{member} not found, suffixes that use it are disabled");
                return missing($"{member} is not available in the installed MechJeb2 version");
            }
        }

        protected class MemberBinder
        {
            private readonly object _target;
            private readonly string _name;

            public MemberBinder(object target, string name)
            {
                _target = target;
                _name = name;
            }

            public Func<object, T> GetField<T>()
                => BindOrMissing(_target, _name, () => Reflect.On(_target).Field(_name).AsGetter<T>(),
                    Getter<T>);

            public Action<object, T> SetField<T>()
                => BindOrMissing(_target, _name, () => Reflect.On(_target).Field(_name).AsSetter<T>(),
                    Setter<T>);

            public Func<object, T> GetProp<T>()
                => BindOrMissing(_target, _name, () => Reflect.On(_target).Property(_name).AsGetter<T>(),
                    Getter<T>);

            public Action<object, T> SetProp<T>()
                => BindOrMissing(_target, _name, () => Reflect.On(_target).Property(_name).AsSetter<T>(),
                    Setter<T>);

            private static Func<object, T> Getter<T>(string error) => _ => throw new KOSException(error);

            private static Action<object, T> Setter<T>(string error) => (_, __) => throw new KOSException(error);
        }
    }
}