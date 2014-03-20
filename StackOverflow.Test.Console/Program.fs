namespace StackOverflow.Test.Console

module main =
   open System
   open System.Windows.Forms

   open WebUtilities.WebResponse
   open StackOverflowApi.OAuth

   open BrowserWindow
   open OAuthClientAppSettings

   let getAccessToken accessCode = 
      async {
         let! accessResponse = sendAccessRequest OAuthParams accessCode
         match accessResponse with
         | Success(token) -> System.Diagnostics.Debug.WriteLine(token); return token.Token
         | WebException(exn) -> System.Diagnostics.Debug.WriteLine(exn.ToString()); return exn.Message
         | ApiErrorResponse(statusCode, statusDesc, apiResponse) -> System.Diagnostics.Debug.WriteLine(apiResponse); return statusDesc
      } 

   let simulateSomeRequest accessToken = 
      System.Diagnostics.Debug.WriteLine(accessToken)

   let onUserHasBeenAuthenticated accessCode =
      async {
         let! accessToken = getAccessToken accessCode
         simulateSomeRequest accessToken
         return ()
      } |> Async.Start


   [<EntryPoint>]
   [<STAThread>]
   let main argv = 

      let soAuthUrl = buildRequestUrl OAuthParams

      Application.EnableVisualStyles()
      let form, browser = showWebBrowser

      browser.Navigate(new System.Uri(soAuthUrl))
      browser.Navigated 
         |> Observable.filter ( fun args -> args.Url.Host = OAuthParams.RedirectUrl.Replace("http://", ""))
         |> Observable.map ( fun args -> getAccessCodeFromUrl args.Url)
         |> Observable.subscribe (fun accessCode ->  onUserHasBeenAuthenticated accessCode.Value |> ignore)
         |> ignore

      Application.Run(form)
      0
       
        