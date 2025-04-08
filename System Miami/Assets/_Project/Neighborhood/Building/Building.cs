using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;

namespace SystemMiami
{
    /// <summary>
    /// TODO: write a Building sub tilemap class or smth
    /// </summary>
    public class Building : MonoBehaviour, ITransparency
    {
        [Header("Internal Refs")]
        [SerializeField] private List<Tilemap> allTilemaps = new();
        [SerializeField] private List<Light2D> allLights = new();

        private Dictionary<Tilemap, Color> opaqueColors = new();


        [Range(0, 1)] private float fallbackOpacity = .2f;

        [Header("Testing")]
        [SerializeField] private bool TRIGGER_transparent;
        [SerializeField] private bool TRIGGER_opaque;

        public bool IsTransparent { get; private set; }

        private void Awake()
        {
            foreach (Tilemap map in allTilemaps)
            {
                TilemapCollider2D mapCollider = map.gameObject.AddComponent<TilemapCollider2D>();
                mapCollider.isTrigger = true;
            }
        }

        private void Update()
        {
            if (TRIGGER_transparent)
            {
                TRIGGER_transparent = false;
                TRIGGER_opaque = false;
                SetTransparent(fallbackOpacity);
            }

            if (TRIGGER_opaque)
            {
                TRIGGER_transparent = false;
                TRIGGER_opaque = false;
                SetOpaque();
            }
        }

        public void SetTransparent(float opacityPercent)
        {
            float opacity = opacityPercent < 0 || opacityPercent > 1
                ? fallbackOpacity
                : opacityPercent;

            SetTilemapsTransparent(opacity);
            SetLightsOff();
            IsTransparent = true;
        }

        public void SetOpaque()
        {
            SetTilemapsOpaque();
            SetLightsOn();
            IsTransparent = false;
        }

        private void SetTilemapsTransparent(float opacity)
        {
            opaqueColors.Clear();
            foreach (Tilemap map in allTilemaps)
            {
                opaqueColors[map] = map.color;

                map.color = new Color(
                    map.color.r,
                    map.color.g,
                    map.color.b,
                    opacity);
            }
        }

        private void SetTilemapsOpaque()
        {
            foreach (Tilemap map in allTilemaps)
            {
                if (opaqueColors.ContainsKey(map))
                {
                    map.color = opaqueColors[map];
                }
            }
        }

        private void SetLightsOff()
        {
            foreach (Light2D light in allLights)
            {
                light.enabled = false;
            }
        }

        private void SetLightsOn()
        {
            foreach (Light2D light in allLights)
            {
                light.enabled = true;
            }
        }
    }
}
