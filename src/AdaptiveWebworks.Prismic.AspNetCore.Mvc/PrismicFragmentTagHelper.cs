using Microsoft.AspNetCore.Razor.TagHelpers;
using prismic;

namespace AdaptiveWebworks.Prismic.AspNetCore.Mvc
{
    public abstract class PrismicFragmentTagHelper<TFragment> : TagHelper
        where TFragment : IFragment
    {
        private TFragment _fragment;

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

        public Document Document { get; set; }

        public string Field { get; set; }

        protected abstract TFragment GetField();

    }
}
