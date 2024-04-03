using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WTNS;

/// <summary>
/// A registry for managing Configurations.
/// </summary>
public sealed class ConfigurationRegistry
{
    /// <summary>
    /// Support for lazy initialization of the ConfigurationRegistry.
    /// </summary>
    private static readonly Lazy<ConfigurationRegistry> instance = new Lazy<ConfigurationRegistry>(
        () => new ConfigurationRegistry()
    );

    /// <summary>
    /// Dictionary of configurations. The key is the configuration name and the value is the Configuration object.
    /// </summary>
    private readonly Dictionary<string, Configuration> configurations =
        new Dictionary<string, Configuration>();

    /// <summary>
    /// Gets the singleton instance of the ConfigurationRegistry.
    /// </summary>
    public static ConfigurationRegistry Instance => instance.Value;

    /// <summary>
    /// The current configuration being used.
    /// </summary>
    public Configuration CurrentConfiguration => new();

    /// <summary>
    /// The default configuration for templating or fallback.
    /// </summary>
    public static readonly Configuration DefaultConfiguration = new();

    /// <summary>
    /// Contructor for ConfigurationRegistry.
    /// </summary>
    private ConfigurationRegistry() { }

    /// <summary>
    /// Adds a Configuration to the registry.
    /// </summary>
    /// <param name="configurationName">The name of the Configuration.</param>
    /// <param name="configuration">The Configuration object.</param>
    public void AddConfiguration(string configurationName, Configuration configuration)
    {
        configurations[configurationName] = configuration;
    }

    /// <summary>
    /// Removes a Configuration from the registry.
    /// </summary>
    /// <param name="configurationName">The name of the Configuration to remove.</param>
    /// <returns>True if the Configuration was removed successfully; otherwise, false.</returns>
    public bool RemoveConfiguration(string configurationName)
    {
        return configurations.Remove(configurationName);
    }

    /// <summary>
    /// Gets a Configuration from the registry.
    /// </summary>
    /// <param name="configurationName">The name of the Configuration to retrieve.</param>
    /// <returns>The Configuration object if found; otherwise, null.</returns>
    public Configuration? GetConfiguration(string configurationName)
    {
        return configurations.TryGetValue(configurationName, out var configuration)
            ? configuration
            : null;
    }

    /// <summary>
    /// Gets all Configurations from the registry.
    /// </summary>
    /// <returns>An enumerable collection of all Configurations.</returns>
    public IEnumerable<Configuration> GetAllConfigurations()
    {
        return configurations.Values;
    }

    /// <summary>
    /// Applies a Configuration by name.
    /// </summary>
    /// <param name="configurationName">The name of the Configuration to apply.</param>
    public void ApplyConfiguration(string configurationName)
    {
        if (configurations.TryGetValue(configurationName, out var configuration))
        {
            // Apply the configuration logic here
            Console.WriteLine($"Applying Configuration: {configurationName}");
        }
        else
        {
            Console.WriteLine($"Configuration '{configurationName}' not found.");
        }
    }

    /// <summary>
    /// Applies a Configuration object.
    /// </summary>
    /// <param name="configuration">The Configuration object to apply.</param>
    public void ApplyConfiguration(Configuration configuration)
    {
        // Apply the configuration logic here
        Console.WriteLine($"Applying Configuration: {configuration.Name}");
    }

    /// <summary>
    /// Physically saves a Configuration object to disk.
    /// </summary>
    /// <param name="configuration">The configuration to save.</param>
    /// <param name="useBinarySerialization">Whether or not to use Binary Serialization</param>
    /// <exception cref="NotImplementedException"></exception>
    public static void SaveConfiguration(
        Configuration configuration,
        bool useBinarySerialization = false
    )
    {
        if (useBinarySerialization)
        {
            throw new NotImplementedException();
        }
        var json = JsonSerializer.Serialize(configuration);
        File.WriteAllText($"./config/{configuration.Name}.json", json);
    }
}
