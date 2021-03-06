﻿using Newbe.Mahua.MahuaEvents;
using System;

namespace Newbe.Mahua.Plugins.iChunqiuQQBoot.Beta.MahuaEvents
{
    /// <summary>
    /// 入群邀请接收事件
    /// </summary>
    public class GroupJoiningInvitationReceivedMahuaEvent
        : IGroupJoiningInvitationReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public GroupJoiningInvitationReceivedMahuaEvent(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessJoinGroupRequest(GroupJoiningRequestReceivedContext context)
        {
            // 自动同意入群邀请
            _mahuaApi.AcceptGroupJoiningInvitation(context.GroupJoiningRequestId,context.ToGroup,context.FromQq);
        }
    }
}
