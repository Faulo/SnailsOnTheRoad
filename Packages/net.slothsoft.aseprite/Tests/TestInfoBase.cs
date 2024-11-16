using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.Aseprite.Tests {
    public abstract class TestInfoBase<T> where T : struct {
        public static readonly IEnumerable<T> testAssets = Enum
            .GetValues(typeof(T))
            .Cast<T>();

        protected abstract string GetAssetPath(T asset);

        public TAsset GetAsset<TAsset>(T asset) where TAsset : UnityObject {
            var result = AssetDatabase.LoadAssetAtPath<TAsset>(GetAssetPath(asset));
            Assert.IsTrue(result, $"Failed to load asset {asset} of type {typeof(TAsset)} at '{GetAssetPath(asset)}'!");
            return result;
        }

        public AssetReferenceT<TAsset> GetAssetReference<TAsset>(T asset) where TAsset : UnityObject {
            return new AssetReferenceT<TAsset>(GetGUID(asset));
        }

        public string GetGUID(T asset) {
            return AssetDatabase.AssetPathToGUID(GetAssetPath(asset));
        }

        public FileInfo GetFile(T asset) {
            return new(GetAssetPath(asset));
        }

        [Test]
        public void TestGetAssetPath([ValueSource(nameof(testAssets))] T asset) {
            string path = GetAssetPath(asset);
            FileAssert.Exists(path, $"Failed to verify asset path {path}!");
        }

        [Test]
        public void TestGetFile([ValueSource(nameof(testAssets))] T asset) {
            var file = GetFile(asset);
            FileAssert.Exists(file, $"Failed to find file {asset}!");
        }

        [Test]
        public void TestGetAsset([ValueSource(nameof(testAssets))] T asset) {
            var obj = GetAsset<UnityObject>(asset);
            Assert.IsTrue(obj, $"Failed to load asset {asset}!");
        }

        [Test]
        public void TestGetGUID([ValueSource(nameof(testAssets))] T asset) {
            string guid = GetGUID(asset);
            Assert.IsTrue(GUID.TryParse(guid, out _), $"Failed to find GUID for asset {asset}!");
        }

        [UnityTest]
        [RequiresPlayMode]
        public IEnumerator TestGetAssetReference([ValueSource(nameof(testAssets))] T asset) {
            var assetRef = GetAssetReference<UnityObject>(asset);
            yield return assetRef.LoadAssetAsync();
            Assert.AreEqual(GetAsset<UnityObject>(asset), assetRef.Asset, $"Failed to create reference for asset {asset}!");
        }
    }
}
