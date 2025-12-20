using System;
using kOS.MechJeb2.Addon.Utils;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Exceptions;
using kOS.Safe.Utilities;

namespace kOS.MechJeb2.Addon.Wrapeers
{
    [KOSNomenclature("NodeExecutorWrapper")]
    public class MechJebNodeExecutorWrapper : BaseWrapper
    {
        private Func<object, object> _nodeExecutorGetter;

        // ExecuteOneNode and Abort methods for proper engagement
        private Action<object, object> _executeOneNode;
        private Action<object, object> _abort;  // second arg ignored for Abort
        private readonly object _userIdentity = new object();

        // Internal enabled property getter
        private Func<object, bool> _getEnabled;

        protected override void BindObject()
        {
            _nodeExecutorGetter = Member(MasterMechJeb, "Node").GetField<object>();
            var nodeExecutor = _nodeExecutorGetter(MasterMechJeb);

            _getEnabled = Member(nodeExecutor, "Enabled").GetProp<bool>();

            // Bind to ExecuteOneNode and Abort methods
            _executeOneNode = Reflect.On(nodeExecutor).Method("ExecuteOneNode").WithArgs(typeof(object)).AsAction();
            _abort = Reflect.On(nodeExecutor).Method("Abort").AsAction();

            // Settings
            GetAutoWarp = Member(nodeExecutor, "Autowarp").GetField<bool>();
            SetAutoWarp = Member(nodeExecutor, "Autowarp").SetField<bool>();

            // TODO: Additional settings available in MechJebModuleNodeExecutor:
            // - leadTime (EditableDouble) - seconds before burn to orient vessel
            // - rcsOnly (bool) - use RCS only for the burn
            // These require binding to EditableDouble which needs special handling
            // for the val property. See MechJeb source: MechJebModuleNodeExecutor.cs
        }

        protected override void InitializeSuffixes()
        {
            AddSuffix("ENABLED",
                new SetSuffix<BooleanValue>(() => Enabled, value => Enabled = value,
                    "Is node executor enabled?"));

            AddSuffix(new[] { "AUTOWARP", "WARP" },
                new SetSuffix<BooleanValue>(
                    () => GetAutoWarp(_nodeExecutorGetter(MasterMechJeb)),
                    value => SetAutoWarp(_nodeExecutorGetter(MasterMechJeb), value),
                    "Enable automatic time warp to node"));
        }

        public override string context() => nameof(MechJebNodeExecutorWrapper);

        public BooleanValue Enabled
        {
            get =>
                Initialized
                    ? new BooleanValue(_getEnabled(_nodeExecutorGetter(MasterMechJeb)))
                    : throw new KOSException("Cannot get Enabled property of not initialized MechJebNodeExecutorWrapper");
            set
            {
                if (!Initialized) return;

                var nodeExecutor = _nodeExecutorGetter(MasterMechJeb);

                // Call ExecuteOneNode to start, Abort to stop
                if (value)
                    _executeOneNode(nodeExecutor, _userIdentity);
                else
                    _abort(nodeExecutor, null);  // second arg ignored
            }
        }

        // Settings
        private Func<object, bool> GetAutoWarp { get; set; }
        private Action<object, bool> SetAutoWarp { get; set; }
    }
}
