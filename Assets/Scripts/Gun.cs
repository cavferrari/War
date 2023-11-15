using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform muzzle;

    private Projectile bullet;

    void Start()
    {
        bullet = new Projectile("bullet_01",
                                0.286f,
                                Projectile.gModel.G1,
                                3150f,
                                40f,
                                40f,
                                0.223f,
                                0.5f,
                                12f,
                                59f,
                                0f,
                                29.92f,
                                Vector3.zero,
                                1f,
                                60f,
                                muzzle.position,
                                new Vector3(0.223f / 2f, 0.223f / 2f, 0.5f),
                                muzzle.rotation);
        bullet.Enable(Time.fixedDeltaTime, Time.time);
        Fire();
        /* for (int i = 0; i < 100; i++)
        {
            bullet.Integrate(0.1f, 0.1f * i);
            Debug.DrawLine(bullet.PreviousPosition, bullet.position, Color.red, 1000f);
        } */
    }

    void Update()
    {
        Debug.DrawLine(bullet.PreviousPosition, bullet.position, Color.red, 1000f);
    }

    void FixedUpdate()
    {
        bullet.Integrate(Time.fixedDeltaTime, Time.time);
    }

    public void Fire()
    {
        bullet.velocity = bullet.muzzleVelocity * 0.3048f * muzzle.forward;
    }
}
