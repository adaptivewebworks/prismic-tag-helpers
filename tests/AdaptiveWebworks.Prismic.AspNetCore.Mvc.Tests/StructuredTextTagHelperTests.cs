using Xunit;
using prismic;
using prismic.fragments;
using System.Collections.Generic;
using static prismic.fragments.StructuredText;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;
using System;

namespace AdaptiveWebworks.Prismic.AspNetCore.Mvc.Tests
{
    public class StructuredTextTagHelperTests
    {
        private readonly DocumentLinkResolver linkResolver = DocumentLinkResolver.For(_ => "/");

        private readonly StructuredText Fragment = new StructuredText(new List<Block>{
            new Paragraph("Test", new List<Span>(), null),
        });

        private readonly StructuredText FragmentWithLabel = new StructuredText(new List<Block>{
            new Paragraph("Test", new List<Span>(), "test-label"),
        });

        [Fact]
        public void StructuredTextTagHelper_throws_an_exception_when_constructor_arguments_are_not_supplied()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new StructuredTextTagHelper(null));
            Assert.Equal("linkResolver", exception.ParamName);
        }

        [Fact]
        public void StructuredTextTagHelper_does_not_output_null_fragment()
        {

            var ctx = CreateContext();
            var output = CreateOutput();

            var tagHelper = new StructuredTextTagHelper(linkResolver)
            {
                Fragment = null
            };

            tagHelper.Process(ctx, output);

            Assert.Null(output.TagName);
            Assert.Equal(string.Empty, output.Content.GetContent());
            Assert.False(output.Attributes.TryGetAttribute("content", out var _));
        }

        [Fact]
        public void StructuredTextTagHelper_outputs_content()
        {
            var ctx = CreateContext();
            var output = CreateOutput();

            var tagHelper = new StructuredTextTagHelper(linkResolver)
            {
                Fragment = Fragment
            };

            tagHelper.Process(ctx, output);

            Assert.Null(output.TagName);
            Assert.Equal("<p>Test</p>", output.Content.GetContent());
            Assert.False(output.Attributes.TryGetAttribute("content", out var _));
        }

        [Fact]
        public void StructuredTextTagHelper_outputs_attributes_correctly()
        {
            var ctx = CreateContext();
            var output = CreateOutput(new List<TagHelperAttribute> {
                new TagHelperAttribute("class", "test-class"),
                new TagHelperAttribute("single", "quotes", HtmlAttributeValueStyle.SingleQuotes),
                new TagHelperAttribute("no", "quotes", HtmlAttributeValueStyle.NoQuotes),
                new TagHelperAttribute("minimized", true, HtmlAttributeValueStyle.Minimized),
            });

            var tagHelper = new StructuredTextTagHelper(linkResolver)
            {
                Fragment = Fragment
            };

            tagHelper.Process(ctx, output);

            Assert.Null(output.TagName);
            Assert.Equal("<p class=\"test-class\" single='quotes' no=quotes minimized>Test</p>", output.Content.GetContent());
            Assert.False(output.Attributes.TryGetAttribute("content", out var _));
        }

        [Fact]
        public void StructuredTextTagHelper_outputs_multiple_blocks_wrapped_in_a_div()
        {
            var ctx = CreateContext();
            var output = CreateOutput();

            var tagHelper = new StructuredTextTagHelper(linkResolver)
            {
                Fragment = new StructuredText(new List<Block>{
                    new Paragraph("Test", new List<Span>(), null),
                    new Preformatted("Case", new List<Span>(), null),
                })
            };

            tagHelper.Process(ctx, output);

            Assert.Equal("div", output.TagName);
            Assert.Equal(TagMode.StartTagAndEndTag, output.TagMode);
            Assert.Equal("<p>Test</p><pre>Case</pre>", output.Content.GetContent());
            Assert.False(output.Attributes.TryGetAttribute("content", out var _));
        }

        [Fact]
        // TODO: PR Prismic SDK to not output empty content then amend this test.
        public void StructuredTextTagHelper_outputs_elements_with_empty_body()
        {
            var ctx = CreateContext();
            var output = CreateOutput();

            var tagHelper = new StructuredTextTagHelper(linkResolver)
            {
                Fragment = new StructuredText(new List<Block>{
                    new Paragraph(string.Empty, new List<Span>(), null),
                })
            };

            tagHelper.Process(ctx, output);

            Assert.Null(output.TagName);
            Assert.Equal("<p></p>", output.Content.GetContent());
            Assert.False(output.Attributes.TryGetAttribute("content", out var _));
        }

        [Fact]
        public void StructuredTextTagHelper_adds_label_to_cssClass_for_single_block_elements()
        {
            var ctx = CreateContext();
            var output = CreateOutput();

            var tagHelper = new StructuredTextTagHelper(linkResolver)
            {
                Fragment = FragmentWithLabel
            };

            tagHelper.Process(ctx, output);

            Assert.Null(output.TagName);
            Assert.Equal("<p class=\"test-label\">Test</p>", output.Content.GetContent());
            Assert.False(output.Attributes.TryGetAttribute("content", out var _));
        }

        [Fact]
        public void StructuredTextTagHelper_combines_cssClass_and_label_single_block_elements()
        {
            var ctx = CreateContext();
            var output = CreateOutput(new List<TagHelperAttribute> {
                new TagHelperAttribute("class", "test-class", HtmlAttributeValueStyle.DoubleQuotes)
            });

            var tagHelper = new StructuredTextTagHelper(linkResolver)
            {
                Fragment = FragmentWithLabel
            };

            tagHelper.Process(ctx, output);

            Assert.Null(output.TagName);
            Assert.Equal("<p class=\"test-class test-label\">Test</p>", output.Content.GetContent());
            Assert.False(output.Attributes.TryGetAttribute("content", out var _));
        }

        [Theory]
        [MemberData(nameof(TestBlocks))]
        public void StructuredTextTagHelper_correctly_renders_all_single_block_type(Block block, List<TagHelperAttribute> attributes, string expectedResult)
        {

            var ctx = CreateContext();
            var output = CreateOutput(attributes);

            var tagHelper = new StructuredTextTagHelper(linkResolver)
            {
                Fragment = new StructuredText(new List<Block>{
                    block
                })
            };

            tagHelper.Process(ctx, output);

            Assert.Equal(expectedResult, output.Content.GetContent());
        }

        private TagHelperOutput CreateOutput(List<TagHelperAttribute> attributes = null)
        {
            var attributesList = new List<TagHelperAttribute>();

            if (attributes != null)
                attributesList.AddRange(attributes);

            return new TagHelperOutput(
                           "structured-text",
                           new TagHelperAttributeList(attributesList),
                           (useCachedResult, encoder) =>
                               {
                                   var tagHelperContent = new DefaultTagHelperContent();
                                   tagHelperContent.SetContent(string.Empty);
                                   return Task.FromResult<TagHelperContent>(tagHelperContent);
                               }
                       );
        }

        private TagHelperContext CreateContext()
            => new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N")
            );

        public static IEnumerable<object[]> TestBlocks()
        {
            const string label = "test-label";
            const string expectedAttrs = " data-test-attr=\"attr-test-case\" class=\"test-class test-label\"";

            var attrs = new List<TagHelperAttribute> {
                new TagHelperAttribute("data-test-attr", "attr-test-case"),
                new TagHelperAttribute("class", "test-class")
            };

            yield return new object[]{
                new Paragraph("Test", new List<Span>(), label),
                attrs,
                $"<p{expectedAttrs}>Test</p>"
            };

            yield return new object[]{
                new Heading("Test", new List<Span>(), 1, label),
                attrs,
                $"<h1{expectedAttrs}>Test</h1>"
            };

            yield return new object[]{
                new Preformatted("Test", new List<Span>(), label),
                attrs,
                $"<pre{expectedAttrs}>Test</pre>"
            };

            var imgUrl = "https://example.com/image.png";
            var alt = "Exmaple Image";

            yield return new object[]{
                new StructuredText.Image(new prismic.fragments.Image.View(imgUrl, 1, 2, alt, null, null), label),
                attrs,
                $"<img src=\"{imgUrl}\" alt=\"{alt}\"{expectedAttrs} width=\"1\" height=\"2\" />"
            };
        }
    }
}
