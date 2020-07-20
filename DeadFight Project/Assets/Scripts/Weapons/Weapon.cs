using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour {

	public WeaponTypes Type;
	public PickupTypes PickupType;
	public float ProjectileSpeed = 40f;
	public float timeBetweenShots = 0.5f;
	public float minTimeBetweenShots = 0.2f;
	[HideInInspector]public bool shootDelaying = false;

	protected List<GameObject> projectiles = new List<GameObject>();

	protected SpriteAnimator animator;
	protected float savedTimeBetweenShots;

	void Awake(){
		savedTimeBetweenShots = timeBetweenShots;
		animator = GetComponent<SpriteAnimator> ();
	}

	public void Shoot (Vector3 shootVec){
		if (!shootDelaying) {
			WeaponSpecific(shootVec);
			shootDelaying = true;
			StopAllCoroutines ();
			StartCoroutine (ShotTimer ());
		}
	}

	protected abstract void WeaponSpecific (Vector3 shootVec);

	protected void FireProjectile(Vector3 shootVec, GameObject projectile){
		//spawn under player sprite
		Vector3 spawnVec = transform.position;
		spawnVec.y -= 1;

		GameObject projectileInstance = Instantiate (projectile, spawnVec, Quaternion.identity);
		BasicProjectile projectileScript = projectileInstance.GetComponent<BasicProjectile> ();
		projectileScript.owner = BasicProjectile.Owner.Player;
		projectileScript.SetVelocity (shootVec * ProjectileSpeed * Time.fixedDeltaTime);

		projectiles.Add (projectileInstance);
	}

	public void ChangeState(States state, Directions dir){
		animator.ChangeState (state, dir);
	}

	public void Toggle(){
		GetComponent<SpriteRenderer> ().enabled = !GetComponent<SpriteRenderer> ().enabled;
	}

	public void Show(){
		GetComponent<SpriteRenderer> ().enabled = true;
	}

	public void Hide(){
		GetComponent<SpriteRenderer> ().enabled = false;
	}

	public void Reset(){
		DestroyProjectiles ();
		timeBetweenShots = savedTimeBetweenShots;
	}

	public void ChangeFireRate(float val){
		timeBetweenShots = Mathf.Clamp (timeBetweenShots - val, minTimeBetweenShots, savedTimeBetweenShots);
	}

	public void DestroyProjectiles(){
		foreach (GameObject projectile in projectiles) {
			Destroy (projectile);
		}
		projectiles.Clear ();
	}

	IEnumerator ShotTimer(){
		yield return new WaitForSeconds (timeBetweenShots + 0.01f);
		shootDelaying = false;
	}

}
