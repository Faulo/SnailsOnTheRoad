using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace Slothsoft.Aseprite {
    [Serializable]
    public sealed record AsepriteData {
        static readonly bool useUnityJson = false;
        public static AsepriteData FromJson(string json) {
            return useUnityJson
                ? JsonUtility.FromJson<AsepriteData>(json)
                : JsonConvert.DeserializeObject<AsepriteData>(json, new JsonSerializerSettings() {
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                });
        }
        public AsepriteData() {
        }
        public AsepriteDataFrame this[string frameName] {
            get {
                for (int i = 0; i < frames.Length; i++) {
                    if (frames[i].filename == frameName) {
                        return frames[i];
                    }
                }

                throw new ArgumentOutOfRangeException(frameName);
            }
        }
        [SerializeField, JsonProperty]
        public AsepriteDataFrame[] frames = Array.Empty<AsepriteDataFrame>();
        [SerializeField, JsonProperty]
        public AsepriteDataMeta meta = new();

        public bool Equals(AsepriteData other)
            => other is not null
            && frames.SequenceEqual(other.frames)
            && meta == other.meta;

        public override int GetHashCode() => HashCode.Combine(frames.Length, meta);

        public override string ToString() {
            var result = new StringBuilder();
            PrintMembers(result);
            PrintArray(result, frames);
            return result.ToString();
        }

        internal static void PrintArray<T>(StringBuilder builder, T[] values) {
            builder = builder.Replace(
                typeof(T).FullName + "[]",
                values.Length == 0
                    ? "[]"
                    : $"[ {string.Join(", ", values.Select(f => f.ToString()))} ]"
            );
        }

        internal bool HasLayer(string name) => meta.layers.Any(layer => layer.name == name);
    }
    [Serializable]
    public sealed record AsepriteDataFrame {
        [SerializeField, JsonProperty]
        public string filename = string.Empty;
        [SerializeField, JsonProperty]
        public AsepriteDataRect frame = new();
        [SerializeField, JsonProperty]
        public bool rotated;
        [SerializeField, JsonProperty]
        public bool trimmed;
        [SerializeField, JsonProperty]
        public AsepriteDataRect spriteSourceSize = new();
        [SerializeField, JsonProperty]
        public AsepriteDataSize sourceSize = new();
        [SerializeField, JsonProperty]
        public int duration;
    }
    [Serializable]
    public sealed record AsepriteDataMeta {
        [SerializeField, JsonProperty]
        public string version = string.Empty;
        [SerializeField, JsonProperty]
        public string format = string.Empty;
        [SerializeField, JsonProperty]
        public AsepriteDataSize size = new();
        [SerializeField, JsonProperty]
        public int scale;
        [SerializeField, JsonProperty]
        public AsepriteDataFrameTag[] frameTags = Array.Empty<AsepriteDataFrameTag>();
        [SerializeField, JsonProperty]
        public AsepriteDataLayer[] layers = Array.Empty<AsepriteDataLayer>();
        [SerializeField, JsonProperty]
        public AsepriteDataSlice[] slices = Array.Empty<AsepriteDataSlice>();

        public bool Equals(AsepriteDataMeta other)
            => other is not null
            && frameTags.SequenceEqual(other.frameTags)
            && layers.SequenceEqual(other.layers)
            && format == other.format
            && size == other.size
            && scale == other.scale;

        public override int GetHashCode() => HashCode.Combine(frameTags.Length, layers.Length, format, size, scale);

        public override string ToString() {
            var result = new StringBuilder();
            PrintMembers(result);
            AsepriteData.PrintArray(result, frameTags);
            AsepriteData.PrintArray(result, layers);
            AsepriteData.PrintArray(result, slices);
            return result.ToString();
        }
    }
    [Serializable]
    public sealed record AsepriteDataSize {
        [SerializeField, JsonProperty]
        public int w;

        [SerializeField, JsonProperty]
        public int h;

        public static implicit operator Vector2Int(AsepriteDataSize position) => new(position.w, position.h);
        public static implicit operator Vector2(AsepriteDataSize position) => new(position.w, position.h);
    }
    [Serializable]
    public sealed record AsepriteDataPosition {
        [SerializeField, JsonProperty]
        public int x;

        [SerializeField, JsonProperty]
        public int y;

        public AsepriteDataPosition() {
        }
        public AsepriteDataPosition(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public static implicit operator Vector2Int(AsepriteDataPosition position) => new(position.x, position.y);
        public static implicit operator Vector2(AsepriteDataPosition position) => new(position.x, position.y);
    }
    [Serializable]
    public sealed record AsepriteDataRect {
        [SerializeField, JsonProperty]
        public int x;

        [SerializeField, JsonProperty]
        public int y;

        [SerializeField, JsonProperty]
        public int w;

        [SerializeField, JsonProperty]
        public int h;

        public AsepriteDataRect() {
        }
        public AsepriteDataRect(int x, int y, int w, int h) {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }

        public static implicit operator RectInt(AsepriteDataRect rect) => new(rect.x, rect.y, rect.w, rect.h);
        public static implicit operator Rect(AsepriteDataRect rect) => new(rect.x, rect.y, rect.w, rect.h);
    }
    [Serializable]
    public sealed record AsepriteDataLayer {
        [SerializeField, JsonProperty]
        public string name = string.Empty;

        [SerializeField, JsonProperty]
        public int opacity;

        [SerializeField, JsonProperty]
        public string blendMode = string.Empty;

        [SerializeField, JsonProperty]
        public string data = string.Empty;
    }
    [Serializable]
    public sealed record AsepriteDataFrameTag {
        [SerializeField, JsonProperty]
        public string name = string.Empty;

        [SerializeField, JsonProperty]
        public int from;

        [SerializeField, JsonProperty]
        public int to;

        [SerializeField, JsonProperty]
        string direction;
        [JsonIgnore]
        public AsepriteDataFrameDirection directionValue => Enum.TryParse<AsepriteDataFrameDirection>(direction, out var result)
            ? result
            : AsepriteDataFrameDirection.forward;

        [SerializeField, JsonProperty]
        string color;
        [JsonIgnore]
        public Color colorValue => ColorUtility.TryParseHtmlString(color, out var result)
            ? result
            : Color.white;

        [SerializeField, JsonProperty]
        string repeat;
        [JsonIgnore]
        public bool isLooping => string.IsNullOrEmpty(repeat);
    }
    [Serializable]
    public sealed record AsepriteDataSlice {
        [SerializeField, JsonProperty]
        public string name = string.Empty;

        [SerializeField, JsonProperty]
        string color = string.Empty;
        [JsonIgnore]
        public Color colorValue => ColorUtility.TryParseHtmlString(color, out var result)
            ? result
            : Color.white;

        [SerializeField, JsonProperty]
        public string data = string.Empty;

        [SerializeField, JsonProperty]
        AsepriteDataSliceKey[] keys = Array.Empty<AsepriteDataSliceKey>();

        [JsonIgnore]
        public AsepriteDataRect bounds => keys[0].bounds;

        [JsonIgnore]
        internal AsepriteDataRect center => keys[0].center;

        [JsonIgnore]
        internal AsepriteDataPosition pivot => keys[0].pivot;

        public bool TryGetNineSlice(out Vector4 border) {
            if (center is null) {
                border = default;
                return false;
            }

            border = new Vector4 {
                // left
                x = center.x,
                // bottom
                y = bounds.h - center.y - center.h,
                // right
                z = bounds.w - center.x - center.w,
                // top
                w = center.y
            };
            return true;
        }

        public bool TryGetPivot(out Vector2 normalizedPivot) {
            if (pivot is null) {
                normalizedPivot = default;
                return false;
            }

            normalizedPivot = new(pivot.x, bounds.h - pivot.y);
            normalizedPivot.x /= bounds.w;
            normalizedPivot.y /= bounds.h;
            return true;
        }

        public AsepriteDataSlice() {
        }

        public AsepriteDataSlice(string name, string color, string data, AsepriteDataRect bounds, AsepriteDataRect center = default, AsepriteDataPosition pivot = default) {
            this.name = name;
            this.color = color;
            this.data = data;

            keys = new AsepriteDataSliceKey[] {
                new() {
                    bounds = bounds,
                    center = center,
                    pivot = pivot,
                }
            };
        }

        public bool Equals(AsepriteDataSlice other)
            => other is not null
            && keys.SequenceEqual(other.keys)
            && name == other.name
            && color == other.color
            && data == other.data;
        public override int GetHashCode() => HashCode.Combine(keys.Length, name, color, data);

        public override string ToString() {
            var result = new StringBuilder();
            PrintMembers(result);
            AsepriteData.PrintArray(result, keys);
            return result.ToString();
        }
    }
    [Serializable]
    public sealed record AsepriteDataSliceKey {
        [SerializeField, JsonProperty]
        public AsepriteDataRect bounds;
        [SerializeField, JsonProperty]
        public AsepriteDataRect center;
        [SerializeField, JsonProperty]
        public AsepriteDataPosition pivot;
    }
    public enum AsepriteDataFrameDirection {
        forward,
        pingpong,
        reverse
    }
}
