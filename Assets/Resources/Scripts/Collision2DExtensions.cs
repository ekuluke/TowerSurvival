using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Collision2DExtensions
{
    public static float GetImpactForce(this Collision2D col)
    {
        float impulse = 0f;
        ContactPoint2D[] contacts = new ContactPoint2D[col.contactCount];
        col.GetContacts(contacts);
        foreach(ContactPoint2D cp in contacts)
        {
            impulse += cp.normalImpulse;
        }
        return impulse / Time.fixedDeltaTime;
    } 
    
}
