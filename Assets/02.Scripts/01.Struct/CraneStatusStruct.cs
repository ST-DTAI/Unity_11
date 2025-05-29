public struct CrStatus
{
    public int CrNo;         // ���� ����
    public string Status;    // Status
    public int Locus;       // Locus
    public float GoalDx;         // Dx
    public float GoalDy;         // Dy
    public float GoalDz;         // Dz
    public string Addr;     // Laddr (Addr�� ����)
    public string PdNo;      // PdNo
    public float Dx;         // Dx
    public float Dy;         // Dy
    public float Dz;         // Dz

    public int SwivAng;      // SwivAng
    public int ArmWid;       // ArmWid
    public int LdWeight;     // LdWeight
    public int TLSway;       // ���ο� �ʵ�
    public int TSSway;       // ���ο� �ʵ�
    public int Temp;         // Temp
    public int ErrCode;      // ErrCode
    public int Input;        // ���ο� �ʵ�
    public int Output;       // ���ο� �ʵ�
    public int ComChk;       // ComChk
    public int CycleTime;    // ���ο� �ʵ�

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
        Addr = addr; // Laddr �ʵ�
        PdNo = pdNo;
        Dx = dx;
        Dy = dy;
        Dz = dz;
        SwivAng = swivAng;
        ArmWid = armWid;
        LdWeight = ldWeight;
        TLSway = tlsway; // ���ο� �ʵ�
        TSSway = tssway; // ���ο� �ʵ�
        Temp = temp;
        ErrCode = errCode;
        Input = input;   // ���ο� �ʵ�
        Output = output;  // ���ο� �ʵ�
        ComChk = comChk;
        CycleTime = cycleTime; // ���ο� �ʵ�
    }
}