
namespace EPiTube.FasetFilter.Core.DataAnnotation
{
    public class TextboxFilterAttribute : FasetFilterAttribute
    {
        public TextboxFilterAttribute()
            : this(100)
        {
        }

        public TextboxFilterAttribute(int delay)
        {
            FilterPath = "epitubefasetfilter/widget/TextboxFasetFilter";
            Delay = delay;
        }

        public int Delay { get; set; }
    }
}
