using BepInEx;

namespace VanillaPlus {

    [BepInPlugin("teamgrad.vanillaplus", "VanillaPlus", "2.0.0")]
    public class VPLauncher : BaseUnityPlugin {

        public VPLauncher()
        {
            VPBinder.UnitGlad();
        }
    }
}