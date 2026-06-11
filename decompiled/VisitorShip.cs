using System;
using System.Xml;
using UnityEngine;

namespace Planetbase
{
	// Token: 0x02000259 RID: 601
	public class VisitorShip : LandingShip
	{
		// Token: 0x06001186 RID: 4486 RVA: 0x000600D4 File Offset: 0x0005E2D4
		protected override void init(Module targetModule, LandingShip.Size size, VisitorShipType visitorShipType = VisitorShipType.Count)
		{
			base.init(targetModule, size, VisitorShipType.Count);
			this.mShipType = visitorShipType;
			if (this.mShipType == VisitorShipType.Count)
			{
				this.mShipType = ((Random.Range(0, 3) == 0) ? VisitorShipType.Military : VisitorShipType.Civillian);
			}
		}

		// Token: 0x06001187 RID: 4487 RVA: 0x00060104 File Offset: 0x0005E304
		protected override GameObject getPrefab()
		{
			ResourceList instance = ResourceList.getInstance();
			GameObject[] array;
			if (this.mShipType == VisitorShipType.Civillian)
			{
				array = ((this.mSize == LandingShip.Size.Regular) ? instance.Ships.VisitorSmallCivillian : instance.Ships.VisitorLargeCivillian);
			}
			else
			{
				array = ((this.mSize == LandingShip.Size.Regular) ? instance.Ships.VisitorSmallMilitary : instance.Ships.VisitorLargeMilitary);
			}
			return array[this.mId % array.Length];
		}

		// Token: 0x06001188 RID: 4488 RVA: 0x00060171 File Offset: 0x0005E371
		protected override void postInit()
		{
			base.postInit();
			this.mIcon = ((this.mSize == LandingShip.Size.Regular) ? ResourceUtil.loadIconColor("Ships/icon_ship_personnel") : ResourceUtil.loadIconColor("Ships/icon_ship_personnel_big"));
		}

		// Token: 0x06001189 RID: 4489 RVA: 0x0006019D File Offset: 0x0005E39D
		public override void onLanded()
		{
			base.onLanded();
			if (this.mVisitorEvent != null)
			{
				this.onLandedEvent(this.mVisitorEvent);
			}
			else
			{
				this.onLandedGeneric();
			}
			this.addDescriptionItem();
		}

		// Token: 0x0600118A RID: 4490 RVA: 0x000601C8 File Offset: 0x0005E3C8
		private void addDescriptionItem()
		{
			if (this.mPendingVisitorsDescriptionItem == null && this.mPendingVisitors > 0)
			{
				this.mPendingVisitorsDescriptionItem = new DescriptionItem(this.mPendingVisitors.ToString(), ResourceList.StaticIcons.Male, StringList.get("tooltip_visitors_on_base", (float)this.mPendingVisitors), null);
				this.mDescriptionItems.add(this.mPendingVisitorsDescriptionItem);
			}
		}

		// Token: 0x0600118B RID: 4491 RVA: 0x0006022C File Offset: 0x0005E42C
		public void onLandedEvent(VisitorEvent visitorEvent)
		{
			for (int i = 0; i < visitorEvent.getVisitorCount(); i++)
			{
				Vector3 vector = this.getPosition() + MathUtil.randFlatVector(4f);
				Guest guest = (Guest)Character.create(TypeList<Specialization, SpecializationList>.find<Visitor>(), vector, Location.Exterior);
				visitorEvent.getVisitorEventType().setStatus(guest);
				guest.setOwnedShip(this);
			}
			this.mPendingVisitors = visitorEvent.getVisitorCount();
		}

		// Token: 0x0600118C RID: 4492 RVA: 0x00060294 File Offset: 0x0005E494
		public void onLandedGeneric()
		{
			float value = Singleton<Colony>.getInstance().getWelfareIndicator().getValue();
			int num = 1;
			if (value > 0.9f)
			{
				num = Random.Range(2, 4);
			}
			else if (value > 0.7f)
			{
				num = Random.Range(1, 3);
			}
			if (this.mSize == LandingShip.Size.Large)
			{
				num++;
			}
			if (this.mIntruders)
			{
				num += LandingShipManager.getExtraIntruders();
				for (int i = 0; i < num; i++)
				{
					Character.create(TypeList<Specialization, SpecializationList>.find<Intruder>(), this.getPosition(), Location.Exterior);
					this.mPendingVisitors = 0;
				}
				return;
			}
			this.mPendingVisitors = num;
			for (int j = 0; j < num; j++)
			{
				Guest guest = (Guest)Character.create(TypeList<Specialization, SpecializationList>.find<Visitor>(), base.getSpawnPosition(j), Location.Exterior);
				guest.decayIndicator(CharacterIndicator.Nutrition, Random.Range(0f, 0.75f));
				guest.decayIndicator(CharacterIndicator.Morale, Random.Range(0f, 1f));
				guest.decayIndicator(CharacterIndicator.Hydration, Random.Range(0f, 0.75f));
				guest.decayIndicator(CharacterIndicator.Sleep, Random.Range(0f, 0.75f));
				guest.setFee(5 * Random.Range(2, 5));
				guest.setOwnedShip(this);
				if (Random.Range(0, 20) == 0)
				{
					guest.setCondition(TypeList<ConditionType, ConditionTypeList>.find<ConditionFlu>());
				}
			}
		}

		// Token: 0x0600118D RID: 4493 RVA: 0x000603D8 File Offset: 0x0005E5D8
		public void setVisitorEvent(VisitorEvent visitorEvent)
		{
			this.mVisitorEvent = visitorEvent;
			this.mVisitorEventName = visitorEvent.getVisitorEventType().GetType().Name;
			VisitorShipType shipType = visitorEvent.getVisitorEventType().getShipType();
			if (shipType != VisitorShipType.Count)
			{
				this.mShipType = shipType;
			}
		}

		// Token: 0x0600118E RID: 4494 RVA: 0x00060419 File Offset: 0x0005E619
		public override string getName()
		{
			return StringList.get("visitor_ship");
		}

		// Token: 0x0600118F RID: 4495 RVA: 0x00060425 File Offset: 0x0005E625
		protected override bool canTakeOff()
		{
			return base.canTakeOff() && this.mPendingVisitors == 0;
		}

		// Token: 0x06001190 RID: 4496 RVA: 0x0006043C File Offset: 0x0005E63C
		public void onVisitorReturned(Human human)
		{
			this.mPendingVisitors--;
			if (this.mPendingVisitorsDescriptionItem != null)
			{
				this.mPendingVisitorsDescriptionItem.setText(this.mPendingVisitors.ToString());
				this.mPendingVisitorsDescriptionItem.setTooltip(StringList.get("tooltip_visitors_on_base", (float)this.mPendingVisitors));
			}
			Guest guest = human as Guest;
			if (guest != null)
			{
				if (!guest.isDead())
				{
					this.mFee += guest.getFee();
					this.mPrestige += guest.getPrestige();
				}
				if (this.mPendingVisitors == 0)
				{
					if (this.mVisitorEventName != null)
					{
						VisitorEventType visitorEventType = TypeList<VisitorEventType, VisitorEventTypeList>.find(this.mVisitorEventName);
						if (visitorEventType != null)
						{
							if (this.mFee > 0)
							{
								Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_visitors_event_left_fee", visitorEventType.getVisitorDescription(), this.mFee.ToString()), this.getIcon(), 0));
							}
							if (this.mPrestige > 0)
							{
								Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_visitors_event_left_prestige", visitorEventType.getVisitorDescription(), this.mPrestige.ToString()), this.getIcon(), 0));
								return;
							}
						}
					}
					else
					{
						if (this.mFee > 0)
						{
							Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_visitors_left_fee", (float)this.mFee), this.getIcon(), 0));
						}
						if (this.mPrestige > 0)
						{
							Singleton<MessageLog>.getInstance().addMessage(new Message(StringList.get("message_visitors_left_prestige", (float)this.mPrestige), this.getIcon(), 0));
						}
					}
				}
			}
		}

		// Token: 0x06001191 RID: 4497 RVA: 0x000605CC File Offset: 0x0005E7CC
		protected override void serialize(XmlNode parent, string name)
		{
			base.serialize(parent, name);
			XmlNode lastChild = parent.LastChild;
			Serialization.serializeString(lastChild, "visitor-event-name", this.mVisitorEventName);
			Serialization.serializeInt(lastChild, "ship-type", (int)this.mShipType);
			Serialization.serializeInt(lastChild, "pending-visitors", this.mPendingVisitors);
			Serialization.serializeInt(lastChild, "fee", this.mFee);
			Serialization.serializeInt(lastChild, "prestige", this.mPrestige);
		}

		// Token: 0x06001192 RID: 4498 RVA: 0x0006063C File Offset: 0x0005E83C
		protected override void deserialize(XmlNode node)
		{
			base.deserialize(node);
			this.mVisitorEventName = Serialization.deserializeString(node["visitor-event-name"]);
			this.mShipType = (VisitorShipType)Serialization.deserializeInt(node["ship-type"]);
			this.mPendingVisitors = Serialization.deserializeInt(node["pending-visitors"]);
			this.mFee = Serialization.deserializeInt(node["fee"]);
			this.mPrestige = Serialization.deserializeInt(node["prestige"]);
			this.addDescriptionItem();
		}

		// Token: 0x06001193 RID: 4499 RVA: 0x000606C4 File Offset: 0x0005E8C4
		public override string getDescription()
		{
			string text = base.getDescription();
			if (Singleton<DebugManager>.getInstance().showExtraDescriptionInfo())
			{
				text = text + "Pending Visitors: " + this.mPendingVisitors.ToString();
			}
			return text;
		}

		// Token: 0x06001194 RID: 4500 RVA: 0x000606FC File Offset: 0x0005E8FC
		public void addPendingVisitor()
		{
			this.mPendingVisitors++;
			this.addDescriptionItem();
			this.mPendingVisitorsDescriptionItem.setText(this.mPendingVisitors.ToString());
		}

		// Token: 0x06001195 RID: 4501 RVA: 0x00060728 File Offset: 0x0005E928
		public int getPendingVisitorCount()
		{
			return this.mPendingVisitors;
		}

		// Token: 0x04000CF1 RID: 3313
		private VisitorShipType mShipType;

		// Token: 0x04000CF2 RID: 3314
		private VisitorEvent mVisitorEvent;

		// Token: 0x04000CF3 RID: 3315
		private int mFee;

		// Token: 0x04000CF4 RID: 3316
		private int mPrestige;

		// Token: 0x04000CF5 RID: 3317
		private string mVisitorEventName;

		// Token: 0x04000CF6 RID: 3318
		private int mPendingVisitors;

		// Token: 0x04000CF7 RID: 3319
		private DescriptionItem mPendingVisitorsDescriptionItem;
	}
}
