using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SystemMiami
{
    public class Collectable : MonoBehaviour, IInteractable
    {
        Tilemap _tilemap;

        [SerializeField] Color _interactableColor;
        Color _nonInteractableColor;

        private void Start()
        {
            _tilemap = GetComponent<Tilemap>();
            _nonInteractableColor = _tilemap.color;
        }

        public void Interact()
        {
            print("trying to destroy");

            Destroy(gameObject);
        }

        public void PlayerEnter()
        {
            _tilemap.color = _interactableColor;
        }

        public void PlayerExit()
        {
            _tilemap.color = _nonInteractableColor;
        }
    }
}
