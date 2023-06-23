using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestPlanService
{
    public enum PlanState
    {
        Planed = 0,
        Active = 1,
        Closed = 2
    }

    public enum AutomationStatus
    {
        Manual = 0,
        Automated = 1
    }

    public enum WiState
    {
        Design = 0,
        Closed = 1,
        Ready = 2
    }

    public enum WiType
    {
        TestCase = 0,
    }
}