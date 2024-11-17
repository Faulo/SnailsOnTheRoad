using SotR.Player;
using UnityEngine;

namespace SotR.Game {
    sealed class ProfileField : MonoBehaviour, ISnailEffector {
        public void EnterSnail(SnailController controller) {
            Debug.Log("oily!");
        }
        public void EffectSnail(SnailController controller) {

        }
        public void ExitSnail(SnailController controller) {
            Debug.Log("no oil");
        }
    }
}
