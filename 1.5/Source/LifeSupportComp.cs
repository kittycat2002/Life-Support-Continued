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

            foreach (var pawn in parent.CellsAdjacent8WayAndInside()
                                       .SelectMany(cell => cell.GetThingList(map).Where(thing => thing is Building_Bed).SelectMany(thing => (thing as Building_Bed).CurOccupants))
                                       .Where(pawn => !pawn.health.Dead)
                                       .Distinct()) {
                LifeSupportUtility.SetHediffs(pawn);
            }
        }
        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            foreach (var pawn in parent.CellsAdjacent8WayAndInside()
                                       .SelectMany(cell => cell.GetThingList(previousMap).Where(thing => thing is Building_Bed).SelectMany(thing => (thing as Building_Bed).CurOccupants))
                                       .Where(pawn => !pawn.health.Dead)
                                       .Distinct())
            {
                LifeSupportUtility.SetHediffs(pawn, false);
            }
        }
    }
}
