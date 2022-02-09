namespace Raymap {
    /// <summary>
    /// The engine categories (purely for the settings window)
    /// </summary>
    public enum EngineCategory {
        [EngineCategory(Engine.CPA, "Tonic Trouble")] CPA_TonicTrouble,
        [EngineCategory(Engine.CPA, "Rayman 2")] CPA_Rayman2,
        [EngineCategory(Engine.CPA, "Rayman M")] CPA_RaymanM,
        [EngineCategory(Engine.CPA, "Rayman 3")] CPA_Rayman3,
        [EngineCategory(Engine.CPA, "Rayman Raving Rabbids")] CPA_RaymanRavingRabbids,
        [EngineCategory(Engine.CPA, "Licensed - Disney")] CPA_Disney,
        [EngineCategory(Engine.CPA, "Licensed - Playmobil")] CPA_Playmobil,
        [EngineCategory(Engine.CPA, "Other")] CPA_Other,
    }
}