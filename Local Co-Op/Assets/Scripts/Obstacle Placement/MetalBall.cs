using UnityEngine;

public class MetalBall : MonoBehaviour
{
    [Header("Swing Settings")]
    public float amplitude = 30f;
    public float swingSpeed = 2f;
    public float phaseOffset = 0f;

    [Header("Collision Settings")]
    public float ballForce = 10f;

    [Header("References")]
    public Transform ballTransform;

    private float _initialAngle;

    private void Start()
    {
        _initialAngle = transform.eulerAngles.z;
        if(ballTransform == null)
        {
            Debug.LogError("Please assign the ballTransform (child with the collider) in the inspector.");
        }
    }

    private void Update()
    {
        float angle = _initialAngle + amplitude * Mathf.Sin(Time.time * swingSpeed + phaseOffset);
        Vector3 euler = transform.eulerAngles;
        euler.z = angle;
        transform.eulerAngles = euler;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null && ballTransform != null)
            {
                Vector2 impactDirection = ((Vector2)collision.contacts[0].point - (Vector2)ballTransform.position).normalized;
                playerRb.AddForce(impactDirection * ballForce, ForceMode2D.Impulse);
            }
        }
    }
}
