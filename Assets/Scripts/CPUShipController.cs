using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class CPUShipController : MonoBehaviour
{

    [Header("Selection")]
    public CharacterData currentCharacter;
    public ShipData currentShip;
    public GameDatabase gameDatabase;
    public bool randomize = true;

    private ShipManager shipManager;

    void Awake()
    {
        shipManager = gameObject.GetComponent<ShipManager>();
    }

    void RandomizeSelection()
    {
        // get length of arrays
        int char_n = gameDatabase.characters.Count();
        int ship_n = gameDatabase.ships.Count();

        // get random number in the range of objects
        int char_r = Random.Range(0, char_n);
        int ship_r = Random.Range(0, ship_n);
        
        // set selection to the randomly selected one
        shipManager.currentCharacter = gameDatabase.characters[char_r];
        shipManager.currentShip = gameDatabase.ships[ship_r];
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
