namespace TwitterApi.Tests

open WebUtilities
open NUnit.Framework
open FsUnit
open TwitterApi.Json

[<TestFixture>]
type TweetSerializationFixture() = 
    [<Test>]
    member this.``Parse the sample tweet list from the twitter website``() = 
      let file = System.IO.File.ReadAllText("SampleTweetList.json")
      let deserialized = file |> WebResponse.Parsers.asJsonObject<Tweet array>
      printfn "%A" deserialized
