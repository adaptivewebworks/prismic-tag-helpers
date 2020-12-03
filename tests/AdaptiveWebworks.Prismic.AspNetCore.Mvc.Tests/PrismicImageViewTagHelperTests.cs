using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdaptiveWebworks.Prismic.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Xunit;
using ImageFragment = prismic.fragments.Image;

namespace AdaptiveWebworks.Prismic.Tests.AspNetCoreMvc
{
    public class PrismicImageViewTagHelperTests {

        [Fact]
        public void PrismicImageViewTagHelper_suppress_output_if_image_view_is_null()
        {
            var ctx = CreateContext();
            var output = CreateOutput();

            var tagHelper = new PrismicImageTagHelper()
            {
                Image = new ImageFragment(new ImageFragment.View("main.jpg", 0, 0, null, null, null)),
                ViewName = "DOES_NOT_EXIST"
            };

            tagHelper.Process(ctx, output);

            Assert.Null(output.TagName);
        }

        [Fact]
        public void PrismicImageViewTagHelper_outputs_main_by_default()
        {
            var mainSrc = "./main.jpg";
            var mainAlt = "main image";

            var attributes = new List<TagHelperAttribute> {
                new TagHelperAttribute("prismic-src"),
                new TagHelperAttribute("view-name")
            };
            var ctx = CreateContext(attributes);
            var output = CreateOutput(attributes);

            var tagHelper = new PrismicImageTagHelper()
            {
                Image = new ImageFragment(new ImageFragment.View(mainSrc, 0, 0, mainAlt, null, null)),
            };

            tagHelper.Process(ctx, output);

            Assert.Equal("img", output.TagName);
            Assert.False(output.Attributes.ContainsName("prismic-src"));
            Assert.False(output.Attributes.ContainsName("view-name"));
            Assert.True(output.Attributes.TryGetAttribute("src", out var src));
            Assert.Equal(mainSrc, src.Value);
            Assert.True(output.Attributes.TryGetAttribute("alt", out var alt));
            Assert.Equal(mainAlt, alt.Value);

        }

        private TagHelperOutput CreateOutput(List<TagHelperAttribute> attributes = null)
            => new TagHelperOutput(
                "img",
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
    }
}