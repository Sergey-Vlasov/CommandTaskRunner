using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace CommandTaskRunner.ProjectReload
{
    class InfoBar
    {
        public static async void ShowAsync(Microsoft.VisualStudio.Shell.SVsServiceProvider serviceProvider, EnvDTE.Project project)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            try
            {
                Show(serviceProvider, project);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private static void Show(Microsoft.VisualStudio.Shell.SVsServiceProvider serviceProvider, EnvDTE.Project project)
        {
            var shell = serviceProvider.GetService(typeof(SVsShell)) as IVsShell;
            if (shell != null)
            {
                // Get the main window handle to host our InfoBar
                shell.GetProperty((int)__VSSPROPID7.VSSPROPID_MainWindowInfoBarHost, out var obj);
                var host = (IVsInfoBarHost)obj;

                //If we cannot find the handle, we cannot do much, so return.
                if (host == null)
                {
                    return;
                }

                InfoBarModel infoBarModel = CreateInfoBarModel(project);

                //Get the factory object from IVsInfoBarUIFactory, create it and add it to host.
                var factory = serviceProvider.GetService(typeof(SVsInfoBarUIFactory)) as IVsInfoBarUIFactory;
                IVsInfoBarUIElement element = factory.CreateInfoBar(infoBarModel);

                var infoBarEventsHandler = new InfoBarEventsHandler(serviceProvider, project);

                element.Advise(infoBarEventsHandler, out var _cookie);
                host.AddInfoBar(element);
            }
        }

        private static InfoBarModel CreateInfoBarModel(EnvDTE.Project project)
        {
            var model = new InfoBarModel
            (
                textSpans: new[]
                {
                    new InfoBarTextSpan("Please "),
                    new InfoBarHyperlink("reload"),
                    new InfoBarTextSpan(" the project " + project.Name + ".")
                },
                actionItems: new[]
                {
                    new InfoBarButton("Reload")
                },
                image: KnownMonikers.StatusInformation,
                isCloseButtonVisible: true
            );

            return model;
        }
    }
}
