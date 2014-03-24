module JsonConverterTests

   open NUnit.Framework
   open FsUnit

   open Newtonsoft.Json

   type DebugConverter() =
      inherit JsonConverter()
       override x.CanConvert(t) = true
       override x.WriteJson(writer, value, serializer) = ()
       override x.ReadJson(reader, t, existingValue, serializer) = 
         System.Diagnostics.Debug.WriteLine("Reading JSON")
         existingValue


   [<JsonObject>]
   [<NoComparison>]
   type StylingItem = {
      [<field: JsonProperty("link_color")>]
      [<field: JsonConverter(typeof<WebUtilities.JsonConverters.ToOptionConverter>)>]
      LinkColour : string option 

      [<field: JsonProperty("noneField")>] 
      [<field: JsonConverter(typeof<WebUtilities.JsonConverters.ToOptionConverter>)>]
      NoneField : string option
   }

   [<TestFixture>]
   type JsonConverterFixtures() =
      [<Test>]
      member x.convertTypeWithOption() =
         let json = """{ "link_color": "#0077CC" }"""
         let obj = JsonConvert.DeserializeObject<StylingItem>(json)
          
         obj.LinkColour |> should equal (Some("#0077CC"))
         obj.NoneField |> should equal None
   
//      [<Test>]
//      let failure = 
//         Assert.Fail()
