using UnityEngine;

public class Particle
{
    public string id;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;
    public Vector3 acceleration;
    public float damping = 0.99f;
    [Tooltip("Grains")]
    public float inverseMass;
    public bool active = false;

    protected Vector3 gravity;
    protected Vector3 forceAccum;
    protected BoundingBox boundingBox;

    private Vector3 previousPosition;

    public Particle(string _id, Vector3 _position, Vector3 _scale, Quaternion _rotation, float mass)
    {
        SetMass(mass);
        id = _id;
        position = _position;
        rotation = _rotation;
        previousPosition = _position;
        boundingBox = new BoundingBox(_id, _position, _scale);
    }

    public double GetMass()
    {
        if (inverseMass == 0)
        {
            return float.PositiveInfinity;
        }
        else
        {
            return 1.0 / inverseMass;
        }
    }

    public void SetMass(float mass)
    {
        if (mass <= 0f)
        {
            inverseMass = 0f;
        }
        else
        {
            inverseMass = 1.0f / mass;
        }
    }

    public Vector3 PreviousPosition
    {
        get { return previousPosition; }
    }

    public Vector3 ForceAccum
    {
        get { return forceAccum; }
    }

    public void ClearAccumulator()
    {
        forceAccum = Vector3.zero;
    }

    public virtual void Integrate(float dt, float time = 0f)
    {
        if (inverseMass <= 0.0) return;
        if (dt <= 0) return;
        previousPosition = position;
        position += velocity * dt;
        if (velocity != Vector3.zero)
        {
            rotation = Quaternion.LookRotation(velocity);
        }
        if (position.y <= 0f)
        {
            active = false;
        }
        boundingBox.Integrate(position, velocity, dt);
    }
}
