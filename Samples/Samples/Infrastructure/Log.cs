using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Samples.Infrastructure
{
    public class Log : TableEntity
    {
        public string Message { get; set; }
    }
}
