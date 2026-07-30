/*
 * Strawberry Game Engine
 * File: AssetManager.cs
 * Author: Koosha Aabedini Nassab
 *
 * Hierarchical asset registry with string-based lookup and reverse lookup.
 */

namespace Strawberry.Core
{
    /// <summary>
    /// A hierarchical string-keyed registry for engine assets (textures, sprites,
    /// fonts, sounds, layers, prefabs, etc.). Supports reverse lookup (asset → name)
    /// Lookup order: 1. this manager 2. parent manager 3. return null.
    /// </summary>
    public class AssetManager : DisposableReferenceObject
    {
        private readonly Dictionary<string, object> assets = new Dictionary<string, object>();

        private readonly Dictionary<object, string> assetsByRef =
            new Dictionary<object, string>(ReferenceEqualityComparer.Instance);

        private readonly HashSet<object> ownedAssets = new HashSet<object>(
            ReferenceEqualityComparer.Instance);

        private readonly AssetManager parent;

        /// <summary>
        /// Gets the parent asset manager, or null if this is a root (global) manager.
        /// </summary>
        public AssetManager Parent => parent;

        /// <summary>
        /// Gets the number of assets registered directly in this manager
        /// (excluding assets in parent managers).
        /// </summary>
        public int Count => assets.Count;

        /// <summary>
        /// Creates a root asset manager with no parent. Use this for the global managers (Like IGameContext.Assets).
        /// </summary>
        public AssetManager() : this(null) { }

        /// <summary>
        /// Creates an asset manager that falls back to <paramref name="parent"/>
        /// for assets not found locally. (Like Scene.Assets)
        /// </summary>
        /// <param name="parent">The parent manager to fall back to, or null for a root.</param>
        public AssetManager(AssetManager parent)
        {
            this.parent = parent;
        }


        /// <summary>
        /// Registers an asset under <paramref name="name"/>. The manager takes ownership
        /// of the asset (will dispose it on <see cref="Dispose"/>) unless
        /// <paramref name="owned"/> is false.
        ///
        /// Names must be unique within a single manager.
        /// </summary>
        /// <typeparam name="T">The asset type.</typeparam>
        /// <param name="name">Unique name for the asset within this manager.</param>
        /// <param name="asset">The asset object.</param>
        /// <param name="owned">If true (default), the asset will be disposed when this
        /// manager is disposed. If false, the asset's lifetime is managed elsewhere.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> or
        /// <paramref name="asset"/> is null.</exception>
        /// <exception cref="ArgumentException">An asset with the same name is already
        /// registered in this manager.</exception>
        public void Register<T>(string name, T asset, bool owned = true) where T : class
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name), "Asset name cannot be null or empty.");
            if (asset == null)
                throw new ArgumentNullException(nameof(asset), "Asset cannot be null.");

            if (assets.ContainsKey(name))
                throw new ArgumentException(
                    $"An asset named '{name}' is already registered in this AssetManager.",
                    nameof(name));

            assets[name] = asset;

            // First registration wins for reverse lookup. This is intentional:
            // if a user registers the same asset under two names, GetName returns
            // the first name (deterministic), not the most recent (surprising).
            if (!assetsByRef.ContainsKey(asset))
                assetsByRef[asset] = name;

            if (owned)
                ownedAssets.Add(asset);
        }

        /// <summary>
        /// Unregisters an asset by name. The asset is NOT disposed.
        /// </summary>
        /// <param name="name">The name of the asset to unregister.</param>
        /// <returns>The unregistered asset, or null if no asset with that name was found.</returns>
        public object Unregister(string name)
        {
            if (!assets.TryGetValue(name, out object asset))
                return null;

            assets.Remove(name);
            ownedAssets.Remove(asset);

            // Only remove from reverse lookup if this asset's recorded name matches.
            // (If the asset was registered under a second name, the reverse lookup
            // still points to the first name, and we shouldn't corrupt that entry.)
            if (assetsByRef.TryGetValue(asset, out string recordedName) && recordedName == name)
                assetsByRef.Remove(asset);

            return asset;
        }

        /// <summary>
        /// Unregisters a specific asset instance (by reference identity).
        /// Will not unregister on parent managers (only the current one).
        /// </summary>
        /// <typeparam name="T">The asset type.</typeparam>
        /// <param name="asset">The asset instance to unregister.</param>
        /// <returns>The name the asset was registered under, or null if not found.</returns>
        public string Unregister<T>(T asset) where T : class
        {
            if (asset == null) return null;

            if (!assetsByRef.TryGetValue(asset, out string name))
                return null;

            assets.Remove(name);
            assetsByRef.Remove(asset);
            ownedAssets.Remove(asset);
            return name;
        }

        /// <summary>
        /// Removes all assets from this manager.
        /// </summary>
        public void Clear()
        {
            assets.Clear();
            assetsByRef.Clear();
            ownedAssets.Clear();
        }


        /// <summary>
        /// Gets an asset by name, walking the parent chain if not found locally.
        /// </summary>
        /// <typeparam name="T">The expected asset type.</typeparam>
        /// <param name="name">The asset name.</param>
        /// <returns>The asset.</returns>
        /// <exception cref="KeyNotFoundException">No asset with the given name
        /// exists in this manager or any parent.</exception>
        /// <exception cref="InvalidCastException">An asset with the name exists
        /// but is not of type <typeparamref name="T"/>.</exception>
        public T Get<T>(string name) where T : class
        {
            object raw = GetObject(name);
            if (raw == null)
                throw new KeyNotFoundException(
                    $"Asset '{name}' not found in this AssetManager or any parent.");

            if (raw is T typed)
                return typed;

            throw new InvalidCastException(
                $"Asset '{name}' is of type {raw.GetType().FullName}, " +
                $"but was requested as {typeof(T).FullName}.");
        }

        /// <summary>
        /// Tries to get an asset by name, walking the parent chain if not found locally.
        /// </summary>
        /// <typeparam name="T">The type of the asset</typeparam>
        /// <param name="name">The name of the asset</param>
        /// <param name="asset">The asset object if found and the type is <typeparamref name="T"/> or null</param>
        /// <returns>True if the asset is found and the type is <typeparamref name="T"/> otherwise null</returns>
        public bool TryGet<T>(string name, out T asset) where T : class
        {
            object raw = GetObject(name);
            if (raw == null)
            {
                asset = null;
                return false;
            }

            asset = raw as T;
            return asset != null;
        }

        /// <summary>
        /// Non-generic get. Returns the raw object or null if not found.
        /// </summary>
        public object GetObject(string name)
        {
            if (assets.TryGetValue(name, out object asset))
                return asset;

            return parent?.GetObject(name);
        }

        /// <summary>
        /// Checks if an asset with the given name exists in this manager or any parent.
        /// </summary>
        /// <param name="name">The asset name</param>
        /// <returns>True if asset exists in the manager or its parent chain, otherwise false</returns>
        public bool Contains(string name)
        {
            return assets.ContainsKey(name) || (parent?.Contains(name) ?? false);
        }

        /// <summary>
        /// Checks if an asset of type <typeparamref name="T"/> with the given name
        /// exists in this manager or any parent.
        /// </summary>
        /// <typeparam name="T">The expected asset type</typeparam>
        /// <param name="name">The asset name</param>
        /// <returns>True if asset exists in the manager or its parent chain, otherwise false</returns>
        public bool Contains<T>(string name) where T : class
        {
            return TryGet<T>(name, out _);
        }


        /// <summary>
        /// Returns the name an asset is registered under, or null if the asset
        /// is not registered in this manager or any parent.
        /// If an asset is registered under multiple names in the same manager, the first registered name is returned.
        /// </summary>
        /// <typeparam name="T">The asset type.</typeparam>
        /// <param name="asset">The asset instance.</param>
        /// <returns>The asset's registered name, or null if not found.</returns>
        public string GetName<T>(T asset) where T : class
        {
            if (asset == null) return null;

            if (assetsByRef.TryGetValue(asset, out string name))
                return name;

            return parent?.GetName(asset);
        }

        /// <summary>
        /// Tries to get the name of an asset.
        /// </summary>
        /// <typeparam name="T">The expected asset type</typeparam>
        /// <param name="asset">The asset object to look for</param>
        /// <param name="name">The name of the asset or null if the asset does not exists in the manager or its parent chain</param>
        /// <returns>Returns true if the name was found, false otherwise</returns>
        public bool TryGetName<T>(T asset, out string name) where T : class
        {
            name = GetName(asset);
            return name != null;
        }


        /// <summary>
        /// Returns an enumerable sequence of all (name, asset) pairs registered
        /// directly in this manager. Does NOT include parent assets.
        /// </summary>
        public IEnumerable<KeyValuePair<string, object>> GetLocalAssets()
        {
            foreach (var pair in assets)
                yield return pair;
        }

        /// <summary>
        /// Returns all asset names registered directly in this manager.
        /// Does NOT include parent assets.
        /// </summary>
        public IEnumerable<string> GetLocalNames()
        {
            return assets.Keys;
        }


        protected override void CleanUnmanaged()
        {
            foreach (object asset in ownedAssets)
            {
                (asset as IDisposable)?.Dispose();
            }

            ownedAssets.Clear();
            assets.Clear();
            assetsByRef.Clear();

            base.CleanUnmanaged();
        }
    }


    internal sealed class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        public static readonly ReferenceEqualityComparer Instance =
            new ReferenceEqualityComparer();

        private ReferenceEqualityComparer() { }

        public new bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(object obj)
        {
            return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
        }
    }
}
