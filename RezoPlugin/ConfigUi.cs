using Dalamud.Interface.Windowing;
using ImGuiNET;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Graphics.Render;

namespace RezoPlugin
{
    internal unsafe class ConfigUi : Window
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

            this.Size = new(480, 140);
            this.SizeCondition = ImGuiCond.FirstUseEver;
        }

        public override void Draw()
        {
            if (ImGui.Checkbox("Enabled", ref this.Config._enabled) && !this.Config._enabled) this.Plugin.RestoreGameSetting();
            if (ImGui.IsItemHovered()) ImGui.SetTooltip("Enable plugin");

            if (GraphicsConfig.Instance()->GraphicsRezoUpscaleType > 1)
            {
                ImGui.TextColored(Dalamud.Interface.Colors.ImGuiColors.DalamudRed, "Plugin only works with AMD FSR Graphics Upscaling");
                return;
            }

            ImGui.BeginDisabled(!this.Config._enabled);
            ImGui.Checkbox("Only at 50", ref this.Config._onlyMin);
            if (ImGui.IsItemHovered()) ImGui.SetTooltip("Apply only if 3D Resolution Scale is to 50 via the game's System Configuration window");

            ImGui.SliderFloat("3D Resolution Scale", ref this.Config._scale, 0.1f, this.Config.OnlyMinimum ? 0.5f : 1.0f);

            ImGui.Checkbox("Disable Sharpen filter", ref this.Config._disableFsrSharpen);
            if (ImGui.IsItemHovered()) ImGui.SetTooltip("Disables the sharpen filter from FSR");
            ImGui.EndDisabled();
        }

        public override void OnClose()
        {
            this.PluginInterface.SavePluginConfig(this.Config);
            this.Logger.Information("Configuration Saved!");
        }
    }
}
