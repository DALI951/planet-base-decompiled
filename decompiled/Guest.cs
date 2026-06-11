using System;
using System.Xml;
using UnityEngine;

namespace Planetbase
{
	// Token: 0x020000BB RID: 187
	public class Guest : Human
	{
		// Token: 0x06000491 RID: 1169 RVA: 0x0001BE1F File Offset: 0x0001A01F
		public override void destroy()
		{
			this.clearHighlight();
			base.destroy();
		}

		// Token: 0x06000492 RID: 1170 RVA: 0x0001BE2D File Offset: 0x0001A02D
		protected override void init(Specialization specialization, Vector3 position, Location location)
		{
			base.init(specialization, position, location);
			if (specialization == SpecializationList.IntruderInstance)
			{
				base.setArmed(true);
				this.setAggressionTime(60f);
			}
		}

		// Token: 0x06000493 RID: 1171 RVA: 0x0001BE54 File Offset: 0x0001A054
		public override void tick(float timeStep)
		{
			base.tick(timeStep);
			if (this.mSpecialization.hasFlag(64))
			{
				if (this.mAggressionTime > 0f)
				{
					this.mAggressionTime -= timeStep;
				}
				if (!base.hasStatusFlag(2) && this.mLocation == Location.Interior)
				{
					if (this.mAggressionTime <= 0f)
					{
						this.setAggressive();
					}
					if (!base.getIndicator(CharacterIndicator.Health).isMaxedOut())
					{
						this.setAggressive();
					}
					if (this.mCurrentConstruction != null && this.mCurrentConstruction.isSurveyed())
					{
						this.setDetected();
					}
				}
			}
		}

		// Token: 0x06000494 RID: 1172 RVA: 0x0001BEE8 File Offset: 0x0001A0E8
		protected override void serialize(XmlNode parent, string name)
		{
			base.serialize(parent, name);
			XmlNode lastChild = parent.LastChild;
			Serialization.serializeFloat(lastChild, "agression-time", this.mAggressionTime);
			Serialization.serializeInt(lastChild, "fee", this.mFee);
			Serialization.serializeInt(lastChild, "prestige", this.mPrestige);
		}

		// Token: 0x06000495 RID: 1173 RVA: 0x0001BF38 File Offset: 0x0001A138
		protected override void deserialize(XmlNode node)
		{
			base.deserialize(node);
			this.mAggressionTime = Serialization.deserializeFloat(node["agression-time"]);
			this.mFee = Serialization.deserializeInt(node["fee"]);
			this.mPrestige = Serialization.deserializeInt(node["prestige"]);
		}

		// Token: 0x06000496 RID: 1174 RVA: 0x0001BF90 File Offset: 0x0001A190
		public void setDetected()
		{
			Singleton<MusicManager>.getInstance().onTension();
			if (!base.hasStatusFlag(4))
			{
				Singleton<TimeManager>.getInstance().setNormalSpeed();
				this.mStatusFlags |= 4;
				this.mIntruderHighlight = SpriteManager.add(ResourceList.getInstance().TextureIntruderHighlight, 2f, 0f, Color.red, this.mModel);
				this.mIntruderHighlight.transform.localPosition = new Vector3(0f, 1f, 0f);
				Message message = new Message(StringList.get("message_intruder"), ResourceList.StaticIcons.Intruder, this, 0);
				message.setCondensedMessage(Message.Intruders);
				Singleton<MessageLog>.getInstance().addMessage(message);
				Human.setIdleRadius(this.getPosition(), 15f);
			}
		}

		// Token: 0x06000497 RID: 1175 RVA: 0x0001C05B File Offset: 0x0001A25B
		public void setAggressive()
		{
			this.setDetected();
			this.mStatusFlags |= 2;
		}

		// Token: 0x06000498 RID: 1176 RVA: 0x0001C071 File Offset: 0x0001A271
		public override string getSubtitle()
		{
			if (!base.hasStatusFlag(4) && this.mSpecialization == SpecializationList.IntruderInstance)
			{
				return SpecializationList.VisitorInstance.getName(Human.Gender.Unknown);
			}
			return base.getSubtitle();
		}

		// Token: 0x06000499 RID: 1177 RVA: 0x0001C09B File Offset: 0x0001A29B
		public void setAggressionTime(float aggressionTime)
		{
			this.mAggressionTime = aggressionTime;
		}

		// Token: 0x0600049A RID: 1178 RVA: 0x0001C0A4 File Offset: 0x0001A2A4
		public override void onReturnToShip()
		{
			base.onReturnToShip();
			if (this.mFee > 0)
			{
				Resource.addInmaterialResource(new ResourceAmount(TypeList<ResourceType, ResourceTypeList>.find<Coins>(), this.mFee));
			}
			if (this.mPrestige > 0)
			{
				Singleton<Colony>.getInstance().addExtraPrestige(this.mPrestige);
			}
		}

		// Token: 0x0600049B RID: 1179 RVA: 0x0001C0E3 File Offset: 0x0001A2E3
		public override void onKo()
		{
			base.onKo();
			this.clearHighlight();
		}

		// Token: 0x0600049C RID: 1180 RVA: 0x0001C0F1 File Offset: 0x0001A2F1
		public int getFee()
		{
			return this.mFee;
		}

		// Token: 0x0600049D RID: 1181 RVA: 0x0001C0F9 File Offset: 0x0001A2F9
		public int getPrestige()
		{
			return this.mPrestige;
		}

		// Token: 0x0600049E RID: 1182 RVA: 0x0001C101 File Offset: 0x0001A301
		public void setFee(int fee)
		{
			this.mFee = fee;
		}

		// Token: 0x0600049F RID: 1183 RVA: 0x0001C10A File Offset: 0x0001A30A
		public void setPrestige(int fee)
		{
			this.mPrestige = fee;
		}

		// Token: 0x060004A0 RID: 1184 RVA: 0x0001C113 File Offset: 0x0001A313
		private void clearHighlight()
		{
			if (this.mIntruderHighlight != null)
			{
				SpriteManager.remove(this.mIntruderHighlight);
				this.mIntruderHighlight = null;
			}
		}

		// Token: 0x0400049B RID: 1179
		private GameObject mIntruderHighlight;

		// Token: 0x0400049C RID: 1180
		private int mFee;

		// Token: 0x0400049D RID: 1181
		private int mPrestige;

		// Token: 0x0400049E RID: 1182
		private float mAggressionTime = -1f;
	}
}
