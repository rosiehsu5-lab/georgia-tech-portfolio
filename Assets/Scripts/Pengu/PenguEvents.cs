using UnityEngine;
using UnityEngine.Events;

// All events are parameterized with Vector3 for 3D position
public class SnowFootstepEvent : UnityEvent<Vector3> { }
public class JumpEvent : UnityEvent<Vector3> { }
public class SlideEvent : UnityEvent<Vector3> { }
public class InteractEvent : UnityEvent<Vector3> { }
public class DeathEvent : UnityEvent<Vector3> { }
public class RespawnEvent : UnityEvent<Vector3> { }
public class StopSlideEvent : UnityEvent<Vector3> { }