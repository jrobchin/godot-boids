using System;

public struct Limits
{
    public int XMin;
    public int XMax;
    public int YMin;
    public int YMax;

    public Limits(int XMin, int XMax, int YMin, int YMax)
    {
        this.XMin = XMin;
        this.XMax = XMax;
        this.YMin = YMin;
        this.YMax = YMax;
    }
}
