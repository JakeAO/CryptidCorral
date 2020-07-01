namespace Core.Code.StateMachine.CombatEngine
{
    public enum EWeightClass
    {
        E = 0, // < 100 stats
        D = 1, // 100-300 stats
        C = 2, // 301-600 stats
        B = 3, // 601-1200 stats
        A = 4, // 1201-1800 stats
        S = 5, // > 1800 stats

        _Count
    }
}