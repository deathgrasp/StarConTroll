using System.Collections.Generic;
using Assets.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class GameWorld : UnitySingleton<GameWorld>
    {
        public Ship Ship1; // TODO: this is temporary! will be a list of all the ships of player 1
        public Ship Ship2; // TODO: this is temporary! will be a list of all the ships of player 2
        public LineRenderer LineRenderer;
        public GameObject Marker;
        public bool ExecutionPhase;
        private Ship currentSelection;
        public Player currentPlayer;
        public Player[] Players;
        private bool isPlayer1Done = false;
        private bool isPlayer2Done = false;
        public Camera Camera;
        public List<GameObject> DisableInExecutionPhase = new List<GameObject>();
        private Vector3 _panStart;
        private Vector3 _panTarget;
        // Use this for initialization
        void Awake()
        {
            currentSelection = Ship1;
            Ship1.RotateShipTowards(Vector3.zero, 360);
            Ship2.RotateShipTowards(Vector3.zero, 360);
            LineRenderer.startColor = Color.red;
            LineRenderer.endColor = Color.yellow;
            Players = new[] { new Player(1), new Player(2) };
            if (ConfigurationManager.Instance.VersusAI)
            {
                Players[1] = new AIPlayer(2);
            }
            DisableInExecutionPhase.Add(GameObject.Find("EndTurn"));
            _panTarget = Camera.transform.position;
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
            Camera.orthographicSize -= amount * ConfigurationManager.Instance.ScrollZoomSpeed;
        }
        private void OnLeftMouseClick()
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                var clicked = hit.collider.GetComponent<Ship>();
                if (clicked != null && currentPlayer.Number == clicked.Owner)
                {
                    currentSelection = clicked;
                }
            }
            else
            {
                var mousePointer = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var missile = Instantiate(Missile.MissilePrefab, currentSelection.transform.position, currentSelection.transform.rotation) as Missile;

                missile.Destination = mousePointer;
            }
        }


        private void SetSelection(Ship ship)
        {
            if (ship.Owner == currentPlayer.Number)
            {
                currentSelection = ship;
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
            if (currentPlayer == Players[0])
            {
                isPlayer1Done = true;
                currentPlayer = Players[1]; // TODO: this will change we will stop using hot-seat playerd
                currentSelection = Ship2;
                if (currentPlayer.IsHuman == false)
                {
                    var target = ((AIPlayer)currentPlayer).IssueOrders(Ship2);
                    var myMissile = Instantiate(Missile.MissilePrefab, Ship2.transform.position, Ship2.transform.rotation) as Missile;
                    myMissile.Destination = target;
                    isPlayer2Done = true;
                    currentPlayer = Players[0]; // TODO: this will change we will stop using hot-seat playerd
                    currentSelection = Ship1;
                }
            }
            else if (currentPlayer == Players[1])
            {
                isPlayer2Done = true;
                currentPlayer = Players[0]; // TODO: this will change we will stop using hot-seat playerd
                currentSelection = Ship1;
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
            currentPlayer = Players[0];
            currentSelection = Ship1;
            foreach (var o in DisableInExecutionPhase)
            {
                o.SetActive(true);
            }
        }


        private void SwitchToExecutionPhase()
        {
            Marker.SetActive(false);
            TurnOffMouseLineRenderer();
            TurnOffAllGuides();
            foreach (var o in DisableInExecutionPhase)
            {
                o.SetActive(false);
            }
            // TODO: remove all the ships' and projectiles trajectories UI
            ExecutionPhase = true;
            currentSelection = null;
            Time.timeScale = 1;
            TurnManager.Instance.ToNextTurn = (int)(ConfigurationManager.Instance.TurnDuration / ConfigurationManager.Instance.FixedUpdateStep);
            //print("look at me    " + TurnManager.Instance.ToNextTurn);
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
