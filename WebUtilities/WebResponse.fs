namespace WebUtilities

module WebResponse =
   module Parsers =
      let asString str = str

   open Ionic.Zlib
   open System.Net
   open System.IO
   open Parsers

   type StatusDescription = string
   type ApiReasonDescription = string

   [<NoComparison>]
   type WebResponseValue<'a> =
      | Success of 'a
      | WebException of WebException
      | ApiErrorResponse of HttpStatusCode * StatusDescription * ApiReasonDescription

   let asStream (response : WebResponse) =
      if response.Headers.["Content-Encoding"] = "gzip"
      then GZipStream(response.GetResponseStream(), CompressionMode.Decompress) :> Stream
      else response.GetResponseStream()
      

   let parseWebResponse parser (response : WebResponse) =
      use stream = response |> asStream
      let reader = new StreamReader(stream)
      let html = reader.ReadToEnd()
      parser html      

   let sendRequest parser (request : WebRequest) =
      async {
         try
            let! response = request.AsyncGetResponse()
            return Success(parseWebResponse parser response)
         with
            | :? WebException as webException when (webException.Response :? HttpWebResponse)
               -> 
                  let httpResponse = webException.Response :?> HttpWebResponse
                  let apiResponse =  parseWebResponse asString webException.Response
                  return ApiErrorResponse(httpResponse.StatusCode, httpResponse.StatusDescription, apiResponse)
            | :? WebException as webException -> return WebException(webException)
      }         
