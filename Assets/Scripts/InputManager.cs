using System.Collections.Generic;
using Assets.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class InputManager : UnitySingleton<InputManager>
    {

        public Ship Ship;
        public LineRenderer LineRenderer;
        public GameObject Marker;
        // Use this for initialization
        void Start()
        {
            LineRenderer.startColor=Color.red;
            LineRenderer.endColor=Color.yellow;
        }

        // Update is called once per frame
        void Update()
        {
            if (Ship)
            {
                Marker.SetActive(true);
                Marker.transform.position = Ship.transform.position;
                var mousePointer = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (LineRenderer.enabled)
                {
                    PathsManager.Instance.DrawPath(Ship, mousePointer, LineRenderer);
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




        private Missile _missilePrefab;

        private Missile MissilePrefab
        {
            get { return _missilePrefab ?? (_missilePrefab = Resources.Load<Missile>("Missile")); }
        }

        void OnLeftMouseClick()
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                var clicked = hit.collider.GetComponent<Ship>();
                if (clicked != null)
                {
                    Ship = clicked;
                }
            }
            else
            {
                var mousePointer = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var missile = Instantiate(MissilePrefab, Ship.transform.position, Ship.transform.rotation) as Missile;

                missile.Destination = mousePointer;
            }
        }

        void OnRightMouseClick()
        {
            Vector3 mousePointer = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePointer.z = 0;
            Ship.SetDestination(mousePointer);
            PathsManager.Instance.DrawPath(Ship, mousePointer);
        }
    }
}
