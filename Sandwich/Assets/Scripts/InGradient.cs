using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGradient : MonoBehaviour {

    public LeanTweenType easeType;
    public float speed = 0.4f;

    private Vector3 upJoint;
    private Vector3 rightJoint;
    private Vector3 downJoint;
    private Vector3 leftJoint;

    public bool isFlippedHorizontal = false;
    public bool isFlippedVertical = false;

    [SerializeField]
    private GameObject _rotatePivot = null;

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
        if (direction == SwipeHandler.Direction.None || IsAnyTweenActive()) {
            return;
        }

        switch (direction) {
            case SwipeHandler.Direction.Up:
                LeanAnimation(upJoint, Vector3.right, 180, true);
                break;
            case SwipeHandler.Direction.Down:
                LeanAnimation(downJoint, Vector3.right, -180, true);
                break;
            case SwipeHandler.Direction.Right:
                LeanAnimation(rightJoint, Vector3.forward, -180, false);
                break;
            case SwipeHandler.Direction.Left:
                LeanAnimation(leftJoint, Vector3.forward, 180, false);
                break;
        }
    }

    private bool IsAnyTweenActive() {
        return LeanTween.isTweening(_rotatePivot);
    }

    private void LeanAnimation(Vector3 joint, Vector3 axis, float angle, bool isVertical) {
        _rotatePivot.transform.position = joint;
        _rotatePivot.transform.eulerAngles = Vector3.zero;

        transform.SetParent(_rotatePivot.transform);

        LeanTween.rotateAroundLocal(_rotatePivot, axis, angle, speed)
            .setEase(easeType)
            .setOnComplete(() => {
                transform.SetParent(null);

                if (isVertical) {
                    isFlippedVertical = !isFlippedVertical;
                } else {
                    isFlippedHorizontal = !isFlippedHorizontal;
                }

                CalculateJoints();
            });
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
