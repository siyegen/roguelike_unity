using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour {

    public float moveTime = 0.1f;
    public LayerMask blockingLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2d;
    private float inverseMoveTime;

	// Use this for initialization
	protected virtual void Start () {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
	}

    protected bool Move (int xDir, int yDir, out RaycastHit2D hit) {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform == null) {
            StartCoroutine(SmoothMovement(end));
            return true;
        }

        return false;
    }

    protected IEnumerator SmoothMovement (Vector3 end) {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon) {
            Vector3 newPos = Vector3.MoveTowards(rb2d.position, end, inverseMoveTime * Time.deltaTime);
            rb2d.MovePosition(newPos);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }

    protected virtual void AttemptMove<T>(int xDir, int yDir) where T : IHittable {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);
        if (hit.transform == null) {
            return;
        }

        Debug.Log("tt" + hit.transform.GetType());
        IHittable hitComponent = (IHittable)hit.transform.GetComponent(typeof(IHittable));
        if (!canMove && hitComponent != null) {
            if (hitComponent is Wall) {
                OnCantMove((Wall)hitComponent);
            } else if (hitComponent is Player) {
                OnCantMove((Player)hitComponent);
            } else if (hitComponent is Enemy) {
                OnCantMove((Enemy)hitComponent);
            }
        }

        //T hitComponent = hit.transform.GetComponent<T>();
        //if (!canMove && hitComponent != null) {
        //    OnCantMove(hitComponent);
        //}
    }

    protected abstract void OnCantMove<T>(T component) where T : IHittable;
}
