using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public static Action<Direction, GameObject> onSwiped;
    public LayerMask layerMask;

    public enum Direction {
        None,
        Up,
        Right,
        Down,
        Left
    }

    public void OnBeginDrag(PointerEventData eventData) {
        Direction direction = GetDirection(eventData.delta);

        GameObject selectedGameObject = SelectGameObject();
        if (selectedGameObject == null) {
            return;
        }

        onSwiped?.Invoke(GetDirection(eventData.delta), selectedGameObject);
        Debug.Log("SWIPED TO: " + direction);
    }

    public void OnDrag(PointerEventData eventData) {
    }

    public void OnEndDrag(PointerEventData eventData) {
    }

    private GameObject SelectGameObject() {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
            Transform objectHit = hit.transform;

            return objectHit.gameObject;
        }
        return null;
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
