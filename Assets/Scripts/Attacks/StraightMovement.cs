using UnityEngine;

public class StraightMovement : MonoBehaviour
{
    private Rigidbody _rb;
    public int speed = 20;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _rb.velocity = transform.forward * speed;
    }
}