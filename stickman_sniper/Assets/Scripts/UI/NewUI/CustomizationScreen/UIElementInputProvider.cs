using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIElementInputProvider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler, IScrollHandler
{
    private Subject<Vector2> _mouseMoveSubject = new();
    public IObservable<Vector2> MouseMoveObservable => _mouseMoveSubject;


    private Subject<Unit> _mouseClickSubject = new();
    public IObservable<Unit> _mouseClickObservable => _mouseClickSubject;


    private Subject<Vector2> _mouseScrollSubject = new();
    public IObservable<Vector2> MouseScrollObservable => _mouseScrollSubject;


    private bool _isLocked;

    public void OnPointerDown(PointerEventData eventData)
    {
        _isLocked = true;
        _mouseClickSubject.OnNext(Unit.Default);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isLocked = false;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (!_isLocked)
            return;

        _mouseMoveSubject.OnNext(eventData.delta);
    }

    public void OnScroll(PointerEventData eventData)
    {
        _mouseScrollSubject.OnNext(eventData.delta);
    }
}
