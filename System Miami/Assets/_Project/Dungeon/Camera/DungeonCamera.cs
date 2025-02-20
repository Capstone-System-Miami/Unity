using System.Collections.Generic;
using SystemMiami.Dungeons;
using UnityEngine;
using SystemMiami.Utilities;

namespace SystemMiami
{
    /// <summary>
    /// TODO:
    /// Most of this is not in use right now. I might be wigging out but
    /// as far as I can tell we have no way of directly determining the
    /// difficulty of the dungeon that has been loaded.
    /// For now, this just sets the camera's position to the center of the
    /// GameBoard
    /// </summary>
    public class DungeonCamera : MonoBehaviour
    {
        [SerializeField] float orthoSizeEasy = 5f;
        [SerializeField] float orthoSizeMed = 6f;
        [SerializeField] float orthoSizeHard = 7f;

        [SerializeField] float yOffsetEasy = -1f;
        [SerializeField] float yOffsetMed = -1.5f;
        [SerializeField] float yOffsetHard = -2f;

        private Dictionary<DifficultyLevel, float> orthoSize = new();
        private Dictionary<DifficultyLevel, float> yOffset = new();

        private void Awake()
        {
            orthoSize = new Dictionary<DifficultyLevel, float>
            {
                { DifficultyLevel.EASY, orthoSizeEasy },
                { DifficultyLevel.MEDIUM, orthoSizeMed },
                { DifficultyLevel.HARD, orthoSizeHard },
            };

            yOffset = new Dictionary<DifficultyLevel, float>
            {
                { DifficultyLevel.EASY, yOffsetEasy },
                { DifficultyLevel.MEDIUM, yOffsetMed },
                { DifficultyLevel.HARD, yOffsetHard },
            };
        }

        private void Start()
        {
            // TODO: figure out how to position the camera at the center.

            // Set orthoSize to medium by default.
            Camera.main.orthographicSize = orthoSize[DifficultyLevel.MEDIUM];
        }
    }
}
