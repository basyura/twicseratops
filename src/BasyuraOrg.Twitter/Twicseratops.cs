using System;
using System.IO;
using System.Collections.Generic;
using System.Dynamic;
using Codeplex.Data;
using BasyuraOrg.Twitter;
using System.Text.RegularExpressions;

namespace BasyuraOrg.Twitter {
    /**
     *
     */
    public class Twicseratops : DynamicObject {
        /** */
        protected const string CONSUMER_KEY    = "IGWZ6nY3v3cHBh0yfj6RJw";
        /** */
        protected const string CONSUMER_SECRET = "zfwTlwwjGSAKChGwJi5DpJSBsIlZ7HE3ZCMUfelCk";
        /** */
        private const string API_URL  = "https://api.twitter.com/1";
        /** */
        const string APIS = @"
          UpdateStatus          /statuses/update                POST
          RemoveStatus          /statuses/destroy/{0}           DELETE
          PublicTimeline        /statuses/public_timeline
          HomeTimeline          /statuses/home_timeline
          FriendsTimeline       /statuses/friends_timeline
          Replies               /statuses/replies
          Mentions              /statuses/mentions
          UserTimeline          /statuses/user_timeline/{0}
          Show                  /statuses/show/{0}
          Friends               /statuses/friends/{0}
          Followers             /statuses/followers/{0}
          Retweet               /statuses/retweet/{0}           POST
          Retweets              /statuses/retweets/{0}
          RetweetedByMe         /statuses/retweeted_by_me
          RetweetedToMe         /statuses/retweeted_to_me
          RetweetsOfMe          /statuses/retweets_of_me
          User                  /users/show/{0}
          DirectMessages        /direct_messages
          SentDirectMessages    /direct_messages/sent
          SendDirectMessage     /direct_messages/new            POST
          RemoveDirectMessage   /direct_messages/destroy/{0}    DELETE
          Follow                /friendships/create/{0}         POST
          Leave                 /friendships/destroy/{0}        DELETE
          FriendshipExists      /friendships/exists
          FollowersIds          /followers/ids/{0}
          FriendsIds            /friends/ids/{0}
          Favorites             /favorites/{0}
          Favorite              /favorites/create/{0}           POST
          RemoveFavorite        /favorites/destroy/{0}          DELETE
          VerifyCredentials     /account/verify_credentials     GET
          EndSession            /account/end_session            POST
          UpdateDeliveryDevice  /account/update_delivery_device POST
          UpdateProfileColors   /account/update_profile_colors  POST
          LimitStatus           /account/rate_limit_status
          UpdateProfile         /account/update_profile         POST
          EnableNotification    /notifications/follow/{0}       POST
          DisableNotification   /notifications/leave/{0}        POST
          Block                 /blocks/create/{0}              POST
          Unblock               /blocks/destroy/{0}             DELETE
          BlockExists           /blocks/exists/{0}              GET
          Blocking              /blocks/blocking                GET
          BlockingIds           /blocks/blocking/ids            GET
          SavedSearches         /saved_searches                 GET
          SavedSearch           /saved_searches/show/{0}        GET
          CreateSavedSearch     /saved_searches/create          POST
          RemoveSavedSearch     /saved_searches/destroy/{0}     DELETE
          CreateList            /{0}/lists                      POST
          UpdateList            /{0}/lists/{1}                  PUT
          DELETEList            /{0}/lists/{1}                  DELETE
          List                  /{0}/lists/{1}
          Lists                 /{0}/lists
          ListsFollowers        /{0}/lists/memberships
          ListStatuses          /{0}/lists/{1}/statuses
          ListMembers           /{0}/{1}/members
          AddMemberToList       /{0}/{1}/members                POST
          RemoveMemberFromList  /{0}/{1}/members                DELETE
          ListFollowing         /{0}/{1}/subscribers
          FollowList            /{0}/{1}/subscribers            POST
          RemoveList            /{0}/{1}/subscribers            DELETE
          ";

        private static Dictionary<string, string> API_MAP = new Dictionary<string, string>();
        /**
         *
         */
        static Twicseratops() {
            Regex regex = new Regex(@"\s+");
            foreach (string line in APIS.Split('\n')) {
                if (String.IsNullOrEmpty(line.Trim())) {
                    continue;
                }
                string[] api = regex.Split(line.Trim());
                API_MAP[api[0]] = API_URL + api[1] + ".json";
                string method = "GET";
                if (api.Length > 2) {
                    method = api[2];
                }
                API_MAP[api[0] + ":method"] = method;
            }
        }
        /** */
        private TwicseraAuth auth_ = null;
        /**
         *
         */
        public Twicseratops(string accessToken, string accessTokenSecret) {
            auth_ = new TwicseraAuth(CONSUMER_KEY, CONSUMER_SECRET, accessToken, accessTokenSecret);
        }
        /**
         *
         */
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result) {
           string api = binder.Name; 
            if (!API_MAP.ContainsKey(api)) {
                result = null;
                return false;
            }
           result = Request(api, args);
           return true;
        }
        /*
         *
         */
        private dynamic Request(string api , object[] args) {

            Dictionary<string, string> inParam = new Dictionary<string, string>();
            if (args.Length != 0 && args[args.Length - 1] is Dictionary<string, string>) {
                inParam = (Dictionary<string, string>)args[args.Length - 1];
                Array.Resize(ref args, args.Length - 1);
            }

            string url = String.Format(API_MAP[api] , args);

            if ("GET" == API_MAP[api + ":method"]) {
                return auth_.HttpGet(url  , inParam);
            }
            else {
                return auth_.HttpPost(url , inParam);
            }
        }
        /*
         *
         */
        public dynamic Update(string status) {
            Dictionary<string, string> param = new Dictionary<string, string>() {
                {"status" , status}
            };
            return Request("UpdateStatus", new object[]{param});
        }
        /*
         *
         */
        public static AuthRegister NewRegister() {
            return new AuthRegister(CONSUMER_KEY, CONSUMER_SECRET);
        }
    }
}
