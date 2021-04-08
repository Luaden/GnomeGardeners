using UnityEngine;

public class CarryingTool : CoreTool, ITool
{
    private GameObject heldItem = null;
    [SerializeField] private float dropRange = 3f;

    public GameObject HeldItem { get => heldItem; set => heldItem = value; }

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    public new void Interact(ITool tool = null)
    {
        // todo: gets equipped
        base.Interact(tool);

    }

    public new void UseTool(Vector3 origin, Vector3 direction, float distance)
    {
        lastHitTransform = null;
        base.UseTool(origin, direction, distance);

        if (heldItem != null)
        {
            if (heldItem.GetComponent<IHarvest>() != null)
            {
                if(heldItem.GetComponent<IHarvest>().Deliver(origin, direction, distance))
                {
                    heldItem = null;
                    return;
                }
            }

            heldItem.GetComponent<IHeldItem>().DropItem(origin + direction * dropRange, direction);
            heldItem = null;
                

        }

        if (lastHitTransform.GetComponent<IHeldItem>() != null)
        {
            heldItem = lastHitTransform.gameObject;
        }
            
        if(lastHitTransform.GetComponent<IObjectDispenser>() != null)
        {
            lastHitTransform.GetComponent<IObjectDispenser>().DispenseItem(this, "Plant");
        }
    }

    public new void DropItem(Vector3 position, Vector3 direction)
    {
        // todo: drop tool
        base.DropItem(position, direction);
        heldItem = null;
        Debug.Log("Dropped carrying tool.");
    }
}
