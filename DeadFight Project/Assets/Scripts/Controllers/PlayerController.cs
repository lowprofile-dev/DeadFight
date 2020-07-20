using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CnControls;

public class PlayerController : MonoBehaviour {

	public static PlayerController Instance = null;
	void Awake(){
		if (Instance == null) Instance = this;
		else if (Instance != this) Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	private GameController gc;

	public float health = 3;
	private float maxHealth;

	public GameObject BaseWeapon;
	public GameObject CurrentWeapon;
	public Weapon CurrentWeaponScript;
	public GameObject WeaponSpawnOrigin;

	public float speed = 20f;
	public float speedPenaltyMultiplier = 0.1f;
	public float score = 0f;
	private bool dead = true;

	public States state;
	private SpriteRenderer spriteRenderer;
	private MeshRenderer shadow;
	private Vector3 startPos = new Vector3(0,1,0);

	Vector3 moveVec = Vector3.zero;
	Vector3 shootVec = Vector3.zero;

	private FSM fsm;
	private Idle idle;
	private Move move;
	private Attack attack;
	private MoveAttack moveAttack;

	private Rigidbody rb;

	public int maxSentries = 3;
	private List<GameObject> amigos = new List<GameObject> ();
	public int SentryCount() { return amigos.Count; }

	// Use this for initialization
	void Start () {
		spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer> ();
		shadow = gameObject.GetComponentInChildren<MeshRenderer>();
		gc = GameController.Instance;
		rb = GetComponent<Rigidbody> ();
		maxHealth = health;
		Reset ();
		Hide ();

		//STATE MACHINE INITIALISATION
		fsm = new FSM();
		idle = new Idle ();
		move = new Move ();
		attack = new Attack ();
		moveAttack = new MoveAttack ();

		//idle:			moveVec.sqrMagnitude == 0 && shootVec.sqrMagnitude == 0
		//move:			moveVec.sqrMagnitude != 0 && shootVec.sqrMagnitude == 0
		//attack:		moveVec.sqrMagnitude == 0 && shootVec.sqrMagnitude != 0
		//moveattack:	moveVec.sqrMagnitude != 0 && shootVec.sqrMagnitude != 0

		idle.AddTransition (new Transition (move, () => moveVec.sqrMagnitude != 0 && shootVec.sqrMagnitude == 0));
		idle.AddTransition (new Transition (attack, () => moveVec.sqrMagnitude == 0 && shootVec.sqrMagnitude != 0));
		idle.AddTransition (new Transition (moveAttack, () => moveVec.sqrMagnitude != 0 && shootVec.sqrMagnitude != 0));

		move.AddTransition (new Transition (idle, () => moveVec.sqrMagnitude == 0 && shootVec.sqrMagnitude == 0));
		move.AddTransition (new Transition (attack, () => moveVec.sqrMagnitude == 0 && shootVec.sqrMagnitude != 0));
		move.AddTransition (new Transition (moveAttack, () => moveVec.sqrMagnitude != 0 && shootVec.sqrMagnitude != 0));

		//attack not moving
		attack.AddTransition (new Transition (move, () => moveVec.sqrMagnitude != 0 && shootVec.sqrMagnitude == 0 && !CurrentWeaponScript.shootDelaying));
		attack.AddTransition (new Transition (idle, () => moveVec.sqrMagnitude == 0 && shootVec.sqrMagnitude == 0 && !CurrentWeaponScript.shootDelaying));
		attack.AddTransition (new Transition (moveAttack, () => moveVec.sqrMagnitude != 0 && shootVec.sqrMagnitude != 0));

		//attack while moving
		moveAttack.AddTransition (new Transition (move, () => moveVec.sqrMagnitude != 0 && shootVec.sqrMagnitude == 0 && !CurrentWeaponScript.shootDelaying));
		moveAttack.AddTransition (new Transition (idle, () => moveVec.sqrMagnitude == 0 && shootVec.sqrMagnitude == 0 && !CurrentWeaponScript.shootDelaying));
		moveAttack.AddTransition (new Transition (attack, () => moveVec.sqrMagnitude == 0 && shootVec.sqrMagnitude != 0));


		fsm.AddState (idle);
		fsm.AddState (move);
		fsm.AddState (attack);

		fsm.Init (idle, Directions.Unspecified, Directions.Unspecified);
	}

	// Update is called once per frame
	void Update () {
		if (!dead) {

			state = fsm.CurrentState.state;

			GetInputVectors ();

			fsm.Update ();
			Directions shootDir = CalculateDirection (shootVec);
			Directions moveDir = CalculateDirection (moveVec);
			fsm.UpdateDirection (moveDir, shootDir);

			UpdateWeaponZIndex (moveDir, shootDir);

			if (state != States.Idle)
				Move ();

			if (shootVec.sqrMagnitude > 0)
				CurrentWeaponScript.Shoot (shootVec);

			if (amigos.Count > 0)
				SpreadSentries ();
		}
	}

	void UpdateWeaponZIndex(Directions moveDir, Directions shootDir){
		bool under = true;
		Vector3 temp = WeaponSpawnOrigin.transform.position;

		switch (fsm.CurrentState.state) {
		case States.Move:
			if (moveDir == Directions.S || moveDir == Directions.Unspecified)
				under = false;
			else
				under = true;
			break;
		case States.MoveAttack:
			if (shootDir == Directions.S || shootDir == Directions.Unspecified)
				under = false;
			else
				under = true;
			break;
		case States.Idle:
		case States.Attack:
			under = false;
			break;
		default:
			under = true; //Cria arma Padrao por baixo do jogador.
			break;
		}

		if (under)
			temp.y = transform.position.y - 0.1f;
		else
			temp.y = transform.position.y + 0.1f;

		WeaponSpawnOrigin.transform.position = temp;

	}

	void GetInputVectors(){
#if UNITY_ANDROID || UNITY_IOS
		shootVec.x = CnInputManager.GetAxis("ShootHorizontal");
		shootVec.z = CnInputManager.GetAxis("ShootVertical");
		moveVec.x = CnInputManager.GetAxis("Horizontal");
		moveVec.z = CnInputManager.GetAxis("Vertical");
#else
		shootVec.x = Input.GetAxisRaw("ShootHorizontal");
		shootVec.z = Input.GetAxisRaw("ShootVertical");
		moveVec.x = Input.GetAxisRaw("Horizontal");
		moveVec.z = Input.GetAxisRaw("Vertical");
#endif
		shootVec = shootVec.normalized;
		moveVec = moveVec.normalized;
	}

	void Move(){
		if (state == States.Attack && !SameDir(CalculateDirection(moveVec), CalculateDirection(shootVec)))
			rb.velocity = moveVec * speed * (1 - speedPenaltyMultiplier);
		else
			rb.velocity = moveVec * speed;

		rb.position = new Vector3 (
			Mathf.Clamp (rb.position.x, GameController.Instance.boundary.xMin, GameController.Instance.boundary.xMax),
			0f,
			Mathf.Clamp (rb.position.z, GameController.Instance.boundary.zMin, GameController.Instance.boundary.zMax));
	}

	void OnTriggerEnter(Collider collider){
		if (!dead) {
			if (collider.gameObject.CompareTag ("Door")) {
				//collide with player
				PlayerDoorCollide (transform.position);
				LevelController.Instance.PlayerDoorCollide (collider.gameObject.GetComponent<DoorController>().type);
			}

			if (collider.gameObject.CompareTag ("Pickup")) {
				CollectPickup (collider.gameObject.GetComponent<Pickup> ());
			}

			if ((collider.gameObject.CompareTag ("Projectile") && collider.gameObject.GetComponent<BasicProjectile> ().owner == BasicProjectile.Owner.Enemy) ||
			   collider.gameObject.CompareTag ("Enemy")) {
				//Jogador colide com disparo do inimigo ou inimigo
				PlayerHit(collider.gameObject);
			}

			if (collider.gameObject.CompareTag ("Bank")) {
				gc.OpenBank ();
			}
		}
	}

	void OnTriggerExit(Collider collider){
		if (!dead) {
			if (collider.gameObject.CompareTag ("Bank")) {
				gc.CloseBank ();
			}
		}
	}

	void PlayerHit(GameObject collider){
		float damage;
		//Dano de acordo com a vida atual do inimigo ou dependendo do tipo de Disparo
		if (collider.CompareTag ("Enemy")) {
			damage = collider.GetComponentInChildren<HealthBar> ().health;
		} else {
			damage = collider.GetComponent<BasicProjectile> ().Damage;
			Destroy (collider);
		}
		SubHealth (damage);
	}

	public void PlayerLaserHit(float damage){
		SubHealth (damage);
	}

	void SubHealth(float damage){
		health = Mathf.Clamp (health - damage, 0, maxHealth);

		StopAllCoroutines ();
		spriteRenderer.enabled = true;
		StartCoroutine (FlashSprite(3, 0.1f));

		if (health <= 0) {
			DestroySentries ();
			Hide ();
			gc.PlayerDie ();
		}
	}

	public void SwapWeapon(GameObject NewWeapon){
		if (CurrentWeapon == null || CurrentWeaponScript.Type != NewWeapon.GetComponent<Weapon>().Type) {
			Destroy (CurrentWeapon);
			CurrentWeapon = Instantiate (NewWeapon, WeaponSpawnOrigin.transform);
			CurrentWeaponScript = CurrentWeapon.GetComponent<Weapon> ();
		}
	}

	public void Hide(){
		spriteRenderer.enabled = false;
		shadow.enabled = false;
		if (CurrentWeaponScript != null) {
			CurrentWeaponScript.Hide ();
			CurrentWeaponScript.Reset ();
		}
		StopAllCoroutines ();
		dead = true;
	}

	public void Show(){
		spriteRenderer.enabled = true;
		shadow.enabled = true;
		if (CurrentWeaponScript != null)
			CurrentWeaponScript.Show ();
		if (dead)
			Reset ();
		StopAllCoroutines ();
		dead = false;
	}

	public void EnemyKilled(){
		score += ((gc.level - 1) * gc.sublevelMax) + gc.sublevel;
	}

	public void PlayerDoorCollide(Vector3 position){
		//Acontece assim que entrar na Porta
		Vector3 nextPos;
		float x = position.x;
		float y = position.y;
		float z = position.z;
		if (z > gc.boundary.zMax - 5)
			nextPos = new Vector3 (x, y, z * -1);
		else if (z < gc.boundary.zMin + 5)
			nextPos = new Vector3 (x, y, z * -1);
		else if (x > gc.boundary.xMax - 5)
			nextPos = new Vector3 (x * -1, y, z);
		else
			nextPos = new Vector3 (x * -1, y, z);
		rb.position = nextPos;

		//Posiciona Amigos no Jogador, deixando.os espalhar naturalmente.
		foreach (GameObject amigo in amigos)
			amigo.transform.position = rb.position;
	}

	public void CollectPickup(Pickup pickup){
		switch (pickup.type) {
		case PickupTypes.firerateUp:
			CurrentWeaponScript.ChangeFireRate(pickup.value);
			break;
		case PickupTypes.sentry:
			amigos.Add(Instantiate(pickup.SpawnPrefab, transform.position, Quaternion.identity));
			break;
		case PickupTypes.basicWeapon:
			SwapWeapon (WeaponsList.Instance.GetWeaponOfType (WeaponTypes.basic));
			break;
		case PickupTypes.tridentWeapon:
			SwapWeapon (WeaponsList.Instance.GetWeaponOfType (WeaponTypes.triple));
			break;
		case PickupTypes.SMGWeapon:
			SwapWeapon (WeaponsList.Instance.GetWeaponOfType (WeaponTypes.smg));
			break;
		default:
			Debug.Log ("Pickup not found");
			break;
		}
	}

	public void Reset(){
		ResetPos ();
		DestroySentries ();
		score = 0;
		health = maxHealth;
		SwapWeapon (BaseWeapon);
		if (CurrentWeaponScript != null)
			CurrentWeaponScript.Reset ();
	}

	bool SameDir(Directions move, Directions shoot){
		switch (move) {
		case Directions.N:
			if (shoot == Directions.W || shoot == Directions.NW || shoot == Directions.N || shoot == Directions.NE || shoot == Directions.E)
				return true;
			break;
		case Directions.NE:
			if (shoot == Directions.NW || shoot == Directions.N || shoot == Directions.NE || shoot == Directions.E || shoot == Directions.SE)
				return true;
			break;
		case Directions.E:
			if (shoot == Directions.N || shoot == Directions.NE || shoot == Directions.E || shoot == Directions.SE || shoot == Directions.S)
				return true;
			break;
		case Directions.SE:
			if (shoot == Directions.NE || shoot == Directions.E || shoot == Directions.SE || shoot == Directions.S || shoot == Directions.SW)
				return true;
			break;
		case Directions.S:
			if (shoot == Directions.E || shoot == Directions.SE || shoot == Directions.S || shoot == Directions.SW || shoot == Directions.W)
				return true;
			break;
		case Directions.SW:
			if (shoot == Directions.SE || shoot == Directions.S || shoot == Directions.SW || shoot == Directions.W || shoot == Directions.NW)
				return true;
			break;
		case Directions.W:
			if (shoot == Directions.S || shoot == Directions.SW || shoot == Directions.W || shoot == Directions.NW || shoot == Directions.N)
				return true;
			break;
		case Directions.NW:
			if (shoot == Directions.SW || shoot == Directions.W || shoot == Directions.NW || shoot == Directions.N || shoot == Directions.NE)
				return true;
			break;
		}
		return false;
	}

	void SpreadSentries(){
		for (int i = 0; i < amigos.Count; i++) {
			for (int j = 0; j < amigos.Count; j++) {
				if (i != j) { //don't compare same indexes
					float distance = Vector3.Distance (amigos [i].transform.position, amigos [j].transform.position);
					if (distance < 2) {
						if (distance == 0) {
							//se distancia igual a 0,mantem a mesma posicao.
							Vector3 tempPos = amigos [i].transform.position;
							tempPos.x += 0.5f; 
							amigos [i].transform.position = tempPos;
						} else {
							amigos [i].transform.position += (amigos [i].transform.position - amigos [j].transform.position).normalized * Time.deltaTime;
							amigos [j].transform.position += (amigos [j].transform.position - amigos [i].transform.position).normalized * Time.deltaTime;
						}
					}
				}
			}
		}
	}

	void DestroySentries(){
		foreach (GameObject sentry in amigos) {
			Destroy (sentry);
		}
		amigos.Clear ();
	}

	public void ResetPos(){
		transform.position = startPos;
	}

	protected Directions CalculateDirection(Vector3 vec){
		float x = vec.x;
		float z = vec.z;

		if (x < -0.3) { //left
			if (z < 0.3 && z > -0.3) {
				return Directions.W;
			} else if (z >= 0.3) {
				return Directions.NW;
			} else if (z <= -0.3) {
				return Directions.SW;
			}
		} else if (x > 0.3) { //right
			if (z < 0.3 && z > -0.3) {
				return Directions.E;
			} else if (z >= 0.3) {
				return Directions.NE;
			} else if (z <= -0.3) {
				return Directions.SE;
			}
		} else if (z > 0) { //up
			return Directions.N;
		} else if (z < 0) { //down
			return Directions.S;
		}
		//default
		return Directions.Unspecified;
	}

	IEnumerator FlashSprite(int noOfFlashes, float flashTime){
		for (int i = 0; i < 2 * noOfFlashes; i++) {
			spriteRenderer.enabled = !spriteRenderer.enabled;
			if (CurrentWeaponScript != null)
				CurrentWeaponScript.Toggle ();
			yield return new WaitForSeconds (flashTime);
		}

	}
}
