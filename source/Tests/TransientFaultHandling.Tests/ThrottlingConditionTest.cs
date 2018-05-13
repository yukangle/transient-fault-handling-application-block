﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using EnterpriseLibrary.TransientFaultHandling.Data;
using EnterpriseLibrary.TransientFaultHandling.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EnterpriseLibrary.TransientFaultHandling.Tests
{
    [TestClass]
    public class ThrottlingConditionTest
    {
        [TestMethod]
        public void TestDecodeThrottlingReasonCode()
        {
            SqlError err = FakeSqlExceptionGenerator.GenerateFakeSqlError(ThrottlingCondition.ThrottlingErrorNumber, Resources.SampleThrottlingErrorMsg);
            ThrottlingCondition condition = ThrottlingCondition.FromError(err);

            Assert.AreEqual<ThrottlingMode>(ThrottlingMode.RejectAll, condition.ThrottlingMode, "Unexpected throttling mode.");

            var throttlingType = condition.ThrottledResources.Where(x => x.Item1 == ThrottledResourceType.Cpu).Select(x => x.Item2).FirstOrDefault();
            Assert.AreEqual<ThrottlingType>(ThrottlingType.Hard, throttlingType, "Unexpected throttling type.");
        }

        [TestMethod]
        [Ignore]    // REVIEW - Test will continue throwing the exception so the action will eventually run out of retry attempts
        public void TestThrottlingConditionSimulation()
        {
            // Instantiate a retry policy capable of detecting transient faults that are specific to SQL Database. Use the default retry count.
            RetryPolicy retryPolicy = new RetryPolicy<SqlDatabaseTransientErrorDetectionStrategy>(RetryStrategy.DefaultClientRetryCount);

            // Register a custom event handler that will be invoked by the policy whenever a retry condition is encountered.
            retryPolicy.Retrying += (sender, args) =>
            {
                // Log a warning message to indicate that a retry loop kicked in.
                Trace.TraceWarning("Retry condition encountered. Reason: {0} (retry count: {1}, retry delay: {2})", args.LastException.Message, args.CurrentRetryCount, args.Delay);

                // Make sure we are dealing with a SQL exception.
                if (args.LastException is SqlException)
                {
                    // Parse the exception to find out whether it is indicative of any throttling conditions.
                    ThrottlingCondition throttlingCondition = ThrottlingCondition.FromException(args.LastException as SqlException);

                    // Verify whether throttling condition were detected.
                    if (!throttlingCondition.IsUnknown)
                    {
                        // Log a further warning message with details on throttling conditions encountered.
                        Trace.TraceWarning("Throttling condition detected. Details: {0}", throttlingCondition);
                    }
                }
            };

            // Invoke a database operation or set of database operations against SQL Database.
            retryPolicy.ExecuteAction(() =>
            {
                // Open a connection, execute a SQL query or stored procedure, perform any kind of database operations.
                // ...

                var sqlEx = FakeSqlExceptionGenerator.GenerateFakeSqlException(ThrottlingCondition.ThrottlingErrorNumber, Resources.SampleThrottlingErrorMsg);
                throw sqlEx;
            });
        }
    }
}
