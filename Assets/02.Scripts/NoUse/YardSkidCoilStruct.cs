public struct YardSkidCoil
{
    public int SkidNo;
    public string Dong;
    public string Skid;
    public string Sect;
    public int DxNo;
    public int DyNo;
    public int DzNo;
    public string Addr;
    public string PdNo;
    public float Dx;
    public float Dy;
    public float Dz;
    public float Dir;
    public int MaxWid;
    public int MaxDia;
    public string PdYN;
    public string Hold;
    public int CrRev;
    public string SupRev;
    public string OutRev;
    public string FwdYN;
    public string BwdYN;
    public string State;
    public float Width;
    public float Outdia;
    public float India;
    public float Thick;
    public float Weight;
    public float Temp;
    public string Date;
    public string ToNo;

    public YardSkidCoil(int skidNo, string dong, string skid, string sect, int dxNo, int dyNo, int dzNo,
                   string addr, string pdNo, float dx, float dy, float dz, float dir, int maxWid,
                   int maxDia, string pdYN, string hold, int crRev, string supRev, string outRev,
                   string fwdYN, string bwdYN,
                   string state, float width, float outdia, float india, float thick, float weight, float temp, string date, string toNo )
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
        this.State = state;
        this.Width = width;
        this.Outdia = outdia;
        this.India = india;
        this.Thick = thick;
        this.Weight = weight;
        this.Temp = temp;
        this.Date = date;
        this.ToNo = toNo;
            

    }
}
