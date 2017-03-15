using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    public class PathsManager : UnitySingleton<PathsManager>
    {
        private Stack<Vector3> _pathPointStack = new Stack<Vector3>(); //current path. getting restarted on each DrawPath and then repopulated again
        private Stack<Vector3> _helperStack = new Stack<Vector3>();//helper to keep avoid instantiating new vectors each time
        private const int MAXPATHPOINTS = 1000;
        void Start()
        {
            for (int i = 0; i < MAXPATHPOINTS; i++)
            {
                _helperStack.Push(new Vector3());
            }
        }
        public void DrawPath(SpaceObject so, Vector3 destination, LineRenderer lineRenderer=null)
        {
            ClearStack();
            destination.z = 0;
            if (!lineRenderer) //if not given a linerenderer, uses the spaceobject's
            {
                lineRenderer = so.LineRenderer;
            }
            //save ship location
            var initialPosition = so.transform.position;
            var initialRotation = so.transform.rotation;
            var counter = 0;
            while (Vector2.Distance(destination, so.transform.position) > so.MoveSpeed && counter < MAXPATHPOINTS)
            {
                // move the ship one step at a time, and save its location. stops when the ship reaches the destination.
                AddPoint(so.transform.position);
                so.Move(destination);
                counter++;
            }

            //add points to the linerenderer
            lineRenderer.numPositions=_pathPointStack.Count;
            lineRenderer.SetPositions(_pathPointStack.Reverse().ToArray());
            //return to initial state
            so.transform.position = initialPosition;
            so.transform.rotation = initialRotation;
        }
        //move a point from the pool into the stack and set its values
        void AddPoint(Vector3 point)
        {
            var popped = _helperStack.Pop();
            popped.Set(point.x, point.y, point.z);
            _pathPointStack.Push(popped);
        }

        void ClearStack()
        {
            var count = _pathPointStack.Count;
            for (int i = 0; i < count; i++)
            {
                _helperStack.Push(_pathPointStack.Pop());
            }
        }
    }
}
