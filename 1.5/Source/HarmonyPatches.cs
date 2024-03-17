using Verse;
using RimWorld;
using Verse.AI;
using HarmonyLib;
using System.Reflection;

namespace LifeSupport {
    [StaticConstructorOnStartup]
    public static class HarmonyPatches {
        static HarmonyPatches() {
            Harmony harmony = new Harmony("LifeSupport");
            if (ModsConfig.IsActive("Troopersmith1.DeathRattle"))
                harmony.Patch(AccessTools.Method(typeof(HediffSet), "DirtyCache"),postfix: new HarmonyMethod(typeof(HediffSet_DirtyCache_Patch), "DirtyCache_Postfix"));
            harmony.PatchAll();
        }
        [HarmonyPatch(typeof(Pawn_HealthTracker), "ShouldBeDeadFromRequiredCapacity")]
        public static class ShouldBeDeadFromRequiredCapacity_Patch
        {
            [HarmonyPrefix]
            public static bool ShouldBeDeadFromRequiredCapacity_Prefix(ref Pawn_HealthTracker __instance, ref PawnCapacityDef __result)
            {
                Pawn_HealthTracker health = __instance;
                Pawn pawn = health.hediffSet.pawn;

                if (health.hediffSet.HasHediff(LifeSupportDefOf.QE_LifeSupport) && pawn.ValidLifeSupportNearby() && health.capacities.CapableOf(PawnCapacityDefOf.Consciousness))
                {
                    __result = null;
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(Toils_LayDown), "LayDown")]
        public static class LayDown_Patch
        {
            [HarmonyPostfix]
            public static void LayDown_Postfix(ref Toil __result)
            {
                Toil toil = __result;
                if (toil == null)
                    return;

                toil.AddPreTickAction(delegate ()
                {
                    Pawn pawn = toil.actor;
                    if (pawn is null || pawn.Dead)
                    {
                        return;
                    }

                    LifeSupportUtility.SetHediffs(pawn);
                });
            }
        }
        [HarmonyPatch(typeof(PawnCapacitiesHandler), "Notify_CapacityLevelsDirty")]
        public static class PawnCapacitiesHandler_Notify_CapacityLevelsDirty_Patch
        {
            [HarmonyPostfix]
            public static void Notify_CapacityLevelsDirty_Postfix(Pawn ___pawn)
            {
                if (Scribe.mode != LoadSaveMode.LoadingVars)
                {
                    if (!___pawn.health.ShouldBeDead())
                        LifeSupportUtility.SetHediffs(___pawn);
                }
            }
        }
        public static class HediffSet_DirtyCache_Patch
        {
            [HarmonyPostfix]
            public static void DirtyCache_Postfix(Pawn ___pawn)
            {
                if (!___pawn.health.ShouldBeDead())
                {
                    LifeSupportUtility.SetHediffs(___pawn);
                }
            }
        }
    }
}
