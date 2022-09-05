using BepInEx;

namespace VanillaPlus {

    [BepInPlugin("teamgrad.vanillaplus", "VanillaPlus", "1.1.2")]
    public class VPLauncher : BaseUnityPlugin {

        public VPLauncher() { VPBinder.UnitGlad(); }
    }
}