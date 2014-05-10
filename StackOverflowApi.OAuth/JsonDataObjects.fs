namespace StackOverflowApi

// See the docs here: http://json.codeplex.com/
//                    http://james.newtonking.com/json/help/index.html
module JsonDataObjects =
   open Newtonsoft.Json

   [<JsonObject>]
   [<NoComparison>]
   type JsonStackExchangeCollection<'a> = {
      [<field: JsonProperty("items")>] Id : 'a[]
      [<field: JsonProperty("has_more")>] HasMore : bool
      [<field: JsonProperty("quota_max")>] QuotaMax : int
      [<field: JsonProperty("quota_remaining")>] QuotaRemaining : int
   }

   [<JsonObject>]
   [<NoComparison>]
   type JsonOwner = {
      [<field: JsonProperty("reputation")>] Reputation : string
      [<field: JsonProperty("user_id")>] UserId : string
      [<field: JsonProperty("user_type")>] UserType : string
      [<field: JsonProperty("profile_image")>] ProfileImageUrl : string
      [<field: JsonProperty("display_name")>] DisplayName : string
      [<field: JsonProperty("link")>] Link: string
   }

   [<JsonObject>]
   [<NoComparison>]
   type JsonSearchResultItem = {
      [<field: JsonProperty("tags")>] Tags : string[]
      [<field: JsonProperty("owner")>] Owner : JsonOwner
      [<field: JsonProperty("is_answered")>] IsAnswered : bool
      [<field: JsonProperty("view_count")>] ViewCount : int
      [<field: JsonProperty("accepted_answer_id")>] AcceptedAnswerId : string
      [<field: JsonProperty("answer_count")>] AnswerCount: int
      [<field: JsonProperty("score")>] Score : int
      [<field: JsonProperty("last_activity_date")>] LastActivityDate : string
      [<field: JsonProperty("creation_date")>] CreationDate : string
      [<field: JsonProperty("last_edit_date")>] LastEditDate : string
      [<field: JsonProperty("question_id")>] QuestionId : string
      [<field: JsonProperty("link")>] Link: string
      [<field: JsonProperty("title")>] Title: string
   }

   // http://api.stackexchange.com/docs/types/styling
   [<JsonObject>]
   [<NoComparison>]
   type JsonStylingItem = {
      [<field: JsonProperty("link_color")>] LinkColour : string
      [<field: JsonProperty("tag_background_color")>] TagBackgroundColour : string
      [<field: JsonProperty("tag_foreground_color")>] TagForegroundColour : string
   }

   // http://api.stackexchange.com/docs/types/site
   [<JsonObject>]
   [<NoComparison>]
   type JsonSiteItem = {
      [<field: JsonProperty("aliases") >]
      [<field: JsonConverter(typeof<WebUtilities.JsonConverters.ToOptionConverter>)>]
      Aliases : string[] option
            
      [<field: JsonProperty("api_site_parameter")>] ApiSiteParameter : string
      [<field: JsonProperty("audience")>] Audience : string
//            [<field: JsonProperty("closed_beta_data")>] ClosedBetaDate : System.DateTime option
      [<field: JsonProperty("favicon_url")>] FavIconUrl : string

      [<field: JsonProperty("high_resolution_icon_url")>]
      [<field: JsonConverter(typeof<WebUtilities.JsonConverters.ToOptionConverter>)>]
      HighResolutionIconUrl : string option

      [<field: JsonProperty("icon_url")>] IconUrl : string
            
      [<field: JsonProperty("launch_date")>] 
      [<field: JsonConverter(typeof<WebUtilities.JsonConverters.FromUnixEpochToDateTimeConverter>) >]
      LaunchDate : System.DateTime
            
      [<field: JsonProperty("logo_url")>] LogoUrl : string

      [<field: JsonProperty("markdown_extensions") >]
      [<field: JsonConverter(typeof<WebUtilities.JsonConverters.ToOptionConverter>)>]
      MarkdownExtensions : string[] option
            
      [<field: JsonProperty("name")>] Name : string
            
//          [<field: JsonProperty("open_beta_date") >]
//          [<field: JsonConverter(typeof<WebUtilities.JsonConverters.ToOptionConverter>)>]
//          OpenBetaDate : System.DateTime option
            
      [<field: JsonProperty("related_sites") >]
      [<field: JsonConverter(typeof<WebUtilities.JsonConverters.ToOptionConverter>)>]
      RelatedSites : JsonSiteItem[] option
            
      [<field: JsonProperty("site_state")>] SiteState : string
      [<field: JsonProperty("site_type")>] SiteType : string
      [<field: JsonProperty("site_url")>] SiteUrl : string
      [<field: JsonProperty("styling")>] Styling : JsonStylingItem

      [<field: JsonProperty("twitter_account") >]
      [<field: JsonConverter(typeof<WebUtilities.JsonConverters.ToOptionConverter>)>]
      TwitterAccount : string option
   }

   // https://api.stackexchange.com/docs/types/notification
   [<JsonObject>]
   [<NoComparison>]
   type JsonNotificationItem = {
      [<field: JsonProperty("body")>] Body : string
            
      [<field: JsonProperty("creation_date")>] 
      [<field: JsonConverter(typeof<WebUtilities.JsonConverters.FromUnixEpochToDateTimeConverter>) >]
      CreationDate : System.DateTime
            
      [<field: JsonProperty("is_unread")>] Unread : bool
      [<field: JsonProperty("notification_type")>] NotificationType : string

      [<field: JsonProperty("post_id") >]
      [<field: JsonConverter(typeof<WebUtilities.JsonConverters.ToOptionConverter>)>]
      PostId : int option
            
      [<field: JsonProperty("site")>] Site : string          
   }

   // https://api.stackexchange.com/docs/types/inbox-item
   [<JsonObject>]
   [<NoComparison>]
   type JsonInboxItem = {
      [<field: JsonProperty("site")>] Site : JsonSiteItem

      [<field: JsonProperty("answer_id") >]
      [<field: JsonConverter(typeof<WebUtilities.JsonConverters.ToOptionConverter>)>]
      AnswerId : int option
            
      [<field: JsonProperty("body") >]
      [<field: JsonConverter(typeof<WebUtilities.JsonConverters.ToOptionConverter>)>]
      Body : string option
            
      [<field: JsonProperty("comment_id") >]
      [<field: JsonConverter(typeof<WebUtilities.JsonConverters.ToOptionConverter>)>]
      CommentId : int option
            
      [<field: JsonProperty("creation_date")>] 
      [<field: JsonConverter(typeof<WebUtilities.JsonConverters.FromUnixEpochToDateTimeConverter>) >]
      CreationDate : System.DateTime
            
      [<field: JsonProperty("is_unread")>] IsUnread: bool
      [<field: JsonProperty("item_type")>] ItemType : string
      [<field: JsonProperty("link")>] Link : string
            
      [<field: JsonProperty("question_id") >]
      [<field: JsonConverter(typeof<WebUtilities.JsonConverters.ToOptionConverter>)>]
      QuestionId : int option
            
      [<field: JsonProperty("title")>] Title : string
   }
