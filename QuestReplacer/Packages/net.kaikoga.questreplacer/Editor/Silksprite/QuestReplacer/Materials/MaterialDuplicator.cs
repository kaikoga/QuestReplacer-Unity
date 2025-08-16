using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer.Materials
{
    public class MaterialDuplicator
    {
        public static bool RegisterExt(QuestReplacerGenerateMode generateMode, ISingleMaterialDuplicator duplicator)
        {
            if (!Exts.TryGetValue(generateMode, out var list))
            {
                return false;
            }
            list.Add(duplicator);
            return true;
        }

        static readonly Dictionary<QuestReplacerGenerateMode, ISingleMaterialDuplicator[]> Builtins = new Dictionary<QuestReplacerGenerateMode, ISingleMaterialDuplicator[]>
        {
            [QuestReplacerGenerateMode.GenerateVRChatToonLit] = new ISingleMaterialDuplicator[]
            {
                new SingleMaterialDuplicator("Standard", Shaders.VrcMobileStandardLite),
                new SingleMaterialDuplicator("", Shaders.VrcMobileToonLit)
            },
            [QuestReplacerGenerateMode.GenerateVRChatToonStandard] = new ISingleMaterialDuplicator[]
            {
                new SingleMaterialDuplicator("Standard", Shaders.VrcMobileStandardLite),
                new SingleMaterialDuplicator("", Shaders.VrcMobileToonStandard)
            },
            [QuestReplacerGenerateMode.GenerateVRChatToonStandardOutline] = new ISingleMaterialDuplicator[]
            {
                new SingleMaterialDuplicator("Standard", Shaders.VrcMobileStandardLite),
                new SingleMaterialDuplicator("", Shaders.VrcMobileToonStandardOutline)
            },
            [QuestReplacerGenerateMode.GenerateMToon] = new ISingleMaterialDuplicator[]
            {
                new SingleMaterialDuplicator("Standard", Shaders.Standard),
                new SingleMaterialDuplicator("", Shaders.VrmMToon)
            },
            [QuestReplacerGenerateMode.GenerateMToon10] = new ISingleMaterialDuplicator[]
            {
                new SingleMaterialDuplicator("Standard", Shaders.Standard),
                new SingleMaterialDuplicator("", Shaders.VrmMToon10)
            },
        };
        
        static readonly Dictionary<QuestReplacerGenerateMode, List<ISingleMaterialDuplicator>> Exts = new Dictionary<QuestReplacerGenerateMode, List<ISingleMaterialDuplicator>>
        {
            [QuestReplacerGenerateMode.ExtConvertMToon] = new List<ISingleMaterialDuplicator>(),
            [QuestReplacerGenerateMode.ExtConvertMToon10] = new List<ISingleMaterialDuplicator>(),
            [QuestReplacerGenerateMode.ExtConvertVRChatToonStandard] = new List<ISingleMaterialDuplicator>(),
        };
        
        public static IEnumerable<ISingleMaterialDuplicator> VRChatToonLitMaterialProcessors()
        {
            return Builtins[QuestReplacerGenerateMode.GenerateVRChatToonLit];
        }

        public static IEnumerable<ISingleMaterialDuplicator> MToonMaterialProcessors(bool ext)
        {
            if (ext) foreach (var duplicator in Exts[QuestReplacerGenerateMode.ExtConvertMToon]) yield return duplicator; 
            foreach (var duplicator in Builtins[QuestReplacerGenerateMode.GenerateMToon]) yield return duplicator;
        }

        public static IEnumerable<ISingleMaterialDuplicator> MToon10MaterialProcessors(bool ext)
        {
            if (ext) foreach (var duplicator in Exts[QuestReplacerGenerateMode.ExtConvertMToon10]) yield return duplicator; 
            foreach (var duplicator in Builtins[QuestReplacerGenerateMode.GenerateMToon10]) yield return duplicator;
        }

        public static IEnumerable<ISingleMaterialDuplicator> VRChatToonStandardMaterialProcessors(bool ext)
        {
            if (ext) foreach (var duplicator in Exts[QuestReplacerGenerateMode.ExtConvertVRChatToonStandard]) yield return duplicator; 
            foreach (var duplicator in Builtins[QuestReplacerGenerateMode.GenerateVRChatToonStandard]) yield return duplicator;
        }

        public static IEnumerable<ISingleMaterialDuplicator> VRChatToonStandardOutlineMaterialProcessors()
        {
            return Builtins[QuestReplacerGenerateMode.GenerateVRChatToonStandardOutline];
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