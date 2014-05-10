namespace TwitterApi

module Crypto =
   open System
   open System.Text

   // Unfortunately the standard HMACSHA1 utility provided by System.Security.Cryptography
   // isn't available from the Portable Class Libraries
   // I'm therefore using the Org.BouncyCastly.Crypto library Java port
   open Org.BouncyCastle.Crypto
   open Org.BouncyCastle.Crypto.Parameters

   let asUtfBytes (str : string) = Encoding.UTF8.GetBytes str
   let toBase64   (bytes : byte[]) = Convert.ToBase64String bytes
   let initHMac signingKey digest =
      let mac = Macs.HMac digest
      mac.Init (KeyParameter (signingKey |> asUtfBytes ))
      mac

   let setMessage (message : byte[]) (mac : IMac) =
      mac.BlockUpdate(message, 0, message.Length)
      mac

   let encryptedAsString size (mac : IMac) =
      let result = [| for i in 1 .. size -> (byte)0 |]
      mac.DoFinal(result, 0) |> ignore
      result

   let encryptedAsBase64 size (mac : IMac) =
      encryptedAsString size mac |> toBase64

   let hmacsha1Encrypt (signingKey : string) (str : string) = 
      let digest = Digests.Sha1Digest()
      let encrypted = 
         digest |> initHMac signingKey 
                |> setMessage (str |> asUtfBytes)
                |> encryptedAsString (digest.GetDigestSize())

      encrypted |> toBase64
