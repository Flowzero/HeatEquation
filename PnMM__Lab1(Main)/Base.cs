
using System;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;


public abstract class BaseSolver
{
    protected readonly float alpha;          // коэффицент теплопроводности
    protected readonly float init_pos = 0;   // начальная позиция
    protected readonly float final_pos;      // конечная позиция (L)
    protected readonly float total_time;     // общее время моделирования            

    protected float dx, dt;
    protected float percent;                 // процент от максимально устойчивого dt_max
    protected int nPoints;                   // кол-во точек по пространству
    protected int nTimeSteps;                // кол-во шагов по времени

    protected float[] temperature;           // слой (совокупность узлов в фиксированный момент времени)
    protected float currentTime;
    protected int stepCount = 0;

    public BaseSolver(float alpha,
                      float final_pos,
                      float total_time,
                      float percent,
                      float dx)
    {
        this.alpha = alpha;
        this.final_pos = final_pos;
        this.total_time = total_time;
        this.percent = percent;
        this.dx = dx;

        __init__grid();
        __init__temperature();
    }

    
    protected virtual void __init__grid()
    {
        nPoints = (int)Math.Ceiling(final_pos / dx);
                                                              // For DirectSheme:              //     ^                                 ^                                  ^   
        float dt_max = (dx * dx) / (2 * alpha);               //        dx^2                   // 100 |         ╭●╮                     |                                  |
        dt = dt_max * percent;                                // dt <= ------                  //     |         │ │                     |                                  |
        nTimeSteps = (int)Math.Ceiling(total_time / dt) + 1;  //         2a                    //     |         │ │                     |                                  |
                                                              //                               //     |         │ │                     |                                  |
        // dt всегда вычисляеься с учетом соблюдения          // The less dt we take, the      //     |         │ │                     |                                  |
        // условия устойчивости. Если alpha будет нефизично   // better the sheme works.       //     |         │ │                  50 |   ╭●╮         ╭●╮                |         ╭●╮
        // большим, то dt будет черезвычайно малым            //                               //     |         │ │                     |   │ │         │ │                |         │ │
                                                              // percent - percentage from     //     |         │ │                     |   │ │         │ │             25 |   ╭●╮   │ │   ╭●╮
        // Также программа может крашнутся при следующих      // max_dt to get smaller dt      //     |         │ │                     |   │ │         │ │                |   │ │   │ │   │ │
        // параметрах: (0.0002f, 2f, 1f, 0.25f, 1f)           //                               //     |         │ │                     |   │ │         │ │                |   │ │   │ │   │ │
        // Это объясняется тем, что для построения графика    //                               //     |         │ │                     |   │ │         │ │                |   │ │   │ │   │ │
        // нужно как минимум две точки, а не одна:            // DO NOT TAKE dt = dt_max!      //     |         │ │                     |   │ │         │ │                |   │ │   │ │   │ │
        // nPoints = (int)Math.Ceiling(1f / 1f) = 1           // Otherwise the result will     //     +------------------------------>  +------------------------------>   +------------------------------>
        //                                                    // be like that or it will lead  //               0.4                         0.2   0.4   0.6                    0.2   0.4   0.6
        // (0.0002f, 2f, 1f, 0.25f, 1f) -> 0.4 дошло до 20 C° //                               //     step = 0                          step = 1                           step = 2
        // за 4 сек                                           //                               //
        // (0.02f, 2f, 1f, 0.25f, 1f) -> 0.4 дошло до 20 C°   // for demo: dx = 0.02, dt = 0.5 // 
        // за 0.040 сек                                       // dt_max = 0.5, a = 1           //

        Console.WriteLine($"alpha = {alpha}\ndx = {dx}\ndt = {dt}\nTotal Time = {total_time}\nnPoints = {nPoints}\nnTimePoints = {nTimeSteps}");
    }


    protected virtual void __init__temperature()
    {
        temperature = new float[nPoints];
        double heatSource_pos = 0.4;
        double heatSource_width = dx; 

        for (int i = 0; i < nPoints; i++)
        {
            float x = i * dx;
            if (x >= heatSource_pos - heatSource_width / 2 && x <= heatSource_pos + heatSource_width / 2)
            {
                temperature[i] = 100.0f;
                //Console.WriteLine($"Initial: temperature[{i}] = {temperature[i]}, x = {i * dx}");
            }
            else
            {
                temperature[i] = 0.0f;
            }
        }
    }

    public abstract float[] SolveStep();

    public virtual void Reset()
    {
        currentTime = 0;
        stepCount = 0;
        __init__temperature();
    }

    public float[] Temperature => (float[])temperature.Clone();
    public float CurrentTime => currentTime;
    public float TotalTime => total_time;
    public float Dx => dx;
    public float Dt => dt;
    public int NumberOfPoints => nPoints;
    public int StepCount => stepCount;
    public float FinalPos => final_pos;
    public float Percent => percent;
    public float Alpha => alpha;
}