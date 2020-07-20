using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Pickup : MonoBehaviour {

	public PickupTypes type;
	public float value = -0.02f;
	public string displayText;
	public GameObject SpawnPrefab;
	public bool weapon;
	[Range(0f,100f)]public int RelativeRarity;

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.CompareTag("Player")){
			//Colisao com Jogador
			PickupController.Instance.ShowPickupText(gameObject, type, transform.position, displayText);
		}
	}

	public Sprite GetSprite(){
		return GetComponentInChildren<SpriteRenderer> ().sprite;
	}
}
