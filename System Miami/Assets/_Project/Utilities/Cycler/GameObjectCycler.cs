using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SystemMiami.Management;

namespace SystemMiami.Utilities
{
    public class GameObjectCycler : MonoBehaviour
    {
        [Header("Elements")]
        [SerializeField] private List<GameObject> gameObjects = new();

        [Header("Settings")]
        [SerializeField] private bool wrapStart = false;
        [SerializeField] private bool wrapEnd = false;
        [SerializeField] private bool openOnStart = false;

        private int currentIndex = 0;
        private GameObject currentGameObject;

        public bool IsRunning { get; private set; }

        private List<GameObject> GameObjects
        {
            get { return gameObjects ?? new(); }
        }

        private bool NeedsUpdate
        {
            get
            {
                if (!GameObjects.Any()) { return false; }

                return currentGameObject != GameObjects[currentIndex];
            }
        }

        private bool IsGreater => currentIndex >= GameObjects.Count;
        private bool IsLess => currentIndex < 0;

        //private void OnEnable()
        //{
        //    GAME.MGR.Pause += HandlePause;
        //    GAME.MGR.Resume += HandleResume;
        //}

        //private void OnDisable()
        //{
        //    GAME.MGR.Pause -= HandlePause;
        //    GAME.MGR.Resume -= HandleResume;
        //}

        private void Start()
        {
            if (openOnStart)
            {
                BeginCycle();
            }
        }

        private void Update()
        {
            if (!IsRunning) { return; }

            if (NeedsUpdate)
            {
                UpdateCurrentPanel();
                ActivateCurrentPanel();
            }
        }

        public void NextPanel()
        {
            currentIndex++;

            if (IsGreater)
            {
                if (wrapEnd)
                {
                    WrapIndex();
                }
                else
                {
                    EndCycle();
                }
            }
        }

        public void PrevGameObject()
        {
            currentIndex--;

            if (IsLess)
            {
                if (wrapStart)
                {
                    WrapIndex();
                }
                else
                {
                    currentIndex = 0;
                }
            }
        }

        public void BeginCycle()
        {
            IsRunning = true;
            currentIndex = 0;
        }

        public void EndCycle()
        {
            currentIndex = 0;
            currentGameObject = null;
            GameObjects.ForEach(panel => panel.SetActive(false));
            IsRunning = false;
        }

        private void UpdateCurrentPanel()
        {
            if (!GameObjects.Any()) { return; }

            currentGameObject = GameObjects[currentIndex];
        }

        private void ActivateCurrentPanel()
        {
            foreach (GameObject panel in GameObjects)
            {
                panel.SetActive(panel == currentGameObject);
            }
        }

        private void WrapIndex()
        {
            if (IsLess)
            {
                currentIndex = GameObjects.Count - 1;
            }
            else if (IsGreater)
            {
                currentIndex = 0;
            }
        }

        //private void HandlePause()
        //{
        //    Button[] buttons = GetComponentsInChildren<Button>();
        //    buttons.ToList().ForEach(button => button.interactable = false);
        //}

        //private void HandleResume()
        //{
        //    Button[] buttons = GetComponentsInChildren<Button>();
        //    buttons.ToList().ForEach(button => button.interactable = true);
        //}
    }
}
