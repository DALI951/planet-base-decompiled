using System;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Planetbase
{
	// Token: 0x0200009F RID: 159
	public class CameraManager
	{
		// Token: 0x060002CD RID: 717 RVA: 0x00013D58 File Offset: 0x00011F58
		public CameraManager()
		{
			CameraManager.mInstance = this;
			this.mMainCamera = this.createMainCamera();
			this.mSkydomeCamera = new GameObject();
			this.mSkydomeCamera.name = "Skydome Camera";
			Camera camera = this.mSkydomeCamera.AddComponent<Camera>();
			camera.nearClipPlane = 0.5f;
			camera.farClipPlane = 1000000f;
			camera.clearFlags = CameraClearFlags.Nothing;
			camera.depth = -100f;
			camera.cullingMask = 262144;
			camera.renderingPath = RenderingPath.DeferredShading;
			CameraManager.DefaultCameraTransform.apply(this.mMainCamera.transform);
			CameraManager.DefaultCameraTransform.apply(this.mSkydomeCamera.transform);
			this.initCamera();
		}

		// Token: 0x060002CE RID: 718 RVA: 0x00013E44 File Offset: 0x00012044
		private GameObject createMainCamera()
		{
			int qualityLevel = QualitySettings.GetQualityLevel();
			GameObject gameObject = ResourceList.getInstance().QualityCameras[qualityLevel];
			GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject);
			gameObject2.name = "Main Camera (" + gameObject.name.Replace("PrefabCamera", "") + ")";
			return gameObject2;
		}

		// Token: 0x060002CF RID: 719 RVA: 0x00013E94 File Offset: 0x00012094
		public void onQualityChanged()
		{
			GameObject gameObject = this.createMainCamera();
			gameObject.transform.position = this.mMainCamera.transform.position;
			gameObject.transform.rotation = this.mMainCamera.transform.rotation;
			Object.Destroy(this.mMainCamera);
			this.mMainCamera = gameObject;
			this.initCamera();
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x00013EF8 File Offset: 0x000120F8
		private void initCamera()
		{
			this.mMainCameraComponent = this.mMainCamera.GetComponent<Camera>();
			this.mMainCameraComponent.farClipPlane = 4000f;
			this.mMainCameraComponent.nearClipPlane = 0.5f;
			this.mMainCameraComponent.clearFlags = CameraClearFlags.Skybox;
			this.mMainCameraComponent.allowHDR = false;
			this.mMainCameraComponent.useOcclusionCulling = true;
			this.mMainCameraComponent.renderingPath = RenderingPath.DeferredShading;
			float[] array = new float[32];
			float layerDistanceFactor = ExtraQualitySettings.getQualityLevelSettings().LayerDistanceFactor;
			array[10] = 150f * layerDistanceFactor;
			array[15] = 115f * layerDistanceFactor;
			array[13] = 200f * layerDistanceFactor;
			array[14] = 110f * layerDistanceFactor;
			array[17] = 250f * layerDistanceFactor;
			array[19] = 500f * layerDistanceFactor;
			array[12] = 210f * layerDistanceFactor;
			this.mMainCameraComponent.layerCullDistances = array;
			this.mMainCameraComponent.layerCullSpherical = true;
			this.mMainCameraComponent.cullingMask = -2359297;
			this.mMainCameraComponent.useOcclusionCulling = false;
			this.mMainCamera.AddComponent<AudioListener>();
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x00014008 File Offset: 0x00012208
		public void onGameStart()
		{
			this.mMainCameraComponent.farClipPlane = 20000f;
			this.mMainCameraComponent.clearFlags = CameraClearFlags.Depth;
			this.mMainCameraComponent.fieldOfView = 60f;
			this.mSkydomeCamera.SetActive(true);
			this.mMainCamera.GetComponent<MadrugaScattering>().enabled = true;
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x0001405E File Offset: 0x0001225E
		public void onNewGame()
		{
			this.mCurrentHeight = 21f;
			this.mTargetHeight = this.mCurrentHeight;
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x00014078 File Offset: 0x00012278
		public void onTitleScene()
		{
			this.mMainCameraComponent.clearFlags = CameraClearFlags.Color;
			this.mMainCameraComponent.farClipPlane = 50000f;
			this.setCameraTransform(CameraManager.DefaultCameraTransform);
			this.mSkydomeCamera.SetActive(false);
			this.mCinematic = null;
			this.mLocked = false;
			this.mZoomLocked = false;
			this.mMainCamera.GetComponent<MadrugaScattering>().enabled = false;
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x000140DE File Offset: 0x000122DE
		public void onLogoStart()
		{
			this.onTitleScene();
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x000140E8 File Offset: 0x000122E8
		public void setAntiAliasingEnabled(bool enabled)
		{
			Antialiasing component = this.mMainCamera.GetComponent<Antialiasing>();
			if (component != null)
			{
				component.enabled = enabled;
			}
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x00014114 File Offset: 0x00012314
		public void fixedUpdate(float timeStep, int frameIndex)
		{
			if (this.mCinematic == null)
			{
				float num = timeStep * 6f;
				float num2 = timeStep * 10f;
				float num3 = timeStep * 10f;
				GameState gameState = GameManager.getInstance().getGameState();
				if (this.mTargetHeight != this.mCurrentHeight)
				{
					this.mCurrentHeight += Mathf.Sign(this.mTargetHeight - this.mCurrentHeight) * timeStep * 30f;
					if (Mathf.Abs(this.mCurrentHeight - this.mTargetHeight) < 0.5f)
					{
						this.mCurrentHeight = this.mTargetHeight;
					}
				}
				if (gameState != null && !gameState.isCameraFixed() && !this.mLocked && !Singleton<TimeManager>.getInstance().isPaused())
				{
					KeyBindingManager instance = Singleton<KeyBindingManager>.getInstance();
					this.mAcceleration.x = this.mAcceleration.x + instance.getCompositeAxis(ActionType.CameraMoveLeft, ActionType.CameraMoveRight) * num;
					this.mAcceleration.z = this.mAcceleration.z + instance.getCompositeAxis(ActionType.CameraMoveBack, ActionType.CameraMoveForward) * num;
					if (!this.mZoomLocked)
					{
						this.mAcceleration.y = this.mAcceleration.y - this.mZoomAxis * num2;
						this.mAcceleration.y = this.mAcceleration.y - instance.getCompositeAxis(ActionType.CameraZoomOut, ActionType.CameraZoomIn) * num2;
					}
					this.mRotationAcceleration += instance.getCompositeAxis(ActionType.CameraRotateLeft, ActionType.CameraRotateRight) * num3;
					float num4 = Input.mousePosition.x - this.mPreviousMouseX;
					if (Input.GetMouseButton(2) && Mathf.Abs(num4) > Mathf.Epsilon)
					{
						this.mRotationAcceleration += num3 * num4 * 0.1f;
					}
					if (!Application.isEditor)
					{
						float num5 = (float)Screen.height * 0.01f;
						if (Input.mousePosition.x < num5)
						{
							this.mAcceleration.x = this.mAcceleration.x - num;
						}
						else if (Input.mousePosition.x > (float)Screen.width - num5)
						{
							this.mAcceleration.x = this.mAcceleration.x + num;
						}
						if (Input.mousePosition.y < num5)
						{
							this.mAcceleration.z = this.mAcceleration.z - num;
						}
						else if (Input.mousePosition.y > (float)Screen.height - num5)
						{
							this.mAcceleration.z = this.mAcceleration.z + num;
						}
					}
					float num6 = ((Input.GetKey(KeyCode.LeftShift) || this.mCapsLock) ? 0.25f : 1f);
					this.mAcceleration.x = Mathf.Clamp(this.mAcceleration.x - this.mAcceleration.x * num, -num6, num6);
					this.mAcceleration.z = Mathf.Clamp(this.mAcceleration.z - this.mAcceleration.z * num, -num6, num6);
					this.mAcceleration.y = Mathf.Clamp(this.mAcceleration.y - this.mAcceleration.y * num2, -num6, num6);
					this.mRotationAcceleration = Mathf.Clamp(this.mRotationAcceleration - this.mRotationAcceleration * num3, -num6, num6);
				}
				else
				{
					this.mAcceleration = Vector3.zero;
					this.mRotationAcceleration = 0f;
				}
				this.mPreviousMouseX = Input.mousePosition.x;
			}
			else
			{
				this.mCinematic.fixedUpdate(timeStep);
			}
			this.mZoomAxis = 0f;
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x00014450 File Offset: 0x00012650
		public void update(float timeStep)
		{
			if (this.mZoomAxis == 0f)
			{
				this.mZoomAxis = Input.GetAxis("Zoom");
			}
			GameState gameState = GameManager.getInstance().getGameState();
			if (gameState != null && !gameState.isCameraFixed() && !this.mLocked)
			{
				if (this.mCinematic == null)
				{
					float x = this.mAcceleration.x;
					float num = this.mAcceleration.z;
					if (Mathf.Abs(this.mAcceleration.y) > 0.01f)
					{
						float num2 = Mathf.Clamp(60f * timeStep, 0.01f, 100f);
						float num3 = Mathf.Clamp(this.mCurrentHeight + this.mAcceleration.y * num2, 12f, 30f);
						num += (this.mCurrentHeight - num3) / num2;
						this.mCurrentHeight = num3;
						this.mTargetHeight = this.mCurrentHeight;
					}
					Transform transform = this.mMainCamera.transform;
					if (Mathf.Abs(num) > 0.01f && Mathf.Abs(num) > 0.001f)
					{
						transform.position += new Vector3(transform.forward.x, 0f, transform.forward.z) * num * timeStep * 80f;
					}
					if (Mathf.Abs(x) > 0.01f && Mathf.Abs(x) > 0.001f)
					{
						transform.position += new Vector3(transform.right.x, 0f, transform.right.z) * x * timeStep * 80f;
					}
					if (Mathf.Abs(this.mRotationAcceleration) > 0.01f)
					{
						transform.RotateAround(this.mMainCamera.transform.position, new Vector3(0f, 1f, 0f), this.mRotationAcceleration * timeStep * 120f);
					}
					Vector3 vector = new Vector3(2000f, 0f, 2000f) * 0.5f;
					Vector3 vector2 = this.mMainCamera.transform.position - vector;
					if (vector2.magnitude > 375f)
					{
						this.mMainCamera.transform.position = vector + vector2.normalized * 750f * 0.5f;
					}
					if (Mathf.Abs(num) > 0.001f || Mathf.Abs(x) > 0.001f || this.mZoomLocked)
					{
						this.placeOnFloor(this.mCurrentHeight);
						Vector3 eulerAngles = this.mMainCamera.transform.rotation.eulerAngles;
						eulerAngles.x = 25f;
						this.mMainCamera.transform.rotation = Quaternion.Euler(eulerAngles);
					}
				}
				else
				{
					this.updateCinematic(timeStep);
				}
			}
			if (this.mCameraTransition < 1f)
			{
				if (this.mTransitionTime == 0f)
				{
					this.mCameraTransition = 1f;
				}
				else
				{
					float num4 = timeStep / this.mTransitionTime;
					this.mCameraTransition += num4;
				}
				this.mCurrentTransform.interpolate(this.mSourceTransform, this.mTargetTransform, this.mCameraTransition);
				this.mCurrentTransform.apply(this.mMainCamera.transform);
			}
			this.mSkydomeCamera.transform.rotation = this.mMainCamera.transform.rotation;
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x000147D5 File Offset: 0x000129D5
		public bool isTransitioning()
		{
			return this.mCameraTransition < 1f;
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x000147E4 File Offset: 0x000129E4
		public void updateCinematic(float timeStep)
		{
			if (this.mCinematic != null)
			{
				this.mCinematic.update(timeStep);
			}
		}

		// Token: 0x060002DA RID: 730 RVA: 0x000147FA File Offset: 0x000129FA
		public void setCinematic(Cinematic cimenatic)
		{
			this.mCinematic = cimenatic;
		}

		// Token: 0x060002DB RID: 731 RVA: 0x00014803 File Offset: 0x00012A03
		public bool isCinematic()
		{
			return this.mCinematic != null;
		}

		// Token: 0x060002DC RID: 732 RVA: 0x0001480E File Offset: 0x00012A0E
		public Cinematic getCinematic()
		{
			return this.mCinematic;
		}

		// Token: 0x060002DD RID: 733 RVA: 0x00014818 File Offset: 0x00012A18
		public void lookAtColonyShip(Ship colonyShip, float time)
		{
			Vector3 vector = colonyShip.getPosition();
			Quaternion quaternion = Quaternion.Euler(25f, 180f + PlanetManager.getCurrentPlanet().getColonyShipRotation(), 0f);
			vector += colonyShip.getDirection() * 45f;
			vector = this.calculateFloorPosition(vector, 21f);
			SimpleTransform simpleTransform = new SimpleTransform(vector, quaternion);
			this.transition(simpleTransform, time);
		}

		// Token: 0x060002DE RID: 734 RVA: 0x00014880 File Offset: 0x00012A80
		public void placeOnFloor(float height)
		{
			this.mMainCamera.transform.position = this.calculateFloorPosition(this.getPosition(), height);
		}

		// Token: 0x060002DF RID: 735 RVA: 0x000148A0 File Offset: 0x00012AA0
		public Vector3 calculateFloorPosition(Vector3 position, float height)
		{
			Vector3 vector;
			if (PhysicsUtil.findFloor(position, out vector, 256))
			{
				vector.y = Mathf.Max(vector.y - 7.5f, 0f);
				return vector + Vector3.up * height;
			}
			return position;
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x000148EC File Offset: 0x00012AEC
		public Vector3 calculateCameraPosition(Vector3 position, float height)
		{
			Vector3 vector;
			if (PhysicsUtil.findFloor(position, out vector, 256))
			{
				return vector + new Vector3(0f, height, 0f);
			}
			Debug.LogWarning("Could not find camera floor!!");
			return position;
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x0001492C File Offset: 0x00012B2C
		public void scrollToPosition(Vector3 targetPosition)
		{
			if (!this.mLocked)
			{
				Vector3 vector = this.mMainCamera.transform.position;
				Vector3 forward = this.mMainCamera.transform.forward;
				forward.y = 0f;
				forward.Normalize();
				vector.x = targetPosition.x;
				vector.z = targetPosition.z;
				vector -= forward * 30f;
				vector = this.calculateCameraPosition(vector, this.mCurrentHeight);
				SimpleTransform simpleTransform = new SimpleTransform(vector, this.mMainCamera.transform.rotation);
				this.transition(simpleTransform, 1f);
			}
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x000149D8 File Offset: 0x00012BD8
		public void focusOnPosition(Vector3 position, float distance)
		{
			SimpleTransform simpleTransform = new SimpleTransform(this.mMainCamera.transform);
			simpleTransform.focusOn(position, distance);
			this.transition(simpleTransform, 1f);
			this.mLocked = true;
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x00014A11 File Offset: 0x00012C11
		public void unfocus()
		{
			this.mLocked = false;
			this.transition(this.mSourceTransform, 1f);
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x00014A2B File Offset: 0x00012C2B
		public void lockZoom()
		{
			this.mAcceleration.y = 0f;
			if (this.mCurrentHeight < 21f)
			{
				this.mTargetHeight = 21f;
			}
			this.mZoomLocked = true;
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x00014A5C File Offset: 0x00012C5C
		public void unlockZoom()
		{
			this.mZoomLocked = false;
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x00014A65 File Offset: 0x00012C65
		public void setCameraTransform(SimpleTransform targetTransform)
		{
			this.mTransitionTime = 0f;
			this.mCameraTransition = 1f;
			targetTransform.apply(this.mMainCamera.transform);
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x00014A8E File Offset: 0x00012C8E
		public void transition(SimpleTransform targetTransform, float time)
		{
			this.mCameraTransition = 0f;
			this.mTransitionTime = time;
			this.mSourceTransform = new SimpleTransform(this.mMainCamera.transform);
			this.mTargetTransform = targetTransform;
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x00014AC0 File Offset: 0x00012CC0
		public void serialize(XmlNode rootNode, string name)
		{
			XmlNode xmlNode = Serialization.createNode(rootNode, name, null);
			Serialization.serializeFloat(xmlNode, "height", this.mCurrentHeight);
			Serialization.serializeVector3(xmlNode, "position", this.mMainCamera.transform.position);
			Serialization.serializeQuaternion(xmlNode, "orientation", this.mMainCamera.transform.localRotation);
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x00014B1C File Offset: 0x00012D1C
		public void deserialize(XmlNode node)
		{
			this.mMainCamera.transform.position = Serialization.deserializeVector3(node["position"]);
			Vector3 eulerAngles = CameraManager.StartupOrientation.eulerAngles;
			eulerAngles.y = Serialization.deserializeQuaternion(node["orientation"]).eulerAngles.y;
			this.mMainCamera.transform.localRotation = Quaternion.Euler(eulerAngles);
			this.mCurrentHeight = Serialization.deserializeFloat(node["height"]);
			this.mTargetHeight = this.mCurrentHeight;
		}

		// Token: 0x060002EA RID: 746 RVA: 0x00014BB3 File Offset: 0x00012DB3
		public GameObject getCamera()
		{
			return this.mMainCamera;
		}

		// Token: 0x060002EB RID: 747 RVA: 0x00014BBB File Offset: 0x00012DBB
		public static CameraManager getInstance()
		{
			if (CameraManager.mInstance == null)
			{
				CameraManager.mInstance = new CameraManager();
			}
			return CameraManager.mInstance;
		}

		// Token: 0x060002EC RID: 748 RVA: 0x00014BD3 File Offset: 0x00012DD3
		public Transform getTransform()
		{
			return this.mMainCamera.transform;
		}

		// Token: 0x060002ED RID: 749 RVA: 0x00014BE0 File Offset: 0x00012DE0
		public Vector3 getPosition()
		{
			return this.mMainCamera.transform.position;
		}

		// Token: 0x060002EE RID: 750 RVA: 0x00014BF2 File Offset: 0x00012DF2
		public Vector3 getDirection()
		{
			return this.mMainCamera.transform.forward;
		}

		// Token: 0x060002EF RID: 751 RVA: 0x00014C04 File Offset: 0x00012E04
		public Vector3 worldToScreenPoint(Vector3 point)
		{
			return this.mMainCameraComponent.WorldToScreenPoint(point);
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x00014C12 File Offset: 0x00012E12
		public void setNearClipPlane(float nearClipPlane)
		{
			this.mMainCameraComponent.nearClipPlane = nearClipPlane;
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x00014C20 File Offset: 0x00012E20
		public void resetSettings()
		{
			this.mMainCameraComponent.nearClipPlane = 0.5f;
			this.mMainCameraComponent.fieldOfView = 60f;
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x00014C44 File Offset: 0x00012E44
		public void renderCubemap()
		{
			int num = 1024;
			Cubemap cubemap = new Cubemap(num, TextureFormat.RGB24, false);
			Texture2D texture2D = new Texture2D(num * 6, num, TextureFormat.RGB24, false);
			Color[] array = new Color[num * 6 * num];
			GameObject gameObject = new GameObject("CubemapCamera");
			Camera camera = gameObject.AddComponent<Camera>();
			gameObject.transform.position = this.mMainCamera.transform.position;
			gameObject.transform.rotation = Quaternion.identity;
			camera.cullingMask = -1;
			camera.farClipPlane = 2000000f;
			gameObject.GetComponent<Camera>().RenderToCubemap(cubemap);
			for (int i = 0; i < 6; i++)
			{
				Color[] pixels = cubemap.GetPixels((CubemapFace)i);
				for (int j = 0; j < num; j++)
				{
					Array.Copy(pixels, num * (num - j - 1), array, num * i + num * 6 * j, num);
				}
			}
			texture2D.SetPixels(array);
			Directory.CreateDirectory(Util.getFilesFolder());
			byte[] array2 = texture2D.EncodeToPNG();
			File.WriteAllBytes(Util.getFilesFolder() + "/cubemap" + DateTime.Now.ToString("_h_mm_ss_ff") + ".png", array2);
			Object.Destroy(gameObject);
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x00014D70 File Offset: 0x00012F70
		public bool isSoundInRange(SoundDefinition definition, Vector3 soundPosition)
		{
			float num = definition.getMaxDistance() + 10f;
			return (this.getPosition() - soundPosition).sqrMagnitude < num * num;
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x00014DA3 File Offset: 0x00012FA3
		public float getHeightRatio()
		{
			return Mathf.Clamp01((this.mMainCamera.transform.position.y - 12f) / 18f);
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x00014DCB File Offset: 0x00012FCB
		public void setScatteringParameters(float scatteringIntensity, float scatteringHeightFalloff)
		{
			MadrugaScattering component = this.mMainCamera.GetComponent<MadrugaScattering>();
			component.ScatteringIntensity = scatteringIntensity;
			component.ScatteringHeightFalloff = scatteringHeightFalloff;
		}

		// Token: 0x0400034B RID: 843
		private static CameraManager mInstance = null;

		// Token: 0x0400034C RID: 844
		private GameObject mMainCamera;

		// Token: 0x0400034D RID: 845
		private Camera mMainCameraComponent;

		// Token: 0x0400034E RID: 846
		private GameObject mSkydomeCamera;

		// Token: 0x0400034F RID: 847
		private Cinematic mCinematic;

		// Token: 0x04000350 RID: 848
		private SimpleTransform mTargetTransform;

		// Token: 0x04000351 RID: 849
		private SimpleTransform mSourceTransform;

		// Token: 0x04000352 RID: 850
		private SimpleTransform mCurrentTransform = new SimpleTransform();

		// Token: 0x04000353 RID: 851
		private float mCameraTransition = 1f;

		// Token: 0x04000354 RID: 852
		private float mTransitionTime;

		// Token: 0x04000355 RID: 853
		private bool mLocked;

		// Token: 0x04000356 RID: 854
		private bool mZoomLocked;

		// Token: 0x04000357 RID: 855
		private float mCurrentHeight = 21f;

		// Token: 0x04000358 RID: 856
		private float mTargetHeight = 21f;

		// Token: 0x04000359 RID: 857
		private Vector3 mAcceleration = Vector3.zero;

		// Token: 0x0400035A RID: 858
		private float mRotationAcceleration;

		// Token: 0x0400035B RID: 859
		private float mPreviousMouseX;

		// Token: 0x0400035C RID: 860
		private bool mCapsLock;

		// Token: 0x0400035D RID: 861
		private float mZoomAxis;

		// Token: 0x0400035E RID: 862
		private const float MinMovement = 0.01f;

		// Token: 0x0400035F RID: 863
		private const float TranslationStep = 80f;

		// Token: 0x04000360 RID: 864
		private const float ZoomStep = 60f;

		// Token: 0x04000361 RID: 865
		private const float RotationStep = 120f;

		// Token: 0x04000362 RID: 866
		private const float MinHeight = 12f;

		// Token: 0x04000363 RID: 867
		private const float MaxHeight = 30f;

		// Token: 0x04000364 RID: 868
		private const float HeightTolerance = 7.5f;

		// Token: 0x04000365 RID: 869
		private const float MinDisplacement = 0.001f;

		// Token: 0x04000366 RID: 870
		private const float VerticalAngle = 25f;

		// Token: 0x04000367 RID: 871
		private const float TitleClipDistance = 50000f;

		// Token: 0x04000368 RID: 872
		private const float GameClipDistance = 20000f;

		// Token: 0x04000369 RID: 873
		public const float DefaultHeight = 21f;

		// Token: 0x0400036A RID: 874
		public const float DefaultFov = 60f;

		// Token: 0x0400036B RID: 875
		public const float DefaultNearClipPlane = 0.5f;

		// Token: 0x0400036C RID: 876
		public static readonly Quaternion StartupOrientation = Quaternion.Euler(25f, 180f, 0f);

		// Token: 0x0400036D RID: 877
		private static readonly SimpleTransform DefaultCameraTransform = new SimpleTransform(new Vector3(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f));
	}
}
