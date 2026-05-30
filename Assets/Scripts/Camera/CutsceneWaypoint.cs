using System;
using UnityEngine;

namespace Camera {
    [Serializable]
    public struct CutsceneWaypoint {
        public Transform point;
        [Tooltip("Check this if reaching this waypoint should fade to the next image.")]
        public bool triggerNextImage; 
    }
}