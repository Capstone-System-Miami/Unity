using UnityEngine;
using UnityEngine.Tilemaps;
using SystemMiami.Management;

namespace SystemMiami
{
    public class DungeonEntrance : MonoBehaviour
    {
        #region SERIALIZED
        // ======================================

        //=======================================
        #endregion // SERIALIZED


        #region PRIVATE VARS
        // ======================================

        private DungeonPreset _currentPreset;
        private Material _material;

        //=======================================
        #endregion // PRIVATE VARS


        #region PROPERTIES
        // ======================================

        public DungeonPreset CurrentPreset { get { return _currentPreset; } }

        //=======================================
        #endregion // PROPERTIES


        #region UNITY METHODS
        // ======================================

        //=======================================
        #endregion // UNITY METHODS


        #region PUBLIC METHODS
        // ======================================

        public void ApplyPreset(DungeonPreset preset)
        {
            if (preset == null)
            {
                Debug.LogError(
                    $"{gameObject.name}'s {name} has been passed a null DungeonEntrancePreset.\n" +
                    $"Its CurrentPreset will remain unchanged, " +
                    $"and it will not call ApplyPreset().");
                return;
            }

            // This subscription feels weird an out of place but should work for now.
            // Maybe this script should just set all of its IInteractable Event Listeners
            // instead of doing it manually in the inspector?
            // Otherwise we'll have to make a change
            // to every single DungeonEntrance in every single prefab any time
            // a change is made to what's subscribing to OnPlayerEnter or OnInteract, etc.
            InteractionTrigger entranceTrigger = GetComponentInChildren<InteractionTrigger>();
            entranceTrigger.OnInteract.AddListener(onInteract);

            _currentPreset = preset;

            applyStoredPreset();
        }

        public void TurnOffDungeonColor()
        {
            if (_material == null) { return; }
            if (CurrentPreset == null) { return; }

            _material.SetColor("_Color", CurrentPreset.DoorOffColor);

            Debug.Log($"Turned off dungeon color for {gameObject.name}.");
        }

        public void TurnOnDungeonColor()
        {
            if (_material == null) { return; }
            if (CurrentPreset == null) { return; }

            _material.SetColor("_Color", CurrentPreset.DoorOnColor);

            Debug.Log($"Turned on dungeon color for {gameObject.name}.");
        }

        //=======================================
        #endregion // PUBLIC METHODS


        #region PRIVATE METHODS
        // ======================================

        private void applyStoredPreset()
        {
            if (CurrentPreset == null)
            {
                Debug.LogError($"{gameObject.name}'s {name} script" +
                    $"is trying to apply a null CurrentPreset.");
                return;
            }

            if (CurrentPreset.EmmissiveMaterial == null)
            {
                return;
            }

            // Instantiate a copy of the emmissive material
            // so we can change it without affecting the others
            _material = new Material(CurrentPreset.EmmissiveMaterial);

            // Set the renderer's material to this temporary copy.
            GetComponent<TilemapRenderer>().material = _material;

            TurnOffDungeonColor();
        }

        private void onInteract()
        {
            GAME.MGR.GoToDungeon(_currentPreset);
        }
        
        //=======================================
        #endregion // PRIVATE METHODS


    }
}