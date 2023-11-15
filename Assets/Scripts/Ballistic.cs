using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Ballistic
{
    private static Vector3 trueVelocity;
    private static float timeOfFlight;
    private static float bulletDirection;
    private static float distance;
    private static float retardation;
    private static Vector3 vectorDrag;
    private static Vector3 vectorSpin;
    private static Vector3 previousDrift;
    private static Vector3 vectorCoriolis;
    private static Vector3 previousCoriolisDeflection;
    private static Vector3 vectorCentripetal;
    private static Vector3 aux = Vector3.zero;
    private const float k_msToFps = 3.2808399f;
    private const float k_omega = 0.000072921159f;
    private const float k_gravity = 9.80665f;

    public static void Integrate(Projectile projectile, Vector3 gravity, float dt, float time)
    {
        trueVelocity = CalculateTrueVelocity(projectile, dt);
        timeOfFlight = GetTimeOfFlight(projectile, time);
        bulletDirection = GetBulletDirection(projectile);
        distance = GetDistance(projectile);
        retardation = CalculateRetardation(projectile);
        vectorDrag = CalculateDrag(trueVelocity, retardation, dt);
        vectorSpin = CalculateSpinDrift(projectile, timeOfFlight);
        vectorCoriolis = CalculateCoriolis(projectile, distance, timeOfFlight, bulletDirection);
        vectorCentripetal = CalculateCentripetal(projectile, bulletDirection, dt);
        projectile.velocity += gravity;
        projectile.velocity -= vectorDrag;
        projectile.velocity += vectorCentripetal;
        projectile.velocity += projectile.windVect * dt;
        projectile.velocity += (projectile.acceleration + (projectile.ForceAccum * projectile.inverseMass)) * dt;
        projectile.velocity *= Mathf.Pow(projectile.damping, dt);
        projectile.ClearAccumulator();
        if (!float.IsNaN(vectorCoriolis.x) && !float.IsNaN(vectorCoriolis.y) && !float.IsNaN(vectorCoriolis.z))
        {
            projectile.position += vectorCoriolis;
        }
        if (!float.IsNaN(vectorSpin.x) && !float.IsNaN(vectorSpin.y) && !float.IsNaN(vectorSpin.z))
        {
            projectile.position += vectorSpin;
        }
    }

    private static Vector3 CalculateTrueVelocity(Projectile projectile, float dt)
    {
        return projectile.velocity + projectile.windVect * dt;
    }

    private static float GetTimeOfFlight(Projectile projectile, float time)
    {
        return time - projectile.GetStartTime();
    }

    private static float GetBulletDirection(Projectile projectile)
    {
        return Mathf.Atan2(projectile.velocity.z, projectile.velocity.x);
    }

    private static float GetDistance(Projectile projectile)
    {
        return Vector3.Distance(projectile.position, projectile.GetStartPosition());
    }

    private static float CalculateRetardation(Projectile projectile)
    {
        float velFps = projectile.velocity.magnitude * k_msToFps;
        float A = -1;
        float N = -1;

        if (projectile.bulletGModel == Projectile.gModel.G1)
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

        if (projectile.bulletGModel == Projectile.gModel.G2)
        {
            if (velFps > 1674) { A = .0079470052136733f; N = 1.36999902851493f; }
            else if (velFps > 1172) { A = 1.00419763721974e-03f; N = 1.65392237010294f; }
            else if (velFps > 1060) { A = 7.15571228255369e-23f; N = 7.91913562392361f; }
            else if (velFps > 949) { A = 1.39589807205091e-10f; N = 3.81439537623717f; }
            else if (velFps > 670) { A = 2.34364342818625e-04f; N = 1.71869536324748f; }
            else if (velFps > 335) { A = 1.77962438921838e-04f; N = 1.76877550388679f; }
            else if (velFps > 0) { A = 5.18033561289704e-05f; N = 1.98160270524632f; }
        }

        if (projectile.bulletGModel == Projectile.gModel.G5)
        {
            if (velFps > 1730) { A = 7.24854775171929e-03f; N = 1.41538574492812f; }
            else if (velFps > 1228) { A = 3.50563361516117e-05f; N = 2.13077307854948f; }
            else if (velFps > 1116) { A = 1.84029481181151e-13f; N = 4.81927320350395f; }
            else if (velFps > 1004) { A = 1.34713064017409e-22f; N = 7.8100555281422f; }
            else if (velFps > 837) { A = 1.03965974081168e-07f; N = 2.84204791809926f; }
            else if (velFps > 335) { A = 1.09301593869823e-04f; N = 1.81096361579504f; }
            else if (velFps > 0) { A = 3.51963178524273e-05f; N = 2.00477856801111f; }
        }

        if (projectile.bulletGModel == Projectile.gModel.G6)
        {
            if (velFps > 3236) { A = 0.0455384883480781f; N = 1.15997674041274f; }
            else if (velFps > 2065) { A = 7.167261849653769e-02f; N = 1.10704436538885f; }
            else if (velFps > 1311) { A = 1.66676386084348e-03f; N = 1.60085100195952f; }
            else if (velFps > 1144) { A = 1.01482730119215e-07f; N = 2.9569674731838f; }
            else if (velFps > 1004) { A = 4.31542773103552e-18f; N = 6.34106317069757f; }
            else if (velFps > 670) { A = 2.04835650496866e-05f; N = 2.11688446325998f; }
            else if (velFps > 0) { A = 7.50912466084823e-05f; N = 1.92031057847052f; }
        }

        if (projectile.bulletGModel == Projectile.gModel.G7)
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

        if (projectile.bulletGModel == Projectile.gModel.G8)
        {
            if (velFps > 3571) { A = .0112263766252305f; N = 1.33207346655961f; }
            else if (velFps > 1841) { A = .0167252613732636f; N = 1.28662041261785f; }
            else if (velFps > 1120) { A = 2.20172456619625e-03f; N = 1.55636358091189f; }
            else if (velFps > 1088) { A = 2.0538037167098e-16f; N = 5.80410776994789f; }
            else if (velFps > 976) { A = 5.92182174254121e-12f; N = 4.29275576134191f; }
            else if (velFps > 0) { A = 4.3917343795117e-05f; N = 1.99978116283334f; }
        }

        float _retardation = 0f;
        if (A != -1 && N != -1 && velFps > 0 && velFps < 100000)
        {
            _retardation = A * Mathf.Pow(velFps, N) / projectile.ballisticCoefficient;
            _retardation /= k_msToFps;
        }
        return _retardation;
    }

    private static Vector3 CalculateDrag(Vector3 _trueVelocity, float _retardation, float dt)
    {
        float drag = dt * _retardation;
        return Vector3.Normalize(_trueVelocity) * drag;
    }

    private static Vector3 CalculateSpinDrift(Projectile projectile, float _timeOfFlight)
    {
        float spinDrift = 1.25f * (projectile.GetStabilityFactor() + 1.2f) * Mathf.Pow(_timeOfFlight, 1.83f);
        aux.Set(spinDrift, 0f, 0f);
        previousDrift = projectile.GetPreviousDrift();
        projectile.SetPreviousDrift(aux);
        aux -= previousDrift;
        return aux;
    }

    private static Vector3 CalculateCoriolis(Projectile projectile, float _distance, float _timeOfFlight, float _bulletDirection)
    {
        float speed = _distance / _timeOfFlight;
        float deflectionX = k_omega * Mathf.Pow(_distance, 2f) * Mathf.Sin(projectile.currentLatitude) / speed;
        float deflectionY = 1f - 2f * (k_omega * projectile.muzzleVelocity / k_gravity) * Mathf.Cos(projectile.currentLatitude) * Mathf.Sin(_bulletDirection);
        float drop = projectile.GetStartPosition().y - projectile.position.y;
        deflectionY = deflectionY * drop - drop;
        aux.Set(deflectionX, deflectionY, 0f);
        previousCoriolisDeflection = projectile.GetPreviousCoriolisDeflection();
        projectile.SetPreviousCoriolisDeflection(aux);
        aux -= previousCoriolisDeflection;
        return aux;
    }

    private static Vector3 CalculateCentripetal(Projectile projectile, float _bulletDirection, float dt)
    {
        float centripetalAcceleration = 2 * k_omega * (projectile.muzzleVelocity / k_gravity) * Mathf.Cos(projectile.currentLatitude) * Mathf.Sin(_bulletDirection);
        centripetalAcceleration *= dt;
        aux.Set(0f, -centripetalAcceleration, 0f);
        return aux;
    }
}
