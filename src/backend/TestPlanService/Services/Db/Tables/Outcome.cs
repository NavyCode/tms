using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestPlanService
{
    public enum Outcome
    {
        Unknow = 0,
        Planed = 1,
        Passed = 2,
        Failed = 3,
        Blocked = 4,
        Skipped = 5,
        Paused = 6,
    }
    public enum RunState
    {
        NotStarted = 0,
        InProgress = 1,
        Waiting = 2,
        Completed = 3,
    }

}