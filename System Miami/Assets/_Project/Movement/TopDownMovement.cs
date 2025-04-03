//Author: Johnny, Layla Hoey
using SystemMiami.Animation;
using SystemMiami.Enums;
using SystemMiami.Utilities;
using UnityEngine;
using SystemMiami.Drivers;

namespace SystemMiami
{
    public class TopDownMovement : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private dbug log;

        public Rigidbody2D body;
        public SpriteRenderer spriteRenderer;
        public float walkSpeed;
        public float frameRate;

        [SerializeField] private Animator animator;

        /// <summary>
        /// Must be assigned at runtime by <see cref="CharClassAnimationDriver"/>
        /// </summary>
        [SerializeField, ReadOnly] private StandardAnimSet animSet;


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
            MovePlayer();

            if (input == Vector2.zero)
            {
                animator.runtimeAnimatorController = animSet.idle;
            }
            else
            {
                animator.runtimeAnimatorController = animSet.walking;
                SetAnim();
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

            int inputX = rawInput.x == 0f
            ? 0
            : rawInput.x < 0 ? -1 : 1;
            int inputY = rawInput.y == 0f
            ? 0
            : rawInput.y < 0 ? -1 : 1;
            input = new Vector2Int(inputX, inputY);

            moveDirection = new Vector2(rawInput.x, rawInput.y * .5f).normalized; // Handles input
        }

        private void MovePlayer()
        {
            body.velocity = moveDirection * walkSpeed; // Apply velocity to Rigidbody
        }

        private void SetAnim()
        {
            TileDir dir = DirectionHelper.GetTileDir(input);
            animator.SetInteger("TileDir", (int)dir);
        }

        private void OnDisable()
        {
            animator.runtimeAnimatorController = animSet.idle;
        }
    }
}