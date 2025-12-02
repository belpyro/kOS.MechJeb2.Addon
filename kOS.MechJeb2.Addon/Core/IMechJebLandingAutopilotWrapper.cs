using kOS.Safe.Encapsulation;

namespace kOS.MechJeb2.Addon.Core
{
    public interface IMechJebLandingAutopilotWrapper : IBaseWrapper
    {
        BooleanValue Enabled { get; set; }
    }
}
