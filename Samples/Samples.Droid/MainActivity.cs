﻿using System;
using Acr.UserDialogs;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


namespace Samples.Droid
{
    [Activity(
        Label = "ACR BluetoothLE",
        Icon = "@drawable/icon",
        Theme = "@style/MainTheme",
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation
    )]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                AppCenter.Start("c787ca2412e270b908c52038422266ed", typeof(Analytics), typeof(Crashes));

                AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

                base.OnCreate(bundle);
                Forms.Init(this, bundle);
                FormsAppCompatActivity.ToolbarResource = Resource.Layout.Toolbar;
                FormsAppCompatActivity.TabLayoutResource = Resource.Layout.Tabbar;

                UserDialogs.Init(() => (Activity)Forms.Context);

                this.LoadApplication(new App(new PlatformInitializer()));
                this.RequestPermissions(new[]
                {
                Manifest.Permission.AccessCoarseLocation,
                Manifest.Permission.BluetoothPrivileged
                }, 0);
            }
            catch(Exception e)
            {
                Crashes.TrackError(e);
            }

        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var newExc = new Exception("CurrentDomainOnUnhandledException", unhandledExceptionEventArgs.ExceptionObject as Exception);
            Crashes.TrackError(newExc);
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
        }
    }
}

