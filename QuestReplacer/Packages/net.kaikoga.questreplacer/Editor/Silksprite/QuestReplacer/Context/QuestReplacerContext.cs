using System;
using System.Collections.Generic;
using System.Linq;
using Silksprite.QuestReplacer.Extensions;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Silksprite.QuestReplacer.Context
{
    public class QuestReplacerContext
    {
        readonly QuestReplacement[] _replacements;

        readonly Dictionary<(Type, bool), SerializedProperty[]> _cachedTargetProperties = new Dictionary<(Type, bool), SerializedProperty[]>();
        readonly Dictionary<Type, QuestStatus> _cachedQuestStatus = new Dictionary<Type, QuestStatus>();
        readonly QuestReplacerTransformTarget _transforms;
        readonly QuestReplacerAnimatorControllerTarget _animatorControllers;

        public QuestReplacerContext(IEnumerable<Transform> targets, IEnumerable<AnimatorController> animatorControllers, IEnumerable<QuestTypeFilter> componentFilters, IEnumerable<QuestReplacement> pairs)
        {
            var filters = (componentFilters ?? DeepCollectComponentTypes().ToTypeFilters()).Reverse().ToArray();
            _transforms = new QuestReplacerTransformTarget(targets.Where(target => target).ToArray(), filters);
            _animatorControllers = new QuestReplacerAnimatorControllerTarget(animatorControllers.ToArray());
            _replacements = pairs.ToArray();
        }

        public QuestStatus ToQuestStatus<T>()
            where T : Object
        {
            if (_cachedQuestStatus.TryGetValue(typeof(T), out var questStatus)) return questStatus;
            
            return _cachedQuestStatus[typeof(T)] = DeepCollectReferences<T>()
                .SelectMany(c => _replacements.Select(r => r.GetStatus(c)).Where(s => s != QuestStatus.Unmanaged))
                .Aggregate(QuestStatus.Either, (accumulator, v) => accumulator.Merge(v));
        }

        public bool Query<T>(T fromValue, bool toRight, out T toValue)
        where T : Object
        {
            return _replacements.Query(fromValue, toRight, out toValue);
        }

        public IEnumerable<Type> DeepCollectComponentTypes()
        {
            return _transforms.DeepCollectComponentTypes();
        }

        public IEnumerable<T> DeepCollectReferences<T>()
            where T : Object
        {
            return DeepCollectProperties<T>(true).Select(prop => (T)prop.objectReferenceValue);
        }

        public void DeepOverrideReferences<T>(bool toRight)
            where T : Object
        {
            var serializedObjects = new HashSet<SerializedObject>(); 
            foreach (var prop in DeepCollectProperties<T>(false))
            {
                // T might be Object, so do all replacements but ignore unregistered components
                if (!Query(prop.objectReferenceValue, toRight, out var toValue)) continue;
                prop.objectReferenceValue = toValue;
                serializedObjects.Add(prop.serializedObject);
            }

            foreach (var serializedObject in serializedObjects)
            {
                Undo.RecordObject(serializedObject.targetObject, "Quest Replacer");
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(serializedObject.targetObject);
            }
        }

        IEnumerable<SerializedProperty> DeepCollectProperties<T>(bool withAssets)
            where T : Object
        {
            if (_cachedTargetProperties.TryGetValue((typeof(T), withAssets), out var properties)) return properties;

            var targets = _transforms.DeepCollectComponents(false).OfType<Object>();
            if (withAssets)
            {
                targets = targets.Concat(_animatorControllers.CollectMotions());
            }

            return _cachedTargetProperties[(typeof(T), withAssets)] = targets 
                .SelectMany(CollectProperties<T>)
                .ToArray();
        }

        static IEnumerable<SerializedProperty> CollectProperties<T>(Object obj)
            where T : Object
        {
            var serializedObj = new SerializedObject(obj);
            var it = serializedObj.GetIterator();
            while (it.Next(true))
            {
                if (it.propertyType != SerializedPropertyType.ObjectReference) continue;
                if (it.objectReferenceValue is T)
                {
                    yield return it.Copy();
                }
            }
        }
    }
}