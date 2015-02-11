using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.ServiceLocation;
using EPiServer.Shell;

namespace Beque.Find.Widget.UIDescriptors.ViewConfigurations
{
    [ServiceConfiguration(typeof(ViewConfiguration))]
    public class FilterCatalogListView : ViewConfiguration<RootContent>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterCatalogListView"/> class.
        /// </summary>
        public FilterCatalogListView()
        {
            Key = "cataloglist";
            LanguagePath = "/commerce/contentediting/views/cataloglist";
            ControllerType = "beque/widget/filtercataloglist";
            IconClass = "epi-iconList";
            SortOrder = 10;
            Category = "catalog";
        }
    }
}
