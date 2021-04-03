using UnityEngine;
using UnityEngine.InputSystem;

public class Gnome : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float interactRange = 5f;
    public ITool activeTool;
    private Vector2 direction = Vector2.zero;
    private Vector3 velocity;
    private Rigidbody body;
    private bool canMove = true;
    private Vector3 interactDirection = Vector3.back;

    // temp
    public GameObject activeToolObject;

    GnomeSkin skin;

    void Start()
    {
        velocity = new Vector3(speed, 0f, speed);
        body = GetComponent<Rigidbody>();

        // temp
        activeTool = activeToolObject.GetComponent<ITool>();

    }

    void FixedUpdate()
    {
        velocity = new Vector3(direction.x * speed, 0f, direction.y * speed);
        body.MovePosition(body.position + velocity * Time.fixedDeltaTime);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!canMove)
        {
            return;
        }
        direction = context.ReadValue<Vector2>();

        // calculate an interactionDirection vector which can never be 0
        if(direction.x != 0 && direction.y != 0)
        {
            interactDirection.x = direction.x > 0f ? 1f : -1f;
            interactDirection.z = direction.y > 0f ? 1f : -1f;
        }
        else if(direction.x != 0 && direction.y == 0)
        {
            interactDirection.x = direction.x > 0f ? 1f : -1f;
            interactDirection.z = direction.y;
        }
        else if (direction.x == 0 && direction.y != 0)
        {
            interactDirection.x = direction.x;
            interactDirection.z = direction.y > 0f ? 1f : -1f;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        // todo: interact with tools
        Debug.DrawLine(transform.position, transform.position + interactDirection * interactRange);
        RaycastHit hit;
        if(Physics.Raycast(transform.position, interactDirection, out hit, interactRange))
        {
            if(hit.transform.GetComponent<ITool>() != null)
            {
                ChangeArm(hit.transform.GetComponent<ITool>());

                // temp: move tool to separate container
                hit.transform.GetComponent<ITool>().Interact();
            }

            // todo: check IHarvest, IInteractable etc..
        }
        else if (activeTool != null)
        {
            activeTool.UseTool();
        }
    }

    public void OnDropItem(InputAction.CallbackContext context)
    {
        activeTool.DropItem(transform.position + interactDirection);
    }
    
    public void ChangeArm(ITool tool)
    {
        // todo: change animation sprite
        activeTool = tool;
    }

}
