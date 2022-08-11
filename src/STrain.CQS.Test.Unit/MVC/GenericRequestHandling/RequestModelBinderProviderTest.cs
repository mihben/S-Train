using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Moq;
using STrain.CQS.MVC.GenericRequestHandling;
using STrain.CQS.Test.Unit.Supports;

namespace STrain.CQS.Test.Unit.MVC.GenericRequestHandling
{
    public class RequestModelBinderProviderTest
    {
        private RequestModelBinderProvider CreateSUT()
        {
            return new RequestModelBinderProvider();
        }

        [Fact(DisplayName = "[UNIT][RMBP-001]: Resolve CommandModelBinder")]
        public void RequestModelBinderProvider_GetBinder_CommandModelBinder()
        {
            // Arrange
            var sut = CreateSUT();
            var context = new Mock<ModelBinderProviderContext>();

            context.Setup(c => c.Metadata)
                .Returns(new TestModelMetadata<TestCommand>());

            // Act
            var result = sut.GetBinder(context.Object);

            // Assert
            Assert.IsType<BinderTypeModelBinder>(result);
        }

        [Fact(DisplayName = "[UNIT][RMBP-002]: Resolve QueryModelBinder")]
        public void RequestModelBinderProvider_GetBinder_QueryModelBinder()
        {
            // Arrange
            var sut = CreateSUT();
            var context = new Mock<ModelBinderProviderContext>();

            context.Setup(c => c.Metadata)
                .Returns(new TestModelMetadata<TestQuery>());

            // Act
            var result = sut.GetBinder(context.Object);

            // Assert
            Assert.IsType<BinderTypeModelBinder>(result);
        }

        [Fact(DisplayName = "[UNIT][RMBP-003]: Unsupported type")]
        public void RequestModelBinderProvider_GetBinder_UnsupportedType()
        {
            // Arrange
            var sut = CreateSUT();
            var context = new Mock<ModelBinderProviderContext>();

            context.Setup(c => c.Metadata)
                .Returns(new TestModelMetadata<object>());

            // Act
            var result = sut.GetBinder(context.Object);

            // Assert
            Assert.Null(result);
        }
    }

    internal class TestModelMetadata<T> : ModelMetadata
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public TestModelMetadata()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            : base(ModelMetadataIdentity.ForType(typeof(T)))
        {
        }

        public override IReadOnlyDictionary<object, object> AdditionalValues { get; }
        public override ModelPropertyCollection Properties { get; }
        public override string BinderModelName { get; }
        public override Type BinderType { get; }
        public override BindingSource BindingSource { get; }
        public override bool ConvertEmptyStringToNull { get; }
        public override string DataTypeName { get; }
        public override string Description { get; }
        public override string DisplayFormatString { get; }
        public override string DisplayName { get; }
        public override string EditFormatString { get; }
        public override ModelMetadata ElementMetadata { get; }
        public override IEnumerable<KeyValuePair<EnumGroupAndName, string>> EnumGroupedDisplayNamesAndValues { get; }
        public override IReadOnlyDictionary<string, string> EnumNamesAndValues { get; }
        public override bool HasNonDefaultEditFormat { get; }
        public override bool HtmlEncode { get; }
        public override bool HideSurroundingHtml { get; }
        public override bool IsBindingAllowed { get; }
        public override bool IsBindingRequired { get; }
        public override bool IsEnum { get; }
        public override bool IsFlagsEnum { get; }
        public override bool IsReadOnly { get; }
        public override bool IsRequired { get; }
        public override ModelBindingMessageProvider ModelBindingMessageProvider { get; }
        public override int Order { get; }
        public override string Placeholder { get; }
        public override string NullDisplayText { get; }
        public override IPropertyFilterProvider PropertyFilterProvider { get; }
        public override bool ShowForDisplay { get; }
        public override bool ShowForEdit { get; }
        public override string SimpleDisplayProperty { get; }
        public override string TemplateHint { get; }
        public override bool ValidateChildren { get; }
        public override IReadOnlyList<object> ValidatorMetadata { get; }
        public override Func<object, object> PropertyGetter { get; }
        public override Action<object, object> PropertySetter { get; }
    }
}
