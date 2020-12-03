using System;
using System.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;
using prismic;
using prismic.fragments;

namespace AdaptiveWebworks.Prismic.AspNetCore.Mvc
{
    [HtmlTargetElement("structured-text")]
    public class StructuredTextTagHelper : PrismicFragmentTagHelper<StructuredText>
    {
        private readonly DocumentLinkResolver _linkResolver;

        public StructuredTextTagHelper(DocumentLinkResolver linkResolver)
        {
            _linkResolver = linkResolver ?? throw new ArgumentNullException(nameof(linkResolver));
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Fragment == null || !Fragment.Blocks.Any())
            {
                output.SuppressOutput();
                return;
            }

            if (Fragment.Blocks.Count > 1)
            {
                output.TagName = "div";
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Content.SetHtmlContent(GetHtmlForMulptipleBlocks(output));
                return;
            }

            output.SuppressOutput();
            output.Content.SetHtmlContent(GetHtmlForSingleBlock(output));
        }

        protected virtual string GetHtmlForMulptipleBlocks(TagHelperOutput output)
            => Fragment.AsHtml(_linkResolver);

        protected virtual string GetHtmlForSingleBlock(TagHelperOutput output)
        {
            var label = Fragment.Blocks.First().Label;

            if (!string.IsNullOrWhiteSpace(label))
                UpdateCssClass(output, label);

            var attrs = CreateHtmlAttributeString(output.Attributes);

            return Fragment.AsHtml(_linkResolver, CreateSingleElementSerializer(attrs));
        }

        protected string CreateHtmlAttributeString(TagHelperAttributeList attributeList)
        {
            var attributes = string.Join(
                " ",
                attributeList
                .Select(BuildAttribute)
                .Where(x => !string.IsNullOrWhiteSpace(x))
            );

            if (string.IsNullOrWhiteSpace(attributes))
                return string.Empty;

            return $" {attributes}";
        }

        protected string BuildAttribute(TagHelperAttribute attr)
        {
            switch (attr.ValueStyle)
            {
                case HtmlAttributeValueStyle.SingleQuotes:
                    return $"{attr.Name}='{attr.Value}'";
                case HtmlAttributeValueStyle.NoQuotes:
                    return $"{attr.Name}={attr.Value}";
                case HtmlAttributeValueStyle.Minimized:
                    return attr.Name;
                case HtmlAttributeValueStyle.DoubleQuotes:
                default:
                    return $"{attr.Name}=\"{attr.Value}\"";
            }
        }

        protected virtual HtmlSerializer CreateSingleElementSerializer(string attributes)
            => HtmlSerializer.For(
                (el, body) =>
                {
                    switch (el)
                    {
                        case StructuredText.Heading h:
                            return $"<h{h.Level}{attributes}>{body}</h{h.Level}>";
                        case StructuredText.Paragraph p:
                            return $"<p{attributes}>{body}</p>";
                        case StructuredText.Preformatted pre:
                            return $"<pre{attributes}>{body}</pre>";
                        case StructuredText.Embed em:
                            return $"<div{attributes}>{em.Obj.AsHtml()}</div>";
                        case StructuredText.Image img:
                            var sizeAttrs = string.Empty;

                            if (img.Width > 0)
                                sizeAttrs += $" width=\"{img.Width}\"";

                            if (img.Height > 0)
                                sizeAttrs += $" height=\"{img.Height}\"";

                            return $"<img src=\"{img.Url}\" alt=\"{img.View.Alt}\"{attributes}{sizeAttrs} />";
                        default:
                            return null;
                    }
                }
            );

        protected override StructuredText GetField() => Document.GetStructuredText(Field);

        private void UpdateCssClass(TagHelperOutput output, string label)
        {
            var cssClassAttr = output.Attributes.FirstOrDefault(a => a?.Name == "class");
            var cssClass = label;

            if (cssClassAttr != null)
            {
                output.Attributes.Remove(cssClassAttr);
                cssClass = string.Join(" ", new[] { cssClassAttr.Value.ToString(), label }.Where(x => !string.IsNullOrWhiteSpace(x)));
            }

            output.Attributes.Add(CreateCssClassAttribute(cssClass));
        }

        private TagHelperAttribute CreateCssClassAttribute(string cssClass)
            => new TagHelperAttribute(
                "class",
                cssClass,
                HtmlAttributeValueStyle.DoubleQuotes
            );

    }
}