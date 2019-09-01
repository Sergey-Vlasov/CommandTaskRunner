using Microsoft.VisualStudio.Shell.Interop;

namespace CommandTaskRunner.ProjectReload
{
    class InfoBarEventsHandler : IVsInfoBarUIEvents
    {
        public InfoBarEventsHandler(System.IServiceProvider serviceProvider, EnvDTE.Project project)
        {
            _serviceProvider = serviceProvider;
            _project = project;
        }

        public void OnClosed(IVsInfoBarUIElement infoBarUIElement)
        {
        }

        public void OnActionItemClicked(IVsInfoBarUIElement infoBarUIElement, IVsInfoBarActionItem actionItem)
        {
            try
            {
                ProjectService.Reload(_serviceProvider, _project);
                infoBarUIElement.Close();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private EnvDTE.Project _project;
        private System.IServiceProvider _serviceProvider;
    }
}
