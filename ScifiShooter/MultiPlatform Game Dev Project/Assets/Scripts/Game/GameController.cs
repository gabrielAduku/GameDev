﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

/* 
    GameController.cs handles player progression through the level, key management, and level completion / failure.
*/

public class GameController : MonoBehaviour
{
    #region Variables
    [SerializeField]
    public GameObject key;
    public int CurrentLevel;
    public GameObject enemy1;
    public GameObject enemy2;
    public GameObject enemy3;
    public GameObject enemy4;
    public int NumberOfTurrets;
    private GameObject player;
    public GameObject[] spawnPointsEnemies;
    public GameObject[] spawnPointsKey;
    private int currentWaveLevel;
    public Text YouWonText;
    public Text YouLostText;
    private bool isGameFinished = false;
    private bool playerLost = false;
    private bool allKeysCollected = false;
    private int spawnedKeys;
    public bool SecondWeaponUnlocked = false;
    public bool ThirdWeaponUnlocked = false;
    private const float SpawnRate = 0.25f;
    public GameObject bossSpawnPoint;
    private bool isBossDead;
    #endregion


    [NonSerialized]
    private int currentEnemyCount = 0;
    //public float countdown = 10;


    // On start, set everything to normal.
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentWaveLevel = 0;
        YouLostText.text = "";
        YouWonText.text = "";
        isGameFinished = false;
        currentEnemyCount = 0;
        allKeysCollected = false;
        spawnedKeys = 0;
    }

    // Generating keys at random spawn points.
    public void SpawnKey(int keysToSpawn)
    {
        Vector3 keyPosition = new Vector3(0, 0, 0);
        var sp = spawnPointsKey.ToList();
        // Randomly choose spawn points for every key
        for (int i = 0; i < keysToSpawn -1; i++) 
        {
            var index = UnityEngine.Random.Range(0, sp.Count() -1);
            Instantiate(key, sp.ElementAt(index).transform.position, Quaternion.identity);            
            spawnedKeys++;
            //Debug.Log("spawned " + spawnedKeys);
            sp.Remove(sp.ElementAt(index));
        }
        if (spawnPointsKey.Last() != null) //Adding the last key into the spawn point manually
        {
            var index = UnityEngine.Random.Range(0, -1);
            Instantiate(key, spawnPointsKey.Last().transform.position, Quaternion.identity);
            spawnedKeys++;
            //Debug.Log("spawned " + spawnedKeys);
        }

    }




    private void Update()
    {
        // Reset game if player has won or lost the game.
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("MainMenu");
        }
        if (isGameFinished && Input.GetKeyDown(KeyCode.Space))
        {
            isGameFinished = true;
            Start();

            Cursor.visible = false;
            if (CurrentLevel == 1 && playerLost)
            {
                SceneManager.LoadScene("Level 1");
            }
            else if(CurrentLevel == 2 && playerLost)
            {
                SceneManager.LoadScene("Level 2");                
            }
            else if (CurrentLevel == 3 && playerLost)
            {
                SceneManager.LoadScene("Level 3");
            }
            else if (CurrentLevel == 1 && !playerLost)
            {
                SceneManager.LoadScene("Level 2");
            }
            else if (CurrentLevel == 2 && !playerLost)
            {
                SceneManager.LoadScene("Level 3");
            }
            else if (CurrentLevel == 3 && !playerLost)
            {
                SceneManager.LoadScene("MainMenu");
            }

        }

        if (!isGameFinished && currentEnemyCount == 0 && currentWaveLevel >= 0 && currentWaveLevel < 3)
        {
            StartLevel(ref currentWaveLevel);
            Debug.Log(currentWaveLevel + " spawned");
            SecondWeaponUnlocked = true;
            player.GetComponent<PlayerStatController>().UpdateWaveText(currentWaveLevel);
            if (CurrentLevel == 2)
            {
                StartLevel(ref currentWaveLevel);
                Debug.Log(currentWaveLevel + " spawned");
                player.GetComponent<PlayerStatController>().UpdateWaveText(currentWaveLevel);

                StartLevel(ref currentWaveLevel);
                Debug.Log(currentWaveLevel + " spawned");
                player.GetComponent<PlayerStatController>().UpdateWaveText(currentWaveLevel);
                ThirdWeaponUnlocked = true;
            }

            if (CurrentLevel == 3 && currentWaveLevel == 1)
            {
                ThirdWeaponUnlocked = true;
                var ene = Instantiate(enemy4, bossSpawnPoint.transform.position, bossSpawnPoint.transform.rotation);
                ene.GetComponent<EnemyController>().StartEnemies(1);                
            }
        }


        if (!isGameFinished && currentEnemyCount == 0 && currentWaveLevel == 3)
        {
            if (currentEnemyCount < 0) 
            {
                Debug.Log("Check the enemy count it is bugged!");
            }

            if ((allKeysCollected && CurrentLevel != 3) || (allKeysCollected && CurrentLevel == 3 && isBossDead))
            {
                GameWon();
            }
            
            player.GetComponent<PlayerStatController>().UpdateWaveText(currentWaveLevel + 1);
            
        }
    }

    // Handles logic relating to starting the current level
    private void StartLevel(ref int currentWaveLevel)
    {
        currentWaveLevel++;

        player.GetComponent<PlayerStatController>().SetPlayerLevel(CurrentLevel, currentWaveLevel);

        //SpawnEnemies();
        foreach (var sp in spawnPointsEnemies)
        {
            if (sp == spawnPointsEnemies.First() || (UnityEngine.Random.Range(0, 1f) < (SpawnRate * currentWaveLevel)))
            {
                var enemyType = enemy1;
                if(CurrentLevel == 1)
                {
                    enemyType = UnityEngine.Random.Range(0, 1f) < 0.75 ? enemy1 : enemy2;
                }
                else if (CurrentLevel == 2)
                {
                    var randomFloat = UnityEngine.Random.Range(0, 1f);
                    if (randomFloat < 0.5f)
                    {
                        enemyType = enemy1;
                    }
                    else if(randomFloat < 0.85)
                    {
                        enemyType = enemy2;
                    }
                    else if (randomFloat < 1)
                    {
                        enemyType = enemy3;
                    }
                }
                else if (CurrentLevel == 3)
                {
                    var randomFloat = UnityEngine.Random.Range(0, 1f);
                    if (randomFloat < 0.3f)
                    {
                        enemyType = enemy1;
                    }
                    else if (randomFloat < 0.6)
                    {
                        enemyType = enemy2;
                    }
                    else if (randomFloat < 1)
                    {
                        enemyType = enemy3;
                    }
                }
                

                var ene = Instantiate(enemyType, sp.transform.position, sp.transform.rotation);
                ene.GetComponent<EnemyController>().StartEnemies(currentWaveLevel);
                currentEnemyCount++;
            }
        }

    }

    public void OnEnemyDestroyed(GameObject enemy)
    {
        //We might decrease the count of different enemy types.
        if (enemy.tag == "Turret")
        {
            return; //No need to decrease enemy count if it is a turret
        }
        else if (enemy.name.Contains("Boss"))
        {
            isBossDead = true;//No need to decrease enemy count if it is a bass
        }
        else
        {
            currentEnemyCount--;
        }
    }

    public void OnKeysCollected()
    {
        allKeysCollected = true;
    }

    private void GameWon()
    {
        player.GetComponent<PlayerStatController>().FreezePlayer();

        if (CurrentLevel == 3)
        {
            YouWonText.text = "You Won!\n" + "Total time spent: " + Time.time.ToString("#.##") + " secs\n" +
                "Press SPACE to return main menu";
        }
        else
        {
            YouWonText.text = "You cleared the level!\n" + "A new weapon unlocked!\n" + "Time spent on this level: " + Time.timeSinceLevelLoad.ToString("#.##") + " secs\n" +
            "Press SPACE to move to the next level";
        }

        isGameFinished = true;
        playerLost = false;

    }

    public void GameLost()
    {
        player.GetComponent<PlayerStatController>().FreezePlayer();
        
        YouLostText.text = "You Died!\nPress SPACE TO TRY AGAIN!\nPress BACKSPACE for main menu";
        //currentWaveLevel = 0;
        playerLost = true;
        isGameFinished = true;
    }
}
