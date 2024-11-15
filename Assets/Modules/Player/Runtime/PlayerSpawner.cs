using UnityEngine;

namespace SitS.Player {
    sealed class PlayerSpawner : MonoBehaviour {
        [SerializeField]
        PlayerModel player;

        void Update() {
            if (player) {
                player.SpawnIfNotSpawnedAlready(transform);
            }
        }
    }
}
