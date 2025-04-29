using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ApplyForce : MonoBehaviour
{
    Rigidbody stoneRb;

    [Header ("<b>Components")]
    public AudioSource audioSoruce;
    public GameObject dustParticles;
    public AudioClip[] stoneFalling;
    public Transform forceOrigin; 
    public List<Collider> nearObjectList = new List<Collider>();

    [Header ("<b>Values")]
    public float forceMagnitude = 10f;
    public LayerMask stoneLayer;

    int index = 0;
    float checkRadius = 0;

    void Start()
    {
        InitComponents();
        PhysicsStatus(false);
        SetRadius();
        GetNearObjects();
    }

    private void OnMouseDown()
    {
        ApplyForceOnStone();
    }

    private void SetRadius()
    {
        // Auto-set check radius from mesh size
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            float size = meshFilter.mesh.bounds.size.magnitude;
            checkRadius = size * 2f;
        }
    }

    public void ApplyForceOnStone()
    {
        if (forceOrigin == null || stoneRb == null) return;

        // change settings of the stone
        PhysicsStatus(true);
        transform.SetParent(null);

        // change layer of the object
        //gameObject.tag = "FallingStone";

        // Direction from the point to this object
        Vector3 direction = (transform.position - forceOrigin.position).normalized;

        // ApplyForceOnStone the force
        stoneRb.AddForce(direction * forceMagnitude, ForceMode.Impulse);

        index = Random.Range(0, stoneFalling.Length);
        audioSoruce.PlayOneShot(stoneFalling[index]);

        Instantiate(dustParticles, transform.position, Quaternion.identity, transform);
    }

    private void PhysicsStatus(bool check)
    {
        if (check)
        {
            stoneRb.isKinematic = false;
            stoneRb.useGravity = true;
        }
        else
        {
            stoneRb.isKinematic = true;
            stoneRb.useGravity = false;
        }
    }

    private void InitComponents()
    {
        stoneRb = GetComponent<Rigidbody>();
    }

    bool isFalled = false;
    private void GetNearObjects()
    {
        if (isFalled) return;
        Collider[] overlaps = Physics.OverlapSphere(transform.position, checkRadius, stoneLayer);

        nearObjectList = overlaps.ToList();
    }

    public bool GetFallenSatus()
    {
        return isFalled;
    }

    bool allIntact = false;
    private void Update()
    {
        foreach (var item in nearObjectList)
        {
            allIntact = item.GetComponent<ApplyForce>().GetFallenSatus();
        }
    }
}
