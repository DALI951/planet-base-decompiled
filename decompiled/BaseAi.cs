using System;
using System.Collections.Generic;

namespace Planetbase
{
	// Token: 0x02000092 RID: 146
	public abstract class BaseAi
	{
		// Token: 0x0600023A RID: 570 RVA: 0x00011796 File Offset: 0x0000F996
		public BaseAi()
		{
			this.mRuleTimer = new PerformanceTimer(base.GetType().Name + " rules", 0);
		}

		// Token: 0x0600023B RID: 571 RVA: 0x000117C0 File Offset: 0x0000F9C0
		public void updateIdle(Character character)
		{
			this.mRuleTimer.start();
			if (character.isConscious())
			{
				int count = this.mIdleRules.Count;
				for (int i = 0; i < count; i++)
				{
					AiRule aiRule = this.mIdleRules[i];
					if (aiRule.update(character))
					{
						Singleton<DebugManager>.getInstance().logAi(character, " executing rule " + aiRule.GetType().Name);
						character.setCurrentAiRule(aiRule);
						break;
					}
				}
			}
			this.mRuleTimer.stop();
		}

		// Token: 0x0600023C RID: 572 RVA: 0x00011844 File Offset: 0x0000FA44
		public void onTargetReached(Character character)
		{
			int count = this.mTargetRules.Count;
			for (int i = 0; i < count; i++)
			{
				AiTargetRule aiTargetRule = this.mTargetRules[i];
				if (aiTargetRule.update(character))
				{
					Singleton<DebugManager>.getInstance().logAi(character, " executing target rule " + aiTargetRule.GetType().Name);
					return;
				}
			}
		}

		// Token: 0x0600023D RID: 573 RVA: 0x000118A0 File Offset: 0x0000FAA0
		protected void addHumanSurvivalRules()
		{
			this.mIdleRules.Add(new AiRuleGoInteriorLowOxygen());
			this.mIdleRules.Add(new AiRuleGoDrink(IndicatorLevel.Low));
			this.mIdleRules.Add(new AiRuleGoGetResourceToConsume<Meal>(CharacterIndicator.Nutrition, IndicatorLevel.Low));
			this.mIdleRules.Add(new AiRuleGoGetHealing());
			this.mIdleRules.Add(new AiRuleGoDrink(IndicatorLevel.Suboptimal));
			this.mIdleRules.Add(new AiRuleGoGetResourceToConsume<Meal>(CharacterIndicator.Nutrition, IndicatorLevel.Suboptimal));
			this.mIdleRules.Add(new AiRuleGoConsumeResource());
			this.mIdleRules.Add(new AiRuleGoSleep());
		}

		// Token: 0x0600023E RID: 574 RVA: 0x00011934 File Offset: 0x0000FB34
		protected void addHumanSurvivalTargetRules()
		{
			this.mTargetRules.Add(new AiRuleAirlockInteraction());
			this.mTargetRules.Add(new AiRuleDrink());
			this.mTargetRules.Add(new AiRuleGetHealing());
			this.mTargetRules.Add(new AiRuleLoadResource());
			this.mTargetRules.Add(new AiRuleConsumeResource());
			this.mTargetRules.Add(new AiRuleSleep());
		}

		// Token: 0x0400031E RID: 798
		protected List<AiRule> mIdleRules;

		// Token: 0x0400031F RID: 799
		protected List<AiTargetRule> mTargetRules;

		// Token: 0x04000320 RID: 800
		protected PerformanceTimer mRuleTimer;
	}
}
