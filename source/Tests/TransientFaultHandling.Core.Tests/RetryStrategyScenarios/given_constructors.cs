﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace EnterpriseLibrary.TransientFaultHandling.Tests.RetryStrategyScenarios.given_constructors
{
    using System;
    using EnterpriseLibrary.Common.TestSupport.ContextBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class Context : ArrangeActAssert
    {
    }

    [TestClass]
    public class when_creating_a_fixed_interval_strategy : Context
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void then_throws_if_retry_count_is_negative()
        {
            new FixedInterval("", -1, TimeSpan.FromSeconds(1), true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void then_throws_if_retry_interval_is_negative()
        {
            new FixedInterval("", 1, TimeSpan.FromSeconds(-1), true);
        }
    }
}
