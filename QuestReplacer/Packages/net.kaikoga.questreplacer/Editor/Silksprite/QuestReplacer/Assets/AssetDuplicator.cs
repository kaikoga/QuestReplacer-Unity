using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer.Assets
{
    public class AssetDuplicator<T>
    where T : Object
    {
        readonly string _directory;
        readonly string _filePrefix;
        readonly string _fileSuffix;
        readonly ISingleAssetDuplicator<T>[] _processors;

        public AssetDuplicator(string directory, string filePrefix, string fileSuffix, ISingleAssetDuplicator<T>[] processors)
        {
            _directory = directory;
            _filePrefix = filePrefix;
            _fileSuffix = fileSuffix;
                 
            _processors = processors;
        }

        public T Duplicate(T original)
        {
            var originalAssetPath = AssetDatabase.GetAssetPath(original);
            var assetDirectory = _directory.StartsWith("Assets/") ? _directory : Path.Combine(Path.GetDirectoryName(originalAssetPath) ?? string.Empty, _directory);
            EnsureDirectory(assetDirectory);
            var assetPath = Path.Combine(assetDirectory, $"{_filePrefix}{Path.GetFileNameWithoutExtension(originalAssetPath)}{_fileSuffix}.mat");
            
            var existingMaterial = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (existingMaterial) return existingMaterial;

            var bakedAssetDirectoryPath = Path.GetDirectoryName(assetPath);
            var material = _processors.First(processor => processor.IsTarget(original))
                .Duplicate(original, bakedAssetDirectoryPath);
            if (material != original)
            {
                AssetDatabase.CreateAsset(material, assetPath);
            }
            return material;
        }
        
        static void EnsureDirectory(string path)
        {
            Directory.CreateDirectory(path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        }
    }
}