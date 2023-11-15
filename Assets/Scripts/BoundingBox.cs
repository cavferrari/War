using UnityEngine;

public class BoundingBox
{
    public string id;
    public Vector3 position;
    public Vector3 scale;
    public Vector3 velocity;
    public Vector3 direction;
    public bool contacting;

    private Vector3 upperLeftFront;
    private Vector3 upperRightFront;
    private Vector3 lowerRightFront;
    private Vector3 lowerLeftFront;
    private Vector3 upperLeftBack;
    private Vector3 upperRightBack;
    private Vector3 lowerRightBack;
    private Vector3 lowerLeftBack;
    private Vector3 u;
    private Vector3 v;
    private Vector3 w;

    public BoundingBox(string _id, Vector3 _position, Vector3 _scale)
    {
        id = _id;
        position = _position;
        scale = _scale;

        upperLeftFront = new Vector3(position.x - scale.x, position.y + scale.y, position.z + scale.z);
        upperRightFront = new Vector3(position.x + scale.x, position.y + scale.y, position.z + scale.z);
        lowerRightFront = new Vector3(position.x + scale.x, position.y - scale.y, position.z + scale.z);
        lowerLeftFront = new Vector3(position.x - scale.x, position.y - scale.y, position.z + scale.z);

        upperLeftBack = new Vector3(position.x - scale.x, position.y + scale.y, position.z - scale.z);
        upperRightBack = new Vector3(position.x + scale.x, position.y + scale.y, position.z - scale.z);
        lowerRightBack = new Vector3(position.x + scale.x, position.y - scale.y, position.z - scale.z);
        lowerLeftBack = new Vector3(position.x - scale.x, position.y - scale.y, position.z - scale.z);

        contacting = false;

        CollisionDetection.Instance.Add(this);
    }

    public void Integrate(Vector3 _position, Vector3 _velocity, float _time)
    {
        direction = (_position - position).normalized;
        position = _position;
        velocity = _velocity;

        upperLeftFront.Set(position.x - scale.x, position.y + scale.y, position.z + scale.z);
        upperRightFront.Set(position.x + scale.x, position.y + scale.y, position.z + scale.z);
        lowerRightFront.Set(position.x + scale.x, position.y - scale.y, position.z + scale.z);
        lowerLeftFront.Set(position.x - scale.x, position.y - scale.y, position.z + scale.z);

        upperLeftBack.Set(position.x - scale.x, position.y + scale.y, position.z - scale.z);
        upperRightBack.Set(position.x + scale.x, position.y + scale.y, position.z - scale.z);
        lowerRightBack.Set(position.x + scale.x, position.y - scale.y, position.z - scale.z);
        lowerLeftBack.Set(position.x - scale.x, position.y - scale.y, position.z - scale.z);

        Draw(_time);
    }

    public bool IsInside(Vector3 point)
    {
        u = lowerLeftBack - upperLeftBack;
        v = lowerLeftBack - lowerRightBack;
        w = lowerLeftBack - lowerLeftFront;
        return Vector3.Dot(u, point) >= Vector3.Dot(u, upperLeftBack) && Vector3.Dot(u, point) <= Vector3.Dot(u, lowerLeftBack) &&
               Vector3.Dot(v, point) >= Vector3.Dot(v, lowerRightBack) && Vector3.Dot(v, point) <= Vector3.Dot(v, lowerLeftBack) &&
               Vector3.Dot(w, point) >= Vector3.Dot(w, lowerLeftFront) && Vector3.Dot(w, point) <= Vector3.Dot(w, lowerLeftBack);
    }

    private void Draw(float _time)
    {
        //front
        Debug.DrawLine(upperLeftFront, upperRightFront, Color.white, _time);
        Debug.DrawLine(upperRightFront, lowerRightFront, Color.white, _time);
        Debug.DrawLine(lowerRightFront, lowerLeftFront, Color.white, _time);
        Debug.DrawLine(lowerLeftFront, upperLeftFront, Color.white, _time);

        //left
        Debug.DrawLine(upperLeftBack, upperLeftFront, Color.blue, _time);
        Debug.DrawLine(upperLeftFront, lowerLeftFront, Color.blue, _time);
        Debug.DrawLine(lowerLeftFront, lowerLeftBack, Color.blue, _time);
        Debug.DrawLine(lowerLeftBack, upperLeftBack, Color.blue, _time);

        //right
        Debug.DrawLine(upperRightBack, upperRightFront, Color.yellow, _time);
        Debug.DrawLine(upperRightFront, lowerRightFront, Color.yellow, _time);
        Debug.DrawLine(lowerRightFront, lowerRightBack, Color.yellow, _time);
        Debug.DrawLine(lowerRightBack, upperRightBack, Color.yellow, _time);

        //back
        Debug.DrawLine(upperLeftBack, upperRightBack, Color.green, _time);
        Debug.DrawLine(upperRightBack, lowerRightBack, Color.green, _time);
        Debug.DrawLine(lowerRightBack, lowerLeftBack, Color.green, _time);
        Debug.DrawLine(lowerLeftBack, upperLeftBack, Color.green, _time);
    }
}
