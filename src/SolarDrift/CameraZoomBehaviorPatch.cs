using CameraControl;
using Game.Info;
using HarmonyLib;
using UnityEngine;

namespace SolarDrift;

internal static class CameraZoomResetGate
{
    private static int _allowUntilFrame = -1;

    internal static void AllowForCurrentFrame()
    {
        _allowUntilFrame = Time.frameCount;
    }

    internal static bool IsAllowedFor(object? target)
    {
        if (!(Plugin.PreserveCameraZoom?.Value ?? true))
        {
            return true;
        }

        if (_allowUntilFrame == Time.frameCount || IsControlHeld())
        {
            return true;
        }

        return false;
    }

    internal static bool IsControlHeld() =>
        Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
}

[HarmonyPatch(typeof(ButtonDoubelClick), nameof(ButtonDoubelClick.OnPointerClick))]
internal static class ButtonControlClickCameraZoomPatch
{
    private static void Prefix()
    {
        if (CameraZoomResetGate.IsControlHeld())
        {
            CameraZoomResetGate.AllowForCurrentFrame();
        }
    }
}

[HarmonyPatch(typeof(MyCameraController), "SetTargetAndZoomBasedOnTimeScaleAndObjectType")]
internal static class PreserveCameraZoomOnTimeScaleChangePatch
{
    private static void Prefix(MyCameraController __instance, bool timeScaleChange, out float __state)
    {
        __state = (Plugin.PreserveCameraZoom?.Value ?? true) && timeScaleChange ? __instance.CurrentFinalZoom : float.NaN;
    }

    private static void Postfix(MyCameraController __instance, float __state)
    {
        if (!float.IsNaN(__state))
        {
            __instance.FinalZoom = __state;
        }
    }
}

[HarmonyPatch(typeof(MyCameraController), nameof(MyCameraController.ChangeTarget), typeof(Transform), typeof(bool))]
internal static class PreserveCameraZoomUnlessControlClickTransformPatch
{
    private static void Prefix(MyCameraController __instance, Transform newTarget, out float __state)
    {
        __state = CameraZoomResetGate.IsAllowedFor(newTarget) ? float.NaN : __instance.CurrentFinalZoom;
    }

    private static void Postfix(MyCameraController __instance, float __state)
    {
        if (!float.IsNaN(__state))
        {
            __instance.FinalZoom = __state;
        }
    }
}

[HarmonyPatch(typeof(MyCameraController), nameof(MyCameraController.ChangeTarget), typeof(ObjectInfo), typeof(bool))]
internal static class PreserveCameraZoomUnlessControlClickObjectInfoPatch
{
    private static void Prefix(MyCameraController __instance, ObjectInfo newTarget, out float __state)
    {
        __state = CameraZoomResetGate.IsAllowedFor(newTarget) ? float.NaN : __instance.CurrentFinalZoom;
    }

    private static void Postfix(MyCameraController __instance, float __state)
    {
        if (!float.IsNaN(__state))
        {
            __instance.FinalZoom = __state;
        }
    }
}
