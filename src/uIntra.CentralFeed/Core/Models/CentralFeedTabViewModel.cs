﻿using uIntra.Core.TypeProviders;

namespace uIntra.CentralFeed
{
    public class CentralFeedTabViewModel
    {
        public IIntranetType Type { get; set; }       
        public string CreateUrl { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
    }
}
