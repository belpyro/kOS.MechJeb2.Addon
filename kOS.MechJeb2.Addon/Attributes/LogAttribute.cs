using kOS.Safe.Exceptions;
using KSPBuildTools;
using MethodBoundaryAspect.Fody.Attributes;

namespace kOS.MechJeb2.Addon.Attributes
{
    public class LogAttribute : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            var obj = args.Instance;
            if (obj is ILogContextProvider provider)
            {
                provider.LogDebug($"OnEnter to method {args.Method.Name}");
            }
        }

        public override void OnException(MethodExecutionArgs args)
        {
            var obj = args.Instance;
            if (obj is not ILogContextProvider provider) return;
            provider.LogError($"Error in " +
                              $"OnException to method {args.Method.Name} with exception {args.Exception}");
            args.FlowBehavior = FlowBehavior.RethrowException;
            args.Exception = new KOSException("Unhandled exception in " + args.Method.Name);
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            var obj = args.Instance;
            if (obj is ILogContextProvider provider)
            {
                provider.LogDebug($"OnEnter to method {args.Method.Name}");
            }
        }
        
    }
}