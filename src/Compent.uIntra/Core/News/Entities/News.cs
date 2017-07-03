﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using uIntra.CentralFeed;
using uIntra.Comments;
using uIntra.Core.Activity;
using uIntra.Likes;
using uIntra.News;
using uIntra.Subscribe;

namespace Compent.uIntra.Core.News.Entities
{
    public class News : NewsBase, ICentralFeedItem, ICommentable, ILikeable, ISubscribable
    {
        [JsonIgnore]
        public DateTime SortDate => PublishDate;
        [JsonIgnore]
        public IEnumerable<LikeModel> Likes { get; set; }
        [JsonIgnore]
        public IEnumerable<Comment> Comments { get; set; }
        [JsonIgnore]
        public IEnumerable<global::uIntra.Subscribe.Subscribe> Subscribers { get; set; }

    }
}