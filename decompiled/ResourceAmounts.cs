using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Planetbase
{
	// Token: 0x0200020E RID: 526
	public class ResourceAmounts
	{
		// Token: 0x06000FD5 RID: 4053 RVA: 0x0005962E File Offset: 0x0005782E
		public ResourceAmounts(string name = null)
		{
			this.mName = name;
		}

		// Token: 0x06000FD6 RID: 4054 RVA: 0x00059648 File Offset: 0x00057848
		public IEnumerator<ResourceAmount> GetEnumerator()
		{
			return this.mResourceAmounts.GetEnumerator();
		}

		// Token: 0x06000FD7 RID: 4055 RVA: 0x0005965A File Offset: 0x0005785A
		public void clear()
		{
			this.mResourceAmounts.Clear();
		}

		// Token: 0x06000FD8 RID: 4056 RVA: 0x00059668 File Offset: 0x00057868
		public ResourceAmounts(ProductAmounts amounts)
		{
			if (amounts != null)
			{
				foreach (ProductAmount productAmount in amounts)
				{
					ProductResource productResource = productAmount.getProduct() as ProductResource;
					if (productResource != null)
					{
						this.add(productResource.getResourceType(), productAmount.getAmount());
					}
				}
				this.mName = amounts.getName();
			}
		}

		// Token: 0x06000FD9 RID: 4057 RVA: 0x000596EC File Offset: 0x000578EC
		public ResourceAmounts clone()
		{
			ResourceAmounts resourceAmounts = new ResourceAmounts(null);
			foreach (ResourceAmount resourceAmount in this.mResourceAmounts)
			{
				resourceAmounts.mResourceAmounts.Add(new ResourceAmount(resourceAmount.getResourceType(), resourceAmount.getAmount()));
			}
			return resourceAmounts;
		}

		// Token: 0x06000FDA RID: 4058 RVA: 0x0005975C File Offset: 0x0005795C
		public void add(ResourceAmounts resourceAmounts)
		{
			if (resourceAmounts != null)
			{
				foreach (ResourceAmount resourceAmount in resourceAmounts)
				{
					this.add(resourceAmount.getResourceType(), resourceAmount.getAmount());
				}
			}
		}

		// Token: 0x06000FDB RID: 4059 RVA: 0x000597B4 File Offset: 0x000579B4
		public void add(ResourceAmount amount)
		{
			this.add(amount.getResourceType(), amount.getAmount());
		}

		// Token: 0x06000FDC RID: 4060 RVA: 0x000597C8 File Offset: 0x000579C8
		public void add(ResourceType resourceType, int amount)
		{
			if (resourceType == null)
			{
				Debug.LogError("Adding null resource amount");
			}
			int count = this.mResourceAmounts.Count;
			for (int i = 0; i < count; i++)
			{
				ResourceAmount resourceAmount = this.mResourceAmounts[i];
				if (resourceType == resourceAmount.getResourceType())
				{
					resourceAmount.setAmount(resourceAmount.getAmount() + amount);
					return;
				}
			}
			this.mResourceAmounts.Add(new ResourceAmount(resourceType, amount));
		}

		// Token: 0x06000FDD RID: 4061 RVA: 0x00059831 File Offset: 0x00057A31
		public void remove(ResourceAmount amount)
		{
			this.remove(amount.getResourceType(), amount.getAmount());
		}

		// Token: 0x06000FDE RID: 4062 RVA: 0x00059848 File Offset: 0x00057A48
		public void remove(ResourceType resourceType, int amount)
		{
			foreach (ResourceAmount resourceAmount in this.mResourceAmounts)
			{
				if (resourceType == resourceAmount.getResourceType())
				{
					int num = resourceAmount.getAmount() - amount;
					if (num > 0)
					{
						resourceAmount.setAmount(num);
						return;
					}
					this.mResourceAmounts.Remove(resourceAmount);
					return;
				}
			}
			Debug.LogWarning("Resource type not found in list: " + resourceType.getName());
		}

		// Token: 0x06000FDF RID: 4063 RVA: 0x000598D8 File Offset: 0x00057AD8
		public ResourceAmount get(int index)
		{
			return this.mResourceAmounts[index];
		}

		// Token: 0x06000FE0 RID: 4064 RVA: 0x000598E6 File Offset: 0x00057AE6
		public bool isEmpty()
		{
			return this.mResourceAmounts.Count == 0;
		}

		// Token: 0x06000FE1 RID: 4065 RVA: 0x000598F8 File Offset: 0x00057AF8
		public bool containsResourceType(ResourceType resourceType)
		{
			using (List<ResourceAmount>.Enumerator enumerator = this.mResourceAmounts.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.getResourceType() == resourceType)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000FE2 RID: 4066 RVA: 0x00059954 File Offset: 0x00057B54
		public int getCount()
		{
			return this.mResourceAmounts.Count;
		}

		// Token: 0x06000FE3 RID: 4067 RVA: 0x00059964 File Offset: 0x00057B64
		public override string ToString()
		{
			string text = "";
			foreach (ResourceAmount resourceAmount in this.mResourceAmounts)
			{
				text = string.Concat(new string[]
				{
					text,
					resourceAmount.getResourceType().getName(),
					" x",
					resourceAmount.getAmount().ToString(),
					" "
				});
			}
			return text;
		}

		// Token: 0x06000FE4 RID: 4068 RVA: 0x000599F8 File Offset: 0x00057BF8
		public string getName()
		{
			return this.mName;
		}

		// Token: 0x06000FE5 RID: 4069 RVA: 0x00059A00 File Offset: 0x00057C00
		public void setName(string name)
		{
			this.mName = name;
		}

		// Token: 0x06000FE6 RID: 4070 RVA: 0x00059A0C File Offset: 0x00057C0C
		public int getTotalAmount()
		{
			int num = 0;
			foreach (ResourceAmount resourceAmount in this.mResourceAmounts)
			{
				num += resourceAmount.getAmount();
			}
			return num;
		}

		// Token: 0x06000FE7 RID: 4071 RVA: 0x00059A64 File Offset: 0x00057C64
		public int getAmount(ResourceType resourceType)
		{
			for (int i = 0; i < this.mResourceAmounts.Count; i++)
			{
				if (this.mResourceAmounts[i].getResourceType() == resourceType)
				{
					return this.mResourceAmounts[i].getAmount();
				}
			}
			return 0;
		}

		// Token: 0x06000FE8 RID: 4072 RVA: 0x00059AAE File Offset: 0x00057CAE
		public ResourceAmount getAmount(int i)
		{
			return this.mResourceAmounts[i];
		}

		// Token: 0x06000FE9 RID: 4073 RVA: 0x00059ABC File Offset: 0x00057CBC
		public void serialize(XmlNode parent, string name)
		{
			XmlNode xmlNode = Serialization.createNode(parent, name, null);
			Serialization.serializeString(xmlNode, "container-name", this.mName);
			foreach (ResourceAmount resourceAmount in this.mResourceAmounts)
			{
				resourceAmount.serialize(xmlNode, "amount");
			}
		}

		// Token: 0x06000FEA RID: 4074 RVA: 0x00059B2C File Offset: 0x00057D2C
		public void deserialize(XmlNode node)
		{
			this.mName = Serialization.deserializeString(node["container-name"]);
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Name == "amount")
				{
					ResourceAmount resourceAmount = new ResourceAmount();
					resourceAmount.deserialize(xmlNode);
					this.mResourceAmounts.Add(resourceAmount);
				}
			}
		}

		// Token: 0x06000FEB RID: 4075 RVA: 0x00059BC0 File Offset: 0x00057DC0
		public int getValue()
		{
			int num = 0;
			foreach (ResourceAmount resourceAmount in this.mResourceAmounts)
			{
				num += resourceAmount.getValue();
			}
			return num;
		}

		// Token: 0x04000C39 RID: 3129
		private string mName;

		// Token: 0x04000C3A RID: 3130
		private List<ResourceAmount> mResourceAmounts = new List<ResourceAmount>();
	}
}
