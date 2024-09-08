using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace SpyMod.Spy.Components
{
    public class SpyPassive : MonoBehaviour
    {
        public SkillDef spyPassive;

        public GenericSkill passiveSkillSlot;

        public bool isJump
        {
            get
            {
                if (spyPassive && passiveSkillSlot)
                {
                    return passiveSkillSlot.skillDef == spyPassive;
                }

                return false;
            }
        }
    }
}