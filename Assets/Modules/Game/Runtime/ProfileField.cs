using Slothsoft.UnityExtensions;
using SotR.Player;
using UnityEngine;

namespace SotR.Game {
    sealed class ProfileField : MonoBehaviour, ISnailEffector {
        [SerializeField, Expandable]
        ProfileModel profile;
        [SerializeField]
        float profileGain = 1;

        public void EnterSnail(SnailController controller) {
        }
        public void EffectSnail(SnailController controller) {
            controller.snail.AddProfile(profile, Time.deltaTime * profileGain);
        }
        public void ExitSnail(SnailController controller) {
        }
    }
}
