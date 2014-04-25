using System;
using Stored.Query;
using Xunit;

namespace Stored.Tests
{
    public class ExpressionHelperTests
    {
        [Fact]
        public void CanGetMemberExpressionName()
        {
            // Act / Assert
            Assert.Equal("PropertyOne", ExpressionHelper.GetName<TestClass>(x => x.PropertyOne));
        }

        [Fact]
        public void CanGetUnaryExpressionName()
        {
            // Act / Assert
            Assert.Equal("PropertyTwo", ExpressionHelper.GetName<TestClass>(x => x.PropertyTwo));
        }

        class TestClass
        {
            public string PropertyOne { get; set; }
            public Guid PropertyTwo { get; set; }
        }
    }
}
