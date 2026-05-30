using System;
using UnityEngine;

namespace Camera {
    [Serializable]
    public struct CutsceneWaypoint {
        public Transform point;
        public float zoom;
    }
}