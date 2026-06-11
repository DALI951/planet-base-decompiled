using System;

namespace Planetbase
{
	// Token: 0x02000294 RID: 660
	public struct Vector2i
	{
		// Token: 0x06001308 RID: 4872 RVA: 0x00068DAD File Offset: 0x00066FAD
		public Vector2i(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		// Token: 0x06001309 RID: 4873 RVA: 0x00068DBD File Offset: 0x00066FBD
		public static Vector2i operator +(Vector2i v1, Vector2i v2)
		{
			return new Vector2i(v1.x + v2.x, v1.y + v2.y);
		}

		// Token: 0x0600130A RID: 4874 RVA: 0x00068DDE File Offset: 0x00066FDE
		public static Vector2i operator -(Vector2i v1, Vector2i v2)
		{
			return new Vector2i(v1.x - v2.x, v1.y - v2.y);
		}

		// Token: 0x04000DA8 RID: 3496
		public int x;

		// Token: 0x04000DA9 RID: 3497
		public int y;
	}
}
