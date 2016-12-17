using UnityEngine;

namespace Assets.Scripts
{
    public class Missile : SpaceObject
    {
        public CircleCollider2D CircleCollider2D;
        private bool _destroyNextFrame;
        public int Damage;
        // Use this for initialization
        void Start ()
        {
            MoveSpeed = MoveSpeedSec * ConfigurationManager.Instance.FixedUpdateStep;
            TurnSpeed = TurnSpeedSec * ConfigurationManager.Instance.FixedUpdateStep;
            Destination.z = 0;
            transform.position += transform.right*2;
        }


        // Update is called once per frame
        void FixedUpdate()
        {
            if (_destroyNextFrame)
            {
                Destroy(gameObject);
            }
            if ((Vector2.Distance(Destination, transform.position)) < MoveSpeed)
            {
                transform.position = Destination;
                DestroyNextFrame();
            }
            Move(Destination);
        }


        void OnTriggerEnter2D(Collider2D other)
        {
            DestroyNextFrame();
        }

        void DestroyNextFrame()
        {
            _destroyNextFrame = true;
            CircleCollider2D.radius = 10;
        }
    }
}
