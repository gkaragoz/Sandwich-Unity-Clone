using System.Collections;
using UnityEngine;

public class HistoryTracker : MonoBehaviour {

    public static Stack inGradients = new Stack();

    public static bool hasUndoFinished = true;

    public void Undo() {
        if (hasUndoFinished) {
            hasUndoFinished = false;
            (inGradients.Pop() as InGradient).Undo();
        }
    }

}
