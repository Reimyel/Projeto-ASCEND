using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChanging : MonoBehaviour
{
    [Header("Armas")]
    public GameObject pistol;
    public GameObject machinegun;
    public GameObject shotgun;
    public GameObject minigun;
    public GameObject grenadeLauncher;
    GameObject currentWeapon;
    GameObject lastWeapon;

    [Header("Sprites das Armas")]
    public GameObject pistolUI;
    public GameObject machinegunUI;
    public GameObject shotgunUI;
    public GameObject minigunUI;
    public GameObject grenadeLauncherUI;
    GameObject currentWeaponUI;
    GameObject lastWeaponUI;

    void Start()
    {
        currentWeapon = pistol;
        lastWeapon = pistol;

        currentWeaponUI = pistolUI;
        lastWeaponUI = pistolUI;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(pistol);
            SwitchWeaponUI(pistolUI);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(machinegun);
            SwitchWeaponUI(machinegunUI);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchWeapon(shotgun);
            SwitchWeaponUI(shotgunUI);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchWeapon(minigun);
            SwitchWeaponUI(minigunUI);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchWeapon(grenadeLauncher);
            SwitchWeaponUI(grenadeLauncherUI);
        }
        else if (Input.GetKeyDown(KeyCode.Q)) //Arma anterior
        {
            SwitchWeapon(lastWeapon);
            SwitchWeaponUI(lastWeaponUI);
        }
    }

    void SwitchWeapon(GameObject newWeapon)
    {
        currentWeapon.SetActive(false);
        lastWeapon = currentWeapon;
        currentWeapon = newWeapon;
        currentWeapon.SetActive(true);
    }

    void SwitchWeaponUI(GameObject newWeaponUI)
    {
        currentWeaponUI.SetActive(false);
        lastWeaponUI = currentWeaponUI;
        currentWeaponUI = newWeaponUI;
        currentWeaponUI.SetActive(true);
    }
}
