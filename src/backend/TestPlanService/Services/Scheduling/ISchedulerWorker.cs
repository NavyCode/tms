using System.Threading.Tasks;

namespace TestPlanService.Services.Scheduling
{
    public interface ISchedulerWorker
    {
        Task Start();
    }

}
