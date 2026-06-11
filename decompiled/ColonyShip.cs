using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Planetbase
{
	// Token: 0x02000252 RID: 594
	public class ColonyShip : Ship
	{
		// Token: 0x060010F8 RID: 4344 RVA: 0x0005CBD0 File Offset: 0x0005ADD0
		public static ColonyShip create(Vector3 position, Vector3 targetPosition, ResourceAmounts resourceAmounts)
		{
			ColonyShip colonyShip = new ColonyShip();
			colonyShip.init(position, targetPosition, resourceAmounts);
			colonyShip.postInit();
			return colonyShip;
		}

		// Token: 0x060010F9 RID: 4345 RVA: 0x0005CBE6 File Offset: 0x0005ADE6
		public static ColonyShip create(XmlNode node)
		{
			ColonyShip colonyShip = new ColonyShip();
			colonyShip.deserialize(node);
			colonyShip.postInit();
			return colonyShip;
		}

		// Token: 0x060010FA RID: 4346 RVA: 0x0005CBFC File Offset: 0x0005ADFC
		protected ColonyShip()
		{
			this.mModel = Object.Instantiate<GameObject>(ResourceList.getInstance().Ships.Colony);
			this.mModel.transform.parent = this.mObject.transform;
			this.mModel.name = "Ship Model";
			this.mModel.layer = 16;
			this.mResourceContainer = new ResourceContainer(this);
			this.mEngineParticles = new List<ParticleSystemData>();
		}

		// Token: 0x060010FB RID: 4347 RVA: 0x0005CC78 File Offset: 0x0005AE78
		public static void addColonyEngineParticles(GameObject model, List<ParticleSystemData> particleList)
		{
			GameObject gameObject = model.findTaggedObject("ShipColonyEngineParticles");
			if (gameObject != null)
			{
				ParticleSystemData particleSystemData = Singleton<ParticleManager>.getInstance().create(ResourceList.getInstance().Particles.ColonyEngine);
				GameObject gameObject2 = particleSystemData.getGameObject();
				gameObject2.transform.position -= gameObject2.transform.forward;
				gameObject2.transform.SetParent(gameObject.transform, false);
				if (particleList != null)
				{
					particleSystemData.setEmissionEnabled(false);
					particleList.Add(particleSystemData);
				}
			}
			List<GameObject> list = model.findTaggedObjects("ShipEngineParticles");
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				if (list[i].GetComponentInChildren<ParticleSystem>())
				{
					Debug.LogWarning(list[i].name + " already has a particle system on it, you need to delete it from the prefab");
				}
				ParticleSystemData particleSystemData2 = Singleton<ParticleManager>.getInstance().create(ResourceList.getInstance().Particles.ColonyEngineSmall);
				GameObject gameObject3 = particleSystemData2.getGameObject();
				gameObject3.transform.position -= gameObject3.transform.forward;
				gameObject3.transform.SetParent(list[i].transform, false);
				if (particleList != null)
				{
					particleSystemData2.setEmissionEnabled(false);
					particleList.Add(particleSystemData2);
				}
			}
		}

		// Token: 0x060010FC RID: 4348 RVA: 0x0005CDD0 File Offset: 0x0005AFD0
		protected void init(Vector3 position, Vector3 targetPosition, ResourceAmounts resourceAmounts)
		{
			this.mResourceContainer.setCapacity(Mathf.Max(resourceAmounts.getTotalAmount(), 150));
			foreach (ResourceAmount resourceAmount in resourceAmounts)
			{
				this.addResource(resourceAmount.getResourceType(), resourceAmount.getAmount());
			}
			Vector3 vector;
			if (PhysicsUtil.findFloor(position, out vector, 256))
			{
				targetPosition = vector;
			}
			else
			{
				string text = "Could not hit terrain trying to place ColonyShip: ";
				Vector3 vector2 = position;
				Debug.LogWarning(text + vector2.ToString());
			}
			this.mObject.transform.position = position;
			this.mTargetPosition = targetPosition;
			this.updateRotation();
			ColonyShip.addColonyEngineParticles(this.mModel, this.mEngineParticles);
		}

		// Token: 0x060010FD RID: 4349 RVA: 0x0005CEA0 File Offset: 0x0005B0A0
		private void setBlockedNodes(bool blocked)
		{
			NavigationGraph exterior = NavigationGraph.getExterior();
			float num = this.getRadius() * 2f;
			if (blocked)
			{
				exterior.addBlocker(this.mTargetPosition, num);
				NavigationNode navigationNode = exterior.findNearestNode(this.mTargetPosition + base.getDirection() * 8f + Vector3.right * 4f, 0, null);
				if (navigationNode != null)
				{
					navigationNode.addBlocker();
				}
				NavigationNode navigationNode2 = exterior.findNearestNode(this.mTargetPosition + base.getDirection() * 8f - Vector3.right * 4f, 0, null);
				if (navigationNode2 != null)
				{
					navigationNode2.addBlocker();
				}
				exterior.removeBlocker(this.mTargetPosition, this.getRadius() * 0.25f);
				exterior.removeBlocker(this.mTargetPosition + base.getDirection() * 4f, 2f);
				exterior.removeBlocker(this.mTargetPosition + base.getDirection() * 8f, 2f);
				return;
			}
			exterior.removeBlocker(this.mTargetPosition, num * 1.25f);
		}

		// Token: 0x060010FE RID: 4350 RVA: 0x0005CFD0 File Offset: 0x0005B1D0
		protected void postInit()
		{
			Singleton<TerrainGenerator>.getInstance().clearArea(this.mTargetPosition, 20f);
			this.setBlockedNodes(true);
			this.offsetNodes(1f);
			this.mObject.setLayerRecursive(16);
			if (this.mState == ColonyShip.State.Landing)
			{
				CameraManager.getInstance().setCinematic(new IntroCinemetic(this));
			}
			else
			{
				this.mModel.playDefaultAnimationLastFrame();
			}
			this.mIcon = ResourceUtil.loadIconColor("Ships/icon_ship_colony");
		}

		// Token: 0x060010FF RID: 4351 RVA: 0x0005D048 File Offset: 0x0005B248
		public void offsetNodes(float multiplier)
		{
			float num = this.getRadius() * 1.5f;
			float num2 = 2.6f * multiplier;
			List<NavigationNode> list = NavigationGraph.getExterior().findNearestNodes(this.mTargetPosition, num);
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				NavigationNode navigationNode = list[i];
				Vector3 position = navigationNode.getPosition();
				(navigationNode.getPosition() - this.mTargetPosition).y = 0f;
				navigationNode.setPosition(position + Vector3.up * num2);
			}
		}

		// Token: 0x06001100 RID: 4352 RVA: 0x0005D0DC File Offset: 0x0005B2DC
		private void addResource(ResourceType type, int amount)
		{
			if (type.isInmaterial())
			{
				Resource.addInmaterialResource(new ResourceAmount(type, amount));
				return;
			}
			for (int i = 0; i < amount; i++)
			{
				ResourceSubtype resourceSubtype = ((type == ResourceTypeList.MealInstance) ? ResourceSubtype.Pasta : ResourceSubtype.None);
				Resource resource = Resource.create(type, resourceSubtype, this.getPosition(), Location.Exterior);
				resource.onEmbed(this.mResourceContainer);
				resource.setDurability(Resource.Durability.High);
				this.mResourceContainer.add(resource);
			}
		}

		// Token: 0x06001101 RID: 4353 RVA: 0x0005D148 File Offset: 0x0005B348
		public override void recycle()
		{
			foreach (Resource resource in this.mResourceContainer.getResources())
			{
				resource.onExtract();
				resource.setPosition(this.getPosition() + MathUtil.randFlatVector(5f).Rounded() * 1.2f);
				resource.drop(Resource.State.Idle);
			}
			ResourceType resourceType = TypeList<ResourceType, ResourceTypeList>.find<Metal>();
			for (int i = 0; i < 15; i++)
			{
				Resource resource2 = Resource.create(resourceType, this.getPosition() + MathUtil.randFlatVector(5f).Rounded() * 1.2f, Location.Exterior);
				resource2.drop(Resource.State.Idle);
				resource2.setDurability(Resource.Durability.High);
			}
			ResourceType resourceType2 = TypeList<ResourceType, ResourceTypeList>.find<Bioplastic>();
			for (int j = 0; j < 10; j++)
			{
				Resource resource3 = Resource.create(resourceType2, this.getPosition() + MathUtil.randFlatVector(5f).Rounded() * 1.2f, Location.Exterior);
				resource3.drop(Resource.State.Idle);
				resource3.setDurability(Resource.Durability.High);
			}
			this.offsetNodes(-1f);
			this.setBlockedNodes(false);
		}

		// Token: 0x06001102 RID: 4354 RVA: 0x0005D27C File Offset: 0x0005B47C
		public override float getRadius()
		{
			return 5f;
		}

		// Token: 0x06001103 RID: 4355 RVA: 0x0005D284 File Offset: 0x0005B484
		public override void update(float timeStep)
		{
			this.mStateTimer += timeStep;
			switch (this.mState)
			{
			case ColonyShip.State.Landing:
			{
				if (this.mEngineParticles.Count > 0 && !this.mEngineParticles[0].isEmissionEnabled())
				{
					this.setEnginesActive(true);
				}
				float num = timeStep * 0.5f;
				float magnitude = (this.getPosition() - this.mTargetPosition).magnitude;
				if (this.mDustParticles == null && magnitude < 10f)
				{
					GameObject colonyShipDust = PlanetManager.getCurrentPlanet().getDefinition().ColonyShipDust;
					if (colonyShipDust != null)
					{
						this.mDustParticles = Singleton<ParticleManager>.getInstance().create(colonyShipDust);
						this.mDustParticles.getGameObject().transform.position = this.mTargetPosition;
						this.mOriginalEmissionRate = this.mDustParticles.getEmissionRate();
					}
				}
				if (this.mDustParticles != null && magnitude < 2f)
				{
					this.mDustParticles.setEmissionRate(this.mOriginalEmissionRate * magnitude * 0.5f);
				}
				ref Vector3 position = this.getPosition();
				float floorHeight = Singleton<TerrainGenerator>.getInstance().getFloorHeight();
				if (position.y < 5f + floorHeight)
				{
					this.mEngineParticles[0].getGameObject().transform.position += Vector3.up * timeStep * 1.5f;
				}
				if (magnitude > num)
				{
					Vector3 normalized = (this.mTargetPosition - this.getPosition()).normalized;
					this.mObject.transform.position += this.getSpeed() * timeStep * normalized;
					this.updateRotation();
					return;
				}
				this.setState(ColonyShip.State.Landed);
				this.onLanded();
				return;
			}
			case ColonyShip.State.Landed:
				if (this.mStateTimer > 2f)
				{
					this.setState(ColonyShip.State.OpenDoor);
					this.onOpenDoor();
					return;
				}
				break;
			case ColonyShip.State.OpenDoor:
				if (this.mStateTimer > 2f)
				{
					this.setState(ColonyShip.State.Spawn);
					return;
				}
				break;
			case ColonyShip.State.Spawn:
			{
				Planet currentPlanet = PlanetManager.getCurrentPlanet();
				if (this.mSpawnTimer <= 0f)
				{
					this.updateSpawnInitialCharacters(currentPlanet, this.getPosition() + base.getDirection() * 20f);
					this.mSpawnTimer = 1f + Random.Range(0f, 0.5f);
				}
				else
				{
					this.mSpawnTimer -= timeStep;
				}
				if (this.mSpawnIndex >= this.mSpawnList.Count && this.mSpawnTimer < 0.5f)
				{
					this.setState(ColonyShip.State.AfterSpawn);
					this.onSpawnDone();
					return;
				}
				break;
			}
			case ColonyShip.State.AfterSpawn:
				if (this.mStateTimer > 2f)
				{
					this.setState(ColonyShip.State.Done);
					this.onDone();
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06001104 RID: 4356 RVA: 0x0005D537 File Offset: 0x0005B737
		private void setState(ColonyShip.State state)
		{
			this.mState = state;
			this.mStateTimer = 0f;
		}

		// Token: 0x06001105 RID: 4357 RVA: 0x0005D54C File Offset: 0x0005B74C
		private void onLanded()
		{
			Singleton<AudioPlayer>.getInstance().play(SoundList.getInstance().ShipAirRelease, this);
			this.mObject.transform.position = this.mTargetPosition;
			this.stopParticles();
			this.mSpawnList = new List<Specialization>();
			List<SpecializationCount> startingSpecializations = PlanetManager.getCurrentPlanet().getStartingSpecializations();
			for (int i = 0; i < startingSpecializations.Count; i++)
			{
				SpecializationCount specializationCount = startingSpecializations[i];
				for (int j = 0; j < specializationCount.getCount(); j++)
				{
					this.mSpawnList.Add(specializationCount.getSpecialization());
				}
			}
		}

		// Token: 0x06001106 RID: 4358 RVA: 0x0005D5DC File Offset: 0x0005B7DC
		private void stopParticles()
		{
			Singleton<ParticleManager>.getInstance().stop(this.mDustParticles);
			Singleton<ParticleManager>.getInstance().stop(this.mEngineParticles);
			this.mDustParticles = null;
			this.mEngineParticles.Clear();
		}

		// Token: 0x06001107 RID: 4359 RVA: 0x0005D610 File Offset: 0x0005B810
		private void onOpenDoor()
		{
			Singleton<AudioPlayer>.getInstance().play(SoundList.getInstance().ShipDoor, this);
			this.mModel.playDefaultAnimation(0.2f);
		}

		// Token: 0x06001108 RID: 4360 RVA: 0x0005D638 File Offset: 0x0005B838
		private void onSpawnDone()
		{
			NavigationGraph.requestUpdateCache();
		}

		// Token: 0x06001109 RID: 4361 RVA: 0x0005D63F File Offset: 0x0005B83F
		private void onDone()
		{
		}

		// Token: 0x0600110A RID: 4362 RVA: 0x0005D644 File Offset: 0x0005B844
		private void updateRotation()
		{
			float magnitude = (this.getPosition() - this.mTargetPosition).magnitude;
			Vector3 vector = new Vector3(Mathf.Sin(magnitude * 0.05f), Mathf.Sin(magnitude * 0.03f), Mathf.Sin(magnitude * 0.02f)) * 2f;
			vector.y += PlanetManager.getCurrentPlanet().getColonyShipRotation();
			this.mObject.transform.rotation = Quaternion.Euler(vector);
		}

		// Token: 0x0600110B RID: 4363 RVA: 0x0005D6CC File Offset: 0x0005B8CC
		private float getSpeed()
		{
			float magnitude = (this.getPosition() - this.mTargetPosition).magnitude;
			return 10f * Mathf.Clamp(magnitude * 0.055f, 0.05f, 20f);
		}

		// Token: 0x0600110C RID: 4364 RVA: 0x0005D710 File Offset: 0x0005B910
		protected override void serialize(XmlNode parent, string name)
		{
			base.serialize(parent, name);
			XmlNode lastChild = parent.LastChild;
			Serialization.serializeVector3(lastChild, "target-position", this.mTargetPosition);
			Serialization.serializeInt(lastChild, "state", (int)this.mState);
			if (this.mResourceContainer != null)
			{
				this.mResourceContainer.serialize(lastChild, "resource-container");
			}
		}

		// Token: 0x0600110D RID: 4365 RVA: 0x0005D768 File Offset: 0x0005B968
		protected override void deserialize(XmlNode node)
		{
			base.deserialize(node);
			this.mTargetPosition = Serialization.deserializeVector3(node["target-position"]);
			this.mState = (ColonyShip.State)Serialization.deserializeInt(node["state"]);
			this.mResourceContainer.deserialize(node["resource-container"]);
		}

		// Token: 0x0600110E RID: 4366 RVA: 0x0005D7BE File Offset: 0x0005B9BE
		public override ResourceContainer getResourceContainer()
		{
			return this.mResourceContainer;
		}

		// Token: 0x0600110F RID: 4367 RVA: 0x0005D7C8 File Offset: 0x0005B9C8
		private void updateSpawnInitialCharacters(Planet planet, Vector3 startPosition)
		{
			Specialization specialization = this.mSpawnList[this.mSpawnIndex];
			Vector3 vector = this.getPosition() + Vector3.up * 5f + MathUtil.randFlatVector(0.5f);
			int num = this.mSpawnList.Count - this.mSpawnIndex - 1;
			int num2 = ((this.mSpawnList.Count >= 10) ? 5 : 3);
			Vector3 vector2 = base.getTransform().right * (float)(num % num2 - num2 / 2) * 4f;
			vector2 += base.getDirection() * (float)(num / num2 - 1) * 4f;
			vector2 += MathUtil.randFlatVector(1f);
			Vector3 vector3;
			PhysicsUtil.findFloor(startPosition + vector2, out vector3, 256);
			Character.create(specialization, vector, Location.Exterior).startWalking(new Target(vector3, Location.Exterior), null);
			this.mSpawnIndex++;
		}

		// Token: 0x06001110 RID: 4368 RVA: 0x0005D8CB File Offset: 0x0005BACB
		public override bool isDeleteable(out bool buttonEnabled)
		{
			if (Singleton<ChallengeManager>.getInstance().isGameplayModifierActive(GameplayModifierType.DisableColonyShipRecycling))
			{
				buttonEnabled = false;
				return false;
			}
			buttonEnabled = true;
			return true;
		}

		// Token: 0x06001111 RID: 4369 RVA: 0x0005D8E4 File Offset: 0x0005BAE4
		public override string getName()
		{
			return StringList.get("colony_ship");
		}

		// Token: 0x06001112 RID: 4370 RVA: 0x0005D8F0 File Offset: 0x0005BAF0
		private void setEnginesActive(bool active)
		{
			if (active)
			{
				this.mAudioSource.play(SoundList.getInstance().ShipEngineLanding, false, Singleton<Profile>.getInstance().getSfxVolumeNormalized(), -1);
			}
			else
			{
				this.mAudioSource.Stop();
			}
			int count = this.mEngineParticles.Count;
			for (int i = 0; i < count; i++)
			{
				this.mEngineParticles[i].setEmissionEnabled(active);
			}
		}

		// Token: 0x06001113 RID: 4371 RVA: 0x0005D958 File Offset: 0x0005BB58
		public override GameObject getSelectionModel()
		{
			return this.mModel;
		}

		// Token: 0x06001114 RID: 4372 RVA: 0x0005D960 File Offset: 0x0005BB60
		public bool isDone()
		{
			return this.mState == ColonyShip.State.Done;
		}

		// Token: 0x04000CA0 RID: 3232
		private Vector3 mTargetPosition;

		// Token: 0x04000CA1 RID: 3233
		private ResourceContainer mResourceContainer;

		// Token: 0x04000CA2 RID: 3234
		private float mStateTimer;

		// Token: 0x04000CA3 RID: 3235
		private ColonyShip.State mState;

		// Token: 0x04000CA4 RID: 3236
		private GameObject mModel;

		// Token: 0x04000CA5 RID: 3237
		private List<ParticleSystemData> mEngineParticles;

		// Token: 0x04000CA6 RID: 3238
		private ParticleSystemData mDustParticles;

		// Token: 0x04000CA7 RID: 3239
		private List<Specialization> mSpawnList;

		// Token: 0x04000CA8 RID: 3240
		private float mSpawnTimer;

		// Token: 0x04000CA9 RID: 3241
		private int mSpawnIndex;

		// Token: 0x04000CAA RID: 3242
		private float mOriginalEmissionRate;

		// Token: 0x04000CAB RID: 3243
		private const float LandingClearRadius = 20f;

		// Token: 0x04000CAC RID: 3244
		private const float Speed = 10f;

		// Token: 0x04000CAD RID: 3245
		private const float WaitBeforeOpeningDoor = 2f;

		// Token: 0x04000CAE RID: 3246
		private const float WaitBeforeSpawning = 2f;

		// Token: 0x04000CAF RID: 3247
		private const float WaitBeforeCameraReset = 2f;

		// Token: 0x020002CC RID: 716
		private enum State
		{
			// Token: 0x04000E85 RID: 3717
			Landing,
			// Token: 0x04000E86 RID: 3718
			Landed,
			// Token: 0x04000E87 RID: 3719
			OpenDoor,
			// Token: 0x04000E88 RID: 3720
			Spawn,
			// Token: 0x04000E89 RID: 3721
			AfterSpawn,
			// Token: 0x04000E8A RID: 3722
			Done
		}
	}
}
