﻿using System;
using System.Collections.Generic;

namespace Uintra.CentralFeed
{
    public class FeedOverviewModel
    {
        public IEnumerable<ActivityFeedTabViewModel> Tabs { get; set; }
        public IEnumerable<ActivityFeedTabViewModel> TabsWithCreateUrl { get; set; }
        public Enum CurrentType { get; set; }
        public bool IsFiltersOpened { get; set; }
    }
}