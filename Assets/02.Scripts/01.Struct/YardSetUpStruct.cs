public struct YardSetUp
{
    public int Dong;
    public float DxOffset;
    public float DxMax;
    public float DyOffset;
    public float DyMax;
    public float Height;
    public float DxSpacing;

    public YardSetUp(int dong, float dxOffset, float dxMax, float dyOffset, float dyMax, float height, float dxSpacing)
    {
        Dong = dong;
        DxOffset = dxOffset;
        DxMax = dxMax;
        DyOffset = dyOffset;
        DyMax = dyMax;
        Height = height;
        DxSpacing = dxSpacing;
    }
}
