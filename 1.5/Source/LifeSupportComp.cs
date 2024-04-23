using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Noise;

namespace LifeSupport {
    /// <summary>
    /// Tags this Thing as being valid life support.
    /// </summary>
    public class LifeSupportComp : ThingComp {
        public bool Active => !(parent.TryGetComp<CompPowerTrader>() is CompPowerTrader power) || power.PowerOn;

        public override void ReceiveCompSignal(string signal) {
            if (!(signal == "PowerTurnedOn" || signal == "PowerTurnedOff")) {
                return;
            }
            
            //Check for state change in surrounding pawns in beds.
            Map map = parent.Map;
            var pawns = parent.CellsAdjacent8WayAndInside()
                                .SelectMany(cell => cell.GetThingList(map).OfType<Building_Bed>().SelectMany(bed => bed.CurOccupants))
                                .Where(pawn => !pawn.health.Dead)
                                .Distinct()
                                .ToList();
            
            foreach (var pawn in pawns) {
                LifeSupportUtility.SetHediffs(pawn);
            }
        }
        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            var pawns = parent.CellsAdjacent8WayAndInside()
                                .SelectMany(cell => cell.GetThingList(previousMap).OfType<Building_Bed>().SelectMany(bed => bed.CurOccupants))
                                .Where(pawn => !pawn.health.Dead)
                                .Distinct()
                                .ToList();
            
            foreach (var pawn in pawns) {
                LifeSupportUtility.SetHediffs(pawn, false);
            }
        }
    }
}
