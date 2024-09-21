using BepInEx;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Card;
using InscryptionAPI.Guid;
using InscryptionAPI.RuleBook;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tribal_Rulebook
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("cyantist.inscryption.api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("zorro.inscryption.infiniscryption.packmanager", BepInDependency.DependencyFlags.SoftDependency)]

    public class TheHexExpansion : BaseUnityPlugin
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
                null, // optional argument, if left null one will be created for you
                getStartingNumberFunc: GetStartingNumber, // optional argument, if left null the starting number will be 1
                fillPageAction: FillPage // also optional, but if you want to display custom names, descriptions, etc you will need to set this
            );
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
            List<TribeManager.TribeInfo> allTribes = TribeManager.NewTribes.ToList();
            List<RuleBookPageInfo> retval = new();
            foreach (var tribe in allTribes)
            {
                RuleBookPageInfo page = new();
                page.pageId = tribe.tribe.ToString();
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
                TribeManager.TribeInfo tribe = TribeManager.NewTribes.FirstOrDefault(x => x.tribe == (Tribe)id);
                if (tribe != null)
                {
                    boonPage.nameTextMesh.text = Localization.Translate(tribe.name.ToLowerInvariant().Replace("tribe", ""));
                    boonPage.descriptionTextMesh.text = "";
                    boonPage.iconRenderer.material.mainTexture = boonPage.iconRenderer2.material.mainTexture = tribe.icon.texture;
                }
            }
        }

        public static Tribe GetCustomTribe(string GUID, string name)
        {
            return GuidManager.GetEnumValue<Tribe>(GUID, name);
        }

        public static void Descriptor(Tribe tribe, string description)
        {
            // Prepare List
            string[] Info;
            // Add each Tribeinfo to Info
            foreach (tribes,0,TribeManager.NewTribes)
            {
                Info.AppendWith(Info, TribeManager.NewTribes.);
            }
            // Get GUID and Name from Info
            string Guid = Info[0];
            string Name = Info[1];
            // Fetch the real tribe
            Tribe TribeName = GetCustomTribe(Guid, Name);
            // Set the Description
            string TribeDescription = description;
            // Pass Info
        }
    }
}