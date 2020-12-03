using Microsoft.AspNetCore.Razor.TagHelpers;
using prismic;

namespace AdaptiveWebworks.Prismic.AspNetCore.Mvc
{
    public abstract class PrismicFragmentTagHelper<TFragment> : TagHelper
        where TFragment : IFragment
    {
        private TFragment _fragment;

        [HtmlAttributeName("prismic-fragment")]
        public virtual TFragment Fragment
        {
            get
            {
                if (_fragment != null)
                    return _fragment;

                if (string.IsNullOrWhiteSpace(Field))
                    return default;

                return GetField();
            }

            set { _fragment = value; }
        }

        [HtmlAttributeName("prismic-document")]
        public Document Document { get; set; }

        [HtmlAttributeName("prismic-field")]
        public string Field { get; set; }

        protected abstract TFragment GetField();

    }
}
