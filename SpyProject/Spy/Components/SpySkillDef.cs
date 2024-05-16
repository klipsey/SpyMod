using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;

namespace SpyMod.Spy.Components
{
    public class SpySkillDef : SkillDef
    {
        protected class InstanceData : BaseSkillInstanceData
        {
            public SpyController spyController;
        }

        public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            return new InstanceData
            {
                spyController = skillSlot.GetComponent<SpyController>()
            };
        }
        private static bool WatchOut([NotNull] GenericSkill skillSlot)
        {
            if (((InstanceData)skillSlot.skillInstanceData).spyController.IsStopWatchOut())
            {
                return false;
            }
            return true;
        }
        public override bool CanExecute([NotNull] GenericSkill skillSlot)
        {
            if (!WatchOut(skillSlot))
            {
                return false;
            }
            return base.CanExecute(skillSlot);
        }

        public override bool IsReady([NotNull] GenericSkill skillSlot)
        {
            if (base.IsReady(skillSlot))
            {
                return WatchOut(skillSlot);
            }
            return false;
        }
    }
}
