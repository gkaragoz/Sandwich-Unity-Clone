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
    [SerializeField]
    private SwipeHandler.Direction _lastDirection;
    [SerializeField]
    private int _lastStackedAmount;


    private Vector3 _upJoint;
    private Vector3 _rightJoint;
    private Vector3 _downJoint;
    private Vector3 _leftJoint;

    private float _inGradientHeight = 0.25f;
    private InGradient _lastNeighbourInGradient;

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

        _lastNeighbourInGradient = HasNeighbour(direction);
        if (_lastNeighbourInGradient == null) {
            return;
        }

        _lastStackedAmount = _lastNeighbourInGradient.GetStackedInGradients().Count;
        _lastDirection = direction;

        switch (_lastDirection) {
            case SwipeHandler.Direction.Up:
                LeanAnimation(_upJoint, Vector3.right, 180, true, _lastStackedAmount, false);
                break;
            case SwipeHandler.Direction.Down:
                LeanAnimation(_downJoint, Vector3.right, -180, true, _lastStackedAmount, false);
                break;
            case SwipeHandler.Direction.Right:
                LeanAnimation(_rightJoint, Vector3.forward, -180, false, _lastStackedAmount, false);
                break;
            case SwipeHandler.Direction.Left:
                LeanAnimation(_leftJoint, Vector3.forward, 180, false, _lastStackedAmount, false);
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

    private void LeanAnimation(Vector3 joint, Vector3 axis, float angle, bool isVertical, int stackedAmount, bool reversed) {
        if (reversed == false) {
            _rotatePivot.transform.position = joint + (Vector3.up * GetTargetHeight(stackedAmount));
        } else {
            _rotatePivot.transform.position = joint - (Vector3.up * GetTargetHeight(stackedAmount));
        }
        _rotatePivot.transform.eulerAngles = Vector3.zero;

        transform.SetParent(_rotatePivot.transform);

        LeanTween.rotateAroundLocal(_rotatePivot, axis, angle, speed)
            .setEase(easeType)
            .setOnComplete(() => {
                if (reversed == false)
                    transform.SetParent(_lastNeighbourInGradient.transform);
                else
                    transform.SetParent(null);

                if (reversed == false)
                    this.GetComponent<Collider>().enabled = false;
                else
                    this.GetComponent<Collider>().enabled = true;

                if (isVertical) {
                    _isFlippedVertical = !_isFlippedVertical;
                } else {
                    _isFlippedHorizontal = !_isFlippedHorizontal;
                }

                CalculateJoints();

                if (reversed == false)
                    MovementCount++;
                else
                    MovementCount--;

                if (reversed == false)
                    _lastNeighbourInGradient.AddInGradient(this);
                else
                    _lastNeighbourInGradient.RemoveInGradient(this);

                if (reversed == false)
                    HistoryTracker.inGradients.Push(this);
                else
                    HistoryTracker.hasUndoFinished = true;
            });
    }

    public void Undo() {
        Debug.Log("Undo: " + this.gameObject);

        switch (_lastDirection) {
            case SwipeHandler.Direction.Up:
                LeanAnimation(_downJoint, Vector3.right, -180, true, _lastStackedAmount, true);
                break;
            case SwipeHandler.Direction.Down:
                LeanAnimation(_upJoint, Vector3.right, 180, true, _lastStackedAmount, true);
                break;
            case SwipeHandler.Direction.Right:
                LeanAnimation(_leftJoint, Vector3.forward, 180, false, _lastStackedAmount, true);
                break;
            case SwipeHandler.Direction.Left:
                LeanAnimation(_rightJoint, Vector3.forward, -180, false, _lastStackedAmount, true);
                break;
        }
    }

    private void AddInGradient(InGradient inGradient) {
        this._inGradients.AddRange(inGradient.GetStackedInGradients());
    }

    private void RemoveInGradient(InGradient inGradient) {
        foreach (var item in inGradient.GetStackedInGradients()) {
            this._inGradients.Remove(item);
        }
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
