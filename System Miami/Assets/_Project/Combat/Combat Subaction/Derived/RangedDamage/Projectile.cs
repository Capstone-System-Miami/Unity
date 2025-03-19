using UnityEngine;
using System.Collections;
using System;

public class Projectile : MonoBehaviour
{
    private Vector3 _targetPosition;
    private float _speed;
    private Action _onHit; // Callback function to execute when projectile arrives

    public void Launch(Vector3 targetPosition, float speed, Action onHit)
    {
        _targetPosition = targetPosition;
        _speed = speed;
        _onHit = onHit;
        StartCoroutine(MoveToTarget());
    }

    private IEnumerator MoveToTarget()
    {
        while (Vector3.Distance(transform.position, _targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _speed * Time.deltaTime);
            yield return null;
        }

        // Apply damage when projectile reaches the target
        _onHit?.Invoke();

        
        Destroy(gameObject);
    }
}
