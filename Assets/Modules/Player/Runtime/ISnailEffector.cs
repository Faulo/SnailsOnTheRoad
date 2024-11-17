
namespace SotR.Player {
    public interface ISnailEffector {
        void EnterSnail(SnailController controller);
        void EffectSnail(SnailController controller);
        void ExitSnail(SnailController controller);
    }
}
