using System;
using UnityEngine;

namespace Planetbase
{
	// Token: 0x020001E2 RID: 482
	public class ResourceList : MonoBehaviour
	{
		// Token: 0x06000E00 RID: 3584 RVA: 0x00052FD2 File Offset: 0x000511D2
		public ResourceList()
		{
			ResourceList.mInstance = this;
		}

		// Token: 0x06000E01 RID: 3585 RVA: 0x00052FEC File Offset: 0x000511EC
		private void Start()
		{
			if (!this.mLoaded)
			{
				this.loadOverlays();
				this.mLoaded = true;
			}
		}

		// Token: 0x06000E02 RID: 3586 RVA: 0x00053004 File Offset: 0x00051204
		private void loadOverlays()
		{
			this.Ui.load();
			this.Icons.process();
			this.TextureTickerArrowUp = Util.applyColor(this.TextureTickerArrowUp);
			this.TextureTickerArrowDown = Util.applyColor(this.TextureTickerArrowDown);
			this.TextureHighlight = Util.applyColor(this.TextureHighlight, GuiStyles.UiColorLow);
			this.TextureSelectionArrow = Util.applyColor(this.TextureSelectionArrow, GuiStyles.UiColorLow);
			ResourceList.StaticIcons = this.Icons;
		}

		// Token: 0x06000E03 RID: 3587 RVA: 0x00053080 File Offset: 0x00051280
		private void Update()
		{
		}

		// Token: 0x06000E04 RID: 3588 RVA: 0x00053082 File Offset: 0x00051282
		public static ResourceList getInstance()
		{
			return ResourceList.mInstance;
		}

		// Token: 0x06000E05 RID: 3589 RVA: 0x00053089 File Offset: 0x00051289
		public static Texture2D loadTexture(string path)
		{
			Texture2D texture2D = Resources.Load<Texture2D>(path);
			texture2D.name = path;
			if (texture2D == null)
			{
				Debug.LogWarning("Could not load texture: " + path);
			}
			return texture2D;
		}

		// Token: 0x04000AEC RID: 2796
		public static ResourceList mInstance;

		// Token: 0x04000AED RID: 2797
		[Header("Materials")]
		public MaterialContainer Materials;

		// Token: 0x04000AEE RID: 2798
		[Header("Cubemaps")]
		public Cubemap BlackCubemap;

		// Token: 0x04000AEF RID: 2799
		[Header("Textures")]
		public Texture2D OverlayRedAlert;

		// Token: 0x04000AF0 RID: 2800
		public Texture2D OverlayYellowAlert;

		// Token: 0x04000AF1 RID: 2801
		public UiIcons Ui;

		// Token: 0x04000AF2 RID: 2802
		public TitleTextures Title;

		// Token: 0x04000AF3 RID: 2803
		public Texture2D TextureCrosshair;

		// Token: 0x04000AF4 RID: 2804
		public Texture2D TextureTransform;

		// Token: 0x04000AF5 RID: 2805
		public Texture2D TextureSlot;

		// Token: 0x04000AF6 RID: 2806
		public Texture2D TextureBar;

		// Token: 0x04000AF7 RID: 2807
		public Texture2D TextureBarBackground;

		// Token: 0x04000AF8 RID: 2808
		public Texture2D TextureBarVertical;

		// Token: 0x04000AF9 RID: 2809
		public Texture2D TextureBarVerticalBackground;

		// Token: 0x04000AFA RID: 2810
		public Texture2D TextureCompanyLogo;

		// Token: 0x04000AFB RID: 2811
		public Texture2D TextureCursor;

		// Token: 0x04000AFC RID: 2812
		public Texture2D TextureMilestoneGauge;

		// Token: 0x04000AFD RID: 2813
		public Texture2D TextureMilestoneGaugeCompleted;

		// Token: 0x04000AFE RID: 2814
		public Texture2D ChallengeNotCompleted;

		// Token: 0x04000AFF RID: 2815
		public Texture2D ChallengeCompleted;

		// Token: 0x04000B00 RID: 2816
		public Texture2D TextureTickerArrowUp;

		// Token: 0x04000B01 RID: 2817
		public Texture2D TextureTickerArrowDown;

		// Token: 0x04000B02 RID: 2818
		public Texture2D TextureHighlight;

		// Token: 0x04000B03 RID: 2819
		public Texture2D TextureSelectionArrow;

		// Token: 0x04000B04 RID: 2820
		public Texture2D TextureIntruderHighlight;

		// Token: 0x04000B05 RID: 2821
		[Header("Icons")]
		public IconContainer Icons;

		// Token: 0x04000B06 RID: 2822
		[Header("Models")]
		public GameObject[] PrefabCorridorBase;

		// Token: 0x04000B07 RID: 2823
		public GameObject[] PrefabCorridorFloor;

		// Token: 0x04000B08 RID: 2824
		public GameObject[] PrefabCorridorTranslucent;

		// Token: 0x04000B09 RID: 2825
		public GameObject[] PrefabCorridorJoint;

		// Token: 0x04000B0A RID: 2826
		public GameObject[] PrefabCorridorJointBase;

		// Token: 0x04000B0B RID: 2827
		public GameObject[] PrefabCorridorFrame;

		// Token: 0x04000B0C RID: 2828
		public GameObject[] PrefabCorridorFrameBase;

		// Token: 0x04000B0D RID: 2829
		public GameObject[] PrefabPathFloor;

		// Token: 0x04000B0E RID: 2830
		public GameObject PrefabSphere;

		// Token: 0x04000B0F RID: 2831
		public GameObject PrefabCable;

		// Token: 0x04000B10 RID: 2832
		public GameObject PrefabCableJoint;

		// Token: 0x04000B11 RID: 2833
		public GameObject PrefabCorridorDoor;

		// Token: 0x04000B12 RID: 2834
		public GameObject PrefabMeteorSoundEffectsNear;

		// Token: 0x04000B13 RID: 2835
		public GameObject PrefabMeteorSoundEffectsFar;

		// Token: 0x04000B14 RID: 2836
		public GameObject PrefabThunderSoundEffectsNear;

		// Token: 0x04000B15 RID: 2837
		public GameObject PrefabThunderSoundEffectsFar;

		// Token: 0x04000B16 RID: 2838
		public GameObject PrefabFloorCylinder;

		// Token: 0x04000B17 RID: 2839
		public GameObject PrefabBlinkingLight;

		// Token: 0x04000B18 RID: 2840
		public GameObject PrefabSkydome;

		// Token: 0x04000B19 RID: 2841
		public GameObject PrefabTitleScene;

		// Token: 0x04000B1A RID: 2842
		public GameObject PrefabPlanetSelectionScene;

		// Token: 0x04000B1B RID: 2843
		public GameObject PrefabSignpostTextScene;

		// Token: 0x04000B1C RID: 2844
		public GameObject PrefabActionRadius;

		// Token: 0x04000B1D RID: 2845
		public ShipPrefabs Ships;

		// Token: 0x04000B1E RID: 2846
		[Header("Meteors")]
		public GameObject[] PrefabMeteors;

		// Token: 0x04000B1F RID: 2847
		[Header("Thunder")]
		public GameObject Thunder;

		// Token: 0x04000B20 RID: 2848
		[Header("Animations")]
		public GameObject PrefabHumanAnimations;

		// Token: 0x04000B21 RID: 2849
		[Header("Particles")]
		public ParticlePrefabs Particles;

		// Token: 0x04000B22 RID: 2850
		[Header("Fonts")]
		public Font FontMain;

		// Token: 0x04000B23 RID: 2851
		public Font FontTitles;

		// Token: 0x04000B24 RID: 2852
		public Font FontTitlesChinese;

		// Token: 0x04000B25 RID: 2853
		[Header("Cameras")]
		public GameObject[] QualityCameras = new GameObject[4];

		// Token: 0x04000B26 RID: 2854
		public static IconContainer StaticIcons;

		// Token: 0x04000B27 RID: 2855
		private bool mLoaded;
	}
}
