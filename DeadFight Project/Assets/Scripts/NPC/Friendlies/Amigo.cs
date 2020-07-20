using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amigo: NPC {

	public float DistanceThreshold;
	public float moveTime = 1f;

	private SpriteAnimator animator;
	private Transform playerTransform;
	public bool attacking = false;

	// Use this for initialization
	void Start () {
		playerTransform = GameObject.FindGameObjectWithTag ("Player").transform;

		state = States.Idle;
		animator = gameObject.GetComponentInChildren<SpriteAnimator> ();
		projectiles = new List<GameObject> ();
		nextShot = timeBetweenShots;
		StartCoroutine (MoveTimer ());
		gc = GameController.Instance;
	}

	protected override void Shoot(){
		if (PlayerController.Instance != null && EnemyController.Instance.GetEnemies ().Count > 0) {

			Vector3 shootVec = NearestEnemyPos () - transform.position;
			CalculateDirection (shootVec);
			FireProjectile (BasicProjectile.Owner.Player, shootVec);
			state = States.Attack;
			animator.ChangeState (state, direction);
			attacking = true;
		}
	}

	protected override void Move(){
		if (PlayerController.Instance != null) {
			//Movimento
			float DistanceFromPlayer = Vector3.Distance(playerTransform.position, transform.position);
			Vector3 dir;

			if (changeVelocity) {
				//Pouco tempo de Idle
				if (DistanceFromPlayer < DistanceThreshold - DistanceThreshold * 0.5 || DistanceFromPlayer > DistanceThreshold + DistanceThreshold * 0.5) {
					if (DistanceFromPlayer > DistanceThreshold)
						//Anda atras do jogador
						dir = (playerTransform.position - transform.position).normalized;
					else
						//Anda longe do jogador
						dir = (transform.position - playerTransform.position).normalized;

					CalculateDirection (dir);
					//Disfarça movimentos na diagonal.
					if (direction == Directions.NE || direction == Directions.SE)
						direction = Directions.E;
					if (direction == Directions.NW || direction == Directions.SW)
						direction = Directions.W;
					if (!attacking)
						state = States.Move;
					moveVelocity = dir * movespeed * Time.deltaTime;

				} else {
					if (!attacking)
						state = States.Idle;
					moveVelocity = Vector3.zero;
				}
				animator.ChangeState (state, direction);
				changeVelocity = false;
				StopAllCoroutines ();
				StartCoroutine (MoveTimer ());
			}

			transform.position += moveVelocity;
		}
	}

	protected override void Attack(){
		if (EnemyController.Instance.GetEnemies ().Count > 0)
			ProjectileTimer ();
		else
			attacking = false;
	}

	protected Vector3 NearestEnemyPos(){
		List<GameObject> enemies = EnemyController.Instance.GetEnemies ();
		Vector3 closestPos = enemies[0].transform.position;
		float closestDist = Vector3.Distance (closestPos, transform.position);

		foreach (GameObject enemy in enemies) {
			float dist = Vector3.Distance (enemy.transform.position, transform.position);
			if (dist < closestDist) {
				closestPos = enemy.transform.position;
				closestDist = dist;
			}
		}

		return closestPos;
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

	IEnumerator MoveTimer(){
		yield return new WaitForSeconds (moveTime);
		changeVelocity = true;
	}

	void OnDestroy(){
		ClearProjectiles();
	}
}
