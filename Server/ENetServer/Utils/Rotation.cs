
using System;

public class Rotation
{

    public float z;
    public float w;

    public Rotation(float z, float w)
    {

        this.z = z;
        this.w = w;

    }

    public bool Equals(Rotation r)
    {

        if (Math.Round(z, 2) == Math.Round(r.z, 2) && Math.Round(w, 2) == Math.Round(r.w, 2)) {

            return true;

        } else return false;

    }

}