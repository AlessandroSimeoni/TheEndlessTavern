using UnityEngine;

namespace TillLastTooth
{
    public class TillLastToothSpawner : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private GameObject playerPrefab = null;
        [SerializeField] private PlayerSettings playerSettings = null;
        [SerializeField] private AudioClip playerDodgeSFX = null;
        [Header("Enemy")]
        [SerializeField] private Enemy enemyPrefab = null;

        public Player SpawnPlayer(Vector3 position, Quaternion rotation)
        {
            GameObject playerInstance = Instantiate(playerPrefab, position, rotation);
            playerInstance.name = "Player";

            Player player = playerInstance.AddComponent<Player>();
            player.settings = playerSettings;
            player.dodgeSFX = playerDodgeSFX;

            return player;
        }
        
        public Enemy SpawnEnemy(Vector3 position) => Instantiate<Enemy>(enemyPrefab, position, enemyPrefab.transform.rotation);
    }
}
