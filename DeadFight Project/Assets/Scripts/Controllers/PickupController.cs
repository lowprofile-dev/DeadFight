using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour {
	
	public static PickupController Instance = null;
	void Awake(){
		if (Instance == null) Instance = this;
		else if (Instance != this) Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	[Range(0f, 100f)]public int DropChance;

	public List<Pickup> Pickups;
	public GameObject PickupTextPrefab;
	public float PickupTextDespawnTime = 1f;

	private List<GameObject> spawnedTexts = new List<GameObject>();
	private List<GameObject> spawnedPickups = new List<GameObject>();

	private int activeSentryPickups = 0;

	public void Clear(){
		StopAllCoroutines ();
		foreach (GameObject p in spawnedPickups)
			Destroy (p);
		foreach (GameObject text in spawnedTexts)
			Destroy (text);

		spawnedPickups.Clear ();
		spawnedTexts.Clear ();

		activeSentryPickups = 0;
	}

	public void NewPickup(Vector3 position){
		if (Random.Range (0, 100) <= DropChance) {
			//Ve se o Inimigo dropou um Item de acordo com a Chance de Drop.
			PickupTypes type = RandomPickupType();
			Debug.Log("pickup: " + type.ToString());
			spawnedPickups.Add(Instantiate (Pickups.Find(p => p.type == type).gameObject, position, Quaternion.identity));
		}
	}

	public void ShowPickupText(GameObject pickup, PickupTypes type, Vector3 position, string text){
		if (type == PickupTypes.sentry)
			activeSentryPickups--;
		spawnedPickups.Remove (pickup);
		Destroy (pickup);
		GameObject newPickupText = Instantiate (PickupTextPrefab, position, Quaternion.Euler (new Vector3 (90, 0, 0)));
		newPickupText.GetComponent<TextMesh> ().text = text;
		spawnedTexts.Add (newPickupText);
		StartCoroutine (PickupTextTimer ());
	}

	PickupTypes RandomPickupType(){
		//Item random de acordo com a Drop Chance
        //remove a arma atual da lista temporaria para nao dropar repetido
		//exclui items de raridade 0
		List<Pickup> temp = new List<Pickup>();
		int weight = 0;
		foreach (Pickup p in Pickups) {
			if (p.RelativeRarity > 0 && p.type != PlayerController.Instance.CurrentWeapon.GetComponent<Weapon> ().PickupType) {
				//adiciona o item apanhado á arma atual.

				if (p.type != PickupTypes.sentry || (p.type == PickupTypes.sentry && DropSentry ())) {
					//adiciona se nao for um Amigo, se for confirma e ve se ja nao existe.
					temp.Add (p);
					weight += p.RelativeRarity;
				}

			}
		}
		
		int rand = Random.Range (0, weight);
		foreach (Pickup p in temp) {
			if (rand <= p.RelativeRarity) {
				if (p.type == PickupTypes.sentry)
					activeSentryPickups++;
				return p.type;
			}
			rand -= p.RelativeRarity;
		}

		//Isto nunca deve acontecer previne erros de compilaçao.
		return (PickupTypes) 0;
	}

	bool DropSentry(){
		int activeSentries = PlayerController.Instance.SentryCount ();

		if (activeSentries + activeSentryPickups < PlayerController.Instance.maxSentries)
			return true;
		return false;

	}

	IEnumerator PickupTextTimer(){
		yield return new WaitForSeconds (PickupTextDespawnTime);
		Destroy (spawnedTexts[0]);
		spawnedTexts.RemoveAt (0);
	}
}
