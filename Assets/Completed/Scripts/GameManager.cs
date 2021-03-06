﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [HideInInspector]
    public static Camera mainCamera = null;
    [HideInInspector]
    public static Player mainPlayer = null;

    public float turnDelay = .1f;
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints = 100;
    [HideInInspector]
    public bool playersTurn = true;

    private bool firstInit = true;

    private Text levelText;
    private GameObject levelImage;
    private int level = 4;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;
    private float levelStartDelay = 1.5f;

    // Use this for initialization
    void Awake () {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        Debug.Log("columns " + boardScript.columns);
        InitGame();
	}

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        if (!firstInit) {
            string msg = scene.ToString();
            Debug.Log("Called finishedLoading Level " + msg);
            level++;
            InitGame();
        } else {
            firstInit = false;
        }
    }

    // XXX: WTF
    void SetupCameraPlayer() {
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        if (mainCamera == null) {
            Debug.Log("no camera");
        }
        mainPlayer = GameObject.FindWithTag("Player").GetComponent<Player>();
        if (mainPlayer == null) {
            Debug.Log("no player");
        }
    }

    void OnEnable() {
        Debug.Log("OnEnable, adding sceneload");
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable() {
        Debug.Log("OnDisable, rming sceneload");
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void InitGame() {
        doingSetup = true;
        SetupCameraPlayer();
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardScript.SetupScene(level);
    }

    private void HideLevelImage() {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public void GameOver() {
        levelText.text = "After " + level + " days, you starved";
        levelImage.SetActive(true);
        enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        mainCamera.transform.position = new Vector3(mainPlayer.transform.position.x, mainPlayer.transform.position.y, mainCamera.transform.position.z);
        if (playersTurn || enemiesMoving || doingSetup) {
            return;
        }

        StartCoroutine(MoveEnemies());
	
	}

    public void AddEnemyToList(Enemy script) {
        Debug.Log("adding enemy");
        enemies.Add(script);
    }

    IEnumerator MoveEnemies() {
        enemiesMoving = true;
        // yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0) {
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i=0; i < enemies.Count; i++) {
            if( enemies[i].isActiveAndEnabled) {
                enemies[i].MoveEnemy();
                yield return new WaitForSeconds(enemies[i].moveTime);
            }
            
        }

        playersTurn = true;
        enemiesMoving = false;
    }
}
