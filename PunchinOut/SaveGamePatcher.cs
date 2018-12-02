using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleTech.Save;
using Harmony;
using BattleTech;
using BattleTech.Save.SaveGameStructure;

namespace BasicPanic
{
    [HarmonyPatch(typeof(GameInstanceSave))]
    [HarmonyPatch(new Type[] { typeof(GameInstance), typeof(SaveReason) })]
    public static class GameInstanceSave_Constructor_Patch
    {
        static void Postfix(GameInstanceSave __instance)
        {
            try
            {
                Holder.SerializeStorageJson(__instance.InstanceGUID, __instance.SaveTime);
            }
            catch
            {
                // TODO: should do something here if the holder reports an error trying to serialize
                return;
            }
        }
    }

    [HarmonyPatch(typeof(GameInstance), "Load")]
    public static class GameInstance_Load_Patch
    {
        static void Prefix(GameInstance __instance, GameInstanceSave save)
        {
            try
            {
                Holder.Resync(save.SaveTime);
            }
            catch
            {
                // TODO: should do something here if the holder reports an error trying to serialize
                return;
            }
        }
    }

    [HarmonyPatch(typeof(SimGameState), "_OnFirstPlayInit")]
    public static class SimGameState_FirstPlayInit_Patch
    {
        static void Postfix(SimGameState __instance, SimGameState.SimGameType gameType, bool allowDebug) //we're doing a new campaign, so we need to sync the json with the new addition
        {
            try
            {
                Holder.SyncNewCampaign();
            }
            catch
            {
                // TODO:  should do something here if the holder reports an error
                return;
            }
        }
    }
}
