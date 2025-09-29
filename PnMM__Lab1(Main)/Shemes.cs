
using System;

public class DirectSheme : BaseSolver
{
    public DirectSheme(float alpha,
                       float final_pos,
                       float total_time,
                       float percent,
                       float dx)
        : base(alpha, final_pos, total_time, percent, dx)
    { }

    public override float[] SolveStep()
    {
        if (dt > (dx * dx) / (2 * alpha))
        {
            throw new InvalidOperationException("Схема неустойчива!");
        }

        float[] newTemperature = new float[nPoints];
        float k = alpha * dt / (dx * dx);

        for (int i = 1; i < nPoints - 1; i++)
        {
            newTemperature[i] = temperature[i] +
                k * (temperature[i + 1] - 2 * temperature[i] + temperature[i - 1]);
        }

        newTemperature[0] = temperature[0];
        newTemperature[nPoints - 1] = temperature[nPoints - 1];

        temperature = newTemperature;
        currentTime += dt;
        stepCount++;

        return temperature;
    }
}


public class ReversedSheme : BaseSolver
{
    public ReversedSheme(float alpha,
                       float final_pos,
                       float total_time,
                       float percent,
                       float dx)
        : base(alpha, final_pos, total_time, percent, dx)
    { }

    public override float[] SolveStep()
    {
        float[] newTemperature = new float[nPoints];
        float k = alpha * dt / (dx * dx);

        float[] a = new float[nPoints]; // 
        float[] b = new float[nPoints]; //   
        float[] c = new float[nPoints]; // 
        float[] d = new float[nPoints]; // 

        // 
        for (int i = 1; i < nPoints - 1; i++)
        {
            a[i] = -k;
            b[i] = 1 + 2 * k;
            c[i] = -k;
            d[i] = temperature[i];
        }

        // 
        b[0] = 1;
        c[0] = 0;
        d[0] = temperature[0];

        a[nPoints - 1] = 0;
        b[nPoints - 1] = 1;
        d[nPoints - 1] = temperature[nPoints - 1];

        // 
        newTemperature = ThomasAlgorithm(a, b, c, d, nPoints);

        temperature = newTemperature;
        currentTime += dt;
        stepCount++;
        return temperature;
    }

    private float[] ThomasAlgorithm(float[] a, float[] b, float[] c, float[] d, int n)
    {
        float[] x = new float[n];

        // 
        for (int i = 1; i < n; i++)
        {
            float m = a[i] / b[i - 1];
            b[i] = b[i] - m * c[i - 1];
            d[i] = d[i] - m * d[i - 1];
        }

        // 
        x[n - 1] = d[n - 1] / b[n - 1];
        for (int i = n - 2; i >= 0; i--)
        {
            x[i] = (d[i] - c[i] * x[i + 1]) / b[i];
        }

        return x;
    }
}