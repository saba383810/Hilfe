using Fusion;
using UnityEngine;

public class BulletContainerData : NetworkBehaviour
{
    [Networked, Capacity(100)] public NetworkArray<Vector2> BulletList => default;
}
