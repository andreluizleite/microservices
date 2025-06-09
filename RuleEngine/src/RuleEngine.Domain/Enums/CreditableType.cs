namespace RuleEngine.Domain.Enums
{
    public enum CreditableType
    {
        StandaloneGroundActivity = 1 << 0,
        DutyPeriod = 1 << 1,
        GroundActivity = 1 << 2,
        Leg = 1 << 3,
        Pairing = 1 << 4,
        CreditableFootprint = 1 << 5,
    }
}
