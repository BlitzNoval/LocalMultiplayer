// Title: The ULTIMATE 2D Character CONTROLLER in UNITY
// Author: Shinjingi
// Date: 01 March  2025
// Availability: https://www.youtube.com/watch?v=lcw6nuc2uaU

using UnityEngine;

public class CollisionDataRetriever : MonoBehaviour
{
    public bool OnGround { get; private set; }
    public bool OnWall { get; private set; }
    public float Friction { get; private set; }
    public Vector2 ContactNormal { get; private set; }

    private PhysicsMaterial2D _material;

    private void OnCollisionExit2D(Collision2D collision)
    {
        OnGround = false;
        Friction = 0;
        OnWall = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EvaluateCollision(collision);
        RetrieveFriction(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        EvaluateCollision(collision);
        RetrieveFriction(collision);
    }

    public void EvaluateCollision(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactNormal = collision.GetContact(i).normal;
            OnGround |= ContactNormal.y >= 0.9f;
            OnWall = Mathf.Abs(ContactNormal.x) >= 0.9f;
        }
    }

    private void RetrieveFriction(Collision2D collision)
    {
        Rigidbody2D otherRb = collision.rigidbody;
        if (otherRb != null)
        {
            _material = otherRb.sharedMaterial;
            Friction = (_material != null) ? _material.friction : 0;
        }
        else
        {
            Friction = 0;
        }
    }
}
