namespace WebUtilities

module JsonConverters =

   open System
   open System.Reflection
   open Microsoft.FSharp.Reflection
   open Newtonsoft.Json
   open Newtonsoft.Json.Converters

   /// My original version of the JsonConverter which I quite liked but am now
   /// thinking that specifying the additional type parameter is redundant
   type ToOptionConverter2<'a>() =
      inherit JsonConverter() 
         override x.WriteJson(writer, value, serializer) = ()
         override x.ReadJson(reader, objectType, existingValue, serializer) = 
            let value = serializer.Deserialize(reader, typeof<'a>)
            let cases = FSharpType.GetUnionCases(typeof<Option<'a>>)
            if value = null then 
               FSharpValue.MakeUnion(cases.[0], [||])
            else 
               FSharpValue.MakeUnion(cases.[1], [|value|])
            
         override x.CanConvert(objectType : System.Type) = 
            objectType = typeof<'a>

   // Taken from here: http://gorodinski.com/blog/2013/01/05/json-dot-net-type-converters-for-f-option-list-tuple/
   type ToOptionConverter() =
       inherit JsonConverter()
    
       override x.CanConvert(t) = 
           t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() = typedefof<option<_>>

       override x.WriteJson(writer, value, serializer) =
           let value = 
               if value = null then null
               else 
                   let _,fields = FSharpValue.GetUnionFields(value, value.GetType())
                   fields.[0]  
           serializer.Serialize(writer, value)

       override x.ReadJson(reader, t, existingValue, serializer) =    
           let innerType = t.GenericTypeArguments.[0]
           let innerType = 
               if innerType.GetTypeInfo().IsValueType then (typedefof<Nullable<_>>).MakeGenericType([|innerType|])
               else innerType        
           let value = serializer.Deserialize(reader, innerType)
           let cases = FSharpType.GetUnionCases(t)
           if value = null then FSharpValue.MakeUnion(cases.[0], [||])
           else FSharpValue.MakeUnion(cases.[1], [|value|])

   type FromUnixEpochToDateTimeConverter() =
       inherit JsonConverter()

       override x.CanConvert(t) = t.GetType() = typeof<int>
       override x.WriteJson(writer, value, serializer) = ()
       override x.ReadJson(reader, t, existingValue, serializer) = System.DateTime.Now :> obj // For now I'm just going to return Now 


