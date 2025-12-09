using System.Collections.Generic;
using System.Linq;
using kOS.MechJeb2.Addon.Core;
using kOS.MechJeb2.Addon.Wrapeers;
using kOS.Safe.Encapsulation;

namespace kOS.MechJeb2.Addon
{
    /// <summary>
    /// Controller that manages MechJeb wrapper instances.
    /// Subscribes to GameEvents to detect save reloads and vessel changes,
    /// ensuring that stale MechJeb references are automatically refreshed.
    /// </summary>
    public class MechJebController
    {
        private readonly Dictionary<WrapperTypes, IBaseWrapper> _wrappers;
        private static bool _eventsSubscribed;

        /// <summary>
        /// Set to true when GameEvents fire, signaling that cached references may be stale.
        /// Addon should check this and call ForceReinitialize() if true.
        /// </summary>
        public static bool NeedsReinitialization { get; private set; }

        private MechJebController()
        {
            _wrappers = new Dictionary<WrapperTypes, IBaseWrapper>()
            {
                { WrapperTypes.Core, new MechJebCoreWrapper() },
                { WrapperTypes.Ascent, new MechJebAscentWrapper() },
                { WrapperTypes.Vessel, new VesselStateWrapper() },
                { WrapperTypes.Info, new MechJebInfoItemsWrapper() },
            };
            foreach (var baseWrapper in _wrappers)
            {
                baseWrapper.Value.Initialize();
            }

            EnsureEventsSubscribed();
        }

        private static MechJebController _instance;

        public static MechJebController Instance => _instance ??= new MechJebController();

        public bool IsAvailable => _wrappers.All(w => w.Value.Initialized);

        public MechJebCoreWrapper Core => _wrappers[WrapperTypes.Core] as MechJebCoreWrapper;
        public MechJebAscentWrapper AscentWrapper => _wrappers[WrapperTypes.Ascent] as MechJebAscentWrapper;
        public VesselStateWrapper VesselState => _wrappers[WrapperTypes.Vessel] as VesselStateWrapper;
        public MechJebInfoItemsWrapper InfoItems => _wrappers[WrapperTypes.Info] as MechJebInfoItemsWrapper;

        /// <summary>
        /// Force all wrappers to reinitialize, clearing stale MechJeb references.
        /// Call this after detecting that GameEvents have fired (save reload, vessel change).
        /// </summary>
        public void ForceReinitialize()
        {
            foreach (var wrapper in _wrappers.Values)
            {
                wrapper.Reinitialize();
            }
            NeedsReinitialization = false;
        }

        /// <summary>
        /// Subscribe to KSP GameEvents to detect when saves are reloaded or vessels change.
        /// This ensures we clear stale MechJeb references when the flight scene changes.
        /// </summary>
        private static void EnsureEventsSubscribed()
        {
            if (_eventsSubscribed) return;

            // Guard against GameEvents not being initialized yet
            if (GameEvents.onFlightReady == null ||
                GameEvents.onVesselChange == null ||
                GameEvents.onGameSceneLoadRequested == null)
            {
                return;
            }

            try
            {
                // onFlightReady fires when entering flight scene (including save reloads)
                GameEvents.onFlightReady.Add(OnFlightReady);

                // onVesselChange fires when switching between vessels
                GameEvents.onVesselChange.Add(OnVesselChange);

                // onGameSceneLoadRequested fires before loading any scene (cleanup opportunity)
                GameEvents.onGameSceneLoadRequested.Add(OnGameSceneLoadRequested);

                _eventsSubscribed = true;
            }
            catch
            {
                // GameEvents exist but aren't ready to accept subscribers yet - will retry next access
            }
        }

        private static void OnFlightReady()
        {
            NeedsReinitialization = true;
        }

        private static void OnVesselChange(Vessel vessel)
        {
            NeedsReinitialization = true;
        }

        private static void OnGameSceneLoadRequested(GameScenes scene)
        {
            NeedsReinitialization = true;
        }
    }

    internal enum WrapperTypes
    {
        Info,
        Vessel,
        Ascent,
        Core
    }
}
