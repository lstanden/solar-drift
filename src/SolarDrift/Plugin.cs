using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Linq;

namespace SolarDrift;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Plugin : BaseUnityPlugin
{
    public const string PluginGuid = "local.solar-drift";
    public const string PluginName = "Solar Drift";
    public const string PluginVersion = "0.1.0";

    internal static ManualLogSource? Log { get; private set; }
    internal static ConfigEntry<bool>? PreserveCameraZoom { get; private set; }
    internal static ConfigEntry<bool>? NonDepletingMining { get; private set; }
    internal static ConfigEntry<bool>? NonDecliningFunds { get; private set; }
    internal static ConfigEntry<bool>? EnablePlayerMiningMultiplier { get; private set; }
    internal static ConfigEntry<double>? PlayerMiningMultiplier { get; private set; }
    internal static ConfigEntry<bool>? EnablePlayerResearchMultiplier { get; private set; }
    internal static ConfigEntry<double>? PlayerResearchMultiplier { get; private set; }

    private static Harmony? _harmony;

    public void Awake()
    {
        Log = Logger;
        PreserveCameraZoom = Config.Bind(
            "Camera",
            "PreserveCameraZoom",
            true,
            "When enabled, Space pause/unpause and normal target changes preserve your current zoom. Hold Ctrl while clicking a target to allow the game's normal zoom reset.");
        NonDepletingMining = Config.Bind(
            "Cheats",
            "NonDepletingMining",
            false,
            "When enabled, mining does not reduce resource deposits.");
        NonDecliningFunds = Config.Bind(
            "Cheats",
            "NonDecliningFunds",
            false,
            "When enabled, player money is not reduced when spending.");
        EnablePlayerMiningMultiplier = Config.Bind(
            "Cheats",
            "EnablePlayerMiningMultiplier",
            false,
            "When enabled, player-owned mining machines use PlayerMiningMultiplier. Non-player machines always use the base game value.");
        PlayerMiningMultiplier = Config.Bind(
            "Cheats",
            "PlayerMiningMultiplier",
            1.0,
            "Multiplier applied to player-owned mining machines. Non-player machines always use the base game value.");
        EnablePlayerResearchMultiplier = Config.Bind(
            "Cheats",
            "EnablePlayerResearchMultiplier",
            false,
            "When enabled, player research uses PlayerResearchMultiplier. Non-player research always uses the base game value.");
        PlayerResearchMultiplier = Config.Bind(
            "Cheats",
            "PlayerResearchMultiplier",
            1.0,
            "Multiplier applied to player research progress. Non-player research always uses the base game value.");

        Logger.LogInfo($"{PluginName} {PluginVersion} loaded");
        Logger.LogInfo($"Preserve camera zoom: {PreserveCameraZoom.Value}");
        Logger.LogInfo($"Non-depleting mining: {NonDepletingMining.Value}");
        Logger.LogInfo($"Non-declining funds: {NonDecliningFunds.Value}");
        Logger.LogInfo($"Player mining multiplier enabled: {EnablePlayerMiningMultiplier.Value}, multiplier: {PlayerMiningMultiplier.Value}");
        Logger.LogInfo($"Player research multiplier enabled: {EnablePlayerResearchMultiplier.Value}, multiplier: {PlayerResearchMultiplier.Value}");

        _harmony?.UnpatchSelf();
        _harmony = new Harmony(PluginGuid);
        _harmony.PatchAll(typeof(Plugin).Assembly);

        var patchedMethods = _harmony.GetPatchedMethods().ToList();
        Logger.LogInfo($"Harmony patched {patchedMethods.Count} method(s): {string.Join(", ", patchedMethods.Select(method => $"{method.DeclaringType?.FullName}.{method.Name}"))}");
    }

    public void Start()
    {
        Logger.LogInfo($"{PluginName} {PluginVersion} started");
    }
}
