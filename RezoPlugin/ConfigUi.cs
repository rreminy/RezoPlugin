using Dalamud.Interface.Windowing;
using ImGuiNET;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace RezoPlugin
{
    internal class ConfigUi : Window
    {
        private Plugin Plugin { get; }
        private Config Config { get; }
        private IDalamudPluginInterface PluginInterface { get; }
        private IPluginLog Logger { get; }

        public ConfigUi(Plugin plugin, Config config, IDalamudPluginInterface pluginInterface, IPluginLog logger) : base("Rezo Scale Configuration")
        {
            this.Plugin = plugin;
            this.Config = config;
            this.PluginInterface = pluginInterface;
            this.Logger = logger;

            this.Size = new(480, 120);
            this.SizeCondition = ImGuiCond.FirstUseEver;
        }

        public override void Draw()
        {
            if (ImGui.Checkbox("Enabled", ref this.Config._enabled) && !this.Config._enabled) this.Plugin.RestoreGameSetting();
            if (ImGui.IsItemHovered()) ImGui.SetTooltip("Enable plugin");

            ImGui.BeginDisabled(!this.Config._enabled);
            ImGui.Checkbox("Only at 50", ref this.Config._onlyMin);
            if (ImGui.IsItemHovered()) ImGui.SetTooltip("Apply only if 3D Resolution Scale is to 50 via the game's System Configuration window");

            ImGui.SliderFloat("3D Resolution Scale", ref this.Config._scale, 0.1f, this.Config.OnlyMinimum ? 0.5f : 1.0f);
            ImGui.EndDisabled();
        }

        public override void OnClose()
        {
            this.PluginInterface.SavePluginConfig(this.Config);
            this.Logger.Information("Configuration Saved!");
        }
    }
}
