// NOTE: This file contains all my private details and therefore I've only committed a skeleton
//       file to the repository.
//       Don't check this file in with the secret details filled in

namespace StackOverflow.Test.Console

module OAuthClientAppSettings =

   open StackOverflowApi.OAuth

   let OAuthParams = { 
      ClientId = ClientId(1234); 
      ClientSecret = "abcdefghijklmn"
      ClientKey = "qwertyuip" 
      RedirectUrl = "http://www.test.com"
   }

