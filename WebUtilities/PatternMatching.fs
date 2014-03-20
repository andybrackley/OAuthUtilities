﻿namespace WebUtilities

module PatternMatching =
   let (|StartsWith|_|) (p:string) (s:string) =
      if s.StartsWith(p) then
         Some(s.Substring(p.Length))
      else
         None
