namespace StackOverflow.Test.Console

module BrowserWindow =

   open System
   open System.Windows.Forms
   open System.Windows

   let showWebBrowser = 
      let window = new Form()
      window.Height <- 500
      window.Width <- 600

      let wb = new WebBrowser()
      wb.Height <- 500
      wb.Width <- 600
      window.Controls.Add(wb)
      (window, wb)
