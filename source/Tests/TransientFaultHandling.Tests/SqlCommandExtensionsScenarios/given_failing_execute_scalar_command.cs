﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace EnterpriseLibrary.TransientFaultHandling.Tests.SqlCommandExtensionsScenarios.given_failing_execute_scalar_command
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using Common.TestSupport.ContextBase;

    using EnterpriseLibrary.TransientFaultHandling.TestSupport;
    using EnterpriseLibrary.TransientFaultHandling;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public abstract class Context : ArrangeActAssert
    {
        protected TestRetryStrategy connectionStrategy;
        protected TestRetryStrategy commandStrategy;
        protected SqlCommand command;
        protected RetryPolicy connectionPolicy;
        protected RetryPolicy commandPolicy;

        protected override void Arrange()
        {
            this.command = new SqlCommand(TestSqlSupport.InvalidSqlText);

            this.connectionStrategy = new TestRetryStrategy();

            this.commandStrategy = new TestRetryStrategy();

            this.connectionPolicy = new RetryPolicy<AlwaysTransientErrorDetectionStrategy>(this.connectionStrategy);

            this.commandPolicy = new RetryPolicy<AlwaysTransientErrorDetectionStrategy>(this.commandStrategy);
        }
    }

    [TestClass]
    public class when_executing_command_with_closed_connection : Context
    {
        protected override void Act()
        {
            try
            {
                this.command.Connection = new SqlConnection(TestSqlSupport.SqlDatabaseConnectionString);
                this.command.ExecuteScalarWithRetry(this.commandPolicy, this.connectionPolicy);
                Assert.Fail();
            }
            catch (SqlException)
            {
            }
        }

        [TestMethod]
        public void then_connection_is_closed()
        {
            Assert.IsNotNull(this.command.Connection);
            Assert.IsTrue(this.command.Connection.State == ConnectionState.Closed);
        }

        [TestMethod]
        public void then_retried()
        {
            Assert.AreEqual(0, this.connectionStrategy.ShouldRetryCount);
            Assert.AreEqual(1, this.commandStrategy.ShouldRetryCount);
        }
    }

    [TestClass]
    public class when_executing_command_with_opened_connection : Context
    {
        protected override void Act()
        {
            try
            {
                this.command.Connection = new SqlConnection(TestSqlSupport.SqlDatabaseConnectionString);
                this.command.Connection.Open();
                this.command.ExecuteScalarWithRetry(this.commandPolicy, this.connectionPolicy);
                Assert.Fail();
            }
            catch (SqlException)
            {
            }
        }

        [TestMethod]
        public void then_connection_is_opened()
        {
            Assert.IsNotNull(this.command.Connection);
            Assert.IsTrue(this.command.Connection.State == ConnectionState.Open);
        }

        [TestMethod]
        public void then_retried()
        {
            Assert.AreEqual(0, this.connectionStrategy.ShouldRetryCount);
            Assert.AreEqual(1, this.commandStrategy.ShouldRetryCount);
        }
    }
}