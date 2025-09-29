
using System;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;


public abstract class BaseSolver
{
    protected readonly float alpha;          // coefficient of thermal conductivity
    protected readonly float init_pos = 0;   // start (initial) position
    protected readonly float final_pos;      // end position (L)
    protected readonly float total_time;     // total simulation time            

    protected float dx, dt;
    protected float percent;                 // percentage of maximum stable dt_max. 0 < percent < 1
                                             // See the comments below

    protected int nPoints;                   // number of points in the space
    protected int nTimeSteps;                // number of time steps

    protected float[] temperature;           // layer (a set of nodes at a fixed point in time)
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
                                                              // For DirectSheme:          //     ^                                 ^                                  ^   
        float dt_max = (dx * dx) / (2 * alpha);               //          dx^2             // 100 |         ╭●╮                     |                                  |
        dt = dt_max * percent;                                // dt <= ----------          //     |         │ │                     |                                  |
        nTimeSteps = (int)Math.Ceiling(total_time / dt) + 1;  //         2*alpha           //     |         │ │                     |                                  |
                                                              //                           //     |         │ │                     |                                  |
        // dt is always calculated due to stability condition // The less dt we take, the  //     |         │ │                     |                                  |
        // If alpha is extremely large (which is not possible // better the sheme works.   //     |         │ │                  50 |   ╭●╮         ╭●╮                |         ╭●╮
        // in the real world, but it is an ideal simulation), //                   dx^2    //     |         │ │                     |   │ │         │ │                |         │ │
        // then dt will be extremely small and vice versa     // if you take dt = ------   //     |         │ │                     |   │ │         │ │             25 |   ╭●╮   │ │   ╭●╮
        //                                                    //                    2a     //     |         │ │                     |   │ │         │ │                |   │ │   │ │   │ │
        //                                                    // the following happens     //     |         │ │                     |   │ │         │ │                |   │ │   │ │   │ │
        //                                                    //                           //     |         │ │                     |   │ │         │ │                |   │ │   │ │   │ │
        //                                                    //                           //     |         │ │                     |   │ │         │ │                |   │ │   │ │   │ │
        //                                                    //                           //     +------------------------------>  +------------------------------>   +------------------------------>
        // To take smaller dt:                                //                           //               0.4                         0.2   0.4   0.6                    0.2   0.4   0.6
        // dt = dt_max * percent→0                            //                           //                             step = 0                          step = 1                           step = 2


        Console.WriteLine($"alpha = {alpha}\ndx = {dx}\ndt = {dt}\nTotal Time = {total_time}\nnPoints = {nPoints}\nnTimePoints = {nTimeSteps}");
    }

    protected virtual void __init__temperature()
    {
        temperature = new float[nPoints];
        double source_pos = 0.4;
        double source_width = dx;

        for (int i = 0; i < nPoints; i++)
        {
            double x = i * dx;
            double leftBound = source_pos - source_width / 2;
            double rightBound = source_pos + source_width / 2;

            if (x >= leftBound - 0.0001 && x <= rightBound + 0.0001)
            {
                temperature[i] = 100.0f;
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