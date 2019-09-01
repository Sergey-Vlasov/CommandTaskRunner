using Microsoft.VisualStudio.TaskRunnerExplorer;
using System.Threading.Tasks;

namespace CommandTaskRunner
{
    class TaskRunnerNodeContinuation : TaskRunnerNode
    {
        public TaskRunnerNodeContinuation(string name, System.Action<Task<ITaskRunnerCommandResult>> continueWith) : base(name, true)
        {
            _continueWith = continueWith;
        }

        public override Task<ITaskRunnerCommandResult> Invoke(ITaskRunnerCommandContext context)
        {
            Task<ITaskRunnerCommandResult> result = base.Invoke(context);
            result.ContinueWith(_continueWith);
            return result;
        }

        private System.Action<Task<ITaskRunnerCommandResult>> _continueWith;
    }
}
