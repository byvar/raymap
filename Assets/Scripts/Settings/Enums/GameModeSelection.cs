using System.ComponentModel;

namespace Raymap {
    /// <summary>
    /// The available games
    /// </summary>
    public enum GameModeSelection {
        [GameMode(EngineCategory.CPA_Rayman2, typeof(LegacyGameManager), "Rayman 2 (PC)")] Rayman2PC,
        [GameMode(EngineCategory.CPA_Rayman2, typeof(LegacyGameManager), "R2 (PC) Demo (1999/08/19)")] Rayman2PCDemo_1999_08_18,
        [GameMode(EngineCategory.CPA_Rayman2, typeof(LegacyGameManager), "R2 (PC) Demo (1999/09/11)")] Rayman2PCDemo_1999_09_04,
        [GameMode(EngineCategory.CPA_Rayman2, typeof(LegacyGameManager), "Rayman 2 (DC)")] Rayman2DC,
        [GameMode(EngineCategory.CPA_Rayman2, typeof(LegacyGameManager), "Rayman 2 (iOS)")] Rayman2IOS,
        [GameMode(EngineCategory.CPA_Rayman2, typeof(LegacyGameManager), "Rayman 2 (iOS) Demo")] Rayman2IOSDemo,
        [GameMode(EngineCategory.CPA_Rayman2, typeof(CPA_PS1Manager), "Rayman 2 (PS1)")] Rayman2PS1,
        [GameMode(EngineCategory.CPA_Rayman2, typeof(LegacyGameManager), "Rayman 2 (PS2)")] Rayman2PS2,
        [GameMode(EngineCategory.CPA_Rayman2, typeof(CPA_U64Manager), "Rayman 2 (N64)")] Rayman2N64,
        [GameMode(EngineCategory.CPA_Rayman2, typeof(CPA_U64Manager), "Rayman 2 (DS)")] Rayman2DS,
        [GameMode(EngineCategory.CPA_Rayman2, typeof(CPA_U64Manager), "Rayman 2 (3DS)")] Rayman23DS,

        [GameMode(EngineCategory.CPA_RaymanM, typeof(LegacyGameManager), "Rayman M (PC)")] RaymanMPC,
        [GameMode(EngineCategory.CPA_RaymanM, typeof(LegacyGameManager), "Rayman M (PS2)")] RaymanMPS2,
        [GameMode(EngineCategory.CPA_RaymanM, typeof(LegacyGameManager), "RM (PS2) Demo (2001/07/25)")] RaymanMPS2Demo_2001_07_25,
        [GameMode(EngineCategory.CPA_RaymanM, typeof(LegacyGameManager), "Rayman Arena (PC)")] RaymanArenaPC,
        [GameMode(EngineCategory.CPA_RaymanM, typeof(LegacyGameManager), "Rayman Arena (PS2)")] RaymanArenaPS2,
        [GameMode(EngineCategory.CPA_RaymanM, typeof(LegacyGameManager), "Rayman Arena (GC)")] RaymanArenaGC,
        [GameMode(EngineCategory.CPA_RaymanM, typeof(LegacyGameManager), "RA (GC) Demo (2002/03/07)")] RaymanArenaGCDemo_2002_03_07,
        [GameMode(EngineCategory.CPA_RaymanM, typeof(LegacyGameManager), "Rayman Arena (Xbox)")] RaymanArenaXbox,
        [GameMode(EngineCategory.CPA_RaymanM, typeof(CPA_PS1Manager), "Rayman Rush (PS1)")] RaymanRushPS1,

        [GameMode(EngineCategory.CPA_Rayman3, typeof(LegacyGameManager), "Rayman 3 (PC)")] Rayman3PC,
        [GameMode(EngineCategory.CPA_Rayman3, typeof(LegacyGameManager), "R3 (PC) Demo (2002/10/01)")] Rayman3PCDemo_2002_10_01,
        [GameMode(EngineCategory.CPA_Rayman3, typeof(LegacyGameManager), "R3 (PC) Demo (2002/10/21)")] Rayman3PCDemo_2002_10_21,
        [GameMode(EngineCategory.CPA_Rayman3, typeof(LegacyGameManager), "R3 (PC) Demo (2002/12/09)")] Rayman3PCDemo_2002_12_09,
        [GameMode(EngineCategory.CPA_Rayman3, typeof(LegacyGameManager), "R3 (PC) Demo (2003/01/06)")] Rayman3PCDemo_2003_01_06,
        [GameMode(EngineCategory.CPA_Rayman3, typeof(LegacyGameManager), "Rayman 3 (MacOS)")] Rayman3MacOS,
        [GameMode(EngineCategory.CPA_Rayman3, typeof(LegacyGameManager), "Rayman 3 (GC)")] Rayman3GC,
        [GameMode(EngineCategory.CPA_Rayman3, typeof(LegacyGameManager), "Rayman 3 (PS2)")] Rayman3PS2,
        [GameMode(EngineCategory.CPA_Rayman3, typeof(LegacyGameManager), "R3 (PS2) Demo (2002/05/17)")] Rayman3PS2Demo_2002_05_17,
        [GameMode(EngineCategory.CPA_Rayman3, typeof(LegacyGameManager), "R3 (PS2) Demo (2002/08/07)")] Rayman3PS2Demo_2002_08_07,
        [GameMode(EngineCategory.CPA_Rayman3, typeof(LegacyGameManager), "R3 (PS2) Dev Build (2002/09/06)")] Rayman3PS2DevBuild_2002_09_06,
        [GameMode(EngineCategory.CPA_Rayman3, typeof(LegacyGameManager), "R3 (PS2) Demo (2002/10/29)")] Rayman3PS2Demo_2002_10_29,
        [GameMode(EngineCategory.CPA_Rayman3, typeof(LegacyGameManager), "R3 (PS2) Demo (2002/12/18)")] Rayman3PS2Demo_2002_12_18,
        [GameMode(EngineCategory.CPA_Rayman3, typeof(LegacyGameManager), "Rayman 3 (Xbox)")] Rayman3Xbox,
        [GameMode(EngineCategory.CPA_Rayman3, typeof(LegacyGameManager), "Rayman 3 (Xbox 360)")] Rayman3Xbox360,
        [GameMode(EngineCategory.CPA_Rayman3, typeof(LegacyGameManager), "Rayman 3 (PS3)")] Rayman3PS3,

        [GameMode(EngineCategory.CPA_RaymanRavingRabbids, typeof(CPA_U64Manager), "Rayman Raving Rabbids (DS)")] RaymanRavingRabbidsDS,
        [GameMode(EngineCategory.CPA_RaymanRavingRabbids, typeof(CPA_U64Manager), "Rayman Raving Rabbids (DS) Dev Build (2006/05/25)")] RaymanRavingRabbidsDSDevBuild_2006_05_25,
        
        [GameMode(EngineCategory.CPA_TonicTrouble, typeof(LegacyGameManager), "Tonic Trouble (PC)")] TonicTroublePC,
        [GameMode(EngineCategory.CPA_TonicTrouble, typeof(LegacyGameManager), "Tonic Trouble: SE (PC)")] TonicTroubleSEPC,
        [GameMode(EngineCategory.CPA_TonicTrouble, typeof(CPA_U64Manager), "Tonic Trouble (N64 NTSC)")] TonicTroubleN64,

        [GameMode(EngineCategory.CPA_Disney, typeof(LegacyGameManager), "Donald Duck: Quack Attack (PC)")] DonaldDuckPC,
        [GameMode(EngineCategory.CPA_Disney, typeof(LegacyGameManager), "Donald Duck: Quack Attack (PC) Demo")] DonaldDuckPCDemo,
        [GameMode(EngineCategory.CPA_Disney, typeof(LegacyGameManager), "Donald Duck: Quack Attack (DC)")] DonaldDuckDC,
        [GameMode(EngineCategory.CPA_Disney, typeof(CPA_U64Manager), "Donald Duck: Quack Attack (N64)")] DonaldDuckN64,
        [GameMode(EngineCategory.CPA_Disney, typeof(CPA_PS1Manager), "Donald Duck: Quack Attack (PS1)")] DonaldDuckPS1,
        [GameMode(EngineCategory.CPA_Disney, typeof(LegacyGameManager), "Donald Duck: PK (GC)")] DonaldDuckPKGC,
        [GameMode(EngineCategory.CPA_Disney, typeof(LegacyGameManager), "Disney's Dinosaur (PC)")] DinosaurPC,
        [GameMode(EngineCategory.CPA_Disney, typeof(CPA_PS1Manager), "Jungle Book: Groove Party (PS1)")] JungleBookPS1,

        [GameMode(EngineCategory.CPA_Playmobil, typeof(LegacyGameManager), "Playmobil: Hype (PC)")] PlaymobilHypePC,
        [GameMode(EngineCategory.CPA_Playmobil, typeof(LegacyGameManager), "Playmobil: Laura (PC)")] PlaymobilLauraPC,
        [GameMode(EngineCategory.CPA_Playmobil, typeof(LegacyGameManager), "Playmobil: Alex (PC)")] PlaymobilAlexPC,

        [GameMode(EngineCategory.CPA_Other, typeof(LegacyGameManager), "Largo Winch (PC)")] LargoWinchPC,
        [GameMode(EngineCategory.CPA_Other, typeof(CPA_PS1Manager), "VIP (PS1)")] VIPPS1,
        [GameMode(EngineCategory.CPA_Other, typeof(LegacyGameManager), "Red Planet (PC)")] RedPlanetPC,
    }
}