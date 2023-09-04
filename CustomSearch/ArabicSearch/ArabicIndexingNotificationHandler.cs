﻿using Examine;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services.Changes;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Sync;
using Umbraco.Cms.Core;
using Umbraco.Cms.Infrastructure.Search;
using Umbraco.Cms.Infrastructure;

namespace UmbracoStarterProject.CustomSearch.ArabicSearch
{
    public class ArabicIndexingNotificationHandler : INotificationHandler<ContentCacheRefresherNotification>
    {
        private readonly IRuntimeState _runtimeState;
        private readonly IUmbracoIndexingHandler _umbracoIndexingHandler;
        private readonly IExamineManager _examineManager;
        private readonly IContentService _contentService;
        private readonly ArabicIndexValueSetBuilder _arabicIndexValueSetBuilder;
        public ArabicIndexingNotificationHandler(IRuntimeState runtimeState, IUmbracoIndexingHandler umbracoIndexingHandler, IExamineManager examineManager, IContentService contentService, ArabicIndexValueSetBuilder arabicIndexValueSetBuilder)
        {
            _runtimeState = runtimeState;
            _umbracoIndexingHandler = umbracoIndexingHandler;
            _examineManager = examineManager;
            _contentService = contentService;
            _arabicIndexValueSetBuilder = arabicIndexValueSetBuilder;
        }
        /// <summary>
        ///     Updates the index based on content changes.
        /// </summary>
        public void Handle(ContentCacheRefresherNotification notification)
        {
            if (NotificationHandlingIsDisabled())
            {
                return;
            }

            if (!_examineManager.TryGetIndex("ArabicIndex", out IIndex? index))
            {
                throw new InvalidOperationException("Could not obtain the Arabic index");
            }

            ContentCacheRefresher.JsonPayload[] payloads = GetNotificationPayloads(notification);

            foreach (ContentCacheRefresher.JsonPayload payload in payloads)
            {
                // Remove
                if (payload.ChangeTypes.HasType(TreeChangeTypes.Remove))
                {
                    index.DeleteFromIndex(payload.Id.ToString());
                }
                // Reindex
                else if (payload.ChangeTypes.HasType(TreeChangeTypes.RefreshNode) ||
                         payload.ChangeTypes.HasType(TreeChangeTypes.RefreshBranch))
                {
                    IContent? content = _contentService.GetById(payload.Id);
                    if (content == null || content.Trashed)
                    {
                        index.DeleteFromIndex(payload.Id.ToString());
                        continue;
                    }

                    IEnumerable<ValueSet> valueSets = _arabicIndexValueSetBuilder.GetValueSets(content);
                    index.IndexItems(valueSets);
                }
            }
        }

        private bool NotificationHandlingIsDisabled()
        {
            // Only handle events when the site is running.
            if (_runtimeState.Level != RuntimeLevel.Run)
            {
                return true;
            }

            if (_umbracoIndexingHandler.Enabled == false)
            {
                return true;
            }

            if (Suspendable.ExamineEvents.CanIndex == false)
            {
                return true;
            }

            return false;
        }

        private ContentCacheRefresher.JsonPayload[] GetNotificationPayloads(CacheRefresherNotification notification)
        {
            if (notification.MessageType != MessageType.RefreshByPayload ||
                notification.MessageObject is not ContentCacheRefresher.JsonPayload[] payloads)
            {
                throw new NotSupportedException();
            }

            return payloads;
        }
    }
}
