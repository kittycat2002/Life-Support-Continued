using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace LifeSupport;

[StaticConstructorOnStartup]
internal static class LifeSupportUtility
{
	private static readonly bool hasDeathRattle;
	private static readonly HashSet<PawnCapacityDef> capacitySetFlesh;
	private static readonly HashSet<PawnCapacityDef> capacitySetMechanoid;
	private static readonly HashSet<HediffDef> deathRattleHediffs = [];

	static LifeSupportUtility()
	{
		if (ModsConfig.IsActive("Troopersmith1.DeathRattle"))
		{
			hasDeathRattle = true;
			deathRattleHediffs =
			[
				HediffDefOf.IntestinalFailure!, HediffDefOf.LiverFailure!, HediffDefOf.KidneyFailure!,
				HediffDefOf.ClinicalDeathNoHeartbeat!, HediffDefOf.ClinicalDeathAsphyxiation!
			];
		}

		capacitySetFlesh = DefDatabase<PawnCapacityDef>.AllDefsListForReading.Where(capacity => capacity.lethalFlesh)
			.ToHashSet();
		capacitySetMechanoid = DefDatabase<PawnCapacityDef>.AllDefsListForReading
			.Where(capacity => capacity.lethalMechanoids).ToHashSet();
	}

	internal static bool ValidLifeSupportNearby(this Pawn pawn) => pawn.CurrentBed() is { } bed
	                                                               && GenAdj.CellsAdjacent8Way(bed)
		                                                               .Any(cell =>
			                                                               cell.GetFirstThingWithComp<LifeSupportComp>(
				                                                               bed.Map) != null
			                                                               && cell
				                                                               .GetFirstThingWithComp<LifeSupportComp>(
					                                                               bed.Map).GetComp<LifeSupportComp>()
				                                                               .Active);

	private static bool NeedsLifeSupport(this Pawn pawn) =>
		(pawn.RaceProps.IsFlesh ? capacitySetFlesh : capacitySetMechanoid).Any(capacity =>
			!pawn.health.capacities.CapableOf(capacity))
		|| (hasDeathRattle && deathRattleHediffs.Any(hediff => pawn.health.hediffSet.HasHediff(hediff)));

	internal static void SetHediffs(Pawn pawn)
	{
		bool validLifeSupportNearby = pawn.ValidLifeSupportNearby();
		SetHediffs(pawn, validLifeSupportNearby);
	}

	internal static void SetHediffs(Pawn pawn, bool validLifeSupportNearby)
	{
		Hediff hediffLifeSupport = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.QE_LifeSupport);

		if (validLifeSupportNearby)
		{
			hediffLifeSupport ??= pawn.health.AddHediff(HediffDefOf.QE_LifeSupport);
			hediffLifeSupport.Severity = pawn.NeedsLifeSupport() ? 1.0f : 0.5f;
		}
		else if (hediffLifeSupport is not null)
		{
			pawn.health.RemoveHediff(hediffLifeSupport);
		}
	}
}