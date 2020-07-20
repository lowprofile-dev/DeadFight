using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class InventoryManagement : MonoBehaviour {

	public Pickup defaultItem;
	public List<ItemSlot> bankSlots = new List<ItemSlot> ();
	public ItemSlot inventorySlot;

	// Use this for initialization
	void Start () {
		//Se falhou abrir o Banco de Items (nenhuns dados sao gravados)
		if (!LoadBank())
			foreach (ItemSlot itemSlot in bankSlots)
				itemSlot.AddItem (defaultItem);
	}

	public void Show(){
		//adiciona o item atual ao inventario.
		inventorySlot.AddItem (ConvertCurrentWeaponToPickup());
	}

	Pickup ConvertCurrentWeaponToPickup(){
		foreach(Pickup p in PickupController.Instance.Pickups)
			if (p.type == PlayerController.Instance.CurrentWeaponScript.PickupType)
				return p;
		//Nunca deve acontecer.
		return PickupController.Instance.Pickups[0];
	}

	public void ItemSwap(int index){
		Pickup temp = bankSlots [index].item;
		bankSlots [index].AddItem (inventorySlot.item);
		inventorySlot.AddItem (temp);
		PlayerController.Instance.CollectPickup (temp);

		SaveBank ();
	}

	[Serializable]
	class BankData{
		public List<PickupTypes> BankItems;
		public BankData(List<ItemSlot> bankSlots){
			BankItems = new List<PickupTypes>();
			foreach(ItemSlot itemSlot in bankSlots)
				BankItems.Add(itemSlot.item.type);
		}
	}

	void SaveBank(){
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/SavedBank.dat");
		BankData SaveData = new BankData (bankSlots);

		bf.Serialize (file, SaveData);
		file.Close ();
	}

	bool LoadBank(){
		if (File.Exists (Application.persistentDataPath + "/SavedBank.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/SavedBank.dat", FileMode.Open);
			BankData SaveData = (BankData)bf.Deserialize (file);
			file.Close ();

			for (int i = 0; i < SaveData.BankItems.Count; i++)
				bankSlots [i].AddItem (PickupController.Instance.Pickups.Find(p => p.type == SaveData.BankItems[i]));
			
			return true;
		} else
			return false;
	}
}
