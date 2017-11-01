using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class CoordPair
{
    Point unity, leftPupil, rightPupil;

    public CoordPair(Point unity, Point leftPupil, Point rightPupil)
    {
        this.unity = unity;
        this.leftPupil = leftPupil;
        this.rightPupil = rightPupil;
    }

}

class Point
{
    public int x, y, z;
    public Point(int x, int y ,int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}
