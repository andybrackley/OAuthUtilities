namespace StackOverflowApi
    // https://stackapps.com/apps/oauth/view/2773
    // http://api.stackoverflow.com/1.0/usage
    // http://api.stackexchange.com/docs/authentication    
    // http://api.stackexchange.com/docs

module OAuth =
   open System

   open WebUtilities.PatternMatching
   open WebUtilities.WebRequest
   open WebUtilities.WebResponse

   // Not sure what types a client id can be
   // In Pocket and StackExchange it's an int
   type ClientId<'a> = ClientId of 'a
   let clientIdAsString = function ClientId(x) -> sprintf "%A" x
      
   type AuthCode = AuthCode of string
   let authCodeAsString = function AuthCode(x) -> sprintf "%s" x
         
   type AccessToken = { Token : string; Expires : int }

   let parseAccessToken (response : string) = 
      let response = response.Split([|'='; '&'|])
      { 
         Token = response.[1]; 
         Expires = Int32.Parse(response.[3])
      }

   type OAuthAppSettings = { 
      ClientId : ClientId<int> 
      ClientSecret : string 
      ClientKey : string
      RedirectUrl : string 
   }
      
   let OAuthUrl = "https://stackexchange.com/oauth"
   let AccessTokenUrl = "https://stackexchange.com/oauth/access_token"
   let PostEncodingType = UrlFormEncoded
      
   let buildRequestUrl oAuthParams =
      let scope = "read_inbox,private_info"
      OAuthUrl + "?" + "client_id=" + (oAuthParams.ClientId |> clientIdAsString) +
                  "&" + "scope=" + scope + 
                  "&" + "redirect_uri=" + oAuthParams.RedirectUrl

   let getAccessCodeFromUrl (redirectUrl : Uri) =
      match redirectUrl.PathAndQuery with
      | StartsWith("/?code=")(rest) -> Some(AuthCode(rest))
      | _ -> None
         
   let sendAccessRequest oAuthParams accessToken =
      async {
         let post_data = 
            "client_id=" + (oAuthParams.ClientId |> clientIdAsString) 
            + "&" + "client_secret=" + oAuthParams.ClientSecret
            + "&" + "code=" + (accessToken |> authCodeAsString)
            + "&" + "redirect_uri=" + oAuthParams.RedirectUrl

         let! request = createHttpPostRequest { Url = AccessTokenUrl ; PostRequestType = POST(PostEncodingType, post_data) }
         return! sendRequest parseAccessToken request
   }

//   module Notifications =
//      let unreadUrl = "/2.2/notifications/unread"

         
