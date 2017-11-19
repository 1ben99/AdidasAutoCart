Imports System.IO
Imports System.Net
Imports System.Text

Module Module1
    Sub Main()
        Dim success As Boolean = False
        Do Until success
            Console.WriteLine("Please enter a Product Code: ")
            Dim producCode As String = Console.ReadLine()
            Console.WriteLine("Please enter a Shoe Size: ")
            Dim shoeShize As Integer = Console.ReadLine()
            Dim code As Integer = (500 + ((shoeShize * 20) - 40))
            Dim url As String = "https://www.adidas.co.uk/" & producCode & ".html"
            Try
                shoeRequest(url, producCode, code, shoeShize)
                success = True
            Catch ex As Exception
                success = False
            End Try
        Loop
        Console.ReadLine()
    End Sub

    Sub shoeRequest(ByVal url As String, ByVal productId As String, ByVal code As Integer, ByVal shoeSize As Integer)
        Dim requestCookies As New CookieContainer
        requestCookies.Add(New Cookie("DNT", "1") With {.Domain = "adidas.co.uk"})
        Dim request As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
        request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36"
        request.Host = "www.adidas.co.uk"
        request.KeepAlive = True
        request.Headers.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8")
        request.Headers.Add("Upgrade-Insecure-Requests", "1")
        request.Headers.Add("Accept-Encoding", "gzip, deflate, br")
        request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8"
        request.CookieContainer = requestCookies

        Dim response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
        requestCookies.Add(response.Cookies)

        Dim postData As String = "{""product_id"":""" & productId & """,""quantity"":1,""product_variation_sku"":""" & productId & "_" & code.ToString & """,""size"":""" & shoeSize.ToString & """,""recipe"":null,""invalidFields"":[],""isValidating"":false,""clientCaptchaResponse"":""""}"
        Dim encoding As New UTF8Encoding
        Dim byteData As Byte() = encoding.GetBytes(postData)

        Dim checkoutRequest As HttpWebRequest = DirectCast(WebRequest.Create("https://www.adidas.co.uk/api/cart_items"), HttpWebRequest)
        checkoutRequest.Method = "POST"
        checkoutRequest.KeepAlive = True
        checkoutRequest.Host = "www.adidas.co.uk"
        checkoutRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36"
        checkoutRequest.Headers.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8")
        checkoutRequest.Accept = "*/*"
        checkoutRequest.ContentType = "application/json"
        checkoutRequest.CookieContainer = requestCookies
        checkoutRequest.ContentLength = byteData.Length

        Dim postreqstream As Stream = checkoutRequest.GetRequestStream()
        postreqstream.Write(byteData, 0, byteData.Length)
        postreqstream.Close()

        Dim postresponse As HttpWebResponse = DirectCast(checkoutRequest.GetResponse(), HttpWebResponse)
        requestCookies.Add(postresponse.Cookies)

        Dim cookies As String = "["
        For Each cookie In requestCookies.GetCookies(New Uri("https://www.adidas.co.uk"))
            If cookie.ToString.Contains("%") Then
                Dim decodedCookie As String = Uri.UnescapeDataString(cookie.ToString.Split("=")(1))
                cookie.name = cookie.ToString.Split("=")(0)
                cookie.value = decodedCookie
            End If
            If Not cookie.value.ToString.Contains("}") Then
                cookies &= ("{""domain"": """ & cookie.domain & """," & """name"": """ & cookie.name & """," & """path"": """ & cookie.path & """," & """value"": """ & cookie.value & """},")
            End If
        Next
        cookies = cookies.Trim(",") & "]"
        Console.WriteLine(cookies)
    End Sub
End Module
