using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour
{
    public float MoveSpeedSec = 4; //movement speed in units per second
    public float MoveSpeed;
    public float TurnSpeedSec = 30; //turning speed in degrees
    public float TurnSpeed;

    public Vector3 Direction;
    public CircleCollider2D CircleCollider2D;
    private bool _destroyNextFrame;
	// Use this for initialization
	void Start ()
	{
        MoveSpeed = MoveSpeedSec * ConfigurationManager.Instance.FixedUpdateStep;
        TurnSpeed = TurnSpeedSec * ConfigurationManager.Instance.FixedUpdateStep;
	    Direction.z = 0;
	    transform.position += transform.right*2;
	}


    // Update is called once per frame
    void FixedUpdate()
    {
        if (_destroyNextFrame)
        {
            Destroy(gameObject);
        }
        Move(Direction);
    }

    public void Move(Vector3 destination)
    {
        if ((Vector2.Distance(destination, transform.position)) < MoveSpeed)
        {
            transform.position = destination;
            DestroyNextFrame();
        }
        else
        {
            transform.rotation = ShipRotation(destination);
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
        return MoveSpeed * transform.right;
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
