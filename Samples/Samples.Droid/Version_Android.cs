using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Samples.Infrastructure;

[assembly: Xamarin.Forms.Dependency(typeof(Samples.Droid.Version_Android))]
namespace Samples.Droid
{
    public class Version_Android : IAppVersion
    {
        public string GetVersion()
        {
            var context = global::Android.App.Application.Context;

            PackageManager manager = context.PackageManager;
            PackageInfo info = manager.GetPackageInfo(context.PackageName, 0);

            return info.VersionName;
        }

        public int GetBuild()
        {
            var context = global::Android.App.Application.Context;
            PackageManager manager = context.PackageManager;
            PackageInfo info = manager.GetPackageInfo(context.PackageName, 0);

            return info.VersionCode;
        }
    }
}