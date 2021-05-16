using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeGardeners
{
    public class Plant : MonoBehaviour, IInteractable, IHoldable
    {
        private bool debug = false;

        public Species species;
        public float timeToGrowVariation = 1f;

        private Sprite spriteInHand;

        private Stage currentStage;
        private float lastStageTimeStamp;
        private float currentGrowTime;
        private bool isOnArableGround;
        private bool isBeingCarried;
        private bool isDecayed;
        private GridCell occupyingCell;
        private float currentNeedValue;
        private bool isNeedFulfilled;
        private ItemType type;
        private float randomizedTimeToGrow;
        private GameObject popUp;
        private AudioSource audioSource;

        public bool IsBeingCarried { get => isBeingCarried; set => isBeingCarried = value; }
        public Stage CurrentStage { get => currentStage; }
        public GameObject AssociatedObject { get => gameObject; }
        public Sprite SpriteInHand { get => spriteInHand; set => spriteInHand = value; }
        public ItemType Type { get => type; set => type = value; }

        public SpriteRenderer spriteRenderer;

        public VoidEventChannelSO OnTileChanged;

        #region Unity Methods

        private void Awake()
        {
            Configure();
        }

        private void Start()
        {
            AssignOccupant();
        }

        private void Update()
        {
            TryGrowing();
            CheckNeedPopUp();
        }

        private void OnDestroy()
        {
            Dispose();
        }
        #endregion

        #region Public Methods
        public void Interact(Tool tool = null)
        {
        }

        public void Destroy()
        {
            occupyingCell.RemoveCellOccupant();
            Destroy(gameObject);
            GameManager.Instance.AudioManager.PlaySound(SoundType.sfx_plants_snapping, audioSource);
        }

        public void HarvestPlant(GridCell cell)
        {
            // todo: object pool stash
            if (currentStage.isHarvestable)
            {
                cell.RemoveCellOccupant();
                isBeingCarried = true;
                GameManager.Instance.AudioManager.PlaySound(SoundType.sfx_tool_cutting_plant);
                ClearPopUp();
                gameObject.SetActive(false);
            }
        }

        public void AddToNeedValue(NeedType type, float amount)
        {
            if(type != currentStage.need.type)
            {
                DebugLogger.Log(this, "Incorrect need type.");
                return;
            }

            DebugLogger.Log(this, "Adding" + amount.ToString() + " to need value.");
            currentNeedValue += amount;

            if(currentNeedValue >= currentStage.need.threshold)
            {
                isNeedFulfilled = true;
            }

            if(type == NeedType.Fertilizer)
                GameManager.Instance.AudioManager.PlaySound(SoundType.sfx_plant_fertilized, audioSource);

        }

        public void PlantSeed(GridCell cell)
        {
            if (currentStage.specifier != PlantStage.Seed)
                return;

            transform.position = cell.WorldPosition;
            cell.AddCellOccupant(this);
            occupyingCell = cell;
            isBeingCarried = false;
            GameManager.Instance.AudioManager.PlaySound(SoundType.sfx_plant_planting, audioSource);
            CheckArableGround();
        }

        public void AssignOccupant()
        {
            GameManager.Instance.GridManager.ChangeTileOccupant(GameManager.Instance.GridManager.GetClosestGrid(AssociatedObject.transform.position), this);
        }
        #endregion

        #region Private Methods
        private void TryGrowing()
        {
            if (isDecayed) { return; }

            if (!isOnArableGround) { return; }

            DebugLogger.LogUpdate(this, "Tries Growing on arable ground.");
            currentGrowTime = GameManager.Instance.Time.GetTimeSince(lastStageTimeStamp) * species.growMultiplier;

            if (currentGrowTime >= currentStage.timeToFulfillNeed)
            {
                if (isNeedFulfilled)
                {
                    if (currentGrowTime >= currentStage.timeToFulfillNeed + randomizedTimeToGrow)
                        AdvanceStages();
                }
                else
                    AdvanceToDecayedStage();
            }
        }

        private void CheckNeedPopUp()
        {
            if (popUp == null && currentNeedValue < currentStage.need.threshold)
                GetPopUp(currentStage.need.popUpType);

            else if (popUp != null && isNeedFulfilled)
            {
                ClearPopUp();
            }
        }

        private void AdvanceToDecayedStage()
        {
            DebugLogger.Log(this, "Grew into decayed stage.");
            currentStage = species.decayedStage;
            lastStageTimeStamp = GameManager.Instance.Time.ElapsedTime;
            spriteRenderer.sprite = species.decayedStage.sprite;
            name = "Decayed" + species.name;
            currentNeedValue = 0f;
            isNeedFulfilled = false;
            isDecayed = true;
            GameManager.Instance.GridManager.ChangeTile(occupyingCell.GridPosition, GroundType.FallowSoil);
            spriteInHand = species.deadSprite;
            GameManager.Instance.AudioManager.PlaySound(SoundType.sfx_plant_wilting, audioSource);
            GetPopUp(PoolKey.PopUp_Recycle);
        }

        private void AdvanceStages()
        {
            DebugLogger.Log(this, "Grew into stage: " + currentStage.specifier.ToString());
            var currentStageIndex = species.stages.IndexOf(currentStage);
            currentStage = species.NextStage(currentStageIndex);
            lastStageTimeStamp = GameManager.Instance.Time.ElapsedTime;
            spriteRenderer.sprite = currentStage.sprite;
            name = currentStage.name + " " + species.name;
            currentNeedValue = 0f;
            isNeedFulfilled = false;
            if(currentStage.specifier == PlantStage.Ripening) 
            { 
                spriteInHand = species.harvestSprite;
            }
            if(type == ItemType.Seed) 
            {
                type = ItemType.Harvest;
            }

            randomizedTimeToGrow = currentStage.timeToGrow + UnityEngine.Random.Range(-timeToGrowVariation, timeToGrowVariation);

            if(popUp != null)
            {
                popUp.gameObject.SetActive(false);
                popUp = null;
            }        
        }

        private void CheckArableGround()
        {
            if(occupyingCell.GroundType == GroundType.ArableSoil)
            {
                DebugLogger.Log(this, "Found Arable Ground!");
                isOnArableGround = true;
                lastStageTimeStamp = GameManager.Instance.Time.ElapsedTime;
            }
            else
            {
                isOnArableGround = false;
            }
        }

        private void CheckOccupyingCell()
        {
            occupyingCell = GameManager.Instance.GridManager.GetClosestCell(transform.position);
        }

        private void Configure()
        {
            if (species.stages.Count > 0)
                currentStage = species.stages[0];
            else
                DebugLogger.LogWarning(this, "Plant.cs: Species not selected or set-up.");
            isOnArableGround = false;
            spriteRenderer.sprite = currentStage.sprite;
            name = currentStage.name+" "+species.name;
            currentNeedValue = 0f;
            isBeingCarried = true;
            spriteInHand = species.prematureSprite;
            type = ItemType.Seed;
            isNeedFulfilled = false;
            randomizedTimeToGrow = currentStage.timeToGrow + UnityEngine.Random.Range(-timeToGrowVariation, timeToGrowVariation);
            OnTileChanged.OnEventRaised += CheckOccupyingCell;
            OnTileChanged.OnEventRaised += CheckArableGround;
            audioSource = GetComponent<AudioSource>();
        }

        private void Dispose()
        {
            DebugLogger.Log(this, "Being disposed of.");
            ClearPopUp();
            spriteRenderer.sprite = null;
            name = species.name;
            currentStage = null;
            OnTileChanged.OnEventRaised -= CheckOccupyingCell;
            OnTileChanged.OnEventRaised -= CheckArableGround;
        }

        private void GetPopUp(PoolKey popUpType)
        {
            ClearPopUp();

            GameObject newPopUp = GameManager.Instance.PoolController.GetObjectFromPool(transform.position + currentStage.popUpPositionOffset, Quaternion.identity, popUpType);
            popUp = newPopUp;
        }

        private void ClearPopUp()
        {
            if(popUp != null)
            {
                popUp.gameObject.SetActive(false);
                popUp = null;
            }        
        }

        #endregion
    }
}
