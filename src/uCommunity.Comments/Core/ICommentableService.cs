﻿using System;

namespace uCommunity.Comments
{
    public interface ICommentableService
    {
        void CreateComment(Guid userId, Guid activityId, string text, Guid? parentId);

        void UpdateComment(Guid id, string text);

        void DeleteComment(Guid id);

        ICommentable GetCommentsInfo(Guid activityId);
    }
}
