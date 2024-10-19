//Author: Johnny
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownMovement : MonoBehaviour
{
    public Rigidbody2D body; // Fixed the case of Rigidbody2D
    public SpriteRenderer spriteRenderer;

    public List<Sprite> nSprite;
    public List<Sprite> neSprite;
    public List<Sprite> eSprite;
    public List<Sprite> seSprite;
    public List<Sprite> sSprite;

    public float walkSpeed;
    public float frameRate;

    float idleTime;
    Vector2 direction;

    // Start is called before the first frame update
    void Start()
    {
        idleTime = Time.time; // Initialize idleTime at start
    }

    // Update is called once per frame
    void Update()
    {
        direction = new Vector2( Input.GetAxis("Horizontal"), Input.GetAxis("Vertical") * .5f).normalized; // Handles input
        body.velocity = direction * walkSpeed; // Apply velocity to Rigidbody

        HandleSpriteFlip(); // Flips sprite based on movement direction
        SetSprite(); // Sets the current sprite
    }

    void SetSprite()
    {
        List<Sprite> directionSprites = GetSpriteDirection(); // Get the sprite list for the current direction

        if (directionSprites != null && directionSprites.Count > 0)
        {
            float playTime = Time.time - idleTime;
            int totalFrames = (int)(playTime * frameRate);
            int frame = totalFrames % directionSprites.Count; // Fixed indexing to the number of available sprites

            spriteRenderer.sprite = directionSprites[frame];
        }
        else
        {
            idleTime = Time.time; // Reset idle time if no direction is pressed
        }
    }

    void HandleSpriteFlip()
    {
        if (!spriteRenderer.flipX && direction.x < 0)
        {
            spriteRenderer.flipX = true; // Flip the sprite when moving left
        }
        else if (spriteRenderer.flipX && direction.x > 0)
        {
            spriteRenderer.flipX = false; // Flip back when moving right
        }
    }

    List<Sprite> GetSpriteDirection() // Changed void to List<Sprite> to return the sprite list
    {
        List<Sprite> selectedSprites = null;

        if (direction.y > 0) // North
        {
            if (Mathf.Abs(direction.x) > 0) // Northeast or Northwest
            {
                selectedSprites = neSprite;
            }
            else // Straight North
            {
                selectedSprites = nSprite;
            }
        }
        else if (direction.y < 0) // South
        {
            if (Mathf.Abs(direction.x) > 0) // Southeast or Southwest
            {
                selectedSprites = seSprite;
            }
            else // Straight South
            {
                selectedSprites = sSprite;
            }
        }
        else if (Mathf.Abs(direction.x) > 0) // East or West
        {
            selectedSprites = eSprite;
        }

        return selectedSprites; // Return the selected sprite list
    }
}