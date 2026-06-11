using System;
using System.Collections.Generic;

namespace Planetbase
{
	// Token: 0x02000099 RID: 153
	public class VisitorAi : BaseAi
	{
		// Token: 0x0600024B RID: 587 RVA: 0x00012270 File Offset: 0x00010470
		public VisitorAi()
		{
			this.mIdleRules = new List<AiRule>();
			this.mTargetRules = new List<AiTargetRule>();
			base.addHumanSurvivalRules();
			this.mIdleRules.Add(new AiRuleGoGetResourceToConsume<AlcoholicDrink>(CharacterIndicator.Morale, IndicatorLevel.Suboptimal));
			this.mIdleRules.Add(new AiRuleGoRelax(IndicatorLevel.Suboptimal));
			this.mIdleRules.Add(new AiRuleGoBackToShip());
			this.mIdleRules.Add(new AiRuleGoInterior());
			this.mIdleRules.Add(new AiRuleWanderInterior());
			base.addHumanSurvivalTargetRules();
			this.mTargetRules.Add(new AiRuleRelax());
			this.mTargetRules.Add(new AiRuleEnterShip());
			this.mTargetRules.Add(new AiRuleSetIdle());
		}

		// Token: 0x0600024C RID: 588 RVA: 0x00012328 File Offset: 0x00010528
		public static VisitorAi getInstance()
		{
			if (VisitorAi.mAi == null)
			{
				VisitorAi.mAi = new VisitorAi();
			}
			return VisitorAi.mAi;
		}

		// Token: 0x04000327 RID: 807
		private static VisitorAi mAi;
	}
}
