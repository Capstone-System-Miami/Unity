//Author: Johnny, Layla Hoey
using SystemMiami.Animation;
using SystemMiami.Enums;
using SystemMiami.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using SystemMiami.Management;
using System;

namespace SystemMiami
{
    public class TopDownMovement : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private dbug log;

        [Header("Settings")]
        public float walkSpeed;
        public float frameRate;

        [Header("Inspect")]
        /// <summary>
        /// Must be assigned at runtime by <see cref="CharClassAnimationDriver"/>
        /// </summary>
        [SerializeField, ReadOnly] private StandardAnimSet animSet;
        [field: SerializeField, ReadOnly] public bool CanMove { get; private set; } = false;


        // Components
        private Rigidbody2D body;
        private SpriteRenderer spriteRenderer;
        private Animator animator;

        // Input
        private Vector2 rawInput;
        private Vector2Int input;
        private Vector2 moveDirection;

        private void Awake()
        {
            if (body == null && !TryGetComponent(out body))
            {
                log.error($"{name}'s {this} couldnt find a Rigidbody2D");
            }

            if (spriteRenderer == null && !TryGetComponent(out spriteRenderer))
            {
                log.error($"{name}'s {this} couldnt find a SpriteRenderer");
            }

            if (animator == null && !TryGetComponent(out animator))
            {
                log.error($"{name}'s {this} couldnt find an animator");
            }

            DisableMovement();
        }

        private void OnEnable()
        {
            DisableMovement();
            SceneManager.sceneLoaded += HandleSceneLoaded;
            GAME.MGR.GamePaused += HandleGamePause;
            GAME.MGR.GameResumed += HandleGameResume;
            UI.MGR.CharacterMenuOpened += HandleCharacterMenuOpened;
            UI.MGR.CharacterMenuClosed += HandleCharacterMenuClosed;
        }

        private void OnDisable()
        {
            GAME.MGR.GamePaused -= HandleGamePause;
            GAME.MGR.GameResumed -= HandleGameResume;
            UI.MGR.CharacterMenuOpened -= HandleCharacterMenuOpened;
            UI.MGR.CharacterMenuClosed -= HandleCharacterMenuClosed;
            animator.runtimeAnimatorController = animSet.idle;
        }

        private void Update()
        {
            if (animSet == null)
            {
                log.error(
                    $"{name} has not been assigned a StandardAnimSet. " +
                    $"Ensure the CharClassAnimationDriver is set up properly " +
                    $"on the player.");
                return;
            }

            UpdateDirections();
            SetAnim();

            if (!CanMove) { return; }
            MovePlayer();
        }

        public void EnableMovement()
        {
            CanMove = true;
        }
        public void DisableMovement()
        {
            CanMove = false;
            if (body != null)
            {
                body.velocity = Vector2.zero;
            }
        }

        public void SetAnimSet(StandardAnimSet animSet)
        {
            log.print($"{name}'s animSet is being set publicly");
            this.animSet = animSet;
        }

        private void UpdateDirections()
        {
            rawInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            // For animation
            int inputX = rawInput.x == 0f
                ? 0
                : rawInput.x < 0 ? -1 : 1;
            int inputY = rawInput.y == 0f
                ? 0
                : rawInput.y < 0 ? -1 : 1;
            input = new Vector2Int(inputX, inputY);

            // For rigidbody
            moveDirection = new Vector2(rawInput.x, rawInput.y * .5f).normalized;
        }

        private void MovePlayer()
        {
            body.velocity = moveDirection * walkSpeed; // Apply velocity to Rigidbody
        }

        private void SetAnim()
        {
            if (input == Vector2.zero)
            {
                animator.runtimeAnimatorController = animSet.idle;
            }
            else
            {
                TileDir dir = DirectionHelper.GetTileDir(input);
                animator.SetInteger("TileDir", (int)dir);
                animator.runtimeAnimatorController = animSet.walking;
            }
        }

        #region Event Responses
        // =====================================================================
        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == GAME.MGR.NeighborhoodSceneName)
            {
                DisableMovement();
                IntersectionManager.MGR.GenerationComplete += HandleGenerationComplete;
            }
        }

        private void HandleGenerationComplete()
        {
            EnableMovement();
            IntersectionManager.MGR.GenerationComplete -= HandleGenerationComplete;
        }

        private void HandleGamePause()
        {
            DisableMovement();
        }
        private void HandleGameResume()
        {
            EnableMovement();
        }

        private void HandleCharacterMenuOpened()
        {
            DisableMovement();
        }
        private void HandleCharacterMenuClosed()
        {
            EnableMovement();
        }
        #endregion // Event Responses
    }
}
