using MelonLoader;
using System.Reflection;
using CustomSkillsAPI;
using UnityEngine;
using MelonLoader.Utils;
using Alta.Inventory;

[assembly: MelonInfo(typeof(StingerReadded.Core), "StingerReadded", "1.0.0", "CGNik", null)]
[assembly: MelonGame("Alta", "A Township Tale")]

namespace StingerReadded
{
    public class Core : MelonMod
    {
        private List<ProgressionSlot> progressionSlots = [];

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized.");

            CustomSkillsAPI.Core.SetUpProgressionSlots += _SetUpProgressionSlots;
            CustomSkillsAPI.Core.AddProgressionSlots += _AddProgressionSlots;
            CustomSkillsAPI.Core.AddInherits += _AddInherits;
        }

        private void _SetUpProgressionSlots()
        {
            ProfessionSkill professionSkill = ProfessionSkill.All.Where(skill => skill.Hash == 60198u).First(); // this is PowerStinger
            PowerStinger powerStinger = professionSkill as PowerStinger;

            // makes the correct achievement trigger when you obtain PowerStinger
            typeof(ProfessionSkill).GetField("targetAchievement", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(professionSkill,
                GameAchievement.All.Where(achievement => achievement.Hash == 7222u).First() // this is "Unlock a High Level Melee Skill"
            );

            // makes the orb visible
            PooledObjectDefinition targetDefinition = (PooledObjectDefinition)powerStinger.GetType().GetField("targetDefinition", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(powerStinger);
            PooledObject prefab = (PooledObject)targetDefinition.GetType().GetField("prefab", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(targetDefinition);
            GameObject stingerTargetOrb = (prefab as SkillHitTarget).gameObject;
            stingerTargetOrb.transform.Find("Orb windows").gameObject.SetActive(true);

            ProgressionSlot progressionSlot = new(professionSkill);
            progressionSlot.Path = ProgressionPath.Melee;
            foreach (var fieldInfo in progressionSlot.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                switch (fieldInfo.Name)
                {
                    case "name":
                        fieldInfo.SetValue(progressionSlot, "Power Stinger Readded");
                        break;
                    case "cost":
                        fieldInfo.SetValue(progressionSlot, 1u);
                        break;
                    case "dependencies":
                        fieldInfo.SetValue(progressionSlot, new List<uint>([32655u])); // this is PowerHit
                        break;
                    case "hash":
                        fieldInfo.SetValue(progressionSlot, 3671701u);
                        break;
                    case "position":
                        fieldInfo.SetValue(progressionSlot, (2 * new Vector3(-0.176f, 0.515f, 0.223f)) - new Vector3(0, 0.335f, 0.227f));
                        break;
                    case "slotIcon":
                        AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory, "StingerReadded/AssetBundles/!stingerreadded"));
                        Texture2D texture = null;
                        if (assetBundle)
                        {
                            texture = assetBundle.LoadAsset<Texture2D>("StingerSlotIconV4");
                        }
                        fieldInfo.SetValue(progressionSlot, texture);
                        break;
                    case "inheritSlots":
                        fieldInfo.SetValue(progressionSlot, new List<ProgressionSlot>([]));
                        break;
                    default:
                        break;
                }
            }
            progressionSlot.GetType().GetField("indentLevel", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(progressionSlot, progressionSlot.GetIndentLevel(ProfessionSkillTree.GetProfessionTree(progressionSlot.Path)));
            progressionSlots.Add(progressionSlot);
        }

        private void _AddProgressionSlots()
        {
            foreach (ProgressionSlot progressionSlot in progressionSlots)
            {
                ProfessionSkillTree professionSkillTree = ProfessionSkillTree.GetProfessionTree(progressionSlot.Path);

                Dictionary<uint, ProgressionSlot> slotMap = (Dictionary<uint, ProgressionSlot>)professionSkillTree.GetType().GetField("slotMap", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(professionSkillTree);
                slotMap[progressionSlot.Hash] = progressionSlot;
                professionSkillTree.Slots.Add(progressionSlot);
            }
        }

        private void _AddInherits()
        {
            foreach (ProgressionSlot progressionSlot in progressionSlots)
            {
                progressionSlot.AddInherit(ProfessionSkillTree.GetProfessionTree(progressionSlot.Path));
            }
        }
    }
}