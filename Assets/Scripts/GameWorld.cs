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
        public LineRenderer LineRenderer;
        public GameObject Marker;
        private Ship currentSelection;
        private Ability currentAbility;
        public int currentPlayer = Player.PLAYER_1;
        private bool isPlayer1Done = false;
        private bool isPlayer2Done = false;


        private Missile _missilePrefab;
        private Missile MissilePrefab
        {
            get { return _missilePrefab ?? (_missilePrefab = Resources.Load<Missile>("Missile")); }
        }

        // Use this for initialization
        void Awake()
        {
            Ship1.Owner = Player.PLAYER_1;
            Ship2.Owner = Player.PLAYER_2;
            Ship1.RotateShipTowards(Vector3.zero,360);
            Ship2.RotateShipTowards(Vector3.zero,360);
            SetSelection(Ship1);

            LineRenderer.startColor=Color.red;
            LineRenderer.endColor=Color.yellow;
            SwitchToPlanningPhase();
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
                    SetSelection(clicked);
                }
            }
            else
            {
                Command command = Command_ShootMissile.Create(currentSelection, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                currentSelection.SetCommand(currentSelection.GetUnassignedCommand(), command);
                updateCommandQueueUI();
            }
        }


        private void SetSelection(Ship ship)
        {
            if (ship != null && ship.Owner == currentPlayer) 
            {
                currentSelection = ship;
                updateCommandQueueUI();
            }
        }


        private void updateCommandQueueUI()
        {
            Command[] commandQueue = currentSelection.GetCommandQueue();
            for (int i=0; i < commandQueue.Length; i++)
            {
                if (commandQueue[i] == null) { CommandUiIcons[i].GetComponent<Image>().color = Color.white; }
                else { CommandUiIcons[i].GetComponent<Image>().color = Color.magenta; }
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
                SetSelection(Ship2);
            }
            else if (currentPlayer == Player.PLAYER_2) 
            { 
                isPlayer2Done = true; 
                currentPlayer = Player.PLAYER_1; // TODO: this will change we will stop using hot-seat playerd
                SetSelection(Ship1);
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
            SetSelection(Ship1);
        }


        private void SwitchToExecutionPhase()
        {
            Marker.SetActive(false);
            TurnOffMouseLineRenderer();
            TurnOffAllGuides();
            // TODO: remove all the ships' and projectiles trajectories UI
            currentPlayer = Player.NO_ONE;
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
