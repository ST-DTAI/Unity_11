public struct yard_map
{
    public int SkidNo;
    public int Dong;
    public string Skid;
    public int Sect;
    public int DxNo;
    public int DyNo;
    public int DzNo;
    public int Addr;
    public string PdNo;
    public float Dx;
    public float Dy;
    public float Dz;
    public float Dir;
    public int MaxWid;
    public int MaxDia;
    public int PdYN;
    public int Hold;
    public int CrRev;
    public int SupRev;
    public int OutRev;
    public int FwdYN;
    public int BwdYN;

    public yard_map(int skidNo, int dong, string skid, int sect, int dxNo, int dyNo, int dzNo,
                   int addr, string pdNo, float dx, float dy, float dz, float dir, int maxWid,
                   int maxDia, int pdYN, int hold, int crRev, int supRev, int outRev,
                   int fwdYN, int bwdYN)
    {
        this.SkidNo = skidNo;
        this.Dong = dong;
        this.Skid = skid;
        this.Sect = sect;
        this.DxNo = dxNo;
        this.DyNo = dyNo;
        this.DzNo = dzNo;
        this.Addr = addr;
        this.PdNo = pdNo;
        this.Dx = dx;
        this.Dy = dy;
        this.Dz = dz;
        this.Dir = dir;
        this.MaxWid = maxWid;
        this.MaxDia = maxDia;
        this.PdYN = pdYN;
        this.Hold = hold; 
        this.CrRev = crRev; 
        this.SupRev = supRev;
        this.OutRev = outRev;
        this.FwdYN = fwdYN; 
        this.BwdYN = bwdYN;
    }
}
