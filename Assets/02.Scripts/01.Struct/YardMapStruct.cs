public struct YardMap
{
    //1
    public int SkidNo;
    public int Dong;
    public string Skid;
    public int Sect;
    public int DxNo;
    public int DyNo;
    public int DzNo;
    public string Addr;
    //9
    public float Dx;
    public float Dy;
    public float Dz;
    public float Dir;
    public int MaxWid;
    public int MaxDia;
    public string PdYN;
    public string Hold;
    public int CrRev;
    public int SupRev;
    public int OutRev;
    public string FwdYN;
    public string BwdYN;
    //22
    public string PdNo;
    public string State;
    public int Width;
    public int Outdia;
    public int India;
    public float Thick;
    public int Weight;
    public int Temp;
    public string Date;
    public string PdNew;

    public YardMap(int skidNo, int dong, string skid, int sect, int dxNo, int dyNo, int dzNo,
                   string addr,  float dx, float dy, float dz, float dir, int maxWid,
                   int maxDia, string pdYN, string hold, int crRev, int supRev, int outRev,
                   string fwdYN, string bwdYN, string pdNo, string state,
                   int width, int outdia, int india, float thick, int weight, int temp, string date, string pdNew)
    {
        this.SkidNo = skidNo;
        this.Dong = dong;
        this.Skid = skid;
        this.Sect = sect;
        this.DxNo = dxNo;
        this.DyNo = dyNo;
        this.DzNo = dzNo;
        this.Addr = addr;
        
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

        this.PdNo = pdNo;
        this.State = state;
        this.Width = width;
        this.Outdia = outdia;
        this.India = india;
        this.Thick = thick;
        this.Weight = weight;
        this.Temp = temp;
        this.Date = date;
        this.PdNew = pdNew;
    }
}
