using UnityEngine;

namespace Assets.Scripts
{
    public class Ship : SpaceObject
    {
        public int Owner;//Rani: TODO: change to get/set. Make sure name tells us the owner

        // Use this for initialization
        void Start()
        {
            Destination = transform.position;
            MoveSpeed = MoveSpeedSec * ConfigurationManager.Instance.FixedUpdateStep;
            TurnSpeed = TurnSpeedSec * ConfigurationManager.Instance.FixedUpdateStep;
            LineRenderer.startColor = Color.green;
            LineRenderer.endColor = Color.blue;
            LineRenderer.startWidth = 0.05f;
            LineRenderer.endWidth = 0.05f;
            PathsManager.Instance.DrawPath(this, Destination);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            Move(Destination);
        }

        public void SetDestination(Vector3 destination)
        {
            Destination = destination;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var collided = other.GetComponent<Missile>();
            if (collided)
            {
                SufferDamage(collided.Damage);
            }
        }
    }
}
