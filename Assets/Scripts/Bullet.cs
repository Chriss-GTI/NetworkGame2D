using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public int damage = 25;
    public float bulletLifetime = 3f;

    private void Start()
    {
        if (!IsServer) return;
        {
            Invoke(nameof(ExpireBulletServerRpc), bulletLifetime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsServer)
        {

            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null && player != GetComponent<Player>())
            {
                player.TakeDamageServerRpc(damage);

                DespawnBulletServerRpc();
            }
        }
    }

    [Rpc(SendTo.Server)]
    private void DespawnBulletServerRpc()
    {
        if (NetworkObject != null && NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ExpireBulletServerRpc()
    {
        DespawnBulletServerRpc();
    }
}