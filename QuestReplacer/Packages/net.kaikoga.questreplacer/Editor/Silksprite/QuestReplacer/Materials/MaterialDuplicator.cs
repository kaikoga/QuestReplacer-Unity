using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer.Materials
{
    public class MaterialDuplicator
    {
        public static IEnumerable<ISingleMaterialDuplicator> VRChatToonLitMaterialProcessors()
        {
            yield return new SingleMaterialDuplicator("Standard", Shaders.VrcMobileStandardLite);
            yield return new SingleMaterialDuplicator("", Shaders.VrcMobileToonLit);
        }

        public static IEnumerable<ISingleMaterialDuplicator> MToonMaterialProcessors(bool ext)
        {
            if (ext) yield return new lilToonToVRMMaterialDuplicator();
            yield return new SingleMaterialDuplicator("Standard", Shaders.Standard);
            yield return new SingleMaterialDuplicator("", Shaders.VrmMToon);
        }

        public static IEnumerable<ISingleMaterialDuplicator> MToon10MaterialProcessors(bool ext)
        {
            if (ext) yield return new lilToonToVRMMaterialDuplicator(Shaders.VrmMToon10);
            yield return new SingleMaterialDuplicator("Standard", Shaders.Standard);
            yield return new SingleMaterialDuplicator("", Shaders.VrmMToon10);
        }

        public static IEnumerable<ISingleMaterialDuplicator> VRChatToonStandardMaterialProcessors(bool ext)
        {
            if (ext) yield return new VRCQuestToolsMaterialDuplicator();
            yield return new SingleMaterialDuplicator("Standard", Shaders.VrcMobileStandardLite);
            yield return new SingleMaterialDuplicator("", Shaders.VrcMobileToonStandard);
        }

        public static IEnumerable<ISingleMaterialDuplicator> VRChatToonStandardOutlineMaterialProcessors()
        {
            yield return new SingleMaterialDuplicator("Standard", Shaders.VrcMobileStandardLite);
            yield return new SingleMaterialDuplicator("", Shaders.VrcMobileToonStandardOutline);
        }

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