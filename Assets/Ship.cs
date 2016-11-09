using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

public class Ship : MonoBehaviour
{

    public float MoveSpeedSec=2; //movement speed in units per second
    public float MoveSpeed;
    public float TurnSpeedSec =50; //turning speed in degrees
    public float TurnSpeed;
    private Vector2 _destination = new Vector2();

    // Use this for initialization
    void Start () {
	    _destination.Set(transform.position.x,transform.position.y);
        MoveSpeed = MoveSpeedSec*ConfigurationManager.Instance.FixedUpdateStep;
        TurnSpeed= TurnSpeedSec* ConfigurationManager.Instance.FixedUpdateStep;
        _destination.Set(4,4);
    }

    // Update is called once per frame
    void FixedUpdate () {
	        Move(_destination);
	    }

       public void Move(Vector3 destination)
        {
        if ((Vector2.Distance(destination, transform.position)) < MoveSpeed)
        {
            transform.position = destination;
        }
        else
        {

           
            transform.rotation =ShipRotation(destination);

            //move forward
            transform.position += ShipMovement(); //move forward (right = x axis = red arrow = forward)
        }

    }
    /// <summary>
    /// Computes the ship's new rotation
    /// </summary>
    /// <returns>Rotation after a single frame</returns>
     Quaternion ShipRotation(Vector2 destination)
    {
        //Find rotation and adjust angle
        Vector2 vectorToTarget = destination - (Vector2)transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        //return new rotation
        return Quaternion.RotateTowards(transform.rotation, rotation, TurnSpeed);
    }
    /// <summary>
    /// Computes the new ship's location
    /// </summary>
    /// <returns>New ships location</returns>
     Vector3 ShipMovement()
    {
        return MoveSpeed*transform.right;
    }
}
