using UnityEngine;

namespace SystemMiami
{
    public class CharacterInfo : MonoBehaviour
    {
        // Tile that the character is occupying
        public OverlayTile activeTile;

        // Movement points available for this turn
        public int movementPoints;

        // Maximum movement points (determined by Dexterity)
        public int maxMovementPoints;

        // Indicates if the character has performed an action this turn
        public bool hasActed;

        private void Start()
        {
            InitializeCharacter();
        }

        /// <summary>
        /// Initializes the character's stats.
        /// </summary>
        public void InitializeCharacter()
        {
            // Set maxMovementPoints to Dexterity attribute
            Attributes attributes = GetComponent<Attributes>();
            if (attributes != null)
            {
                maxMovementPoints = attributes.GetAttribute(AttributeType.DEXTERITY) * 10;
            }
            else
            {
                // If Attributes component is missing, set a default value
                maxMovementPoints = 5; // Default movement points
                Debug.LogWarning("Attributes component missing on character. Using default movement points.");
            }
            movementPoints = maxMovementPoints;
        }

        /// <summary>
        /// Resets movement points and action flag for a new turn.
        /// </summary>
        public void ResetTurn()
        {
            movementPoints = maxMovementPoints;
            hasActed = false;
        }

        
    }
}