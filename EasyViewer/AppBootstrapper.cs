namespace EasyViewer {
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;
    using Caliburn.Micro;
    using Helpers;
    using LibVLCSharp.Shared;
    using Settings.FilmEditorFolder.ViewModels;
    using ViewModels;

    public class AppBootstrapper : BootstrapperBase {
        SimpleContainer container;

        public AppBootstrapper() {
            Initialize();
            Core.Initialize();

            var defaultApplyAvailabilityEffect = ActionMessage.ApplyAvailabilityEffect;

            ActionMessage.ApplyAvailabilityEffect  = context =>
            {
                if (context.Source is ListBox)
                    return true;

                return defaultApplyAvailabilityEffect(context);
            };
        }

        protected override void Configure() {
            container = new SimpleContainer();

            container.Singleton<IWindowManager, WindowManager>();
            container.Singleton<IEventAggregator, EventAggregator>();
            container.PerRequest<MainViewModel>();
            container.PerRequest<AddressEditingViewModel>();
            container.PerRequest<EditorSelectorViewModel>();
            container.PerRequest<EpisodeEditingViewModel>();
            container.PerRequest<EpisodesEditorViewModel>();
            container.PerRequest<FilmsEditingViewModel>();
            container.PerRequest<SeasonsEditingViewModel>();
        }

        protected override object GetInstance(Type service, string key) {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service) {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance) {
            container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e) {
            DisplayRootViewFor<MainViewModel>();
        }
    }
}