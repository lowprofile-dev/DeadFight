using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticLaserEnemy : NPC {

	public float idleTime = 1f;
	public float chargeTime = 2f;
	public float shootTime = 3f;

	public float laserDamage = 0.01f;

	public Transform LaserOrigin;

	private Directions shootDirection;
	private Vector3 shootVector;

	public bool laserFiring = false;
	private bool changePhase;

	private LineRenderer line;

	private SpriteAnimator animator;

	// Use this for initialization
	void Start () {
		state = States.Idle;
		animator = gameObject.GetComponentInChildren<SpriteAnimator> ();
		line = gameObject.GetComponentInChildren<LineRenderer> ();
		line.enabled = false;

		shootDirection = RandomDirection ();
		direction = GetEastWest (shootDirection);
		shootVector = CalculateVec3 (shootDirection);
		StartCoroutine (ShootLaser ());

		animator.ChangeState (state, shootDirection);
		StartCoroutine (SequenceTimer ());

		gc = GameController.Instance;
		transform.position = GameController.Instance.RandomPosition();
	}

	protected Directions RandomDirection(){
		return (Directions)Random.Range (0, 8);
	}

	protected Directions GetEastWest(Directions direction){
		switch (direction) {
		case Directions.NE:
		case Directions.E:
		case Directions.SE:
		case Directions.S:
			return Directions.E;
		default:
			return Directions.W;
		}
	}

	protected override void Attack(){
		if (changePhase) {
			if (state == States.Idle) {
				//Carrega Laser
				state = States.Move;
				animator.ChangeState (state, direction);
			} else if (state == States.Move) {
				//Dispara Laser
				state = States.Attack;
				animator.ChangeState (state, direction);
				laserFiring = true;
				StopCoroutine ("ShootLaser");
				StartCoroutine ("ShootLaser");
			} else if (state == States.Attack) {
				state = States.Idle;
				animator.ChangeState (state, direction);
				laserFiring = false;
			}
			changePhase = false;
			StartCoroutine (SequenceTimer ());
		}
	}

	protected Vector3 CalculateVec3(Directions direction){
		switch (direction) {
		case Directions.N:
			return new Vector3 (0, 0, 1).normalized;
		case Directions.NE:
			return new Vector3 (1, 0, 1).normalized;
		case Directions.E:
			return new Vector3 (1, 0, 0).normalized;
		case Directions.SE:
			return new Vector3 (1, 0, -1).normalized;
		case Directions.S:
			return new Vector3 (0, 0, -1).normalized;
		case Directions.SW:
			return new Vector3 (-1, 0, -1).normalized;
		case Directions.W:
			return new Vector3 (-1, 0, 0).normalized;
		default: //NW
			return new Vector3 (-1, 0, 1).normalized;
		}

	}


	IEnumerator SequenceTimer(){
		if (state == States.Idle)
			yield return new WaitForSeconds (idleTime);
		else if (state == States.Move)
			yield return new WaitForSeconds (chargeTime);
		else
			yield return new WaitForSeconds (shootTime);
		changePhase = true;
	}

	IEnumerator ShootLaser(){
		line.enabled = true;
		while (laserFiring) {
			Ray ray = new Ray (LaserOrigin.transform.position, shootVector);
			RaycastHit hit;
			int layerMask = 1 << 8; //Detecta Colisoes com o Jogador.

			line.SetPosition (0, ray.origin);
			if (Physics.Raycast (ray, out hit, 100, layerMask)) {
				line.SetPosition (1, ray.GetPoint (hit.distance + 2f));
				PlayerController.Instance.PlayerLaserHit (laserDamage);
			}
			else
				line.SetPosition (1, ray.GetPoint (100));
			
			yield return null;
		}

		line.enabled = false;
	}
}
