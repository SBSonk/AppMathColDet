using Helpers;
using UnityEngine;

public class SplineFollower : MonoBehaviour
{
    [SerializeField] Spline splineToFollow;
    [SerializeField] float speed = 25f;
    [SerializeField] Ease.EaseType easeType = Ease.EaseType.Linear;

    float _t;

    void Update()
    {
        if (!splineToFollow) return;
        _t += Time.deltaTime * speed;

        transform.position = splineToFollow.EvaluateSplineGroup(Ease.EvaluateEaseType(easeType, _t));
    }

    public void SetSpline(Spline s)
    {
        splineToFollow = s;
        _t = 0;
    }

    public void SetSpeed(float s) => speed = s;

    public void SetEase(Ease.EaseType easeType) => this.easeType = easeType;
}
