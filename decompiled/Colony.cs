using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Planetbase
{
	// Token: 0x020000B9 RID: 185
	public class Colony : Singleton<Colony>
	{
		// Token: 0x06000474 RID: 1140 RVA: 0x0001B790 File Offset: 0x00019990
		public Colony()
		{
			this.mWelfareIndicator = new Indicator(StringList.get("welfare"), ResourceList.StaticIcons.Welfare, IndicatorType.Condition, 0f, 1f, SignType.Unknown);
			this.mWelfareIndicator.setLevels(0.3f, 0.5f, 0.7f, 0.9f);
			this.mPrestigeIndicator = new Indicator(StringList.get("prestige"), ResourceList.StaticIcons.Prestige, IndicatorType.Normal, 0f, 1000f, SignType.Unknown);
			this.mPrestigeIndicator.setLevels(0.3f, 0.5f, 0.7f, 0.9f);
			this.mColonistCount = int.MaxValue;
			this.mGameTime = 0.0;
			this.mRealGameTime = 0.0;
			this.mName = "Unnamed";
		}

		// Token: 0x06000475 RID: 1141 RVA: 0x0001B879 File Offset: 0x00019A79
		public Indicator getPrestigeIndicator()
		{
			return this.mPrestigeIndicator;
		}

		// Token: 0x06000476 RID: 1142 RVA: 0x0001B881 File Offset: 0x00019A81
		public Indicator getWelfareIndicator()
		{
			return this.mWelfareIndicator;
		}

		// Token: 0x06000477 RID: 1143 RVA: 0x0001B88C File Offset: 0x00019A8C
		public void update(float timeStep, float unscaledTimeStep)
		{
			this.mTime += timeStep;
			this.mGameTime += (double)timeStep;
			this.mRealGameTime += (double)unscaledTimeStep;
			if (this.mTime >= 2.6f)
			{
				this.mTime = 0f;
				this.tick(2.6f);
			}
		}

		// Token: 0x06000478 RID: 1144 RVA: 0x0001B8E8 File Offset: 0x00019AE8
		public float getDisasterInterceptionChance()
		{
			return this.mDisasterInterceptionChance;
		}

		// Token: 0x06000479 RID: 1145 RVA: 0x0001B8F0 File Offset: 0x00019AF0
		public int getColonistCount()
		{
			return this.mColonistCount;
		}

		// Token: 0x0600047A RID: 1146 RVA: 0x0001B8F8 File Offset: 0x00019AF8
		public double getGameTime()
		{
			return this.mGameTime;
		}

		// Token: 0x0600047B RID: 1147 RVA: 0x0001B900 File Offset: 0x00019B00
		private void tick(float timeStep)
		{
			this.mColonistCount = Character.getCountOfType<Colonist>();
			this.calculateWelfare();
			this.calculatePrestige();
			this.mDisasterInterceptionChance = Module.calculateDisasterInterceptionChance();
			this.mTimeSinceWarningCheck += timeStep;
			if (this.mTimeSinceWarningCheck > 120f)
			{
				this.checkWarnings();
			}
		}

		// Token: 0x0600047C RID: 1148 RVA: 0x0001B950 File Offset: 0x00019B50
		private void checkWarnings()
		{
			int humanCount = Character.getHumanCount();
			int overallOxygenGeneration = Module.getOverallOxygenGeneration();
			if (overallOxygenGeneration < humanCount && overallOxygenGeneration > 0)
			{
				Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_oxygen_balance"), ResourceList.StaticIcons.OxygenGeneration, 1));
			}
			this.mTimeSinceWarningCheck = 0f;
			int count = TypeList<ResourceType, ResourceTypeList>.getCount();
			for (int i = 0; i < count; i++)
			{
				ResourceType resourceType = TypeList<ResourceType, ResourceTypeList>.get()[i];
				if (resourceType.hasFlag(32) && (resourceType != ResourceTypeList.MedicalSuppliesInstance || !Singleton<ChallengeManager>.getInstance().isGameplayModifierActive(GameplayModifierType.DisableNoMedicalSuppliesWarning)) && Resource.getCountOfType(resourceType) == 0)
				{
					Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_out_of_resource", resourceType.getName()), resourceType.getIcon(), 1));
				}
			}
			if (!Singleton<ChallengeManager>.getInstance().isGameplayModifierActive(GameplayModifierType.DisableColonyShipRecycling) && Ship.getFirstOfType<ColonyShip>() != null && Resource.getCountOfType(ResourceTypeList.BioplasticInstance) > 5)
			{
				Resource.getCountOfType(ResourceTypeList.MetalInstance);
			}
			if (this.mColonistCount >= 50)
			{
				List<Module> categoryModules = Module.getCategoryModules(Module.Category.Storage);
				bool flag = false;
				if (categoryModules != null)
				{
					int count2 = categoryModules.Count;
					for (int j = 0; j < count2; j++)
					{
						if (categoryModules[j].isBuilt() && categoryModules[j].getResourceSpaceRatio() < 0.9f)
						{
							flag = true;
						}
					}
				}
				if (!flag)
				{
					Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_not_enough_storage_space"), TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeStorage>().getIcon(), 1));
				}
			}
			int totalFood = this.getTotalFood();
			if (totalFood == 0)
			{
				Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_no_food"), ResourceTypeList.MealInstance.getIcon(), 1));
				return;
			}
			if (totalFood < this.getLowFoodThreshold())
			{
				Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_low_food"), ResourceTypeList.MealInstance.getIcon(), 0));
			}
		}

		// Token: 0x0600047D RID: 1149 RVA: 0x0001BB24 File Offset: 0x00019D24
		private int getLowFoodThreshold()
		{
			return 5 + Character.getCountOfType<Colonist>() / 10;
		}

		// Token: 0x0600047E RID: 1150 RVA: 0x0001BB30 File Offset: 0x00019D30
		private int getTotalFood()
		{
			return Resource.getCountOfType(ResourceTypeList.MealInstance) + Resource.getCountOfType(ResourceTypeList.VegetablesInstance) + Resource.getCountOfType(ResourceTypeList.VitromeatInstance);
		}

		// Token: 0x0600047F RID: 1151 RVA: 0x0001BB52 File Offset: 0x00019D52
		public bool isLowOnFood()
		{
			return this.getTotalFood() < this.getLowFoodThreshold();
		}

		// Token: 0x06000480 RID: 1152 RVA: 0x0001BB62 File Offset: 0x00019D62
		private void calculateWelfare()
		{
			this.mWelfareIndicator.setValue(Character.calculateOverallStatus());
		}

		// Token: 0x06000481 RID: 1153 RVA: 0x0001BB74 File Offset: 0x00019D74
		private void calculatePrestige()
		{
			int num = Mathf.Min(Module.calculateTotalPrestige(), 300);
			int num2 = Mathf.Min(Character.getCount(), 300);
			int num3 = Mathf.Min(Resource.getCount() / 3, 200);
			int num4 = Mathf.Min(this.mExtraPrestige, 200);
			this.mPrestigeIndicator.setValue((float)(num2 + num3 + num + num4));
		}

		// Token: 0x06000482 RID: 1154 RVA: 0x0001BBD8 File Offset: 0x00019DD8
		public void setName(string name)
		{
			for (int i = 0; i < name.Length; i++)
			{
				if (!this.isValidNameCharacter(name[i]))
				{
					name = name.Remove(i, 1);
					i--;
				}
			}
			if (name.Length > 16)
			{
				name = name.Substring(0, 16);
			}
			this.mName = name;
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x0001BC2F File Offset: 0x00019E2F
		public void setLatitude(int latitude)
		{
			this.mLatitude = latitude;
		}

		// Token: 0x06000484 RID: 1156 RVA: 0x0001BC38 File Offset: 0x00019E38
		public void setLongitude(int longitude)
		{
			this.mLongitude = longitude;
		}

		// Token: 0x06000485 RID: 1157 RVA: 0x0001BC41 File Offset: 0x00019E41
		private bool isValidNameCharacter(char c)
		{
			return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == ' ';
		}

		// Token: 0x06000486 RID: 1158 RVA: 0x0001BC6F File Offset: 0x00019E6F
		public bool isValidName()
		{
			return this.mName.Length >= 4 && !this.mName.StartsWith(" ") && !this.mName.Contains("  ");
		}

		// Token: 0x06000487 RID: 1159 RVA: 0x0001BCA6 File Offset: 0x00019EA6
		public string getName()
		{
			return this.mName;
		}

		// Token: 0x06000488 RID: 1160 RVA: 0x0001BCAE File Offset: 0x00019EAE
		public int getLatitude()
		{
			return this.mLatitude;
		}

		// Token: 0x06000489 RID: 1161 RVA: 0x0001BCB6 File Offset: 0x00019EB6
		public int getLongitude()
		{
			return this.mLongitude;
		}

		// Token: 0x0600048A RID: 1162 RVA: 0x0001BCBE File Offset: 0x00019EBE
		public void addExtraPrestige(int offset)
		{
			this.mExtraPrestige += offset;
		}

		// Token: 0x0600048B RID: 1163 RVA: 0x0001BCD0 File Offset: 0x00019ED0
		public void serialize(XmlNode rootNode, string name)
		{
			XmlNode xmlNode = Serialization.createNode(rootNode, name, null);
			Serialization.serializeInt(xmlNode, "extra-prestige", this.mExtraPrestige);
			Serialization.serializeDouble(xmlNode, "game-time", this.mGameTime);
			Serialization.serializeDouble(xmlNode, "real-game-time", this.mRealGameTime);
			Serialization.serializeString(xmlNode, "name", this.mName);
			Serialization.serializeInt(xmlNode, "latitude", this.mLatitude);
			Serialization.serializeInt(xmlNode, "longitude", this.mLongitude);
		}

		// Token: 0x0600048C RID: 1164 RVA: 0x0001BD4C File Offset: 0x00019F4C
		public void deserialize(XmlNode node)
		{
			if (node != null)
			{
				this.mExtraPrestige = Serialization.deserializeInt(node["extra-prestige"]);
				this.mGameTime = Serialization.deserializeDouble(node["game-time"]);
				this.mRealGameTime = Serialization.deserializeDouble(node["real-game-time"]);
				this.mLatitude = Serialization.deserializeInt(node["latitude"]);
				this.mLongitude = Serialization.deserializeInt(node["longitude"]);
				if (node["name"] != null)
				{
					this.mName = Serialization.deserializeString(node["name"]);
				}
			}
		}

		// Token: 0x0400048B RID: 1163
		private const float TickPeriod = 2.6f;

		// Token: 0x0400048C RID: 1164
		private const float WarningCheckPeriod = 120f;

		// Token: 0x0400048D RID: 1165
		private string mName;

		// Token: 0x0400048E RID: 1166
		private int mLatitude;

		// Token: 0x0400048F RID: 1167
		private int mLongitude;

		// Token: 0x04000490 RID: 1168
		private int mExtraPrestige;

		// Token: 0x04000491 RID: 1169
		private int mColonistCount;

		// Token: 0x04000492 RID: 1170
		private double mGameTime;

		// Token: 0x04000493 RID: 1171
		private double mRealGameTime;

		// Token: 0x04000494 RID: 1172
		private float mTime = 2.6f;

		// Token: 0x04000495 RID: 1173
		private float mTimeSinceWarningCheck;

		// Token: 0x04000496 RID: 1174
		private float mDisasterInterceptionChance;

		// Token: 0x04000497 RID: 1175
		private Indicator mWelfareIndicator;

		// Token: 0x04000498 RID: 1176
		private Indicator mPrestigeIndicator;
	}
}
