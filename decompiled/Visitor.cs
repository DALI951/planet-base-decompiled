using System;

namespace Planetbase
{
	// Token: 0x020000C4 RID: 196
	public class Visitor : Specialization
	{
		// Token: 0x0600050A RID: 1290 RVA: 0x0001EB6C File Offset: 0x0001CD6C
		public Visitor()
		{
			this.mAi = VisitorAi.getInstance();
			this.mIcon = base.loadIcon();
			this.mModel = base.loadMaleModel();
			this.mModelFemale = base.loadFemaleModel();
			this.mModelExterior = ResourceUtil.loadPrefab("Prefabs/Characters/PrefabAstronaut");
			this.mModelTracksuit = ResourceUtil.loadPrefab("Prefabs/Characters/PrefabMaleTracksuit");
			this.mModelTracksuitFemale = ResourceUtil.loadPrefab("Prefabs/Characters/PrefabFemaleTracksuit");
			this.mCharacterType = typeof(Guest);
			this.mColor = CharacterDefinitions.getInstance().Guest.MainColor;
			base.addHeads(CharacterDefinitions.getInstance().Guest);
			this.mFlags = 384;
		}
	}
}
