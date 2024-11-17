using TMPro;
using UnityEngine;

namespace SotR.Game {
    sealed class TimerDisplay : MonoBehaviour {
        [SerializeField]
        GameSettings settings;
        [SerializeField]
        TextMeshProUGUI text;

        void Update() {
            text.text = settings.timer.ToString(".0") + "s";
            text.enabled = settings.state is LevelState.Racing;
        }
    }
}
