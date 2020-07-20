using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWeapon : Weapon {

	public GameObject projectile;

	protected override void WeaponSpecific(Vector3 shootVec){
		FireProjectile (shootVec, projectile);
	}

}
