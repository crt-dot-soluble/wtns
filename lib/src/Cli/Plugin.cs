namespace Wtns.Me.Lib.Cli;

/// <summary>
/// Provides an entry point to supply WTNS with custom code and functionality.
/// It's inherited functions should perform an action that is reflective of it's name.
/// Implements the IPlugin interface, while it's code is not exposed it can be found in the documentation under <see cref="IPlugin"/>
/// </summary>
public class Plugin : IPlugin
{
    /// <summary>
    /// The name of the plugin, must be at least 3 characters long.
    /// </summary>
    public string Name
    {
        get { return Name; }
        init
        {
            if (value.Length < 3)
            {
                throw new Exception("Plugin name must be at least 3 characters long");
            }

            Name = value;
        }
    }

    /// <summary>
    /// The description of the plugin, must be at least 3 characters long.
    /// </summary>
    public required string Description
    {
        get { return Description; }
        init
        {
            if (value.Length < 3)
            {
                throw new Exception("Plugin description must be at least 3 characters long");
            }

            Description = value;
        }
    }

    /// <summary>
    /// Called when loading the plugin assembly.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void OnLoad()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Called when the plugin assembly is unloaded.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void OnUnload()
    {
        throw new NotImplementedException();
    }
}
