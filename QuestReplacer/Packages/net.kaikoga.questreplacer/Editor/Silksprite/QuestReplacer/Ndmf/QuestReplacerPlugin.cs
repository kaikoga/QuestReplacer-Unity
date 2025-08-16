#if QUEST_REPLACER_NDMF_SUPPORT

using System;
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

        protected override void Execute(BuildContext buildContext)
        {
#if UNITY_STANDALONE
            var avatarRootObject = buildContext.AvatarRootObject;
            DoExecute(buildContext, IsVrm0(avatarRootObject) || IsVrm1(avatarRootObject));
#elif UNITY_ANDROID || UNITY_IOS
            DoExecute(buildContext, true); 
#endif
        }
        
        void DoExecute(BuildContext buildContext, bool toRight)
        {
            foreach (var replacer in buildContext.AvatarRootTransform.GetComponentsInChildren<QuestReplacer>(true))
            {
                replacer.ToContext().DeepOverrideReferences<Object>(toRight);
                Object.DestroyImmediate(replacer);
            }
        }

    }
}

#endif