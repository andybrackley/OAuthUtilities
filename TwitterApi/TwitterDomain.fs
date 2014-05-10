namespace TwitterApi

module Json =
   open WebUtilities.JsonConverters
   open Newtonsoft.Json

   type UserId = int64
   type TweetId = int64

   [<JsonObject>]
   [<NoComparison>]
   type TweetEntityUser = {
      [<field: JsonProperty("name")>] Name : string
      [<field: JsonProperty("id_str")>] Id_Str : string
      [<field: JsonProperty("id")>] Id : UserId
      [<field: JsonProperty("indices")>] Indices : int list
      [<field: JsonProperty("screen_name")>] ScreenName : string
   }

   [<JsonObject>]
   [<NoComparison>]
   type TweetEntities = {
      [<field: JsonProperty("urls")>] Urls : string list 
      [<field: JsonProperty("hashtags")>] Hashtags : string list 
      [<field: JsonProperty("user_mentions")>] Mentions : TweetEntityUser
   }

   [<JsonObject>]
   [<NoComparison>]
   type UserDetails = {
      [<field: JsonProperty("expanded_url")>] 
      [<field: ToOptionConverter>]
      ExpandedUrl: string option
      
      [<field: JsonProperty("profile_sidebar_border_color")>] ProfileBorderColour: string
      [<field: JsonProperty("profile_background_tile")>] ProfileBackgroundTile: bool
      [<field: JsonProperty("profile_sidebar_fill_color")>] ProfileSidebarFillColour: string
      [<field: JsonProperty("name")>] Name: string
      [<field: JsonProperty("created_at")>] 
      [<field: JsonConverter(typeof<FromTwitterDate>)>]
      CreatedAt: System.DateTime
      
      [<field: JsonProperty("location")>] Location : string
      [<field: JsonProperty("profile_image_url")>] ProfileImageUrl : string
      [<field: JsonProperty("profile_link_color")>] ProfileLinkColour: string
      [<field: JsonProperty("id_str")>] IdStr : string
      [<field: JsonProperty("follow_request_sent")>] FollowRequestSent: bool
      [<field: JsonProperty("is_translator")>] IsTranslator: bool

      [<field: JsonProperty("favourites_count")>] FavouritesCount : int
      [<field: JsonProperty("contributors_enabled")>] ContributorsEnabled : bool
      [<field: JsonProperty("url")>]
      [<field: JsonConverter(typeof<ToOptionConverter>)>]
      Url : string option
      [<field: JsonProperty("default_profile")>] DefaultProfile : bool
      [<field: JsonProperty("id")>] Id : UserId
      
      [<field: JsonProperty("profile_image_url_https")>] ProfileImageUrlHttps: string
      [<field: JsonProperty("utc_offset")>] UtcOffset: int64
      [<field: JsonProperty("profile_use_background_image")>] ProfileUseBackgroundImage: bool
      [<field: JsonProperty("listed_count")>] ListedCount: int
      [<field: JsonProperty("lang")>] Lang: string
      [<field: JsonProperty("followers_count")>] FollowersCount: int64
      [<field: JsonProperty("profile_text_color")>] ProfileTextColour: string
      [<field: JsonProperty("protected")>] Protected: bool
      [<field: JsonProperty("profile_background_color")>] ProfileBackgroundColour: string
      [<field: JsonProperty("verified")>] Verified: bool
      [<field: JsonProperty("time_zone")>] TimeZone: string
      [<field: JsonProperty("description")>] Description: string
      [<field: JsonProperty("geo_enabled")>] GeoEnabled: bool
      [<field: JsonProperty("profile_background_image_url_https")>] ProfileBackgroundImageUrlHttps: string
      [<field: JsonProperty("notifications")>] Notifications: bool
      [<field: JsonProperty("default_profile_image")>] DefaultProfileImage: bool
      [<field: JsonProperty("statuses_count")>] StatusesCount: int64
      
      [<field: JsonProperty("display_url")>]
      [<field: JsonConverter(typeof<ToOptionConverter>)>]
      DisplayUrl: string option
      
      [<field: JsonProperty("friends_count")>] FriendsCount: int
      [<field: JsonProperty("profile_background_image_url")>] ProfileBackgroundImageUrl: string
      [<field: JsonProperty("following")>] Following: bool
      [<field: JsonProperty("show_all_inline_media")>] ShowAllInlineMedia: bool
      [<field: JsonProperty("screen_name")>] ScreenName: string

   }

   [<JsonObject>]
   [<NoComparison>]
   type Tweet = {
      [<field: JsonProperty("coordinates")>]
      [<field: JsonConverter(typeof<ToOptionConverter>)>]
      Coordinates : string option

      [<field: JsonProperty("created_at")>] 
      [<field: JsonConverter(typeof<FromTwitterDate>)>]
      CreatedAt : System.DateTime
      
      [<field: JsonProperty("favorited")>] Favorited : bool
      [<field: JsonProperty("truncated")>] Truncated : bool
      [<field: JsonProperty("id_str")>] Id_Str : string

      [<field: JsonProperty("in_reply_to_user_id_str")>] InReplyToUserIdStr : string

      [<field: JsonProperty("contributors")>]
      [<field: JsonConverter(typeof<ToOptionConverter>)>]
      Contributors : string option

      [<field: JsonProperty("text")>] Text : string
      [<field: JsonProperty("id")>] Id : TweetId
      [<field: JsonProperty("in_reply_to_status_id_str")>] ReplyToStatusIdStr : string
      [<field: JsonProperty("retweet_count")>] RetweetCount : int

      [<field: JsonProperty("geo")>] 
      [<field: JsonConverter(typeof<ToOptionConverter>)>]
      Geo : string option

      [<field: JsonProperty("retweeted")>] Retweeted : bool
      [<field: JsonProperty("in_reply_to_user_id")>] InReplyToUserId : UserId
      [<field: JsonProperty("source")>] Source : string
      [<field: JsonProperty("in_reply_to_screen_name")>] InReplyToScreenName : string 
      [<field: JsonProperty("user")>] User : UserDetails
      [<field: JsonProperty("in_reply_to_status_id")>] ReplyToStatusId : TweetId
   }

//   [<JsonObject>]
//   [<NoComparison>]
//   type TweetList = {
//      
//   }