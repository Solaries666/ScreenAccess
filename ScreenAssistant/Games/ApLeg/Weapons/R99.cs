﻿namespace TiqSoft.ScreenAssistant.Games.ApLeg.Weapons
{
    internal sealed class R99 : UniqueLogicWeapon
    {
        public R99(string name, double burstSeconds, string recognizedName) 
            : base(name, burstSeconds, recognizedName, WeaponAL.R99)
        {
        }

        public override double AdjustMouse(int shotNumber)
        {
            AdjustmentCoefficient = CalculateAdjustment(shotNumber, 35);
            var horizontalOffset = Rnd.NextDouble() * -1 - 1;
            var verticalOffset = Rnd.NextDouble() + 7;
            MoveMouse(horizontalOffset, verticalOffset);

            return GetAdjustmentTime(1d);
        }
    }
}