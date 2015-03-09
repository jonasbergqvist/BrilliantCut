
namespace EPiTube.FasetFilter.Core.Settings
{
    public class TextboxFilterSetting : FasetFilterSetting
    {
        public TextboxFilterSetting()
            : this(1000)
        {
        }

        public TextboxFilterSetting(int delay)
        {
            FilterPath = "epitubefasetfilter/widget/TextboxFasetFilter";
            Delay = delay;
        }

        public int Delay { get; set; }
    }
}
