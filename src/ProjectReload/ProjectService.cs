using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace CommandTaskRunner.ProjectReload
{
    class ProjectService
    {
        public static void Reload(System.IServiceProvider serviceProvider, EnvDTE.Project project)
        {
            System.Guid projectGuid = GetProjectGuid(serviceProvider, project);
            IVsSolution4 solution = serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution4;

            ErrorHandler.ThrowOnFailure(solution.UnloadProject(ref projectGuid, (uint)_VSProjectUnloadStatus.UNLOADSTATUS_UnloadedByUser));
            ErrorHandler.ThrowOnFailure(solution.ReloadProject(ref projectGuid));
        }

        private static System.Guid GetProjectGuid(System.IServiceProvider serviceProvider, EnvDTE.Project project)
        {
            IVsSolution sol = (IVsSolution)serviceProvider.GetService(typeof(SVsSolution));

            Guid ignored = Guid.Empty;
            IEnumHierarchies hierEnum;
            ErrorHandler.ThrowOnFailure(sol.GetProjectEnum((int)__VSENUMPROJFLAGS.EPF_ALLPROJECTS, ref ignored, out hierEnum));

            IVsHierarchy[] hier = new IVsHierarchy[1];
            uint fetched;
            while ((hierEnum.Next((uint)hier.Length, hier, out fetched) == VSConstants.S_OK) && (fetched == hier.Length))
            {
                string uniqueName;
                if (ErrorHandler.Succeeded(sol.GetUniqueNameOfProject(hier[0], out uniqueName)))
                {
                    if (uniqueName == project.UniqueName)
                    {
                        Guid projGuid;
                        if (ErrorHandler.Succeeded(sol.GetGuidOfProject(hier[0], out projGuid)))
                            return projGuid;
                    }
                }
            }
            throw new System.ApplicationException("Can't find project guid.");
        }
    }
}
