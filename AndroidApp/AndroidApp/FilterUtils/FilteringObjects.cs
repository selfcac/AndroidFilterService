using System;
using System.Collections.Generic;
using System.Text;

namespace AndroidApp.FilterUtils
{
    static class FilteringObjects
    {
        public static bool isFiltering = true; // Should we filter with http\time?
        public static bool isInWifiBlockZone = true;

        public static HTTPProtocolFilter.FilterPolicy httpPolicy = new HTTPProtocolFilter.FilterPolicy();
        public static TimeBlockFilter.TimeFilterObject timePolicy = new TimeBlockFilter.TimeFilterObject();
    }
}
