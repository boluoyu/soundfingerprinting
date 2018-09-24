namespace SoundFingerprinting.DAO
{
    using System;
    using System.Collections.Generic;

    using SoundFingerprinting.Configuration;
    using SoundFingerprinting.Data;

    public interface ISubFingerprintDao
    {
        void InsertHashDataForTrack(IEnumerable<HashedFingerprint> hashes, IModelReference trackReference);

        [Obsolete]
        IList<HashedFingerprint> ReadHashedFingerprintsByTrackReference(IModelReference trackReference);

        FingerprintsQueryResponse ReadSubFingerprints(IEnumerable<QueryHash> hashes, QueryConfiguration queryConfiguration);
    }
}