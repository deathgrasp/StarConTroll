using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class Ship : SpaceObject
    {
        public Player Owner {get; set;}
        private Command[] CommandQueue;
        private List<Ability> AbilityList = new List<Ability>();
        private Image[] CommandUiIcons; // TODO: this should be moved to the "ShipUI" class later

        // Use this for initialization
        void Awake()
        {
            CommandQueue = new Command[4]; // TODO: later change '4' to general, per-ship amount
            CommandUiIcons = new Image[4]; // TODO: later change '4' to general, per-ship amount. also this should be moved to the "ShipUI" class later
            Destination = transform.position;
            MoveSpeed = MoveSpeedSec * ConfigurationManager.FixedUpdateStep;
            TurnSpeed = TurnSpeedSec * ConfigurationManager.FixedUpdateStep;
            LineRenderer.startColor = Color.green;
            LineRenderer.endColor = Color.blue;
            LineRenderer.startWidth = 0.05f;
            LineRenderer.endWidth = 0.05f;
            PathsManager.Instance.DrawPath(this, Destination);

            // TODO: pack the linerender and the ability icons shown on the path, and every other UI item into a new class "ShipUI"
            //Image newImage = Instantiate(commandUiIconPrefab, UICanvas.Instance);
            //Image[0] = newImage;
        	//newButton.transform.SetParent(newCanvas.transform, false);

            // add abilites to ship
            AbilityList.Add(ScriptableObject.CreateInstance<Ability_ShootMissile>()); // TODO: change the entire ability list to components?
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


        public List<Ability> GetAbilities()
        {
            return AbilityList;
        }


        // TODO: this is temp for iteration 1. go over and make robust (this is currently used whenever a new command is issued. it finds empty space on command queue nad returns it's index)
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
