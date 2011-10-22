
twicseratops is a twitter library for c# like a rubytter.rb

# license

 MIT License

# compile

    $ rake

# code

## how to get access token
    
    using System;
    using BasyuraOrg.Twitter;

    class Class1 {
        static void Main(string[] args) {
            TwicseraRegister register = Twicseratops.NewRegister();
            string url = register.GetAuthorizeUrl();

            Console.WriteLine("access url to get pin");
            Console.WriteLine(url);
            Console.Write("pin：");
            string pin = Console.ReadLine().Trim();

            string[] keys = register.GetAccessTokenAndSecret(pin);

            Console.WriteLine("access token        : " + keys[0]);
            Console.WriteLine("access token secret : " + keys[1]);
        }
    }


## how to get timeline

    using System;
    using System.Collections.Generic;
    using Twicseratops;

    class Client {
        static void Main(string[] args) {

            string accessToken       = "your access token";
            string accessTokenSecret = "your access token secret";

            dynamic twitter = new Twicseratops(accessToken, accessTokenSecret);
            // parameter
            Dictionary<string, string> param = new Dictionary<string, string> {
                {"per_page" , "100"}
            };
            // get list statuses
            foreach (dynamic status in twitter.ListStatuses("basyura" , "all" , param)) {
                Console.WriteLine(status.user.screen_name.PadRight(15 , ' ') + " : " + status.text);
            }
            Console.WriteLine("----------------------------");
            // get replies
            foreach (dynamic status in twitter.Replies()) {
                Console.WriteLine(status.user.screen_name.PadRight(15 , ' ') + " : " + status.text);
            }
            Console.WriteLine("----------------------------");
            // get home timeline
            foreach (dynamic status in twitter.HomeTimeline()) {
                Console.WriteLine(status.user.screen_name.PadRight(15 , ' ') + " : " + status.text);
            }
            Console.WriteLine("----------------------------");
            // tweet my status
            twitter.Update("(=^・^=)");
        }
    }

# supported? api

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

