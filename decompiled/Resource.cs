using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Planetbase
{
	// Token: 0x0200020C RID: 524
	public class Resource : Selectable
	{
		// Token: 0x06000F74 RID: 3956 RVA: 0x00057920 File Offset: 0x00055B20
		public static Resource create(ResourceType resourceType, Vector3 position, Location location)
		{
			ResourceSubtype resourceSubtype = ResourceSubtype.None;
			if (resourceType == ResourceTypeList.VegetablesInstance)
			{
				resourceSubtype = ResourceSubtype.Tomatoes;
			}
			else if (resourceType == ResourceTypeList.MealInstance)
			{
				resourceSubtype = ResourceSubtype.Basic;
			}
			else if (resourceType == ResourceTypeList.VitromeatInstance)
			{
				resourceSubtype = ResourceSubtype.Chicken;
			}
			return Resource.create(resourceType, resourceSubtype, position, location);
		}

		// Token: 0x06000F75 RID: 3957 RVA: 0x0005795C File Offset: 0x00055B5C
		public static Resource create(ResourceType resourceType, ResourceSubtype subtype, Vector3 position, Location location)
		{
			Resource resource = new Resource();
			resource.init(resourceType, subtype, position, location);
			resource.postInit();
			return resource;
		}

		// Token: 0x06000F76 RID: 3958 RVA: 0x00057973 File Offset: 0x00055B73
		public static Resource create(XmlNode xmlNode)
		{
			Resource resource = new Resource();
			resource.deserialize(xmlNode);
			resource.postInit();
			return resource;
		}

		// Token: 0x06000F77 RID: 3959 RVA: 0x00057988 File Offset: 0x00055B88
		protected Resource()
		{
			if (Resource.mParentObject == null)
			{
				Resource.mParentObject = new GameObject();
				Resource.mParentObject.name = "Resources";
			}
			this.mObject = new GameObject();
			this.mObject.transform.parent = Resource.mParentObject.transform;
			this.mObject.layer = 10;
			Resource.mResourceDictionary.Add(this.mObject, this);
			Resource.mResources.Add(this);
			this.mConditionIndicator = new Indicator(StringList.get("condition"), ResourceList.StaticIcons.Condition, IndicatorType.Condition, 1f, 1f, SignType.Unknown);
			this.mIndicators = new Indicator[1];
			this.mIndicators[0] = this.mConditionIndicator;
		}

		// Token: 0x06000F78 RID: 3960 RVA: 0x00057A5D File Offset: 0x00055C5D
		public void init(ResourceType resourceType, ResourceSubtype subtype, Vector3 position, Location location)
		{
			this.mResourceType = resourceType;
			this.mSubtype = subtype;
			this.mLocation = location;
			this.mState = Resource.State.Idle;
			this.setPosition(position);
			this.mId = Singleton<IdGenerator>.getInstance().generate();
		}

		// Token: 0x06000F79 RID: 3961 RVA: 0x00057A94 File Offset: 0x00055C94
		private void postInit()
		{
			this.setModel(this.mResourceType.createModel());
			Resource.mTotalAmounts.add(this.mResourceType, 1);
			this.updateCollider();
			if (!Resource.mTypeResources.ContainsKey(this.mResourceType))
			{
				Resource.mTypeResources.Add(this.mResourceType, new List<Resource>());
			}
			Resource.mTypeResources[this.mResourceType].Add(this);
		}

		// Token: 0x06000F7A RID: 3962 RVA: 0x00057B08 File Offset: 0x00055D08
		private void setModel(GameObject model)
		{
			this.mModel = model;
			this.mModel.transform.SetParent(this.mObject.transform, false);
			this.mModel.setLayerRecursive(10);
			this.mObject.name = this.mResourceType.getName();
			this.mCollider = this.mModel.GetComponentInChildren<Collider>();
			if (this.mSubtype != ResourceSubtype.None)
			{
				this.mSubtitle += StringList.get("resource_subtype_" + Util.camelCaseToLowercase(this.mSubtype.ToString()));
			}
		}

		// Token: 0x06000F7B RID: 3963 RVA: 0x00057BAA File Offset: 0x00055DAA
		private void updateCollider()
		{
			if (this.mCollider != null)
			{
				this.mCollider.enabled = this.mState != Resource.State.Loaded && !this.isEmbedded();
			}
		}

		// Token: 0x06000F7C RID: 3964 RVA: 0x00057BDA File Offset: 0x00055DDA
		public override void destroy()
		{
			Resource.mResourceDictionary.Remove(this.mObject);
			Resource.mResources.Remove(this);
			Resource.mTypeResources[this.mResourceType].Remove(this);
			this.end();
		}

		// Token: 0x06000F7D RID: 3965 RVA: 0x00057C18 File Offset: 0x00055E18
		private void end()
		{
			Resource.mTotalAmounts.remove(this.mResourceType, 1);
			if (this.mContainer != null)
			{
				this.mContainer.remove(this);
			}
			if (this.mStorageSlot != null)
			{
				this.mStorageSlot.removeResource(this);
			}
			Object.Destroy(this.mObject);
			this.mObject = null;
		}

		// Token: 0x06000F7E RID: 3966 RVA: 0x00057C70 File Offset: 0x00055E70
		public override bool isDestroyed()
		{
			return this.mObject == null;
		}

		// Token: 0x06000F7F RID: 3967 RVA: 0x00057C7E File Offset: 0x00055E7E
		public override bool isDeleteable(out bool buttonEnabled)
		{
			buttonEnabled = this.mState == Resource.State.Idle && !this.isEmbedded() && this.mStorageSlot == null && this.mContainer == null && this.mTraderId == -1;
			return true;
		}

		// Token: 0x06000F80 RID: 3968 RVA: 0x00057CAF File Offset: 0x00055EAF
		public override string getName()
		{
			return this.mResourceType.getName();
		}

		// Token: 0x06000F81 RID: 3969 RVA: 0x00057CBC File Offset: 0x00055EBC
		public override string getSubtitle()
		{
			return this.mSubtitle;
		}

		// Token: 0x06000F82 RID: 3970 RVA: 0x00057CC4 File Offset: 0x00055EC4
		public override string getDescription()
		{
			string text = base.getDescription();
			if (Singleton<DebugManager>.getInstance().showExtraDescriptionInfo())
			{
				text += this.getExtraDescription();
			}
			return text;
		}

		// Token: 0x06000F83 RID: 3971 RVA: 0x00057CF4 File Offset: 0x00055EF4
		public string getExtraDescription()
		{
			string text = "";
			text = text + "Id: " + this.mId.ToString() + "\n";
			text = text + "Location: " + Constants.locationToString(this.mLocation) + "\n";
			text = text + "State: " + Resource.stateToString(this.mState) + "\n";
			text = text + "Durability: " + this.mDurability.ToString() + "\n";
			if (this.mTraderId != -1)
			{
				text = text + "Trader: " + this.mTraderId.ToString() + "\n";
			}
			if (this.mContainer != null)
			{
				text = text + "Container: " + this.mContainer.getParent().getName() + "\n";
			}
			return text;
		}

		// Token: 0x06000F84 RID: 3972 RVA: 0x00057DCC File Offset: 0x00055FCC
		public override Texture2D getIcon()
		{
			return this.mResourceType.getIcon();
		}

		// Token: 0x06000F85 RID: 3973 RVA: 0x00057DD9 File Offset: 0x00055FD9
		public override GameObject getGameObject()
		{
			return this.mObject;
		}

		// Token: 0x06000F86 RID: 3974 RVA: 0x00057DE1 File Offset: 0x00055FE1
		public override GameObject getSelectionModel()
		{
			return this.mModel;
		}

		// Token: 0x06000F87 RID: 3975 RVA: 0x00057DEC File Offset: 0x00055FEC
		public override Vector3 getPosition()
		{
			if (this.mContainer != null && !this.mContainer.getParent().isDestroyed())
			{
				return this.mContainer.getParent().getPosition();
			}
			if (this.mObject == null)
			{
				Debug.LogError("Trying to get position of null resource: " + this.getName() + ", " + this.getExtraDescription());
			}
			return this.mObject.transform.position;
		}

		// Token: 0x06000F88 RID: 3976 RVA: 0x00057E62 File Offset: 0x00056062
		public bool isTraded()
		{
			return this.mTraderId != -1;
		}

		// Token: 0x06000F89 RID: 3977 RVA: 0x00057E70 File Offset: 0x00056070
		public int getTraderId()
		{
			return this.mTraderId;
		}

		// Token: 0x06000F8A RID: 3978 RVA: 0x00057E78 File Offset: 0x00056078
		public override float getHeight()
		{
			return 1f;
		}

		// Token: 0x06000F8B RID: 3979 RVA: 0x00057E7F File Offset: 0x0005607F
		public override float getRadius()
		{
			return this.mResourceType.getRadius();
		}

		// Token: 0x06000F8C RID: 3980 RVA: 0x00057E8C File Offset: 0x0005608C
		public ResourceType getResourceType()
		{
			return this.mResourceType;
		}

		// Token: 0x06000F8D RID: 3981 RVA: 0x00057E94 File Offset: 0x00056094
		public override string getHelpId()
		{
			return this.mResourceType.GetType().Name;
		}

		// Token: 0x06000F8E RID: 3982 RVA: 0x00057EA6 File Offset: 0x000560A6
		public Vector3 getBottomPosition()
		{
			return this.mObject.transform.position - Vector3.up * this.getRadius();
		}

		// Token: 0x06000F8F RID: 3983 RVA: 0x00057ECD File Offset: 0x000560CD
		public void setState(Resource.State state)
		{
			this.mState = state;
		}

		// Token: 0x06000F90 RID: 3984 RVA: 0x00057ED6 File Offset: 0x000560D6
		public void setPosition(Vector3 position)
		{
			if (this.mObject == null)
			{
				Debug.LogWarning("Trying to move destroyed resource: " + this.mResourceType.GetType().Name);
			}
			this.mObject.transform.position = position;
		}

		// Token: 0x06000F91 RID: 3985 RVA: 0x00057F16 File Offset: 0x00056116
		public void setRotation(Quaternion rotation)
		{
			this.mObject.transform.rotation = rotation;
		}

		// Token: 0x06000F92 RID: 3986 RVA: 0x00057F2C File Offset: 0x0005612C
		public void store(StorageSlot storageSlot)
		{
			this.mObject.name = this.mResourceType.getName() + " (stored)";
			this.mStorageSlot = storageSlot;
			this.mState = Resource.State.Stored;
			this.mObject.transform.localRotation = Quaternion.Euler(0f, (float)Random.Range(0, 4) * 90f + Random.Range(-0.5f, 5f), 0f);
			this.mObject.layer = 10;
			this.mModel.layer = 10;
			this.setPosition(storageSlot.getPosition() + Vector3.up * storageSlot.getHeight());
			this.updateCollider();
		}

		// Token: 0x06000F93 RID: 3987 RVA: 0x00057FE5 File Offset: 0x000561E5
		public void removeFromStorage()
		{
			this.mStorageSlot = null;
			this.mState = Resource.State.Idle;
			this.mLocation = Location.Exterior;
		}

		// Token: 0x06000F94 RID: 3988 RVA: 0x00057FFC File Offset: 0x000561FC
		public void load()
		{
			this.mObject.name = this.mResourceType.getName() + " (loaded)";
			if (this.mStorageSlot != null)
			{
				this.mStorageSlot.removeResource(this);
				this.mStorageSlot = null;
			}
			else
			{
				this.mObject.layer = 2;
				this.mModel.layer = 2;
				Resource resource = this.findResourceOnTop();
				if (resource != null)
				{
					resource.dropDown(0);
				}
				this.mObject.layer = 10;
				this.mModel.layer = 10;
			}
			this.mState = Resource.State.Loaded;
			this.updateCollider();
		}

		// Token: 0x06000F95 RID: 3989 RVA: 0x00058098 File Offset: 0x00056298
		public void drop(Resource.State state)
		{
			this.mState = state;
			this.mObject.name = this.mResourceType.getName() + " (" + state.ToString().ToLower() + ")";
			this.mObject.layer = 2;
			this.mModel.layer = 2;
			Vector3 eulerAngles = this.mModel.transform.rotation.eulerAngles;
			eulerAngles.x = 0f;
			eulerAngles.z = 0f;
			this.mObject.transform.rotation = Quaternion.Euler(eulerAngles);
			Vector3 vector2;
			Vector3 vector = (vector2 = this.getPosition() + Vector3.up * 4f);
			int num = 4199680;
			RaycastHit raycastHit;
			if (Physics.SphereCast(vector, this.mResourceType.getSize().x * 0.5f, Vector3.down, out raycastHit, float.PositiveInfinity, num))
			{
				vector2 = raycastHit.point;
				GameObject gameObject = raycastHit.collider.gameObject;
				if (gameObject != null)
				{
					Resource resource = Resource.find(gameObject.getParent());
					if (resource != null)
					{
						vector2.x = resource.getPosition().x;
						vector2.z = resource.getPosition().z;
					}
				}
			}
			this.mObject.layer = 10;
			this.mModel.layer = 10;
			this.setPosition(vector2);
			this.updateCollider();
		}

		// Token: 0x06000F96 RID: 3990 RVA: 0x00058210 File Offset: 0x00056410
		public void dropDown(int depth = 0)
		{
			this.mObject.layer = 2;
			this.mModel.layer = 2;
			int num = 5376;
			RaycastHit raycastHit;
			if (Physics.SphereCast(this.getPosition(), 0.5f, Vector3.down, out raycastHit, float.PositiveInfinity, num))
			{
				this.setPosition(raycastHit.point);
			}
			Resource resource = this.findResourceOnTop();
			this.mObject.layer = 10;
			this.mModel.layer = 10;
			if (depth < 3 && resource != null && resource != this)
			{
				resource.dropDown(depth + 1);
			}
			this.updateCollider();
		}

		// Token: 0x06000F97 RID: 3991 RVA: 0x000582A4 File Offset: 0x000564A4
		private Resource findResourceOnTop()
		{
			int num = 1024;
			RaycastHit raycastHit;
			if (Physics.SphereCast(this.getPosition(), 0.5f, Vector3.up, out raycastHit, float.PositiveInfinity, num))
			{
				GameObject gameObject = raycastHit.collider.gameObject.transform.parent.gameObject;
				if (Resource.mResourceDictionary.ContainsKey(gameObject))
				{
					return Resource.mResourceDictionary[gameObject];
				}
			}
			return null;
		}

		// Token: 0x06000F98 RID: 3992 RVA: 0x0005830C File Offset: 0x0005650C
		public override float getOutlineWidth()
		{
			return 0.25f;
		}

		// Token: 0x06000F99 RID: 3993 RVA: 0x00058313 File Offset: 0x00056513
		public Resource.State getState()
		{
			return this.mState;
		}

		// Token: 0x06000F9A RID: 3994 RVA: 0x0005831B File Offset: 0x0005651B
		public bool isEmbedded()
		{
			return this.mContainer != null;
		}

		// Token: 0x06000F9B RID: 3995 RVA: 0x00058328 File Offset: 0x00056528
		public void onEmbed(ResourceContainer container)
		{
			this.mObject.name = this.mResourceType.getName() + " (embedded - " + this.mState.ToString().ToLower() + ")";
			this.setPosition(container.getParent().getPosition());
			this.mObject.SetActive(false);
			this.mContainer = container;
			this.updateCollider();
		}

		// Token: 0x06000F9C RID: 3996 RVA: 0x0005839A File Offset: 0x0005659A
		public void onExtract()
		{
			this.mObject.name = this.mResourceType.getName();
			this.mObject.SetActive(true);
			this.mContainer = null;
		}

		// Token: 0x06000F9D RID: 3997 RVA: 0x000583C5 File Offset: 0x000565C5
		public ResourceContainer getContainer()
		{
			return this.mContainer;
		}

		// Token: 0x06000F9E RID: 3998 RVA: 0x000583CD File Offset: 0x000565CD
		public override Location getLocation()
		{
			return this.mLocation;
		}

		// Token: 0x06000F9F RID: 3999 RVA: 0x000583D5 File Offset: 0x000565D5
		public void setLocation(Location location)
		{
			this.mLocation = location;
		}

		// Token: 0x06000FA0 RID: 4000 RVA: 0x000583DE File Offset: 0x000565DE
		private bool isIndependent()
		{
			return this.mContainer == null && this.mStorageSlot == null;
		}

		// Token: 0x06000FA1 RID: 4001 RVA: 0x000583F4 File Offset: 0x000565F4
		public void serialize(XmlNode parent, string name)
		{
			XmlNode xmlNode = Serialization.createNode(parent, name, this.mResourceType.GetType().Name);
			Serialization.serializeInt(xmlNode, "id", this.mId);
			Serialization.serializeInt(xmlNode, "trader-id", this.mTraderId);
			Serialization.serializeVector3(xmlNode, "position", this.getPosition());
			Serialization.serializeQuaternion(xmlNode, "orientation", this.mObject.transform.localRotation);
			Serialization.serializeInt(xmlNode, "state", (int)this.mState);
			Serialization.serializeInt(xmlNode, "location", (int)this.mLocation);
			Serialization.serializeInt(xmlNode, "subtype", (int)this.mSubtype);
			Serialization.serializeFloat(xmlNode, "condition", this.mConditionIndicator.getValue());
			Serialization.serializeFloat(xmlNode, "durability", (float)this.mDurability);
		}

		// Token: 0x06000FA2 RID: 4002 RVA: 0x000584C0 File Offset: 0x000566C0
		public void deserialize(XmlNode node)
		{
			this.mResourceType = TypeList<ResourceType, ResourceTypeList>.find(Serialization.deserializeType(node));
			this.mId = Serialization.deserializeId(node["id"]);
			if (node["trader-id"] != null)
			{
				this.mTraderId = Serialization.deserializeInt(node["trader-id"]);
			}
			else
			{
				this.mTraderId = -1;
			}
			this.setPosition(Serialization.deserializeVector3(node["position"]));
			this.setRotation(Serialization.deserializeQuaternion(node["orientation"]));
			this.mState = (Resource.State)Serialization.deserializeInt(node["state"]);
			this.mLocation = (Location)Serialization.deserializeInt(node["location"]);
			this.mSubtype = (ResourceSubtype)Serialization.deserializeInt(node["subtype"]);
			this.setDurability((Resource.Durability)Serialization.deserializeInt(node["durability"]));
			if (node["condition"] != null)
			{
				this.mConditionIndicator.setValue(Serialization.deserializeFloat(node["condition"]));
			}
		}

		// Token: 0x06000FA3 RID: 4003 RVA: 0x000585CC File Offset: 0x000567CC
		public override int getId()
		{
			return this.mId;
		}

		// Token: 0x06000FA4 RID: 4004 RVA: 0x000585D4 File Offset: 0x000567D4
		private bool canBeTraded()
		{
			return !this.isDestroyed() && !this.isTraded() && (this.mState == Resource.State.Stored || (!this.isEmbedded() && this.mState == Resource.State.Idle));
		}

		// Token: 0x06000FA5 RID: 4005 RVA: 0x00058608 File Offset: 0x00056808
		private bool isUsable()
		{
			return !this.isDestroyed() && !this.isTraded() && (this.mState == Resource.State.Idle || this.mState == Resource.State.Stored);
		}

		// Token: 0x06000FA6 RID: 4006 RVA: 0x0005862F File Offset: 0x0005682F
		public bool hasFlag(int flag)
		{
			return this.mResourceType.hasFlag(flag);
		}

		// Token: 0x06000FA7 RID: 4007 RVA: 0x0005863D File Offset: 0x0005683D
		public ResourceSubtype getSubtype()
		{
			return this.mSubtype;
		}

		// Token: 0x06000FA8 RID: 4008 RVA: 0x00058648 File Offset: 0x00056848
		public bool isOnTopOf(Collider collider)
		{
			Transform transform = base.getTransform();
			Vector3 vector = this.mResourceType.getSize() * 0.5f;
			Vector3 vector2 = transform.position + transform.right * vector.x + transform.forward * vector.z;
			Vector3 vector3 = transform.position + transform.right * vector.x - transform.forward * vector.z;
			Vector3 vector4 = transform.position - transform.right * vector.x + transform.forward * vector.z;
			Vector3 vector5 = transform.position - transform.right * vector.x - transform.forward * vector.z;
			RaycastHit raycastHit;
			return collider.Raycast(new Ray(vector2, Vector3.down), out raycastHit, float.PositiveInfinity) || collider.Raycast(new Ray(vector3, Vector3.down), out raycastHit, float.PositiveInfinity) || collider.Raycast(new Ray(vector4, Vector3.down), out raycastHit, float.PositiveInfinity) || collider.Raycast(new Ray(vector5, Vector3.down), out raycastHit, float.PositiveInfinity);
		}

		// Token: 0x06000FA9 RID: 4009 RVA: 0x000587AC File Offset: 0x000569AC
		public void unpack(Vector3 position, Quaternion rotation)
		{
			if (this.mResourceType.hasUnpackedModel())
			{
				Object.Destroy(this.mModel);
				this.setModel(this.mResourceType.createModelUnpacked(this.mSubtype));
				this.mObject.transform.position = position;
				this.mObject.transform.rotation = rotation;
				if (!this.hasFlag(8))
				{
					this.detach();
				}
			}
		}

		// Token: 0x06000FAA RID: 4010 RVA: 0x00058819 File Offset: 0x00056A19
		public void attach(Transform anchorPoint)
		{
			this.mObject.transform.parent = anchorPoint;
			this.mObject.transform.position = anchorPoint.position;
			this.mObject.transform.rotation = anchorPoint.rotation;
		}

		// Token: 0x06000FAB RID: 4011 RVA: 0x00058858 File Offset: 0x00056A58
		public void detach()
		{
			this.mObject.transform.parent = Resource.mParentObject.transform;
		}

		// Token: 0x06000FAC RID: 4012 RVA: 0x00058874 File Offset: 0x00056A74
		public void tick(float timeStep)
		{
			if (this.mState == Resource.State.Idle && !this.isEmbedded() && this.mStorageSlot == null && this.mContainer == null && this.mTraderId == -1)
			{
				float num = ((this.mDurability == Resource.Durability.Normal) ? 1f : 5f);
				this.mConditionIndicator.decrease(timeStep / (3600f * num));
				if (this.mConditionIndicator.isMin())
				{
					Resource.mPendingDestruction.Add(this);
					return;
				}
			}
			else if (this.mState == Resource.State.Stored)
			{
				this.mConditionIndicator.increase(timeStep / 360f);
			}
		}

		// Token: 0x06000FAD RID: 4013 RVA: 0x0005890C File Offset: 0x00056B0C
		public void setDurability(Resource.Durability durability)
		{
			this.mDurability = durability;
			if (this.mDurability == Resource.Durability.High)
			{
				if (!this.mDescriptionItems.contains(DescriptionItem.HighDurability))
				{
					this.mDescriptionItems.add(DescriptionItem.HighDurability);
					return;
				}
			}
			else if (this.mDescriptionItems.contains(DescriptionItem.HighDurability))
			{
				this.mDescriptionItems.remove(DescriptionItem.HighDurability);
			}
		}

		// Token: 0x06000FAE RID: 4014 RVA: 0x0005896E File Offset: 0x00056B6E
		public override ICollection<Indicator> getIndicators()
		{
			return this.mIndicators;
		}

		// Token: 0x06000FAF RID: 4015 RVA: 0x00058978 File Offset: 0x00056B78
		public void reassessLocation()
		{
			if (!this.isEmbedded() && (this.mState == Resource.State.Idle || this.mState == Resource.State.Stored))
			{
				this.mLocation = (Physics.Raycast(new Ray(this.getPosition() + Vector3.up * 5f, Vector3.down), float.PositiveInfinity, 4096) ? Location.Interior : Location.Exterior);
			}
		}

		// Token: 0x06000FB0 RID: 4016 RVA: 0x000589DF File Offset: 0x00056BDF
		private bool calculateReachable()
		{
			return this.mLocation != Location.Exterior || NavigationGraph.isExteriorLocationReachable(this.getPosition());
		}

		// Token: 0x06000FB1 RID: 4017 RVA: 0x000589F8 File Offset: 0x00056BF8
		public static void updateAll(float timeStep, int frameIndex)
		{
			float num = timeStep * 64f;
			float num2 = (float)Resource.mResources.Count;
			int num3 = 0;
			while ((float)num3 < num2)
			{
				Resource resource = Resource.mResources[num3];
				if ((resource.getId() & 63) == frameIndex)
				{
					resource.tick(num);
				}
				num3++;
			}
			for (int i = 0; i < Resource.mPendingDestruction.Count; i++)
			{
				Resource.mPendingDestruction[i].destroy();
			}
			Resource.mPendingDestruction.Clear();
		}

		// Token: 0x06000FB2 RID: 4018 RVA: 0x00058A79 File Offset: 0x00056C79
		public static string stateToString(Resource.State state)
		{
			switch (state)
			{
			case Resource.State.Idle:
				return "Idle";
			case Resource.State.Loaded:
				return "Loaded";
			case Resource.State.Stored:
				return "Stored";
			case Resource.State.Busy:
				return "Busy";
			default:
				return "Unknown";
			}
		}

		// Token: 0x06000FB3 RID: 4019 RVA: 0x00058AB0 File Offset: 0x00056CB0
		public static Resource findStorable(Character character, ResourceType resouceType, bool includeStorages)
		{
			int count = Resource.mResources.Count;
			Resource resource = null;
			float num = float.MaxValue;
			for (int i = 0; i < count; i++)
			{
				Resource resource2 = Resource.mResources[i];
				if (resouceType == null || resouceType == resource2.getResourceType())
				{
					bool flag = !resource2.isEmbedded();
					if (resource2.getLocation() == Location.Exterior && !Singleton<SecurityManager>.getInstance().isGoingOutsideAllowed())
					{
						flag = false;
					}
					else if (!flag)
					{
						ConstructionComponent constructionComponent = resource2.getContainer().getParent() as ConstructionComponent;
						if (constructionComponent != null && constructionComponent.hasFlag(1048576) && constructionComponent.canProduceResource(resource2.getResourceType()))
						{
							flag = true;
						}
					}
					if (flag && !resource2.isTraded() && resource2.getPotentialUserCount(character) == 0 && (resource2.mState == Resource.State.Idle || (includeStorages && resource2.mStorageSlot != null)))
					{
						float num2 = (resource2.getPosition() - character.getPosition()).magnitude;
						if (character.getLocation() == resource2.getLocation())
						{
							num2 -= 20f;
						}
						num2 += resource2.mConditionIndicator.getValue() * 100f;
						if (num2 < num && resource2.calculateReachable())
						{
							num = num2;
							resource = resource2;
						}
					}
				}
			}
			return resource;
		}

		// Token: 0x06000FB4 RID: 4020 RVA: 0x00058BF8 File Offset: 0x00056DF8
		public static Resource findAvailable(Character character, Vector3 position, Location preferredLocation, ResourceType resourceType, bool emergency = false)
		{
			if (character == null)
			{
				throw new Exception("Character is null");
			}
			List<Resource> list = null;
			if (!Resource.mTypeResources.TryGetValue(resourceType, out list))
			{
				return null;
			}
			Resource resource = null;
			float num = float.MaxValue;
			bool flag = Singleton<SecurityManager>.getInstance().isGoingOutsideAllowed();
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				Resource resource2 = list[i];
				if (resource2 == null)
				{
					throw new Exception("Resource is null");
				}
				if ((flag || resource2.getLocation() == Location.Interior) && resource2.isUsable() && resource2.getLocation() == preferredLocation && (emergency || resource2.getPotentialUserCount(character) == 0))
				{
					float num2 = (position - resource2.getPosition()).magnitude;
					num2 -= resource2.getPosition().y * 2f;
					if (num2 < num && resource2.calculateReachable())
					{
						resource = resource2;
						num = num2;
					}
				}
			}
			if (resource != null)
			{
				return resource;
			}
			int count2 = list.Count;
			for (int j = 0; j < count2; j++)
			{
				Resource resource3 = list[j];
				if (resource3 == null)
				{
					throw new Exception("Resource is null");
				}
				if ((flag || resource3.getLocation() == Location.Interior) && resource3.isUsable() && (emergency || resource3.getPotentialUserCount(character) == 0))
				{
					float sqrMagnitude = (position - resource3.getPosition()).sqrMagnitude;
					if (sqrMagnitude < num && resource3.calculateReachable())
					{
						resource = resource3;
						num = sqrMagnitude;
					}
				}
			}
			return resource;
		}

		// Token: 0x06000FB5 RID: 4021 RVA: 0x00058D6C File Offset: 0x00056F6C
		public static void destroyAll()
		{
			if (Resource.mParentObject != null)
			{
				Object.Destroy(Resource.mParentObject);
				Resource.mParentObject = null;
			}
			int count = Resource.mResources.Count;
			for (int i = 0; i < count; i++)
			{
				Resource.mResources[i].end();
			}
			Resource.mTotalAmounts.clear();
			Resource.mResourceDictionary.Clear();
			Resource.mResources.Clear();
			Resource.mPendingDestruction.Clear();
			Resource.mTypeResources.Clear();
			Resource.mInmaterialResources.clear();
		}

		// Token: 0x06000FB6 RID: 4022 RVA: 0x00058DF9 File Offset: 0x00056FF9
		public static Resource find(GameObject gameObject)
		{
			if (Resource.mResourceDictionary.ContainsKey(gameObject))
			{
				return Resource.mResourceDictionary[gameObject];
			}
			return null;
		}

		// Token: 0x06000FB7 RID: 4023 RVA: 0x00058E18 File Offset: 0x00057018
		public static void serializeAll(XmlNode rootNode, string name)
		{
			XmlNode xmlNode = Serialization.createNode(rootNode, name, null);
			int count = Resource.mResources.Count;
			for (int i = 0; i < count; i++)
			{
				Resource resource = Resource.mResources[i];
				if (!resource.isDestroyed() && resource.isIndependent())
				{
					resource.serialize(xmlNode, "resource");
				}
			}
			Resource.mInmaterialResources.serialize(xmlNode, "inmaterial-resources");
		}

		// Token: 0x06000FB8 RID: 4024 RVA: 0x00058E80 File Offset: 0x00057080
		public static void deserializeAll(XmlNode node)
		{
			foreach (object obj in node.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Name == "resource")
				{
					Resource.create(xmlNode);
				}
			}
			Resource.mInmaterialResources.deserialize(node["inmaterial-resources"]);
			Resource.mTotalAmounts.add(Resource.mInmaterialResources);
		}

		// Token: 0x06000FB9 RID: 4025 RVA: 0x00058F10 File Offset: 0x00057110
		public static ResourceAmounts calculateTradeableCounts()
		{
			ResourceAmounts resourceAmounts = new ResourceAmounts(null);
			int count = Resource.mResources.Count;
			for (int i = 0; i < count; i++)
			{
				Resource resource = Resource.mResources[i];
				if (resource.canBeTraded())
				{
					resourceAmounts.add(resource.getResourceType(), 1);
				}
			}
			resourceAmounts.add(Resource.mInmaterialResources);
			return resourceAmounts;
		}

		// Token: 0x06000FBA RID: 4026 RVA: 0x00058F68 File Offset: 0x00057168
		public static ResourceAmounts calculateUsableCounts()
		{
			ResourceAmounts resourceAmounts = new ResourceAmounts(null);
			int[] array = new int[TypeList<ResourceType, ResourceTypeList>.getCount()];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = 0;
			}
			int count = Resource.mResources.Count;
			for (int j = 0; j < count; j++)
			{
				Resource resource = Resource.mResources[j];
				ResourceType resourceType = resource.getResourceType();
				if (!Resource.isBusy(resource) && !resource.isTraded() && !resource.isDestroyed())
				{
					array[resourceType.getTypeIndex()]++;
				}
			}
			for (int k = 0; k < array.Length; k++)
			{
				resourceAmounts.add(TypeList<ResourceType, ResourceTypeList>.get()[k], array[k]);
			}
			resourceAmounts.add(Resource.mInmaterialResources);
			return resourceAmounts;
		}

		// Token: 0x06000FBB RID: 4027 RVA: 0x0005902C File Offset: 0x0005722C
		private static bool isBusy(Resource resource)
		{
			return resource.hasFlag(128) && resource.mState == Resource.State.Busy;
		}

		// Token: 0x06000FBC RID: 4028 RVA: 0x00059046 File Offset: 0x00057246
		public static ResourceAmounts getTotalAmounts()
		{
			return Resource.mTotalAmounts;
		}

		// Token: 0x06000FBD RID: 4029 RVA: 0x0005904D File Offset: 0x0005724D
		public static int getCountOfType(ResourceType resourceType)
		{
			return Resource.mTotalAmounts.getAmount(resourceType);
		}

		// Token: 0x06000FBE RID: 4030 RVA: 0x0005905C File Offset: 0x0005725C
		public static Resource find(int id)
		{
			int count = Resource.mResources.Count;
			for (int i = 0; i < count; i++)
			{
				Resource resource = Resource.mResources[i];
				if (resource.getId() == id)
				{
					return resource;
				}
			}
			return null;
		}

		// Token: 0x06000FBF RID: 4031 RVA: 0x00059098 File Offset: 0x00057298
		public static Resource findNearest(Vector3 position, Resource excludeResource)
		{
			int count = Resource.mResources.Count;
			float num = float.MaxValue;
			Resource resource = null;
			for (int i = 0; i < count; i++)
			{
				Resource resource2 = Resource.mResources[i];
				if (resource2 != excludeResource)
				{
					float sqrMagnitude = (resource2.getPosition() - position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						num = sqrMagnitude;
						resource = resource2;
					}
				}
			}
			return resource;
		}

		// Token: 0x06000FC0 RID: 4032 RVA: 0x000590FC File Offset: 0x000572FC
		public static bool markForTrading(ResourceType resourceType, MerchantShip trader)
		{
			int id = trader.getId();
			Vector3 position = trader.getPosition();
			if (resourceType.isInmaterial())
			{
				if (Resource.mInmaterialResources.getAmount(resourceType) > 0)
				{
					Resource.mInmaterialResources.remove(resourceType, 1);
					Resource.mTotalAmounts.remove(resourceType, 1);
					return true;
				}
			}
			else
			{
				int count = Resource.mResources.Count;
				float num = float.MaxValue;
				Resource resource = null;
				for (int i = 0; i < count; i++)
				{
					Resource resource2 = Resource.mResources[i];
					if (resource2.getResourceType() == resourceType && resource2.canBeTraded())
					{
						float sqrMagnitude = (resource2.getPosition() - position).sqrMagnitude;
						if (sqrMagnitude < num)
						{
							resource = resource2;
							num = sqrMagnitude;
						}
					}
				}
				if (resource != null)
				{
					resource.mTraderId = id;
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000FC1 RID: 4033 RVA: 0x000591C4 File Offset: 0x000573C4
		public static void unmarkForTrading(int traderId)
		{
			int count = Resource.mResources.Count;
			for (int i = 0; i < count; i++)
			{
				Resource resource = Resource.mResources[i];
				if (resource.mTraderId == traderId)
				{
					resource.mTraderId = -1;
				}
			}
		}

		// Token: 0x06000FC2 RID: 4034 RVA: 0x00059204 File Offset: 0x00057404
		public static void unmarkForTrading(int traderId, ResourceType resourceType)
		{
			if (resourceType.isInmaterial())
			{
				Resource.mInmaterialResources.add(resourceType, 1);
				Resource.mTotalAmounts.add(resourceType, 1);
				return;
			}
			int count = Resource.mResources.Count;
			for (int i = 0; i < count; i++)
			{
				Resource resource = Resource.mResources[i];
				if (resource.getResourceType() == resourceType && resource.mTraderId == traderId)
				{
					resource.mTraderId = -1;
					return;
				}
			}
			Debug.LogWarning("Could not unmark resource for trading: " + ((resourceType != null) ? resourceType.ToString() : null));
		}

		// Token: 0x06000FC3 RID: 4035 RVA: 0x0005928C File Offset: 0x0005748C
		public static Resource findTraded(Character character)
		{
			Resource resource = null;
			float num = float.MaxValue;
			Vector3 position = character.getPosition();
			int count = Resource.mResources.Count;
			for (int i = 0; i < count; i++)
			{
				Resource resource2 = Resource.mResources[i];
				if (resource2.isTraded() && (!resource2.anyTargeters() || resource2.isTargeter(character)) && resource2.getState() != Resource.State.Loaded)
				{
					float num2 = (position - resource2.getPosition()).sqrMagnitude;
					if (character.getLocation() == resource2.getLocation())
					{
						num2 -= 50f;
					}
					if (num2 < num)
					{
						num = num2;
						resource = resource2;
					}
				}
			}
			return resource;
		}

		// Token: 0x06000FC4 RID: 4036 RVA: 0x00059334 File Offset: 0x00057534
		public static Resource findTraded()
		{
			int count = Resource.mResources.Count;
			for (int i = 0; i < count; i++)
			{
				Resource resource = Resource.mResources[i];
				if (resource.isTraded())
				{
					return resource;
				}
			}
			return null;
		}

		// Token: 0x06000FC5 RID: 4037 RVA: 0x0005936F File Offset: 0x0005756F
		public static void addInmaterialResource(ResourceAmount amount)
		{
			Resource.mInmaterialResources.add(amount);
			Resource.mTotalAmounts.add(amount);
		}

		// Token: 0x06000FC6 RID: 4038 RVA: 0x00059387 File Offset: 0x00057587
		public static bool removeInmaterialResource(ResourceAmount amount)
		{
			if (Resource.mInmaterialResources.getAmount(amount.getResourceType()) >= amount.getAmount())
			{
				Resource.mInmaterialResources.remove(amount);
				Resource.mTotalAmounts.remove(amount);
				return true;
			}
			return false;
		}

		// Token: 0x06000FC7 RID: 4039 RVA: 0x000593BA File Offset: 0x000575BA
		public static int getCount()
		{
			return Resource.mResourceDictionary.Count;
		}

		// Token: 0x06000FC8 RID: 4040 RVA: 0x000593C8 File Offset: 0x000575C8
		public static void setResourcesLocation(Collider collider, Location location)
		{
			PerformanceTimer performanceTimer = new PerformanceTimer();
			int count = Resource.mResources.Count;
			for (int i = 0; i < count; i++)
			{
				Resource resource = Resource.mResources[i];
				if (!resource.isEmbedded() && (resource.getState() == Resource.State.Idle || resource.getState() == Resource.State.Stored) && resource.isOnTopOf(collider))
				{
					resource.setLocation(location);
				}
			}
			performanceTimer.check("Setting resources location collider", 1);
		}

		// Token: 0x06000FC9 RID: 4041 RVA: 0x00059434 File Offset: 0x00057634
		public static void setResourcesLocation(Vector3 position, float radius, Location location)
		{
			PerformanceTimer performanceTimer = new PerformanceTimer();
			float num = radius * radius;
			int count = Resource.mResources.Count;
			for (int i = 0; i < count; i++)
			{
				Resource resource = Resource.mResources[i];
				if (resource.getLocation() != location && !resource.isEmbedded() && (resource.getState() == Resource.State.Idle || resource.getState() == Resource.State.Stored) && (position - resource.getPosition()).sqrMagnitude < num)
				{
					resource.setLocation(location);
				}
			}
			performanceTimer.check("Setting resources location radius", 1);
		}

		// Token: 0x06000FCA RID: 4042 RVA: 0x000594C4 File Offset: 0x000576C4
		public static int getHighestCountResourceIndex(List<ResourceType> resourceTypes)
		{
			int num = 0;
			int num2 = 0;
			ResourceAmounts totalAmounts = Resource.getTotalAmounts();
			for (int i = 0; i < resourceTypes.Count; i++)
			{
				int amount = totalAmounts.getAmount(resourceTypes[i]);
				if (amount > num)
				{
					num2 = i;
					num = amount;
				}
			}
			return num2;
		}

		// Token: 0x04000C21 RID: 3105
		private string mSubtitle;

		// Token: 0x04000C22 RID: 3106
		private ResourceContainer mContainer;

		// Token: 0x04000C23 RID: 3107
		private ResourceSubtype mSubtype;

		// Token: 0x04000C24 RID: 3108
		private Resource.State mState;

		// Token: 0x04000C25 RID: 3109
		private ResourceType mResourceType;

		// Token: 0x04000C26 RID: 3110
		private GameObject mObject;

		// Token: 0x04000C27 RID: 3111
		private GameObject mModel;

		// Token: 0x04000C28 RID: 3112
		private Location mLocation;

		// Token: 0x04000C29 RID: 3113
		private int mId;

		// Token: 0x04000C2A RID: 3114
		private StorageSlot mStorageSlot;

		// Token: 0x04000C2B RID: 3115
		private int mTraderId = -1;

		// Token: 0x04000C2C RID: 3116
		private Collider mCollider;

		// Token: 0x04000C2D RID: 3117
		private Indicator mConditionIndicator;

		// Token: 0x04000C2E RID: 3118
		private Resource.Durability mDurability;

		// Token: 0x04000C2F RID: 3119
		private Indicator[] mIndicators;

		// Token: 0x04000C30 RID: 3120
		private static List<Resource> mPendingDestruction = new List<Resource>();

		// Token: 0x04000C31 RID: 3121
		private static List<Resource> mResources = new List<Resource>();

		// Token: 0x04000C32 RID: 3122
		private static Dictionary<ResourceType, List<Resource>> mTypeResources = new Dictionary<ResourceType, List<Resource>>();

		// Token: 0x04000C33 RID: 3123
		private static ResourceAmounts mTotalAmounts = new ResourceAmounts(null);

		// Token: 0x04000C34 RID: 3124
		private static GameObject mParentObject = null;

		// Token: 0x04000C35 RID: 3125
		private static ResourceAmounts mInmaterialResources = new ResourceAmounts(null);

		// Token: 0x04000C36 RID: 3126
		private static Dictionary<GameObject, Resource> mResourceDictionary = new Dictionary<GameObject, Resource>();

		// Token: 0x020002C9 RID: 713
		public enum State
		{
			// Token: 0x04000E7D RID: 3709
			Idle,
			// Token: 0x04000E7E RID: 3710
			Loaded,
			// Token: 0x04000E7F RID: 3711
			Stored,
			// Token: 0x04000E80 RID: 3712
			Busy
		}

		// Token: 0x020002CA RID: 714
		public enum Durability
		{
			// Token: 0x04000E82 RID: 3714
			Normal,
			// Token: 0x04000E83 RID: 3715
			High
		}
	}
}
