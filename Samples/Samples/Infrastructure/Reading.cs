using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Samples.Infrastructure
{
    public class Reading : TableEntity
    {
        public DateTime ReadingTime { get; set; }

        public string ReadingValue { get; set; }
    }
}
