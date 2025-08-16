#if QUEST_REPLACER_NDMF_SUPPORT

using System;
using System.Linq;
using nadena.dev.ndmf;
using Silksprite.QuestReplacer.Extensions;
using Silksprite.QuestReplacer.Ndmf;
using UnityEngine;
using Object = UnityEngine.Object;

[assembly: ExportsPlugin(typeof(QuestReplacerPlugin))]

namespace Silksprite.QuestReplacer.Ndmf
{
    class QuestReplacerPlugin : Plugin<QuestReplacerPlugin>
    {
        public override string QualifiedName => "net.kaikoga.questreplacer";
        public override string DisplayName => "QuestReplacer";

        protected override void OnUnhandledException(Exception e)
        {
            Debug.LogException(e);
        }

        protected override void Configure()
        {
            var transforming = InPhase(BuildPhase.Transforming);
            transforming.Run(QuestReplacerPass.Instance);
        }
    }

    class QuestReplacerPass : Pass<QuestReplacerPass>
    {
#if QUEST_REPLACER_VRM0
        bool IsVrm0(GameObject avatarRootObject) => avatarRootObject.GetComponent<VRM.VRMMeta>();
#else
        bool IsVrm0(GameObject avatarRootObject) => false;
#endif

#if QUEST_REPLACER_VRM1
        bool IsVrm1(GameObject avatarRootObject) => avatarRootObject.GetComponent<UniVRM10.Vrm10Instance>();
#else
        bool IsVrm1(GameObject avatarRootObject) => false;
#endif

#if QUEST_REPLACER_VRCSDK3_AVATARS
        bool IsVRChat(GameObject avatarRootObject) => avatarRootObject.GetComponent<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>();
#else
        bool IsVRChatPC(GameObject avatarRootObject) => false;
#endif

#if UNITY_STANDALONE
        bool IsStandalone() => true;
#else
        bool IsStandalone() => false;
#endif

#if UNITY_ANDROID
        bool IsAndroid() => true;
#else
        bool IsAndroid() => false;
#endif

#if UNITY_IOS
        bool IsIos() => true;
#else
        bool IsIos() => false;
#endif

        bool TryGetPlatformForAvatar(BuildContext buildContext, out QuestReplacerPlatform platform)
        {
            var avatarRootObject = buildContext.AvatarRootObject;
            
            if (IsVrm0(avatarRootObject))
            {
                platform = QuestReplacerPlatform.VRM0;
            }
            else if (IsVrm1(avatarRootObject))
            {
                platform = QuestReplacerPlatform.VRM1;
            }
            else if (IsVRChat(avatarRootObject))
            {
                if (IsStandalone())
                {
                    platform = QuestReplacerPlatform.VRChatPC;
                }
                if (IsAndroid())
                {
                    platform = QuestReplacerPlatform.VRChatAndroid;
                }
                else if (IsIos())
                {
                    platform = QuestReplacerPlatform.VRChatIos;
                }
                else
                {
                    platform = default;
                    return false;
                }
            }
            else
            {
                platform = QuestReplacerPlatform.Generic;
            }
            return true;
        }

        protected override void Execute(BuildContext buildContext)
        {
            if (TryGetPlatformForAvatar(buildContext, out var platform))
            {
                DoExecute(buildContext, platform);
            }
        }
        
        void DoExecute(BuildContext buildContext, QuestReplacerPlatform platform)
        {
            var allReplacers = buildContext.AvatarRootTransform.GetComponentsInChildren<QuestReplacer>(true);
            foreach (var replacer in allReplacers.Where(replacer => replacer.database && replacer.database.platform != platform))
            {
                replacer.ToContext().DeepOverrideReferences<Object>(false);
                Object.DestroyImmediate(replacer);
            }
            foreach (var replacer in allReplacers.Where(replacer => !replacer.database || replacer.database.platform == platform))
            {
                replacer.ToContext().DeepOverrideReferences<Object>(true);
                Object.DestroyImmediate(replacer);
            }
        }

    }
}

#endif