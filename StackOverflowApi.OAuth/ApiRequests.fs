namespace StackOverflowApi

module ApiRequests =
   open WebUtilities.WebRequest
   open WebUtilities.WebResponse

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


   module V2_2 =
      let baseApiUrl = "https://api.stackexchange.com/2.2/"

      module Notifications = 
         type Notifications() = interface IAuthenticatedApiRequest with member x.requestUrl() = baseApiUrl + "notifications"
         type UnreadNotifications = interface IAuthenticatedApiRequest with member x.requestUrl() = baseApiUrl + "notifications/unread"
         type NotificationssForUser(site, userId) = interface IAuthenticatedApiRequest with member x.requestUrl() = baseApiUrl + sprintf "users/%s/notifications?site=%s" userId site
         type UnreadNotificationsForUser(site, userId) = interface IAuthenticatedApiRequest with member x.requestUrl() = baseApiUrl + sprintf "users/%s/notifications/unread?site=%s" userId site
         type NotificationsForMe(site) = interface IAuthenticatedApiRequest with member x.requestUrl() = baseApiUrl + sprintf "me/notifications?site=%s" site
         type UnreadNotificationsForMe(site) = interface IAuthenticatedApiRequest with member x.requestUrl() = baseApiUrl + sprintf "me/notifications/unread?site=%s" site

//      type IApiRequest with member x.Url with get() = baseApiUrl + x.requestUrl()
//      type IAuthenticatedApiRequest with member x.Url with get() = baseApiUrl + x.requestUrl()
      
