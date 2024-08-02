using BehaviorDesigner.Runtime;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Reflection;
using UniRx;
using UnityEngine;
using Zenject;
//using UnityEngine;

[RequireComponent(typeof(BehaviorManager))]
public class InjectBehaviorManagerTasks : MonoBehaviour
{
    private HashSet<int> processedIndexes = new();
    private IDisposable _collectionDisposable;
    private bool _isInitialied;
    private List<BehaviorDesigner.Runtime.BehaviorManager.BehaviorTree> _behaviorTrees;
    private int _count=0;
    BehaviorManager bm;

    [Inject] private DiContainer _diContainer;

    [Inject]
    private void Construct()
    {
        if (_diContainer == null)
        {
            Debug.LogError("No diContainer in bootstrreap!? InjectBehaviorManagerTasks");
            return;
        }

        bm = GetComponent<BehaviorManager>();
        _behaviorTrees =
            (List<BehaviorDesigner.Runtime.BehaviorManager.BehaviorTree>)typeof(BehaviorManager).GetField("behaviorTrees", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(bm);

        _isInitialied = true;

        //_collectionDisposable = behaviorTrees.ToReactiveCollection().ObserveAdd().Subscribe(e => { ProcessTree(e.Value); });
        //foreach (var tree in behaviorTrees)
        //    ProcessTree(tree);
    }

    public void ClearTreeData()
    {
        processedIndexes.Clear();
    }

    private void Update()
    {
        if (!_isInitialied)
            return;

        if (_behaviorTrees.Count == _count)
            return;

        _behaviorTrees.ForEach(ProcessTree);
        _count = _behaviorTrees.Count;
    }

    private void ProcessTree(BehaviorManager.BehaviorTree tree)
    {
        if (processedIndexes.Contains(tree.GetHashCode()))
            return;

        foreach (var task in tree.taskList)
            _diContainer.Inject(task);

        processedIndexes.Add(tree.GetHashCode());
    }

    private void Start()
    {
        //if (_diContainer == null)
        //{
        //    Debug.LogError("No diContainer in bootstrreap!? InjectBehaviorManagerTasks");
        //    return;
        //}
        //
        //var bm = GetComponent<BehaviorManager>();
        //List<BehaviorDesigner.Runtime.BehaviorManager.BehaviorTree> behaviorTrees = 
        //    (List<BehaviorDesigner.Runtime.BehaviorManager.BehaviorTree>)typeof(BehaviorManager).GetField("behaviorTrees", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(bm);
        //
        //_collectionDisposable = behaviorTrees.ToReactiveCollection().ObserveAdd().Subscribe(e =>
        //{
        //    var tree = e.Value;
        //    if (processedIndexes.Contains(tree.GetHashCode()))
        //        return;
        //
        //    foreach (var task in tree.taskList)
        //        _diContainer.Inject(task);
        //
        //    processedIndexes.Add(tree.GetHashCode());
        //});
    }

    private void OnDestroy()
    {
        _collectionDisposable?.Dispose();
    }
}
