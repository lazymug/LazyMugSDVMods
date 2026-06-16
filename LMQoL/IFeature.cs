using StardewModdingAPI;

namespace LMQoL
{
    public interface IFeature
    {
        string Id { get; }
        void Register(IModHelper helper, IMonitor monitor);
    }
}
