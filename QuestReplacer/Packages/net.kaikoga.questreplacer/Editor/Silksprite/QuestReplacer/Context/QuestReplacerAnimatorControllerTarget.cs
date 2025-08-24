using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

namespace Silksprite.QuestReplacer.Context
{
    public class QuestReplacerAnimatorControllerTarget
    {
        readonly AnimatorController[] _animatorControllers;
        public QuestReplacerAnimatorControllerTarget(AnimatorController[] animatorControllers)
        {
            _animatorControllers = animatorControllers;
        }
        
        public IEnumerable<Motion> CollectMotions()
        {
            return _animatorControllers.SelectMany(controller => controller.animationClips);
        }
    }
}
