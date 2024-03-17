using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace LifeSupport {
    [StaticConstructorOnStartup]
    public static class LifeSupportUtility
    {
        public static readonly bool hasDeathRattle = false;
        public static HashSet<PawnCapacityDef> capacitySetFlesh = new HashSet<PawnCapacityDef>();
        public static HashSet<PawnCapacityDef> capacitySetMechanoid = new HashSet<PawnCapacityDef>();
        public static HashSet<HediffDef> deathRattleHediffs = new HashSet<HediffDef>();
        static LifeSupportUtility() {
            if (ModsConfig.IsActive("Troopersmith1.DeathRattle"))
            {
                hasDeathRattle = true;
                deathRattleHediffs = new HashSet<HediffDef>() { HediffDef.Named("IntestinalFailure"), HediffDef.Named("LiverFailure"), HediffDef.Named("KidneyFailure"), HediffDef.Named("ClinicalDeathNoHeartbeat"), HediffDef.Named("ClinicalDeathAsphyxiation") };
            }
            capacitySetFlesh = DefDatabase<PawnCapacityDef>.AllDefsListForReading.Where((capacity) => capacity.lethalFlesh).ToHashSet();
            capacitySetMechanoid = DefDatabase<PawnCapacityDef>.AllDefsListForReading.Where((capacity) => capacity.lethalMechanoids).ToHashSet();
        }
        public static bool ValidLifeSupportNearby(this Pawn pawn) => pawn.CurrentBed() is Building_Bed bed
                                                                     && GenAdj.CellsAdjacent8Way(bed)
                                                                              .Any(cell => cell.GetFirstThingWithComp<LifeSupportComp>(bed.Map) != null
                                                                                               && cell.GetFirstThingWithComp<LifeSupportComp>(bed.Map).GetComp<LifeSupportComp>().Active);

        internal static bool NeedsLifeSupport(this Pawn pawn) => (pawn.RaceProps.IsFlesh ? capacitySetFlesh : capacitySetMechanoid).Any(capacity => !pawn.health.capacities.CapableOf(capacity))
                                                                 || (hasDeathRattle && deathRattleHediffs.Any(hediff => pawn.health.hediffSet.HasHediff(hediff)));

        public static void SetHediffs(Pawn pawn) {
            bool validLifeSupportNearby = pawn.ValidLifeSupportNearby();
            SetHediffs(pawn, validLifeSupportNearby);
        }

        public static void SetHediffs(Pawn pawn, bool validLifeSupportNearby) {

            Hediff hediff_lifesupport = pawn.health.hediffSet.GetFirstHediffOfDef(LifeSupportDefOf.QE_LifeSupport);

            if (validLifeSupportNearby) {
                if (hediff_lifesupport is null) {
                    hediff_lifesupport = pawn.health.AddHediff(LifeSupportDefOf.QE_LifeSupport);
                }
                hediff_lifesupport.Severity = pawn.NeedsLifeSupport() ? 1.0f : 0.5f;
            } else if (!(hediff_lifesupport is null)) {
                pawn.health.RemoveHediff(hediff_lifesupport);
            }
        }
    }
}
