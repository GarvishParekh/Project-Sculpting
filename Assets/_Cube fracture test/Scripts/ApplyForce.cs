using System.Linq;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class ApplyForce : MonoBehaviour
{
    Rigidbody stoneRb;

    [Header ("<b>Components")]
    public AudioSource audioSoruce;
    public GameObject dustParticles;
    public AudioClip[] stoneFalling;
    public Transform forceOrigin; 
    public List<ApplyForce> nearObjectList = new List<ApplyForce>();

    [Header ("<b>Values")]
    public float forceMagnitude = 10f;
    public LayerMask stoneLayer;

    int index = 0;
    float checkRadius = 0;
    bool alreadyFallen = false;

    void Start()
    {
        InitializeComponents();
        PhysicsStatus(false);
        SetRadius();
        GetNearObjects();
    }

    private void InitializeComponents()
    {
        stoneRb = GetComponent<Rigidbody>();
    }

    // ------------------------------ listners ------------------------------
    private void OnEnable()
    {
        ActionHandler.RockPartFall += OnRockFalled;
    }

    private void OnDisable()
    {
        ActionHandler.RockPartFall -= OnRockFalled;
    }

    public void OnRockFalled(ApplyForce fallenRock)
    {
        if (alreadyFallen) return;

        if (nearObjectList.Contains(fallenRock))
        {
            nearObjectList.Remove(fallenRock);
        }
        if (nearObjectList.Count == 1) ApplyForceOnStone();
    }

    // ------------------------------ inputs ------------------------------
    private void OnMouseDown() => ApplyForceOnStone();


    // ------------------------------ calculations ------------------------------
    private void SetRadius()
    {
        // Auto-set check radius from mesh size
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            float size = meshFilter.mesh.bounds.size.magnitude;
            checkRadius = size * 5f;
        }
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



    private void GetNearObjects()
    {
        Renderer rend = GetComponent<Renderer>();
        Bounds bounds = rend.bounds;

        Vector3 center = bounds.center;
        Vector3 halfExtents = bounds.extents * 0.5f; // This is bounds.size / 2

        Collider[] boxOverlaps = Physics.OverlapBox(center, halfExtents, Quaternion.identity, stoneLayer);
        //Collider[] overlaps = Physics.OverlapSphere(transform.position, checkRadius, stoneLayer);

        foreach (var item in boxOverlaps)
        {
            ApplyForce applyForce = item.GetComponent<ApplyForce>();
            nearObjectList.Add(applyForce);
        }

    }

    // ------------------------------ main logic ------------------------------
    public void ApplyForceOnStone()
    {
        if (forceOrigin == null || stoneRb == null) return;
        alreadyFallen = true;

        // change settings of the stone
        PhysicsStatus(true);
        transform.SetParent(null);


        // get direction and apply force
        Vector3 direction = (transform.position - forceOrigin.position).normalized;
        stoneRb.AddForce(direction * forceMagnitude, ForceMode.Impulse);

        // get random audio and play the sound
        index = Random.Range(0, stoneFalling.Length);
        audioSoruce.PlayOneShot(stoneFalling[index]);

        // instentiate smoke particle and call the action
        Instantiate(dustParticles, transform.position, Quaternion.identity, transform);
        ActionHandler.RockPartFall?.Invoke(this);
    }
}
