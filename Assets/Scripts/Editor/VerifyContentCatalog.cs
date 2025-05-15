using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class VerifyContentCatalog : MonoBehaviour
{
    [UnityEditor.MenuItem("Debug/Validate Dependencies")]
    public static void ValidateDependencies()
    {
        string assetPath = EditorUtility.OpenFilePanel("Addressables Catalog", Path.GetDirectoryName(Application.dataPath), "json");
        if (File.Exists(assetPath) == false)
            return;
        
        var json = File.ReadAllText(assetPath);
        ContentCatalogData d = JsonUtility.FromJson<ContentCatalogData>(json);
        
        var locator = d.CreateLocator();
        StringBuilder b = new StringBuilder();
        
        foreach (KeyValuePair<object,IList<IResourceLocation>> pair in locator.Locations) // all the entries from the catalog.
        {
            foreach (var loc in pair.Value)
            {
                var dependencies = loc.Dependencies;
                
                if (!loc.ProviderId.Contains("AssetBundle") && dependencies.Count > 1) // only checking the non-bundle locations.
                {
                    var mainBundle = dependencies[0]; // the first dependency is always the main bundle.

                    if (!IsRemote(mainBundle)) // checking all the local bundles.
                    {
                        for (int i = 1; i < dependencies.Count; i++)
                        {
                            var otherBundle = dependencies[i];
                            if (IsRemote(otherBundle))
                                Debug.LogError($"The bundle for the asset: {loc.InternalId}, {mainBundle.PrimaryKey} is local but depends on the remote bundle: {otherBundle.PrimaryKey}");
                        }
                    }
                }
            }
        }
    }

    [UnityEditor.MenuItem("Debug/Remote Bundles Size")]
    public static void GetRemoteBundlesSize()
    {
        string assetPath = EditorUtility.OpenFilePanel("Addressables Catalog", Path.GetDirectoryName(Application.dataPath), "json");
        if (File.Exists(assetPath) == false)
            return;
        
        var json = File.ReadAllText(assetPath);
        ContentCatalogData d = JsonUtility.FromJson<ContentCatalogData>(json);
        
        var locator = d.CreateLocator();
        StringBuilder b = new StringBuilder();
        
        foreach (KeyValuePair<object,IList<IResourceLocation>> pair in locator.Locations)
        {
            foreach (var loc in pair.Value)
            {
                var dependencies = loc.Dependencies;
                if (loc.ProviderId.Contains("AssetBundle"))
                    if (loc.Data != null && IsRemote(loc))
                        if (loc.Data is AssetBundleRequestOptions options)
                            b.AppendLine($"Bundle->({loc.PrimaryKey}), Bundle Size->({options.BundleSize}).");
            }
        }
       
        Debug.Log(b.ToString());
    }
    
    private static bool IsRemote(IResourceLocation location)
    {
        return location.InternalId.StartsWith("http", System.StringComparison.Ordinal);
    }
}
