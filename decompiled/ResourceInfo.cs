using System;
using UnityEngine;

namespace Planetbase
{
	// Token: 0x0200015A RID: 346
	public class ResourceInfo
	{
		// Token: 0x06000B06 RID: 2822 RVA: 0x00042836 File Offset: 0x00040A36
		public ResourceInfo(ResourceType resourceType, int amount, int freeAmount)
		{
			this.mContent = new GUIContent();
			this.setAmount(resourceType, amount, freeAmount);
		}

		// Token: 0x06000B07 RID: 2823 RVA: 0x00042852 File Offset: 0x00040A52
		public GUIContent getContent()
		{
			return this.mContent;
		}

		// Token: 0x06000B08 RID: 2824 RVA: 0x0004285C File Offset: 0x00040A5C
		public void setAmount(ResourceType resourceType, int amount, int freeAmount)
		{
			if (resourceType != this.mResourceType || this.mAmount != amount || this.mFreeAmount != freeAmount)
			{
				this.mResourceType = resourceType;
				this.mAmount = amount;
				this.mFreeAmount = freeAmount;
				if (amount == freeAmount || !this.mResourceType.hasFlag(128))
				{
					this.mContent.tooltip = resourceType.getName() + ": " + amount.ToString();
				}
				else
				{
					this.mContent.tooltip = StringList.get("tooltip_resource_usable", resourceType.getName(), freeAmount.ToString(), amount.ToString());
				}
				if (amount < 10000)
				{
					this.mContent.text = this.mFreeAmount.ToString();
				}
				else
				{
					this.mContent.text = StringList.get("infinity");
				}
				this.mContent.image = resourceType.getIcon();
			}
		}

		// Token: 0x06000B09 RID: 2825 RVA: 0x00042945 File Offset: 0x00040B45
		public ResourceType getResourceType()
		{
			return this.mResourceType;
		}

		// Token: 0x06000B0A RID: 2826 RVA: 0x0004294D File Offset: 0x00040B4D
		public int getAmount()
		{
			return this.mAmount;
		}

		// Token: 0x06000B0B RID: 2827 RVA: 0x00042955 File Offset: 0x00040B55
		public int getFreeAmount()
		{
			return this.mFreeAmount;
		}

		// Token: 0x0400085A RID: 2138
		private GUIContent mContent;

		// Token: 0x0400085B RID: 2139
		private int mAmount;

		// Token: 0x0400085C RID: 2140
		private int mFreeAmount;

		// Token: 0x0400085D RID: 2141
		private ResourceType mResourceType;
	}
}
