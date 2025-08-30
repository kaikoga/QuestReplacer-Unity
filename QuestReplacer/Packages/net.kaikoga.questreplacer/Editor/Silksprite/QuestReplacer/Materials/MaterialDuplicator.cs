using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Silksprite.QuestReplacer.Materials.VRMShaders;
using UnityEditor;
using UnityEngine;

namespace Silksprite.QuestReplacer.Materials
{
    public class MaterialDuplicator
    {
        public static bool RegisterExt(QuestReplacerMaterialGenerationMode materialGenerationMode, ISingleMaterialDuplicator duplicator)
        {
            if (!Exts.TryGetValue(materialGenerationMode, out var list))
            {
                return false;
            }
            list.Add(duplicator);
            return true;
        }

        static readonly Dictionary<QuestReplacerMaterialGenerationMode, SortedSet<ISingleMaterialDuplicator>> Builtins = new Dictionary<QuestReplacerMaterialGenerationMode, SortedSet<ISingleMaterialDuplicator>>
        {
            [QuestReplacerMaterialGenerationMode.GenerateVRChatToonLit] = new SortedSet<ISingleMaterialDuplicator>(SingleMaterialDuplicatorComparer.Instance)
            {
                new SingleMaterialDuplicator("Standard", Shaders.VrcMobileStandardLite),
                new SingleMaterialDuplicator("", Shaders.VrcMobileToonLit)
            },
            [QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandard] = new SortedSet<ISingleMaterialDuplicator>(SingleMaterialDuplicatorComparer.Instance)
            {
                new SingleMaterialDuplicator("Standard", Shaders.VrcMobileStandardLite),
                new SingleMaterialDuplicator("", Shaders.VrcMobileToonStandard)
            },
            [QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandardOutline] = new SortedSet<ISingleMaterialDuplicator>(SingleMaterialDuplicatorComparer.Instance)
            {
                new SingleMaterialDuplicator("Standard", Shaders.VrcMobileStandardLite),
                new SingleMaterialDuplicator("", Shaders.VrcMobileToonStandardOutline)
            },
            [QuestReplacerMaterialGenerationMode.GenerateMToon] = new SortedSet<ISingleMaterialDuplicator>(SingleMaterialDuplicatorComparer.Instance)
            {
                new SingleMaterialDuplicator("Standard", Shaders.Standard),
                new SingleMaterialDuplicator("", Shaders.VrmMToon)
            },
            [QuestReplacerMaterialGenerationMode.GenerateMToon10] = new SortedSet<ISingleMaterialDuplicator>(SingleMaterialDuplicatorComparer.Instance)
            {
                new MToonUpgrader(),
                new SingleMaterialDuplicator("Standard", Shaders.Standard),
                new SingleMaterialDuplicator("", Shaders.VrmMToon10)
            },
        };

        static readonly Dictionary<QuestReplacerMaterialGenerationMode, SortedSet<ISingleMaterialDuplicator>> Exts = new Dictionary<QuestReplacerMaterialGenerationMode, SortedSet<ISingleMaterialDuplicator>>
        {
            [QuestReplacerMaterialGenerationMode.ExtConvertMToon] = new SortedSet<ISingleMaterialDuplicator>(SingleMaterialDuplicatorComparer.Instance),
            [QuestReplacerMaterialGenerationMode.ExtConvertMToon10] = new SortedSet<ISingleMaterialDuplicator>(SingleMaterialDuplicatorComparer.Instance),
            [QuestReplacerMaterialGenerationMode.ExtConvertVRChatToonStandard] = new SortedSet<ISingleMaterialDuplicator>(SingleMaterialDuplicatorComparer.Instance),
        };

        class SingleMaterialDuplicatorComparer : IComparer<ISingleMaterialDuplicator>
        {
            public static SingleMaterialDuplicatorComparer Instance => new SingleMaterialDuplicatorComparer();

            public int Compare(ISingleMaterialDuplicator x, ISingleMaterialDuplicator y)
            {
                return Comparer.Default.Compare(y?.Priority, x?.Priority);
            }
        }

        public static IEnumerable<ISingleMaterialDuplicator> VRChatToonLitMaterialProcessors()
        {
            return Builtins[QuestReplacerMaterialGenerationMode.GenerateVRChatToonLit];
        }

        public static IEnumerable<ISingleMaterialDuplicator> MToonMaterialProcessors(bool ext)
        {
            if (ext) foreach (var duplicator in Exts[QuestReplacerMaterialGenerationMode.ExtConvertMToon]) yield return duplicator; 
            foreach (var duplicator in Builtins[QuestReplacerMaterialGenerationMode.GenerateMToon]) yield return duplicator;
        }

        public static IEnumerable<ISingleMaterialDuplicator> MToon10MaterialProcessors(bool ext)
        {
            if (ext) foreach (var duplicator in Exts[QuestReplacerMaterialGenerationMode.ExtConvertMToon10]) yield return duplicator; 
            foreach (var duplicator in Builtins[QuestReplacerMaterialGenerationMode.GenerateMToon10]) yield return duplicator;
        }

        public static IEnumerable<ISingleMaterialDuplicator> VRChatToonStandardMaterialProcessors(bool ext)
        {
            if (ext) foreach (var duplicator in Exts[QuestReplacerMaterialGenerationMode.ExtConvertVRChatToonStandard]) yield return duplicator; 
            foreach (var duplicator in Builtins[QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandard]) yield return duplicator;
        }

        public static IEnumerable<ISingleMaterialDuplicator> VRChatToonStandardOutlineMaterialProcessors()
        {
            return Builtins[QuestReplacerMaterialGenerationMode.GenerateVRChatToonStandardOutline];
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