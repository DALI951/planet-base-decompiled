using System;
using UnityEngine;

namespace Planetbase
{
	// Token: 0x02000213 RID: 531
	public abstract class ResourceType : Localizable
	{
		// Token: 0x06001010 RID: 4112 RVA: 0x0005A3F2 File Offset: 0x000585F2
		public ResourceType()
		{
			this.initStrings();
		}

		// Token: 0x06001011 RID: 4113 RVA: 0x0005A42C File Offset: 0x0005862C
		public void initStrings()
		{
			this.mName = StringList.get(Util.camelCaseToLowercase(base.GetType().Name));
			string text = Util.camelCaseToLowercase(base.GetType().Name) + "_plural";
			if (StringList.exists(text))
			{
				this.mNamePlural = StringList.get(text);
			}
			else
			{
				this.mNamePlural = this.mName;
			}
			this.mSubtitle = StringList.get("resource");
		}

		// Token: 0x06001012 RID: 4114 RVA: 0x0005A4A1 File Offset: 0x000586A1
		public string getName()
		{
			return this.mName;
		}

		// Token: 0x06001013 RID: 4115 RVA: 0x0005A4A9 File Offset: 0x000586A9
		public string getNamePlural()
		{
			return this.mNamePlural;
		}

		// Token: 0x06001014 RID: 4116 RVA: 0x0005A4B1 File Offset: 0x000586B1
		public string getSubtitle()
		{
			return this.mSubtitle;
		}

		// Token: 0x06001015 RID: 4117 RVA: 0x0005A4B9 File Offset: 0x000586B9
		public GameObject createModel()
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.mModel);
			gameObject.name = base.GetType().Name + " Model";
			gameObject.GetComponent<Rigidbody>();
			gameObject.disablePhysics();
			return gameObject;
		}

		// Token: 0x06001016 RID: 4118 RVA: 0x0005A4EE File Offset: 0x000586EE
		public bool hasUnpackedModel()
		{
			return this.mUnpackedModels != null;
		}

		// Token: 0x06001017 RID: 4119 RVA: 0x0005A4FC File Offset: 0x000586FC
		public GameObject createModelUnpacked(ResourceSubtype mealSubtype = ResourceSubtype.None)
		{
			GameObject gameObject;
			if (mealSubtype == ResourceSubtype.None)
			{
				gameObject = Object.Instantiate<GameObject>(this.mUnpackedModels[0]);
			}
			else
			{
				gameObject = Object.Instantiate<GameObject>(this.mUnpackedModels[mealSubtype - ResourceSubtype.Basic]);
			}
			gameObject.name = base.GetType().Name + " Model";
			gameObject.GetComponent<Rigidbody>();
			gameObject.disablePhysics();
			return gameObject;
		}

		// Token: 0x06001018 RID: 4120 RVA: 0x0005A558 File Offset: 0x00058758
		public Texture2D getIcon()
		{
			return this.mIcon;
		}

		// Token: 0x06001019 RID: 4121 RVA: 0x0005A560 File Offset: 0x00058760
		public Vector3 getSize()
		{
			return this.mSize;
		}

		// Token: 0x0600101A RID: 4122 RVA: 0x0005A568 File Offset: 0x00058768
		public float getRadius()
		{
			return this.mSize.x * 0.7f;
		}

		// Token: 0x0600101B RID: 4123 RVA: 0x0005A57B File Offset: 0x0005877B
		public int getValue()
		{
			return this.mValue;
		}

		// Token: 0x0600101C RID: 4124 RVA: 0x0005A583 File Offset: 0x00058783
		public bool isInmaterial()
		{
			return this.mModel == null;
		}

		// Token: 0x0600101D RID: 4125 RVA: 0x0005A591 File Offset: 0x00058791
		public CharacterIndicator getStatusRecovery()
		{
			return this.mStatusRecovery;
		}

		// Token: 0x0600101E RID: 4126 RVA: 0x0005A599 File Offset: 0x00058799
		public float getStatusRecoveryAmount()
		{
			return this.mStatusRecoveryAmount;
		}

		// Token: 0x0600101F RID: 4127 RVA: 0x0005A5A1 File Offset: 0x000587A1
		public float getConsumptionTime()
		{
			return this.mConsumptionTime;
		}

		// Token: 0x06001020 RID: 4128 RVA: 0x0005A5A9 File Offset: 0x000587A9
		public MerchantCategory getMerchantCategory()
		{
			return this.mMerchantCategory;
		}

		// Token: 0x06001021 RID: 4129 RVA: 0x0005A5B1 File Offset: 0x000587B1
		public Color getStatsColor()
		{
			return this.mStatsColor;
		}

		// Token: 0x06001022 RID: 4130 RVA: 0x0005A5B9 File Offset: 0x000587B9
		public bool hasFlag(int flag)
		{
			return (this.mFlags & flag) != 0;
		}

		// Token: 0x06001023 RID: 4131 RVA: 0x0005A5C8 File Offset: 0x000587C8
		protected GameObject loadModel()
		{
			string text = "Prefabs/Resources/Prefab" + base.GetType().Name;
			GameObject gameObject = ResourceUtil.loadPrefab(text);
			if (gameObject == null)
			{
				Debug.LogWarning("Could not load model: " + text);
			}
			return gameObject;
		}

		// Token: 0x06001024 RID: 4132 RVA: 0x0005A60C File Offset: 0x0005880C
		protected GameObject loadModelUnpacked(string suffix = "")
		{
			string text = "Prefabs/Resources/PrefabUnpacked" + base.GetType().Name + suffix;
			GameObject gameObject = ResourceUtil.loadPrefab(text);
			if (gameObject == null)
			{
				Debug.LogWarning("Could not load model: " + text);
			}
			return gameObject;
		}

		// Token: 0x06001025 RID: 4133 RVA: 0x0005A64F File Offset: 0x0005884F
		protected Texture2D loadIcon()
		{
			return Util.applyColor(ResourceUtil.loadIcon("Resources/icon_" + Util.camelCaseToLowercase(base.GetType().Name)), this.mStatsColor);
		}

		// Token: 0x06001026 RID: 4134 RVA: 0x0005A680 File Offset: 0x00058880
		public void setTypeIndex(int typeIndex)
		{
			this.mTypeIndex = typeIndex;
		}

		// Token: 0x06001027 RID: 4135 RVA: 0x0005A689 File Offset: 0x00058889
		public int getTypeIndex()
		{
			return this.mTypeIndex;
		}

		// Token: 0x06001028 RID: 4136 RVA: 0x0005A691 File Offset: 0x00058891
		public static bool isMealSubtype(ResourceSubtype subtype)
		{
			return subtype == ResourceSubtype.Basic || subtype == ResourceSubtype.Salad || subtype == ResourceSubtype.Pasta || subtype == ResourceSubtype.Burger;
		}

		// Token: 0x04000C54 RID: 3156
		public const int FlagWeapon = 1;

		// Token: 0x04000C55 RID: 3157
		public const int FlagCurrency = 2;

		// Token: 0x04000C56 RID: 3158
		public const int FlagLight = 4;

		// Token: 0x04000C57 RID: 3159
		public const int FlagAttachToConsume = 8;

		// Token: 0x04000C58 RID: 3160
		public const int FlagCanConsumeOnFloor = 16;

		// Token: 0x04000C59 RID: 3161
		public const int FlagWarningWhenNone = 32;

		// Token: 0x04000C5A RID: 3162
		public const int FlagManufactured = 64;

		// Token: 0x04000C5B RID: 3163
		public const int FlagCanBeBusy = 128;

		// Token: 0x04000C5C RID: 3164
		protected GameObject mModel;

		// Token: 0x04000C5D RID: 3165
		protected GameObject[] mUnpackedModels;

		// Token: 0x04000C5E RID: 3166
		protected Texture2D mIcon;

		// Token: 0x04000C5F RID: 3167
		protected string mName;

		// Token: 0x04000C60 RID: 3168
		protected string mNamePlural;

		// Token: 0x04000C61 RID: 3169
		protected string mSubtitle;

		// Token: 0x04000C62 RID: 3170
		protected Material mMaterial;

		// Token: 0x04000C63 RID: 3171
		protected Vector3 mSize = Vector3.one;

		// Token: 0x04000C64 RID: 3172
		protected int mValue = 10;

		// Token: 0x04000C65 RID: 3173
		protected int mFlags;

		// Token: 0x04000C66 RID: 3174
		protected CharacterIndicator mStatusRecovery = CharacterIndicator.Count;

		// Token: 0x04000C67 RID: 3175
		protected float mStatusRecoveryAmount;

		// Token: 0x04000C68 RID: 3176
		protected float mConsumptionTime;

		// Token: 0x04000C69 RID: 3177
		protected MerchantCategory mMerchantCategory = MerchantCategory.Count;

		// Token: 0x04000C6A RID: 3178
		protected Color mStatsColor = Color.red;

		// Token: 0x04000C6B RID: 3179
		protected int mTypeIndex;

		// Token: 0x04000C6C RID: 3180
		protected static readonly Vector3 LargeResourceSize = new Vector3(0.9f, 0.9f, 0.9f);

		// Token: 0x04000C6D RID: 3181
		protected static readonly Vector3 SmallResourceSize = new Vector3(0.9f, 0.45f, 0.9f);

		// Token: 0x04000C6E RID: 3182
		protected static readonly Vector3 DrinkResourceSize = new Vector3(0.9f, 0.45f, 0.45f);
	}
}
