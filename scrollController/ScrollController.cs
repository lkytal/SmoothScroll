using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace ScrollShared
{
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
  public class ScrollController
  {
    private const long      Interval            = 33;
    private const long      MinInterval         = 5;
    private const int       AccelerateThreshold = 2;
    private const double    Accelerator         = 2.0;
    private readonly double dpiRatio;

    private readonly object locker = new object();
    private readonly IPageScroller pageScroller;
    private readonly ScrollingDirection direction;

    private readonly Dictionary<ScrollingSpeeds, double> speedTable = new Dictionary<ScrollingSpeeds, double>()
    {
      {ScrollingSpeeds.Slow, 1.5},
      {ScrollingSpeeds.Normal, 1.0},
      {ScrollingSpeeds.Fast, 0.65}
    };

    private double totalDistance;
    private double remain;
    private int    totalRounds;
    private int    round;

    private AutoResetEvent   stopScrollEvent       = new AutoResetEvent(false);
    private AutoResetEvent   startScrollEvent      = new AutoResetEvent(false);
    private ManualResetEvent exitScrollThreadEvent = new ManualResetEvent(false);

    private Thread workingThread;

    public ScrollController(IPageScroller _pageScroller, ScrollingDirection _direction)
    {
      pageScroller  = _pageScroller;
      direction     = _direction;
      dpiRatio      = SystemParameters.PrimaryScreenHeight / 720.0;
      workingThread = new Thread(new ThreadStart(ScrollingThread));
      workingThread.Start();
    }

    ~ScrollController()
    {
      exitScrollThreadEvent.Set();
      workingThread.Join();
    }

    private int CalculateTotalRounds(double timeRatio, double requestDistance)
    {
      double distanceRatio = Math.Sqrt(Math.Abs(requestDistance / dpiRatio) / 720);
      double maxTotalSteps = 10 + 25 * Math.Min(distanceRatio, 1.5);

      return (int)(maxTotalSteps * timeRatio);
    }

    public void ScrollView(double distance, ScrollingSpeeds speedLever)
    {
      double intervalRatio = GetSpeedRatio(speedLever);

      lock (locker)
      {
        if (Math.Sign(distance) != Math.Sign(remain))
        {
          remain = (int)distance;
        }
        else
        {
          remain += (int)(distance * (round < AccelerateThreshold ? 1 : Accelerator));
        }

        round = 0;
        totalDistance = remain;
      }

      totalRounds = CalculateTotalRounds(intervalRatio, totalDistance);

      startScrollEvent.Set();
    }

    private double GetSpeedRatio(ScrollingSpeeds speedLever)
    {
      return speedTable[speedLever];
    }

    private int CalculateScrollDistance()
    {
      var percent    = (double)round / totalRounds;
      var stepLength = 2 * totalDistance / totalRounds * (1 - percent);
      return (int)Math.Round(stepLength);
    }

    public void StopScroll()
    {
      stopScrollEvent.Set();
    }

    private void CleanupScroll()
    {
      lock (locker)
      {
        round = totalRounds;
        totalDistance = remain = 0;
      }
    }

    private void ScrollingThread()
    {
      var  sleepWaitHandles    = new WaitHandle[] { startScrollEvent, exitScrollThreadEvent };
      var  exitLoopWaitHandles = new WaitHandle[] { stopScrollEvent, exitScrollThreadEvent };
      var  stopWatch           = new Stopwatch();
      long remainingDelay;

      // The first loop, sleep/run loop.
      for (;;)
      {
        WaitHandle.WaitAny(sleepWaitHandles);
        if (exitScrollThreadEvent.WaitOne(0))
        {
          return;
        }

        // The second loop, scrolling loop.
        while (round <= totalRounds && WaitHandle.WaitAny(exitLoopWaitHandles, 0) == WaitHandle.WaitTimeout)
        {
          if (exitScrollThreadEvent.WaitOne(0))
          {
            return;
          }

          var stepLength = CalculateScrollDistance();

          if (stepLength == 0)
          {
            break;
          }

          lock (locker)
          {
            ++round;
            remain -= stepLength;
          }

          stopWatch.Restart();

          pageScroller.Scroll(direction, stepLength);

          stopWatch.Stop();
          remainingDelay = Math.Max(Interval - stopWatch.ElapsedMilliseconds, MinInterval);

          // Delay.
          WaitHandle.WaitAny(exitLoopWaitHandles, (int)remainingDelay);
        }

        CleanupScroll();
      }
    }
  }
}
