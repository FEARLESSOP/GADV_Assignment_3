using UnityEngine;

public class SawTrap : MonoBehaviour
{
    public GameObject sawBladePrefab;
    public Transform shootPoint;
    public float shootInterval = 3f;
    public Transform playerRespawnPoint;

    public Vector3 sawScale = Vector3.one; //set saw size

    private float shootTimer = 0f;

    private void Update()
    {
        //count time
        shootTimer += Time.deltaTime;

        //shoot blade if timer reached
        if (shootTimer >= shootInterval)
        {
            ShootSawBlade();
            shootTimer = 0f;
        }
    }

    void ShootSawBlade()
    {
        //get shoot direction
        Vector3 baseDirection = transform.right;
        Vector3 shootDirection = Quaternion.Euler(0, 0, 90) * baseDirection;

        //get rotation for blade
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        //spawn saw blade
        GameObject saw = Instantiate(sawBladePrefab, shootPoint.position, rotation);

        //set saw scale
        saw.transform.localScale = sawScale;

        //set saw direction and respawn
        SawBlade sawBladeScript = saw.GetComponent<SawBlade>();
        sawBladeScript.moveDirection = shootDirection;
        sawBladeScript.playerRespawnPoint = playerRespawnPoint;

        //ignore collision with trap
        Collider2D sawCollider = saw.GetComponent<Collider2D>();
        Collider2D trapCollider = GetComponent<Collider2D>();
        if (sawCollider != null && trapCollider != null)
        {
            Physics2D.IgnoreCollision(sawCollider, trapCollider);
        }
    }
}
