using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdaptiveWebworks.Prismic.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using prismic;
using prismic.fragments;
using Xunit;

namespace AdaptiveWebworks.Prismic.Tests.AspNetCoreMvc
{
    public class PrismicLinkResolverTagHelperTests
    {

        [Fact]
        public void PrismicLinkResolverTagHelper_suppresses_output_href_is_null()
        {
            var ctx = CreateContext();
            var output = CreateOutput();

            var linkResolver = DocumentLinkResolver.For(l => string.Empty);
            var tagHelper = new LinkResolverTagHelper(linkResolver);

            tagHelper.Process(ctx, output);

            Assert.Empty(output.Content.GetContent());
        }

        [Theory]
        [MemberData(nameof(TestLinks))]
        public void PrismicLinkResolverTagHelper_link_resolves_correctly(
            ILink link,
            string expectedHref
        )
        {
            var ctx = CreateContext();
            var output = CreateOutput();

            var linkResolver = DocumentLinkResolver.For(l => l.Uid);
            var tagHelper = new LinkResolverTagHelper(linkResolver)
            {
                Link = link,
            };

            tagHelper.Process(ctx, output);
            Assert.True(output.Attributes.ContainsName("href"));
            Assert.Equal(expectedHref, output.Attributes["href"].Value);
        }

        private TagHelperOutput CreateOutput(List<TagHelperAttribute> attributes = null)
            => new TagHelperOutput(
                "a",
                new TagHelperAttributeList(attributes ?? new List<TagHelperAttribute>()),
                (useCachedResult, encoder) =>
                    {
                        var tagHelperContent = new DefaultTagHelperContent();
                        tagHelperContent.SetContent(string.Empty);
                        return Task.FromResult<TagHelperContent>(tagHelperContent);
                    }
                );

        private TagHelperContext CreateContext(List<TagHelperAttribute> attributes = null)
        {
            return new TagHelperContext(
                new TagHelperAttributeList(attributes ?? new List<TagHelperAttribute>()),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N")
            );
        }

        public static IEnumerable<object[]> TestLinks()
        {
            yield return new object[]{
                new WebLink("http://example.com", null, null),
                "http://example.com"
            };

            yield return new object[]{
                new FileLink("./file", null, 0, null),
                "./file"
            };

            yield return new object[]{
                new ImageLink("./image.jpg"),
                "./image.jpg"
            };


            yield return new object[]{
                new DocumentLink("id", "test", null, new HashSet<string>{}, null, null, new Dictionary<string, IFragment>(), false),
                "test"
            };

            yield return new object[]{
                new UnknownLink(),
                "#"
            };
        }

        private class UnknownLink : ILink
        {
            public string GetUrl(DocumentLinkResolver resolver)
            {
                return null;
            }
        }
    }
}