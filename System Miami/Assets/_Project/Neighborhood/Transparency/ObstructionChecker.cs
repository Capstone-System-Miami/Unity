using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace SystemMiami
{
    public class ObstructionChecker : MonoBehaviour
    {
        [Header("Raycast Target")]
        [SerializeField] private Transform target;

        [Header("Settings")]
        [SerializeField] private float opacity;

        public List<ITransparency> PreviousObstructions { get; private set; } = new();
        public List<ITransparency> CurrentObstructions { get; private set; } = new();

        private float offsetY => target.transform.position.y - transform.position.y;
        private float offsetX => target.transform.position.x - transform.position.x;

        private void Update()
        {
            CurrentObstructions = GetObstructions();

            if (CurrentObstructions.Count != PreviousObstructions.Count)
            {
                List<ITransparency> newObstructions = CurrentObstructions
                    .Where(obstruction => !PreviousObstructions.Contains(obstruction)).ToList();

                List <ITransparency> staleObstructions = PreviousObstructions
                    .Where(previous => !CurrentObstructions.Contains(previous)).ToList();

                foreach (ITransparency obstruction in newObstructions)
                {
                    obstruction.SetTransparent(opacity);
                }

                foreach (ITransparency obstruction in staleObstructions)
                {
                    obstruction.SetOpaque();
                }
            }

            PreviousObstructions = CurrentObstructions;
        }

        private List<ITransparency> GetObstructions()
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(
                (Vector2)transform.position + new Vector2(offsetX, offsetY),
                (Vector2)target.transform.position);

            List<ITransparency> obstructions = new();

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    break;
                }

                if (hit.collider.TryGetComponent(out ITransparency obstruction))
                {
                    obstructions.Add(obstruction);
                }
            }

            return obstructions;
        }
    }
}
