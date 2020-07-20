using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : NPC {

	public float directionChangeTime = 2f;
	public float stationaryTime = 1f;

	private SpriteAnimator animator;

	// Use this for initialization
	void Start () {
		state = States.Idle;
		animator = gameObject.GetComponentInChildren<SpriteAnimator> ();
		projectiles = new List<GameObject> ();
		nextShot = timeBetweenShots;
		gc = GameController.Instance;
		StartCoroutine (UpdateVelocityTimer ());
		transform.position = GameController.Instance.RandomPosition();
	}
		
	protected override void Shoot(){
		Vector3 shootVec = PlayerController.Instance.transform.position - transform.position;
		FireProjectile (BasicProjectile.Owner.Enemy, shootVec);

		animator.ChangeState (States.Attack, direction);
	}

	protected override void Move(){
		if (PlayerController.Instance != null) {
			//Movimento
			if (changeVelocity) {
				if (moveVelocity == Vector3.zero) {
					Vector3 dir = (PlayerController.Instance.transform.position - transform.position).normalized;
					state = States.Move;
					CalculateDirection (dir);
					//Realiza Movimento
					moveVelocity = dir * movespeed * Time.deltaTime;
				} else {
					moveVelocity = Vector3.zero;
					state = States.Idle;
				}

				animator.ChangeState (state, direction);

				changeVelocity = false;
				StopAllCoroutines ();
				StartCoroutine (UpdateVelocityTimer ());
			}
			transform.position += moveVelocity;
		}
	}

	protected override void Attack(){
		ProjectileTimer ();
	}

	protected void CalculateDirection(Vector3 dir){
		float x = dir.x;
		float z = dir.z;

		if (x < 0) { //left
			if (z < 0.5 && z > -0.5) {
				direction = Directions.W;
			} else if (z >= 0.5) {
				direction = Directions.NW;
			} else if (z <= -0.5) {
				direction = Directions.SW;
			}
		} else if (x > 0) { //right
			if (z < 0.5 && z > -0.5) {
				direction = Directions.E;
			} else if (z >= 0.5) {
				direction = Directions.NE;
			} else if (z <= -0.5) {
				direction = Directions.SE;
			}
		} else if (z > 0) { //up
			direction = Directions.N;
		} else if (z < 0) { //down
			direction = Directions.S;
		} else {
			direction = Directions.Unspecified;
		}
	}

	IEnumerator UpdateVelocityTimer(){
		if (moveVelocity == Vector3.zero)
			yield return new WaitForSeconds (stationaryTime);
		else
			yield return new WaitForSeconds (directionChangeTime);
		changeVelocity = true;
	}
}
