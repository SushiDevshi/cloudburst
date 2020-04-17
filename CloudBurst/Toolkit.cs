using R2API;
using R2API.Utils;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Skills;
using UnityEngine;

namespace CloudBurst
{
    public static class Toolkit
    {
        public static void DestroyGenericSkillComponents(GameObject gameObject)
        {
            foreach (GenericSkill skill in gameObject.GetComponentsInChildren<GenericSkill>())
            {
                UnityEngine.Object.DestroyImmediate(skill);
            }
        }
        public static void DestroyAISkillDrivers(GameObject gameObject)
        {
            foreach (AISkillDriver driver in gameObject.GetComponentsInChildren<AISkillDriver>())
            {
                UnityEngine.Object.DestroyImmediate(driver);
            }
        }
        public static void CreateSkillFamilies(SkillLocator skillLocator, GameObject myCharacter)
        {
            skillLocator.SetFieldValue<GenericSkill[]>("allSkills", new GenericSkill[0]);
            {
                skillLocator.primary = myCharacter.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[1];
                LoadoutAPI.AddSkillFamily(newFamily);
                skillLocator.primary.SetFieldValue("_skillFamily", newFamily);
            }
            {
                skillLocator.secondary = myCharacter.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[1];
                LoadoutAPI.AddSkillFamily(newFamily);
                skillLocator.secondary.SetFieldValue("_skillFamily", newFamily);
            }
            {
                skillLocator.utility = myCharacter.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[1];
                LoadoutAPI.AddSkillFamily(newFamily);
                skillLocator.utility.SetFieldValue("_skillFamily", newFamily);
            }
            {
                skillLocator.special = myCharacter.AddComponent<GenericSkill>();
                SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
                newFamily.variants = new SkillFamily.Variant[1];
                LoadoutAPI.AddSkillFamily(newFamily);
                skillLocator.special.SetFieldValue("_skillFamily", newFamily);
            }
        }

    }
}