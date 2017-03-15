using UnityEngine;

namespace Assets.Scripts
{
    public class Ship : SpaceObject
    {
        public int Owner {get; set;}
        private Command[] CommandQueue;

        // Use this for initialization
        void Awake()
        {
            CommandQueue = new Command[4]; // TODO: later change '4' to general, per-ship amount
            Destination = transform.position;
            MoveSpeed = MoveSpeedSec * ConfigurationManager.FixedUpdateStep;
            TurnSpeed = TurnSpeedSec * ConfigurationManager.FixedUpdateStep;
            LineRenderer.startColor = Color.green;
            LineRenderer.endColor = Color.blue;
            LineRenderer.startWidth = 0.05f;
            LineRenderer.endWidth = 0.05f;
            PathsManager.Instance.DrawPath(this, Destination);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (TurnManager.Instance.CurrentUpdate == 0) print("moo");
            Move(Destination); // update movement

            // activate abilities from queue
            for (int i=0; i<CommandQueue.Length; i++)
            {
                if (CommandQueue[i] != null && TurnManager.Instance.CurrentUpdate == i*ConfigurationManager.UpdatesInTurn/4)
                {
                    CommandQueue[i].Execute();
                    SetCommand(i, null);
                }
            }
        }


        public void SetDestination(Vector3 destination)
        {
            Destination = destination;
        }


        public void SetCommand(int index, Command command)
        {
            CommandQueue[index] = command;
        }


        public Command[] GetCommandQueue()
        {
            return CommandQueue;
        }


        // TODO: this is temp for iteration 1. go over and make robust
        public int GetUnassignedCommand()
        {
            for (int i=0; i<CommandQueue.Length; i++)
            {
                if (CommandQueue[i] == null) { return i; }
            }

            // TODO: the following is temp behaviour. decide on how we want to handle this case (new command when queue is full)
            // if the entire command queue is filled override the last one
            return CommandQueue.Length-1;
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
