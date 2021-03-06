﻿namespace SoundFingerprinting
{
    using System.Collections.Generic;
    using System.Linq;

    using SoundFingerprinting.Configuration;
    using SoundFingerprinting.DAO;
    using SoundFingerprinting.DAO.Data;
    using SoundFingerprinting.Data;

    public abstract class ModelService : IModelService
    {
        protected ModelService(string id, ITrackDao trackDao, ISubFingerprintDao subFingerprintDao)
        {
            Id = id;
            TrackDao = trackDao;
            SubFingerprintDao = subFingerprintDao;
        }

        public virtual IEnumerable<ModelServiceInfo> Info => new[] { new ModelServiceInfo(Id, TrackDao.Count, SubFingerprintDao.SubFingerprintsCount, SubFingerprintDao.HashCountsPerTable.ToArray()) };

        protected string Id { get; }
        
        protected ITrackDao TrackDao { get; }
        
        protected ISubFingerprintDao SubFingerprintDao { get; }

        public virtual void Insert(TrackInfo trackInfo, Hashes hashes)
        {
            var fingerprints = hashes.ToList();
            if (!fingerprints.Any())
            {
                return;
            }

            var trackReference = TrackDao.InsertTrack(trackInfo, hashes.DurationInSeconds).TrackReference;
            SubFingerprintDao.InsertHashDataForTrack(fingerprints, trackReference);
        }

        public virtual IEnumerable<SubFingerprintData> Query(IEnumerable<int[]> hashes, QueryConfiguration config)
        {
            var queryHashes = hashes.ToList();
            return queryHashes.Any() ? SubFingerprintDao.ReadSubFingerprints(queryHashes, config) : Enumerable.Empty<SubFingerprintData>();
        }

        public virtual IEnumerable<TrackData> ReadAllTracks()
        {
            return TrackDao.ReadAll();
        }

        public virtual IEnumerable<TrackData> ReadTrackByTitle(string title)
        {
            return TrackDao.ReadTrackByTitle(title);
        }

        public virtual IEnumerable<TrackData> ReadTracksByReferences(IEnumerable<IModelReference> references)
        {
            return TrackDao.ReadTracksByReferences(references);
        }

        public virtual TrackInfo ReadTrackById(string trackId)
        {
            var trackData = TrackDao.ReadTrackById(trackId);
            if (trackData == null)
            {
                return null;
            }

            var metaFields = CopyMetaFields(trackData.MetaFields);
            metaFields.Add("TrackLength", $"{trackData.Length: 0.000}");
            return new TrackInfo(trackData.Id, trackData.Title, trackData.Artist, metaFields, trackData.MediaType);
        }

        public virtual int DeleteTrack(string trackId)
        {
            var track = TrackDao.ReadTrackById(trackId);
            if (track == null)
            {
                return 0;
            }

            var trackReference = track.TrackReference;
            return SubFingerprintDao.DeleteSubFingerprintsByTrackReference(trackReference) + TrackDao.DeleteTrack(trackReference);
        }

        private static IDictionary<string, string> CopyMetaFields(IDictionary<string, string> metaFields)
        {
            return metaFields == null ? new Dictionary<string, string>() : metaFields.ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}
