namespace Strawberry.Platform;

public static class PlatformServices
{
    private static readonly Dictionary<Type, IPlatformService> services = new();

    /// <summary>
    /// Registers a platform service implementation
    /// </summary>
    /// <typeparam name="T">The service interface type to register</typeparam>
    /// <param name="service">The platform-specific implementation of the service</param>
    public static void RegisterService<T>(T service) where T : IPlatformService =>
        services[typeof(T)] = service;

    /// <summary>
    /// Gets a platform service of type <typeparamref name="T"/>.
    /// Returns <c>default</c> if the service is not registered on this platform.
    /// </summary>
    /// <typeparam name="T">The service interface type to retrieve</typeparam>
    /// <returns>The platform-specific service, or <c>default</c> if not available</returns>
    public static T GetService<T>() where T : IPlatformService => services.TryGetValue(typeof(T), out var service) ? (T)service : default;

    /// <summary>
    /// Clears the service registry
    /// </summary>
    public static void Reset() => services.Clear();
}