using System;
using System.Reflection;
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
        // Cached reflection info for getting fresh MasterMechJeb
        private static MethodInfo _getMasterMechJebMethod;
        private static bool _reflectionInitialized;

        // Cached MasterMechJeb reference - may become stale after save reload or vessel switch
        private PartModule _cachedMasterMechJeb;
        // Track which vessel the cached MasterMechJeb belongs to
        private Vessel _cachedVessel;

        /// <summary>
        /// Gets MasterMechJeb, automatically refreshing from FlightGlobals.ActiveVessel if the cached instance is stale.
        /// This handles:
        /// - Save reloads where the old MasterMechJeb becomes a destroyed Unity object ("fake null")
        /// - Vessel switches where we need MechJeb from the new active vessel
        /// </summary>
        protected PartModule MasterMechJeb
        {
            get
            {
                var activeVessel = FlightGlobals.ActiveVessel;

                // Check if cached reference is still valid AND belongs to the current active vessel
                if (_cachedMasterMechJeb != null && _cachedVessel != null)
                {
                    try
                    {
                        // Check if the cached vessel is still the active vessel
                        // Using ReferenceEquals for identity check, then validate both aren't destroyed
                        if (ReferenceEquals(_cachedVessel, activeVessel) &&
                            _cachedVessel.ToString() != "null" &&
                            _cachedMasterMechJeb.ToString() != "null")
                        {
                            return _cachedMasterMechJeb;
                        }
                    }
                    catch
                    {
                        // Fall through to get fresh instance - object was destroyed
                    }
                }

                // MasterMechJeb is stale, wrong vessel, or null - get a fresh one
                _cachedMasterMechJeb = GetFreshMasterMechJeb();
                _cachedVessel = activeVessel;
                return _cachedMasterMechJeb;
            }
        }

        /// <summary>
        /// Gets a fresh MasterMechJeb from FlightGlobals.ActiveVessel using reflection.
        /// </summary>
        private static PartModule GetFreshMasterMechJeb()
        {
            if (!_reflectionInitialized)
            {
                var vesselExtensionType = Constants.VesselExtensionName.GetTypeFromCache();
                if (vesselExtensionType != null)
                {
                    _getMasterMechJebMethod = vesselExtensionType.GetMethod("GetMasterMechJeb",
                        BindingFlags.Public | BindingFlags.Static);
                }
                _reflectionInitialized = true;
            }

            if (_getMasterMechJebMethod == null)
                return null;

            var vessel = FlightGlobals.ActiveVessel;
            if (vessel == null)
                return null;

            // Check if vessel is valid (not a destroyed Unity object)
            try
            {
                var _ = vessel.vesselName;
            }
            catch
            {
                return null;
            }

            try
            {
                return _getMasterMechJebMethod.Invoke(null, new object[] { vessel }) as PartModule;
            }
            catch
            {
                return null;
            }
        }

        public void Initialize()
        {
            BindObject();
            RegisterInitializer(InitializeSuffixes);
            Initialized = true;
        }

        /// <summary>
        /// Reinitialize the wrapper, clearing any cached references.
        /// Called when GameEvents signal that the scene or vessel has changed.
        /// </summary>
        public virtual void Reinitialize()
        {
            _cachedMasterMechJeb = null;
            _cachedVessel = null;
            Initialized = false;
            Initialize();
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
                => Reflect.On(_target).Field(_name).AsGetter<T>();

            public Action<object, T> SetField<T>()
                => Reflect.On(_target).Field(_name).AsSetter<T>();

            public Func<object, T> GetProp<T>()
                => Reflect.On(_target).Property(_name).AsGetter<T>();

            public Action<object, T> SetProp<T>()
                => Reflect.On(_target).Property(_name).AsSetter<T>();
        }
    }
}
