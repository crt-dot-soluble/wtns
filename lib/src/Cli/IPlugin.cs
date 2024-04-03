namespace Wtns.Me.Lib.Cli;

/// <summary>
/// Describes the bare minimum functionality of a WTNS plugin.
/// </summary>
interface IPlugin
{
    /// <summary>
    /// Called when the plugin is initially loaded.
    /// </summary>
    void OnLoad();

    /// <summary>
    /// Called when the plugin is unloaded.
    /// Not called in the case of a Reload.
    /// </summary>
    void OnUnload();
}
