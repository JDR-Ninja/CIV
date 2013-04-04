using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Cache;
using System.IO;
using CIV.Common;
using System.ServiceModel;
using CIV.CentralCenterClient.CentralCenterServiceReference;

namespace CIV
{
    public class InternetAccessUpdater
    {
        public EventHandler IsNotCompatibleClient;

        private CentralCenterSoapClient CreateClient(BasicHttpBinding binding, EndpointAddress endpoint)
        {
            if (endpoint == null)
                return new CentralCenterSoapClient();
            else
                return new CentralCenterSoapClient(binding, endpoint);
        }

        public void Execute(BasicHttpBinding binding, EndpointAddress endpoint)
        {
            //try
            //{
            bool modified = false;

            using (CentralCenterSoapClient centralCenter = CreateClient(binding, endpoint))
            {
                if (centralCenter.IsCompatibleClient(App.VersionStr()))
                {
                    CIV.CentralCenterClient.CentralCenterServiceReference.InternetAccessBO[] updatedAccess = centralCenter.GetInternetAccess();

                    if (updatedAccess != null)
                    {
                        InternetAccess oldAccess;
                        InternetAccess newAccess;

                        for (int i = 0; i < updatedAccess.Count(); i++)
                        {
                            newAccess = new InternetAccess()
                                {
                                    Id = updatedAccess[i].Id,
                                    MaxCost = updatedAccess[i].MaxCost,
                                    OverCharge = updatedAccess[i].OverCharge,
                                    TotalCombined = updatedAccess[i].TotalCombined,
                                    UploadSpeed = updatedAccess[i].UploadSpeed,
                                    DownloadSpeed = updatedAccess[i].DownloadSpeed,
                                    LastUpdate = updatedAccess[i].LastUpdate
                                };
                            newAccess.Name[SupportedLanguages.French] = updatedAccess[i].NameFr;
                            newAccess.Name[SupportedLanguages.English] = updatedAccess[i].NameEn;

                            oldAccess = InternetAccesList.Instance.Access.SingleOrDefault(p => p.Id == newAccess.Id);

                            // Nouvelle accès
                            if (oldAccess == null)
                            {
                                modified = true;
                                InternetAccesList.Instance.Access.Add(newAccess);
                            }

                            // Mise à jour d'un accès
                            else if (oldAccess.LastUpdate.CompareTo(updatedAccess[i].LastUpdate) != 0)
                            {
                                modified = true;
                                oldAccess.MaxCost = newAccess.MaxCost;
                                oldAccess.OverCharge = newAccess.OverCharge;
                                oldAccess.TotalCombined = newAccess.TotalCombined;
                                oldAccess.UploadSpeed = newAccess.UploadSpeed;
                                oldAccess.DownloadSpeed = newAccess.DownloadSpeed;
                                oldAccess.Name = newAccess.Name;
                                oldAccess.LastUpdate = newAccess.LastUpdate;
                            }
                        }
                    }
                    if (modified)
                    {
                        InternetAccesList.Instance.Save();
                        InternetAccesList.Reload();
                    }
                }
                else if (IsNotCompatibleClient != null)
                    IsNotCompatibleClient(this, null);
            }
            //}
            //catch
            //{
            //    // Si ça plante, on met pas à jours
            //    return new InternetAccesList() { Publication = DateTime.Now.AddYears(-1)};
            //}
        }
    }
}
