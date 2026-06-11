using System;

namespace Planetbase
{
	// Token: 0x020000C3 RID: 195
	public class Guard : Specialization
	{
		// Token: 0x06000509 RID: 1289 RVA: 0x0001EAA0 File Offset: 0x0001CCA0
		public Guard()
		{
			this.mAi = GuardAi.getInstance();
			this.mIcon = base.loadIcon();
			this.mIconFemale = base.loadIconFemale();
			this.mModel = base.loadMaleModel();
			this.mModelFemale = base.loadFemaleModel();
			this.mModelExterior = ResourceUtil.loadPrefab("Prefabs/Characters/PrefabAstronaut");
			this.mModelTracksuit = ResourceUtil.loadPrefab("Prefabs/Characters/PrefabMaleTracksuit");
			this.mModelTracksuitFemale = ResourceUtil.loadPrefab("Prefabs/Characters/PrefabFemaleTracksuit");
			this.mDefaultRatio = 0f;
			this.mCharacterType = typeof(Colonist);
			this.mColor = CharacterDefinitions.getInstance().Guard.MainColor;
			base.addHeads(CharacterDefinitions.getInstance().Guard);
			this.mFlags = 160;
		}
	}
}
