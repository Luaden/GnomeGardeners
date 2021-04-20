using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Ground : MonoBehaviour, IInteractable
{
    public GroundType type = GroundType.Count;

    [SerializeField] private Sprite arableSprite;
    [SerializeField] private Sprite grassSprite;
    [SerializeField] private Sprite dirtSprite;
    [SerializeField] private Sprite sandSprite;

    private delegate void TypeChanged(GroundType type);
    private event TypeChanged typeChanged;
    private SpriteRenderer spriteRenderer;

    #region Unity Methods
    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        typeChanged += UpdateSprite;
    }
    #endregion

    #region Public Methods
    public void Interact(ITool tool = null)
    {
        if(tool == null)
        {
        }
        else
        {
            switch (tool.Type)
            {
                case ToolType.Carrying:
                    break;
                case ToolType.Digging:
                    switch (type)
                    {
                        case GroundType.Arable:
                            break;
                        case GroundType.Grass:
                            break;
                        case GroundType.Dirt:
                            type = GroundType.Arable;
                            break;
                        case GroundType.Sand:
                            break;
                        default:
                            break;
                    }
                    break;
                case ToolType.Watering:
                    break;
                case ToolType.Harvesting:
                    break;
                default:
                    break;
            }

            typeChanged.Invoke(type);
        }
    }
    #endregion

    #region Private Methods
    private void UpdateSprite(GroundType type)
    {
        switch (type)
        {
            case GroundType.Arable:
                spriteRenderer.sprite = arableSprite;
                break;
            case GroundType.Grass:
                spriteRenderer.sprite = grassSprite;
                break;
            case GroundType.Dirt:
                spriteRenderer.sprite = dirtSprite;
                break;
            case GroundType.Sand:
                spriteRenderer.sprite = sandSprite;
                break;
            default:
                break;
        }
    }
    #endregion
}