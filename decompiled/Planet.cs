using System;
using System.Collections.Generic;
using UnityEngine;

namespace Planetbase
{
	// Token: 0x020001FB RID: 507
	public abstract class Planet : Localizable
	{
		// Token: 0x06000EB4 RID: 3764 RVA: 0x0005504A File Offset: 0x0005324A
		public Planet()
		{
		}

		// Token: 0x06000EB5 RID: 3765 RVA: 0x00055086 File Offset: 0x00053286
		public void destroy()
		{
			Object.Destroy(this.mDefinition.gameObject);
			Object.Destroy(this.mDefinition.EnvironmentParameters);
		}

		// Token: 0x06000EB6 RID: 3766 RVA: 0x000550A8 File Offset: 0x000532A8
		public void initStrings()
		{
			this.mName = StringList.get(("planet_" + this.mLetter.ToString() + "_name").ToLower());
			this.mDifficultyString = StringList.get(("planet_" + this.mLetter.ToString() + "_difficulty").ToLower());
			this.mDescription = StringList.get(("planet_" + this.mLetter.ToString() + "_description").ToLower());
			this.mCharacteristicsString = string.Concat(new string[]
			{
				StringList.get("light_amount"),
				": ",
				Planet.lightAmountToString(this.mLightAmount),
				"\n",
				StringList.get("atmosphere_density"),
				": ",
				Planet.atmosphereDensityToString(this.mAtmosphereDensity),
				"\n"
			});
			if (this.mSandstormRisk > Planet.Quantity.None)
			{
				this.mCharacteristicsString = string.Concat(new string[]
				{
					this.mCharacteristicsString,
					StringList.get("sandstorm_risk"),
					": ",
					Planet.quantityToString(this.mSandstormRisk),
					"\n"
				});
			}
			if (this.mSolarFlareRisk > Planet.Quantity.None)
			{
				this.mCharacteristicsString = string.Concat(new string[]
				{
					this.mCharacteristicsString,
					StringList.get("solar_flare_risk"),
					": ",
					Planet.quantityToString(this.mSolarFlareRisk),
					"\n"
				});
			}
			if (this.mBlizzardRisk > Planet.Quantity.None)
			{
				this.mCharacteristicsString = string.Concat(new string[]
				{
					this.mCharacteristicsString,
					StringList.get("blizzard_risk"),
					": ",
					Planet.quantityToString(this.mBlizzardRisk),
					"\n"
				});
			}
			if (this.mMeteorRisk > Planet.Quantity.None)
			{
				this.mCharacteristicsString = string.Concat(new string[]
				{
					this.mCharacteristicsString,
					StringList.get("meteor_risk"),
					": ",
					Planet.quantityToString(this.mMeteorRisk),
					"\n"
				});
			}
			if (this.mThunderstormRisk > Planet.Quantity.None)
			{
				this.mCharacteristicsString = string.Concat(new string[]
				{
					this.mCharacteristicsString,
					StringList.get("thunderstorm_risk"),
					": ",
					Planet.quantityToString(this.mThunderstormRisk),
					"\n"
				});
			}
		}

		// Token: 0x06000EB7 RID: 3767 RVA: 0x00055320 File Offset: 0x00053520
		public GameObject getPrefab()
		{
			return this.mDefinition.Prefab;
		}

		// Token: 0x06000EB8 RID: 3768 RVA: 0x0005532D File Offset: 0x0005352D
		public string getName()
		{
			return this.mName;
		}

		// Token: 0x06000EB9 RID: 3769 RVA: 0x00055335 File Offset: 0x00053535
		public string getDescription()
		{
			return this.mDescription;
		}

		// Token: 0x06000EBA RID: 3770 RVA: 0x0005533D File Offset: 0x0005353D
		public GameObject getBotDust()
		{
			return this.mDefinition.BotDust;
		}

		// Token: 0x06000EBB RID: 3771 RVA: 0x0005534C File Offset: 0x0005354C
		public Texture2D loadTexture(Planet.TerrainTexture terrainTexture)
		{
			string text = "Textures/" + this.mTexures[(int)terrainTexture] + "_df";
			Texture2D texture2D = Resources.Load<Texture2D>(text);
			texture2D.name = text;
			if (texture2D == null)
			{
				Debug.LogWarning("Can not load texture: " + text);
			}
			return texture2D;
		}

		// Token: 0x06000EBC RID: 3772 RVA: 0x00055398 File Offset: 0x00053598
		public Texture2D loadNormalMap(Planet.TerrainTexture terrainTexture)
		{
			string text = "Textures/" + this.mTexures[(int)terrainTexture] + "_nm";
			Texture2D texture2D = Resources.Load<Texture2D>(text);
			texture2D.name = text;
			if (texture2D == null)
			{
				Debug.LogWarning("Can not load normal map: " + text);
			}
			return texture2D;
		}

		// Token: 0x06000EBD RID: 3773 RVA: 0x000553E3 File Offset: 0x000535E3
		public int getMilestonesToUnlock()
		{
			return this.mMilestonesToUnlock;
		}

		// Token: 0x06000EBE RID: 3774 RVA: 0x000553EB File Offset: 0x000535EB
		public List<SpecializationCount> getStartingSpecializations()
		{
			return this.mStartingSpecializations;
		}

		// Token: 0x06000EBF RID: 3775 RVA: 0x000553F3 File Offset: 0x000535F3
		public ResourceAmounts getStartingResources()
		{
			return this.mStartingResources;
		}

		// Token: 0x06000EC0 RID: 3776 RVA: 0x000553FB File Offset: 0x000535FB
		protected void addStartingSpecialization<T>(int count)
		{
			this.mStartingSpecializations.Add(new SpecializationCount(TypeList<Specialization, SpecializationList>.find<T>(), count));
		}

		// Token: 0x06000EC1 RID: 3777 RVA: 0x00055413 File Offset: 0x00053613
		protected void addStartingResource<T>(int count)
		{
			this.mStartingResources.add(TypeList<ResourceType, ResourceTypeList>.find<T>(), count);
		}

		// Token: 0x06000EC2 RID: 3778 RVA: 0x00055426 File Offset: 0x00053626
		public GameObject[] getBackdrops()
		{
			return this.mDefinition.Backdrops;
		}

		// Token: 0x06000EC3 RID: 3779 RVA: 0x00055433 File Offset: 0x00053633
		public Planet.Quantity getAtmosphereDensity()
		{
			return this.mAtmosphereDensity;
		}

		// Token: 0x06000EC4 RID: 3780 RVA: 0x0005543B File Offset: 0x0005363B
		public Planet.Quantity getSandstormRisk()
		{
			return this.mSandstormRisk;
		}

		// Token: 0x06000EC5 RID: 3781 RVA: 0x00055443 File Offset: 0x00053643
		public Planet.Quantity getBlizzardRisk()
		{
			return this.mBlizzardRisk;
		}

		// Token: 0x06000EC6 RID: 3782 RVA: 0x0005544B File Offset: 0x0005364B
		public Planet.Quantity getLightAmount()
		{
			return this.mLightAmount;
		}

		// Token: 0x06000EC7 RID: 3783 RVA: 0x00055453 File Offset: 0x00053653
		public Planet.Quantity getSolarFlareRisk()
		{
			return this.mSolarFlareRisk;
		}

		// Token: 0x06000EC8 RID: 3784 RVA: 0x0005545B File Offset: 0x0005365B
		public Planet.Quantity getMeteorRisk()
		{
			return this.mMeteorRisk;
		}

		// Token: 0x06000EC9 RID: 3785 RVA: 0x00055463 File Offset: 0x00053663
		public bool anyMeteorRisk()
		{
			return this.mMeteorRisk > Planet.Quantity.None;
		}

		// Token: 0x06000ECA RID: 3786 RVA: 0x0005546E File Offset: 0x0005366E
		public bool anyPredictableDisasters()
		{
			return this.mMeteorRisk != Planet.Quantity.None || this.mSandstormRisk != Planet.Quantity.None || this.mBlizzardRisk != Planet.Quantity.None || this.mSolarFlareRisk > Planet.Quantity.None;
		}

		// Token: 0x06000ECB RID: 3787 RVA: 0x00055493 File Offset: 0x00053693
		public Planet.Quantity getThunderstormRisk()
		{
			return this.mThunderstormRisk;
		}

		// Token: 0x06000ECC RID: 3788 RVA: 0x0005549B File Offset: 0x0005369B
		public bool anyThunderstormRisk()
		{
			return this.mThunderstormRisk > Planet.Quantity.None;
		}

		// Token: 0x06000ECD RID: 3789 RVA: 0x000554A6 File Offset: 0x000536A6
		public string getDifficultyString()
		{
			return this.mDifficultyString;
		}

		// Token: 0x06000ECE RID: 3790 RVA: 0x000554AE File Offset: 0x000536AE
		public static string quantityToString(Planet.Quantity quantity)
		{
			switch (quantity)
			{
			case Planet.Quantity.None:
				return StringList.get("quantity_none");
			case Planet.Quantity.Low:
				return StringList.get("quantity_low");
			case Planet.Quantity.High:
				return StringList.get("quantity_high");
			default:
				return "Unknown";
			}
		}

		// Token: 0x06000ECF RID: 3791 RVA: 0x000554EA File Offset: 0x000536EA
		public static string atmosphereDensityToString(Planet.Quantity quantity)
		{
			return StringList.get("atmosphere_density_" + quantity.ToString().ToLower());
		}

		// Token: 0x06000ED0 RID: 3792 RVA: 0x0005550D File Offset: 0x0005370D
		public static string lightAmountToString(Planet.Quantity quantity)
		{
			return StringList.get("light_amount_" + quantity.ToString().ToLower());
		}

		// Token: 0x06000ED1 RID: 3793 RVA: 0x00055530 File Offset: 0x00053730
		public string getCharacteristicsString()
		{
			return this.mCharacteristicsString;
		}

		// Token: 0x06000ED2 RID: 3794 RVA: 0x00055538 File Offset: 0x00053738
		public PlanetDefinition getDefinition()
		{
			return this.mDefinition;
		}

		// Token: 0x06000ED3 RID: 3795 RVA: 0x00055540 File Offset: 0x00053740
		public bool isAvailable()
		{
			return !this.getDefinition().Locked;
		}

		// Token: 0x06000ED4 RID: 3796 RVA: 0x00055550 File Offset: 0x00053750
		public int getStartingHumans()
		{
			int num = 0;
			for (int i = 0; i < this.mStartingSpecializations.Count; i++)
			{
				if (this.mStartingSpecializations[i].getSpecialization().hasFlag(128))
				{
					num += this.mStartingSpecializations[i].getCount();
				}
			}
			return num;
		}

		// Token: 0x06000ED5 RID: 3797 RVA: 0x000555A7 File Offset: 0x000537A7
		public char getLetter()
		{
			return this.mLetter;
		}

		// Token: 0x06000ED6 RID: 3798 RVA: 0x000555AF File Offset: 0x000537AF
		public float getIntruderMinPrestige()
		{
			return this.mIntruderMinPrestige;
		}

		// Token: 0x06000ED7 RID: 3799 RVA: 0x000555B7 File Offset: 0x000537B7
		public float getColonyShipRotation()
		{
			return this.mColonyShipRotation;
		}

		// Token: 0x06000ED8 RID: 3800 RVA: 0x000555BF File Offset: 0x000537BF
		public int getInitialMusicTrack()
		{
			return this.mInitialMusicTrack;
		}

		// Token: 0x06000ED9 RID: 3801 RVA: 0x000555C7 File Offset: 0x000537C7
		public int getExtraIntruders()
		{
			return this.mExtraIntruders;
		}

		// Token: 0x06000EDA RID: 3802 RVA: 0x000555CF File Offset: 0x000537CF
		public float getFlareScale()
		{
			return this.mFlareScale;
		}

		// Token: 0x06000EDB RID: 3803 RVA: 0x000555D7 File Offset: 0x000537D7
		public Material createLiquidMaterial()
		{
			return Object.Instantiate<Material>(ResourceList.getInstance().Materials.Liquid);
		}

		// Token: 0x06000EDC RID: 3804 RVA: 0x000555F0 File Offset: 0x000537F0
		public Planet clone()
		{
			Planet planet = (Planet)Activator.CreateInstance(base.GetType());
			planet.mExtraIntruders = this.mExtraIntruders;
			planet.mStartingResources = this.mStartingResources;
			planet.mStartingSpecializations = this.mStartingSpecializations;
			planet.mMilestonesToUnlock = this.mMilestonesToUnlock;
			planet.mTexures = this.mTexures;
			planet.mAtmosphereDensity = this.mAtmosphereDensity;
			planet.mLightAmount = this.mLightAmount;
			planet.mBlizzardRisk = this.mBlizzardRisk;
			planet.mSandstormRisk = this.mSandstormRisk;
			planet.mSolarFlareRisk = this.mSolarFlareRisk;
			planet.mMeteorRisk = this.mMeteorRisk;
			planet.mThunderstormRisk = this.mThunderstormRisk;
			planet.mCharacteristicsString = this.mCharacteristicsString;
			planet.mName = this.mName;
			planet.mDifficultyString = this.mDifficultyString;
			planet.mDescription = this.mDescription;
			planet.mDefinition = Object.Instantiate<PlanetDefinition>(this.mDefinition);
			planet.mDefinition.EnvironmentParameters = Object.Instantiate<GameObject>(planet.mDefinition.EnvironmentParameters);
			planet.mLetter = this.mLetter;
			planet.mIntruderMinPrestige = this.mIntruderMinPrestige;
			planet.mColonyShipRotation = this.mColonyShipRotation;
			planet.mInitialMusicTrack = this.mInitialMusicTrack;
			planet.mFlareScale = this.mFlareScale;
			return planet;
		}

		// Token: 0x06000EDD RID: 3805 RVA: 0x00055738 File Offset: 0x00053938
		public void applyCustomParameters(Challenge challenge)
		{
			challenge.applyTerrainModifiers(this.mDefinition);
			challenge.applyEnvironmentModifiers(this.mDefinition, this.mDefinition.EnvironmentParameters.GetComponent<EnvironmentParameters>());
			List<SpecializationCount> startingSpecializations = challenge.getStartingSpecializations();
			if (startingSpecializations != null)
			{
				this.mStartingSpecializations = startingSpecializations;
			}
			ResourceAmounts startingResources = challenge.getStartingResources();
			if (startingResources != null)
			{
				this.mStartingResources = startingResources;
			}
			GameplayModifier gameplayModifier = challenge.getGameplayModifier(GameplayModifierType.SandstormRisk);
			if (gameplayModifier != null)
			{
				this.mSandstormRisk = gameplayModifier.getQuantity();
			}
			GameplayModifier gameplayModifier2 = challenge.getGameplayModifier(GameplayModifierType.MeteorRisk);
			if (gameplayModifier2 != null)
			{
				this.mMeteorRisk = gameplayModifier2.getQuantity();
			}
			GameplayModifier gameplayModifier3 = challenge.getGameplayModifier(GameplayModifierType.BlizzardRisk);
			if (gameplayModifier3 != null)
			{
				this.mBlizzardRisk = gameplayModifier3.getQuantity();
			}
			GameplayModifier gameplayModifier4 = challenge.getGameplayModifier(GameplayModifierType.SolarFlareRisk);
			if (gameplayModifier4 != null)
			{
				this.mSolarFlareRisk = gameplayModifier4.getQuantity();
			}
			GameplayModifier gameplayModifier5 = challenge.getGameplayModifier(GameplayModifierType.ThunderstormRisk);
			if (gameplayModifier5 != null)
			{
				this.mThunderstormRisk = gameplayModifier5.getQuantity();
				PlanetDefinition planetDefinition = TypeList<Planet, PlanetList>.get()[3].mDefinition;
				EnvironmentParameters environmentParameters = planetDefinition.getEnvironmentParameters();
				this.mDefinition.MaterialSkydomeClose = planetDefinition.MaterialSkydomeClose;
				this.mDefinition.getEnvironmentParameters().EnvironmentParticles = environmentParameters.EnvironmentParticles;
			}
			GameplayModifier gameplayModifier6 = challenge.getGameplayModifier(GameplayModifierType.LightAmount);
			if (gameplayModifier6 != null)
			{
				this.mLightAmount = gameplayModifier6.getQuantity();
			}
			GameplayModifier gameplayModifier7 = challenge.getGameplayModifier(GameplayModifierType.AtmosphereDensity);
			if (gameplayModifier7 != null)
			{
				this.mAtmosphereDensity = gameplayModifier7.getQuantity();
			}
			GameplayModifier gameplayModifier8 = challenge.getGameplayModifier(GameplayModifierType.ExtraIntruders);
			if (gameplayModifier8 != null)
			{
				this.mExtraIntruders = gameplayModifier8.getInt();
			}
			GameplayModifier gameplayModifier9 = challenge.getGameplayModifier(GameplayModifierType.IntruderMinPrestige);
			if (gameplayModifier9 != null)
			{
				this.mIntruderMinPrestige = (float)gameplayModifier9.getInt() * 0.001f;
			}
		}

		// Token: 0x04000BE2 RID: 3042
		protected int mExtraIntruders;

		// Token: 0x04000BE3 RID: 3043
		protected ResourceAmounts mStartingResources = new ResourceAmounts(null);

		// Token: 0x04000BE4 RID: 3044
		protected List<SpecializationCount> mStartingSpecializations = new List<SpecializationCount>();

		// Token: 0x04000BE5 RID: 3045
		protected int mMilestonesToUnlock;

		// Token: 0x04000BE6 RID: 3046
		protected string[] mTexures;

		// Token: 0x04000BE7 RID: 3047
		protected Planet.Quantity mAtmosphereDensity;

		// Token: 0x04000BE8 RID: 3048
		protected Planet.Quantity mLightAmount;

		// Token: 0x04000BE9 RID: 3049
		protected Planet.Quantity mBlizzardRisk;

		// Token: 0x04000BEA RID: 3050
		protected Planet.Quantity mSandstormRisk;

		// Token: 0x04000BEB RID: 3051
		protected Planet.Quantity mSolarFlareRisk;

		// Token: 0x04000BEC RID: 3052
		protected Planet.Quantity mMeteorRisk;

		// Token: 0x04000BED RID: 3053
		protected Planet.Quantity mThunderstormRisk;

		// Token: 0x04000BEE RID: 3054
		protected string mCharacteristicsString;

		// Token: 0x04000BEF RID: 3055
		protected string mName;

		// Token: 0x04000BF0 RID: 3056
		protected string mDifficultyString;

		// Token: 0x04000BF1 RID: 3057
		protected string mDescription;

		// Token: 0x04000BF2 RID: 3058
		protected PlanetDefinition mDefinition;

		// Token: 0x04000BF3 RID: 3059
		protected char mLetter;

		// Token: 0x04000BF4 RID: 3060
		protected float mIntruderMinPrestige = 0.1f;

		// Token: 0x04000BF5 RID: 3061
		protected float mColonyShipRotation;

		// Token: 0x04000BF6 RID: 3062
		protected int mInitialMusicTrack = -1;

		// Token: 0x04000BF7 RID: 3063
		protected float mFlareScale = 1f;

		// Token: 0x020002C7 RID: 711
		public enum Quantity
		{
			// Token: 0x04000E72 RID: 3698
			None,
			// Token: 0x04000E73 RID: 3699
			Low,
			// Token: 0x04000E74 RID: 3700
			High,
			// Token: 0x04000E75 RID: 3701
			Variable
		}

		// Token: 0x020002C8 RID: 712
		public enum TerrainTexture
		{
			// Token: 0x04000E77 RID: 3703
			Flat1,
			// Token: 0x04000E78 RID: 3704
			Flat2,
			// Token: 0x04000E79 RID: 3705
			Slope,
			// Token: 0x04000E7A RID: 3706
			Foundations,
			// Token: 0x04000E7B RID: 3707
			Count
		}
	}
}
