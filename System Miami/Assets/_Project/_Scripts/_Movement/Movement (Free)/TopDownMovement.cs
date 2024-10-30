//Author: Johnny, Layla Hoey
using System.Collections;
using System.Collections.Generic;
using SystemMiami.Enums;
using SystemMiami.Utilities;
using UnityEngine;

public class TopDownMovement : MonoBehaviour
{
    public Rigidbody2D body; // Fixed the case of Rigidbody2D
    public SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    public float walkSpeed;
    public float frameRate;

    float idleTime;

    Vector2 rawInput;
    Vector2 moveDirection;
    Vector2Int roundedDirection;

    // Start is called before the first frame update
    void Start()
    {
        idleTime = Time.time; // Initialize idleTime at start
    }

    // Update is called once per frame
    void Update()
    {
        updateDirections();
        movePlayer();

        if (roundedDirection == Vector2.zero)
        {
            animator.enabled = false;
        }
        else
        {
            animator.enabled = true;
            setAnim();
        }

        //HandleSpriteFlip(); // Flips sprite based on movement moveDirection
        //SetAll(); // Sets the current sprite
    }

    private void updateDirections()
    {
        rawInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        roundedDirection = new Vector2Int(Mathf.RoundToInt(rawInput.x), Mathf.RoundToInt(rawInput.y));

        moveDirection = new Vector2(rawInput.x, rawInput.y * .5f).normalized; // Handles input
    }

    private void movePlayer()
    {
        body.velocity = moveDirection * walkSpeed; // Apply velocity to Rigidbody
    }

    private void setAnim()
    {
        TileDir dir = DirectionHelper.GetTileDir(roundedDirection);
        animator.SetInteger("TileDir", (int)dir);
    }

    #region old (sprite-flipper)
    //void SetAll()
    //{
    //    List<Sprite> directionSprites = GetSpriteDirection(); // Get the sprite list for the current moveDirection

    //    if (directionSprites != null && directionSprites.Count > 0)
    //    {
    //        float playTime = Time.time - idleTime;
    //        int totalFrames = (int)(playTime * frameRate);
    //        int frame = totalFrames % directionSprites.Count; // Fixed indexing to the number of available sprites

    //        spriteRenderer.sprite = directionSprites[frame];
    //    }
    //    else
    //    {
    //        idleTime = Time.time; // SetDefault idle time if no moveDirection is pressed
    //    }
    //}

    //void HandleSpriteFlip()
    //{
    //    if (!spriteRenderer.flipX && moveDirection.x < 0)
    //    {
    //        spriteRenderer.flipX = true; // Flip the sprite when moving left
    //    }
    //    else if (spriteRenderer.flipX && moveDirection.x > 0)
    //    {
    //        spriteRenderer.flipX = false; // Flip back when moving right
    //    }
    //}

    //List<Sprite> GetSpriteDirection() // Changed void to List<Sprite> to return the sprite list
    //{
    //    List<Sprite> selectedSprites = null;

    //    if (moveDirection.y > 0) // North
    //    {
    //        if (Mathf.Abs(moveDirection.x) > 0) // Northeast or Northwest
    //        {
    //            selectedSprites = neSprite;
    //        }
    //        else // Straight North
    //        {
    //            selectedSprites = nSprite;
    //        }
    //    }
    //    else if (moveDirection.y < 0) // South
    //    {
    //        if (Mathf.Abs(moveDirection.x) > 0) // Southeast or Southwest
    //        {
    //            selectedSprites = seSprite;
    //        }
    //        else // Straight South
    //        {
    //            selectedSprites = sSprite;
    //        }
    //    }
    //    else if (Mathf.Abs(moveDirection.x) > 0) // East or West
    //    {
    //        selectedSprites = eSprite;
    //    }

    //    return selectedSprites; // Return the selected sprite list
    //}
    #endregion
}