using UnityEngine;

public class StackController : MonoBehaviour {

    public LayerMask layerMask;

    [SerializeField]
    private GameObject _testInGradient;

    private void Awake() {
        SwipeHandler.onSwiped += OnSwiped;
    }

    private void OnSwiped(SwipeHandler.Direction direction, GameObject swipedObject) {
        if (direction == SwipeHandler.Direction.None) {
            return;
        }

        InGradient neighbourInGradient = HasNeighbour(direction);
        if (neighbourInGradient == null) {
            return;
        } else {
            int stackedAmount = 0;
            swipedObject.GetComponent<InGradient>().Move(direction, stackedAmount);
        }
    }

    private InGradient HasNeighbour(SwipeHandler.Direction direction) {
        RaycastHit hitInfo;
        Ray upRay = new Ray(_testInGradient.transform.position, Vector3.forward);
        Ray rightRay = new Ray(_testInGradient.transform.position, Vector3.right);
        Ray downRay = new Ray(_testInGradient.transform.position, Vector3.back);
        Ray leftRay = new Ray(_testInGradient.transform.position, Vector3.left);

        switch (direction) {
            case SwipeHandler.Direction.Up:
                if (Physics.Raycast(upRay, out hitInfo, _testInGradient.transform.localScale.z, layerMask)) {
                    return hitInfo.transform.gameObject.GetComponent<InGradient>();
                }
                break;
            case SwipeHandler.Direction.Right:
                if (Physics.Raycast(rightRay, out hitInfo, _testInGradient.transform.localScale.x, layerMask)) {
                    return hitInfo.transform.gameObject.GetComponent<InGradient>();
                }
                break;
            case SwipeHandler.Direction.Down:
                if (Physics.Raycast(downRay, out hitInfo, _testInGradient.transform.localScale.z, layerMask)) {
                    return hitInfo.transform.gameObject.GetComponent<InGradient>();
                }
                break;
            case SwipeHandler.Direction.Left:
                if (Physics.Raycast(leftRay, out hitInfo, _testInGradient.transform.localScale.x, layerMask)) {
                    return hitInfo.transform.gameObject.GetComponent<InGradient>();
                }
                break;
        }

        return null;
    }

}
