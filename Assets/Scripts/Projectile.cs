using UnityEngine;

public class Projectile : Particle
{
    public enum gModel { G1, G2, G5, G6, G7, G8 };

    public float ballisticCoefficient;
    public gModel bulletGModel;
    [Tooltip("Feet per second")]
    public float muzzleVelocity;
    [Tooltip("Degrees from equator")]
    public float currentLatitude;
    [Tooltip("Grains")]
    public float bulletMass;
    [Tooltip("Inches")]
    public float bulletDiameter;
    [Tooltip("Inches")]
    public float bulletLength;
    [Tooltip("Inches per twist")]
    public float barrelTwist;
    [Tooltip("Fahrenheit")]
    public float temperature;
    [Tooltip("Percent")]
    public float relativeHumidity;
    [Tooltip("In Hg")]
    public float airPressure;
    [Tooltip("m/s")]
    public Vector3 windVect;
    public float lifeTime;

    //private float m_retardation;
    //private float m_drag;
    //private Vector3 m_vectorDrag;
    //private Vector3 m_trueVelocity;
    private float startTime;
    //private float m_timeOfFlight;
    //private float m_bulletDirection;
    //private float m_distance;
    private Vector3 startPosition;
    //private Vector3 m_vectorCoriolis;
    private Vector3 previousCoriolisDeflection;
    //private Vector3 m_vectorCentripetal;
    private float stabilityFactor;
    private Vector3 previousDrift;
    //private Vector3 m_vectorSpin;
    //private const float k_msToFps = 3.2808399f;
    private const float k_fpsToMs = 0.3048f;
    //private const float k_omega = 0.000072921159f;
    //private const float k_gravity = 9.80665f;
    private const float k_dryAir = 287.058f;
    private const float k_waterVapor = 461.495f;
    private const float k_kgm3Togrin3 = 0.252891f;
    private const float k_HgToPa = 3386.3886666718315f;

    public Projectile(string _id,
                      float _ballisticCoefficient,
                      gModel _bulletGModel,
                      float _muzzleVelocity,
                      float _currentLatitude,
                      float _bulletMass,
                      float _bulletDiameter,
                      float _bulletLength,
                      float _barrelTwist,
                      float _temperature,
                      float _relativeHumidity,
                      float _airPressure,
                      Vector3 _windVect,
                      float physicsMass,
                      float _lifeTime,
                      Vector3 _position,
                      Vector3 _scale,
                      Quaternion _rotation) : base(_id, _position, _scale, _rotation, physicsMass)
    {
        ballisticCoefficient = _ballisticCoefficient;
        bulletGModel = _bulletGModel;
        muzzleVelocity = _muzzleVelocity;
        currentLatitude = _currentLatitude;
        bulletMass = _bulletMass;
        bulletDiameter = _bulletDiameter;
        bulletLength = _bulletLength;
        barrelTwist = _barrelTwist;
        temperature = _temperature;
        relativeHumidity = _relativeHumidity;
        airPressure = _airPressure;
        windVect = _windVect;
        lifeTime = _lifeTime;
    }

    public float GetStartTime()
    {
        return startTime;
    }

    public Vector3 GetStartPosition()
    {
        return startPosition;
    }

    public float GetStabilityFactor()
    {
        return stabilityFactor;
    }

    public Vector3 GetPreviousDrift()
    {
        return previousDrift;
    }

    public void SetPreviousDrift(Vector3 _previousDrift)
    {
        previousDrift = _previousDrift;
    }

    public Vector3 GetPreviousCoriolisDeflection()
    {
        return previousCoriolisDeflection;
    }

    public void SetPreviousCoriolisDeflection(Vector3 _previousCoriolisDeflection)
    {
        previousCoriolisDeflection = _previousCoriolisDeflection;
    }

    public void Enable(float dt, float _startTime)
    {
        if (!active)
        {
            ConvertUnits();
            startTime = _startTime;
            startPosition = position;
            gravity = new Vector3(0f, Physics.gravity.y * dt, 0f);
            CalculateStabilityFactor();
            active = true;
        }
    }

    public override void Integrate(float dt, float time)
    {
        if (active)
        {
            base.Integrate(dt);
            Ballistic.Integrate(this, gravity, dt, time);
            lifeTime -= dt;
            if (lifeTime <= 0f)
            {
                active = false;
            }
            /* GetSpeed(dt);
            GetTimeOfFlight(time);
            GetDistance();
            CalculateRetardation();
            CalculateDrag(dt);
            CalculateSpinDrift();
            CalculateCoriolis();
            CalculateCentripetal();
            velocity += gravity;
            velocity -= m_vectorDrag;
            velocity += m_vectorCentripetal;
            velocity += windVect * dt;
            velocity += (acceleration + (forceAccum * inverseMass)) * dt;
            velocity *= Mathf.Pow(damping, dt);
            ClearAccumulator();
            if (!float.IsNaN(m_vectorCoriolis.x) && !float.IsNaN(m_vectorCoriolis.y) && !float.IsNaN(m_vectorCoriolis.z))
            {
                position += m_vectorCoriolis;
            }
            if (!float.IsNaN(m_vectorSpin.x) && !float.IsNaN(m_vectorSpin.y) && !float.IsNaN(m_vectorSpin.z))
            {
                position += m_vectorSpin;
            }
            lifeTime -= dt;
            if (lifeTime <= 0f)
            {
                active = false;
            } */
        }
    }

    private void CalculateStabilityFactor()
    {
        float dewPoint = temperature - ((100f - relativeHumidity) / 5f);
        float exponent = 7.5f * dewPoint / (dewPoint + 237.8f);
        float pSat = 6.102f * Mathf.Pow(10, exponent);
        float pv = relativeHumidity / 100f * pSat;
        float pd = airPressure - pv;
        float temperatureKelvin = temperature;
        float pAir = k_kgm3Togrin3 * (pd / (k_dryAir * temperatureKelvin)) + (pv / (k_waterVapor * temperatureKelvin));
        float l = bulletLength / bulletDiameter;
        stabilityFactor = 8 * Mathf.PI / (pAir * Mathf.Pow(barrelTwist, 2) * Mathf.Pow(bulletDiameter, 5) * 0.57f * l) * (bulletMass * Mathf.Pow(bulletDiameter, 2) / (4.83f * (1 + Mathf.Pow(l, 2))));
    }

    private void ConvertUnits()
    {
        currentLatitude = Mathf.PI / 180 * currentLatitude;
        muzzleVelocity *= k_fpsToMs;
        temperature = (temperature - 32) * 5f / 9f;
        airPressure *= k_HgToPa;
    }
}



/* private void CalculateDrag(float dt)
    {
        m_drag = dt * m_retardation;
        m_vectorDrag = Vector3.Normalize(m_trueVelocity) * m_drag;
    }

    private void GetSpeed(float dt)
    {
        m_trueVelocity = velocity + windVect * dt;
    }

    private void GetTimeOfFlight(float time)
    {
        m_timeOfFlight = time - m_startTime;
    }

    private void GetDistance()
    {
        m_bulletDirection = Mathf.Atan2(velocity.z, velocity.x);
        m_distance = Vector3.Distance(position, m_startPosition);
    }

    private void CalculateCoriolis()
    {
        float speed = m_distance / m_timeOfFlight;
        float deflectionX = k_omega * Mathf.Pow(m_distance, 2) * Mathf.Sin(currentLatitude) / speed;
        float deflectionY = 1 - 2 * (k_omega * muzzleVelocity / k_gravity) * Mathf.Cos(currentLatitude) * Mathf.Sin(m_bulletDirection);
        float drop = m_startPosition.y - position.y;
        deflectionY = deflectionY * drop - drop;
        m_vectorCoriolis = new Vector3(deflectionX, deflectionY, 0);
        m_vectorCoriolis -= m_previousCoriolisDeflection;
        m_previousCoriolisDeflection = new Vector3(deflectionX, deflectionY, 0);
    }

    private void CalculateCentripetal()
    {
        float centripetalAcceleration = 2 * k_omega * (muzzleVelocity / k_gravity) * Mathf.Cos(currentLatitude) * Mathf.Sin(m_bulletDirection);
        centripetalAcceleration *= Time.fixedDeltaTime;
        m_vectorCentripetal = new Vector3(0, -centripetalAcceleration, 0);
    }

    private void CalculateSpinDrift()
    {
        float spinDrift = 1.25f * (m_stabilityFactor + 1.2f) * Mathf.Pow(m_timeOfFlight, 1.83f);
        m_vectorSpin = new Vector3(spinDrift, 0, 0);
        m_vectorSpin -= m_previousDrift;
        m_previousDrift = new Vector3(spinDrift, 0, 0);
    } 
    
    private void CalculateRetardation()
    {
        float velFps = velocity.magnitude * k_msToFps;
        float A = -1;
        float N = -1;

        if (bulletGModel == gModel.G1)
        {
            if (velFps > 4230) { A = 1.477404177730177e-04f; N = 1.9565f; }
            else if (velFps > 3680) { A = 1.920339268755614e-04f; N = 1.925f; }
            else if (velFps > 3450) { A = 2.894751026819746e-04f; N = 1.875f; }
            else if (velFps > 3295) { A = 4.349905111115636e-04f; N = 1.825f; }
            else if (velFps > 3130) { A = 6.520421871892662e-04f; N = 1.775f; }
            else if (velFps > 2960) { A = 9.748073694078696e-04f; N = 1.725f; }
            else if (velFps > 2830) { A = 1.453721560187286e-03f; N = 1.675f; }
            else if (velFps > 2680) { A = 2.162887202930376e-03f; N = 1.625f; }
            else if (velFps > 2460) { A = 3.209559783129881e-03f; N = 1.575f; }
            else if (velFps > 2225) { A = 3.904368218691249e-03f; N = 1.55f; }
            else if (velFps > 2015) { A = 3.222942271262336e-03f; N = 1.575f; }
            else if (velFps > 1890) { A = 2.203329542297809e-03f; N = 1.625f; }
            else if (velFps > 1810) { A = 1.511001028891904e-03f; N = 1.675f; }
            else if (velFps > 1730) { A = 8.609957592468259e-04f; N = 1.75f; }
            else if (velFps > 1595) { A = 4.086146797305117e-04f; N = 1.85f; }
            else if (velFps > 1520) { A = 1.954473210037398e-04f; N = 1.95f; }
            else if (velFps > 1420) { A = 5.431896266462351e-05f; N = 2.125f; }
            else if (velFps > 1360) { A = 8.847742581674416e-06f; N = 2.375f; }
            else if (velFps > 1315) { A = 1.456922328720298e-06f; N = 2.625f; }
            else if (velFps > 1280) { A = 2.419485191895565e-07f; N = 2.875f; }
            else if (velFps > 1220) { A = 1.657956321067612e-08f; N = 3.25f; }
            else if (velFps > 1185) { A = 4.745469537157371e-10f; N = 3.75f; }
            else if (velFps > 1150) { A = 1.379746590025088e-11f; N = 4.25f; }
            else if (velFps > 1100) { A = 4.070157961147882e-13f; N = 4.75f; }
            else if (velFps > 1060) { A = 2.938236954847331e-14f; N = 5.125f; }
            else if (velFps > 1025) { A = 1.228597370774746e-14f; N = 5.25f; }
            else if (velFps > 980) { A = 2.916938264100495e-14f; N = 5.125f; }
            else if (velFps > 945) { A = 3.855099424807451e-13f; N = 4.75f; }
            else if (velFps > 905) { A = 1.185097045689854e-11f; N = 4.25f; }
            else if (velFps > 860) { A = 3.566129470974951e-10f; N = 3.75f; }
            else if (velFps > 810) { A = 1.045513263966272e-08f; N = 3.25f; }
            else if (velFps > 780) { A = 1.291159200846216e-07f; N = 2.875f; }
            else if (velFps > 750) { A = 6.824429329105383e-07f; N = 2.625f; }
            else if (velFps > 700) { A = 3.569169672385163e-06f; N = 2.375f; }
            else if (velFps > 640) { A = 1.839015095899579e-05f; N = 2.125f; }
            else if (velFps > 600) { A = 5.71117468873424e-05f; N = 1.950f; }
            else if (velFps > 550) { A = 9.226557091973427e-05f; N = 1.875f; }
            else if (velFps > 250) { A = 9.337991957131389e-05f; N = 1.875f; }
            else if (velFps > 100) { A = 7.225247327590413e-05f; N = 1.925f; }
            else if (velFps > 65) { A = 5.792684957074546e-05f; N = 1.975f; }
            else if (velFps > 0) { A = 5.206214107320588e-05f; N = 2.000f; }
        }

        if (bulletGModel == gModel.G2)
        {
            if (velFps > 1674) { A = .0079470052136733f; N = 1.36999902851493f; }
            else if (velFps > 1172) { A = 1.00419763721974e-03f; N = 1.65392237010294f; }
            else if (velFps > 1060) { A = 7.15571228255369e-23f; N = 7.91913562392361f; }
            else if (velFps > 949) { A = 1.39589807205091e-10f; N = 3.81439537623717f; }
            else if (velFps > 670) { A = 2.34364342818625e-04f; N = 1.71869536324748f; }
            else if (velFps > 335) { A = 1.77962438921838e-04f; N = 1.76877550388679f; }
            else if (velFps > 0) { A = 5.18033561289704e-05f; N = 1.98160270524632f; }
        }

        if (bulletGModel == gModel.G5)
        {
            if (velFps > 1730) { A = 7.24854775171929e-03f; N = 1.41538574492812f; }
            else if (velFps > 1228) { A = 3.50563361516117e-05f; N = 2.13077307854948f; }
            else if (velFps > 1116) { A = 1.84029481181151e-13f; N = 4.81927320350395f; }
            else if (velFps > 1004) { A = 1.34713064017409e-22f; N = 7.8100555281422f; }
            else if (velFps > 837) { A = 1.03965974081168e-07f; N = 2.84204791809926f; }
            else if (velFps > 335) { A = 1.09301593869823e-04f; N = 1.81096361579504f; }
            else if (velFps > 0) { A = 3.51963178524273e-05f; N = 2.00477856801111f; }
        }

        if (bulletGModel == gModel.G6)
        {
            if (velFps > 3236) { A = 0.0455384883480781f; N = 1.15997674041274f; }
            else if (velFps > 2065) { A = 7.167261849653769e-02f; N = 1.10704436538885f; }
            else if (velFps > 1311) { A = 1.66676386084348e-03f; N = 1.60085100195952f; }
            else if (velFps > 1144) { A = 1.01482730119215e-07f; N = 2.9569674731838f; }
            else if (velFps > 1004) { A = 4.31542773103552e-18f; N = 6.34106317069757f; }
            else if (velFps > 670) { A = 2.04835650496866e-05f; N = 2.11688446325998f; }
            else if (velFps > 0) { A = 7.50912466084823e-05f; N = 1.92031057847052f; }
        }

        if (bulletGModel == gModel.G7)
        {
            if (velFps > 4200) { A = 1.29081656775919e-09f; N = 3.24121295355962f; }
            else if (velFps > 3000) { A = 0.0171422231434847f; N = 1.27907168025204f; }
            else if (velFps > 1470) { A = 2.33355948302505e-03f; N = 1.52693913274526f; }
            else if (velFps > 1260) { A = 7.97592111627665e-04f; N = 1.67688974440324f; }
            else if (velFps > 1110) { A = 5.71086414289273e-12f; N = 4.3212826264889f; }
            else if (velFps > 960) { A = 3.02865108244904e-17f; N = 5.99074203776707f; }
            else if (velFps > 670) { A = 7.52285155782535e-06f; N = 2.1738019851075f; }
            else if (velFps > 540) { A = 1.31766281225189e-05f; N = 2.08774690257991f; }
            else if (velFps > 0) { A = 1.34504843776525e-05f; N = 2.08702306738884f; }
        }

        if (bulletGModel == gModel.G8)
        {
            if (velFps > 3571) { A = .0112263766252305f; N = 1.33207346655961f; }
            else if (velFps > 1841) { A = .0167252613732636f; N = 1.28662041261785f; }
            else if (velFps > 1120) { A = 2.20172456619625e-03f; N = 1.55636358091189f; }
            else if (velFps > 1088) { A = 2.0538037167098e-16f; N = 5.80410776994789f; }
            else if (velFps > 976) { A = 5.92182174254121e-12f; N = 4.29275576134191f; }
            else if (velFps > 0) { A = 4.3917343795117e-05f; N = 1.99978116283334f; }
        }

        if (A != -1 && N != -1 && velFps > 0 && velFps < 100000)
        {
            m_retardation = A * Mathf.Pow(velFps, N) / ballisticCoefficient;
            m_retardation /= k_msToFps;
        }
    } */