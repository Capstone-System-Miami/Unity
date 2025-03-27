namespace SystemMiami
{
    public interface ITransparency
    {
        bool IsTransparent { get; }
        void SetTransparent(float opacityPercent);
        void SetOpaque();
    }
}
