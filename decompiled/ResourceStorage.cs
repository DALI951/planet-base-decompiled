using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Planetbase
{
	// Token: 0x0200025E RID: 606
	public class ResourceStorage
	{
		// Token: 0x060011AD RID: 4525 RVA: 0x00060E64 File Offset: 0x0005F064
		public ResourceStorage(Vector3 modulePosition, float radius)
		{
			float num = radius * 0.6f;
			float num2 = radius * 1.5f;
			for (float num3 = -num2; num3 <= num2; num3 += 1.15f)
			{
				for (float num4 = -num2; num4 <= num2; num4 += 1.15f)
				{
					Vector3 vector = new Vector3(num3, 0f, num4);
					float magnitude = vector.magnitude;
					if (magnitude < num)
					{
						this.mSlots.Add(new StorageSlot(modulePosition + vector, 0.5f + (num - magnitude)));
					}
				}
			}
		}

		// Token: 0x060011AE RID: 4526 RVA: 0x00060EF4 File Offset: 0x0005F0F4
		public void destroy()
		{
			int count = this.mSlots.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.mSlots[i] != null)
				{
					this.mSlots[i].destroy();
				}
			}
		}

		// Token: 0x060011AF RID: 4527 RVA: 0x00060F38 File Offset: 0x0005F138
		public ResourceStorage()
		{
		}

		// Token: 0x060011B0 RID: 4528 RVA: 0x00060F4C File Offset: 0x0005F14C
		public StorageSlot findFreeSlot(Vector3 refPosition)
		{
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			StorageSlot storageSlot = null;
			foreach (StorageSlot storageSlot2 in this.mSlots)
			{
				if (storageSlot2.isSpaceAvailable())
				{
					float sqrMagnitude = (refPosition - storageSlot2.getPosition()).sqrMagnitude;
					if (storageSlot2.getHeight() < num || (storageSlot2.getHeight() == num && sqrMagnitude < num2))
					{
						storageSlot = storageSlot2;
						num = storageSlot2.getHeight();
						num2 = sqrMagnitude;
					}
				}
			}
			return storageSlot;
		}

		// Token: 0x060011B1 RID: 4529 RVA: 0x00060FF0 File Offset: 0x0005F1F0
		public float getSpaceRatio()
		{
			float num = 0f;
			float num2 = 0f;
			foreach (StorageSlot storageSlot in this.mSlots)
			{
				float maxHeight = storageSlot.getMaxHeight();
				num += Mathf.Min(storageSlot.getHeight() - 0.2f, maxHeight);
				num2 += maxHeight;
			}
			return Mathf.Clamp01(num / num2);
		}

		// Token: 0x060011B2 RID: 4530 RVA: 0x00061074 File Offset: 0x0005F274
		public int getEmptySlotCount()
		{
			int num = 0;
			using (List<StorageSlot>.Enumerator enumerator = this.mSlots.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.isSpaceAvailable())
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x060011B3 RID: 4531 RVA: 0x000610D0 File Offset: 0x0005F2D0
		public void serialize(XmlNode parent, string name)
		{
			XmlNode xmlNode = Serialization.createNode(parent, name, null);
			foreach (StorageSlot storageSlot in this.mSlots)
			{
				storageSlot.serialize(xmlNode, "slot");
			}
		}

		// Token: 0x060011B4 RID: 4532 RVA: 0x00061130 File Offset: 0x0005F330
		public void deserialize(XmlNode node)
		{
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Name == "slot")
				{
					StorageSlot storageSlot = new StorageSlot();
					storageSlot.deserialize(xmlNode);
					this.mSlots.Add(storageSlot);
				}
			}
		}

		// Token: 0x04000D17 RID: 3351
		private List<StorageSlot> mSlots = new List<StorageSlot>();
	}
}
