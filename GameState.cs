using System;

namespace Planetbase
{
	// Token: 0x02000129 RID: 297
	public abstract class GameState
	{
		// Token: 0x060008FB RID: 2299 RVA: 0x00034536 File Offset: 0x00032736
		public virtual void destroy()
		{
		}

		// Token: 0x060008FC RID: 2300 RVA: 0x00034538 File Offset: 0x00032738
		public virtual void onStartUpdating()
		{
		}

		// Token: 0x060008FD RID: 2301 RVA: 0x0003453A File Offset: 0x0003273A
		public virtual void fixedUpdate(float timeStep, int frameIndex)
		{
		}

		// Token: 0x060008FE RID: 2302 RVA: 0x0003453C File Offset: 0x0003273C
		public virtual void update(float timeStep)
		{
		}

		// Token: 0x060008FF RID: 2303 RVA: 0x0003453E File Offset: 0x0003273E
		public virtual void onEndOfFrame()
		{
		}

		// Token: 0x06000900 RID: 2304 RVA: 0x00034540 File Offset: 0x00032740
		public virtual void onGui()
		{
		}

		// Token: 0x06000901 RID: 2305 RVA: 0x00034542 File Offset: 0x00032742
		public virtual bool load()
		{
			return true;
		}

		// Token: 0x06000902 RID: 2306 RVA: 0x00034545 File Offset: 0x00032745
		public virtual bool shouldShowLoadingScreen()
		{
			return false;
		}

		// Token: 0x06000903 RID: 2307 RVA: 0x00034548 File Offset: 0x00032748
		public virtual float getLoadingWaitTime()
		{
			return 0f;
		}

		// Token: 0x06000904 RID: 2308 RVA: 0x0003454F File Offset: 0x0003274F
		public virtual bool shouldShowHint()
		{
			return false;
		}

		// Token: 0x06000905 RID: 2309 RVA: 0x00034552 File Offset: 0x00032752
		public virtual bool shouldFadeIn()
		{
			return false;
		}

		// Token: 0x06000906 RID: 2310 RVA: 0x00034555 File Offset: 0x00032755
		public virtual bool isCameraFixed()
		{
			return true;
		}

		// Token: 0x06000907 RID: 2311 RVA: 0x00034558 File Offset: 0x00032758
		public virtual bool isEndOfFramePending()
		{
			return false;
		}

		// Token: 0x06000908 RID: 2312 RVA: 0x0003455B File Offset: 0x0003275B
		public virtual bool isTitleState()
		{
			return false;
		}

		// Token: 0x06000909 RID: 2313 RVA: 0x0003455E File Offset: 0x0003275E
		public virtual bool shouldDrawAnnouncement()
		{
			return false;
		}
	}
}
