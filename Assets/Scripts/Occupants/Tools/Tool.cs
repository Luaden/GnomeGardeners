using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

namespace GnomeGardeners
{
    public abstract class Tool : Occupant
    {
        [SerializeField] private Sprite icon;
        [SerializeField] protected GameObject toolTutorial;
        [SerializeField] protected bool isTutorialTool = false;

        private bool tutorialComplete = false;
        protected AudioSource audioSource;
        protected ToolType toolType;
        protected bool isEquipped;

        private bool isEquipping = false;
        private SpriteEventChannelSO OnEquippedToolSprite;
        private ToolTutorialEventChannelSO OnFirstEquip; 
        public AudioSource AudioSource { get => audioSource; }
        public ToolType ToolType { get => ToolType; }
        #region Unity Methods

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private new void Start()
        {
            base.Start();
            OnEquippedToolSprite = Resources.Load<SpriteEventChannelSO>("Channels/EquippedToolSpriteEC");
            OnFirstEquip = Resources.Load<ToolTutorialEventChannelSO>("Channels/ToolTutorialEventEC");
        }

        #endregion

        #region Public Methods

        public abstract void UseTool(GridCell cell);

        public void Unequip(GridCell targetCell)
        {
            isEquipping = false;
            cell = targetCell;

            if (targetCell.Occupant != null) 
                return;

            transform.position = targetCell.WorldPosition;
            gameObject.SetActive(true);
            isEquipped = false;
            AddOccupantToCells(targetCell);

            PlayUnequipSound(targetCell.GroundType);
            isEquipping = true;

        }


        public void Equip()
        {
            if (!tutorialComplete && isTutorialTool)
            {
                OnFirstEquip.RaiseEvent(toolTutorial);
                tutorialComplete = true;
            }

            isEquipping = false;
            RemoveOccupantFromCells();
            gameObject.SetActive(false);
            isEquipped = true;
            isEquipping = true;

            OnEquippedToolSprite.RaiseEvent(icon);
            if(popUp != null)
                ClearPopUp();
        }

        public abstract void UpdateSpriteResolvers(SpriteResolver[] resolvers);

        public virtual void UpdateItemRenderers(SpriteRenderer[] renderers)
        {
               
        }
        
        public virtual void UpdateToolRenderers(SpriteRenderer[] renderers)
        {
               
        }

        public virtual void PlayCorrespondingAnimation(Animator animator, string prefix)
        {
            if(isEquipping)
                animator.Play(prefix + "_pick_up");
        }

        #endregion

        #region Private Methods

        private void PlayUnequipSound(GroundType ground)
        {
            switch (ground)
            {
                case GroundType.Grass:
                    GameManager.Instance.AudioManager.PlaySound(SoundType.sfx_tool_thud_on_grass, audioSource);

                    break;
                case GroundType.Path:
                    GameManager.Instance.AudioManager.PlaySound(SoundType.sfx_tool_thud_on_gravel, audioSource);

                    break;
                case GroundType.FallowSoil:
                case GroundType.ArableSoil:
                    GameManager.Instance.AudioManager.PlaySound(SoundType.sfx_tool_thud_on_dirt, audioSource);

                    break;
            }
        }

        #endregion
    }
}
