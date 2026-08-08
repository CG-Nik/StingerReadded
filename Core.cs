using Alta;
using Alta.Inventory;
using CustomSkillsAPI;
using MelonLoader;
using MelonLoader.Utils;
using System.Reflection;
using UnityEngine;

[assembly: MelonInfo(typeof(StingerReadded.Core), "StingerReadded", "1.0.0", "CGNik", null)]
[assembly: MelonGame("Alta", "A Township Tale")]

namespace StingerReadded
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");

            CustomSkillsAPI.Core.SetUpProgressionSlots += _SetUpProgressionSlots;
        }

        private void _SetUpProgressionSlots()
        {
            AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory, "StingerReadded/AssetBundles/!stingerreadded"));

            // SIDE POWER HIT
            //
            ProfessionSkill professionSkill_SidePowerHit = assetBundle.LoadAsset<ProfessionSkill>("Side Power Hit.asset");
            PowerHit sidePowerHit = professionSkill_SidePowerHit as PowerHit;

            PowerHit powerHit = ProfessionSkill.All.Where(skill => skill.Hash == 51076u).First() as PowerHit;
            PowerStinger powerStinger = ProfessionSkill.All.Where(skill => skill.Hash == 60198u).First() as PowerStinger;

            typeof(PowerHit).GetField("targetImpactChannel", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(sidePowerHit,
                typeof(PowerHit).GetField("targetImpactChannel", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(powerHit) // this is CombatImpactChannel
            );
            typeof(PickupBasedSkill).GetField("inputStructure", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(sidePowerHit, 
                typeof(PickupBasedSkill).GetField("inputStructure", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(powerStinger) // this is HandOnSideStructure
            );
            typeof(ProfessionSkill).GetField("practiceGlyphSprites", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(sidePowerHit, 
                new Sprite[2]
                {
                    ((Sprite[])typeof(ProfessionSkill).GetField("practiceGlyphSprites", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(powerStinger))[0], // this is Glyph_Ability_HoldSideBack
                    ((Sprite[])typeof(ProfessionSkill).GetField("practiceGlyphSprites", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(powerHit))[1] // this is Glyph_AbilityHitTrigger
                }
            );
            typeof(PickupBasedSkill).GetField("hitPositionEffect", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(sidePowerHit,
                typeof(PickupBasedSkill).GetField("hitPositionEffect", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(powerHit)
            );
            typeof(PickupBasedSkill).GetField("hitPositionAndBladeAlignedEffect", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(sidePowerHit,
                typeof(PickupBasedSkill).GetField("hitPositionAndBladeAlignedEffect", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(powerHit)
            );

            CustomSkillsAPI.Core.RegisterProfessionSkill(professionSkill_SidePowerHit);
            ProgressionSlot progressionSlot_SidePowerHit = new(professionSkill_SidePowerHit);
            foreach (var fieldInfo in progressionSlot_SidePowerHit.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                switch (fieldInfo.Name)
                {
                    case "name":
                        fieldInfo.SetValue(progressionSlot_SidePowerHit, "Side Power Hit");
                        break;
                    case "cost":
                        fieldInfo.SetValue(progressionSlot_SidePowerHit, 1u);
                        break;
                    case "dependencies":
                        fieldInfo.SetValue(progressionSlot_SidePowerHit, new List<uint>([7141u]));
                        break;
                    case "hash":
                        fieldInfo.SetValue(progressionSlot_SidePowerHit, 51077u);
                        break;
                    case "position":
                        fieldInfo.SetValue(progressionSlot_SidePowerHit, new Vector3(0, 0.1f, 0.1f));
                        break;
                    case "slotIcon":
                        Texture2D texture = null;
                        if (assetBundle)
                        {
                            texture = assetBundle.LoadAsset<Texture2D>("SidePowerHitSlotIcon.png");
                        }
                        fieldInfo.SetValue(progressionSlot_SidePowerHit, texture);
                        break;
                    default:
                        break;
                }
            }
            progressionSlot_SidePowerHit.Validate(ProfessionSkillTree.GetProfessionTree(ProgressionPath.Melee));
            CustomSkillsAPI.Core.progressionSlots.Add(progressionSlot_SidePowerHit);

            // STINGER
            //
            ProfessionSkill professionSkill_PowerStinger = ProfessionSkill.All.Where(skill => skill.Hash == 60198u).First(); // this is PowerStinger
            // PowerStinger powerStinger = professionSkill_PowerStinger as PowerStinger; this would be set here, but it's already set from SidePowerHit using it

            // makes the correct achievement trigger when you obtain PowerStinger
            typeof(ProfessionSkill).GetField("targetAchievement", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(professionSkill_PowerStinger,
                GameAchievement.All.Where(achievement => achievement.Hash == 7222u).First() // this is "Unlock a High Level Melee Skill"
            );

            // makes the orb visible
            PooledObjectDefinition targetDefinition = (PooledObjectDefinition)powerStinger.GetType().GetField("targetDefinition", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(powerStinger);
            PooledObject prefab = (PooledObject)targetDefinition.GetType().GetField("prefab", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(targetDefinition);
            GameObject stingerTargetOrb = (prefab as SkillHitTarget).gameObject;
            stingerTargetOrb.transform.Find("Orb windows").gameObject.SetActive(true);

            ProgressionSlot progressionSlot_PowerStinger = new(professionSkill_PowerStinger);
            foreach (var fieldInfo in progressionSlot_PowerStinger.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                switch (fieldInfo.Name)
                {
                    case "name":
                        fieldInfo.SetValue(progressionSlot_PowerStinger, "Power Stinger Readded");
                        break;
                    case "cost":
                        fieldInfo.SetValue(progressionSlot_PowerStinger, 1u);
                        break;
                    case "dependencies":
                        fieldInfo.SetValue(progressionSlot_PowerStinger, new List<uint>([51077u])); // this is SidePowerHit
                        break;
                    case "hash":
                        fieldInfo.SetValue(progressionSlot_PowerStinger, 60199u);
                        break;
                    case "position":
                        fieldInfo.SetValue(progressionSlot_PowerStinger, new Vector3(0, 0.1f, 0.3f));
                        break;
                    case "slotIcon":
                        Texture2D texture = null;
                        if (assetBundle)
                        {
                            texture = assetBundle.LoadAsset<Texture2D>("StingerSlotIcon.png");
                        }
                        fieldInfo.SetValue(progressionSlot_PowerStinger, texture);
                        break;
                    default:
                        break;
                }
            }
            progressionSlot_PowerStinger.Validate(ProfessionSkillTree.GetProfessionTree(ProgressionPath.Melee));
            CustomSkillsAPI.Core.progressionSlots.Add(progressionSlot_PowerStinger);
        }
    }
}