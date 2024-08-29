using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DWTools.Extensions;
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
        if (WaypointsContainer.Value != null)
            _waypoints = WaypointsContainer.Value.GetComponentsInChildren<Waypoint>().Where(g => g.WaypointType == WaypointType)
                .Select(g => g.transform).ToList();
    }

    public override TaskStatus OnUpdate()
    {
        if (_waypoints == null || _waypoints.Count == 0)
            return TaskStatus.Failure;

        if (CurrentWaypoint.Value == null)
            Next();

        return TaskStatus.Success;
    }

    public Transform Next()
    {
        CurrentWaypoint.Value = _waypoints.Random().transform;
        return CurrentWaypoint.Value;
    }
}
