using System;
using System.Collections.Generic;
using System.Xml;

namespace Planetbase
{
	// Token: 0x02000210 RID: 528
	public class ResourceContainer
	{
		// Token: 0x06000FF5 RID: 4085 RVA: 0x00059E6C File Offset: 0x0005806C
		public ResourceContainer(int capacity, Selectable parent)
		{
			this.mCapacity = capacity;
			this.mParent = parent;
		}

		// Token: 0x06000FF6 RID: 4086 RVA: 0x00059E8D File Offset: 0x0005808D
		public ResourceContainer(Selectable parent)
		{
			this.mCapacity = 0;
			this.mParent = parent;
		}

		// Token: 0x06000FF7 RID: 4087 RVA: 0x00059EAE File Offset: 0x000580AE
		public void destroyResources()
		{
			while (this.mResources.Count > 0)
			{
				this.mResources[0].destroy();
			}
		}

		// Token: 0x06000FF8 RID: 4088 RVA: 0x00059ED1 File Offset: 0x000580D1
		public void setCapacity(int capacity)
		{
			this.mCapacity = capacity;
		}

		// Token: 0x06000FF9 RID: 4089 RVA: 0x00059EDA File Offset: 0x000580DA
		public bool add(Resource resource)
		{
			if (this.mResources.Count < this.mCapacity)
			{
				this.mResources.Add(resource);
				resource.onEmbed(this);
				return true;
			}
			return false;
		}

		// Token: 0x06000FFA RID: 4090 RVA: 0x00059F05 File Offset: 0x00058105
		public Selectable getParent()
		{
			return this.mParent;
		}

		// Token: 0x06000FFB RID: 4091 RVA: 0x00059F0D File Offset: 0x0005810D
		public void remove(Resource resource)
		{
			this.mResources.Remove(resource);
		}

		// Token: 0x06000FFC RID: 4092 RVA: 0x00059F1C File Offset: 0x0005811C
		public Resource remove(ResourceType resourceType)
		{
			foreach (Resource resource in this.mResources)
			{
				if (resource.getResourceType() == resourceType)
				{
					this.mResources.Remove(resource);
					return resource;
				}
			}
			return null;
		}

		// Token: 0x06000FFD RID: 4093 RVA: 0x00059F88 File Offset: 0x00058188
		public Resource remove(ResourceSubtype subtype, List<ResourceSubtype> usedSubtypes)
		{
			foreach (Resource resource in this.mResources)
			{
				ResourceSubtype subtype2 = resource.getSubtype();
				if (subtype == ResourceSubtype.AnyVegetable && resource.getResourceType() == ResourceTypeList.VegetablesInstance && !usedSubtypes.Contains(subtype2))
				{
					usedSubtypes.Add(subtype2);
					this.mResources.Remove(resource);
					return resource;
				}
				if (subtype == ResourceSubtype.AnyMeat && resource.getResourceType() == ResourceTypeList.VitromeatInstance && !usedSubtypes.Contains(subtype2))
				{
					usedSubtypes.Add(subtype2);
					this.mResources.Remove(resource);
					return resource;
				}
				if (resource.getSubtype() == subtype || subtype == ResourceSubtype.None)
				{
					this.mResources.Remove(resource);
					return resource;
				}
			}
			return null;
		}

		// Token: 0x06000FFE RID: 4094 RVA: 0x0005A068 File Offset: 0x00058268
		public Resource extract(ResourceType resourceType)
		{
			foreach (Resource resource in this.mResources)
			{
				if (resource.getResourceType() == resourceType)
				{
					this.remove(resource);
					resource.onExtract();
					return resource;
				}
			}
			return null;
		}

		// Token: 0x06000FFF RID: 4095 RVA: 0x0005A0D4 File Offset: 0x000582D4
		public void extract(Resource targetResource)
		{
			this.remove(targetResource);
			targetResource.onExtract();
		}

		// Token: 0x06001000 RID: 4096 RVA: 0x0005A0E3 File Offset: 0x000582E3
		public bool isFull()
		{
			return this.mResources.Count >= this.mCapacity;
		}

		// Token: 0x06001001 RID: 4097 RVA: 0x0005A0FB File Offset: 0x000582FB
		public bool isSpaceAvailable()
		{
			return this.mResources.Count < this.mCapacity;
		}

		// Token: 0x06001002 RID: 4098 RVA: 0x0005A110 File Offset: 0x00058310
		public int getCapacity()
		{
			return this.mCapacity;
		}

		// Token: 0x06001003 RID: 4099 RVA: 0x0005A118 File Offset: 0x00058318
		public List<Resource> getResources()
		{
			return this.mResources;
		}

		// Token: 0x06001004 RID: 4100 RVA: 0x0005A120 File Offset: 0x00058320
		public bool contains(ResourceType resourceType)
		{
			int count = this.mResources.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.mResources[i].getResourceType() == resourceType)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001005 RID: 4101 RVA: 0x0005A15C File Offset: 0x0005835C
		public bool contains(ResourceSubtype subtype, List<ResourceSubtype> usedSubtypes)
		{
			int count = this.mResources.Count;
			for (int i = 0; i < count; i++)
			{
				Resource resource = this.mResources[i];
				ResourceSubtype subtype2 = resource.getSubtype();
				if (subtype == ResourceSubtype.AnyVegetable && resource.getResourceType() == ResourceTypeList.VegetablesInstance && !usedSubtypes.Contains(subtype2))
				{
					usedSubtypes.Add(subtype2);
					return true;
				}
				if (subtype == ResourceSubtype.AnyMeat && resource.getResourceType() == ResourceTypeList.VitromeatInstance && !usedSubtypes.Contains(subtype2))
				{
					usedSubtypes.Add(subtype2);
					return true;
				}
				if (subtype2 == subtype || subtype == ResourceSubtype.None)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001006 RID: 4102 RVA: 0x0005A1E8 File Offset: 0x000583E8
		public Resource findResource(ResourceType resourceType)
		{
			foreach (Resource resource in this.mResources)
			{
				if (resource.getResourceType() == resourceType)
				{
					return resource;
				}
			}
			return null;
		}

		// Token: 0x06001007 RID: 4103 RVA: 0x0005A244 File Offset: 0x00058444
		public bool isEmpty()
		{
			return this.mResources.Count == 0;
		}

		// Token: 0x06001008 RID: 4104 RVA: 0x0005A254 File Offset: 0x00058454
		public float getStoragetRatio()
		{
			return (float)this.mResources.Count / (float)this.mCapacity;
		}

		// Token: 0x06001009 RID: 4105 RVA: 0x0005A26C File Offset: 0x0005846C
		public void serialize(XmlNode parent, string name)
		{
			XmlNode xmlNode = Serialization.createNode(parent, name, null);
			Serialization.serializeInt(xmlNode, "capacity", this.mCapacity);
			foreach (Resource resource in this.mResources)
			{
				resource.serialize(xmlNode, "resource");
			}
		}

		// Token: 0x0600100A RID: 4106 RVA: 0x0005A2DC File Offset: 0x000584DC
		public void deserialize(XmlNode node)
		{
			this.mCapacity = Serialization.deserializeInt(node["capacity"]);
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Name == "resource")
				{
					Resource resource = Resource.create(xmlNode);
					resource.onEmbed(this);
					this.mResources.Add(resource);
				}
			}
		}

		// Token: 0x0600100B RID: 4107 RVA: 0x0005A370 File Offset: 0x00058570
		public int getCountOf(ResourceType resourceType)
		{
			int count = this.mResources.Count;
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				if (this.mResources[i].getResourceType() == resourceType)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x0600100C RID: 4108 RVA: 0x0005A3B0 File Offset: 0x000585B0
		public int getResourceCount()
		{
			return this.mResources.Count;
		}

		// Token: 0x0600100D RID: 4109 RVA: 0x0005A3BD File Offset: 0x000585BD
		public int getSpaceLeft()
		{
			return this.mCapacity - this.mResources.Count;
		}

		// Token: 0x04000C3C RID: 3132
		public Selectable mParent;

		// Token: 0x04000C3D RID: 3133
		public List<Resource> mResources = new List<Resource>();

		// Token: 0x04000C3E RID: 3134
		private int mCapacity;
	}
}
