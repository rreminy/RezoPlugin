using Dalamud.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace RezoPlugin
{
    [SuppressMessage("Minor Code Smell", "S2292", Justification = "ref access")]
    public sealed class Config : IPluginConfiguration
    {
        internal bool _enabled = true;
        internal bool _onlyMin = false;
        internal float _scale = 0.25f;
        internal bool _disableFsrSharpen = false;

        /// <inheritdoc/>
        int IPluginConfiguration.Version { get; set; } = 0;

        /// <summary>Is plugin enabled?</summary>
        public bool Enabled
        {
            get => this._enabled;
            set => this._enabled = value;
        }

        /// <summary>Only take effect if set to 3D Resoution Scaling is set to 50.</summary>
        public bool OnlyMinimum
        {
            get => this._onlyMin;
            set => this._onlyMin = value;
        }

        /// <summary>3D Resolution Scaling to force.</summary>
        public float Scale
        {
            get => this._scale;
            set => this._scale = value;
        }

        public bool DisableFSRSharpen
        {
            get => this._disableFsrSharpen;
            set => this._disableFsrSharpen = value;
        }
    }
}
