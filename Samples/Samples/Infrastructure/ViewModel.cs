﻿using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Prism.AppModel;
using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;


namespace Samples.Infrastructure
{
    public abstract class ViewModel : ReactiveObject,
                                      INavigatingAware,
                                      INavigatedAware,
                                      IDestructible,
                                      IPageLifecycleAware,
                                      IConfirmNavigationAsync
    {
        CompositeDisposable deactivateWith;
        protected CompositeDisposable DeactivateWith
        {
            get
            {
                if (this.deactivateWith == null)
                    this.deactivateWith = new CompositeDisposable();

                return this.deactivateWith;
            }
        }
        public string AppVersion
        {
            get
            {
                return Xamarin.Forms.DependencyService.Get<IAppVersion>().GetVersion() + " " +
                    Xamarin.Forms.DependencyService.Get<IAppVersion>().GetBuild();
            }
        }
        protected CompositeDisposable DestroyWith { get; } = new CompositeDisposable();


        public virtual void OnNavigatingTo(INavigationParameters parameters)
        {
        }


        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
        }


        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
        }


        public virtual void OnAppearing()
        {
        }


        public virtual void OnDisappearing()
        {
            this.deactivateWith?.Dispose();
            this.deactivateWith = null;
        }


        public virtual void Destroy()
        {
            this.DestroyWith?.Dispose();
        }


        public virtual Task<bool> CanNavigateAsync(INavigationParameters parameters) => Task.FromResult(true);
        [Reactive] public bool IsBusy { get; protected set; }
    }
}
