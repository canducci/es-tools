using Estools.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Estools.Library.Dadger;

namespace Estools
{
    public interface IHidro
    {
        string DocPath { get; set; }
        double[] Earm { get; set; }
        double[] MetaReservatorio { get; set; }
        double[] EarmMax { get; set; }
        IEnumerable<Hidro.Restricao> Restricoes { get; set; }
        IEnumerable<Hidro.Alteracao> Alteracoes { get; set; }
        DateTime Data { get; set; }
    }

    public class Hidro
    {
        public class Restricao
        {
            public int Id { get; set; }
            public double VolMin { get; set; }
            public double VolMax { get; set; }
            public bool Relative { get; set; }
            public Restricao(int id, double volMin, double volMax, bool relative = false)
            {
                Id = id; VolMin = volMin; VolMax = volMax; Relative = relative;
            }

        }

        public class Alteracao
        {
            public int Id { get; set; }
            public string Tipo { get; set; }
            public double Valor { get; set; }
        }

        public static double[] GetEarm(ConfigH configH, bool ree = false)
        {
            Func<double[]> getEarm;
            Func<double[]> getEarmMax;

            if (ree)
            {
                getEarm = configH.GetEarmsRee;
                getEarmMax = configH.GetEarmsMaxRee;
            }
            else
            {
                getEarm = configH.GetEarms;
                getEarmMax = configH.GetEarmsMax;
            }

            var earm = getEarm();
            var earmMax = getEarmMax();

            var result = new double[earm.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Math.Round((earm[i] / earmMax[i]) * 100, 2);
            }

            return result;
        }

        public static IEnumerable<Alteracao> LerAlteracoes(string modifNwPath, DateTime? modifDate = null)
        {
            throw new NotImplementedException();
            //var modifNw = DocumentFactory.Create(modifNwPath) as ModifDatNw;
            ModifDatNw modifNw = default!;
            var inInfo = System.Globalization.NumberFormatInfo.InvariantInfo;
            var result = new List<Alteracao>();

            //canaldefuga
            foreach (var altCfuga in modifNw
                .Where(ac => ac.Chave == "CFUGA")
                .GroupBy(ac => ac.Usina))
            {
                ModifLine alt;
                if (!modifDate.HasValue)
                {
                    alt = altCfuga.First();
                }
                else
                {
                    alt = altCfuga.Last(
                        x => x.DataModif <= modifDate
                        );
                }

                result.Add(
                new Alteracao
                {
                    Id = altCfuga.Key,
                    Tipo = "CFUGA",
                    Valor = float.Parse(alt.NovosValores[2], inInfo)
                });
            }

            return result;
        }

        public static void Reservatorio(IHidro info, ConfigH configH)
        {
            Func<double[]> getEarm;
            Func<double[]> getEarmMax;
            Action<ConfigH, double[], double[]> setUHBlock;

            if (info.MetaReservatorio.Length == 4)
            {
                getEarm = configH.GetEarms;
                getEarmMax = configH.GetEarmsMax;
                setUHBlock = SetUHBlock;
            }
            else
            {
                getEarm = configH.GetEarmsRee;
                getEarmMax = configH.GetEarmsMaxRee;
                setUHBlock = SetUHBlock;
            }

            if (info.Restricoes.Count() > 0)
            {
                configH.CarregarRestricoes(info.Restricoes.Select(x => new Tuple<int, double, double, bool>(x.Id, x.VolMin, x.VolMax, x.Relative)));
            }

            if (info.Alteracoes.Count() > 0)
            {
                foreach (var altcad in info.Alteracoes)
                {
                    switch (altcad.Tipo)
                    {
                        case "CFUGA":
                            configH.usinas[altcad.Id].CanalFugaMedChanged = true;
                            configH.usinas[altcad.Id].CanalFugaMed = altcad.Valor;
                            break;
                        default:
                            break;
                    }
                }
            }

            double[] earmAtual = getEarm();
            //info.Earm = earmAtual;
            double[] earmMeta = info.MetaReservatorio;


            //Se meta for relativa (%) e max não informado, calcular;
            var earmMax = new double[0];
            if (earmMeta.All(x => x <= 100))
            {
                earmMax = getEarmMax();
                // info.EarmMax = earmMax;

                configH.ReloadUH();
            }

            //atualizar UH
            SetUHBlock(configH, earmMeta, earmMax);
            
        }

        


        public static void Reservatorio(IHidro info, ConfigH configH, Dictionary<int, double> reserv)
        {
            configH.SetUH(reserv);
            //configH.baseDoc.SaveToFile(createBackup: true);
        }

        public static IEnumerable<Restricao> LerRestricoes(ConfigH configH, Dadger dadgerRef)
        {
            var curva = new List<Restricao>();

            if (dadgerRef != null)
            {
                //manter restricoes de volume para restringir variacao no atingir meta de armazenamento
                curva.AddRange(dadgerRef.BlocoRhv.RhvGrouped
                    .Where(x => x.Value.Any(y => (y is CvLine) && y[5].Equals("VARM")))
                    .Select(x => new Restricao(
                        x.Value.First(y => (y is CvLine))[3],
                        (x.Value.Any(y => (y is LvLine) && y[2] == 1 && (y[3] is double)) ? x.Value.First(y => (y is LvLine) && y[2] == 1 && (y[3] is double))[3] : 0) / configH.usinas[x.Value.First(y => (y is CvLine))[3]].VolUtil * 100,
                        (x.Value.Any(y => (y is LvLine) && y[2] == 1 && (y[4] is double)) ? x.Value.First(y => (y is LvLine) && y[2] == 1 && (y[4] is double))[4] : configH.usinas[x.Value.First(y => (y is CvLine))[3]].VolUtil) / configH.usinas[x.Value.First(y => (y is CvLine))[3]].VolUtil * 100,
                        true
                    )).ToList());

                curva.AddRange(dadgerRef.BlocoVe.Select(x => new Restricao(x[1], 0,
                    (x[2]), //* configH.usinas[x[1]].VolUtil,
                    true
                    )).ToList());
            }

            return curva.GroupBy(x => x.Id).Select(x => new Restricao(x.Key, x.Min(y => y.VolMin), x.Max(y => y.VolMax), true)).OrderBy(x => x.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configH"></param>
        /// <param name="earmTargetLevel"></param>
        /// <param name="earmMax">Desconsiderado caso a meta seja em valor absoluto</param>
        /// <returns></returns>
        static void SetUHBlock(ConfigH configH, double[] earmTargetLevel, double[] earmMax)
        {
            var earmTarget = new double[earmTargetLevel.Length];

            var itNum = 0;
            if (earmTarget.Length == 4)
            {
                itNum = configH.index_sistemas.Count;
            }
            else
            {
                itNum = ConfigH.uhe_ree.Values.Distinct().Count();
            }


            if (earmTargetLevel.All(x => x <= 1))
            {
                for (int x = 0; x < itNum; x++)
                {
                    earmTarget[x] = earmTargetLevel[x] * earmMax[x];
                }
            }
            else if (earmTargetLevel.All(x => x <= 100))
            {
                for (int x = 0; x < itNum; x++)
                {
                    earmTarget[x] = (earmTargetLevel[x] / 100f) * earmMax[x];
                }

            }
            else
            {
                earmTarget = earmTargetLevel;
            }

            buildReserv(configH, earmTarget);
        }

        static void buildReserv(ConfigH configH, double[] earmTarget)
        {
            if (earmTarget.Length == 4)
            {
                goalSeek(configH, earmTarget);
            }
            else
            {
                goalSeekRee(configH, earmTarget);
            }

            if (configH.baseDoc is Dadger)
            {
                foreach (var uhBase in ((Dadger)configH.baseDoc).BlocoUh)
                {

                    //var newUh = (UhLine)uhBase.Clone();
                    var uhe = configH.usinas[uhBase.Usina];
                    uhBase.VolIniPerc = uhe.VolIni > 0 && uhe.VolUtil > 0 ? (float)Math.Round((uhe.VolIni / uhe.VolUtil) * 100f, 2) : 0f;
                    //uhResult.Add(newUh);
                }
            }
            else
            {
                foreach (var uh in (ConfhdDat)configH.baseDoc)
                {
                    var uhe = configH.usinas[uh.Cod];
                    uh.VolUtil = uhe.VolIni > 0 && uhe.VolUtil > 0 ? (float)Math.Round((uhe.VolIni / uhe.VolUtil) * 100f, 2) : 0f;

                    if (configH.usinas[uh.Cod].IsFict && configH.usinas[uh.Cod].Ez < 1)
                    {
                        var ficVol = uh.VolUtil / configH.usinas[uh.Cod].Ez;


                        uh.VolUtil = Math.Min(ficVol, 100);
                    }

                }
            }
        }

        static void goalSeek(ConfigH configH, double[] earmTarget)
        {

            var fatores = new double[configH.index_sistemas.Max(t => t.Item2) + 1];
            for (int i = 0; i < fatores.Length; i++) fatores[i] = 1;

            double erro = 100;
            double erroAnterior = 0;
            int itNumber = 0;

            do
            {

                var earmCurrent = configH.GetEarms();

                erroAnterior = erro;
                erro = 0;

                for (int x = 0; x < configH.index_sistemas.Count; x++)
                {

                    var sis = configH.index_sistemas[x].Item2;

                    erro = erro + Math.Abs(earmCurrent[x] - earmTarget[x]);
                    var f = (earmTarget[x] / earmCurrent[x]);
                    fatores[sis] = f;
                }

                //se erro pequeno ou não houver grande variação parar iteração
                if (erro < 2 || Math.Abs(erroAnterior - erro) < 1)
                    break;


                //atualiza volumes e queda
                foreach (var uhe in configH.Usinas.Where(u => !u.IsFict && u.VolIni > 0))
                {

                    if (!uhe.CodFicticia.HasValue)
                    {
                        uhe.VolIni *= fatores[uhe.Mercado];
                    }
                    else
                    {
                        // se influenciar em outro sistema, levar em conta o fator do sistema afetado
                        // f = ( fs^3 * ff ) ^ (1/4)
                        var f = (float)Math.Pow(fatores[uhe.Mercado] *
                            fatores[uhe.Mercado] *
                            fatores[uhe.Mercado] *
                            fatores[configH.usinas[uhe.CodFicticia.Value].Mercado],
                            1d / 4d);
                        uhe.VolIni *= f;
                        //configH.usinas[uhe.CodFicticia.Value].atualizaQueda();
                    }
                }

            } while (++itNumber < 100);
        }

        static void goalSeekRee(ConfigH configH, double[] earmTarget)
        {

            var rees = ConfigH.ree_list.Count();

            var fatores = new double[rees];
            for (int i = 0; i < fatores.Length; i++) fatores[i] = 1;

            double erro = 100;
            double erroAnterior = 0;
            int itNumber = 0;

            do
            {

                var earmCurrent = configH.GetEarmsRee();

                erroAnterior = erro;
                erro = 0;

                for (int x = 0; x < rees; x++)
                {
                    if (earmTarget[x] == 0)
                    {
                        fatores[x] = 1;
                    }
                    else
                    {
                        erro = erro + Math.Abs(earmCurrent[x] - earmTarget[x]);
                        var f = (earmTarget[x] / earmCurrent[x]);
                        fatores[x] = f;
                    }
                }

                //se erro pequeno ou não houver grande variação parar iteração
                if (erro < 2 || Math.Abs(erroAnterior - erro) < 1)
                    break;


                //atualiza volumes e queda
                foreach (var uhe in configH.Usinas.Where(u => !u.IsFict && u.VolIni > 0 && u.InDadger))
                {

                    //if (!uhe.CodFicticia.HasValue)
                    //{
                    uhe.VolIni *= fatores[ConfigH.ree_list.IndexOf(uhe.Ree)];
                    //}
                    //else
                    //{
                    //    // se influenciar em outro sistema, levar em conta o fator do sistema afetado
                    //    // f = ( fs^3 * ff ) ^ (1/4)
                    //    var uJus = configH.Usinas.Where(x => x.Cod == uhe.Jusante.Value && !x.IsFict).FirstOrDefault();                        

                    //    var f = (float)Math.Pow(fatores[ConfigH.ree_list.IndexOf(uhe.Ree)] *
                    //        fatores[ConfigH.ree_list.IndexOf(uhe.Ree)] *
                    //        fatores[ConfigH.ree_list.IndexOf(uhe.Ree)] *
                    //        fatores[ConfigH.ree_list.IndexOf(uJus.Ree)],
                    //        1d / 4d);
                    //    uhe.VolIni *= f;
                    //    //configH.usinas[uhe.CodFicticia.Value].atualizaQueda();
                    //}
                }

            } while (++itNumber < 100);
        }

    }
}

