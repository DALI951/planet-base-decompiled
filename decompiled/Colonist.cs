using System;

namespace Planetbase
{
	// Token: 0x020000B8 RID: 184
	public class Colonist : Human
	{
		// Token: 0x06000471 RID: 1137 RVA: 0x0001B648 File Offset: 0x00019848
		public static Colonist findNearestStanding(Character character)
		{
			float num = float.MaxValue;
			Colonist colonist = null;
			foreach (Character character2 in Character.mCharacters)
			{
				Colonist colonist2 = character2 as Colonist;
				if (colonist2 != character && colonist2 != null && !colonist2.isDead() && !colonist2.isKo() && colonist2.getLocation() == Location.Interior)
				{
					float sqrMagnitude = (character.getPosition() - colonist2.getPosition()).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						colonist = colonist2;
						num = sqrMagnitude;
					}
				}
			}
			return colonist;
		}

		// Token: 0x06000472 RID: 1138 RVA: 0x0001B6E8 File Offset: 0x000198E8
		public static Colonist findNearestVictim(Character character)
		{
			float num = float.MaxValue;
			Colonist colonist = null;
			foreach (Character character2 in Character.mCharacters)
			{
				Colonist colonist2 = character2 as Colonist;
				if (colonist2 != null && colonist2.getLocation() == Location.Interior && colonist2 != character && !colonist2.hasFlag(32) && !colonist2.isDead())
				{
					float sqrMagnitude = (character.getPosition() - colonist2.getPosition()).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						colonist = colonist2;
						num = sqrMagnitude;
					}
				}
			}
			return colonist;
		}
	}
}
