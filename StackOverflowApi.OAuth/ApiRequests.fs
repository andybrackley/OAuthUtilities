namespace StackOverflowApi

module ApiRequests =
   open WebUtilities.WebRequest
   open WebUtilities.WebResponse

   type ReadStatus = All | UnreadOnly
   let readStatusAsUrlRequest = function All -> "" | UnreadOnly -> "/unread"

   type RequestType =
      | Mine
      | ForUser of string

   let requestTypeAsUrlRequest = function
      | Mine -> "/me"
      | ForUser(user) -> "/users/" + user


   type IApiRequest = abstract member requestUrl : unit -> string
   type IAuthenticatedApiRequest = inherit IApiRequest  

   let createApiRequest (request : IApiRequest) =
      createHttpRequest   { Url = request.requestUrl(); RequestType = GET }
         |> addHeader( "Accept-Encoding", "gzip,deflate")

   type ApiAccessToken = { ApiAccessToken : string ; ClientKey : string }
   let createAuthenticatedApiRequest accessToken (request : IAuthenticatedApiRequest) = 
      // Need to append the accessToken details to the Url
      createApiRequest 
         { 
            new IApiRequest 
               with member x.requestUrl() = 
                     let requestUrl = request.requestUrl() 
                     sprintf "%s%s%s" requestUrl  (if requestUrl.Contains("?") then "&" else "?") ( sprintf "access_token=%s&key=%s" accessToken.ApiAccessToken accessToken.ClientKey)
         }


   // TODO: Need to add: ?page=1&pagesize=20 to requests
   module V2_2 =
      let baseApiUrl = "https://api.stackexchange.com/2.2/"

      // See the docs here: http://json.codeplex.com/
      //                    http://james.newtonking.com/json/help/index.html
      open Newtonsoft.Json

      [<JsonObject>]
      [<NoComparison>]
      type StackExchangeCollection<'a> = {
         [<field: JsonProperty("items")>] Id : 'a[]
         [<field: JsonProperty("has_more")>] HasMore : bool
         [<field: JsonProperty("quota_max")>] QuotaMax : int
         [<field: JsonProperty("quota_remaining")>] QuotaRemaining : int
      }

      module Site =
         // http://api.stackexchange.com/docs/types/styling
         [<JsonObject>]
         [<NoComparison>]
         type StylingItem = {
            [<field: JsonProperty("link_color")>] LinkColour : string
            [<field: JsonProperty("tag_background_color")>] TagBackgroundColour : string
            [<field: JsonProperty("tag_foreground_color")>] TagForegroundColour : string
         }

         // http://api.stackexchange.com/docs/types/site
         [<JsonObject>]
         [<NoComparison>]
         type SiteItem = {
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
            RelatedSites : SiteItem[] option
            
            [<field: JsonProperty("site_state")>] SiteState : string
            [<field: JsonProperty("site_type")>] SiteType : string
            [<field: JsonProperty("site_url")>] SiteUrl : string
            [<field: JsonProperty("styling")>] Styling : StylingItem

            [<field: JsonProperty("twitter_account") >]
            [<field: JsonConverter(typeof<WebUtilities.JsonConverters.ToOptionConverter>)>]
            TwitterAccount : string option
         }

      module Notifications = 
         // https://api.stackexchange.com/docs/types/notification
         [<JsonObject>]
         [<NoComparison>]
         type NotificationItem = {
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

         let requestName = "Notifications"

         /// Creates a notification request object across all sites
         /// This is for the currently logged in user
         let CreateSiteAgnostic readStatus =
            let url = baseApiUrl + requestName + readStatusAsUrlRequest readStatus
            { new IAuthenticatedApiRequest with member x.requestUrl() = url }

         /// Creates a notification request object for the site specified
         /// and the user requested
         let Create site requestType readStatus =
            let url = 
               baseApiUrl 
               + requestTypeAsUrlRequest requestType 
               + requestName 
               + readStatusAsUrlRequest readStatus
               + "?site=" + site

            { new IAuthenticatedApiRequest with member x.requestUrl() = url }


//         type Notifications() = interface IAuthenticatedApiRequest with member x.requestUrl() = baseApiUrl + "notifications"
//         type UnreadNotifications = interface IAuthenticatedApiRequest with member x.requestUrl() = baseApiUrl + "notifications/unread"
//         type NotificationssForUser(site, userId) = interface IAuthenticatedApiRequest with member x.requestUrl() = baseApiUrl + sprintf "users/%s/notifications?site=%s" userId site
//         type UnreadNotificationsForUser(site, userId) = interface IAuthenticatedApiRequest with member x.requestUrl() = baseApiUrl + sprintf "users/%s/notifications/unread?site=%s" userId site
//         type NotificationsForMe(site) = interface IAuthenticatedApiRequest with member x.requestUrl() = baseApiUrl + sprintf "me/notifications?site=%s" site
//         type UnreadNotificationsForMe(site) = interface IAuthenticatedApiRequest with member x.requestUrl() = baseApiUrl + sprintf "me/notifications/unread?site=%s" site

      module Inbox =
         // https://api.stackexchange.com/docs/types/inbox-item
         [<JsonObject>]
         [<NoComparison>]
         type InboxItem = {
            [<field: JsonProperty("site")>] Site : Site.SiteItem

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

         let requestName = "Inbox"
         type Inbox(readStatus) = 
            interface IAuthenticatedApiRequest with member x.requestUrl() = baseApiUrl + requestName + readStatusAsUrlRequest readStatus  

         let sendRequest token ( request : Inbox ) = 
            createAuthenticatedApiRequest token request
               |> sendRequest Parsers.asJsonObject<StackExchangeCollection<InboxItem>>

//         type Inbox() = interface IAuthenticatedApiRequest with member x.requestUrl() = baseApiUrl + "Inbox"
//         type UnreadInbox() = interface IAuthenticatedApiRequest with member x.requestUrl() = baseApiUrl + "Inbox/Unread"

         // sendRequest rqst token =
         //    createAuthenticatedApiRequest rqst token
         //       |> sendRequest Parsers.fromJson<StackExchangeCollection<InboxItem>>

//      type IApiRequest with member x.Url with get() = baseApiUrl + x.requestUrl()
//      type IAuthenticatedApiRequest with member x.Url with get() = baseApiUrl + x.requestUrl()
      
