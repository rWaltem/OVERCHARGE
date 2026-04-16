using UnityEngine;

[CreateAssetMenu(fileName = "GlobalShipModifiers", menuName = "Game/Global Ship Modifiers")]
public class GlobalShipModifiers : ScriptableObject
{
    [Header("Modifiers")]
    public float recoverySpeedMod = 1f;
    public float speedMod = 1f;
    public float accelMod = 1f;
    public float weightMod = 1f;
    public float handlingMod = 1f;
    public float maxChargeMod = 1f;
    public float rechargeRateMod = 1f;
    public float matchingBoostMod = 1f;
    public float specialtyBoostMod = 1f;
    public float boostDuration = 1f;
    public float boostCost = 1f;
}