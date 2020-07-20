	using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LulaGigante : NPC {

	public int shotLimit = 2, dashLimit = 3;

	public float spread = 30f;
	public int NumOfProjectiles = 12;

	public float phaseChangeTime = 1f;

	public float dashSpeedMultiplier = 10f;

	private bool phaseChange = false;
	private int attackPhase = 0;
	private int shots = 0;
	private int dashes = 0;

	private SpriteAnimator animator;

	// Use this for initialization
	void Start () {
		state = States.Idle;
		animator = gameObject.GetComponentInChildren<SpriteAnimator> ();
		projectiles = new List<GameObject> ();
		nextShot = timeBetweenShots;
		gc = GameController.Instance;
		transform.position = GameController.Instance.RandomPosition();

		StartCoroutine (PhaseChangeTimer ());
	}

	protected override void Shoot(){
		animator.ChangeState (States.Idle, direction); //Animaçao Idle é igual á animaçao de Attack
		Vector3 shootVec = PlayerController.Instance.transform.position - transform.position;
		for (int i = 0; i < NumOfProjectiles; i++)
			FireProjectile (BasicProjectile.Owner.Enemy, Quaternion.Euler (0, i * spread, 0) * shootVec);
	}

	protected override void Move(){
		transform.position += moveVelocity;
	}

	protected override void Attack(){
		//Criaçao de Pattern de Attack =  move -> dash -> shoot -> dash
		if (PlayerController.Instance != null){

			if (attackPhase % 4 == 0) { //move
				if (phaseChange) {
					state = States.Move;
					phaseChange = false;
					UpdateVelocity (1);
					StopAllCoroutines ();
					StartCoroutine (PhaseChangeTimer ());

					attackPhase++;
				}

			} else if (attackPhase % 4 == 1 || attackPhase % 4 == 3) { // dash
				if (phaseChange) {
					dashes++;
					phaseChange = false;
					if (dashes > dashLimit) {
						attackPhase++;
						dashes = 0;
					} else {
						UpdateVelocity (dashSpeedMultiplier);
						state = States.Attack;
					}
					StopAllCoroutines ();
					StartCoroutine (PhaseChangeTimer ());
				}
			} else if (attackPhase % 4 == 2) { //shoot
				if (phaseChange) {
					shots++;
					phaseChange = false;
					if (shots > shotLimit) {
						attackPhase++;
						shots = 0;
					} else {
						UpdateVelocity(0); //don't move
						Shoot ();
						state = States.Idle;
					}
					StopAllCoroutines ();
					StartCoroutine (PhaseChangeTimer ());
				}
			}

			animator.ChangeState (state, direction);
		}
	}

	bool MoveTimer(){
			shotTimer = shotTimer + Time.deltaTime;

			if (shotTimer > nextShot && PlayerController.Instance != null)
			{
				nextShot = shotTimer + timeBetweenShots;
				Shoot ();
				nextShot = nextShot - shotTimer;
				shotTimer = 0.0F;
				return true;
			}
			return false;
	}

	void UpdateVelocity(float multiplier){
		Vector3 dir = (PlayerController.Instance.transform.position - transform.position).normalized;
		CalculateDirection (dir);

		moveVelocity = dir * movespeed * Time.deltaTime * multiplier;
	}

	protected void CalculateDirection(Vector3 dir){
		if (dir.x < 0) //left
			direction = Directions.W;
		else if (dir.x > 0) //right
			direction = Directions.E;
		else
			direction = Directions.Unspecified;
	}

	IEnumerator PhaseChangeTimer(){
		yield return new WaitForSeconds (phaseChangeTime);
		phaseChange = true;
	}

}
