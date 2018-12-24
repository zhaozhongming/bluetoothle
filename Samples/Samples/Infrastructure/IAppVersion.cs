using System;
using System.Collections.Generic;
using System.Text;

namespace Samples.Infrastructure
{
    public interface IAppVersion
    {
        string GetVersion();
        int GetBuild();
    }
}
