using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace keyword_search
{
    class Program
    {
        static string[] InputProductData;
        static List<Caldata> CalculateData = new List<Caldata>();
        static string[] KeyPattern;
        static Bm BoyerOperation = new Bm();

        static void Main(string[] args)
        {
            try
            {
                readData();

                readKeyPattern();

                TextMatching();

                SortingData();

                ShowData();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error :"+ex.Message+Environment.NewLine+" Data:"+ex.Data);
            }
            Console.ReadKey();
        }

        static void readData()
        {
            string path = @"product.txt";

            if(File.Exists(path))
            {
                InputProductData = File.ReadAllLines(path);
            }
            else 
            {
                Console.WriteLine("Error: There is no product data.");
            }
        }

        static void readKeyPattern()
        {
            char[] delimiter = { ' ' };

            Console.WriteLine("----Text Matching----");
            Console.Write("Input Keyword :");
            string input = Console.ReadLine();

            KeyPattern = input.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
        }

        static void TextMatching()
        {
            if (InputProductData.Length != 0 && KeyPattern.Length != 0)
            {

                for (int i = 0; i < InputProductData.Length; i++)
                {
                    List<int> bmcaldata = new List<int>();
                    for (int j = 0; j < KeyPattern.Length; j++)
                    {
                        int[] calbm = BoyerOperation.BM_Matcher(InputProductData[i].ToLower(), KeyPattern[j].ToLower());
                        if (calbm.Length > 0)
                        {
                            bmcaldata.Add(calbm[0]);
                        }
                    }
                    if(bmcaldata.Count >0)
                        CalculateBMData(bmcaldata, i);
                }
            }
        }

        static void CalculateBMData(List<int> bmCal,int i)
        {
            Caldata x = new Caldata();
            x.id = i;
            x.MatchedNo = bmCal.Count;
            x.FirstMatch = bmCal[0];
            int matchdata = x.FirstMatch;
            if (bmCal.Count > 1)
            {
                for (int j = 1; j < bmCal.Count; j++)
                {
                    matchdata = matchdata - bmCal[j];
                }
                x.Distance = Math.Abs(matchdata);
            }

            CalculateData.Add(x);
        }

        static void SortingData()
        {
            List<Caldata> TempList = new List<Caldata>();
            List<int> SortMatchedNo = CalculateData.Select(i=>i.MatchedNo).Distinct().ToList();
            for (int i = 0; i < SortMatchedNo.Count; i++)
            {
                List<Caldata> stSort = CalculateData.Where(a => a.MatchedNo == SortMatchedNo[i]).ToList();
                List<int> ndMatch = stSort.Select(b => b.FirstMatch).Distinct().ToList();
                for (int j = 0; j < ndMatch.Count; j++)
                {
                    List<Caldata> ndSort = stSort.Where(c => c.FirstMatch == ndMatch[j]).ToList();
                    ndSort = ndSort.OrderBy(d => d.Distance).ToList();
                    TempList.AddRange(ndSort);
                }

            }

            CalculateData = TempList;

        }

        static void ShowData()
        {
            Console.WriteLine("Search Result is:" + Environment.NewLine);

            foreach (Caldata item in CalculateData)
            {
                Console.WriteLine("- " + InputProductData[item.id]);
            }

            Console.WriteLine(Environment.NewLine+CalculateData.Count+" product(s) matched");
        }
    }

    public class Caldata
    {
        public int id { get; set; }
        public int MatchedNo { get; set; }
        public int Distance { get; set; }
        public int FirstMatch { get; set; }

        public Caldata()
        {
            Distance = int.MaxValue;
        }
    }
}
