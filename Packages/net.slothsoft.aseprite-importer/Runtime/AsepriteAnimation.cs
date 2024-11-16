using System;
using System.Collections.Generic;
using System.Linq;

namespace CursedBroom.Aseprite {
    public readonly struct AsepriteAnimation : IEquatable<AsepriteAnimation> {
        readonly (int time, int frame)[] keyframes;
        public readonly int duration;
        public readonly bool isLooping;
        public readonly bool isValid;

        public AsepriteAnimation(IEnumerable<(int time, int frame)> keyframes, bool isLooping) {
            this.keyframes = keyframes
                .ToArray();

            this.isLooping = isLooping;

            duration = this.keyframes
                .Sum(f => f.time);

            isValid = duration > 0;
        }

        public int GetFrameAtTime(int time) {
            if (isLooping) {
                time %= duration;
            } else {
                if (time >= duration) {
                    return keyframes[^1].frame;
                }
            }

            int offset = 0;
            for (int i = 0; i < keyframes.Length; i++) {
                int nextOffset = offset + keyframes[i].time;
                if (nextOffset > time) {
                    return keyframes[i].frame;
                }

                offset = nextOffset;
            }

            throw new ArgumentOutOfRangeException();
        }

        public bool Equals(AsepriteAnimation other) {
            if (isValid != other.isValid) {
                return false;
            }

            if (duration != other.duration) {
                return false;
            }

            if (isLooping != other.isLooping) {
                return false;
            }

            if ((keyframes is null) && (other.keyframes is null)) {
                return true;
            }

            if (keyframes.Length != other.keyframes.Length) {
                return false;
            }

            for (int i = 0; i < keyframes.Length; i++) {
                if (keyframes[i] != other.keyframes[i]) {
                    return false;
                }
            }

            return true;
        }
    }
}
