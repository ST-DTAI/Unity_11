public struct CrStatus
{
    public int CrNo;         // 변경 없음
    public string Status;    // Status
    public int Locus;       // Locus
    public float GoalDx;         // Dx
    public float GoalDy;         // Dy
    public float GoalDz;         // Dz
    public string Addr;     // Laddr (Addr로 변경)
    public string PdNo;      // PdNo
    public float Dx;         // Dx
    public float Dy;         // Dy
    public float Dz;         // Dz

    public int SwivAng;      // SwivAng
    public int ArmWid;       // ArmWid
    public int LdWeight;     // LdWeight
    public int TLSway;       // 새로운 필드
    public int TSSway;       // 새로운 필드
    public int Temp;         // Temp
    public int ErrCode;      // ErrCode
    public int Input;        // 새로운 필드
    public int Output;       // 새로운 필드
    public int ComChk;       // ComChk
    public int CycleTime;    // 새로운 필드

    public CrStatus(int crNo, string status, int locus, float goaldx, float goaldy, float goaldz, string addr, string pdNo,
                    float dx, float dy, float dz, int swivAng, int armWid,
                    int ldWeight, int tlsway, int tssway, int temp, int errCode,
                    int input, int output, int comChk, int cycleTime)
    {
        CrNo = crNo;
        Status = status;
        Locus = locus;
        GoalDx = goaldx;
        GoalDy = goaldy;
        GoalDz = goaldz;
        Addr = addr; // Laddr 필드
        PdNo = pdNo;
        Dx = dx;
        Dy = dy;
        Dz = dz;
        SwivAng = swivAng;
        ArmWid = armWid;
        LdWeight = ldWeight;
        TLSway = tlsway; // 새로운 필드
        TSSway = tssway; // 새로운 필드
        Temp = temp;
        ErrCode = errCode;
        Input = input;   // 새로운 필드
        Output = output;  // 새로운 필드
        ComChk = comChk;
        CycleTime = cycleTime; // 새로운 필드
    }
}