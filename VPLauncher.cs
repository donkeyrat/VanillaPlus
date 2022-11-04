using BepInEx;

namespace VanillaPlus {

    [BepInPlugin("teamgrad.vanillaplus", "VanillaPlus", "2.1.0")]
    public class VPLauncher : BaseUnityPlugin {

        public VPLauncher()
        {
            VPBinder.UnitGlad();
        }
    }
}