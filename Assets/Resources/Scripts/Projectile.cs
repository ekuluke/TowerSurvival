using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float baseDamage = 1;

    public float BaseDamage { get => baseDamage; set => baseDamage = value; }

    public void MoveTo(Vector2 force)
    {
        this.gameObject.GetComponent<Rigidbody2D>().AddForce(force);
        print("hi");
    }
        
}
