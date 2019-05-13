
using System;

public class Vector2
{

    public float x;
    public float y;

    public Vector2(float x, float y)
    {

        this.x = x;
        this.y = y;

    }

    public bool Equals(Vector2 v)
    {

        if (Math.Round(x, 2) == Math.Round(v.x, 2) && Math.Round(y, 2) == Math.Round(v.y, 2))
        {

            return true;

        }
        else return false;

    }

}
