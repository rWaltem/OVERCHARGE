using UnityEngine;

public class CPUShipController : MonoBehaviour
{

    [Header("Selection")]
    public CharacterData currentCharacter;
    public ShipData currentShip;
    public bool randomize = true;

    private ShipManager shipManager;

    void Awake()
    {
        shipManager = gameObject.GetComponent<ShipManager>();
    }

    void RandomizeSelection()
    {
        // TODO: randzomize logic here
        
        // temp hard code
        shipManager.currentCharacter = currentCharacter;
        shipManager.currentShip = currentShip;
    }

    void Start()
    {
        if (!randomize) 
        {
            shipManager.currentCharacter = currentCharacter;
            shipManager.currentShip      = currentShip;
        } else {
            RandomizeSelection();
        }
    }

    // set ship inputs
    void UpdateInputs(float throttle, float brake, float steering, bool boost)
    {
        shipManager.SetInput(
            throttle : throttle,
            brake    : brake,
            steering : steering,
            boost    : boost
        );
    } 

    // Update is called once per frame
    void Update()
    {
        //TODO: Follow race line

        UpdateInputs(
            0f,
            0f,
            0f,
            false
        );
    }
}
