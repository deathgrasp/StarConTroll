using System.Collections.Generic;
using Assets.Utils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class GameWorld : UnitySingleton<GameWorld>
    {
        public Ship Ship1; // TODO: this is temporary! will be a list of all the ships of player 1
        public Ship Ship2; // TODO: this is temporary! will be a list of all the ships of player 2
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

        // Use this for initialization
        void Start()
        {
            currentSelection = Ship1;
            Ship1.RotateShipTowards(Vector3.zero, 360);
            Ship2.RotateShipTowards(Vector3.zero, 360);
            LineRenderer.startColor = Color.red;
            LineRenderer.endColor = Color.yellow;
            player1=new Player(1);
            if (ConfigurationManager.Instance.VersusAI)
            {
                player2 = new AIPlayer(2);
            }
            else
            {
                player2=new Player(2);
            }
            DisableInExecutionPhase.Add(GameObject.Find("EndTurn"));
            _panTarget = Camera.transform.position;
            Ship1.Owner = player1;
            Ship2.Owner = player2;
            SetSelection(Ship1);
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

            if (currentSelection)
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
            // TODO: if no ability selected -> this should do movement, else it should do the command-generating code (it's in the "else" block). right click should be ship & ability selection

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                var clicked = hit.collider.GetComponent<Ship>();
                if (clicked != null && currentPlayer.Number == clicked.Owner.Number)
                {
                    SetSelection(clicked);
                }
            }
            else
            {
                CommandParams commandParams = new CommandParams(currentSelection, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                Command command = new Command(currentAbility, commandParams);
                currentSelection.SetCommand(currentSelection.GetUnassignedCommand(), command);
                updateCommandQueueUI();
            }
        }


        private void SetSelection(Ship ship)
        {
            if (ship != null && ship.Owner == currentPlayer) 
            {
                currentSelection = ship;
                currentAbility = ship.GetAbilities()[0]; // TODO: this is temp! this should initially be nothing (=movement)
                updateCommandQueueUI();
                updateSidePanelUI();
            }
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
                AbilityUiIcons[i].GetComponent<Image>().sprite = abilityList[i].GetIcon();
            }
        }


        private void OnRightMouseClick()
        {
            Vector3 mousePointer = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePointer.z = 0;
            currentSelection.SetDestination(mousePointer);
            PathsManager.Instance.DrawPath(currentSelection, mousePointer);
        }


        public void OnEndTurnClick()
        {
            if (currentPlayer == player1) 
            {
                isPlayer1Done = true;
                currentPlayer = player2; // TODO: this will change we will stop using hot-seat playerd
                SetSelection(Ship2);
            }
            else if (currentPlayer == player2) 
            { 
                isPlayer2Done = true; 
                currentPlayer = player1; // TODO: this will change we will stop using hot-seat playerd
                SetSelection(Ship1);
            }

            // resolve turn if both players finished issueing orders
            if (isPlayer1Done && isPlayer2Done) { SwitchToExecutionPhase(); }
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
            SetSelection(Ship1);

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
                obj.GetComponent<SpaceObject>().LineRenderer.enabled = true;
            }
        }

    }
}
