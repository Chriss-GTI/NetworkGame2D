using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    public float movementSpeed;
    public GameObject bulletPrefab;
    public float bulletSpeed;
    public float shootingCooldown = 0.5f;
    public Vector2 shootDirection;
    public Transform BulletSpawnPos;

    private Rigidbody2D rb;
    private NetworkVariable<Vector2> moveInput = new NetworkVariable<Vector2>();
    private float Health = 100f;
    public Vector2 respawnPos;
    public float respawnDelay = 1f;

    private float lastShootTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        respawnPos = transform.position;
    }

    void Update()
    {
        if (IsLocalPlayer)
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            MoveServerRpc(input);
            rb.velocity = input * movementSpeed;

            if (Input.GetMouseButtonDown(0) && Time.time - lastShootTime > shootingCooldown)
            {
                lastShootTime = Time.time;
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                ShootServerRpc(mousePosition);
            }
        }
        else
        {
            rb.velocity = moveInput.Value * movementSpeed;
        }
    }

    [Rpc(SendTo.Server)]
    private void MoveServerRpc(Vector2 input)
    {
        moveInput.Value = input;
    }

    [Rpc(SendTo.Server)]
    private void ShootServerRpc(Vector2 mousePosition)
    {
        ShootServerSide(mousePosition);
    }

    private void ShootServerSide(Vector2 mousePosition)
    {
        if (bulletPrefab != null)
        {
            shootDirection = (mousePosition - (Vector2)transform.position).normalized;

            float offsetDistance = 0.5f;
            Vector2 shootPosition = (Vector2)transform.position + shootDirection * offsetDistance;

            GameObject bullet = Instantiate(bulletPrefab, shootPosition, Quaternion.identity);

            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.velocity = shootDirection * bulletSpeed;

                float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
                bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            }

            NetworkObject bulletNetworkObject = bullet.GetComponent<NetworkObject>();
            if (bulletNetworkObject != null)
            {
                bulletNetworkObject.Spawn(true);
            }
        }
    }

    [Rpc(SendTo.Server)]
    public void TakeDamageServerRpc(float damage)
    {
        if (!IsServer) return;

        Health -= damage;

        if (Health <= 0)
        {
            Health = 0;
            DespawnAndRespawnPlayerServerRpc();
        }
    }

    [Rpc(SendTo.Server)]
    private void DespawnAndRespawnPlayerServerRpc()
    {
        if (!IsServer) return;

        DeactivatePlayerClientRpc();

        Invoke(nameof(RespawnPlayerClientRpc), respawnDelay);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DeactivatePlayerClientRpc()
    {
        gameObject.SetActive(false);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void RespawnPlayerClientRpc()
    {
        Health = 100;
        transform.position = respawnPos;
        movementSpeed = 5;

        gameObject.SetActive(true);
    }
}