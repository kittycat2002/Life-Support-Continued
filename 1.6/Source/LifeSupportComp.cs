using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Noise;

namespace LifeSupport;

/// <summary>
/// Tags this Thing as being valid life support.
/// </summary>
public class LifeSupportComp : ThingComp {
    public bool Active => parent.TryGetComp<CompPowerTrader>() is not { } power || power.PowerOn;

    public override void ReceiveCompSignal(string signal) {
        if (signal is not ("PowerTurnedOn" or "PowerTurnedOff")) {
            return;
        }
            
        //Check for state change in surrounding pawns in beds.
        Map map = parent.Map;
        List<Pawn> pawns = parent.CellsAdjacent8WayAndInside()
            .SelectMany(cell => cell.GetThingList(map).OfType<Building_Bed>().SelectMany(bed => bed.CurOccupants))
            .Where(pawn => !pawn.health.Dead)
            .Distinct()
            .ToList();
            
        foreach (Pawn pawn in pawns) {
            LifeSupportUtility.SetHediffs(pawn);
        }
    }
    public override void PostDestroy(DestroyMode mode, Map previousMap)
    {
        List<Pawn> pawns = parent.CellsAdjacent8WayAndInside()
            .SelectMany(cell => cell.GetThingList(previousMap).OfType<Building_Bed>().SelectMany(bed => bed.CurOccupants))
            .Where(pawn => !pawn.health.Dead)
            .Distinct()
            .ToList();
            
        foreach (Pawn pawn in pawns) {
            LifeSupportUtility.SetHediffs(pawn, false);
        }
    }
}