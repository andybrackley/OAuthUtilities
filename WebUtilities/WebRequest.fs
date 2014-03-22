namespace WebUtilities

module WebRequest =
   open System
   open System.IO
   open System.Net
   open System.Text
   
   type EncodingType =
      | UrlFormEncoded
      | UrlFormEncodedUtf8
      | CustomEncoding of string

   type EncodingType with 
      member x.Value = 
         match x with 
         | UrlFormEncoded -> "application/x-www-form-urlencoded"
         | UrlFormEncodedUtf8 -> "application/x-www-form-urlencoded; charset=UTF-8"
         | CustomEncoding(encoding) -> encoding

   // NOTE: Post requires an async {} code block so I've sererated these into
   //       two different request types so that you can use PUT/GET in a normal code block
   type RequestType = PUT | GET
   type PostRequestType = POST of EncodingType * string

   type Request = { Url : string ; RequestType : RequestType }
   type PostRequest = { Url : string ; PostRequestType : PostRequestType }

   type WebRequest
      with 
         member request.AsyncGetRequestStream() =
            Async.FromBeginEnd<Stream>(request.BeginGetRequestStream, request.EndGetRequestStream)

   let getResponse( request : WebRequest ) = request.AsyncGetResponse()
   let getRequestStream (str : string) = UnicodeEncoding.UTF8.GetBytes(str)

   let addHeader (key : string, value : string) (request : WebRequest) =
      request.Headers.[key] <- value
      request

   let setContentType contentType (request : WebRequest) =
      request.ContentType <- contentType
      request

   let setMethodType methodType (request : WebRequest) =
      request.Method <- methodType
      request

   let setContent body (request : WebRequest) = 
      async {
         let streamBytes = getRequestStream(body)
         use! reqStream = request.AsyncGetRequestStream()
         reqStream.Write(streamBytes, 0, streamBytes.Length)
         return request
      }

   let createHttpRequest (request : Request) = 
      let webRequest = HttpWebRequest.Create(request.Url)
      match request.RequestType with
         | PUT -> webRequest |> setMethodType "PUT"
         | GET -> webRequest |> setMethodType "GET"

   let createHttpPostRequest (request : PostRequest) =
      let webRequest = HttpWebRequest.Create(request.Url)

      System.Diagnostics.Debug.WriteLine(String.Format("Sending To Url: {0}", request.Url))

      match request.PostRequestType with 
      | POST(encoding, post_data) -> 
         System.Diagnostics.Debug.WriteLine(String.Format("With PostData: {0}", post_data ))
         System.Diagnostics.Debug.WriteLine(String.Format("curl -d \"{0}\" {1}", post_data, request.Url))
         webRequest |> setMethodType "POST"
                    |> setContentType encoding.Value
                    |> setContent post_data
