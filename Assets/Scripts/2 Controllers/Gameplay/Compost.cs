using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compost : CoreObjectDispenser
{
    #region Unity Methods

    private void Start()
    {
        AssignOccupant();
    }

    #endregion

    #region Public Methods

    public new void Interact(Tool tool = null)
    {
        Log("Interacting.");
        if (tool != null && tool.Type == ToolType.Harvesting)
        {
            DispenseItem(tool);
        }
        else
        {
            return; // todo: implement drop on floor
        }
    }

    public new void AssignOccupant()
    {
        GameManager.Instance.GridManager.ChangeTileOccupant(GameManager.Instance.GridManager.GetClosestGrid(AssociatedObject.transform.position), this);
    }

    #endregion

    #region Private Methods

    private void DispenseItem(Tool tool)
    {
        Log("Dispensing.");
        var dispensedItem = Instantiate(dispensable, transform.parent);
        tool.heldItem = dispensedItem.GetComponent<IHoldable>();
        Log(dispensedItem.ToString());
    }

    private void DispenseItem(Vector2Int dropLocation)
    {
        throw new System.NotImplementedException();
    }

    private void Log(string msg)
    {
        Debug.Log("[Compost]: " + msg);
    }
    private void LogWarning(string msg)
    {
        Debug.LogWarning("[Compost]: " + msg);
    }

    #endregion
}
