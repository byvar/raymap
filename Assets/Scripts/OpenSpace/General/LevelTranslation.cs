using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace {
    public class LevelTranslation {

        public struct LevelTranslationItem {
            string FolderName;
            string LevelName;

            public LevelTranslationItem(string FolderName, string LevelName)
            {
                this.FolderName = FolderName;
                this.LevelName = LevelName;
            }
        }

        public LevelTranslation(List<(string, string)> items)
        {
            this.items = items;
        }

        public List<(string, string)> items;

        public List<string> SortAndTranslate(List<string> levels)
        {
            List<string> result = new List<string>(levels);
            result = result.OrderBy((l) =>
            {
                return items.IndexOf(items.Find(m => { return m.Item1.ToLower() == l.ToLower(); }));
            }).ToList();

            List<string> translatedResult = new List<string>();

            result.ForEach((l) =>
            {
                (string, string)? item = items.Find(m => { return m.Item1.ToLower() == l.ToLower(); });
                if (item != null) {
                    translatedResult.Add(item.Value.Item2 + " <" + l + ">");
                } else {
                    translatedResult.Add(l);
                }
            });

            return translatedResult;
        }
        public List<string> Sort(List<string> levels) {
            List<string> result = new List<string>(levels);
            result = result.OrderBy((l) => {
                return items.IndexOf(items.Find(m => { return m.Item1.ToLower() == l.ToLower(); }));
            }).ToList();

            return result;
        }
        public List<string> Translate(List<string> levels) {
            List<string> result = new List<string>(levels);
            List<string> translatedResult = new List<string>();

            result.ForEach((l) => {
                translatedResult.Add(Translate(l));
            });

            return translatedResult;
        }
        public string Translate(string level) {
            int itemIndex = items.FindIndex(m => { return m.Item1.ToLower() == level.ToLower(); });
            if(itemIndex != -1) {
                return items[itemIndex].Item2 + " <" + level + ">";
            } else {
                return level;
            }
        }

        public static LevelTranslation levelTranslation_r2 = new LevelTranslation(new List<(string, string)>() {
            ("MENU", "Main Menu"),
            ("JAIL_10", "Prologue (Stormy Seas)"),
            ("JAIL_20", "Prologue (Jail)"),
            ("LEARN_10", "The Woods of Light"),
            ("MAPMONDE", "The Hall of Doors"),
            ("MAPMONDE2", "The Isle of Doors"),
            ("LEARN_30", "The Fairy Glade 1"),
            ("LEARN_31", "The Fairy Glade 2"),
            ("BAST_20", "The Fairy Glade 3"),
            ("BAST_22", "The Fairy Glade 4"),
            ("LEARN_60", "The Fairy Glade 5"),
            ("SKI_10", "The Marshes of Awakening 1"),
            ("SKI_60", "The Marshes of Awakening 2"),
            ("BATAM_10", "Meanwhile, on the Prison Ship"),
            ("CHASE_10", "The Bayou 1"),
            ("CHASE_22", "The Bayou 2"),
            ("LY_10", "The Walk of Life"),
            ("NEGO_10", "The Chamber of the Teensies"),
            ("WATER_10", "The Sanctuary of Water and Ice 1"),
            ("WATER_20", "The Sanctuary of Water and Ice 2"),
            ("POLOC_10", "Polokus - First Mask"),
            ("RODEO_10", "The Menhir Hills 1"),
            ("RODEO_40", "The Menhir Hills 2"),
            ("RODEO_60", "The Menhir Hills 3"),
            ("VULCA_10", "The Cave of Bad Dreams 1"),
            ("VULCA_20", "The Cave of Bad Dreams 2"),
            ("GLOB_30", "The Canopy 1"),
            ("GLOB_10", "The Canopy 2"),
            ("GLOB_20", "The Canopy 3"),
            ("WHALE_00", "Whale Bay 1"),
            ("WHALE_05", "Whale Bay 2"),
            ("WHALE_10", "Whale Bay 3"),
            ("PLUM_00", "The Sanctuary of Stone and Fire 1"),
            ("PLUM_20", "The Sanctuary of Stone and Fire 2"),
            ("PLUM_10", "The Sanctuary of Stone and Fire 3"),
            ("POLOC_20", "Polokus - Second Mask"),
            ("BAST_09", "The Echoing Caves (Intro)"),
            ("BAST_10", "The Echoing Caves 1"),
            ("CASK_10", "The Echoing Caves 2"),
            ("CASK_30", "The Echoing Caves 3"),
            ("NAVE_10", "The Precipice 1"),
            ("NAVE_15", "The Precipice 2"),
            ("NAVE_20", "The Precipice 3"),
            ("SEAT_10", "The Top of the World 1"),
            ("SEAT_11", "The Top of the World 2"),
            ("EARTH_10", "The Sanctuary of Rock and Lava 1"),
            ("EARTH_20", "The Sanctuary of Rock and Lava 2"),
            ("EARTH_30", "The Sanctuary of Rock and Lava 3"),
            ("LY_20", "The Walk of Power"),
            ("HELIC_10", "Beneath the Sanctuary of Rock and Lava 1"),
            ("HELIC_20", "Beneath the Sanctuary of Rock and Lava 2"),
            ("HELIC_30", "Beneath the Sanctuary of Rock and Lava 3"),
            ("POLOC_30", "Polokus - Third Mask"),
            ("MORB_00", "Tomb of the Ancients 1"),
            ("MORB_10", "Tomb of the Ancients 2"),
            ("MORB_20", "Tomb of the Ancients 3"),
            ("LEARN_40", "The Iron Mountains 1"),
            ("BALL", "The Iron Mountains (Balloon Flight)"),
            ("ILE_10", "The Iron Mountains 2 (The Gloomy Island)"),
            ("MINE_10", "The Iron Mountains 3 (The Pirate Mines)"),
            ("POLOC_40", "Polokus - Fourth Mask"),
            ("BATAM_20", "Meanwhile, on the Prison Ship (The Grolgoth)"),
            ("BOAT01", "The Prison Ship 1"),
            ("BOAT02", "The Prison Ship 2"),
            ("ASTRO_00", "The Prison Ship 3"),
            ("ASTRO_10", "The Prison Ship 4"),
            ("LIBER_10", "Freeing the Slaves"),
            ("RHOP_10", "The Crow's Nest"),
            ("END_10", "Ending"),
            ("STAFF_10", "Staff Roll"),
            ("BONUX", "Bonus Level"),
            ("GLOBVILL", "Globox Village"),
            ("B_PYRAM", "Minigame - Pyralums"),
            ("B_TOILE", "Minigame - Weblums"),
            ("GLODISC_CINE", "Minigame - Globox Disc (Intro)"),
            ("B_DISC", "Minigame - Globox Disc"),
            ("B_LIFT", "Minigame - Lift"),
            ("B_INVADE", "Minigame - Invade"),
            ("RAYCAP", "Score Recap"),
        });

        public static LevelTranslation levelTranslation_rarena_pc = new LevelTranslation(new List<(string, string)>() {
            ("menu", "Main Menu"),
            ("1Shadow", "(L1 Battle) Shadow Plain"),
            ("1Shrine", "(L1 Battle) Rise and Shrine"),
            ("1Sunset", "(L1 Battle) Sunset Coast"),
            ("2Haunted", "(L2 Battle) Haunted Yard"),
            ("2Palm", "(L2 Battle) Palm Beach"),
            ("2Timber", "(L2 Battle) Timber Wood"),
            ("3Coconut", "(L3 Battle) Coconut Island"),
            ("3Ly", "(L3 Battle) Ly's Palace"),
            ("3Spell", "(L3 Battle) Spellbound Forest"),
            ("4Forgot", "(L4 Battle) Forgotten Dungeon"),
            ("4Gem", "(L4 Battle) Gemstone Temple"),
            ("4Ghastly", "(L4 Battle) Ghastly Trees"),
            ("BSpooky", "(Bonus Battle) Spooky Towers"),
            ("crypt1", "(L1 Race) First Ruins"),
            ("crypt2", "(L1 Race) Nebulous Tower"),
            ("crypt3", "(L1 Race) Dark Sewer"),
            ("lagoon1", "(L2 Race) Dawn Sand"),
            ("lagoon2", "(L2 Race) Water Canyon"),
            ("lagoon3", "(L2 Race) Thousand Waterfalls"),
            ("pirate1", "(L3 Race) Forest Jump"),
            ("pirate2", "(L3 Race) Zenith Harbour"),
            ("pirate3", "(L3 Race) Treasure Ship"),
            ("factory1", "(L4 Race) Pipe Maze"),
            ("factory2", "(L4 Race) Lava Factory"),
            ("factory3", "(L4 Race) Electric Final"),
            ("bonus01", "(Bonus Race) Big Bang"),
            ("bonus", "(Bonus Race) Future"),
            ("scrol", "(Bonus Race) Born To Slide"),
            ("bonus2", "(Bonus Race) Speed Stress")
        });

        public static LevelTranslation levelTranslation_rarena_xboxgc = new LevelTranslation(new List<(string, string)>() {
            ("podium", "The Podium"),
            ("1Shadow", "(L1 Battle) Shadow Plain"),
            ("1Shrine", "(L1 Battle) Rise and Shrine"),
            ("1Sunset", "(L1 Battle) Sunset Coast"),
            ("2Haunted", "(L2 Battle) Haunted Yard"),
            ("2Palm", "(L2 Battle) Palm Beach"),
            ("2Timber", "(L2 Battle) Timber Wood"),
            ("3Coconut", "(L3 Battle) Coconut Island"),
            ("3Ly", "(L3 Battle) Ly's Palace"),
            ("3Spell", "(L3 Battle) Spellbound Forest"),
            ("4Forgot", "(L4 Battle) Forgotten Dungeon"),
            ("4Gem", "(L4 Battle) Gemstone Temple"),
            ("4Ghastly", "(L4 Battle) Ghastly Trees"),
            ("crypt1", "(L1 Race) First Ruins"),
            ("crypt2", "(L1 Race) Nebulous Tower"),
            ("crypt3", "(L1 Race) Dark Sewer"),
            ("lagoon1", "(L2 Race) Dawn Sand"),
            ("lagoon2", "(L2 Race) Water Canyon"),
            ("lagoon3", "(L2 Race) Thousand Waterfalls"),
            ("pirate1", "(L3 Race) Forest Jump"),
            ("pirate2", "(L3 Race) Zenith Harbour"),
            ("pirate3", "(L3 Race) Treasure Ship"),
            ("factory1", "(L4 Race) Pipe Maze"),
            ("factory2", "(L4 Race) Lava Factory"),
            ("factory3", "(L4 Race) Electric Final"),
            ("Pacbonus", "(Bonus Battle) Pac Arena"),
            ("BGravity01", "(Bonus Battle) Low-Gravity Arena"),
            ("BSpooky", "(Bonus Battle) Spooky Towers"),
            ("BDamien", "(Bonus Battle) KuraÃ¯"),
            ("scrol", "(Bonus Race) Speed Stress"),
            ("bonus2", "(Bonus Race) Run, Run"),
            ("mcstnx45", "(Bonus Race) Extreme Slide"),
            ("bonus", "(Bonus Race) Future"),
        });

        public static LevelTranslation levelTranslation_r3 = new LevelTranslation(new List<(string, string)>() {
            ( "menumap", "Main Menu" ),
            ( "intro_10", "The Fairy Council 1 (Murfy)" ),
            ( "intro_15", "The Fairy Council 2 (Finding Globox)" ),
            ( "Intro_17", "The Fairy Council 3 (Inside)" ),
            ( "intro_20", "The Fairy Council 4" ),
            ( "menu_00", "The Fairy Council 5 (Heart of the World)" ),
            ( "sk8_00", "The Fairy Council 6 (Teensie Highway)" ),
            ( "wood_11", "Clearleaf Forest 1" ),
            ( "Wood_10", "Clearleaf Forest 2" ),
            ( "Wood_19", "Clearleaf Forest 3" ),
            ( "Wood_50", "Clearleaf Forest 4 (Master Kaag)" ),
            ( "menu_10", "Clearleaf Forest 5 (Doctor's Office)" ),
            ( "Sk8_10", "Clearleaf Forest 6 (Teensie Highway)" ),
            ( "Swamp_60", "The Bog of Murk 1 (Bégoniax)" ),
            ( "Swamp_82", "The Bog of Murk 2" ),
            ( "Swamp_81", "The Bog of Murk 3" ),
            ( "swamp_83", "The Bog of Murk 4" ),
            ( "Swamp_50", "The Bog of Murk 5 (Razoff's Mansion)" ),
            ( "Swamp_51", "The Bog of Murk 6 (Razoff's Basement)" ),
            ( "Moor_00", "The Land of the Livid Dead 1" ),
            ( "Moor_30", "The Land of the Livid Dead 2" ),
            ( "moor_60", "The Land of the Livid Dead 3 (Tower)" ),
            ( "moor_19", "The Land of the Livid Dead 4 (Céloche)" ),
            ( "menu_20", "The Land of the Livid Dead 5 (Doctor's Office)" ),
            ( "Sk8_20", "The Land of the Livid Dead 6 (Teensie Highway)" ),
            ( "Knaar_10", "The Desert of the Knaaren 1" ),
            ( "Knaar_20", "The Desert of the Knaaren 2 (The Great Hall)" ),
            ( "Knaar_30", "The Desert of the Knaaren 3 (Tower)" ),
            ( "Knaar_45", "The Desert of the Knaaren 4" ),
            ( "Knaar_60", "The Desert of the Knaaren 5 (Arena)" ),
            ( "Knaar_69", "The Desert of the Knaaren 6 (Grimace Room)" ),
            ( "Knaar_70", "The Desert of the Knaaren 7" ),
            ( "menu_30", "The Desert of the Knaaren 8 (Doctor's Office)" ),
            ( "Flash_20", "The Longest Shortcut 1" ),
            ( "Flash_30", "The Longest Shortcut 2" ),
            ( "flash_10", "The Longest Shortcut 3" ),
            ( "Sea_10", "The Summit Beyond the Clouds 1 (The Looming Sea)" ),
            ( "mount_50", "The Summit Beyond the Clouds 2" ),
            ( "mount_4x", "The Summit Beyond the Clouds 3 (Snowboard)" ),
            ( "Fact_40", "Hoodlum Headquarters 1" ),
            ( "Fact_50", "Hoodlum Headquarters 2 (Firing Range)" ),
            ( "Fact_55", "Hoodlum Headquarters 3" ),
            ( "fact_34", "Hoodlum Headquarters 4 (Horrible Machine)" ),
            ( "Fact_22", "Hoodlum Headquarters 5 (Rising Lava)" ),
            ( "Tower_10", "The Tower of the Leptys 1" ),
            ( "Tower_20", "The Tower of the Leptys 2" ),
            ( "Tower_30", "The Tower of the Leptys 3" ),
            ( "Tower_40", "The Tower of the Leptys 4" ),
            ( "lept_15", "The Tower of the Leptys 5 (Final Battle)" ),
            ( "staff", "Staff Roll" ),
            ( "toudi_00", "Arcade - 2D Madness" ),
            ( "Ten_map", "Arcade - Racket Jump" ),
            ( "crush", "Arcade - Crush" ),
            ( "raz_map", "Arcade - Razoff Circus" ),
            ( "sentinel", "Arcade - Sentinel" ),
            ( "snipe_00", "Arcade - Missile Command" ),
            ( "ballmap", "Arcade - Balloons" ),
            ( "Ship_map", "Arcade - Special Invaders" ),
            ( "Commando", "Arcade - Commando" ),
            ( "roadrun", "Arcade - Mad Trax" ),
            ( "roadrun_4", "Arcade - Wheelis" ),
            ( "toudi_10", "Arcade - 2D Nightmare" ),
            ( "BonusTXT", "Bonus (Empty)" ),
            ( "endgame", "Endgame (Empty)" )
        });

    }
}
