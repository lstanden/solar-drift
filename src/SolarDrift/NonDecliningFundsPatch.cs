using System;
using System.Reflection;
using Game;
using Game.CompanyScripts;
using Game.ObjectInfoDataScripts;
using HarmonyLib;

namespace SolarDrift;

[HarmonyPatch(typeof(MoneyController), nameof(MoneyController.RemoveMoney), typeof(double))]
internal static class NonDecliningFundsPatch
{
    private static bool Prefix(MoneyController __instance, ref bool __result)
    {
        if (!(Plugin.NonDecliningFunds?.Value ?? false) || !IsPlayerMoneyController(__instance))
        {
            return true;
        }

        __result = true;
        return false;
    }

    internal static bool IsPlayerMoneyController(MoneyController moneyController)
    {
        var company = moneyController.gameObject.GetComponent<Company>();
        return company != null && company.IsPlayer;
    }
}

[HarmonyPatch]
internal static class NonDecliningFundsWithTransactionPatch
{
    private static MethodBase TargetMethod()
    {
        return AccessTools.Method(
            typeof(MoneyController),
            nameof(MoneyController.RemoveMoney),
            new[] { typeof(double), typeof(ObjectInfoData.TransactionRemoveResource).MakeByRefType() });
    }

    private static bool Prefix(
        MoneyController __instance,
        double cost,
        ref ObjectInfoData.TransactionRemoveResource transaction,
        ref bool __result)
    {
        if (!(Plugin.NonDecliningFunds?.Value ?? false) || !NonDecliningFundsPatch.IsPlayerMoneyController(__instance))
        {
            return true;
        }

        transaction = new ObjectInfoData.TransactionRemoveResource
        {
            amount = cost,
            removeMoney = true,
            transactionOk = true,
            Company = __instance.gameObject.GetComponent<Company>()
        };

        __result = true;
        return false;
    }
}
