/// Layla, Antony
using System.Collections.Generic;
using SystemMiami.Dungeons;
using SystemMiami.Management;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SystemMiami
{
    public class DungeonEntrance : MonoBehaviour
    {
        #region SERIALIZED
        // ======================================

        [SerializeField] private List<Dungeons.Style> _excludedStyles = new();

        //=======================================
        #endregion // SERIALIZED


        #region PRIVATE VARS
        // ======================================

        private DungeonPreset _currentPreset;
        private DungeonData _dungeonData;
        private Material _material;

        //=======================================
        #endregion // PRIVATE VARS


        #region PROPERTIES
        // ======================================

        public DungeonPreset CurrentPreset { get { return _currentPreset; } }

        //=======================================
        #endregion // PROPERTIES


        #region PUBLIC METHODS
        // ======================================

        public void ApplyNewPreset(DungeonPreset preset)
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
                Debug.LogError(
                    $"{gameObject.name}'s {name} script" +
                    $"is trying to apply a null CurrentPreset."
                    );
                return;
            }

            // Create a little data packet to send to the GAME.MGR
            _dungeonData = CurrentPreset.GetData(_excludedStyles);

            Debug.LogWarning($"DungeonData being stored:\n {_dungeonData}");

            if (!tryApplyMaterial(out string materialError))
            {
                Debug.LogWarning(materialError);
            }

            TurnOffDungeonColor();
        }

        private bool tryApplyMaterial(out string msg)
        {
            msg = "";

            if (CurrentPreset.EmmissiveMaterial == null)
            {
                msg = $"{gameObject.name}'s {name} script " +
                    $"is applying {CurrentPreset}, but it " +
                    $"is missing an EmmisiveMaterial reference.";

                return false;
            }

            // Find the tilemap renderer of the DungeonEntrance
            if (!TryGetComponent(out TilemapRenderer renderer))
            {
                msg = $"{gameObject.name}'s {name} script " +
                    $"is trying to apply {CurrentPreset.EmmissiveMaterial} " +
                    $"but {gameObject.name} is missing a TilemapRenderer.";

                return false;
            }

            // Instantiate a copy of the preset's emmissive material
            // so we can change it without affecting the others
            _material = new Material(CurrentPreset.EmmissiveMaterial);
            
            // Set the renderer's material to this temporary copy.
            renderer.material = _material;
            return true;
        }

        private void onInteract()
        {
            if (GAME.MGR.RegenerateDungeonDataOnInteract)
            {
                Debug.LogWarning($"DungeonData being re-generated:\n {_dungeonData}");
                _dungeonData = CurrentPreset.GetData(_excludedStyles);
            }

            GAME.MGR.GoToDungeon(_dungeonData);
        }
        
        //=======================================
        #endregion // PRIVATE METHODS
    }
}