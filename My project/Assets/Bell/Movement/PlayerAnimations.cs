using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    [SerializeField] float maxSpeed= 5f;

    void Start()
    {
        animator = this.GetComponentInChildren<Animator>();
        rb = this.GetComponent<Rigidbody>();
    }

    void Update()
    {
        animator.SetFloat("Speed", rb.linearVelocity.magnitude / maxSpeed);
    }
}
