using System.Collections.Generic;
using NUnit.Framework;

namespace CursedBroom.Aseprite.Tests {
    [TestFixture(TestOf = typeof(AsepriteAnimation))]
    sealed class AsepriteAnimationTests {
        IEnumerable<(int, int)> testAnimation => new[] {
            (100, 0),
            (100, 1),
            (200, 2),
            (50, 1),
        };

        [TestCase(false, -200, 0)]
        [TestCase(false, 0, 0)]
        [TestCase(false, 99, 0)]
        [TestCase(false, 100, 1)]
        [TestCase(false, 200, 2)]
        [TestCase(false, 450, 1)]
        [TestCase(false, 1000, 1)]
        [TestCase(true, -200, 0)]
        [TestCase(true, 0, 0)]
        [TestCase(true, 200, 2)]
        [TestCase(true, 399, 2)]
        [TestCase(true, 400, 1)]
        [TestCase(true, 449, 1)]
        [TestCase(true, 450, 0)]
        [TestCase(true, 200 + 450, 2)]
        [TestCase(true, 399 + 900, 2)]
        [TestCase(true, 400 + 4500, 1)]
        [TestCase(true, 449 + 9000, 1)]
        public void GivenAsepriteAnimation_WhenGetFrameAtTime_ThenReturnFrame(bool isLooping, int time, int expectedFrame) {
            var animation = new AsepriteAnimation(testAnimation, isLooping);

            int actualFrame = animation.GetFrameAtTime(time);

            Assert.That(actualFrame, Is.EqualTo(expectedFrame));
        }

        [Test]
        public void GivenAsepriteFile_WhenCallGetAnimation_ThenReturnAnimation() {
            var keyframes = new[] {
                (150, 0),
                (100, 1),
                (100, 2),
                (150, 3),
            };
            var expected = new AsepriteAnimation(keyframes, true);

            var sut = new TestInfo().GetAsset<AsepriteFile>(TestAsset.Animator);
            var actual = sut.GetAnimation("Resting");

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
