using Verse;
using RimWorld;
using Verse.AI;
using HarmonyLib;

namespace LifeSupport;

[StaticConstructorOnStartup]
internal static class HarmonyPatches
{
	static HarmonyPatches()
	{
		Harmony harmony = new Harmony("LifeSupport");
		if (ModsConfig.IsActive("Troopersmith1.DeathRattle"))
			harmony.PatchCategory("DeathRattle");
		harmony.PatchAllUncategorized();
	}
}

[HarmonyPatch(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.ShouldBeDeadFromRequiredCapacity))]
internal static class Pawn_HealthTracker_ShouldBeDeadFromRequiredCapacity_Patch
{
	internal static bool Prefix(ref Pawn_HealthTracker __instance, ref PawnCapacityDef? __result)
	{
		Pawn_HealthTracker health = __instance;
		Pawn pawn = health.hediffSet.pawn;

		if (!health.hediffSet.HasHediff(HediffDefOf.QE_LifeSupport) || !pawn.ValidLifeSupportNearby() ||
		    !health.capacities.CapableOf(PawnCapacityDefOf.Consciousness)) return true;
		__result = null;
		return false;
	}
}

[HarmonyPatch(typeof(Toils_LayDown), nameof(Toils_LayDown.LayDown))]
internal static class Toils_LayDown_LayDown_Patch
{
	internal static void Postfix(ref Toil? __result)
	{
		Toil? toil = __result;

		toil?.AddPreTickAction(() =>
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

[HarmonyPatch(typeof(PawnCapacitiesHandler), nameof(PawnCapacitiesHandler.Notify_CapacityLevelsDirty))]
internal static class PawnCapacitiesHandler_Notify_CapacityLevelsDirty_Patch
{
	internal static void Postfix(Pawn ___pawn)
	{
		if (Scribe.mode == LoadSaveMode.LoadingVars) return;
		if (!___pawn.health.ShouldBeDead())
			LifeSupportUtility.SetHediffs(___pawn);
	}
}

[HarmonyPatch(typeof(HediffSet), nameof(HediffSet.DirtyCache))]
[HarmonyPatchCategory("DeathRattle")]
internal static class HediffSet_DirtyCache_Patch
{
	internal static void Postfix(Pawn ___pawn)
	{
		if (!___pawn.health.ShouldBeDead())
		{
			LifeSupportUtility.SetHediffs(___pawn);
		}
	}
}