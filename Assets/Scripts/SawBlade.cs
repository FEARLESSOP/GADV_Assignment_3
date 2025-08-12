using UnityEngine;

public class SawBlade : MonoBehaviour
{
    public float bladeSpeed = 10f;
    public Vector3 moveDirection = Vector3.right;
    public Transform playerRespawnPoint;

    public float maxTravelDistance = 20f;

    private Vector3 initialPosition;
    private Rigidbody2D bladeRigidbody;

    private void Start()
    {
        bladeRigidbody = GetComponent<Rigidbody2D>();
        bladeRigidbody.velocity = moveDirection.normalized * bladeSpeed;
        initialPosition = transform.position;
    }

    private void Update()
    {
        if (Vector3.Distance(initialPosition, transform.position) >= maxTravelDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = playerRespawnPoint.position;
            Destroy(gameObject);
        }
        else if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
