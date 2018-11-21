﻿using System;
using System.Threading;

namespace DotJEM.Diagnostic
{
    /*
     * Log as:
     *
     * Time                            Id                         Thread     CustomFields        Data
     * 2018-11-30T11:16:52.657862      02efb36.b992e58.0000000    47         JMD     >>>         { }
     * 2018-11-30T11:16:53.657862      02efb36.fe92a58.b992e58    134        JMD     >>>         { }
     * 2018-11-30T11:16:54.657862      02efb36.e452fe8.fe92a58    200        JMD     >>>         { }
     * 2018-11-30T11:16:54.757862      02efb36.aef928a.e452fe8    42         JMD     <<<         { }
     * 2018-11-30T11:16:56.657862      02efb36.a129ee8.fe92a58    9          JMD     >>>         { }
     *
     * Custom fields are custom provider for fixed length fields, this can be identity of the user, start/stop signalling etc
     * 
     * Data is a custom data object, which is ToString'ed... Each aditional line will be prepended with a indentation to
     * identify lines belonging to the statement. Data is merely a Custom field provider
     *
     *
     */

    public class CorrelationScope : Disposable
    {
        public static AsyncLocal<CorrelationScope> Current { get; } = new AsyncLocal<CorrelationScope>();
        public string Id { get; }
        public string CorrelationId { get; }
        public CorrelationScope Parent { get; }

        public CorrelationScope()
        {
            Parent = Current.Value;

            Id = IdProvider.Default.Next;
            CorrelationId = Parent?.CorrelationId ?? IdProvider.Default.Next;

            Current.Value = this;
        }

        protected override void Dispose(bool disposing)
        {
            Current.Value = Parent;
            base.Dispose(disposing);
        }

        public override string ToString()
        {
            return $"{CorrelationId}.{Id}.{Parent?.Id??"00000000"}";
        }
    }

    public class TraceEvent
    {
        public DateTimeOffset Time { get; }

    }
}
