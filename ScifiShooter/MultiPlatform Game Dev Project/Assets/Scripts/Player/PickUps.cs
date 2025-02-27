﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class PickUps : MonoBehaviour
{
    #region Variables
    // Stores amount of keys player currently has
    private int haveKey;

    // Stores amount of keys player needs
    public int requiredKeyCount;

    // UI element and sound
    public Text keyCount;
    public AudioSource pickUpSound;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        haveKey = 0;        
        keyCount.text = "";
        SetCountText();
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().SpawnKey(requiredKeyCount);
    }


    // If PlayerController collides with something
    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collided ==================================== " + other.gameObject.name);
        // Handles Key Pickup
        if (other.gameObject.tag == "PickUpKey")
        {
            other.gameObject.SetActive(false);
            haveKey++;
            SetCountText();
            if (haveKey == requiredKeyCount)
            {
                GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().OnKeysCollected();
            }
            pickUpSound.Play();
        }
        // Handles Health Pickup
        else if (other.gameObject.tag == "PickUpHealth")
        {
            other.gameObject.SetActive(false);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStatController>().OnHealthPickedUp();
            pickUpSound.Play();
        }
        // Handles Ammo Pickup
        else if (other.gameObject.tag == "PickUpAmmo")
        {
            other.gameObject.SetActive(false);
            
            if(GameObject.FindGameObjectWithTag("Weapon").GetComponent<PistolShoot>() != null)
            {
                GameObject.FindGameObjectWithTag("Weapon").GetComponent<PistolShoot>().OnAmmoPickedUp();
            }

            if (GameObject.FindGameObjectWithTag("Weapon").GetComponent<ShotgunShoot>() != null)
            {
                GameObject.FindGameObjectWithTag("Weapon").GetComponent<ShotgunShoot>().OnAmmoPickedUp();
            }

            if (GameObject.FindGameObjectWithTag("Weapon").GetComponent<MachineGunShoot>() != null)
            {
                GameObject.FindGameObjectWithTag("Weapon").GetComponent<MachineGunShoot>().OnAmmoPickedUp();
            }
            
          
            pickUpSound.Play();
        }
        else if (other.gameObject.tag == "Water")
        {
            gameObject.GetComponent<PlayerStatController>().TakeDamage(1000000);
        }

    }

    // UI Update Method
    private void SetCountText()
    {
        keyCount.text = "Energy Cells Collected: " + haveKey.ToString() + " / "+ requiredKeyCount.ToString();
    }
}
