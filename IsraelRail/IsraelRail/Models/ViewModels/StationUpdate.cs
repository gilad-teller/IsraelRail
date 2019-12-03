using IsraelRail.Models.ApiModels;

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
                    Content = getStationsInfoResponseData.UpdateContentHeb;
                    Link = getStationsInfoResponseData.UpdateLinkHeb;
                    break;
                case E_Language.English:
                    Name = getStationsInfoResponseData.NameEng;
                    Content = getStationsInfoResponseData.UpdateContentEng;
                    Link = getStationsInfoResponseData.UpdateLinkEng;
                    break;
                case E_Language.Russian:
                    Name = getStationsInfoResponseData.NameRus;
                    Content = getStationsInfoResponseData.UpdateContentRus;
                    Link = getStationsInfoResponseData.UpdateLinkRus;
                    break;
                case E_Language.Arabic:
                    Name = getStationsInfoResponseData.NameArb;
                    Content = getStationsInfoResponseData.UpdateContentArb;
                    Link = getStationsInfoResponseData.UpdateLinkArb;
                    break;
            }
        }
    }
}
