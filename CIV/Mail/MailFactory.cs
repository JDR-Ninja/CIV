using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using System.Net.Mime;
using CIV.Common;
using ZedGraph;
using Videotron;

namespace CIV.Mail
{
    /// <summary>
    /// Classe gère l'envoi d'un courriel
    /// </summary>
    public class MailFactory
    {
        private List<MailTemplate> _mailTemplates = new List<MailTemplate>();

        public List<MailTemplate> MailTemplates
        {
            get { return _mailTemplates; }
            set { _mailTemplates = value; }
        }

        public MailFactory()
        {
            string[] templateFolder = new string[] { System.IO.Path.Combine(CIV.Common.IO.GetCivDataFolder(), @"MailTemplates\"),
                                                     System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"MailTemplates\")};

            try
            {
                // Crée une liste des modèles disponible
                foreach (string folder in templateFolder)
                {
                    // Si le dossier n'existe pas, on le crée
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    foreach (string subFolder in Directory.EnumerateDirectories(folder))
                    {
                        // Charge le fichier info.xml
                        string infoFile = System.IO.Path.Combine(subFolder, "info.xml");
                        if (File.Exists(infoFile))
                        {
                            MailTemplate template =  (MailTemplate)XmlFactory.LoadFromFile(typeof(MailTemplate), infoFile, new XmlFactorySettings());
                            template.Folder = subFolder;
                            MailTemplates.Add(template);
                        }
                    }
                }
            }
            catch (Exception fileExc)
            {
                LogFactory.LogEngine.Instance.Add(fileExc);
            }
        }

        /// <summary>
        /// Envoi un courriel en format HTML
        /// </summary>
        /// <param name="tagFormater">L'objet de conversion des tags initialiser avec un compte</param>
        /// <param name="smtp">La configuration du serveur SMTP</param>
        /// <param name="recipients">La liste des récipients</param>
        /// <param name="subject">Le sujet du courriel</param>
        /// <param name="templateFile">Le modèle à utiliser pour le courriel</param>
        /// <param name="img">L'image du graphique</param>
        public void SendMailHTML(MailTagFormater tagFormater,
                                 SmtpSettings smtp,
                                 string recipients,
                                 string subject,
                                 string templateFile,
                                 VideotronAccount account)
        {
            // Les templates personnalisé sont plus fort que ceux de base (notion d'override)
            // Si le template n'existe pas, on prend le premier de la liste
            if (MailTemplates.FirstOrDefault(p => p.Name == templateFile) == null)
                templateFile = MailTemplates.FirstOrDefault().Name;

            subject = tagFormater.Convert(subject);
            
            StringBuilder template = new StringBuilder(tagFormater.Convert(MailTemplates.FirstOrDefault(p => p.Name == templateFile).GetContent()));

            SmtpClient client = new SmtpClient(smtp.Host, smtp.Port);
            client.Credentials = new NetworkCredential(smtp.Username, smtp.Password);

            MailMessage message = new MailMessage();

            message.From = new MailAddress(smtp.SenderMail, smtp.Sender, System.Text.Encoding.UTF8);

            foreach (string address in recipients.Split(';'))
                message.To.Add(new MailAddress(address));

            message.Subject = subject;

            switch (MailTemplates.FirstOrDefault(p => p.Name == templateFile).Encoding)
            {
                case MailTemplate.MailTemplateEncodingType.ASCII:
                    message.SubjectEncoding = System.Text.Encoding.ASCII;
                    message.BodyEncoding = System.Text.Encoding.ASCII;
                    break;
                case MailTemplate.MailTemplateEncodingType.ISO8859:
                    message.SubjectEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
                    message.BodyEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
                    break;
                default:
                    message.SubjectEncoding = System.Text.Encoding.UTF8;
                    message.BodyEncoding = System.Text.Encoding.UTF8;
                    break;
            }           

            // Modèle HTML
            if (MailTemplates.FirstOrDefault(p => p.Name == templateFile).IsHTML)
            {
                message.IsBodyHtml = true;
                AlternateView htmlView;

                // Embebbed image
                List<string> embeddedImageFile = new List<string>();
                Match embeddedImage;
                string addedImage;
                do
                {
                    embeddedImage = Regex.Match(template.ToString(), "src=\"(?<src>[^cid]+)\"", RegexOptions.IgnoreCase);

                    if (embeddedImage.Success)
                    {
                        addedImage = System.IO.Path.Combine(MailTemplates.FirstOrDefault(p => p.Name == templateFile).Folder,
                                                            embeddedImage.Groups["src"].Value);
                        if (!File.Exists(addedImage))
                            throw new FileNotFoundException(addedImage);

                        embeddedImageFile.Add(addedImage);
                        template.Replace(String.Format("{0}", embeddedImage.Groups["src"].Value), String.Format("cid:img{0}", embeddedImageFile.Count));
                    }
                }
                while (embeddedImage.Success);

                // Insertion du graphique
                // Il faudrait le mettre dans MailTagFormater, mais c'est compliqué, pas le temps
                List<Tuple<int, int>> graphSize = new List<Tuple<int, int>>();
                Match match = Regex.Match(template.ToString(), @"\[GRAPH(\((?<param>[^\)]*)?\))?\]", RegexOptions.IgnoreCase);
                int graphCount = 0;
                int delta = 0;
                while (match.Success)
                {
                    int width = 430;
                    int height = 230;

                    Match paramMatch = Regex.Match(match.Groups["param"].Value,
                                                   @"(?<method>[a-z]{3,})=(?<param>[^,]+)",
                                                   RegexOptions.IgnoreCase);
                    while (paramMatch.Success)
                    {
                        if (paramMatch.Groups["method"].Value.ToUpper() == "WIDTH")
                            width = Convert.ToInt32(paramMatch.Groups["param"].Value);
                        else if (paramMatch.Groups["method"].Value.ToUpper() == "HEIGHT")
                            height = Convert.ToInt32(paramMatch.Groups["param"].Value);
                        paramMatch = paramMatch.NextMatch();
                    }

                    graphSize.Add(new Tuple<int, int>(width, height));

                    template.Remove(match.Index + delta, match.Length);
                    string newValue = String.Format("<img src=\"cid:graph_{0}\" width=\"{1}\" height=\"{2}\" />",
                                                    graphCount,
                                                    width,
                                                    height);
                    template.Insert(match.Index + delta, newValue);

                    if (match.Length > newValue.Length)
                        delta -= match.Length - newValue.Length;
                    else if (match.Length < newValue.Length)
                        delta += newValue.Length - match.Length;

                    graphCount++;
                    match = match.NextMatch();
                }

                htmlView = AlternateView.CreateAlternateViewFromString(template.ToString(), System.Text.Encoding.UTF8, "text/html");
                for (int i = 0; i < graphSize.Count; i++)
                {
                    System.Drawing.Image img;
                    MemoryStream memoryGraph = new MemoryStream();
                    GraphFactory graphFactory = new GraphFactory();
                    ZedGraphControl graph = graphFactory.Generate(account.Username,
                                                    new Period(account.PeriodStart, account.PeriodEnd),
                                                    graphSize[i].Item1, graphSize[i].Item2);

                    img = graph.GetImage();
                    img.Save(memoryGraph, System.Drawing.Imaging.ImageFormat.Jpeg);
                    memoryGraph.Position = 0;

                    htmlView.LinkedResources.Add(new LinkedResource(memoryGraph,
                                                                    MediaTypeNames.Image.Jpeg) {
                                                 ContentId = String.Format("graph_{0}", i),
                                                 TransferEncoding = TransferEncoding.Base64 });
                }

                for (int i = 0; i < embeddedImageFile.Count; i++)
                {
                    if (Regex.Match(embeddedImageFile[i], "(jpg|jpeg)$", RegexOptions.IgnoreCase).Success)
                        htmlView.LinkedResources.Add(new LinkedResource(embeddedImageFile[i], MediaTypeNames.Image.Jpeg) { ContentId = String.Format("img{0}", i + 1), TransferEncoding = TransferEncoding.Base64 });
                    else if (Regex.Match(embeddedImageFile[i], "gif$", RegexOptions.IgnoreCase).Success)
                        htmlView.LinkedResources.Add(new LinkedResource(embeddedImageFile[i], MediaTypeNames.Image.Gif) { ContentId = String.Format("img{0}", i + 1), TransferEncoding = TransferEncoding.Base64 });
                    else if (Regex.Match(embeddedImageFile[i], "tiff$", RegexOptions.IgnoreCase).Success)
                        htmlView.LinkedResources.Add(new LinkedResource(embeddedImageFile[i], MediaTypeNames.Image.Tiff) { ContentId = String.Format("img{0}", i + 1), TransferEncoding = TransferEncoding.Base64 });
                    else
                        htmlView.LinkedResources.Add(new LinkedResource(embeddedImageFile[i]) { ContentId = String.Format("img{0}", i + 1), TransferEncoding = TransferEncoding.Base64 });
                }

                message.AlternateViews.Add(htmlView);
            }

            // Plain/Text
            else
            {
                message.Body = template.ToString();
            }
            client.Send(message);
            message.Dispose();
        }
    }
}
