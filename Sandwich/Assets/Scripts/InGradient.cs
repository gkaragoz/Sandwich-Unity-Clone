using System;
using System.Collections.Generic;
using UnityEngine;

public class InGradient : MonoBehaviour {

    [Header("Initiailizations")]
    public LeanTweenType easeType;
    public float speed = 0.4f;
    public LayerMask layerMask;
    public InGradientType type;

    [SerializeField]
    private GameObject _rotatePivot = null;

    [Header("DEBUG")]
    [SerializeField]
    private List<InGradient> _inGradients = new List<InGradient>();
    [SerializeField]
    private bool _isFlippedHorizontal = false;
    [SerializeField]
    private bool _isFlippedVertical = false;

    private Vector3 _upJoint;
    private Vector3 _rightJoint;
    private Vector3 _downJoint;
    private Vector3 _leftJoint;

    private float _inGradientHeight = 0.25f;
    private InGradient _neighbourInGradient;

    public enum InGradientType {
        Bread,
        Cheese,
        Meat,
        Pickle,
        Tomatoe
    }

    public int MovementCount { get; private set; }

    public List<InGradient> GetStackedInGradients() {
        return _inGradients;
    }

    private void Awake() {
        SwipeHandler.onSwiped += OnSwiped;

        _inGradients.Add(this);

        _inGradientHeight = transform.localScale.y * 0.5f;

        CalculateJoints();
    }
    
    private void OnSwiped(SwipeHandler.Direction direction, GameObject swipedObject) {
        if (direction == SwipeHandler.Direction.None || IsItMe(swipedObject) == false || IsAnyTweenActive()) {
            return;
        }

        _neighbourInGradient = HasNeighbour(direction);
        if (_neighbourInGradient == null) {
            return;
        } 

        int stackedAmount = _neighbourInGradient.GetStackedInGradients().Count;

        switch (direction) {
            case SwipeHandler.Direction.Up:
                LeanAnimation(_upJoint, Vector3.right, 180, true, stackedAmount);
                break;
            case SwipeHandler.Direction.Down:
                LeanAnimation(_downJoint, Vector3.right, -180, true, stackedAmount);
                break;
            case SwipeHandler.Direction.Right:
                LeanAnimation(_rightJoint, Vector3.forward, -180, false, stackedAmount);
                break;
            case SwipeHandler.Direction.Left:
                LeanAnimation(_leftJoint, Vector3.forward, 180, false, stackedAmount);
                break;
        }
    }

    private InGradient HasNeighbour(SwipeHandler.Direction direction) {
        RaycastHit hitInfo;
        Ray upRay = new Ray(transform.position, Vector3.forward);
        Ray rightRay = new Ray(transform.position, Vector3.right);
        Ray downRay = new Ray(transform.position, Vector3.back);
        Ray leftRay = new Ray(transform.position, Vector3.left);

        switch (direction) {
            case SwipeHandler.Direction.Up:
                if (Physics.Raycast(upRay, out hitInfo, transform.localScale.z, layerMask)) {
                    return hitInfo.transform.gameObject.GetComponent<InGradient>();
                }
                break;
            case SwipeHandler.Direction.Right:
                if (Physics.Raycast(rightRay, out hitInfo, transform.localScale.x, layerMask)) {
                    return hitInfo.transform.gameObject.GetComponent<InGradient>();
                }
                break;
            case SwipeHandler.Direction.Down:
                if (Physics.Raycast(downRay, out hitInfo, transform.localScale.z, layerMask)) {
                    return hitInfo.transform.gameObject.GetComponent<InGradient>();
                }
                break;
            case SwipeHandler.Direction.Left:
                if (Physics.Raycast(leftRay, out hitInfo, transform.localScale.x, layerMask)) {
                    return hitInfo.transform.gameObject.GetComponent<InGradient>();
                }
                break;
        }

        return null;
    }

    private void CalculateJoints() {
        if (_isFlippedVertical) {
            _upJoint = transform.position - transform.forward;
            _downJoint = transform.position + transform.forward;
        } else {
            _upJoint = transform.position + transform.forward;
            _downJoint = transform.position - transform.forward;
        }

        if (_isFlippedHorizontal) {
            _rightJoint = transform.position - transform.right;
            _leftJoint = transform.position + transform.right;
        } else {
            _rightJoint = transform.position + transform.right;
            _leftJoint = transform.position - transform.right;
        }
    }

    private float GetTargetHeight(int stackedAmount) {
        return _inGradientHeight * (stackedAmount + this._inGradients.Count - 1);
    }

    private bool IsAnyTweenActive() {
        return LeanTween.isTweening(_rotatePivot);
    }

    private bool IsItMe(GameObject targetGameObject) {
        return this.gameObject == targetGameObject ? true : false;
    }

    private void LeanAnimation(Vector3 joint, Vector3 axis, float angle, bool isVertical, int stackedAmount) {
        _rotatePivot.transform.position = joint + (Vector3.up * GetTargetHeight(stackedAmount));
        _rotatePivot.transform.eulerAngles = Vector3.zero;

        transform.SetParent(_rotatePivot.transform);

        LeanTween.rotateAroundLocal(_rotatePivot, axis, angle, speed)
            .setEase(easeType)
            .setOnComplete(() => {
                transform.SetParent(_neighbourInGradient.transform);
                this.GetComponent<Collider>().enabled = false;

                if (isVertical) {
                    _isFlippedVertical = !_isFlippedVertical;
                } else {
                    _isFlippedHorizontal = !_isFlippedHorizontal;
                }

                CalculateJoints();

                MovementCount++;

                _neighbourInGradient.AddInGradient(this);
            });
    }

    private void AddInGradient(InGradient inGradient) {
        this._inGradients.AddRange(inGradient.GetStackedInGradients());
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_upJoint, 0.25f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(_downJoint, 0.25f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_rightJoint, 0.25f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_leftJoint, 0.25f);
    }

}
