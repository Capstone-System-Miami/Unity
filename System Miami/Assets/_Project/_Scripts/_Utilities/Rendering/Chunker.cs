using System.Collections;
using System.Collections.Generic;
using SystemMiami.Management;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace SystemMiami
{
    public class Chunker : Singleton<Chunker>
    {
        [SerializeField] KeyCode collectAllRends;
        [SerializeField] KeyCode chunk;
        [SerializeField] KeyCode indiv;

        [SerializeField] TilemapRenderer[] rends;
        [SerializeField] bool isChunk;

        private void Start()
        {
            Unchunk();
        }

        private TilemapRenderer[] collectRends()
        {
            return FindObjectsOfType(typeof(TilemapRenderer)) as TilemapRenderer[];
        }

        private void setAll(TilemapRenderer.Mode mode)
        {
            if (rends != null && rends.Length > 0)
            {
                foreach (TilemapRenderer rend in rends)
                {
                    rend.mode = mode;
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(collectAllRends))
            {
                rends = collectRends();
            }

            if (Input.GetKeyDown(chunk) && !isChunk)
            {
                Chunk();
            }

            if (Input.GetKeyDown(indiv) && isChunk)
            {
                Unchunk();
            }
        }

        public void Chunk()
        {
            setAll(TilemapRenderer.Mode.Chunk);
            isChunk = true;
        }

        public void Unchunk()
        {
            setAll(TilemapRenderer.Mode.Individual);
            isChunk = false;
        }
    }
}
