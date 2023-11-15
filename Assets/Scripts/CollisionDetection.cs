using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public static CollisionDetection Instance;

    public float minDistance = 5f;
    public int capacity = 1000;

    private List<BoundingBox> boxes;
    private Vector3 point;
    private bool contact;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        boxes = new List<BoundingBox>(capacity);
    }

    void FixedUpdate()
    {
        for (int i = 0; i < boxes.Count; i++)
        {
            for (int j = 0; j < boxes.Count; j++)
            {
                if (Evaluate(boxes[i], boxes[j]))
                {
                    point = boxes[i].position;
                    if (IsBehind(boxes[i].position, boxes[i].direction, boxes[j].position))
                    {
                        if (boxes[i].contacting)
                        {
                            boxes[i].contacting = false;
                            Debug.Log(boxes[i].id + " is no longer contacting " + boxes[j].id + ".");
                        }
                    }
                    else
                    {
                        contact = false;
                        for (int k = 0; k < 10; k++)
                        {
                            Integrate(boxes[i].velocity, Time.fixedDeltaTime / 10f);
                            //Debug.DrawLine(point, point + new Vector3(0f, 0f, 0.1f), Color.white, 1000);
                            if (boxes[j].IsInside(point))
                            {
                                boxes[i].contacting = true;
                                contact = true;
                                Debug.Log(boxes[i].id + " is contacting " + boxes[j].id + ".");
                                break;
                            }
                        }
                        if (!contact && boxes[i].contacting)
                        {
                            boxes[i].contacting = false;
                            Debug.Log(boxes[i].id + " is no longer contacting " + boxes[j].id + ".");
                        }
                    }
                }
            }
        }
    }

    public void Add(BoundingBox box)
    {
        if (capacity > 0)
        {
            boxes.Add(box);
            capacity -= 1;
        }
        else
        {
            Debug.Log("Maximum collision capacity reached.");
        }
    }

    private bool Evaluate(BoundingBox a, BoundingBox b)
    {
        if (a.id.Equals(b.id))
        {
            return false;
        }
        if (Vector3.Distance(a.position, b.position) > minDistance)
        {
            return false;
        }
        if (a.velocity == Vector3.zero)
        {
            return false;
        }
        return true;
    }

    private bool IsBehind(Vector3 position, Vector3 direction, Vector3 boxPosition)
    {
        float dotProduct = Vector3.Dot(direction, boxPosition - position);
        if (dotProduct >= 0f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void Integrate(Vector3 velocity, float dt)
    {
        point += velocity * dt;
    }
}
