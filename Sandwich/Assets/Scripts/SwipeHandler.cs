using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public static Action<Direction> onSwiped;

    public enum Direction {
        None,
        Up,
        Right,
        Down,
        Left
    }

    public void OnBeginDrag(PointerEventData eventData) {
        Direction direction = GetDirection(eventData.delta);
        Debug.Log("SWIPED TO: " + direction);

        onSwiped?.Invoke(GetDirection(eventData.delta));
    }
    public void OnDrag(PointerEventData eventData) {
    }

    public void OnEndDrag(PointerEventData eventData) {
    }

    private Direction GetDirection(Vector2 delta) {
        if (delta.x == 0 && delta.y == 0) {
            return Direction.None;
        }

        // I. Area
        if (delta.x >= 0 && delta.y >= 0) {
            if (Math.Abs(delta.x) > Math.Abs(delta.y)) {
                return Direction.Right;
            } else {
                return Direction.Up;
            }
        } 
        // II. Area
        else if (delta.x <= 0 && delta.y >= 0) {
            if (Math.Abs(delta.x) > Math.Abs(delta.y)) {
                return Direction.Left;
            } else {
                return Direction.Up;
            }
        }
        // III. Area
        else if (delta.x <= 0 && delta.y <= 0) {
            if (Math.Abs(delta.x) > Math.Abs(delta.y)) {
                return Direction.Left;
            } else {
                return Direction.Down;
            }
        }
        // IV. Area
        else if (delta.x >= 0 && delta.y <= 0) {
            if (Math.Abs(delta.x) > Math.Abs(delta.y)) {
                return Direction.Right;
            } else {
                return Direction.Down;
            }
        }

        return Direction.None;
    }

}
