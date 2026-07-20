using Godot;
using System;
public class CubicBezier
{
    private readonly double x1, y1, x2, y2;

    public CubicBezier(double x1, double y1, double x2, double y2)
    {
        this.x1 = x1;
        this.y1 = y1;
        this.x2 = x2;
        this.y2 = y2;
    }

    private static double Evaluate(double t, double p1, double p2)
    {
        double u = 1 - t;
        return 3 * u * u * t * p1 + 3 * u * t * t * p2 + t * t * t;
    }

    private static double Derivative(double t, double p1, double p2)
    {
        double u = 1 - t;
        return 3 * u * u * p1 + 6 * u * t * (p2 - p1) + 3 * t * t * (1 - p2);
    }

    private double SolveForT(double x)
    {
        double t = x; 
        for (int i = 0; i < 8; i++)
        {
            double currentX = Evaluate(t, x1, x2) - x;
            double currentSlope = Derivative(t, x1, x2);

            if (Math.Abs(currentSlope) < 1e-6) break;

            t -= currentX / currentSlope;
        }

        return Math.Clamp(t, 0, 1);
    }

    // Método principal: dado un tiempo (0 a 1), devuelve el progreso (0 a 1)
    public double Solve(double time)
    {
        if (time <= 0) return 0;
        if (time >= 1) return 1;

        double t = SolveForT(time);
        return Evaluate(t, y1, y2);
    }
}