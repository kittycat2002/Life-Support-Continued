using JetBrains.Annotations;
using Verse;

namespace LifeSupport;

public class HediffComp_OnLifeSupport : HediffComp_SeverityModifierBase
{
    private HediffCompProperties_OnLifeSupport Props => (HediffCompProperties_OnLifeSupport)props;
    private float SeverityPerDayOnLifeSupport => Props.severityPerDayOnLifeSupport;
    private float SeverityPerDayNotOnLifeSupport => Props.severityPerDayNotOnLifeSupport;

    public override float SeverityChangePerDay()
    {
        return Pawn.health.hediffSet.HasHediff(HediffDefOf.QE_LifeSupport) ? SeverityPerDayOnLifeSupport : SeverityPerDayNotOnLifeSupport;
    }
}
public class HediffCompProperties_OnLifeSupport : HediffCompProperties
{
    [UsedImplicitly] public float severityPerDayOnLifeSupport;
    [UsedImplicitly] public float severityPerDayNotOnLifeSupport;
    public HediffCompProperties_OnLifeSupport()
    {
        compClass = typeof(HediffComp_OnLifeSupport);
    }
}