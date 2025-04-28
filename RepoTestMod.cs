using System.Collections.Generic;
using System.IO;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Photon.Pun;
using REPOLib.Modules;
using UnityEngine;

namespace RepoTestMod;

[BepInPlugin("Thorminate.RepoTestMod", "RepoTestMod", "1.0.0")]
[BepInDependency(REPOLib.MyPluginInfo.PLUGIN_GUID)]
public class RepoTestMod : BaseUnityPlugin
{
    internal static RepoTestMod Instance { get; private set; } = null!;
    internal static new ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;
    internal Harmony? Harmony { get; set; }
    public static GameObject CubePrefab { get; private set; } = null!;
    public static GameObject CylinderPrefab { get; private set; } = null!;
    public static AssetBundle? Assets { get; private set; } = null!;

    private void Awake()
    {
        Instance = this;

        // Prevent the plugin from being deleted
        gameObject.transform.parent = null;
        gameObject.hideFlags = HideFlags.HideAndDontSave;

        Patch();
        LoadCube();

        Logger.LogInfo(
            $"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded! Debug mode: {Debug.isDebugBuild}"
        );
    }

    private AssetBundle FetchBundles()
    {
        if (Assets != null)
        {
            return Assets;
        }
        string bundlePath = Path.Combine(Paths.PluginPath, "Thorminate-RepoTestMod", "repotestmod");
        return Assets = AssetBundle.LoadFromFile(bundlePath);
    }

    private void LoadCube()
    {
        var bundle = FetchBundles();

        if (bundle == null)
        {
            Logger.LogError("Cannot load AssetBundle!");
            return;
        }

        var presets = new List<string> { "Valuables - Generic" };

        CubePrefab = bundle.LoadAsset<GameObject>("Cube");

        if (CubePrefab == null)
        {
            Logger.LogError("Prefab 'Cube' is unknown in the bundle!");
            return;
        }

        NetworkPrefabs.RegisterNetworkPrefab(CubePrefab);
        Valuables.RegisterValuable(CubePrefab, presets);
        Logger.LogInfo("Registered 'Cube' successfully");

        CylinderPrefab = bundle.LoadAsset<GameObject>("Cylinder");

        if (CylinderPrefab == null)
        {
            Logger.LogError("Prefab 'Cylinder' is unknown in the bundle!");
            return;
        }

        NetworkPrefabs.RegisterNetworkPrefab(CylinderPrefab);
        Valuables.RegisterValuable(CylinderPrefab, presets);
        Logger.LogInfo("Registered 'Cylinder' successfully");
    }

    internal void Patch()
    {
        Harmony ??= new Harmony(Info.Metadata.GUID);
        Harmony.PatchAll();
    }

    internal void Unpatch()
    {
        Harmony?.UnpatchSelf();
    }

    private void Update()
    {
        // Code that runs every frame goes here
    }
}
