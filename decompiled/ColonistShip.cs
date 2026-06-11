using System;
using System.Collections.Generic;
using UnityEngine;

namespace Planetbase
{
	// Token: 0x02000251 RID: 593
	public class ColonistShip : LandingShip
	{
		// Token: 0x060010F2 RID: 4338 RVA: 0x0005C9AC File Offset: 0x0005ABAC
		protected override GameObject getPrefab()
		{
			GameObject[] array;
			if (this.mSize == LandingShip.Size.Large)
			{
				array = ResourceList.getInstance().Ships.ColonistLarge;
			}
			else
			{
				array = ResourceList.getInstance().Ships.ColonistSmall;
			}
			return array[this.mId % array.Length];
		}

		// Token: 0x060010F3 RID: 4339 RVA: 0x0005C9F2 File Offset: 0x0005ABF2
		protected override void postInit()
		{
			base.postInit();
			this.mIcon = ((this.mSize == LandingShip.Size.Regular) ? ResourceUtil.loadIconColor("Ships/icon_ship_personnel") : ResourceUtil.loadIconColor("Ships/icon_ship_personnel_big"));
		}

		// Token: 0x060010F4 RID: 4340 RVA: 0x0005CA20 File Offset: 0x0005AC20
		public override void onLanded()
		{
			base.onLanded();
			float value = Singleton<Colony>.getInstance().getWelfareIndicator().getValue();
			int num = 1;
			if (value > 0.9f)
			{
				num = Random.Range(2, 4);
			}
			else if (value > 0.7f)
			{
				num = Random.Range(1, 3);
			}
			if (this.mSize == LandingShip.Size.Large)
			{
				num++;
			}
			if (this.mIntruders)
			{
				num += LandingShipManager.getExtraIntruders();
			}
			for (int i = 0; i < num; i++)
			{
				Specialization specialization = (this.mIntruders ? TypeList<Specialization, SpecializationList>.find<Intruder>() : this.calculateSpecialization());
				if (specialization != null)
				{
					Character.create(specialization, base.getSpawnPosition(i), Location.Exterior);
				}
			}
		}

		// Token: 0x060010F5 RID: 4341 RVA: 0x0005CAB8 File Offset: 0x0005ACB8
		private Specialization calculateSpecialization()
		{
			LandingPermissions landingPermissions = Singleton<LandingShipManager>.getInstance().getLandingPermissions();
			List<Specialization> list = new List<Specialization>();
			int countOfType = Character.getCountOfType<Colonist>();
			if (countOfType > 0)
			{
				foreach (Specialization specialization in SpecializationList.getColonistSpecializations())
				{
					if (100 * Character.getCountOfSpecialization(specialization) / countOfType < landingPermissions.getSpecializationPercentage(specialization).get())
					{
						list.Add(specialization);
					}
				}
			}
			if (list.Count == 0)
			{
				foreach (Specialization specialization2 in SpecializationList.getColonistSpecializations())
				{
					if (landingPermissions.getSpecializationPercentage(specialization2).get() > 0)
					{
						list.Add(specialization2);
					}
				}
			}
			if (list.Count > 0)
			{
				return list[Random.Range(0, list.Count)];
			}
			return null;
		}

		// Token: 0x060010F6 RID: 4342 RVA: 0x0005CBBC File Offset: 0x0005ADBC
		public override string getName()
		{
			return StringList.get("colonist_ship");
		}
	}
}
