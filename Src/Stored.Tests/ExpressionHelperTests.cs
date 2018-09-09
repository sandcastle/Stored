using System;
using Stored.Query;
using Xunit;

namespace Stored.Tests
{
    public class ExpressionHelperTests
    {
        [Fact]
        public void CanGetMemberExpressionName() =>
            Assert.Equal("PropertyOne", ExpressionHelper.GetName<TestClass>(x => x.PropertyOne));

        [Fact]
        public void CanGetUnaryExpressionName() =>
            Assert.Equal("PropertyTwo", ExpressionHelper.GetName<TestClass>(x => x.PropertyTwo));

        [Fact]
        public void CanGetMemberExpressionType() =>
            Assert.Equal(typeof(string), ExpressionHelper.GetPropertyType<TestClass>(x => x.PropertyOne));

        [Fact]
        public void CanGetUnaryExpressionType() =>
            Assert.Equal(typeof(Guid), ExpressionHelper.GetPropertyType<TestClass>(x => x.PropertyTwo));


        [Fact]
        public void CanGetDateTimeExpressionType() =>
            Assert.Equal(typeof(DateTime), ExpressionHelper.GetPropertyType<TestClass>(x => x.PropertyDate));

        class TestClass
        {
            public string PropertyOne { get; set; }
            public Guid PropertyTwo { get; set; }
            public DateTime PropertyDate { get; set; }
        }
    }
}
