namespace LifeManagement.ObsoleteEnums
{
    public static class ComplexityExtensions
    {
        public static string ToLocalizedString(this Complexity complexity)
        {
            switch (complexity)
            {
                case Complexity.High:
                    return Resources.ResourceScr.High;
                case Complexity.Medium:
                    return Resources.ResourceScr.Medium;
                case Complexity.VeryHigh:
                    return Resources.ResourceScr.VeryHigh;
                default:
                    return Resources.ResourceScr.Low;
            }
        }
    }
}