using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;

public class Player : MovingObject {

    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsperSoda = 20;
    public float restartLevelDelay = 1f;
    public Text foodScore;

    private Animator animator;
    private int food;

	// Use this for initialization
	protected override void Start () {
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints;
        foodScore.text = "Food: " + food;
        base.Start();
	}

    private void OnDisable() {
        GameManager.instance.playerFoodPoints = food;
    }
	
	// Update is called once per frame
	void Update () {
        if (!GameManager.instance.playersTurn) return;

        int hor = 0, vert = 0;

        hor = (int)Input.GetAxisRaw("Horizontal");
        vert = (int)Input.GetAxisRaw("Vertical");
        if (hor != 0) vert = 0;

        if (hor != 0 || vert != 0) {
            AttemptMove<Wall>(hor, vert);
        }
	}

    protected override void AttemptMove<T>(int xDir, int yDir) {
        food--;
        foodScore.text = "Food: " + food;

        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;

        if (Move(xDir,yDir, out hit)) {

        }

        CheckIfGameOver();

        GameManager.instance.playersTurn = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Exit") {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        } else if (other.tag == "Food") {
            food += pointsPerFood;
            other.gameObject.SetActive(false);
            foodScore.text = "+" + pointsPerFood + " Food: " + food;
        } else if (other.tag == "Soda") {
            food += pointsperSoda;
            other.gameObject.SetActive(false);
            foodScore.text = "+" + pointsperSoda + " Food: " + food;
        }
    }

    protected override void OnCantMove<T>(T component) {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }

    private void Restart() {
        SceneManager.LoadScene(0);
    }

    public void LoseFood (int loss) {
        animator.SetTrigger("playerHit");
        food -= loss;
        foodScore.text = "-" + loss + " Food: " + food;
        CheckIfGameOver();
    }

    private void CheckIfGameOver() {
        if (food <=0) {
            GameManager.instance.GameOver();
        }
    }
}
