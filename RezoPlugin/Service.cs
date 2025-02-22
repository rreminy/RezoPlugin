using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace RezoPlugin
{
    internal class Service
    {
        internal IDalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService] public static ICondition Condition { get; private set; } = null!;
        [PluginService] public static IClientState ClientState { get; private set; } = null!;

        public void InitializeService(IDalamudPluginInterface pluginInterface)
        {
            this.PluginInterface = pluginInterface;
            this.PluginInterface.Create<Service>();
        }
    }
}
