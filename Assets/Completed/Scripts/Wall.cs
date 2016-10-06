using UnityEngine;
using System.Collections;
using System;

public class Wall : MonoBehaviour, IHittable {

    public Sprite dmgSprite;
    public int hp = 4;

    private SpriteRenderer spriteRenderer;

    // Use this for initialization
    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start() {
        SetHp(3);
    }

    public void SetHp(int HP) {
        hp = HP;
    }

    public bool TakeDamage(int loss) {
        spriteRenderer.sprite = dmgSprite;
        hp -= loss;
        if (hp <= 0) {
            gameObject.SetActive(false);
            return true;
        }
        return false;
    }
}
