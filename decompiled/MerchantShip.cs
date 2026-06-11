using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Planetbase
{
	// Token: 0x02000256 RID: 598
	public class MerchantShip : LandingShip
	{
		// Token: 0x06001149 RID: 4425 RVA: 0x0005F068 File Offset: 0x0005D268
		protected override void init(Module targetModule, LandingShip.Size size, VisitorShipType visitorShipType = VisitorShipType.Count)
		{
			base.init(targetModule, size, VisitorShipType.Count);
			this.mName = NameGenerator.getInstance().generate(Human.Gender.Male, false);
			this.mCommission = 35 + Random.Range(0, 7) * 5;
			this.mMerchantCategory = this.calculateCategory();
			this.mProducts = new ProductAmounts();
			this.addResourceProducts();
			this.addBotProducts();
			if (!Singleton<ChallengeManager>.getInstance().isGameplayModifierActive(GameplayModifierType.DisableTraderTechs))
			{
				this.addTechProducts();
			}
		}

		// Token: 0x0600114A RID: 4426 RVA: 0x0005F0D8 File Offset: 0x0005D2D8
		private MerchantCategory calculateCategory()
		{
			Challenge currentChallenge = Singleton<ChallengeManager>.getInstance().getCurrentChallenge();
			if (currentChallenge != null)
			{
				GameplayModifier gameplayModifier = currentChallenge.getGameplayModifier(GameplayModifierType.RestrictedTradingShips);
				if (gameplayModifier != null)
				{
					MerchantCategory[] merchantCategories = gameplayModifier.getMerchantCategories();
					return merchantCategories[Random.Range(0, merchantCategories.Length)];
				}
			}
			if (Singleton<Colony>.getInstance().isLowOnFood())
			{
				if (Random.Range(0, 2) != 0)
				{
					return MerchantCategory.Food;
				}
				return MerchantCategory.Count;
			}
			else if (Resource.getCountOfType(ResourceTypeList.SparesInstance) == 0)
			{
				if (Random.Range(0, 2) != 0)
				{
					return MerchantCategory.Industrial;
				}
				return MerchantCategory.Count;
			}
			else if (Resource.getCountOfType(ResourceTypeList.MedicalSuppliesInstance) == 0)
			{
				if (Random.Range(0, 2) != 0)
				{
					return MerchantCategory.Medical;
				}
				return MerchantCategory.Count;
			}
			else
			{
				if (Random.Range(0, 3) == 0)
				{
					return MerchantCategory.Count;
				}
				return (MerchantCategory)Random.Range(0, 5);
			}
		}

		// Token: 0x0600114B RID: 4427 RVA: 0x0005F16F File Offset: 0x0005D36F
		protected override void postInit()
		{
			base.postInit();
			this.mIcon = ((this.mSize == LandingShip.Size.Regular) ? ResourceUtil.loadIconColor("Ships/icon_ship_trading") : ResourceUtil.loadIconColor("Ships/icon_ship_trading_big"));
		}

		// Token: 0x0600114C RID: 4428 RVA: 0x0005F19B File Offset: 0x0005D39B
		public override void destroy()
		{
			base.destroy();
			Resource.unmarkForTrading(this.getId());
		}

		// Token: 0x0600114D RID: 4429 RVA: 0x0005F1B0 File Offset: 0x0005D3B0
		private void addResourceProducts()
		{
			List<ResourceType> list = new List<ResourceType>();
			List<float> list2 = new List<float>();
			List<ResourceType> list3 = TypeList<ResourceType, ResourceTypeList>.get();
			int num = ((this.mSize == LandingShip.Size.Regular) ? 100 : 300);
			num = num * ((this.mMerchantCategory == MerchantCategory.Count) ? 75 : 150) / 100;
			if (this.mMerchantCategory == MerchantCategory.Count)
			{
				for (int i = 0; i < list3.Count; i++)
				{
					ResourceType resourceType = list3[i];
					if (Random.Range(0, 2) == 0 || resourceType.hasFlag(2))
					{
						list.Add(resourceType);
						list2.Add(Random.Range(0.8f, 1.2f) / (float)resourceType.getValue());
					}
				}
			}
			else
			{
				for (int j = 0; j < list3.Count; j++)
				{
					ResourceType resourceType2 = list3[j];
					if (resourceType2.getMerchantCategory() == this.mMerchantCategory || resourceType2.hasFlag(2))
					{
						list.Add(resourceType2);
						list2.Add(Random.Range(0.5f, 1.5f) / (float)resourceType2.getValue());
					}
				}
			}
			float num2 = 0f;
			for (int k = 0; k < list2.Count; k++)
			{
				if (!list[k].hasFlag(2))
				{
					num2 += list2[k];
				}
			}
			for (int l = 0; l < list2.Count; l++)
			{
				if (!list[l].hasFlag(2))
				{
					list2[l] /= num2;
				}
			}
			int num3 = this.getMaxCargoSpace() / 2;
			int num4 = num3 * 15 / 100;
			num3 += Random.Range(-num4, num4);
			for (int m = 0; m < list2.Count; m++)
			{
				ResourceType resourceType3 = list[m];
				if (resourceType3.hasFlag(2))
				{
					this.mProducts.add(new ProductResource(resourceType3), Random.Range(num / 2, num + 1));
				}
				else
				{
					int num5 = Mathf.Max(1, num / resourceType3.getValue());
					this.mProducts.add(new ProductResource(resourceType3), Mathf.Clamp(Mathf.RoundToInt((float)num3 * list2[m]), 1, num5));
				}
			}
		}

		// Token: 0x0600114E RID: 4430 RVA: 0x0005F3D4 File Offset: 0x0005D5D4
		private void addBotProducts()
		{
			int countOfType = Character.getCountOfType<Colonist>();
			int countOfType2 = Character.getCountOfType<Bot>();
			bool flag = Singleton<ChallengeManager>.getInstance().isGameplayModifierActive(GameplayModifierType.InfiniteTraderBots);
			if (countOfType >= countOfType2 || flag)
			{
				int num = 0;
				if (this.mMerchantCategory == MerchantCategory.Count)
				{
					num = ((this.mSize == LandingShip.Size.Large) ? 2 : 1);
				}
				if (this.mMerchantCategory == MerchantCategory.Electronics)
				{
					num = ((this.mSize == LandingShip.Size.Large) ? 3 : 2);
				}
				if (num > 0)
				{
					foreach (Specialization specialization in SpecializationList.getBotSpecializations())
					{
						int num2 = Random.Range(0, num + 1);
						if (num2 > 0)
						{
							this.mProducts.add(new ProductBot(specialization), num2);
						}
					}
				}
			}
		}

		// Token: 0x0600114F RID: 4431 RVA: 0x0005F4A0 File Offset: 0x0005D6A0
		private void addTechProducts()
		{
			foreach (Tech tech in TypeList<Tech, TechList>.get())
			{
				if (!Singleton<TechManager>.getInstance().isAcquired(tech))
				{
					int num = int.MaxValue;
					if (this.mMerchantCategory == tech.getMerchantCategory())
					{
						num = ((this.mSize == LandingShip.Size.Large) ? 2 : 4);
					}
					else if (this.mMerchantCategory == MerchantCategory.Count)
					{
						num = ((this.mSize == LandingShip.Size.Large) ? 4 : 8);
					}
					if (num != 2147483647 && Random.Range(0, num) == 0)
					{
						this.mProducts.add(new ProductTech(tech), 1);
						if (this.mSize == LandingShip.Size.Regular)
						{
							break;
						}
						if (this.mMerchantCategory == MerchantCategory.Count)
						{
							break;
						}
					}
				}
			}
		}

		// Token: 0x06001150 RID: 4432 RVA: 0x0005F574 File Offset: 0x0005D774
		public override void update(float timeStep)
		{
			base.update(timeStep);
			if (this.mNeededProducts != null && this.mNeededProducts.getCount() == 0 && this.mTradeState == MerchantShip.TradeState.Trading)
			{
				this.deliverProducts();
				this.mNeededProducts = null;
			}
		}

		// Token: 0x06001151 RID: 4433 RVA: 0x0005F5A8 File Offset: 0x0005D7A8
		protected override GameObject getPrefab()
		{
			GameObject[] array;
			if (this.mSize == LandingShip.Size.Large)
			{
				array = ResourceList.getInstance().Ships.MerchantLarge;
			}
			else
			{
				array = ResourceList.getInstance().Ships.MerchantSmall;
			}
			string text = ((this.mMerchantCategory == MerchantCategory.Count) ? "General" : this.mMerchantCategory.ToString());
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].name.Contains(text))
				{
					return array[i];
				}
			}
			return array[this.mId % array.Length];
		}

		// Token: 0x06001152 RID: 4434 RVA: 0x0005F632 File Offset: 0x0005D832
		public override void onLanded()
		{
			base.onLanded();
			Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_ship_landed", this.getName()), ResourceList.StaticIcons.Trade, this, 4));
		}

		// Token: 0x06001153 RID: 4435 RVA: 0x0005F665 File Offset: 0x0005D865
		public override void onTakeOff()
		{
			base.onTakeOff();
			if (this.mNeededProducts != null && this.mNeededProducts.getTotalAmount() > 0)
			{
				this.createProducts(this.mContainedProducts);
			}
			Resource.unmarkForTrading(this.getId());
			Singleton<MessageLog>.getInstance().dismissMessages(this);
		}

		// Token: 0x06001154 RID: 4436 RVA: 0x0005F6A8 File Offset: 0x0005D8A8
		private void createProducts(ProductAmounts amounts)
		{
			foreach (ProductAmount productAmount in amounts)
			{
				productAmount.getProduct().instantiate(this.getPosition(), productAmount.getAmount());
			}
		}

		// Token: 0x06001155 RID: 4437 RVA: 0x0005F700 File Offset: 0x0005D900
		public override string getName()
		{
			return StringList.get("merchant_ship");
		}

		// Token: 0x06001156 RID: 4438 RVA: 0x0005F70C File Offset: 0x0005D90C
		public ProductAmounts getProductAmounts()
		{
			return this.mProducts;
		}

		// Token: 0x06001157 RID: 4439 RVA: 0x0005F714 File Offset: 0x0005D914
		public void startTrading()
		{
			this.mTradeState = MerchantShip.TradeState.Trading;
		}

		// Token: 0x06001158 RID: 4440 RVA: 0x0005F71D File Offset: 0x0005D91D
		public void cancelTrading()
		{
			this.mTradeState = MerchantShip.TradeState.None;
		}

		// Token: 0x06001159 RID: 4441 RVA: 0x0005F726 File Offset: 0x0005D926
		public int getCommission()
		{
			return this.mCommission;
		}

		// Token: 0x0600115A RID: 4442 RVA: 0x0005F72E File Offset: 0x0005D92E
		public bool canTrade()
		{
			return this.mTradeState == MerchantShip.TradeState.None && this.mState == LandingShip.State.Landed;
		}

		// Token: 0x0600115B RID: 4443 RVA: 0x0005F744 File Offset: 0x0005D944
		public void onTradeAgreed(ProductAmounts neededProducts, ProductAmounts providedProducts)
		{
			this.mContainedProducts = new ProductAmounts();
			this.mNeededProducts = neededProducts;
			this.mNeededProducts.setName(StringList.get("pending_resources"));
			this.mProvidedProducts = providedProducts;
			foreach (ProductAmount productAmount in neededProducts.clone())
			{
				ProductResource productResource = productAmount.getProduct() as ProductResource;
				if (productResource != null && productResource.getResourceType().isInmaterial())
				{
					this.mNeededProducts.remove(productAmount.getProduct(), productAmount.getAmount());
					this.mContainedProducts.add(productAmount.getProduct(), productAmount.getAmount());
				}
			}
			if (this.mNeededProducts.getCount() == 0)
			{
				this.deliverProducts();
			}
		}

		// Token: 0x0600115C RID: 4444 RVA: 0x0005F818 File Offset: 0x0005DA18
		public void dismiss()
		{
			this.mTradeState = MerchantShip.TradeState.Done;
		}

		// Token: 0x0600115D RID: 4445 RVA: 0x0005F821 File Offset: 0x0005DA21
		public ProductAmounts getNeededProducts()
		{
			return this.mNeededProducts;
		}

		// Token: 0x0600115E RID: 4446 RVA: 0x0005F82C File Offset: 0x0005DA2C
		protected override bool canTakeOff()
		{
			return !Singleton<DisasterManager>.getInstance().anyInProgress() && ((this.mStateTime > 180f && this.mTradeState == MerchantShip.TradeState.None) || (this.mStateTime > 1200f && this.mTradeState == MerchantShip.TradeState.Trading) || this.mTradeState == MerchantShip.TradeState.Done);
		}

		// Token: 0x0600115F RID: 4447 RVA: 0x0005F87D File Offset: 0x0005DA7D
		public bool isTrading()
		{
			return this.mTradeState == MerchantShip.TradeState.Trading;
		}

		// Token: 0x06001160 RID: 4448 RVA: 0x0005F888 File Offset: 0x0005DA88
		public void tradeResource(Resource resource)
		{
			this.mNeededProducts.remove(new ProductResource(resource.getResourceType()), 1);
			this.mContainedProducts.add(new ProductResource(resource.getResourceType()), 1);
			resource.destroy();
		}

		// Token: 0x06001161 RID: 4449 RVA: 0x0005F8BE File Offset: 0x0005DABE
		private void deliverProducts()
		{
			this.createProducts(this.mProvidedProducts);
			this.mTradeState = MerchantShip.TradeState.Done;
		}

		// Token: 0x06001162 RID: 4450 RVA: 0x0005F8D3 File Offset: 0x0005DAD3
		public string getMerchantName()
		{
			return this.mName;
		}

		// Token: 0x06001163 RID: 4451 RVA: 0x0005F8DB File Offset: 0x0005DADB
		public Texture2D getMerchantIcon()
		{
			return ResourceList.StaticIcons.Male;
		}

		// Token: 0x06001164 RID: 4452 RVA: 0x0005F8E8 File Offset: 0x0005DAE8
		public string getMerchantDescription()
		{
			if (this.mSize == LandingShip.Size.Regular)
			{
				return StringList.get("merchant_freelance_description", StringList.get("resource_category_" + this.mMerchantCategory.ToString().ToLower()));
			}
			return StringList.get("merchant_corporate_description", StringList.get("corporation_" + this.mMerchantCategory.ToString().ToLower()));
		}

		// Token: 0x06001165 RID: 4453 RVA: 0x0005F95C File Offset: 0x0005DB5C
		public MerchantCategory getCategory()
		{
			return this.mMerchantCategory;
		}

		// Token: 0x06001166 RID: 4454 RVA: 0x0005F964 File Offset: 0x0005DB64
		protected override void serialize(XmlNode parent, string name)
		{
			base.serialize(parent, name);
			XmlNode lastChild = parent.LastChild;
			Serialization.serializeInt(lastChild, "trade-state", (int)this.mTradeState);
			Serialization.serializeString(lastChild, "name", this.mName);
			Serialization.serializeInt(lastChild, "merchant-category", (int)this.mMerchantCategory);
			Serialization.serializeInt(lastChild, "commission", this.mCommission);
			if (this.mProducts != null)
			{
				this.mProducts.serialize(lastChild, "products");
			}
			if (this.mNeededProducts != null)
			{
				this.mNeededProducts.serialize(lastChild, "needed-products");
			}
			if (this.mContainedProducts != null)
			{
				this.mContainedProducts.serialize(lastChild, "contained-products");
			}
			if (this.mProvidedProducts != null)
			{
				this.mProvidedProducts.serialize(lastChild, "provided-products");
			}
		}

		// Token: 0x06001167 RID: 4455 RVA: 0x0005FA28 File Offset: 0x0005DC28
		protected override void deserialize(XmlNode node)
		{
			base.deserialize(node);
			this.mTradeState = (MerchantShip.TradeState)Serialization.deserializeInt(node["trade-state"]);
			this.mName = Serialization.deserializeString(node["name"]);
			this.mMerchantCategory = (MerchantCategory)Serialization.deserializeInt(node["merchant-category"]);
			this.mCommission = Serialization.deserializeInt(node["commission"]);
			XmlNode xmlNode = node["products"];
			if (xmlNode != null)
			{
				this.mProducts = new ProductAmounts();
				this.mProducts.deserialize(xmlNode);
			}
			XmlNode xmlNode2 = node["needed-products"];
			if (xmlNode2 != null)
			{
				this.mNeededProducts = new ProductAmounts();
				this.mNeededProducts.deserialize(xmlNode2);
			}
			XmlNode xmlNode3 = node["contained-products"];
			if (xmlNode3 != null)
			{
				this.mContainedProducts = new ProductAmounts();
				this.mContainedProducts.deserialize(xmlNode3);
			}
			XmlNode xmlNode4 = node["provided-products"];
			if (xmlNode4 != null)
			{
				this.mProvidedProducts = new ProductAmounts();
				this.mProvidedProducts.deserialize(xmlNode4);
			}
		}

		// Token: 0x06001168 RID: 4456 RVA: 0x0005FB2C File Offset: 0x0005DD2C
		public int getMaxCargoSpace()
		{
			if (this.mSize != LandingShip.Size.Regular)
			{
				return 120;
			}
			return 60;
		}

		// Token: 0x06001169 RID: 4457 RVA: 0x0005FB3C File Offset: 0x0005DD3C
		public static MerchantShip findInNeedOfResources()
		{
			foreach (Ship ship in Ship.mShipDictionary.Values)
			{
				MerchantShip merchantShip = ship as MerchantShip;
				if (merchantShip != null && merchantShip.getNeededProducts() != null && merchantShip.getNeededProducts().getCount() > 0)
				{
					return merchantShip;
				}
			}
			return null;
		}

		// Token: 0x04000CD7 RID: 3287
		private MerchantShip.TradeState mTradeState;

		// Token: 0x04000CD8 RID: 3288
		private string mName;

		// Token: 0x04000CD9 RID: 3289
		private MerchantCategory mMerchantCategory = MerchantCategory.Count;

		// Token: 0x04000CDA RID: 3290
		private ProductAmounts mProducts;

		// Token: 0x04000CDB RID: 3291
		private ProductAmounts mNeededProducts;

		// Token: 0x04000CDC RID: 3292
		private ProductAmounts mContainedProducts;

		// Token: 0x04000CDD RID: 3293
		private ProductAmounts mProvidedProducts;

		// Token: 0x04000CDE RID: 3294
		private int mCommission;

		// Token: 0x04000CDF RID: 3295
		private const float StayTime = 180f;

		// Token: 0x04000CE0 RID: 3296
		private const float TradeTime = 1200f;

		// Token: 0x04000CE1 RID: 3297
		private const int FreelanceValue = 100;

		// Token: 0x04000CE2 RID: 3298
		private const int CorporateValue = 300;

		// Token: 0x04000CE3 RID: 3299
		private const int CargoMargin = 20;

		// Token: 0x04000CE4 RID: 3300
		private const int FreelanceCargoCapacity = 60;

		// Token: 0x04000CE5 RID: 3301
		private const int CorporateCargoCapacity = 120;

		// Token: 0x020002CF RID: 719
		private enum TradeState
		{
			// Token: 0x04000E95 RID: 3733
			None,
			// Token: 0x04000E96 RID: 3734
			Trading,
			// Token: 0x04000E97 RID: 3735
			Done
		}
	}
}
