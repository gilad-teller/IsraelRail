using IsraelRail.Models.ApiModels;
using System.Web;

namespace IsraelRail.Models.ViewModels
{
    public class StationUpdate
    {
        public E_Language Language { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }

        public StationUpdate(GetStationsInfoResponseData getStationsInfoResponseData, E_Language language)
        {
            Language = language;
            switch (language)
            {
                case E_Language.Hebrew:
                    Name = getStationsInfoResponseData.NameHeb;
                    Content = HttpUtility.HtmlDecode(getStationsInfoResponseData.UpdateContentHeb);
                    Link = getStationsInfoResponseData.UpdateLinkHeb;
                    break;
                case E_Language.English:
                    Name = getStationsInfoResponseData.NameEng;
                    Content = HttpUtility.HtmlDecode(getStationsInfoResponseData.UpdateContentEng);
                    Link = getStationsInfoResponseData.UpdateLinkEng;
                    break;
                case E_Language.Russian:
                    Name = getStationsInfoResponseData.NameRus;
                    Content = HttpUtility.HtmlDecode(getStationsInfoResponseData.UpdateContentRus);
                    Link = getStationsInfoResponseData.UpdateLinkRus;
                    break;
                case E_Language.Arabic:
                    Name = getStationsInfoResponseData.NameArb;
                    Content = HttpUtility.HtmlDecode(getStationsInfoResponseData.UpdateContentArb);
                    Link = getStationsInfoResponseData.UpdateLinkArb;
                    break;
            }
        }
    }
}
