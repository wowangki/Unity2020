using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rigid;
    private AudioSource audioSource;

    [SerializeField] private int damage = 0;
    [SerializeField] private float speed = 0;

    public int Range { get; set; }
    public Vector3 FirePos { get; set; }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnDisable()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        rigid.velocity = Vector3.zero;
        FirePos = Vector3.zero;
        StopAllCoroutines();
    }

    private void Update()
    {
        Debug.DrawLine(FirePos, transform.position, Color.green);

        if(Vector3.Distance(FirePos, transform.position) > Range)
        {
            gameObject.SetActive(false);
        }
    }

    public void Fire(Vector3 target)
    {
        Vector3 fireDir = (target - transform.position).normalized;

        rigid.velocity = fireDir * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            StartCoroutine(UnActiveCoroutine());
        }
    }

    private IEnumerator UnActiveCoroutine()
    {
        audioSource.Play();

        yield return new WaitWhile(() => audioSource.isPlaying);

        gameObject.SetActive(false);
    }
}
