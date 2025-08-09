using UnityEngine;

public class ParallaxBackgroundPart : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 2.0f;
    private float _width;
    private float _distanceTracker = 0.0f;

    private void Start()
    {
        _width = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        float distanceToAdd = _movementSpeed * Time.deltaTime;
        _distanceTracker += distanceToAdd;
        transform.position += new Vector3(distanceToAdd, 0.0f, 0.0f);
        if(Mathf.Abs(_distanceTracker) > _width)
        {
            _distanceTracker = 0.0f;
            Vector3 vectorToSubtract = (_movementSpeed > 0.0f) ? new Vector3(_width, 0.0f, 0.0f) : new Vector3(-_width, 0.0f, 0.0f);
            transform.position -= vectorToSubtract;
        }
    }
}
