using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace LifeSupport
{
    public class HediffComp_OnLifeSupport : HediffComp_SeverityModifierBase
    {
        public HediffCompProperties_OnLifeSupport Props => (HediffCompProperties_OnLifeSupport)props;
        public float SeverityPerDayOnLifeSupport => Props.severityPerDayOnLifeSupport;
        public float SeverityPerDayNotOnLifeSupport => Props.severityPerDayNotOnLifeSupport;

        public override float SeverityChangePerDay()
        {
            Log.Message($"{Pawn.health.hediffSet.HasHediff(LifeSupportDefOf.QE_LifeSupport)},{SeverityPerDayOnLifeSupport},{SeverityPerDayNotOnLifeSupport}");
            return Pawn.health.hediffSet.HasHediff(LifeSupportDefOf.QE_LifeSupport) ? SeverityPerDayOnLifeSupport : SeverityPerDayNotOnLifeSupport;
        }
    }
    public class HediffCompProperties_OnLifeSupport : HediffCompProperties
    {
        public float severityPerDayOnLifeSupport;
        public float severityPerDayNotOnLifeSupport;
        public HediffCompProperties_OnLifeSupport()
        {
            compClass = typeof(HediffComp_OnLifeSupport);
        }
    }
}