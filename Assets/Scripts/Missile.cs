using UnityEngine;
using System;
namespace Assets.Scripts
{
    public class Missile : SpaceObject
    {
        private static Missile _missilePrefab;
        public static Missile MissilePrefab
        {
            get { return _missilePrefab ?? (_missilePrefab = Resources.Load<Missile>("Missile")); }
        }
        public CircleCollider2D CircleCollider2D;
        private bool _destroyNextFrame;
        public int Damage;
        public static float BlastRadius=2;
        public float Lifetime;
        // Use this for initialization
        void Start ()
        {
            MoveSpeed = MoveSpeedSec * ConfigurationManager.Instance.FixedUpdateStep;
            TurnSpeed = TurnSpeedSec * ConfigurationManager.Instance.FixedUpdateStep;
            Destination.z = 0;
            transform.position += transform.right*2;
            PathsManager.Instance.DrawPath(this, Destination);
            LineRenderer.endColor = Color.black;
            LineRenderer.startWidth = 0.05f;
            LineRenderer.endWidth = 0.05f;
        }


        // Update is called once per frame
        void FixedUpdate()
        {
            if (_destroyNextFrame||Lifetime<=0)
            {
                Destroy(gameObject);
            }
            if ((Vector2.Distance(Destination, transform.position)) < MoveSpeed)
            {
                transform.position = Destination;
                DestroyNextFrame();
            }
            Lifetime -= ConfigurationManager.Instance.FixedUpdateStep;
            Move(Destination);
        }


        void OnTriggerEnter2D(Collider2D other)
        {
            var collided = other.GetComponent<Ship>();
            if (collided)
            {
                DestroyNextFrame();
            }
        }

        void DestroyNextFrame()
        {
            _destroyNextFrame = true;
            CircleCollider2D.radius = BlastRadius;
        }
    }
}
