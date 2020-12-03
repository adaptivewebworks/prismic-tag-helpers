using Microsoft.AspNetCore.Razor.TagHelpers;
using prismic.fragments;

namespace AdaptiveWebworks.Prismic.AspNetCore.Mvc
{
    [HtmlTargetElement("img", Attributes = src)]
    public class PrismicImageTagHelper : TagHelper
    {
        const string src = "prismic-src";
        const string viewName = "view-name";

        [HtmlAttributeName(src)]
        public Image Image { get; set; }

        [HtmlAttributeName(viewName)]
        public string ViewName { get; set; } = "main";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Image == null || !Image.TryGetView(ViewName, out var imageView))
            {
                output.SuppressOutput();
                return;
            }

            output.Attributes.RemoveAll(src);
            output.Attributes.RemoveAll(viewName);

            output.Attributes.Add("src", imageView.Url);
            output.Attributes.Add("alt", imageView.Alt);
        }
    }
}
