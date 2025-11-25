using kOS.Safe.Encapsulation;

namespace kOS.MechJeb2.Addon.Core
{
    public interface IMechJebAscentWrapper : IBaseWrapper
    {
        BooleanValue Enabled { get; set; }
    }
}