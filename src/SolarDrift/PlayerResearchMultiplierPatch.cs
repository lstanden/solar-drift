using Game;
using HarmonyLib;
using Manager;

namespace SolarDrift;

[HarmonyPatch(typeof(ResearchManager), nameof(ResearchManager.GetResearchPointPerHour))]
internal static class PlayerResearchMultiplierPatch
{
    private static readonly AccessTools.FieldRef<ResearchManager, Company> CompanyRef =
        AccessTools.FieldRefAccess<ResearchManager, Company>("company");

    private static void Postfix(ResearchManager __instance, ref float __result)
    {
        var multiplierEnabled = Plugin.EnablePlayerResearchMultiplier?.Value ?? false;
        var multiplier = Plugin.PlayerResearchMultiplier?.Value ?? 1.0;
        if (!multiplierEnabled || multiplier <= 0.0 || multiplier == 1.0)
        {
            return;
        }

        var company = CompanyRef(__instance);
        if (company == null || !company.IsPlayer)
        {
            return;
        }

        __result *= (float)multiplier;
    }
}
