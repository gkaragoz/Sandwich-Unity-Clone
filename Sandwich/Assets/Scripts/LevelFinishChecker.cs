using TMPro;
using UnityEngine;

public class LevelFinishChecker : MonoBehaviour {

    [SerializeField]
    private TextMeshProUGUI _txtIsWin;

    [SerializeField]
    private InGradient _bread01;
    [SerializeField]
    private InGradient _bread02;

    public int totalInGradientsCount;

    private void Awake() {
        totalInGradientsCount = GameObject.FindObjectsOfType<InGradient>().Length;
    }

    private void Update() {
        if (_bread01.MovementCount == 0 && _bread02.MovementCount == 1) {
            if (_bread01.GetStackedInGradients().Count == totalInGradientsCount) {
                _txtIsWin.text = "Is_Win: " + true;
                return;
            }
        }
        if (_bread01.MovementCount == 1 && _bread02.MovementCount == 0) {
            if (_bread02.GetStackedInGradients().Count == totalInGradientsCount) {
                _txtIsWin.text = "Is_Win: " + true;
                return;
            }
        }
        _txtIsWin.text = "Is_Win: " + false;
    }

}
