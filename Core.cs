using Alta;
using CustomSkillsAPI;
using MelonLoader;
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

            CustomSkillsAPI.Core.SetUpProgressionSlots += SetUpProgressionSlots;
        }

        private void SetUpProgressionSlots()
        {
            AssetBundle assetBundle;
            using (System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("StingerReadded.AssetBundles.!stingerreadded"))
            {
                assetBundle = AssetBundle.LoadFromStream(stream);
            }
            
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
            typeof(PickupBasedSkill).GetField("hitPositionEffect", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(sidePowerHit,
                typeof(PickupBasedSkill).GetField("hitPositionEffect", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(powerHit)
            );
            typeof(PickupBasedSkill).GetField("hitPositionAndBladeAlignedEffect", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(sidePowerHit,
                typeof(PickupBasedSkill).GetField("hitPositionAndBladeAlignedEffect", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(powerHit)
            );

            CustomSkillsAPI.Core.AddProfessionSkill(
                sidePowerHit,
                [
                    ((Sprite[])typeof(ProfessionSkill).GetField("practiceGlyphSprites", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(powerStinger))[0], // this is Glyph_Ability_HoldSideBack
                    ((Sprite[])typeof(ProfessionSkill).GetField("practiceGlyphSprites", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(powerHit))[1] // this is Glyph_AbilityHitTrigger
                ]
            );

            CustomSkillsAPI.Core.AddProgressionSlot(
                sidePowerHit,
                "Side Power Hit",
                1u,
                new List<uint>([7141u]),
                51077u,
                new Vector3(0, 0.1f, 0.1f),
                ProgressionPath.Melee,
                assetBundle.LoadAsset<Texture2D>("SidePowerHitSlotIcon.png")
            );

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

            CustomSkillsAPI.Core.AddProgressionSlot(
                powerStinger,
                "Power Stinger Readded",
                1u,
                new List<uint>([51077u]),
                60199u,
                new Vector3(0, 0.1f, 0.3f),
                ProgressionPath.Melee,
                assetBundle.LoadAsset<Texture2D>("StingerSlotIcon.png")
            );
        }
    }
}