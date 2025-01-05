/// <summary>
/// WeaponSystem.cs
/// Author: MutantGopher
/// This script manages weapon switching.  It's recommended that you attach this to a parent GameObject of all your weapons, but this is not necessary.
/// This script allows the player to switch weapons in two ways, by pressing the numbers corresponding to each weapon, or by scrolling with the mouse.
/// </summary>

using UnityEngine;
using System.Collections;

public class WeaponSystem : MonoBehaviour
{
	public GameObject[] weapons;				// The array that holds all the weapons that the player has
	public int startingWeaponIndex = 0;			// The weapon index that the player will start with
	private int weaponIndex;					// The current index of the active weapon


	// Use this for initialization
	void Start()
	{
		// Make sure the starting active weapon is the one selected by the user in startingWeaponIndex
		weaponIndex = startingWeaponIndex;
		SetActiveWeapon(weaponIndex);
	}

	public void SetActiveWeapon(int index)
	{
		// Make sure this weapon exists before trying to switch to it
		if (index >= weapons.Length || index < 0)
		{
			Debug.LogWarning("Tried to switch to a weapon that does not exist.  Make sure you have all the correct weapons in your weapons array.");
			return;
		}

		// Send a messsage so that users can do other actions whenever this happens
		SendMessageUpwards("OnEasyWeaponsSwitch", SendMessageOptions.DontRequireReceiver);

		// Make sure the weaponIndex references the correct weapon
		weaponIndex = index;

		// Start be deactivating all weapons
		for (int i = 0; i < weapons.Length; i++)
		{
			weapons[i].SetActive(false);
		}

		// Activate the one weapon that we want
		weapons[index].SetActive(true);
	}

	public void NextWeapon()
	{
		weaponIndex++;
		if (weaponIndex > weapons.Length - 1)
			weaponIndex = 0;
		SetActiveWeapon(weaponIndex);
	}

	public void PreviousWeapon()
	{
		weaponIndex--;
		if (weaponIndex < 0)
			weaponIndex = weapons.Length - 1;
		SetActiveWeapon(weaponIndex);
	}
}
