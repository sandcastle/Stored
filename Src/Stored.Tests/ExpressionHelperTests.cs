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

        [Fact]
        public void CanGetMemberExpressionType()
        {
            // Act / Assert
            Assert.Equal(typeof(string), ExpressionHelper.GetPropertyType<TestClass>(x => x.PropertyOne));
        }

        [Fact]
        public void CanGetUnaryExpressionType()
        {
            // Act / Assert
            Assert.Equal(typeof(Guid), ExpressionHelper.GetPropertyType<TestClass>(x => x.PropertyTwo));
        }

        [Fact]
        public void CanGetDateTimeExpressionType()
        {
            // Act / Assert
            Assert.Equal(typeof(DateTime), ExpressionHelper.GetPropertyType<TestClass>(x => x.PropertyDate));
        }

        class TestClass
        {
            public string PropertyOne { get; set; }
            public Guid PropertyTwo { get; set; }
            public DateTime PropertyDate { get; set; }
        }
    }
}
