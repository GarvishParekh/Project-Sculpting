using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ApplyForce : MonoBehaviour
{
    MeshCollider boxCollider;
    Rigidbody stoneRb;

    [Header ("<b>Components")]
    public AudioSource audioSoruce;
    public GameObject dustParticles;
    public AudioClip[] stoneFalling;
    public Transform forceOrigin; // The point to push away from

    [Header ("<b>Values")]
    public float forceMagnitude = 10f;

    int index = 0;


    void Start()
    {
        stoneRb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<MeshCollider>();
        stoneRb.isKinematic = true;
        stoneRb.useGravity = false;
    }

    private void OnMouseDown()
    {
        stoneRb.isKinematic = false;
        stoneRb.useGravity = true;
        boxCollider.isTrigger = true;
        transform.SetParent(null);
        Apply();
    }

    public void Apply()
    {
        if (forceOrigin == null || stoneRb == null)
            return;

        // Direction from the point to this object
        Vector3 direction = (transform.position - forceOrigin.position).normalized;

        // Apply the force
        stoneRb.AddForce(direction * forceMagnitude, ForceMode.Impulse);

        index = Random.Range(0, stoneFalling.Length);
        audioSoruce.PlayOneShot(stoneFalling[index]);

        Instantiate(dustParticles, transform.position, Quaternion.identity, transform);
    }
}
