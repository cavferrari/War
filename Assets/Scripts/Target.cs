using UnityEngine;

public class Target : MonoBehaviour
{
    private BoundingBox boundingBox;
    // Start is called before the first frame update
    void Start()
    {
        boundingBox = new BoundingBox(gameObject.name, transform.position, transform.localScale);
    }

    // Update is called once per frame
    void Update()
    {
        boundingBox.Integrate(transform.position, Vector3.zero, Time.deltaTime);
    }
}
