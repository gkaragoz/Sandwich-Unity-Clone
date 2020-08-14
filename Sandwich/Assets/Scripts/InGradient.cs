using UnityEngine;

public class InGradient : MonoBehaviour {

    [Header("Initiailizations")]
    public LeanTweenType easeType;
    public float speed = 0.4f;

    [SerializeField]
    private GameObject _rotatePivot = null;

    [Header("DEBUG")]
    [SerializeField]
    private bool _isFlippedHorizontal = false;
    [SerializeField]
    private bool _isFlippedVertical = false;

    private Vector3 _upJoint;
    private Vector3 _rightJoint;
    private Vector3 _downJoint;
    private Vector3 _leftJoint;

    private float _inGradientHeight = 0.25f;

    private void Awake() {
        _inGradientHeight = this.transform.localScale.y * 0.5f;

        CalculateJoints();
    }

    public void Move(SwipeHandler.Direction direction, int stackedAmount) {
        if (direction == SwipeHandler.Direction.None || IsAnyTweenActive()) {
            return;
        }

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
        return _inGradientHeight * stackedAmount;
    }

    private bool IsAnyTweenActive() {
        return LeanTween.isTweening(_rotatePivot);
    }

    private void LeanAnimation(Vector3 joint, Vector3 axis, float angle, bool isVertical, int stackedAmount) {
        _rotatePivot.transform.position = joint + (Vector3.up * GetTargetHeight(stackedAmount));
        _rotatePivot.transform.eulerAngles = Vector3.zero;

        transform.SetParent(_rotatePivot.transform);

        LeanTween.rotateAroundLocal(_rotatePivot, axis, angle, speed)
            .setEase(easeType)
            .setOnComplete(() => {
                transform.SetParent(null);

                if (isVertical) {
                    _isFlippedVertical = !_isFlippedVertical;
                } else {
                    _isFlippedHorizontal = !_isFlippedHorizontal;
                }

                CalculateJoints();
            });
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
