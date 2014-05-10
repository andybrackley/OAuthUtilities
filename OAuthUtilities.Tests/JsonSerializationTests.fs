module JsonConverterTests

   open NUnit.Framework
   open FsUnit
   
   open Newtonsoft.Json
   open WebUtilities.JsonConverters

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
      [<field: JsonConverter(typeof<ToOptionConverter>)>]
      LinkColour : string option 

      [<field: JsonProperty("noneField")>] 
      [<field: JsonConverter(typeof<ToOptionConverter>)>]
      NoneField : string option

      [<field: JsonProperty("created_at")>]
      [<field: JsonConverter(typeof<FromTwitterDate>) >]
      CreatedAt : System.DateTime
   }

   [<TestFixture>]
   type JsonConverterFixtures() =
      [<Test>]
      member x.convertTypeWithOption() =
         let json = """{ "link_color": "#0077CC", 
                         "created_at" : "Fri Jul 01 21:41:35 +0000 2011" }"""
         let obj = JsonConvert.DeserializeObject<StylingItem>(json)
          
         obj.LinkColour |> should equal (Some("#0077CC"))
         obj.NoneField |> should equal None
         obj.CreatedAt |> should equal (System.DateTime(2011, 07, 01, 22, 41, 35))
   
//      [<Test>]
//      let failure = 
//         Assert.Fail()
