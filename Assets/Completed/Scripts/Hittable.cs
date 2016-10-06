using UnityEngine;
using System.Collections;

public interface IHittable{
    void SetHp(int HP);
    bool TakeDamage(int loss);
}
