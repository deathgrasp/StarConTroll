﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public abstract class SpaceObject : MonoBehaviour
    {
        public int Health;
        public float MoveSpeedSec; //movement speed in units per second
        public float MoveSpeed;
        public float TurnSpeedSec; //turning speed in degrees
        public float TurnSpeed;
        public Vector3 Destination;
        public LineRenderer LineRenderer;

        public void SufferDamage(int damage)
        {
            Health -= damage;
            if (Health<=0)
            {
                Destroy(gameObject);
            }
        }

        public void Move(Vector3 destination)
        {
            if ((Vector2.Distance(destination, transform.position)) < MoveSpeed)
            {
                transform.position = destination;
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

    }
}
