using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using SystemMiami.Management;
using UnityEditor.Animations;

// Layla

namespace SystemMiami.Outdated
{
    public class OLDGameObjectCycler : MonoBehaviour
    {
    //    [SerializeField] private List<GameObject> gameObjects = new();
    //    [SerializeField] private bool wrapStart = false;
    //    [SerializeField] private bool wrapEnd = false;
    //    [SerializeField] private bool openOnStart = false;

    //    private Cycler<GOcyclerElement> cycler;
    //    private GameObject currentGameObject;

    //    public bool IsRunning { get; private set; }

    //    //private List<GameObject> GameObjects
    //    //{
    //    //    get { return gameObjects ?? new(); }
    //    //}

    //    //private bool NeedsUpdate
    //    //{
    //    //    get
    //    //    {
    //    //        if (!GameObjects.Any()) { return false; }

    //    //        return currentGameObject != GameObjects[currentIndex];
    //    //    }
    //    //}

    //    private void Awake()
    //    {
    //        List<GOcyclerElement> elements = new();
    //        gameObjects.ForEach(go => elements.Add( new GOcyclerElement(go) ));
    //        cycler = new(elements, wrapStart, wrapEnd);
    //    }

    //    //private void OnEnable()
    //    //{
    //    //    GAME.MGR.Pause += HandlePause;
    //    //    GAME.MGR.Resume += HandleResume;
    //    //}

    //    //private void OnDisable()
    //    //{
    //    //    GAME.MGR.Pause -= HandlePause;
    //    //    GAME.MGR.Resume -= HandleResume;
    //    //}

    //    private void Start()
    //    {
    //        if (openOnStart)
    //        {
    //            BeginCycle();
    //        }
    //    }

    //    private void Update()
    //    {
    //        if (!IsRunning) { return; }

    //        if (NeedsUpdate)
    //        {
    //            UpdateCurrentGameObject();
    //            ActivateCurrentGameObject();
    //        }
    //    }

    //    //public void NextGameObject()
    //    //{
    //    //    currentIndex++;

    //    //    if (IndexIsGreater)
    //    //    {
    //    //        if (wrapEnd)
    //    //        {
    //    //            WrapIndex();
    //    //        }
    //    //        else
    //    //        {
    //    //            EndCycle();
    //    //        }
    //    //    }
    //    //}

    //    //public void PrevGameObject()
    //    //{
    //    //    currentIndex--;

    //    //    if (IndexIsLess)
    //    //    {
    //    //        if (wrapStart)
    //    //        {
    //    //            WrapIndex();
    //    //        }
    //    //        else
    //    //        {
    //    //            currentIndex = 0;
    //    //        }
    //    //    }
    //    //}

    //    //public void BeginCycle()
    //    //{
    //    //    IsRunning = true;
    //    //    currentIndex = 0;
    //    //    //GAME.MGR.OnPanelCyclerInteraction(this);
    //    //}

    //    //public void EndCycle()
    //    //{
    //    //    currentIndex = 0;
    //    //    currentGameObject = null;
    //    //    GameObjects.ForEach(panel => panel.SetActive(false));
    //    //    IsRunning = false;
    //    //}

    //    //private void UpdateCurrentGameObject()
    //    //{
    //    //    if (!GameObjects.Any()) { return; }

    //    //    currentGameObject = GameObjects[currentIndex];
    //    //}

    //    //private void ActivateCurrentGameObject()
    //    //{
    //    //    foreach (GameObject panel in GameObjects)
    //    //    {
    //    //        panel.SetActive(panel == currentGameObject);
    //    //    }
    //    //}

    //    //private void WrapIndex()
    //    //{
    //    //    if (IndexIsLess)
    //    //    {
    //    //        currentIndex = GameObjects.Count - 1;
    //    //    }
    //    //    else if (IndexIsGreater)
    //    //    {
    //    //        currentIndex = 0;
    //    //    }
    //    //}

    //    //private void HandlePause()
    //    //{
    //    //    Button[] buttons = GetComponentsInChildren<Button>();
    //    //    buttons.ToList().ForEach(button => button.interactable = false);
    //    //}

    //    //private void HandleResume()
    //    //{
    //    //    Button[] buttons = GetComponentsInChildren<Button>();
    //    //    buttons.ToList().ForEach(button => button.interactable = true);
    //    //}

    //    private class GOcyclerElement : ICycleable
    //    {
    //        public readonly GameObject go;

    //        public GOcyclerElement(GameObject go)
    //        {
    //            this.go = go;
    //        }

    //        public void CycleAway()
    //        {
    //            go.SetActive(false);
    //        }

    //        public void CycleTo()
    //        {
    //            go.SetActive(true);
    //        }
    //    }
    }
}