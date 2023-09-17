using System;

namespace TeamB.Scripts.Common.API
{
    public static class Models
    {
        [Serializable]
        public class GeoIP
        {
            public string organization;
            public double longitude;
            public string city;
            public string timezone;
            public string isp;
            public int asn;
            public string asn_organization;
            public string country;
            public string ip;
            public double latitude;
            public string postal_code;
            public string continent_code;
            public string country_code;
            public string region_code;
        }

        [Serializable]
        public class LoginRequestBody
        {
            public string id;
        }

        [Serializable]
        public class SignUpRequestBody
        {
            public string user_name;
        }

        [Serializable]
        public class User
        {
            public string user_id;
            public string user_name;
            public uint score;
        }

        [Serializable]
        public class CreateUserBody
        {
            public string user_name;
        }

        [Serializable]
        public class UpdateUserBody
        {
            public string user_name;
        }


        [Serializable]
        public class VoteRanking
        {
            public string id;
            public uint vote_count;
            public string character_name;

            public VoteRanking(string charaId, uint voteCount, string characterName)
            {
                id = charaId;
                vote_count = voteCount;
                character_name = characterName;
            }
        }

        [Serializable]
        public class ScoreRanking
        {
            public string user_id;
            public string user_name;
            public uint score;
        }

        [Serializable]
        public class UpdateScoreRankingBody
        {
            public string character_id;
            public uint score;
        }
    }
}