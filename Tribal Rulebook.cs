using BepInEx;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Card;
using InscryptionAPI.RuleBook;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tribal_Rulebook
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("cyantist.inscryption.api", BepInDependency.DependencyFlags.HardDependency)]

    public class TribalRulebook : BaseUnityPlugin
    {
        // --------------------------------------------------------------------------------------------------------------------------------------------------

        // Declare Harmony here for future Harmony patches. You'll use Harmony to patch the game's code outside of the scope of the API.
        readonly Harmony harmony = new(PluginGuid);

        // These are variables that exist everywhere in the entire class.
        public const string PluginGuid = "creator.TribalRulebook";
        public const string PluginName = "Tribal Rulebook";
        public const string PluginVersion = "1.0.0";
        public const string PluginPrefix = "Tribal_Rulebook";
        public void Awake()
        {
            RuleBookManager.New(
                PluginGuid, // a unique identifier for your mod
                PageRangeType.Boons, // the PageRangeType we want to inherit our page style from
                "Tribes", // the subsection name that appears at the end of the header
                GetInsertPosition, // a function that determines where in the Rulebook to insert our custom section
                CreatePages, // the function to create the pages that will be in our custom section
                "Tribes", // optional argument, if left null one will be created for you
                getStartingNumberFunc: GetStartingNumber, // optional argument, if left null the starting number will be 1
                fillPageAction: FillPage // also optional, but if you want to display custom names, descriptions, etc you will need to set this
            );
        }

        public static Dictionary<string, string> tribedescriptors = new Dictionary<string, string> { };

        public class CustomTribe
        {
            public string guid { get; set; }
            public string name { get; set; }
            public Tribe tribe { get; set; }
            public Sprite icon { get; set; }
            public string description { get; set; }

            public CustomTribe(string GUID, string NAME, Tribe TRIBE, Sprite ICON, string DESCRIPTION)
            {
                guid = GUID;
                name = NAME;
                tribe = TRIBE;
                icon = ICON;
                description = description;
            }
        }

        public static Dictionary<string, CustomTribe> TribeDict = new Dictionary<string, CustomTribe> { };
        public static void Descriptor(Tribe tribe, string description)
        {
            // Add each tribe info to TribeDict
            foreach (TribeManager.TribeInfo tribeinloop in TribeManager.NewTribes)
            {
                CustomTribe NEWCUSTOMTRIBE = new CustomTribe
                (
                    tribeinloop.guid,
                    tribeinloop.name,
                    tribeinloop.tribe,
                    tribeinloop.icon,
                    tribedescriptors.GetValueSafe(tribeinloop.guid + tribeinloop.name)
                );
                TribeDict.Add(tribeinloop.guid + tribeinloop.name, NEWCUSTOMTRIBE);
            }
        }

        // note that the return value and parameters MUST match the parameters and return value of the Func
        public static int GetInsertPosition(PageRangeInfo pageRangeInfo, List<RuleBookPageInfo> pages)
        {
            return pages.FindLastIndex(rbi => rbi.pagePrefab == pageRangeInfo.rangePrefab) + 1;
        }

        public static List<RuleBookPageInfo> CreatePages(RuleBookInfo instance, PageRangeInfo currentRange, AbilityMetaCategory metaCategory)
        {
            // in this example, we're adding a rulebook section for custom Tribes
            // foreach custom tribe that exists, we create a rulebook page then set the pageId to the tribe enum so we can use it later
            List<RuleBookPageInfo> retval = new();
            foreach (string tribe in TribeDict.Keys)
            {
                RuleBookPageInfo page = new();
                page.pageId = TribeDict[tribe].tribe.ToString();
                retval.Add(page);
            }
            return retval;
        }

        public static int GetStartingNumber(List<RuleBookPageInfo> addedPages)
        {
            return (int)Tribe.NUM_TRIBES; // since we're doing mod Tribes only, we start after custom Tribes (pretend we have a separate section for those)
        }

        public static void FillPage(RuleBookPage page, string pageId, object[] otherArgs)
        {
            if (page is BoonPage boonPage && int.TryParse(pageId, out int id))
            {
                foreach (var tribe in TribeDict.Keys)
                {
                        boonPage.nameTextMesh.text = TribeDict[tribe].name;
                        boonPage.descriptionTextMesh.text = TribeDict[tribe].description;
                        boonPage.iconRenderer.material.mainTexture = boonPage.iconRenderer2.material.mainTexture = TribeDict[tribe].icon.texture;
                }
            }
        }
    }
}