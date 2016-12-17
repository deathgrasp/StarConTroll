using UnityEngine;

namespace Assets.Scripts
{
    public class Ship : SpaceObject
    {
        // Use this for initialization
        void Start()
        {
            Destination = transform.position;
            MoveSpeed = MoveSpeedSec * ConfigurationManager.Instance.FixedUpdateStep;
            TurnSpeed = TurnSpeedSec * ConfigurationManager.Instance.FixedUpdateStep;
            LineRenderer.SetColors(Color.green, Color.blue);
            LineRenderer.SetWidth(0.05f, 0.05f);
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
