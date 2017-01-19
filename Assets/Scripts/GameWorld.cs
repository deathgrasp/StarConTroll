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
        private Ship currentSelection;
        public int currentPlayer = Player.PLAYER_1;
        private bool isPlayer1Done = false;
        private bool isPlayer2Done = false;


        private Missile _missilePrefab;
        private Missile MissilePrefab
        {
            get { return _missilePrefab ?? (_missilePrefab = Resources.Load<Missile>("Missile")); }
        }

        // Use this for initialization
        void Start()
        {
            currentSelection = Ship1;
            Ship1.RotateShipTowards(Vector3.zero,360);
            Ship2.RotateShipTowards(Vector3.zero,360);

            LineRenderer.startColor=Color.red;
            LineRenderer.endColor=Color.yellow;
        }

        // Update is called once per frame
        void Update()
        {
            if (currentPlayer == Player.NO_ONE) { return; } // don't respond to inputs if resolving turn

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
                Marker.transform.position = currentSelection.transform.position ;
                Marker.transform.position+=Vector3.back;
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
        }


        private void OnLeftMouseClick()
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                var clicked = hit.collider.GetComponent<Ship>();
                if (clicked != null)
                {
                    currentSelection = clicked;
                }
            }
            else
            {
                var mousePointer = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var missile = Instantiate(MissilePrefab, currentSelection.transform.position, currentSelection.transform.rotation) as Missile;

                missile.Destination = mousePointer;
            }
        }


        private void SetSelection(Ship ship)
        {
            if (ship.Owner == currentPlayer) 
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
            if (currentPlayer == Player.PLAYER_1) 
            {
                isPlayer1Done = true;
                currentPlayer = Player.PLAYER_2; // TODO: this will change we will stop using hot-seat playerd
                currentSelection = Ship2;
            }
            else if (currentPlayer == Player.PLAYER_2) 
            { 
                isPlayer2Done = true; 
                currentPlayer = Player.PLAYER_1; // TODO: this will change we will stop using hot-seat playerd
                currentSelection = Ship1;
            }

            // resolve turn if both players finished issueing orders
            if (isPlayer1Done && isPlayer2Done) { SwitchToExecutionPhase(); }
        }


        public void OnExecutionPhaseEnd()
        {
            SwitchToPlanningPhase();
        }


        private void SwitchToPlanningPhase()
        {
            TurnOnAllGuides();
            Time.timeScale = 0;
            isPlayer1Done = false;
            isPlayer2Done = false;
            currentPlayer = Player.PLAYER_1;
            currentSelection = Ship1;
        }


        private void SwitchToExecutionPhase()
        {
            Marker.SetActive(false);
            TurnOffMouseLineRenderer();
            TurnOffAllGuides();
            // TODO: remove all the ships' and projectiles trajectories UI
            currentPlayer = Player.NO_ONE;
            currentSelection = null;
            Time.timeScale = 1;
            TurnManager.Instance.ToNextTurn = (int)(ConfigurationManager.Instance.TurnDuration/ConfigurationManager.Instance.FixedUpdateStep);  
            print("look at me    "+TurnManager.Instance.ToNextTurn);
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
