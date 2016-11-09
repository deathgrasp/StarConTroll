using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Utils;

public class InputManager : UnitySingleton<InputManager>
{

    public Ship Ship;
    public LineRenderer LineRenderer;
    private const int MAXPATHPOINTS = 1000;
    // Use this for initialization
    void Start()
    {
        LineRenderer.SetColors(Color.red, Color.yellow);
        LineRenderer.SetWidth(0.2f, 0.2f);
        for (int i = 0; i < MAXPATHPOINTS; i++)
        {
            _helperStack.Push(new Vector3());
        }
    }

    // Update is called once per frame
    void Update()
    {
        DrawPath();
    }

    private Stack<Vector3> _pathPointStack = new Stack<Vector3>();
    private Stack<Vector3> _helperStack = new Stack<Vector3>(1000);
    private Vector3 _lastMousePosition = new Vector3();
    //clean code
    void DrawPath()
    {

        //get mouse location
        Vector3 mousePointer = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePointer.z = 0;

        //pravent multi-computation. commented out, as the world currently moves.
        //if (Vector3.Distance(_lastMousePosition, mousePointer) < 0.5)
        //    return;
        //_lastMousePosition = mousePointer;

        //save ship location
        var initialPosition = Ship.transform.position;
        var initialRotation = Ship.transform.rotation;
        var counter = 0;
        while (Vector2.Distance(mousePointer, Ship.transform.position) > Ship.MoveSpeed && counter < MAXPATHPOINTS)
        {
            // move the ship one step at a time, and save its location. stops when the ship reaches the destination.
            AddPoint(Ship.transform.position);
            Ship.Move(mousePointer);
            counter++;
        }

        //add points to the linerenderer
        LineRenderer.SetVertexCount(_pathPointStack.Count);
        LineRenderer.SetPositions(_pathPointStack.ToArray());


        //return to initial state
        Ship.transform.position = initialPosition;
        Ship.transform.rotation = initialRotation;
        ClearStack();
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
