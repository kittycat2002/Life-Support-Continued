﻿using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Noise;

namespace LifeSupport;

/// <summary>
/// Tags this Thing as being valid life support.
/// </summary>
public class LifeSupportComp : ThingComp
{
	public bool Active => parent.TryGetComp<CompPowerTrader>() is not { } power || power.PowerOn;

	public override void ReceiveCompSignal(string signal)
	{
		if (signal is not ("PowerTurnedOn" or "PowerTurnedOff") || parent.Map == null)
		{
			return;
		}

		//Check for state change in surrounding pawns in beds.
		List<Pawn> pawns = parent.CellsAdjacent8WayAndInside()
			.SelectMany(cell =>
				cell.GetThingList(parent.Map).OfType<Building_Bed>().SelectMany(bed => bed.CurOccupants))
			.Where(pawn => !pawn.health.Dead)
			.Distinct()
			.ToList();

		foreach (Pawn pawn in pawns)
		{
			LifeSupportUtility.SetHediffs(pawn);
		}
	}

	public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
	{
		if (mode == DestroyMode.WillReplace)
		{
			base.PostDeSpawn(map, mode);
			return;
		}

		List<Pawn> pawns = parent.CellsAdjacent8WayAndInside()
			.SelectMany(cell => cell.GetThingList(map).OfType<Building_Bed>().SelectMany(bed => bed.CurOccupants))
			.Where(pawn => !pawn.health.Dead)
			.Distinct()
			.ToList();

		foreach (Pawn pawn in pawns)
		{
			LifeSupportUtility.SetHediffs(pawn, false);
		}

		base.PostDeSpawn(map, mode);
	}
}