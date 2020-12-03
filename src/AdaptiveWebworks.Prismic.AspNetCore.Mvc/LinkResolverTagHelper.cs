using Microsoft.AspNetCore.Razor.TagHelpers;
using prismic;
using prismic.fragments;

namespace AdaptiveWebworks.Prismic.AspNetCore.Mvc
{
    [HtmlTargetElement("a", Attributes = href)]
    public class LinkResolverTagHelper : PrismicFragmentTagHelper<ILink>
    {
        const string href = "prismic-href";
        private readonly DocumentLinkResolver _linkResolver;

        [HtmlAttributeName(href)]
        public ILink Link
        {
            set
            {
                Fragment = value;
            }
        }

        public LinkResolverTagHelper(DocumentLinkResolver linkResolver)
        {
            _linkResolver = linkResolver;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.RemoveAll(href);

            if (Fragment == null)
            {
                output.TagName = "span";
                return;
            }

            output.Attributes.Add("href", ResolveLink());
        }

        protected virtual string ResolveLink()
        {
            switch (Fragment)
            {
                case DocumentLink d:
                    return _linkResolver.Resolve(d);
                case WebLink w:
                    return w.Url;
                case ImageLink i:
                    return i.Url;
                case FileLink f:
                    return f.Url;
                default:
                    return "#";
            }
        }

        protected override ILink GetField() => Document.GetLink(Field);
    }
}
