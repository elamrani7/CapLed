using System;
using System.Net;
using System.Net.Mail;

try {
    using var client = new SmtpClient(""smtp.gmail.com"", 587) {
        Credentials = new NetworkCredential(""hamzaelamrani799@gmail.com"", ""uzvulhyodjiixyry""),
        EnableSsl = true
    };
    var msg = new MailMessage(""hamzaelamrani799@gmail.com"", ""hamzakhalis619@gmail.com"");
    msg.Subject = ""Test de livraison depuis CapLed ERP"";
    msg.Body = ""Ceci est un test pour voir si l'email arrive sans HTML ni lien localhost."";
    client.Send(msg);
    Console.WriteLine(""Success!"");
} catch(Exception e) {
    Console.WriteLine(e.Message);
}
