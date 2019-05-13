using System;
using System.Timers;

public class Scheduler
{

    public Timer t;
    public Action f;

    public Scheduler(Action f)
    {

        t = new Timer();
        t.Elapsed += OnTimedEvent;
        this.f = f;

    }

    public Scheduler RunLater(double d) {

        t.Interval = d * 1000; 
        t.AutoReset = false;
        t.Enabled = true;

        return this;

    }

    public Scheduler RunTimer(double d)
    {

        t.Interval = d * 1000;
        t.AutoReset = true;
        t.Enabled = true;

        return this;

    }

    public void Cancel() {

        t.Stop();
        t.Dispose();

    }

    private void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
        f();
    }

}
