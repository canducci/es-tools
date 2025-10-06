using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estools.Library;

public partial class Dadger : BaseDocument
{


    Dictionary<string, IBlock<BaseLine>> blocos = new Dictionary<string, IBlock<BaseLine>>() {
                {"TE"               , new DummyBlock()},
                {"SB"               , new SbBlock()},
                {"UH"               , new UhBlock()},
                {"CT"               , new DummyBlock()},
                {"CI CE"            , new DummyBlock()},
                {"UE"               , new DummyBlock()},
                //{"ME"               , new MeBlock()},
                {"ME"               , new DummyBlock()},
                {"VR"               , new DummyBlock()},
                {"DP"               , new DummyBlock()},
                {"CD"               , new DummyBlock()},
                {"TD"               , new DummyBlock()},
                {"BE"               , new DummyBlock()},
                {"PQ"               , new DummyBlock()},

                {"RI"               , new DummyBlock()},
                {"VL"               , new DummyBlock()},
                {"VU"               , new DummyBlock()},

                {"IT"               , new DummyBlock()},
                {"IA"               , new DummyBlock()},
                {"RC"               , new DummyBlock()},
                {"TX"               , new DummyBlock()},
                {"PE"               , new DummyBlock()},
                {"GP"               , new DummyBlock()},
                {"NI"               , new DummyBlock()},
                {"PD"               , new DummyBlock()},

                {"DT"               , new DummyBlock()},
                {"MP"               , new DummyBlock()},
                {"MT"               , new DummyBlock()},
                {"FD"               , new DummyBlock()},
                {"VE"               , new VeBlock()},

                {"VM"               , new DummyBlock()},
                {"DF"               , new DummyBlock()},

                {"RE LU FU FT FI FE"   , new DummyBlock()},
                {"VI"               , new ViBlock()},
                {"QI"               , new QiBlock()},
                {"AC"               , new AcBlock()},
                {"RV"               , new DummyBlock()},

                {"FP"               , new DummyBlock()},
                {"FQ"               , new DummyBlock()},

                {"PS"               , new DummyBlock()},
                {"PM"               , new DummyBlock()},
                {"PI"               , new DummyBlock()},
                {"IR"               , new DummyBlock()},


                {"RS"               , new DummyBlock()},


                {"QA"               , new DummyBlock()},
                {"QV"               , new DummyBlock()},
                {"FC"               , new DummyBlock()},
                {"EA"               , new DummyBlock()},
                {"ES"               , new DummyBlock()},

                {"TI"               , new DummyBlock()},
                {"RQ"               , new DummyBlock()},
                {"EZ"               , new EzBlock()},
                {"HA LA CA"         , new DummyBlock()},
                {"HV LV CV"         , new RhvBlock()},
                {"HQ LQ CQ"         , new DummyBlock()},

                {"HE CM"            , new DummyBlock()},

                {"DA"               , new DummyBlock()},
                {"PU"               , new DummyBlock()},
                {"VP"               , new DummyBlock()},
                {"RT"               , new DummyBlock()},
                {"SA"               , new DummyBlock()},

                {"AR"               , new DummyBlock()},
                {"EV"               , new DummyBlock()},
                {"FJ"               , new DummyBlock()},

            };

    public override Dictionary<string, IBlock<BaseLine>> Blocos
    {
        get
        {
            return blocos;
        }
    }

    //public DateTime DataEstudo
    //{
    //    get { return BlocoDT.DataEstudo; }
    //    set
    //    {
    //        BlocoDT.DataEstudo = value;
    //    }
    //}

    //public DtBlock BlocoDT { get { return (DtBlock)Blocos["DT"]; } }
    //public RiBlock BlocoRi { get { return (RiBlock)Blocos["RI"]; } }
    public VeBlock BlocoVe { get { return (VeBlock)Blocos["VE"]; } }
    //public MpBlock BlocoMp { get { return (MpBlock)Blocos["MP"]; } }
    //public MtBlock BlocoMt { get { return (MtBlock)Blocos["MT"]; } }
    //public CtBlock BlocoCT { get { return (CtBlock)Blocos["CT"]; } }
    public UhBlock BlocoUh { get { return (UhBlock)Blocos["UH"]; } set { Blocos["UH"] = value; } }
    public AcBlock BlocoAc { get { return (AcBlock)Blocos["AC"]; } }
    //public DpBlock BlocoDp
    //{
    //    get { return (DpBlock)Blocos["DP"]; }
    //    set { Blocos["DP"] = value; }
    //}
    //public CdBlock BlocoCd { get { return (CdBlock)Blocos["CD"]; } }
    //public RhaBlock BlocoRha { get { return (RhaBlock)Blocos["HA LA CA"]; } }
    public RhvBlock BlocoRhv { get { return (RhvBlock)Blocos["HV LV CV"]; } }
    //public RhqBlock BlocoRhq { get { return (RhqBlock)Blocos["HQ LQ CQ"]; } }
    public SbBlock BlocoSb { get { return (SbBlock)Blocos["SB"]; } }
    //public PqBlock BlocoPq { get { return (PqBlock)Blocos["PQ"]; } }
    //public ItBlock BlocoIt { get { return (ItBlock)Blocos["IT"]; } }
    //public IaBlock BlocoIa { get { return (IaBlock)Blocos["IA"]; } }
    public EzBlock BlocoEz { get { return (EzBlock)Blocos["EZ"]; } }
    //public RheBlock BlocoRhe { get { return (RheBlock)Blocos["RE LU FU FT FI FE"]; } }
    public ViBlock BlocoVi { get { return (ViBlock)Blocos["VI"]; } }
    public QiBlock BlocoQi { get { return (QiBlock)Blocos["QI"]; } }
    //public EsBlock BlocoEs { get { return (EsBlock)Blocos["ES"]; } }
    //public EaBlock BlocoEa { get { return (EaBlock)Blocos["EA"]; } }
    //public TiBlock BlocoTi { get { return (TiBlock)Blocos["TI"]; } }

    public override bool IsComment(string line)
    {
        return line.StartsWith("&");
    }

    public DateTime VAZOES_DataDoEstudo
    {
        get
        {

            var i = BottonComments.IndexOf("\n& VAZOES") + 1;
            var vazoes = BottonComments.Substring(i).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);


            var mes = int.Parse(vazoes[4].Substring(39, 2).Trim());
            var ano = int.Parse(vazoes[6].Substring(39, 4).Trim());
            return new DateTime(ano, mes, 1);

        }
        set
        {
            var i = BottonComments.IndexOf("\n& VAZOES") + 1;
            var vazoes = BottonComments.Substring(i).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            vazoes[4] = vazoes[4].Remove(39, 2).Insert(39, value.Month.ToString("00"));
            vazoes[4] = vazoes[4].PadRight(48).Remove(44, 4).Insert(44, value.Year.ToString("0000"));
            vazoes[5] = vazoes[5].Remove(39, 2).Insert(39, value.AddMonths(1).Month.ToString("00"));
            vazoes[5] = vazoes[5].PadRight(48).Remove(44, 4).Insert(44, value.AddMonths(1).Year.ToString("0000"));

            vazoes[6] = vazoes[6].Remove(39, 4).Insert(39, value.Year.ToString("0000"));

            BottonComments = BottonComments.Remove(i).Insert(i,
                String.Join(Environment.NewLine, vazoes)
                );
        }


    }

    public string VAZOES_ArquivoPrevs
    {
        get
        {

            var i = BottonComments.IndexOf("\n& VAZOES") + 1;
            var vazoes = BottonComments.Substring(i).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            return vazoes[1].Substring(39).Trim();
        }
        set
        {
            var i = BottonComments.IndexOf("\n& VAZOES") + 1;
            var vazoes = BottonComments.Substring(i).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            vazoes[1] = vazoes[1].Remove(39).Insert(39, value);

            BottonComments = BottonComments.Remove(i).Insert(i,
                String.Join(Environment.NewLine, vazoes)
                );
        }
    }

    public int VAZOES_AnoTendeciaHidrologica
    {
        get
        {

            var i = BottonComments.IndexOf("\n& VAZOES") + 1;
            var vazoes = BottonComments.Substring(i).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            return int.Parse(vazoes[2].Substring(39, 4).Trim());

        }
        set
        {
            var i = BottonComments.IndexOf("\n& VAZOES") + 1;
            var vazoes = BottonComments.Substring(i).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            vazoes[2] = vazoes[2].Remove(39, 4).Insert(39, value.ToString("0000"));

            BottonComments = BottonComments.Remove(i).Insert(i,
                String.Join(Environment.NewLine, vazoes)
                );
        }
    }
    public int VAZOES_MesInicialDoEstudo
    {
        get
        {

            var i = BottonComments.IndexOf("\n& VAZOES") + 1;
            var vazoes = BottonComments.Substring(i).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            return int.Parse(vazoes[4].Substring(39, 2).Trim());

        }
        set
        {
            var i = BottonComments.IndexOf("\n& VAZOES") + 1;
            var vazoes = BottonComments.Substring(i).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            vazoes[4] = vazoes[4].Remove(39, 2).Insert(39, value.ToString("00"));
            vazoes[5] = vazoes[5].Remove(39).Insert(39, (value < 12 ? value + 1 : 1).ToString("00"));

            BottonComments = BottonComments.Remove(i).Insert(i,
                String.Join(Environment.NewLine, vazoes)
                );
        }
    }
    public int VAZOES_AnoInicialDoEstudo
    {
        get
        {

            var i = BottonComments.IndexOf("\n& VAZOES") + 1;
            var vazoes = BottonComments.Substring(i).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            return int.Parse(vazoes[6].Substring(39).Trim());

        }
        set
        {
            var i = BottonComments.IndexOf("\n& VAZOES") + 1;
            var vazoes = BottonComments.Substring(i).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            vazoes[6] = vazoes[6].Remove(39).Insert(39, value.ToString("0000"));
            vazoes[4] = vazoes[4].Remove(44, 4).Insert(44, value.ToString("0000"));

            BottonComments = BottonComments.Remove(i).Insert(i,
                String.Join(Environment.NewLine, vazoes)
                );
        }
    }
    public int VAZOES_NumeroDeSemanas
    {
        get
        {

            var i = BottonComments.IndexOf("\n& VAZOES") + 1;
            var vazoes = BottonComments.Substring(i).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            return int.Parse(vazoes[7].PadRight(48).Substring(39, 4).Trim());

        }
        set
        {
            var i = BottonComments.IndexOf("\n& VAZOES") + 1;
            var vazoes = BottonComments.Substring(i).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            vazoes[7] = vazoes[7].PadRight(48).Remove(39, 4).Insert(39, value.ToString("0000"));

            BottonComments = BottonComments.Remove(i).Insert(i,
                String.Join(Environment.NewLine, vazoes)
                );
        }
    }
    public int VAZOES_NumeroDeSemanasPassadas
    {
        get
        {

            var i = BottonComments.IndexOf("\n& VAZOES") + 1;
            var vazoes = BottonComments.Substring(i).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            return int.Parse(vazoes[7].PadRight(48).Substring(44, 4).Trim());

        }
        set
        {
            var i = BottonComments.IndexOf("\n& VAZOES") + 1;
            var vazoes = BottonComments.Substring(i).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            vazoes[7] = vazoes[7].PadRight(48).Remove(44, 4).Insert(44, value.ToString("0000"));

            BottonComments = BottonComments.Remove(i).Insert(i,
                String.Join(Environment.NewLine, vazoes)
                );
        }
    }
    public int VAZOES_NumeroDiasDoMes2
    {
        get
        {

            var i = BottonComments.IndexOf("\n& VAZOES") + 1;
            var vazoes = BottonComments.Substring(i).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            return int.Parse(vazoes[8].Substring(39).Trim());

        }
        set
        {
            var i = BottonComments.IndexOf("\n& VAZOES") + 1;
            var vazoes = BottonComments.Substring(i).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            vazoes[8] = vazoes[8].Remove(39).Insert(39, value.ToString());

            BottonComments = BottonComments.Remove(i).Insert(i,
                String.Join(Environment.NewLine, vazoes)
                );
        }
    }
    public int VAZOES_EstruturaDaArvore
    {
        get
        {

            var i = BottonComments.IndexOf("\n& VAZOES") + 1;
            var vazoes = BottonComments.Substring(i).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            return int.Parse(vazoes[9].Substring(39).Trim());

        }
        set
        {
            var i = BottonComments.IndexOf("\n& VAZOES") + 1;
            var vazoes = BottonComments.Substring(i).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            vazoes[9] = vazoes[9].Remove(39).Insert(39, value.ToString());

            BottonComments = BottonComments.Remove(i).Insert(i,
                String.Join(Environment.NewLine, vazoes)
                );
        }
    }


}