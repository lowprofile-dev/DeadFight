using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour {

	public Pickup item;
	public Image icon;

	public void AddItem(Pickup item){
		this.item = item;
		icon.sprite = this.item.GetSprite ();
		icon.enabled = true;
	}
}
