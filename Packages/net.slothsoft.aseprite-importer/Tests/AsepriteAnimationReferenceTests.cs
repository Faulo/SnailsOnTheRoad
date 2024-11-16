using NUnit.Framework;

namespace CursedBroom.Aseprite.Tests {
    [TestFixture(TestOf = typeof(AsepriteAnimationReference))]
    sealed class AsepriteAnimationReferenceTests {
        [TestCase(TestAsset.Animator, "Resting")]
        [TestCase(TestAsset.Animator, "FlyingUp")]
        [TestCase(TestAsset.Animator, "FlyingEven")]
        [TestCase(TestAsset.Animator, "FlyingDown")]
        public void GivenSourceAndTag_WhenCallResolve_ThenReturnAnimation(TestAsset asset, string animation) {
            var aseprite = new TestInfo().GetAsset<AsepriteFile>(asset);
            var expected = aseprite.GetAnimation(animation);

            var sut = new AsepriteAnimationReference() {
                source = aseprite,
                animation = animation
            };

            Assert.That(sut.Resolve(), Is.EqualTo(expected));
        }

        [TestCase(TestAsset.Animator, "Resting")]
        [TestCase(TestAsset.Animator, "FlyingUp")]
        [TestCase(TestAsset.Animator, "FlyingEven")]
        [TestCase(TestAsset.Animator, "FlyingDown")]
        public void GivenSourceAndTag_WhenCallToString_ThenReturnAnimation(TestAsset asset, string animation) {
            var aseprite = new TestInfo().GetAsset<AsepriteFile>(asset);

            var sut = new AsepriteAnimationReference() {
                source = aseprite,
                animation = animation
            };

            Assert.That(sut.ToString(), Is.EqualTo(animation));
        }
    }
}
