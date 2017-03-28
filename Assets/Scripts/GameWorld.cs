using System.Collections.Generic;
using Assets.Utils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class GameWorld : UnitySingleton<GameWorld>
    {
        public List<Ship> P1Ships;
        public List<Ship> P2Ships;
        public Button[] CommandUiIcons = new Button[4]; // TODO: this is temporary! these icons will be created on the fly, and won't be always 4
        public Button[] AbilityUiIcons = new Button[4]; // TODO: this is temporary! these icons will be created on the fly, and won't be always 4
        public LineRenderer LineRenderer;
        public GameObject Marker;
        public bool ExecutionPhase;
        private Ship currentSelection;
        public Player currentPlayer;
        private Player player1;
        private Player player2;
        private Ability currentAbility;
        private int currentSegment = -1; // TODO: -1 means no segment is selected.
        private bool isPlayer1Done = false;
        private bool isPlayer2Done = false;
        public Camera Camera;
        public List<GameObject> DisableInExecutionPhase = new List<GameObject>();
        private Vector3 _panStart;
        private Vector3 _panTarget;
        private Missile _missilePrefab;
        private Missile MissilePrefab
        {
            get { return _missilePrefab ?? (_missilePrefab = Resources.Load<Missile>("Missile")); }
        }

        void Awake()
        {
            player1 = new Player(1);
            if (ConfigurationManager.Instance.VersusAI)
            {
                player2 = new AIPlayer(2);
            }
            else
            {
                player2 = new Player(2);
            }
            P1Ships = ShipSpawner.Instance.CreateShips(player1);
            SetSelection(P1Ships[0]);
            P2Ships = ShipSpawner.Instance.CreateShips(player2);

        }
        // Use this for initialization
        void Start()
        {
            LineRenderer.startColor = Color.red;
            LineRenderer.endColor = Color.yellow;

            for (int i=0; i < CommandUiIcons.Length; i++)
            {
                CommandUiIcons[i].GetComponent<SegmentButton>().SetIndex(i);
            }
            
            DisableInExecutionPhase.Add(GameObject.Find("EndTurn"));
            _panTarget = Camera.transform.position;
            SwitchToPlanningPhase();
        }

        // Update is called once per frame
        void Update()
        {
            Camera.transform.position = Vector3.Lerp(Camera.transform.position, _panTarget, 0.5f);
            if (ExecutionPhase) { return; } // don't respond to inputs if resolving turn
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                MouseScroll(Input.GetAxis("Mouse ScrollWheel"));
            }
            if (EventSystem.current.IsPointerOverGameObject())
            {
                TurnOffMouseLineRenderer();
                return;
            }
            else
            {
                TurnOnMouseLineRenderer();
            }

            if (currentSelection != null)
            {
                Marker.SetActive(true);
                Marker.transform.position = currentSelection.transform.position;
                Marker.transform.position += Vector3.back;
                var mousePointer = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (LineRenderer.enabled)
                {
                    PathsManager.Instance.DrawPath(currentSelection, mousePointer, LineRenderer);
                }
            }
            else
            {
                Marker.SetActive(false);
            }
            if (Input.GetMouseButtonDown(0))
            {
                OnLeftMouseClick();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                OnRightMouseClick();
            }
            else if (Input.GetMouseButton(2))
            {
                OnMiddleMouseClickDown();
            }
            else if (Input.GetMouseButtonUp(2))
            {
                OnMiddleMouseClickUp();
            }
            handleKeys();
        }


        private void handleKeys()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetSelectedAbility(null);
                SetSelection(null);
                SetSelectedSegment(-1); // TODO: turn '-1' from magic number to a named const
            }
        }


        private void OnMiddleMouseClickDown()
        {
            var change = Vector3.zero;
            if (_panStart != Vector3.zero)
            {
                change = _panStart - Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.back;

            }
            _panStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _panTarget += change;
        }


        private void OnMiddleMouseClickUp()
        {
            _panStart = Vector3.zero;
        }


        private void MouseScroll(float amount)
        {
            Camera.orthographicSize -= amount * ConfigurationManager.ScrollZoomSpeed;
        }


        private void OnLeftMouseClick()
        {
            // left mouse-click is used for selection of units and abilities
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                var clicked = hit.collider.GetComponent<Ship>();
                if (clicked != null && currentPlayer.Number == clicked.Owner.Number)
                {
                    SetSelection(clicked);
                }
            }
        }


        private void OnRightMouseClick()
        {
            // right mouse-click is used for issueing orders to current selection
            Vector3 mousePointer = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePointer.z = 0;

            // if no ability is selected - do movement
            if (currentAbility == null)
            {
                currentSelection.SetDestination(mousePointer);
                currentSelection.DrawPathUI();
            }
            // if ability is selected then issue command
            else
            {
                CommandParams commandParams = new CommandParams(currentSelection, mousePointer);
                Command command = new Command(currentAbility, commandParams);
                if (currentSegment == -1)
                {
                    currentSelection.SetCommand(currentSelection.GetUnassignedCommand(), command);
                }
                else
                {
                    currentSelection.SetCommand(currentSegment, command);
                    currentSegment = -1; // TODO: no magic numbers!
                }
                updateCommandQueueUI();
                SetSelectedAbility(null);
            }
        }


        private void SetSelection(Ship ship)
        {
            if (ship != null && ship.Owner == currentPlayer) 
            {
                if(currentSelection != null) { currentSelection.HidePathUIIcons(); } // hide path icons UI for previous selection
                currentSelection = ship;
                updateCommandQueueUI();
                updateSidePanelUI();
                currentSelection.DrawPathUI();
            }
        }


        public void SetSelectedAbility(Ability ability)
        {
            currentAbility = ability;
        }


        public void SetSelectedSegment(int index)
        {
            currentSegment = index;
        }


        private void updateCommandQueueUI()
        {
            Command[] commandQueue = currentSelection.GetCommandQueue();
            for (int i=0; i < commandQueue.Length; i++)
            {
                if (commandQueue[i] == null)
                {
                    CommandUiIcons[i].GetComponent<Image>().sprite = null;
                    CommandUiIcons[i].GetComponent<Image>().color = Color.white;
                }
                else
                {
                    CommandUiIcons[i].GetComponent<Image>().sprite = commandQueue[i].GetAbility().GetIcon();
                    CommandUiIcons[i].GetComponent<Image>().color = Color.magenta;
                }
            }
        }


        private void updateSidePanelUI()
        {
            List<Ability> abilityList = currentSelection.GetAbilities();
            for (int i=0; i < abilityList.Count; i++)
            {
                AbilityUiIcons[i].GetComponent<AbilityButton>().SetAbility(abilityList[i]);
            }
        }


        public void OnEndTurnClick()
        {
            if (currentPlayer == player1) 
            {
                isPlayer1Done = true;
                currentPlayer = player2; // TODO: this will change we will stop using hot-seat playerd
                SetSelection(P2Ships[0]);
            }
            else if (currentPlayer == player2) 
            { 
                isPlayer2Done = true; 
                currentPlayer = player1; // TODO: this will change we will stop using hot-seat playerd
                SetSelection(P1Ships[0]);
            }
            // resolve turn if both players finished issueing orders
            if (!ExecutionPhase && isPlayer1Done && isPlayer2Done) { SwitchToExecutionPhase(); }
        }


        public void OnExecutionPhaseEnd()
        {
            ExecutionPhase = false;
            SwitchToPlanningPhase();
        }


        private void SwitchToPlanningPhase()
        {
            TurnOnAllGuides();
            Time.timeScale = 0;
            isPlayer1Done = false;
            isPlayer2Done = false;
            currentPlayer = player1;
            SetSelection(P1Ships[0]);

            // TODO: change this to method "showUI"
            foreach (var o in DisableInExecutionPhase)
            {
                o.SetActive(true);
            }
        }


        private void SwitchToExecutionPhase()
        {
            Marker.SetActive(false);
            TurnOffMouseLineRenderer();
            TurnOffAllGuides(); // TODO: what is this? is it the same as the loop below?

            // TODO: change this to method "hideUI"
            foreach (var o in DisableInExecutionPhase)
            {
                o.SetActive(false);
            }
            // TODO: remove all the ships' and projectiles trajectories UI
            ExecutionPhase = true;
            SetSelection(null);
            Time.timeScale = 1;
            TurnManager.Instance.CurrentUpdate = 0; // TODO: should it be -1?
        }


        private void TurnOffMouseLineRenderer()
        {
            LineRenderer.enabled = false;
            LineRenderer.SetVertexCount(0);
        }


        private void TurnOnMouseLineRenderer()
        {
            LineRenderer.enabled = true;
        }


        private void TurnOffAllGuides()
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("SpaceObject"))
            {
                obj.GetComponent<SpaceObject>().LineRenderer.enabled = false;
            }
        }


        private void TurnOnAllGuides()
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("SpaceObject"))
            {
                SpaceObject so = obj.GetComponent<SpaceObject>();
                so.LineRenderer.enabled = true;
                PathsManager.Instance.DrawPath(so,so.Destination,so.LineRenderer);
            }
        }

    }
}
