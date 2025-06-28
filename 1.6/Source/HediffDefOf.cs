using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace LifeSupport;

[DefOf]
internal static class HediffDefOf
{
	[UsedImplicitly] public static HediffDef? QE_LifeSupport;

	[MayRequire("Troopersmith1.DeathRattle")] [UsedImplicitly]
	internal static HediffDef? IntestinalFailure;
	
	[MayRequire("Troopersmith1.DeathRattle")] [UsedImplicitly]
	internal static HediffDef? LiverFailure;
	
	[MayRequire("Troopersmith1.DeathRattle")] [UsedImplicitly]
	internal static HediffDef? KidneyFailure;
	
	[MayRequire("Troopersmith1.DeathRattle")] [UsedImplicitly]
	internal static HediffDef? ClinicalDeathNoHeartbeat;
	
	[MayRequire("Troopersmith1.DeathRattle")] [UsedImplicitly]
	internal static HediffDef? ClinicalDeathAsphyxiation;

	static HediffDefOf()
	{
		DefOfHelper.EnsureInitializedInCtor(typeof(HediffDefOf));
	}
}