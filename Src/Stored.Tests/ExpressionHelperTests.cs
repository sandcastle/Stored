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
            Assert.Equal("PropertyString", ExpressionHelper.GetName<TestClass, object>(x => x.PropertyString));
        }

        [Fact]
        public void CanGetUnaryExpressionName()
        {
            Assert.Equal("PropertyGuid", ExpressionHelper.GetName<TestClass, object>(x => x.PropertyGuid));
        }

        [Fact]
        public void CanGetStringPropertyType()
        {
            Assert.Equal(typeof(string), ExpressionHelper.GetPropertyType<TestClass, object>(x => x.PropertyString));
        }

        [Fact]
        public void CanGetGuidPropertyType()
        {
            Assert.Equal(typeof(Guid), ExpressionHelper.GetPropertyType<TestClass, object>(x => x.PropertyGuid));
        }

        [Fact]
        public void CanGetDateTimePropertyType()
        {
            Assert.Equal(typeof(DateTime), ExpressionHelper.GetPropertyType<TestClass, object>(x => x.PropertyDateTime));
        }

        [Fact]
        public void CanGetBoolPropertyType()
        {
            Assert.Equal(typeof(bool), ExpressionHelper.GetPropertyType<TestClass, object>(x => x.PropertyBool));
        }

        [Fact]
        public void CanGetEnumPropertyType()
        {
            Assert.Equal(typeof(TestValues), ExpressionHelper.GetPropertyType<TestClass, object>(x => x.PropertyEnum));
        }

        class TestClass
        {
            public string PropertyString { get; set; }
            public Guid PropertyGuid { get; set; }
            public DateTime PropertyDateTime { get; set; }
            public bool PropertyBool { get; set; }
            public TestValues PropertyEnum { get; set; }
        }

        enum TestValues
        {
            One = 0,
            Two = 1,
            Three = 2
        }
    }
}
