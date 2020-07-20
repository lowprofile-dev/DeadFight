using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMGWeapon : Weapon {

	public GameObject projectile;
	public float maxSpreadDeg = 10f;

	protected override void WeaponSpecific(Vector3 shootVec){
		//random spread

		FireProjectile (Quaternion.Euler (0, Random.Range(-maxSpreadDeg, maxSpreadDeg), 0) * shootVec, projectile);
	}

}
