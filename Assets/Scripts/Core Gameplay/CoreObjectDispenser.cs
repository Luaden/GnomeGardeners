using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreObjectDispenser : MonoBehaviour, IInteractable
{
    [SerializeField] List<GameObject> dispensables;

    public GameObject GameObject => gameObject;

    #region Public Methods
    public void DispenseItem(Tool tool, string itemName)
    {
        Tool toolUsed = tool;

        if (toolUsed == null || toolUsed.Type != ToolType.Harvesting)
            return;

        foreach(GameObject item in dispensables)
        {

            if (itemName == item.name)
            {
                GameObject newPlant = Instantiate(item, transform.position, transform.rotation);
                // toolUsed.HeldItem = newPlant;
                newPlant.SetActive(false);
            }
            return;
        }

        Debug.Log("No object with name " + itemName + " found in list.");
    }

    public void DispenseItem(Vector3 dropLocation, string itemName)
    {
        throw new System.NotImplementedException();
    }

    public void Interact(Tool tool = null)
    {
        DispenseItem(tool, "Plant");
    }
    #endregion
}