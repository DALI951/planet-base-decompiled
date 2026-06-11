using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Planetbase
{
	// Token: 0x02000257 RID: 599
	public abstract class Ship : Selectable
	{
		// Token: 0x0600116B RID: 4459 RVA: 0x0005FBC4 File Offset: 0x0005DDC4
		protected Ship()
		{
			this.mId = Singleton<IdGenerator>.getInstance().generate();
			this.mObject = new GameObject();
			this.mObject.name = string.Concat(new string[]
			{
				"Ship (",
				base.GetType().Name,
				" ",
				this.mId.ToString(),
				")"
			});
			Ship.mShipDictionary.Add(this.mObject, this);
			Ship.mShips.Add(this);
			this.mAudioSource = this.mObject.AddComponent<AudioSource>();
			this.mAudioSource.outputAudioMixerGroup = MixerGroups.getInstance().Game;
		}

		// Token: 0x0600116C RID: 4460 RVA: 0x0005FC7E File Offset: 0x0005DE7E
		public override void destroy()
		{
			Ship.mShipDictionary.Remove(this.mObject);
			Ship.mShips.Remove(this);
			this.end();
		}

		// Token: 0x0600116D RID: 4461 RVA: 0x0005FCA3 File Offset: 0x0005DEA3
		protected virtual void end()
		{
			base.destroy();
			Object.Destroy(this.mObject);
			this.mObject = null;
		}

		// Token: 0x0600116E RID: 4462 RVA: 0x0005FCBD File Offset: 0x0005DEBD
		protected void destroyDeferred()
		{
			Ship.mPendingDestruction.Add(this);
		}

		// Token: 0x0600116F RID: 4463 RVA: 0x0005FCCC File Offset: 0x0005DECC
		protected virtual void serialize(XmlNode parent, string name)
		{
			XmlNode xmlNode = Serialization.createNode(parent, name, base.GetType().Name);
			Serialization.serializeInt(xmlNode, "id", this.mId);
			Serialization.serializeVector3(xmlNode, "position", this.getPosition());
			Serialization.serializeQuaternion(xmlNode, "orientation", this.mObject.transform.localRotation);
		}

		// Token: 0x06001170 RID: 4464 RVA: 0x0005FD28 File Offset: 0x0005DF28
		protected virtual void deserialize(XmlNode node)
		{
			this.mId = Serialization.deserializeId(node["id"]);
			this.mObject.transform.localPosition = Serialization.deserializeVector3(node["position"]);
			this.mObject.transform.localRotation = Serialization.deserializeQuaternion(node["orientation"]);
		}

		// Token: 0x06001171 RID: 4465 RVA: 0x0005FD8B File Offset: 0x0005DF8B
		public override bool isDestroyed()
		{
			return this.mObject == null;
		}

		// Token: 0x06001172 RID: 4466 RVA: 0x0005FD9C File Offset: 0x0005DF9C
		public override string getDescription()
		{
			string text = "";
			if (Singleton<DebugManager>.getInstance().showExtraDescriptionInfo())
			{
				text = text + "Targeters: " + this.mTargeters.Count.ToString() + "\n";
			}
			return text;
		}

		// Token: 0x06001173 RID: 4467 RVA: 0x0005FDE0 File Offset: 0x0005DFE0
		public override GameObject getGameObject()
		{
			return this.mObject;
		}

		// Token: 0x06001174 RID: 4468 RVA: 0x0005FDE8 File Offset: 0x0005DFE8
		public override Vector3 getPosition()
		{
			return this.mObject.transform.position;
		}

		// Token: 0x06001175 RID: 4469 RVA: 0x0005FDFA File Offset: 0x0005DFFA
		public Vector3 getDirection()
		{
			return this.mObject.transform.forward;
		}

		// Token: 0x06001176 RID: 4470 RVA: 0x0005FE0C File Offset: 0x0005E00C
		public override float getHeight()
		{
			return 1f;
		}

		// Token: 0x06001177 RID: 4471 RVA: 0x0005FE13 File Offset: 0x0005E013
		public override float getRadius()
		{
			return 3f;
		}

		// Token: 0x06001178 RID: 4472 RVA: 0x0005FE1A File Offset: 0x0005E01A
		public override Location getLocation()
		{
			return Location.Exterior;
		}

		// Token: 0x06001179 RID: 4473 RVA: 0x0005FE1D File Offset: 0x0005E01D
		public virtual void update(float timeStep)
		{
		}

		// Token: 0x0600117A RID: 4474 RVA: 0x0005FE1F File Offset: 0x0005E01F
		public override int getId()
		{
			return this.mId;
		}

		// Token: 0x0600117B RID: 4475 RVA: 0x0005FE27 File Offset: 0x0005E027
		public override Texture2D getIcon()
		{
			return this.mIcon;
		}

		// Token: 0x0600117C RID: 4476 RVA: 0x0005FE30 File Offset: 0x0005E030
		public static void serializeAll(XmlNode rootNode, string name)
		{
			XmlNode xmlNode = Serialization.createNode(rootNode, name, null);
			int count = Ship.mShips.Count;
			for (int i = 0; i < count; i++)
			{
				Ship ship = Ship.mShips[i];
				if (!ship.isDestroyed())
				{
					ship.serialize(xmlNode, "ship");
				}
			}
		}

		// Token: 0x0600117D RID: 4477 RVA: 0x0005FE80 File Offset: 0x0005E080
		public static void deserializeAll(XmlNode node)
		{
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Name == "ship")
				{
					string text = Serialization.deserializeType(xmlNode);
					if (text == "ColonyShip")
					{
						ColonyShip.create(xmlNode);
					}
					else
					{
						LandingShip.create(text, xmlNode);
					}
				}
			}
		}

		// Token: 0x0600117E RID: 4478 RVA: 0x0005FF0C File Offset: 0x0005E10C
		public static void destroyAll()
		{
			int count = Ship.mShips.Count;
			for (int i = 0; i < count; i++)
			{
				Ship.mShips[i].end();
			}
			Ship.mShipDictionary.Clear();
			Ship.mShips.Clear();
		}

		// Token: 0x0600117F RID: 4479 RVA: 0x0005FF54 File Offset: 0x0005E154
		public static Ship find(GameObject gameObject)
		{
			if (Ship.mShipDictionary.ContainsKey(gameObject))
			{
				return Ship.mShipDictionary[gameObject];
			}
			return null;
		}

		// Token: 0x06001180 RID: 4480 RVA: 0x0005FF70 File Offset: 0x0005E170
		public static Ship find(int id)
		{
			int count = Ship.mShips.Count;
			for (int i = 0; i < count; i++)
			{
				Ship ship = Ship.mShips[i];
				if (ship.getId() == id)
				{
					return ship;
				}
			}
			return null;
		}

		// Token: 0x06001181 RID: 4481 RVA: 0x0005FFAC File Offset: 0x0005E1AC
		public static void updateAll(float timeStep, int frameIndex)
		{
			int count = Ship.mShips.Count;
			for (int i = 0; i < count; i++)
			{
				Ship.mShips[i].update(timeStep);
			}
			int count2 = Ship.mPendingDestruction.Count;
			for (int j = 0; j < count2; j++)
			{
				Ship.mPendingDestruction[j].destroy();
			}
			Ship.mPendingDestruction.Clear();
		}

		// Token: 0x06001182 RID: 4482 RVA: 0x00060012 File Offset: 0x0005E212
		public static Vector3 getAveragePosition()
		{
			return Ship.mShips[0].getPosition();
		}

		// Token: 0x06001183 RID: 4483 RVA: 0x00060024 File Offset: 0x0005E224
		public static int getCountOfType<T>()
		{
			int num = 0;
			int count = Ship.mShips.Count;
			for (int i = 0; i < count; i++)
			{
				if (Ship.mShips[i] is T)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06001184 RID: 4484 RVA: 0x00060064 File Offset: 0x0005E264
		public static T getFirstOfType<T>() where T : Ship
		{
			int count = Ship.mShips.Count;
			for (int i = 0; i < count; i++)
			{
				if (Ship.mShips[i] is T)
				{
					return (T)((object)Ship.mShips[i]);
				}
			}
			return default(T);
		}

		// Token: 0x04000CE6 RID: 3302
		protected Texture2D mIcon;

		// Token: 0x04000CE7 RID: 3303
		protected AudioSource mAudioSource;

		// Token: 0x04000CE8 RID: 3304
		protected GameObject mObject;

		// Token: 0x04000CE9 RID: 3305
		protected int mId;

		// Token: 0x04000CEA RID: 3306
		protected static Dictionary<GameObject, Ship> mShipDictionary = new Dictionary<GameObject, Ship>();

		// Token: 0x04000CEB RID: 3307
		protected static List<Ship> mShips = new List<Ship>();

		// Token: 0x04000CEC RID: 3308
		private static List<Ship> mPendingDestruction = new List<Ship>();
	}
}
