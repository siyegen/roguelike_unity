using UnityEngine;

public class Enemy : MovingObject, IHittable {

    public int playerDamage;

    private Animator animator;
    private Transform target;
    private bool skipMove;
    public int hp = 6;
    
    protected override void Start () {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        SetHp(6);
        base.Start();
	}

    // protected override void AttemptMove<T>(int xDir, int yDir) {
    //     if (skipMove) {
    //         skipMove = false;
    //         return;
    //     } 

    //     base.AttemptMove<T>(xDir, yDir);

    //     skipMove = true;
    // }

    public void MoveEnemy() {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon) {
            yDir = target.position.y > transform.position.y ? 1 : -1;
        } else {
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }

        AttemptMove<IHittable>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component) {
        animator.SetTrigger("enemyAttack");
        component.TakeDamage(playerDamage);
    }

    public void SetHp(int HP) {
        hp = HP;
    }

    public bool TakeDamage(int loss) {
        Debug.Log("bam");
        hp -= loss;
        if (hp <= 0) {
            gameObject.SetActive(false);
            return true;
        }
        return false;
    }
}
