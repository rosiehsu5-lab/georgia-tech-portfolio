using System.Collections;
using UnityEngine;

public class BreakableIce : MonoBehaviour
{
    public float breakDelay = 1.0f;
    public AudioClip breakSound;
    public GameObject fracturedIcePrefab;

    private bool isTriggered = false;
    private Vector3 originalstate;

    private void Start()
    {
        originalstate = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.name.Contains("Pengu") && !isTriggered)
        {
            isTriggered = true;
            StartCoroutine(BreakTrigger());
        }
    }

    private IEnumerator BreakTrigger()
    {
        float time = 0f;
        Vector3 originalstate = transform.position;

        Collider iceCollider = GetComponent<Collider>();
        Bounds bounds = iceCollider.bounds;

        while (time < breakDelay)
        {
            transform.position = originalstate + (Random.insideUnitSphere * 0.05f);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = originalstate;

        if (breakSound != null)
        {
            AudioSource.PlayClipAtPoint(breakSound, transform.position);
        }

        if (fracturedIcePrefab != null)
        {
            int numberOficepices = 15;

            for (int i = 0; i < numberOficepices; i++)
            {
                float randomX = Random.Range(bounds.min.x, bounds.max.x);
                float randomY = Random.Range(bounds.min.y, bounds.max.y);
                float randomZ = Random.Range(bounds.min.z, bounds.max.z);

                Vector3 randomstate = new Vector3(randomX, randomY, randomZ);

                GameObject icepice = Instantiate(fracturedIcePrefab, randomstate, Random.rotation);

                Rigidbody rb = icepice.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);
                }
            }
        }

        Destroy(gameObject);
    }
}