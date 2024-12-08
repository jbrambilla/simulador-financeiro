namespace simulador_financeiro;

public static class NumbersExtensions
{
    public static decimal Round2(this decimal value)
    {
        return Math.Round(value, 2);
    }
    
    public static double Round2(this double value)
    {
        return Math.Round(value, 2);
    }
}