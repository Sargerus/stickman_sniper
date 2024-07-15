using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[TaskCategory("Stickman")]
public class ChooseWaypointTransform : Action
{
    public int WaypointType;
    public SharedTransform WaypointsContainer;
    public SharedTransform CurrentWaypoint;

    private List<Transform> _waypoints;
    private int _currentIndex = -1;

    public override void OnAwake()
    {
        _waypoints = WaypointsContainer.Value.GetComponentsInChildren<Waypoint>().Where(g => g.WaypointType == WaypointType)
            .Select(g => g.transform).ToList();
    }

    public override TaskStatus OnUpdate()
    {
        Next();
        return TaskStatus.Success;
    }

    public Transform Next()
    {
        if (_currentIndex + 1 >= _waypoints.Count)
        {
            _currentIndex = -1;
        }

        _currentIndex++;
        CurrentWaypoint.Value = _waypoints[_currentIndex].transform;

        return CurrentWaypoint.Value;
    }
}
