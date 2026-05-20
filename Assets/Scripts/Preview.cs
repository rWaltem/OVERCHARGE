using UnityEngine;

public class Preview : MonoBehaviour
{
    public void PreviewCharacter(CharacterData charData)
    {

        // remove all chilren of gameobject
        foreach (Transform child in transform)
        {
            //Debug.Log("Removing children");
            Destroy(child.gameObject);
        }

        // spawn new preview model as child
        //Debug.Log("Spawn new preview");

        GameObject preview = Instantiate(
            charData.characterModelPrefab,
            transform
        );

        // reset local position/rotation
        preview.transform.localPosition = Vector3.zero;
        preview.transform.localRotation = Quaternion.identity;

        // scale up
        preview.transform.localScale = Vector3.one * charData.previewSize;
    }

    public void PreviewShip(ShipData shipData)
    {
        // remove all chilren of gameobject
        foreach (Transform child in transform)
        {
            //Debug.Log("Removing children");
            Destroy(child.gameObject);
        }

        // spawn new preview model as child
        //Debug.Log("Spawn new preview");

        GameObject preview = Instantiate(
            shipData.shipModelPrefab,
            transform
        );

        // reset local position/rotation
        preview.transform.localPosition = Vector3.zero + new Vector3(0, 0, -75);
        preview.transform.localRotation = Quaternion.identity;

        // scale up
        preview.transform.localScale = Vector3.one * shipData.previewSize;
    }
}
