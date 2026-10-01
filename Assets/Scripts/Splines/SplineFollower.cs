using UnityEngine;

public class SplineFollower : MonoBehaviour
{
    [SerializeField] Spline splineToFollow;
    [SerializeField] float speed = 25f;

    float _t;

    void Update()
    {
        if (!splineToFollow) return;
        _t += Time.deltaTime * speed;

        transform.position = splineToFollow.EvaluateSplineGroup(_t);
    }
}
