using System;
using System.Xml;

namespace Planetbase
{
	// Token: 0x0200020D RID: 525
	public class ResourceAmount
	{
		// Token: 0x06000FCC RID: 4044 RVA: 0x00059559 File Offset: 0x00057759
		public ResourceAmount(ResourceType resourceType, int amount)
		{
			this.mResourceType = resourceType;
			this.mAmount = amount;
		}

		// Token: 0x06000FCD RID: 4045 RVA: 0x0005956F File Offset: 0x0005776F
		public ResourceAmount()
		{
		}

		// Token: 0x06000FCE RID: 4046 RVA: 0x00059577 File Offset: 0x00057777
		public ResourceType getResourceType()
		{
			return this.mResourceType;
		}

		// Token: 0x06000FCF RID: 4047 RVA: 0x0005957F File Offset: 0x0005777F
		public int getAmount()
		{
			return this.mAmount;
		}

		// Token: 0x06000FD0 RID: 4048 RVA: 0x00059587 File Offset: 0x00057787
		public void setAmount(int amount)
		{
			this.mAmount = amount;
		}

		// Token: 0x06000FD1 RID: 4049 RVA: 0x00059590 File Offset: 0x00057790
		public void serialize(XmlNode parent, string name)
		{
			XmlNode xmlNode = Serialization.createNode(parent, name, null);
			Serialization.serializeString(xmlNode, "resource-type", this.mResourceType.GetType().Name);
			Serialization.serializeInt(xmlNode, "amount", this.mAmount);
		}

		// Token: 0x06000FD2 RID: 4050 RVA: 0x000595C5 File Offset: 0x000577C5
		public void deserialize(XmlNode node)
		{
			this.mResourceType = TypeList<ResourceType, ResourceTypeList>.find(Serialization.deserializeString(node["resource-type"]));
			this.mAmount = Serialization.deserializeInt(node["amount"]);
		}

		// Token: 0x06000FD3 RID: 4051 RVA: 0x000595F8 File Offset: 0x000577F8
		public int getValue()
		{
			return this.mResourceType.getValue() * this.mAmount;
		}

		// Token: 0x06000FD4 RID: 4052 RVA: 0x0005960C File Offset: 0x0005780C
		public override string ToString()
		{
			return this.mResourceType.getName() + ": " + this.mAmount.ToString();
		}

		// Token: 0x04000C37 RID: 3127
		private ResourceType mResourceType;

		// Token: 0x04000C38 RID: 3128
		private int mAmount;
	}
}
