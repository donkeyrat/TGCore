using Landfall.TABS;
using UnityEngine;

namespace TGCore.Library;

public class DontStopForTurns : MonoBehaviour
 {
     private void Start()
     {
         transform.root.GetComponentInChildren<MovementHandler>().stopForTurns = false;
     }
 }