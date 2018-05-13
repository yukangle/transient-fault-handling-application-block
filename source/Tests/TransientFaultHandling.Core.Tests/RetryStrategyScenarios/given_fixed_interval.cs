﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace EnterpriseLibrary.TransientFaultHandling.Tests.RetryStrategyScenarios.given_fixed_interval
{
    using System;
    using Common.TestSupport.ContextBase;

    using EnterpriseLibrary.TransientFaultHandling;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public abstract class Context : ArrangeActAssert
    {
        protected override void Arrange()
        {
        }
    }

    [TestClass]
    public class when_using_default_values : Context
    {
        protected RetryStrategy retryStrategy;
        protected ShouldRetry shouldRetry;

        protected override void Act()
        {
            this.retryStrategy = new FixedInterval();
            this.shouldRetry = this.retryStrategy.GetShouldRetry();
        }

        [TestMethod]
        public void then_default_values_are_used()
        {
            Assert.IsNull(retryStrategy.Name);

            TimeSpan delay;
            Assert.IsTrue(shouldRetry(0, null, out delay));
            Assert.AreEqual(TimeSpan.FromSeconds(1), delay);

            Assert.IsTrue(shouldRetry(9, null, out delay));
            Assert.AreEqual(TimeSpan.FromSeconds(1), delay);

            Assert.IsFalse(shouldRetry(10, null, out delay));
            Assert.AreEqual(TimeSpan.Zero, delay);
        }
    }

    [TestClass]
    public class when_using_default_interval : Context
    {
        protected RetryStrategy retryStrategy;
        protected ShouldRetry shouldRetry;

        protected override void Act()
        {
            this.retryStrategy = new FixedInterval(5);
            this.shouldRetry = this.retryStrategy.GetShouldRetry();
        }

        [TestMethod]
        public void then_default_values_are_used()
        {
            Assert.IsNull(retryStrategy.Name);

            TimeSpan delay;
            Assert.IsTrue(shouldRetry(0, null, out delay));
            Assert.AreEqual(TimeSpan.FromSeconds(1), delay);

            Assert.IsTrue(shouldRetry(4, null, out delay));
            Assert.AreEqual(TimeSpan.FromSeconds(1), delay);

            Assert.IsFalse(shouldRetry(5, null, out delay));
            Assert.AreEqual(TimeSpan.Zero, delay);
        }
    }

    [TestClass]
    public class when_using_custom_values : Context
    {
        protected RetryStrategy retryStrategy;
        protected ShouldRetry shouldRetry;

        protected override void Act()
        {
            this.retryStrategy = new FixedInterval("name", 5, TimeSpan.FromSeconds(5));
            this.shouldRetry = this.retryStrategy.GetShouldRetry();
        }

        [TestMethod]
        public void then_default_values_are_used()
        {
            Assert.AreEqual("name", retryStrategy.Name);

            TimeSpan delay;
            Assert.IsTrue(shouldRetry(0, null, out delay));
            Assert.AreEqual(TimeSpan.FromSeconds(5), delay);

            Assert.IsTrue(shouldRetry(4, null, out delay));
            Assert.AreEqual(TimeSpan.FromSeconds(5), delay);

            Assert.IsFalse(shouldRetry(5, null, out delay));
            Assert.AreEqual(TimeSpan.Zero, delay);
        }
    }
}
