﻿namespace SurvivalTemplatePro.WieldableSystem
{
    public class FirearmInfiniteAmmo : FirearmAmmoBehaviour
    {
        public override int RemoveAmmo(int amount) => amount;
        public override int AddAmmo(int amount) => amount;
        public override int GetAmmoCount() => 1000;
    }
}