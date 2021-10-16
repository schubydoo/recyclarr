﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json.Linq;
using TrashLib.Config;
using TrashLib.Sonarr.Api.Objects;

namespace TrashLib.Sonarr.Api
{
    public class SonarrApi : ISonarrApi
    {
        private readonly ISonarrReleaseProfileCompatibilityHandler _profileHandler;
        private readonly IServerInfo _serverInfo;

        public SonarrApi(IServerInfo serverInfo, ISonarrReleaseProfileCompatibilityHandler profileHandler)
        {
            _serverInfo = serverInfo;
            _profileHandler = profileHandler;
        }

        public async Task<IList<SonarrTag>> GetTags()
        {
            return await BaseUrl()
                .AppendPathSegment("tag")
                .GetJsonAsync<List<SonarrTag>>();
        }

        public async Task<SonarrTag> CreateTag(string tag)
        {
            return await BaseUrl()
                .AppendPathSegment("tag")
                .PostJsonAsync(new {label = tag})
                .ReceiveJson<SonarrTag>();
        }

        public async Task<IList<SonarrReleaseProfile>> GetReleaseProfiles()
        {
            var response = await BaseUrl()
                .AppendPathSegment("releaseprofile")
                .GetJsonAsync<List<JObject>>();

            return response
                .Select(_profileHandler.CompatibleReleaseProfileForReceiving)
                .ToList();
        }

        public async Task UpdateReleaseProfile(SonarrReleaseProfile profileToUpdate)
        {
            await BaseUrl()
                .AppendPathSegment($"releaseprofile/{profileToUpdate.Id}")
                .PutJsonAsync(_profileHandler.CompatibleReleaseProfileForSending(profileToUpdate));
        }

        public async Task<SonarrReleaseProfile> CreateReleaseProfile(SonarrReleaseProfile newProfile)
        {
            var response = await BaseUrl()
                .AppendPathSegment("releaseprofile")
                .PostJsonAsync(_profileHandler.CompatibleReleaseProfileForSending(newProfile))
                .ReceiveJson<JObject>();

            return _profileHandler.CompatibleReleaseProfileForReceiving(response);
        }

        public async Task<IReadOnlyCollection<SonarrQualityDefinitionItem>> GetQualityDefinition()
        {
            return await BaseUrl()
                .AppendPathSegment("qualitydefinition")
                .GetJsonAsync<List<SonarrQualityDefinitionItem>>();
        }

        public async Task<IList<SonarrQualityDefinitionItem>> UpdateQualityDefinition(
            IReadOnlyCollection<SonarrQualityDefinitionItem> newQuality)
        {
            return await BaseUrl()
                .AppendPathSegment("qualityDefinition/update")
                .PutJsonAsync(newQuality)
                .ReceiveJson<List<SonarrQualityDefinitionItem>>();
        }

        private string BaseUrl() => _serverInfo.BuildUrl();
    }
}
