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

        bool IsValidOutput()
        {
            return string.IsNullOrEmpty(_directory) || string.IsNullOrEmpty(_filePrefix) || string.IsNullOrEmpty(_fileSuffix);
        }

        public T Duplicate(T original)
        {
            if (!IsValidOutput())
            {
                return original;
            }

            var originalAssetPath = AssetDatabase.GetAssetPath(original);
            var assetDirectory = _directory.StartsWith("Assets/") ? _directory : Path.Combine(Path.GetDirectoryName(originalAssetPath) ?? string.Empty, _directory);
            EnsureDirectory(assetDirectory);
            var assetPath = Path.Combine(assetDirectory, $"{_filePrefix}{Path.GetFileNameWithoutExtension(originalAssetPath)}{_fileSuffix}.mat");
            
            var existingMaterial = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (existingMaterial) return existingMaterial;

            var bakedAssetDirectoryPath = Path.GetDirectoryName(assetPath);
            foreach (var processor in _processors.Where(processor => processor.IsTarget(original)))
            {
                if (!processor.TryDuplicate(original, bakedAssetDirectoryPath, out var result))
                {
                    continue;
                }
                AssetDatabase.CreateAsset(result, assetPath);
                return result;
            }
            return original;
        }
        
        static void EnsureDirectory(string path)
        {
            Directory.CreateDirectory(path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        }
    }
}