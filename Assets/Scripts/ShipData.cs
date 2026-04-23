using UnityEngine;

[CreateAssetMenu(fileName = "NewShip", menuName = "Ships/Ship Data")]
public class ShipData : ScriptableObject
{
    public string shipName;

    [Header("Public Stats")]
    // 0 - 10 possible
    public int speed;
    public int accel;
    public int handling;
    public int weight;

    [Header("Hidden Stats")]
    // 0 - 10 possible
    public int maxCharge;
    public int rechargeRate;
    public string specialty = "n/a";
    public float boostSpeed;

    [Header("Model")]
    public GameObject shipModelPrefab;
    public Vector3 shipModelScaleFactor;
    public Sprite icon;
}
