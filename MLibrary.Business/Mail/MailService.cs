using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace MLibrary.Business.Mail
{
    class MailService
    {
        private string MailServer { get; set; }
        private string MailPassword { get; set; }
        private string MailPort { get; set; }
        private string MailUser { get; set; }
        private string MailTo { get; set; }
        private string ReplyMail { get; set; }
        private string DisplayName { get; set; }
        private string RelayActive { get; set; }
        private string RelayMailServer { get; set; }
        private string RelayPort { get; set; }
        private string UseSSL { get; set; }
        private MailAddress FromMail { get; set; }
        public object ViewData { get; private set; }

        public MailService(string mailServer,string mailPassword,string mailUser,string mailPort,string relayActive,string relayMailServer,string relayPort,string replyMail,string displayName,string mailTo,string useSSL)
        {
            this.MailServer = mailServer;
            this.MailPassword = mailServer;
            this.MailUser = mailUser;
            this.MailPort = mailPort;
            this.RelayActive = relayActive;
            this.RelayMailServer = relayMailServer;
            this.RelayPort = relayPort;
            this.ReplyMail = replyMail;
            this.DisplayName = displayName;
            this.MailTo = mailTo;
            this.UseSSL = useSSL;
        }

        public void SendEmail(string subject, string body, string toMail, string attachmentPath = null)
        {
            this.FromMail = new MailAddress(this.MailUser, this.DisplayName);

            using (MailMessage mm = new MailMessage())
            {
                mm.From = this.FromMail;

                mm.Subject = subject;
                mm.Body = body;
                mm.IsBodyHtml = true;
                mm.To.Clear();
                mm.Bcc.Clear();
                mm.CC.Clear();

                if (string.IsNullOrEmpty(toMail) == false)
                {
                    if (toMail.Contains(";"))
                    {
                        foreach (string extraMail in toMail.Split(';'))
                        {
                            mm.To.Add(extraMail);
                        }
                    }
                    else
                    {
                        mm.To.Add(toMail);
                    }
                }

                SmtpClient smtp = new SmtpClient();

                smtp.EnableSsl = UseSSL == "1" ? true : false;

                if (RelayActive == "0")
                {
                    smtp.Host = MailServer;
                    NetworkCredential NetworkCred = new NetworkCredential(this.MailUser, this.MailPassword);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = Convert.ToInt32(this.MailPort);
                }
                else
                {
                    smtp.Host = RelayMailServer;
                    smtp.Port = Convert.ToInt32(this.RelayPort);
                }

                if (!string.IsNullOrEmpty(attachmentPath))
                {
                    Attachment attachment = new Attachment(attachmentPath);
                    mm.Attachments.Add(attachment);
                }

                smtp.Timeout = 60 * 1 * 1000;
                smtp.Send(mm);
            }


        }

        public void SendEmailWithImageAndGmail(string subject, string toMail, string decrypt, List<string> imageNames, Template template, List<UploadFile> UploadFiles = null)
        {

            this.FromMail = new MailAddress(this.MailUser, this.DisplayName);
            List<MailImageModel> mailImageModels = new List<MailImageModel>();
            AlternateView avHtml;
            LinkedResource inline;




            try
            {
                string mailBodyForInfoFormat = "";
                string tempTemplate = template.HtmlContent;
                tempTemplate = string.Format(tempTemplate, decrypt);
                tempTemplate = tempTemplate.Replace("{", "{{").Replace("}", "}}");
                avHtml = AlternateView.CreateAlternateViewFromString(tempTemplate, null, MediaTypeNames.Text.Html);
                avHtml.TransferEncoding = System.Net.Mime.TransferEncoding.QuotedPrintable;
                foreach (var item in imageNames)
                {
                    string imageFolderName = item.Replace("cid:", "");
                    string path = $@"{HttpRuntime.AppDomainAppPath}\Image\MailingImage\{imageFolderName}";



                    if (item.Contains("png"))
                    {
                        inline = new LinkedResource(path);
                        inline.ContentType = new ContentType("image/png");
                    }
                    else if (item.Contains("gif"))
                    {
                        inline = new LinkedResource(path, MediaTypeNames.Image.Gif);
                    }
                    else
                    {
                        inline = new LinkedResource(path, MediaTypeNames.Image.Jpeg);
                    }



                    inline.ContentId = imageFolderName;
                    inline.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                    inline.ContentLink = new Uri($"cid:{imageFolderName}");
                    avHtml.LinkedResources.Add(inline);




                    mailBodyForInfoFormat = string.Format(tempTemplate, inline.ContentId);



                    MailImageModel mailImageModel = new MailImageModel
                    {
                        AttachPath = path,
                        AlternateView = avHtml
                    };

                    mailImageModels.Add(mailImageModel);

                }


                mailBodyForInfoFormat = mailImageModels.Count > 0 ? mailBodyForInfoFormat : tempTemplate;





                using (MailMessage mm = new MailMessage())
                {
                    mm.From = this.FromMail;

                    mm.Subject = subject;
                    mm.Body = mailBodyForInfoFormat;
                    mm.IsBodyHtml = true;
                    mm.To.Clear();
                    mm.Bcc.Clear();
                    mm.CC.Clear();

                    mm.To.Add(toMail);

                    SmtpClient smtp = new SmtpClient();

                    smtp.EnableSsl = UseSSL == "1" ? true : false;

                    if (RelayActive == "0")
                    {
                        smtp.Host = MailServer;
                        NetworkCredential NetworkCred = new NetworkCredential(this.MailUser, this.MailPassword);
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = Convert.ToInt32(this.MailPort);
                    }
                    else
                    {
                        smtp.Host = RelayMailServer;
                        smtp.Port = Convert.ToInt32(this.RelayPort);
                    }



                    //if (mailImageModels.Count > 0)
                    //{



                    //    if (mailImageModels.Count != mm.Attachments.Count)
                    //    {
                    //        foreach (var item in mailImageModels)
                    //        {






                    //            mm.AlternateViews.Add(item.AlternateView);

                    //        }
                    //    }
                    //}

                    mm.AlternateViews.Add(avHtml);

                    if (UploadFiles.Count > 0)
                    {
                        foreach (var item in UploadFiles)
                        {
                            System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(item.Path);
                            attachment.Name = item.FileName;
                            mm.Attachments.Add(attachment);



                        }
                    }




                    smtp.Send(mm);
                    smtp.Dispose();

                }
            }
            catch (System.Threading.ThreadAbortException ex)
            {


                Thread.ResetAbort();
            }




        }
    }
}
