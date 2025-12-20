using System.Collections.Generic;
using System.Linq;
using kOS.MechJeb2.Addon.Core;
using kOS.MechJeb2.Addon.Wrapeers;

namespace kOS.MechJeb2.Addon
{
    public class MechJebController
    {
        private readonly Dictionary<WrapperTypes, IBaseWrapper> _wrappers;

        private MechJebController()
        {
            _wrappers = new Dictionary<WrapperTypes, IBaseWrapper>()
            {
                { WrapperTypes.Core, new MechJebCoreWrapper() },
                { WrapperTypes.Ascent, new MechJebAscentWrapper() },
                { WrapperTypes.Vessel, new VesselStateWrapper() },
                { WrapperTypes.Info, new MechJebInfoItemsWrapper() },
                { WrapperTypes.Planner, new MechJebManeuverPlannerWrapper() },
                { WrapperTypes.Node, new MechJebNodeExecutorWrapper() },
            };
            foreach (var baseWrapper in _wrappers)
            {
                baseWrapper.Value.Initialize();
            }
        }

        private static MechJebController _instance;

        public static MechJebController Instance => _instance ??= new MechJebController();

        public bool IsAvailable => _wrappers.All(w => w.Value.Initialized);

        public MechJebCoreWrapper Core => _wrappers[WrapperTypes.Core] as MechJebCoreWrapper;
        public MechJebAscentWrapper AscentWrapper => _wrappers[WrapperTypes.Ascent] as MechJebAscentWrapper;
        public VesselStateWrapper VesselState => _wrappers[WrapperTypes.Vessel] as VesselStateWrapper;
        public MechJebInfoItemsWrapper InfoItems => _wrappers[WrapperTypes.Info] as MechJebInfoItemsWrapper;
        public MechJebManeuverPlannerWrapper ManeuverPlanner => _wrappers[WrapperTypes.Planner] as MechJebManeuverPlannerWrapper;
        public MechJebNodeExecutorWrapper NodeExecutor => _wrappers[WrapperTypes.Node] as MechJebNodeExecutorWrapper;
    }

    internal enum WrapperTypes
    {
        Info,
        Vessel,
        Ascent,
        Core,
        Planner,
        Node
    }
}
