﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Graphics.Render;

namespace RezoPlugin
{
    public sealed unsafe class Plugin : IDalamudPlugin
    {
        private readonly Config _config;
        private readonly ConfigUi _configUi;
        private readonly WindowSystem _windows;

        private IDalamudPluginInterface PluginInterface { get; }
        private IFramework Framework { get; }
        private IGameConfig GameConfig { get; }
        private ICommandManager Commands { get; }
        private IPluginLog Logger { get; }

        public Plugin(IDalamudPluginInterface pluginInterface, IFramework framework, IGameConfig gameConfig, ICommandManager commands, IPluginLog logger)
        {
            this.PluginInterface = pluginInterface;
            this.Framework = framework;
            this.GameConfig = gameConfig;
            this.Commands = commands;
            this.Logger = logger;

            this._config = this.PluginInterface.GetPluginConfig() as Config ?? new Config();
            this._configUi = new(this._config, this.PluginInterface, this.Logger);

            this._windows = new();
            this._windows.AddWindow(this._configUi);

            this.Commands.AddHandler("/prezo", new(this.RezoCommandHandler) { HelpMessage = "Open / Close rezo scale plugin configuration window." });
            this.Framework.Update += Framework_Update;
            this.PluginInterface.UiBuilder.Draw += this._windows.Draw;
            this.PluginInterface.UiBuilder.OpenConfigUi += this._configUi.Toggle;
            this.PluginInterface.UiBuilder.OpenMainUi += this._configUi.Toggle;
        }

        public void RezoCommandHandler(string command, string args)
        {
            float? newScale = null;
            if (float.TryParse(args, out var scaleFloat) && scaleFloat is >= 0.1f and <= 1.0f)
            {
                newScale = scaleFloat;
            }
            else if (uint.TryParse(args, out var scaleUint) && scaleFloat is >= 10 and <= 100)
            {
                newScale = scaleUint / 100f;
            }
            if (newScale is not null)
            {
                this._config.Enabled = true;
                this._config.OnlyMinimum = false;
                this._config.Scale = newScale.Value;
                this.PluginInterface.SavePluginConfig(this._config);
            }
            else 
            {
                this._configUi.Toggle();
            }
        }

        private void Framework_Update(IFramework framework)
        {
            if (!this._config.Enabled) return;
            if (this._config.OnlyMinimum && this.GameConfig.System.GetUInt("GraphicsRezoScale") > 50) return;

            var newScale = this._config.Scale;
            if (newScale is >= 0f and <= 1f) this.SetScale(newScale);
        }

        [SuppressMessage("Major Bug", "S1244", Justification = "Intended.")]
        private void SetScale(float scale)
        {
            var oldScale = GraphicsConfig.Instance()->GraphicsRezoScale;
            if (oldScale == scale) return;
            GraphicsConfig.Instance()->GraphicsRezoScale = scale;
            this.Logger.Info($"Changed Rezo Scale from {oldScale} to {scale} | New Scale: {GraphicsConfig.Instance()->GraphicsRezoScale}");
        }

        public void Dispose()
        {
            this.Commands.RemoveHandler("/prezo");
            this.Framework.Update -= Framework_Update;
            this.PluginInterface.UiBuilder.Draw -= this._windows.Draw;
            this.PluginInterface.UiBuilder.OpenConfigUi -= this._configUi.Toggle;
            this.PluginInterface.UiBuilder.OpenMainUi -= this._configUi.Toggle;

            this.SetScale(this.GameConfig.System.GetUInt("GraphicsRezoScale") / 100f);
        }
    }
}
