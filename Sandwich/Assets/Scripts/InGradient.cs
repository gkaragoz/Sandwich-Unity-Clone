using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGradient : MonoBehaviour {

    private Vector3 upJoint;
    private Vector3 rightJoint;
    private Vector3 downJoint;
    private Vector3 leftJoint;

    public bool isFlippedHorizontal = false;
    public bool isFlippedVertical = false;

    private void Awake() {
        SwipeHandler.onSwiped += OnSwiped;

        CalculateJoints();
    }

    private void CalculateJoints() {
        if (isFlippedVertical) {
            upJoint = transform.localPosition - transform.forward;
            downJoint = transform.localPosition + transform.forward;
        } else {
            upJoint = transform.localPosition + transform.forward;
            downJoint = transform.localPosition - transform.forward;
        }

        if (isFlippedHorizontal) {
            rightJoint = transform.localPosition - transform.right;
            leftJoint = transform.localPosition + transform.right;
        } else {
            rightJoint = transform.localPosition + transform.right;
            leftJoint = transform.localPosition - transform.right;
        }
    }

    private void OnSwiped(SwipeHandler.Direction direction) {
        if (direction == SwipeHandler.Direction.None) {
            return;
        }

        switch (direction) {
            case SwipeHandler.Direction.Up:
                transform.RotateAround(upJoint, Vector3.right, 180);
                isFlippedVertical = !isFlippedVertical;
                break;
            case SwipeHandler.Direction.Down:
                transform.RotateAround(downJoint, Vector3.right, 180);
                isFlippedVertical = !isFlippedVertical;
                break;
            case SwipeHandler.Direction.Right:
                transform.RotateAround(rightJoint, Vector3.forward, -180);
                isFlippedHorizontal = !isFlippedHorizontal;
                break;
            case SwipeHandler.Direction.Left:
                transform.RotateAround(leftJoint, Vector3.forward, 180);
                isFlippedHorizontal = !isFlippedHorizontal;
                break;
        }

        CalculateJoints();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(upJoint, 0.25f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(downJoint, 0.25f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(rightJoint, 0.25f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(leftJoint, 0.25f);
    }

}
