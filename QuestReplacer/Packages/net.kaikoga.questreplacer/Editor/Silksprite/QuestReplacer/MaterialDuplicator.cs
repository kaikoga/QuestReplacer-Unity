using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer
{
    public class MaterialDuplicator
    {
        public static readonly ISingleMaterialDuplicator[] VRChatToonLitMaterialProcessors =
        {
            new SingleMaterialDuplicator("Standard", Shaders.VrcMobileStandardLite),
            new SingleMaterialDuplicator("", Shaders.VrcMobileToonLit)
        };

        public static readonly ISingleMaterialDuplicator[] MToonMaterialProcessors =
        {
            new lilToonMaterialDuplicator(),
            new SingleMaterialDuplicator("Standard", Shaders.Standard),
            new SingleMaterialDuplicator("", Shaders.VrmMToon)
        };

        public static readonly ISingleMaterialDuplicator[] MToon10MaterialProcessors =
        {
            new lilToonMaterialDuplicator(Shaders.VrmMToon10),
            new SingleMaterialDuplicator("Standard", Shaders.Standard),
            new SingleMaterialDuplicator("", Shaders.VrmMToon10)
        };

        public static readonly ISingleMaterialDuplicator[] VRChatToonStandardMaterialProcessors =
        {
            new SingleMaterialDuplicator("Standard", Shaders.VrcMobileStandardLite),
            new SingleMaterialDuplicator("", Shaders.VrcMobileToonStandard)
        };

        readonly string _directory;
        readonly string _filePrefix;
        readonly string _fileSuffix;
        readonly ISingleMaterialDuplicator[] _processors;

        public MaterialDuplicator(string directory, string filePrefix, string fileSuffix, ISingleMaterialDuplicator[] processors)
        {
            _directory = directory;
            _filePrefix = filePrefix;
            _fileSuffix = fileSuffix;
                 
            _processors = processors;
        }

        public Material Duplicate(Material original)
        {
            var originalAssetPath = AssetDatabase.GetAssetPath(original);
            var assetDirectory = _directory.StartsWith("Assets/") ? _directory : Path.Combine(Path.GetDirectoryName(originalAssetPath) ?? string.Empty, _directory);
            EnsureDirectory(assetDirectory);
            var assetPath = Path.Combine(assetDirectory, $"{_filePrefix}{Path.GetFileNameWithoutExtension(originalAssetPath)}{_fileSuffix}.mat");
            
            var existingMaterial = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
            if (existingMaterial) return existingMaterial;

            var material = _processors.First(processor => processor.IsTarget(original))
                .Duplicate(original, assetPath);
            return material;
        }
        
        static void EnsureDirectory(string path)
        {
            Directory.CreateDirectory(path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        }
    }
}