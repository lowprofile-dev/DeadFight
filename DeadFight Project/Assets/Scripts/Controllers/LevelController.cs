using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {

	public static LevelController Instance = null;
	void Awake(){
		if (Instance == null) Instance = this;
		else if (Instance != this) Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
		savedLevelMultiplier = levelMultiplier;
	}

	public float levelTimer = 1f;
	public int totalEnemies;
	public int enemiesToKill;
	public int enemiesSpawned;
	public int levelMultiplier = 5;
	public bool hub;
	public GameObject HubScene;
	private bool bossLevel;
	private bool doorsShown;
	public List<DoorController> Doors;

	private int level = 1;
	private int sublevel;
	private int savedLevelMultiplier;
	private GameController gameController;
	private PickupController pickupController;
	private EnemyController enemyController;
	private PlayerController playerController;

	void Start(){
		gameController = GameController.Instance;
		pickupController = PickupController.Instance;
		enemyController = EnemyController.Instance;
		playerController = PlayerController.Instance;
	}

	public void Update(){
		if (enemiesSpawned == totalEnemies)
			enemyController.StopSpawning ();
		if (enemiesToKill <= 0 && !hub && !doorsShown) {
			ShowDoors ();
			doorsShown = true;
		}
	}

	public void NewLevel(DoorTypes type, int lvl, int sublvl){
		if (gameController == null)
			Start ();

		HideDoors ();
		if (type == DoorTypes.hub) {
			//Volta á BASE
			bossLevel = false;
			hub = true;
			HubLevel ();
			Debug.Log ("BASE level");
		} else {
			hub = false;
			HubScene.SetActive(false);
			level = lvl;
			sublevel = sublvl;
			bossLevel = (sublevel == gameController.sublevelMax);
			levelMultiplier = savedLevelMultiplier * level;
			totalEnemies = sublevel * levelMultiplier;
			enemiesToKill = totalEnemies;
			enemiesSpawned = 0;
		}
		StartCoroutine (LevelStartTimer ());
	}

	public void EnemyKilled(){
		enemiesToKill--;
	}

	public void EnemySpawned(){
		enemiesSpawned++;
	}

	public void BossSpawned(){
		enemiesToKill = 1;
	}

	void LevelStart(){
		playerController.Show ();
		enemyController.LevelStart (level);
	}

	void LevelEnd(){
		//Termina a criaçao de Inimigos.
		enemyController.LevelEnd();
		pickupController.Clear ();
		gameController.LevelEnd ();
	}

	public void PlayerDoorCollide(DoorTypes type){
		gameController.LevelUp (type);
		LevelEnd ();
	}

	void ShowDoors(){
		//Esconde Primeiro todas as Portas.
		foreach (DoorController door in Doors)
			door.Hide ();

		int index = Random.Range (0, Doors.Count);

		Doors [index].SetType (DoorTypes.nextLevel);
		Doors [index].Show ();
		if (bossLevel) {
			int index2 = index;
			while (index == index2)
				index2 = Random.Range (0, Doors.Count);
			Doors [index2].SetType (DoorTypes.hub);
			Doors [index2].Show ();
		}
	}

	void HideDoors(){
		foreach (DoorController door in Doors) {
			door.Hide ();
		}
		doorsShown = false;
	}

	void HubLevel(){
		ShowDoors();

		HubScene.SetActive (true);
	}

	IEnumerator LevelStartTimer() {
		yield return new WaitForSeconds (levelTimer);
		if (hub)
			playerController.Show ();
		else
			LevelStart ();
	}

}
