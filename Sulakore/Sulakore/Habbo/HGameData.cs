using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Drawing;

namespace Sulakore.Habbo
{
    public struct HGameData
    {
        public static HGameData Empty;

        public bool IsEmpty
        {
            get { return this == Empty; }
        }

        private readonly string _Variables;
        public string Variables
        {
            get { return _Variables; }
        }

        private readonly string _Texts;
        public string Texts
        {
            get { return _Texts; }
        }

        private readonly string _FigurePartList;
        public string FigurePartList
        {
            get { return _FigurePartList; }
        }

        private readonly string _OverrideTexts;
        public string OverrideTexts
        {
            get { return _OverrideTexts; }
        }

        private readonly string _OverrideVariables;
        public string OverrideVariables
        {
            get { return _OverrideVariables; }
        }

        private readonly string _ProductDataLoadURL;
        public string ProductDataLoadURL
        {
            get { return _ProductDataLoadURL; }
        }

        private readonly string _FurniDataLoadURL;
        public string FurniDataLoadURL
        {
            get { return _FurniDataLoadURL; }
        }

        public HGameData(string Variables, string Texts, string FigurePartList, string OverrideTexts, string OverrideVariables, string ProductDataLoadURL, string FurniDataLoadURL)
        {
            _Variables = Variables;
            _Texts = Texts;
            _FigurePartList = FigurePartList;
            _OverrideTexts = OverrideTexts;
            _OverrideVariables = OverrideVariables;
            _ProductDataLoadURL = ProductDataLoadURL;
            _FurniDataLoadURL = FurniDataLoadURL;
        }

        public static HGameData Parse(string ClientBody)
        {
            string EV = ClientBody.GetChild("\"external.variables.txt\" : \"", '\"');
            string ET = ClientBody.GetChild("\"external.texts.txt\" : \"", '\"');
            string EFPL = ClientBody.GetChild("\"external.figurepartlist.txt\" : \"", '\"');
            string EOT = ClientBody.GetChild("\"external.override.texts.txt\" : \"", '\"');
            string EOV = ClientBody.GetChild("\"external.override.variables.txt\" : \"", '\"');
            string PDLU = ClientBody.GetChild("\"productdata.load.url\" : \"", '\"');
            string FDLU = ClientBody.GetChild("\"furnidata.load.url\" : \"", '\"');
            return new HGameData(EV, ET, EFPL, EOT, EOV, PDLU, FDLU);
        }

        public static bool operator ==(HGameData x, HGameData y)
        {
            if (x.Variables != y.Variables) return false;
            if (x.Texts != y.Texts) return false;
            if (x.FigurePartList != y.FigurePartList) return false;
            if (x.OverrideTexts != y.OverrideTexts) return false;
            if (x.OverrideVariables != y.OverrideVariables) return false;
            if (x.ProductDataLoadURL != y.ProductDataLoadURL) return false;
            if (x.FurniDataLoadURL != y.FurniDataLoadURL) return false;
            return true;
        }
        public static bool operator !=(HGameData x, HGameData y)
        {
            return !(x == y);
        }
        public override bool Equals(object obj)
        {
            return obj is HGameData && this == (HGameData)obj;
        }
        public override int GetHashCode()
        {
            return _Variables.GetHashCode() ^ _Texts.GetHashCode();
        }
    }
}