﻿namespace SoundFingerprinting.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ProtoBuf;

    [Serializable]
    [ProtoContract]
    public class FingerprintsQueryResponse
    {
        public FingerprintsQueryResponse(IEnumerable<QueryResponseMatch> matches)
        {
            Matches = matches;
        }

        [ProtoMember(1)]
        public IEnumerable<QueryResponseMatch> Matches { get; }

        public bool IsEmpty
        {
            get
            {
                return Matches == null || !Matches.Any();
            }
        }
    }
}
