/// Layla, Antony
using System.Collections.Generic;
using SystemMiami.Dungeons;
using SystemMiami.Management;
using SystemMiami.Utilities;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SystemMiami
{
    public class DungeonEntrance : MonoBehaviour
    {
        #region SERIALIZED
        // ======================================
        [SerializeField] private dbug log;
        [SerializeField] public List<Dungeons.Style> _excludedStyles = new();
        [SerializeField] public bool dungeonCompleted;
        #endregion // SERIALIZED


        #region PRIVATE VARS
        // ======================================
        private DungeonPreset _currentPreset;
        [SerializeField, ReadOnly] private DungeonData _dungeonData;
        private Material _material;
        #endregion // PRIVATE VARS


        #region PROPERTIES
        // ======================================
        public DungeonPreset CurrentPreset { get { return _currentPreset; } }
        public DungeonData DungeonData { get { return _dungeonData; } }
        #endregion // PROPERTIES

        private void OnEnable()
        {
            IntersectionManager.MGR.GenerationComplete += HandleGenerationComplete;

            // This subscription feels weird an out of place but should work for now.
            // Maybe this script should just set all of its IInteractable Event Listeners
            // instead of doing it manually in the inspector?
            // Otherwise we'll have to make a change
            // to every single DungeonEntrance in every single prefab any time
            // a change is made to what's subscribing to OnPlayerEnter or OnInteract, etc.
            InteractionTrigger entranceTrigger = GetComponentInChildren<InteractionTrigger>();
            entranceTrigger.OnInteract.AddListener(OnInteract);
        }

        private void OnDisable()
        {
            IntersectionManager.MGR.GenerationComplete -= HandleGenerationComplete;
        }

        private void HandleGenerationComplete()
        {
            ApplyStoredPreset();
        }

        #region PUBLIC METHODS
        // ======================================
        public void StoreNewPreset(DungeonPreset preset)
        {
            if (preset == null)
            {
                log.error(
                    $"{gameObject.name}'s {name} has been passed a null DungeonEntrancePreset.\n" +
                    $"Its CurrentPreset will remain unchanged, " +
                    $"and it will not call ApplyPreset().");
                return;
            }

            // NOTE: These warnings are useful, don't delete.
            // Uncomment as necessary.
            //
            // else if (_currentPreset == null)
            // {
            //     log.warn($"<color=yellow>{name} is being assigned a preset for " +
            //         $"the first time.</color>",
            //         this);
            // }
            // else
            // {
            //     log.warn($"<color=red>{name} is being assigned a preset for " +
            //         $"AT LEAST THE SECOND TIME.</color>",
            //         this);
            // }

            _currentPreset = preset;

            // NOTE:
            // This ended up being significant in recent debugs re: dungeons
            // loading in and losing (or never gaining) state.
            // ApplyStoredPreset();
        }

        public void TurnOffDungeonColor()
        {
            if (_material == null) { return; }
            if (CurrentPreset == null) { return; }

            _material.SetColor("_Color", CurrentPreset.DoorOffColor);

            // log.print($"Turned off dungeon color for {gameObject.name}.");
        }

        public void TurnOnDungeonColor()
        {
            if (_material == null) { return; }
            if (CurrentPreset == null) { return; }

            _material.SetColor("_Color", CurrentPreset.DoorOnColor);

            // log.print($"Turned on dungeon color for {gameObject.name}.");
        }
        #endregion // PUBLIC METHODS


        #region PRIVATE METHODS
        // ======================================
        private void ApplyStoredPreset()
        {
            if (CurrentPreset == null)
            {
                log.error(
                    $"{gameObject.name}'s {name} script" +
                    $"is trying to apply a null CurrentPreset.");
                return;
            }

            // Create a little Data packet to send to the GAME.MGR
            _dungeonData = CurrentPreset.GetData(_excludedStyles);

            // NOTE: Prints full report of
            // - Dungeon prefab that will be spawned
            // - All enemies that will spawn,
            // - All rewards to be earned.
            //
            // log.warn($"DungeonData being stored:\n {_dungeonData}");

            if (!TryApplyMaterial(out string materialError))
            {
                log.warn(materialError);
            }

            TurnOffDungeonColor();
        }

        private bool TryApplyMaterial(out string msg)
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

        private void OnInteract()
        {
            if (GAME.MGR.RegenerateDungeonDataOnInteract)
            {
                log.warn($"DungeonData being re-generated:\n {_dungeonData}");
                _dungeonData = CurrentPreset.GetData(_excludedStyles);
            }
            if (DungeonRewardsPanel.MGR != null)
            {
                DungeonRewardsPanel.MGR.ShowPanel(_dungeonData);
            }
        }
        #endregion // PRIVATE METHODS
    }
}
