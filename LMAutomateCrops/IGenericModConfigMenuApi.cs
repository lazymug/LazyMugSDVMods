using System;
using StardewModdingAPI;

namespace LMAutomateCrops
{
    /// <summary>Subset of Generic Mod Config Menu's API used here.</summary>
    public interface IGenericModConfigMenuApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name,
            Func<string>? tooltip = null, string? fieldId = null);

        void AddSectionTitle(IManifest mod, Func<string> text, Func<string>? tooltip = null);
    }
}
