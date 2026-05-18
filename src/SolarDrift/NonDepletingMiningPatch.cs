using System.Collections.Generic;
using Game;
using Game.ObjectInfoDataScripts;
using Game.UI.Windows.Elements.ObjectInfoElements;
using HarmonyLib;
using ScriptableObjectScripts;

namespace SolarDrift;

[HarmonyPatch(typeof(ObjectInfoData), nameof(ObjectInfoData.UpdateMining))]
internal static class NonDepletingMiningPatch
{
    private static void Prefix(
        ObjectInfoData __instance,
        ref double power,
        HashSet<ResourceDefinition> resourcesToMine,
        bool fakeMining,
        ref Dictionary<RowResourcesData, double>? __state)
    {
        if ((Plugin.NonDepletingMining?.Value ?? false) && !fakeMining && resourcesToMine != null)
        {
            var deposits = __instance.GetResourcesDepositsSortedByExplorationLevelAndMiningFactor();
            __state = new Dictionary<RowResourcesData, double>();

            foreach (var deposit in deposits)
            {
                if (deposit?.ObservedData == null || !resourcesToMine.Contains(deposit.ResourceType))
                {
                    continue;
                }

                __state[deposit.ObservedData] = deposit.ObservedData.Value;
            }
        }

        ApplyPlayerMiningMultiplier(__instance.company, ref power);
    }

    private static void Postfix(Dictionary<RowResourcesData, double>? __state)
    {
        if (__state == null)
        {
            return;
        }

        foreach (var pair in __state)
        {
            pair.Key.Value = pair.Value;
        }
    }

    internal static void ApplyPlayerMiningMultiplier(Company? company, ref double miningEfficiency)
    {
        var multiplierEnabled = Plugin.EnablePlayerMiningMultiplier?.Value ?? false;
        var multiplier = Plugin.PlayerMiningMultiplier?.Value ?? 1.0;
        if (!multiplierEnabled || company == null || !company.IsPlayer || multiplier <= 0.0 || multiplier == 1.0)
        {
            return;
        }

        miningEfficiency *= multiplier;
    }
}

[HarmonyPatch(typeof(ObjectInfoData), nameof(ObjectInfoData.UpdateMiningOrbitalGasExtractor))]
internal static class PlayerOrbitalGasMiningMultiplierPatch
{
    private static void Prefix(
        ObjectInfoData __instance,
        ref double power,
        bool fakeMining)
    {
        NonDepletingMiningPatch.ApplyPlayerMiningMultiplier(__instance.company, ref power);
    }
}
