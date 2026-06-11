using System;
using System.Collections.Generic;

namespace Planetbase
{
	// Token: 0x02000096 RID: 150
	public class GuardAi : BaseAi
	{
		// Token: 0x06000245 RID: 581 RVA: 0x00011D80 File Offset: 0x0000FF80
		public GuardAi()
		{
			this.mIdleRules = new List<AiRule>();
			this.mTargetRules = new List<AiTargetRule>();
			this.mIdleRules.Add(new AiRuleGoGetWeapon());
			this.mIdleRules.Add(new AiRuleGoAttackIntruder());
			base.addHumanSurvivalRules();
			this.mIdleRules.Add(new AiRuleGoOperate(AiRule.Priority.HighPriorityOnly));
			this.mIdleRules.Add(new AiRuleGoOperate(AiRule.Priority.All));
			this.mIdleRules.Add(new AiRuleGoGetResourceToConsume<AlcoholicDrink>(CharacterIndicator.Morale, IndicatorLevel.Suboptimal));
			this.mIdleRules.Add(new AiRuleGoRelax(IndicatorLevel.Suboptimal));
			this.mIdleRules.Add(new AiRuleGoInterior());
			this.mIdleRules.Add(new AiRuleWanderInterior());
			this.mTargetRules.Add(new AiRulePickUpWeapon());
			base.addHumanSurvivalTargetRules();
			this.mTargetRules.Add(new AiRuleCombat());
			this.mTargetRules.Add(new AiRuleOperate());
			this.mTargetRules.Add(new AiRuleRelax());
			this.mTargetRules.Add(new AiRuleSetIdle());
		}

		// Token: 0x06000246 RID: 582 RVA: 0x00011E8A File Offset: 0x0001008A
		public static GuardAi getInstance()
		{
			if (GuardAi.mGuardAi == null)
			{
				GuardAi.mGuardAi = new GuardAi();
			}
			return GuardAi.mGuardAi;
		}

		// Token: 0x04000324 RID: 804
		private static GuardAi mGuardAi;
	}
}
