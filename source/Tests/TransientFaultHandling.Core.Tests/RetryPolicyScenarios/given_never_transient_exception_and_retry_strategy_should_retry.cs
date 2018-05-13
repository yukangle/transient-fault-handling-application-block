﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

#pragma warning disable 618

namespace EnterpriseLibrary.TransientFaultHandling.Tests.RetryPolicyScenarios.given_never_transient_exception_and_retry_strategy_should_retry
{
    using System;
    using Common.TestSupport.ContextBase;
    using EnterpriseLibrary.TransientFaultHandling.TestSupport;
    using EnterpriseLibrary.TransientFaultHandling;
    using Moq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Threading.Tasks;

    public abstract class Context : ArrangeActAssert
    {
        protected RetryPolicy retryPolicy;
        protected Mock<RetryStrategy> retryStrategyMock;

        protected override void Arrange()
        {
            this.retryStrategyMock = new Mock<RetryStrategy>("name", false);
            this.retryStrategyMock.Setup(x => x.GetShouldRetry())
                .Returns(
                    () => 
                        delegate(int currentRetryCount, Exception lastException, out TimeSpan interval)
                        {
                            interval = TimeSpan.Zero;
                            return currentRetryCount == 0;
                        }
                    );

            this.retryPolicy = new RetryPolicy<NeverTransientErrorDetectionStrategy>(retryStrategyMock.Object);
        }
    }

    [TestClass]
    public class when_executing_action : Context
    {
        private int execCount;

        protected override void Act()
        {
            try
            {
                this.retryPolicy.ExecuteAction(
                    () =>
                        {
                            execCount++;
                            throw new Exception();
                        }
                    );

                Assert.Fail();
            }
            catch
            {
                
            }
        }

        [TestMethod]
        public void then_does_not_retry()
        {
            Assert.AreEqual(1, this.execCount);
        }
    }

    [TestClass]
    public class when_executing_func : Context
    {
        private int execCount;

        protected override void Act()
        {
            try
            {
                this.retryPolicy.ExecuteAction<int>(
                    () =>
                        {
                            execCount++;
                            throw new Exception();
                        }
                    );
            }
            catch
            {

            }
        }

        [TestMethod]
        public void then_does_not_retry()
        {
            Assert.AreEqual(1, this.execCount);
        }
    }

    [TestClass]
    public class when_executing_async : Context
    {
        private int timesStarted;
        private Task task;
        private Exception exception;

        protected override void Act()
        {
            this.task = this.retryPolicy
                .ExecuteAsync(() =>
                {
                    int result = ++timesStarted;
                    return Task.Run<int>((Func<int>)(() => { throw new Exception(); }));
                });

            try
            {
                task.Wait(TimeSpan.FromSeconds(2));
            }
            catch (Exception e)
            {
                this.exception = e;
            }
        }

        [TestMethod]
        public void then_does_not_retry()
        {
            Assert.AreEqual(1, this.timesStarted);
        }

        [TestMethod]
        public void then_is_faulted()
        {
            Assert.IsTrue(this.task.IsFaulted);
        }
    }
}