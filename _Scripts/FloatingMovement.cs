using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FloatingMovement : MonoBehaviour
{
    //Rigidbody2D rb;
    public float coefficient = .3f;
    public float maxVertical = 1;

    void Awake()
    {
        //rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y + GetOffset(),
            transform.position.z
            );
    }

    float GetOffset() {
        return Mathf.Sin(coefficient * Mathf.Sin(Time.time));
    }
}
