namespace TwitterApi

// https://apps.twitter.com/app/5983298
[<AutoOpen>]
module TwitterApi =

   open System
   open PCLWebUtility
   
   let currentUnixTime() = floor (DateTime.UtcNow - DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds

   let apiBase = "https://api.twitter.com"

   type OAuthAppSettings = {
      ClientKey : string
      ClientSecret : string
   }

   type AccessToken = { Token : string ; TokenSecret : string ; CallbackConfirmed : bool }
   let twitterSettings = { 
      ClientKey = TwitterApiSettings.apiKey 
      ClientSecret = TwitterApiSettings.apiSecret 
   }

   type AccessRequestToken = { ClientSecret : string; TokenSecret : string }
   let toUrlParam token = WebUtility.UrlEncode(token.ClientSecret) + "&" + WebUtility.UrlEncode(token.TokenSecret)

   module Authentication =

      open System
      open TwitterApi.Crypto

      open WebUtilities.PatternMatching
      open WebUtilities.WebRequest
      open WebUtilities.WebResponse

      let requestToken = "/oauth/request_token"
      let accessToken  = "/oauth/access_token"
      let authorizeUrl = "/oauth/authorize"
      let redirectUrl  = "http://www.twittyroo.co.uk/callback/"

      let PostEncodingType = UrlFormEncoded

      let parseRequestResponse (response: string) =
         let response = response.Split([| '='; '&' |])
         {
            Token = response.[1]
            TokenSecret = response.[3]
            CallbackConfirmed = response.[5] = "true"
         }

      let baseString httpMethod baseUri queryParameters = 
       httpMethod + "&" + 
       WebUtility.UrlEncode(baseUri) + "&" +
       (queryParameters 
        |> Seq.sortBy (fun (k,v) -> k)
        |> Seq.map (fun (k,v) -> WebUtility.UrlEncode(k) + "%3D" + WebUtility.UrlEncode(v))
        |> String.concat "%26") 

      let createAuthorizeHeader queryParameters = 
          let headerValue = 
              "OAuth " + 
              (queryParameters
               |> Seq.map (fun (k,v) -> WebUtility.UrlEncode(k)+"\x3D\""+ WebUtility.UrlEncode(v)+"\"")
               |> String.concat ",")
          headerValue

      let obtainRequestToken = 
         let signingKey = { ClientSecret = twitterSettings.ClientSecret; TokenSecret = "" } |> toUrlParam

         let queryParameters =
            [
               ("oauth_consumer_key", twitterSettings.ClientKey)
               ("oauth_nonce", System.Guid.NewGuid().ToString().Substring(24))
               ("oauth_signature_method", "HMAC-SHA1")
               ("oauth_timestamp", currentUnixTime().ToString())
               ("oauth_version", "1.0")
            ]

         let requestTokenUri = apiBase + requestToken
         let signingString = baseString "POST" requestTokenUri queryParameters
         let oauthSignature = hmacsha1Encrypt signingKey signingString
         let headerValue = createAuthorizeHeader (("oauth_signature", oauthSignature) :: queryParameters)

         let requestParam = { Url = requestTokenUri; PostRequestType = POST(PostEncodingType, "") }

         async {
            let! request = createHttpPostRequest requestParam

            let request = 
               request 
               |> addHeader ("Authorization", headerValue)
               |> setContentType "application/x-www-form-urlencoded"

            return! sendRequest parseRequestResponse request            
         }

      let obtainAccessToken oauth_token (tokenSecret : string) =
         let signingKey = { ClientSecret = twitterSettings.ClientSecret; TokenSecret = "" } |> toUrlParam

         let queryParameters =
            [
               ("oauth_consumer_key", twitterSettings.ClientKey)
               ("oauth_nonce", System.Guid.NewGuid().ToString().Substring(24))
               ("oauth_signature_method", "HMAC-SHA1")
               ("oauth_timestamp", currentUnixTime().ToString())
               ("oauth_version", "1.0")
               ("oauth_token", oauth_token)
               ("oauth_verifier", "xxx")
            ]

         let signingString = baseString "POST" accessToken queryParameters
         let oauthSignature = hmacsha1Encrypt signingKey signingString
         let headerValue = createAuthorizeHeader (("oauth_signature", oauthSignature) :: queryParameters)

         let requestParam = { Url = accessToken; PostRequestType = POST(PostEncodingType, "") }

         async {
            let! request = createHttpPostRequest requestParam

            let request = 
               request 
               |> addHeader ("Authorization", headerValue)
               |> setContentType "application/x-www-form-urlencoded"

            return! sendRequest parseRequestResponse request            
         }

