using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TridentWeapon : Weapon {

	public GameObject projectile;
	[Range(0,180)] public float spread = 30f;

	protected override void WeaponSpecific(Vector3 shootVec){
		//shoot 3 times, taking spread into account
		FireProjectile (shootVec, projectile);
		FireProjectile (Quaternion.Euler (0, spread, 0) * shootVec, projectile);
		FireProjectile (Quaternion.Euler (0, -spread, 0) * shootVec, projectile);
	}
}
